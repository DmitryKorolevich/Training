using System;
using System.Linq;
using System.Collections.Generic;
using VitalChoice.Domain;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.Validators.UserManagement;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validators.ContentManagement;

namespace VitalChoice.Models.ContentManagement
{
    [ApiValidator(typeof(RecipeManageModelValidator))]
    public class RecipeManageModel : Model<Recipe, IMode>
    {
        public int Id { get; set; }

        [Localized(GeneralFieldNames.Name)]
        public string Name { get; set; }

        [Localized(GeneralFieldNames.Name)]
        public string Url { get; set; }

        public string Template { get; set; }

        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public DateTime Created { get; set; }

        public DateTime Updated { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public IEnumerable<int> ProcessorIds { get; set; }

        public RecipeManageModel(Recipe item)
        {
            if (item != null)
            {
                Id = item.Id;
                Name = item.Name;
                Url = item.Url;
                StatusCode = item.StatusCode;
                Template = item.ContentItem.Template;
                Title = item.ContentItem.Title;
                MetaKeywords = item.ContentItem.MetaKeywords;
                MetaDescription = item.ContentItem.MetaDescription;
                Created = item.ContentItem.Created;
                Updated = item.ContentItem.Updated;
                if (item.ContentItem.ContentItemToContentProcessors!= null)
                {
                    ProcessorIds = item.ContentItem.ContentItemToContentProcessors.Select(p => p.ContentProcessorId).ToList();
                }
                else
                {
                    ProcessorIds = new List<int>();
                }
            }
        }

        public override Recipe Convert()
        {
            Recipe toReturn = new Recipe();
            toReturn.Id = Id;
            toReturn.Name = Name?.Trim();
            toReturn.Url = Url?.Trim();
            toReturn.Url = toReturn.Url?.ToLower();
            toReturn.ContentItem = new ContentItem();
            toReturn.ContentItem.Template = Template;
            toReturn.ContentItem.Title = Title;
            toReturn.ContentItem.MetaKeywords = MetaKeywords;
            toReturn.ContentItem.MetaDescription = MetaDescription;
            if(ProcessorIds!=null)
            {
                toReturn.ContentItem.ContentItemToContentProcessors = ProcessorIds.Select(p => new ContentItemToContentProcessor()
                {
                    ContentProcessorId=p,
                }).ToList();
            }

            return toReturn;
        }
    }
}