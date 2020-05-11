using System;
using System.Collections.Generic;

namespace CRM.Models.Entities
{
    public partial class Status
    {
        public Status()
        {
           // UserPermission = new HashSet<UserPermission>();
        }

        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }

        public int CampaignId { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public virtual UserCredential UserCredential { get; set; }
        //public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
