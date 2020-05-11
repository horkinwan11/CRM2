using System.ComponentModel.DataAnnotations;

namespace CRM.Models.ViewModels
{
    public class AccountLogin
    {
		[Required]
		[EmailAddress]
		public string Email { get; set; }

		[Required]
		[DataType(DataType.Password)]
		public string Password { get; set; }

	}
}
