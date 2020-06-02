using CRM.Models.Entities;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.ViewModels
{
	public class UserManageViewModel : UserManagePasswordViewModel
	{
		[Required, StringLength(50, MinimumLength = 2)]
		[Display(Name = "First Name")]
		public string FirstName { get; set; }

		[Required, StringLength(50, MinimumLength = 2)]
		[Display(Name = "Last Name")]
		public string LastName { get; set; }

		[Required, EmailAddress, StringLength(100)]
		public string Email { get; set; }

		
		public ItemStatus Status { get; set; }
		
		public bool IsTeamLead { get; set; }

		public int RoleId { get; set; }
	}

	public class UserManagePasswordViewModel
	{
		public int Id { get; set; }

		[Required, StringLength(15, MinimumLength = 6)]
		[DataType(DataType.Password)]
		public string Password { get; set; }

		[Required, StringLength(15, MinimumLength = 6)]
		[Compare("Password")]
		[Display(Name = "Confirm Password")]
		[DataType(DataType.Password)]
		public string ConfirmPassword { get; set; }
	}
}
