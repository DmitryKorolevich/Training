using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VC.Admin.Models.OrderNote;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Ecommerce.Domain.Entities.Orders;
using VitalChoice.Ecommerce.Domain.Exceptions;
using VitalChoice.Ecommerce.Domain.Transfer;
using VitalChoice.Infrastructure.Domain.Constants;
using VitalChoice.Infrastructure.Domain.Transfer;
using VitalChoice.Infrastructure.Identity.UserManagers;
using VitalChoice.Interfaces.Services.Orders;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[SuperAdminAuthorize]
    public class OrderNoteController : BaseApiController
    {
		private readonly IOrderNoteService _orderNoteService;
	    private readonly ExtendedUserManager _userManager;


	    public OrderNoteController(IOrderNoteService orderNoteService, ExtendedUserManager userManager)
	    {
	        _orderNoteService = orderNoteService;
	        _userManager = userManager;
	    }

	    [HttpPost]
	    public async Task<Result<PagedList<OrderNoteListItemModel>>> GetOrderNotes([FromBody]FilterBase filter)
		{
			var result =  await _orderNoteService.GetOrderNotesAsync(filter);
		    return new PagedList<OrderNoteListItemModel>()
		    {
				Count = result.Count,
				Items = result.Items.Select(x=> new OrderNoteListItemModel()
				{
					Id = x.Id,
					Title = x.Title,
					Description = x.Description,
					CustomerTypes = x.CustomerTypes?.Select(y => y.IdCustomerType).ToList(),
					DateEdited = x.DateEdited,
					EditedBy = x.AdminProfile?.AgentId
				}).ToList()
		    };
		}

		[HttpPost]
		public Result<ManageOrderNoteModel> CreateOrderNotePrototype()
		{
			return new ManageOrderNoteModel();
		}

		[HttpPost]
		public async Task<Result<bool>> CreateOrderNote([FromBody]ManageOrderNoteModel orderNoteModel)
		{
			if (!Validate(orderNoteModel))
				return false;

			var orderNote = new OrderNote()
			{
				Title = orderNoteModel.Title,
				Description = orderNoteModel.Description,
				CustomerTypes = orderNoteModel.CustomerTypes?.Select(x=> new OrderNoteToCustomerType()
				{
					 IdCustomerType = x
				}).ToList(),
			};

			await _orderNoteService.AddOrderNoteAsync(orderNote, Convert.ToInt32(_userManager.GetUserId(User)));

			return true;
		}

		[HttpPost]
		public async Task<Result<bool>> UpdateOrderNote([FromBody]ManageOrderNoteModel orderNoteModel)
		{
			if (!Validate(orderNoteModel))
				return false;

			var orderNote = await _orderNoteService.GetAsync(orderNoteModel.Id);
			if (orderNote == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}

			orderNote.Title = orderNoteModel.Title;
			orderNote.Description = orderNoteModel.Description;
			orderNote.CustomerTypes = orderNoteModel.CustomerTypes?.Select(x => new OrderNoteToCustomerType()
			{
				IdCustomerType = x
			}).ToList();

			await _orderNoteService.UpdateOrderNoteAsync(orderNote, Convert.ToInt32(_userManager.GetUserId(User)));

			return true;
		}

		[HttpGet]
	    public async Task<Result<ManageOrderNoteModel>> GetOrderNote(int id)
		{
			var orderNote = await _orderNoteService.GetAsync(id);
			if (orderNote == null)
			{
				throw new AppValidationException(ErrorMessagesLibrary.Data[ErrorMessagesLibrary.Keys.CantFindRecord]);
			}
			
			return new ManageOrderNoteModel()
			{
				Title = orderNote.Title,
				Description = orderNote.Description,
				CustomerTypes = orderNote.CustomerTypes?.Select(y => y.IdCustomerType).ToList(),
				Id = orderNote.Id
			};
		}

		[HttpGet]
	    public async Task<Result<bool>> DeleteOrderNote(int id)
	    {
			await _orderNoteService.DeleteOrderNoteAsync(id, Convert.ToInt32(_userManager.GetUserId(User)));

			return true;
	    }
    }
}