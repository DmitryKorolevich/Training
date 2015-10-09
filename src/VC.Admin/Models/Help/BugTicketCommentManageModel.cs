using System;
using System.Collections.Generic;
using System.Linq;
using VC.Admin.Validators.Product;
using VitalChoice.Domain.Entities;
using VitalChoice.Domain.Entities.Content;
using VitalChoice.Validation.Models;
using VitalChoice.Validation.Attributes;
using VitalChoice.Validation.Models.Interfaces;
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
    [ApiValidator(typeof(BugTicketCommentManageModelValidator))]
    public class BugTicketCommentManageModel : BaseModel
    {
        public int Id { get; set; }

        public int IdBugTicket { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateEdited { get; set; }

        public RecordStatusCode StatusCode { get; set; }

        public string EditedBy { get; set; }

        public string EditedByAgent { get; set; }

        public string Comment { get; set; }

        public Guid PublicId { get; set; }

        public ICollection<FileModel> Files { get; set; }

        public BugTicketCommentManageModel(BugTicketComment item)
        {
            if (item != null)
            {
                Id = item.Id;
                IdBugTicket = item.IdBugTicket;
                DateCreated = item.DateCreated;
                DateEdited = item.DateEdited;
                StatusCode = item.StatusCode;
                Comment = item.Comment;
                EditedBy = item.EditedBy;
                EditedByAgent = item.EditedByAgent;
                PublicId = item.PublicId;
                Files = new List<FileModel>();
                if(item.Files!=null)
                {
                    Files = item.Files.Select(p => new FileModel()
                    {
                        Id=p.Id,
                        FileName=p.FileName,
                        Description=p.Description,
                        UploadDate=p.UploadDate,
                    }).ToList();
                }
            }
        }

        public BugTicketComment Convert()
        {
            BugTicketComment toReturn = new BugTicketComment();
            toReturn.Id = Id;
            toReturn.IdBugTicket = IdBugTicket;
            toReturn.Comment = Comment;
            toReturn.PublicId = PublicId;
            if (Files!=null)
            {
                toReturn.Files = Files.Select(p => new BugFile()
                {
                    Id=p.Id,
                    FileName=p.FileName,
                    Description = p.Description ?? String.Empty,
                    UploadDate =DateTime.Now,
                    IdBugTicketComment=toReturn.Id,
                }).ToList();
            }

            return toReturn;
        }
    }
}