using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VC.Public.Models.Checkout
{
    public class EGiftSendEmailCodeModel
    {
        public string Code { get; set; }
    }

    public class EGiftSendEmailModel
	{
        public EGiftSendEmailModel()
        {
            SelectedCodes = new List<string>();
        }

        [Display(Name = "Recipient Email")]
        [EmailAddress]
        [Required]
        [MaxLength(250)]
        public string Email { get; set; }

        [Display(Name = "Recipient Name")]
        [Required]
        [MaxLength(250)]
        public string Recipient { get; set; }

        [Display(Name = "Message")]
        [MaxLength(500)]
        public string Message { get; set; }

        [Display(Name = "All")]
        public bool All { get; set; }

        public IList<EGiftSendEmailCodeModel> Codes { get; set; }

        public IList<string> SelectedCodes { get; set; }
    }
}
