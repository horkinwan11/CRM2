using CRM.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.ViewModels
{
	public class UserPagination
	{

		public List<User> Users { get; set; }  //Users List

		public string SearchText { get; set; }
	 public Pager Pager { get; set; } 
		
		
	}

}
