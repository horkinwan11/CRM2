using CRM.Models.Entities;
using System;
using System.Collections.Generic;

namespace CRM.Models
{
	public class AuthInfo
	{
		public AuthInfo()
		{
			CreatedDate = DateTime.UtcNow;
		}
		public int UserId { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public List<string> Permissions { get; set; }
		public Dictionary<string, string> Claims { get; set; }
		public DateTime CreatedDate { get; set; }

		public bool IsAuthenticated => (UserId > 0);
	}
}
