using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Ecommerce.Domain.Helpers;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Components
{
    [ViewComponent(Name = "Area")]
    public class AreaViewComponent : ViewComponent
    {
        private static readonly Regex TemplateReplaceExpression = new Regex("\\{\\{([a-z_@][a-z0-9_\\.]*)\\}\\}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly IContentAreaService _contentAreaService;

        public AreaViewComponent(IContentAreaService contentAreaService)
        {
            _contentAreaService = contentAreaService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name, Dictionary<string, string> data = null)
        {
            var component = await _contentAreaService.GetContentAreaByNameAsync(name);

            if (component == null)
            {
                throw new NotImplementedException($"Area component with name {name} is not implemented");
            }

            if (data != null)
            {
                component.Template = TemplateReplaceExpression.Replace(component.Template, match =>
                {
                    var matchValue = match.Groups[1].Value;
                    if (!string.IsNullOrEmpty(matchValue))
                    {
                        return data.GetValueOrDefault(matchValue) ?? string.Empty;
                    }
                    return string.Empty;
                });

                //foreach (var item in data)
                //{
                //    component.Template = component.Template.Replace($"{{{item.Key}}}", item.Value);
                //}
            }
			
			return View("~/Views/Shared/Components/AreaComponent.cshtml", component.Template);
    }
}
}