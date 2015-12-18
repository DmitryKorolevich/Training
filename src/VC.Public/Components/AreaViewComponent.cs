using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Components
{
    [ViewComponent(Name = "Area")]
    public class AreaViewComponent : ViewComponent
    {
        private readonly IContentAreaService _contentAreaService;

        public AreaViewComponent(IContentAreaService contentAreaService)
        {
            _contentAreaService = contentAreaService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            var component = await _contentAreaService.GetContentAreaByNameAsync(name);

            if (component == null)
            {
                throw new NotImplementedException($"Area component with name {name} not implemented");
            }

            return View("~/Views/Shared/Components/AreaComponent.cshtml", component.Template);
        }

        public async Task<IViewComponentResult> InvokeAsync(string name, Dictionary<string, string> data = null)
        {
            var component = await _contentAreaService.GetContentAreaByNameAsync(name);

            if (component == null)
            {
                throw new NotImplementedException($"Area component with name {name} not implemented");
            }

            if (data != null)
            {
                foreach (var item in data)
                {
                    component.Template = component.Template.Replace($"{{{item.Key}}}", item.Value);
                }
            }
			
			return View("~/Views/Shared/Components/AreaComponent.cshtml", component.Template);
    }
}
}