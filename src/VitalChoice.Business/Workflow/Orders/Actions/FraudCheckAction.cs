﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Business.Workflow.Orders.Fraud;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Dynamic;
using VitalChoice.Infrastructure.Domain.Transfer.Contexts;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Orders.Actions
{
    public class FraudCheckAction : ComputableAction<OrderDataContext>
    {
        public FraudCheckAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteActionAsync(OrderDataContext context, ITreeContext executionContext)
        {
            //bypass fraud checks
            if (!context.CheckForFraud)
            {
                return 0;
            }
            var ruleService = executionContext.Resolve<IOrderReviewRuleService>();
            var rules = (await ruleService.GetAllRules()).Where(r => r.StatusCode == (int) RecordStatusCode.Active);
            bool isFraud = false;
            foreach (var rule in rules)
            {
                List<string> reasonList = new List<string>();
                bool ruleFraud;
                List<RuleData> checkers;
                switch (rule.ApplyType)
                {
                    case ApplyType.All:
                        ruleFraud = true;
                        checkers = GetCheckers(rule);
                        bool anyChecksPerformed = false;
                        foreach (var checker in checkers.OrderBy(c => c.Priority))
                        {
                            //check if we skipped at least one condition, then go to next global rule with success
                            if (checker.Checker.ShouldCheck(context, executionContext, checker.Data, rule))
                            {
                                anyChecksPerformed = true;
                                var checkResult = await checker.Checker.CheckCondition(context, executionContext, checker.Data, rule);
                                if (!checkResult)
                                {
                                    ruleFraud = false;
                                    break;
                                }
                                reasonList.Add(checkResult.Reason);
                            }
                        }
                        if (anyChecksPerformed && ruleFraud)
                        {
                            isFraud = true;
                        }
                        break;
                    case ApplyType.Any:
                        ruleFraud = false;
                        checkers = GetCheckers(rule);
                        foreach (var checker in checkers.OrderBy(c => c.Priority))
                        {
                            //check if we spot at least one condition to succeed, then go to global exit
                            if (checker.Checker.ShouldCheck(context, executionContext, checker.Data, rule))
                            {
                                var checkResult = await checker.Checker.CheckCondition(context, executionContext, checker.Data, rule);
                                if (checkResult)
                                {
                                    reasonList.Add(checkResult.Reason);
                                    ruleFraud = true;
                                    break;
                                }
                            }
                        }
                        if (ruleFraud)
                        {
                            isFraud = true;
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                if (isFraud)
                {
                    context.IsFraud = true;
                    context.FraudReason = reasonList;
                    return 0;
                }
            }
            return 0;
        }

        private static List<RuleData> GetCheckers(OrderReviewRuleDynamic rule)
        {
            var checkers = new List<RuleData>();
            foreach (var pair in rule.DictionaryData)
            {
                if (pair.Value != null)
                {
                    int priority;
                    var checker = FraudFactory.GetChecker(pair.Key, out priority);
                    if (checker != null)
                    {
                        checkers.Add(new RuleData(checker, priority, pair.Value));
                    }
                }
            }
            return checkers;
        }

        private struct RuleData
        {
            public readonly IFraudChecker Checker;
            public readonly int Priority;
            public readonly object Data;

            public RuleData(IFraudChecker checker, int priority, object data)
            {
                Checker = checker;
                Priority = priority;
                Data = data;
            }
        }
    }
}