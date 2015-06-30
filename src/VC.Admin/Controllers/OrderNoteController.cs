using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using VC.Admin.Models.OrderNote;
using VitalChoice.Core.Base;
using VitalChoice.Core.Infrastructure;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Orders;
using VitalChoice.Domain.Entities.Permissions;
using VitalChoice.Domain.Exceptions;
using VitalChoice.Domain.Transfer.Base;
using VitalChoice.Interfaces.Services.Order;
using VitalChoice.Validation.Models;

namespace VC.Admin.Controllers
{
	[AdminAuthorize(PermissionType.Settings)]
    public class OrderNoteController : BaseApiController
    {
		private readonly IOrderNoteService _orderNoteService;


		public OrderNoteController(IOrderNoteService orderNoteService)
		{
			_orderNoteService = orderNoteService;
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

			await _orderNoteService.AddOrderNoteAsync(orderNote);

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

			await _orderNoteService.UpdateOrderNoteAsync(orderNote);

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
			await _orderNoteService.DeleteOrderNoteAsync(id);

			return true;
	    }
    }
}