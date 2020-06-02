using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace CRM.Models.Entities
{
    public class TeamDistribution
    {
        public int TLeadId  { get; set; }

        public string TLeadName { get; set; }

        public int TMemberId { get; set; }

        public string TMemberName { get; set; }
        //public int CampaignId { get; set; }
        //public string CampaignName  { get; set; }

        public int TId { get; set; }
        public string TName { get; set; }

        public TeamDistributionTLevel TLevel  { get; set; }
        public Int64 Quantity { get; set; }
    }
}
