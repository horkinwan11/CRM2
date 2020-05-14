using System;
using System.Collections.Generic;

namespace CRM.Models.Entities
{
    public partial class Package
    {
        public Package()
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Weight { get; set; }
        public int CampaignId { get; set; }

        public ItemStatus Status { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

     }
}
