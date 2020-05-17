using CRM.Models.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.ViewModels
{
	public class CampaignCopyViewModel
	{

		public List<Campaign> Campaigns { get; set; }

		public List<Package> Packages { get; set; }

		public List<Status> Statuss { get; set; }

	}

}
