using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VitalChoice.Interfaces.Services.Content;

namespace VC.Public.Components
{
	[ViewComponent(Name = "CustomStyles")]
	public class CustomStylesViewComponent : ViewComponent
	{
		private readonly IStylesService _stylesService;

		public CustomStylesViewComponent(IStylesService stylesService)
		{
			_stylesService = stylesService;
		}
		
		public async Task<IViewComponentResult> InvokeAsync()
		{
			var component = await _stylesService.GetStyles();

			if (component == null)
			{
				throw new NotImplementedException("Custom Styles component not implemented");
			}
			
			return View("~/Views/Shared/Components/AreaComponent.cshtml", component.Styles);
		}
	}
}