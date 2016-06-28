using System;
using System.ComponentModel.DataAnnotations;

namespace VC.Public.Models.Checkout
{
    public class EGiftSendEmailModel
	{
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
    }
}
