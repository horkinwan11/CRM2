using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

using CRM.Models.Entities;


namespace CRM.Models.ViewModels
{
	public class CustomerViewModel 
	{
		
		public Customer Customer { get; set; }

		//private readonly List<KeyValuePairCls> _status;
		//[Display(Name = "Status")]
		//public int SelectedStatusId { get; set; }
		//public IEnumerable<SelectListItem> FlavorItems
		//{
		//	get { return new SelectList(_status, "Id", "Name"); }
		//}
		public string SelectedWKCampaignId { get; set; }
		public string CurrentPage { get; set; }
		public string PageSize { get; set; }
		public string SelectedStatusId { get; set; }
		public IEnumerable<SelectListItem> StatusList { get; set; }

	}

}
