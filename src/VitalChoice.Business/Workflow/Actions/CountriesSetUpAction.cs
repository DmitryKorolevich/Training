using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using VitalChoice.Interfaces.Services.Settings;
using VitalChoice.Workflow.Base;
using VitalChoice.Workflow.Contexts;
using VitalChoice.Workflow.Core;

namespace VitalChoice.Business.Workflow.Actions
{
    public class CountriesSetUpAction: ComputableAction<OrderDataContext>
    {
        public CountriesSetUpAction(ComputableTree<OrderDataContext> tree, string actionName) : base(tree, actionName)
        {
        }

        public override async Task<decimal> ExecuteAction(OrderDataContext context, IWorkflowExecutionContext executionContext)
        {
            var countryService = executionContext.Resolve<ICountryService>();
            var countries = await countryService.GetCountriesAsync();
            context.Coutries = countries.ToDictionary(c => c.CountryCode, c => c.Id, StringComparer.OrdinalIgnoreCase);
            context.States = countries.ToDictionary(c => c.CountryCode,
                c => c.States.ToDictionary(s => s.StateCode, s => s.Id, StringComparer.OrdinalIgnoreCase),
                StringComparer.OrdinalIgnoreCase);
            context.CoutryCodes = countries.ToDictionary(c => c.Id);
            context.StateCodes = countries.ToDictionary(c => c.Id, c => c.States.ToDictionary(s => s.Id));
            return 0;
        }
    }
}
