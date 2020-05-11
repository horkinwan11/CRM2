using CRM.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.ViewModels
{
	public class CampaignPagination
	{

		public List<Campaign> Campaigns { get; set; }  //Campaigns List

		public Pager Pager { get; set; } 
		
		
	}

}
