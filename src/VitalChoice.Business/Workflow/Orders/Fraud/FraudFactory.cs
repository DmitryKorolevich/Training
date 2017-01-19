using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using VitalChoice.Ecommerce.Domain.Helpers;

namespace VitalChoice.Business.Workflow.Orders.Fraud
{
    public class FraudFactory
    {
        private static readonly Dictionary<string, Checker> FraudCheckers = new Dictionary<string, Checker>();

        static FraudFactory()
        {
            var suspects =
                typeof(FraudFactory).Assembly.GetTypes()
                    .Where(t => !t.GetTypeInfo().IsAbstract && t.IsImplementGeneric(typeof(BaseFraudChecker<>)));
            foreach (var suspect in suspects)
            {
                var typeInfo = suspect.GetTypeInfo();
                var nameAttribute = typeInfo.GetCustomAttribute<FraudFieldNameAttribute>();
                var priorityAttribute = typeInfo.GetCustomAttribute<FraudPriorityAttribute>();
                if (!string.IsNullOrWhiteSpace(nameAttribute?.Name))
                {
                    FraudCheckers.Add(nameAttribute.Name,
                        new Checker((IFraudChecker) Activator.CreateInstance(suspect), priorityAttribute?.Priority ?? 0));
                }
            }
        }

        public static IFraudChecker GetChecker(string fieldName, out int priority)
        {
            var checker = FraudCheckers.GetValueOrDefault(fieldName);
            priority = checker.Priority;
            return checker.FraudChecker;
        }

        private struct Checker
        {
            public Checker(IFraudChecker fraudChecker, int priority)
            {
                FraudChecker = fraudChecker;
                Priority = priority;
            }

            public readonly IFraudChecker FraudChecker;
            public readonly int Priority;
        }
    }
}