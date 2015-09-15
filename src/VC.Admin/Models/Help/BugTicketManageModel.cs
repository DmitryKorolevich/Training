using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
using VitalChoice.DynamicData.Attributes;
using VitalChoice.DynamicData;
using VitalChoice.Domain.Constants;
using VitalChoice.Domain.Entities.eCommerce.Products;
using VitalChoice.Domain.Entities.Localization.Groups;
using VitalChoice.DynamicData.Entities;
using VitalChoice.Domain.Entities.eCommerce.Discounts;
using VitalChoice.DynamicData.Interfaces;
using VC.Admin.Validators.Affiliate;
using VitalChoice.Domain.Entities.eCommerce.Help;
using VitalChoice.Domain.Entities.Help;

namespace VC.Admin.Models.Help
{
    [ApiValidator(typeof(BugTicketManageModelValidator))]
    public class BugTicketManageModel : BaseModel
    {
        public int Id { get; set; }

        public int IdAddedBy { get; set; }

        public string AddedBy { get; set; }

        public string AddedByAgent { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public TicketPriority Priority { get; set; }

        public string Summary { get; set; }

        public string Description { get; set; }

        public Guid PublicId { get; set; }

        public bool IsAllowEdit { get; set; }

        public ICollection<BugTicketCommentManageModel> Comments { get; set; }

        public ICollection<FileModel> Files { get; set; }

        public BugTicketManageModel(BugTicket item)
        {
            if (item != null)
            {
                Id = item.Id;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Priority = item.Priority;
                Summary = item.Summary;
                Description = item.Description;
                IdAddedBy = item.IdAddedBy;
                AddedBy = item.AddedBy;
                AddedByAgent = item.AddedByAgent;
                PublicId = item.PublicId;
                if (item.Comments!=null)
                {
                    Comments = new List<BugTicketCommentManageModel>();
                    foreach(var comment in item.Comments)
                    {
                        Comments.Add(new BugTicketCommentManageModel(comment));
                    }
                }
                if (item.Files != null)
                {
                    Files = item.Files.Select(p => new FileModel()
                    {
                        Id = p.Id,
                        FileName = p.FileName,
                        Description = p.Description,
                        UploadDate = p.UploadDate,
                    }).ToList();
                }
            }
        }

        public BugTicket Convert()
        {
            BugTicket toReturn = new BugTicket();
            toReturn.Id = Id;
            toReturn.StatusCode = StatusCode;
            toReturn.Priority = Priority;
            toReturn.Summary = Summary;
            toReturn.Description = Description;
            toReturn.PublicId = PublicId;
            if (Files != null)
            {
                toReturn.Files = Files.Select(p => new BugFile()
                {
                    Id = p.Id,
                    FileName = p.FileName,
                    Description = p.Description ?? String.Empty,
                    UploadDate = DateTime.Now,
                    IdBugTicket = toReturn.Id,
                }).ToList();
            }

            return toReturn;
        }
    }
}