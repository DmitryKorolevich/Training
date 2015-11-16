using System;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Validation.Models;

namespace VC.Admin.Models.ContentManagement
{
    public class MasterContentItemListItemModel : BaseModel
	{
	    public int Id { get; set; }

	    public string Name { get; set; }

	    public ContentType Type { get; set; }

        public string TypeName { get; set; }

        public bool IsDefault { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public string AgentId { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public MasterContentItemListItemModel(MasterContentItem item)
        {
            if(item!=null)
            {
                Id = item.Id;
                Name = item.Name;
                Type = (ContentType)item.TypeId;
                if(item.Type!=null)
                {
                    TypeName = item.Type.Name;
                    if (item.Type.DefaultMasterContentItemId == item.Id)
                    {
                        IsDefault = true;
                    }
                }
                Created = item.Created;
                Updated = item.Updated;
                StatusCode = item.StatusCode;
                if(item.User!=null && item.User.Profile!=null)
                {
                    AgentId = item.User.Profile.AgentId;
                }
            }
        }
    }
}