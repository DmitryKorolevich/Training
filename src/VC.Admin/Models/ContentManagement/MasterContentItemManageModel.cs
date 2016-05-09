using System;
using System.Linq;
using System.Collections.Generic;
using VC.Admin.Validators.ContentManagement;
using VitalChoice.Validation.Models;
using VitalChoice.Ecommerce.Domain.Entities;
using VitalChoice.Infrastructure.Domain.Content.Base;
using VitalChoice.Infrastructure.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;

namespace VC.Admin.Models.ContentManagement
{
    public class TemplateValidationModel
    {
        public string Template { get; set; }
    }

    [ApiValidator(typeof(MasterContentItemManageModelValidator))]
    public class MasterContentItemManageModel : BaseModel
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        public string Template { get; set; }

        public ContentType Type { get; set; }

        public bool IsDefault { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public MasterContentItemManageModel()
        {
        }

        public MasterContentItemManageModel(MasterContentItem item)
        {
            if (item != null)
            {
                Id = item.Id;
                Name = item.Name;
                Template = item.Template;
                Type = (ContentType)item.TypeId;
                if(item.Type.DefaultMasterContentItemId==Id)
                {
                    IsDefault = true;
                }
                Created = item.Created;
                Updated = item.Updated;
                StatusCode = item.StatusCode;
                if (item.MasterContentItemToContentProcessors!=null)
                {
                    ProcessorIds = item.MasterContentItemToContentProcessors.Select(p => p.ContentProcessorId).ToList();
                }
                else
                {
                    ProcessorIds = new List<int>();
                }
            }
        }

        public MasterContentItem Convert()
        {
            MasterContentItem toReturn = new MasterContentItem();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Template = Template;
            toReturn.TypeId = (int)Type;
            toReturn.Type = new ContentTypeEntity();
            toReturn.Type.Id = toReturn.TypeId;
            if(IsDefault)
            {
                toReturn.Type.DefaultMasterContentItemId = toReturn.Id;
            }
            if(ProcessorIds!=null)
            {
                toReturn.MasterContentItemToContentProcessors = ProcessorIds.Select(p => new MasterContentItemToContentProcessor()
                {
                    ContentProcessorId=p,
                    MasterContentItemId= toReturn.Id,
                }).ToList();
            }

            return toReturn;
        }
    }
}