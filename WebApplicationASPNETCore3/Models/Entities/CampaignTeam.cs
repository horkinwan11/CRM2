using System;
using System.Collections.Generic;

namespace CRM.Models.Entities
{
    public partial class CampaignTeam
    {


        //public enum Grade
        //{
        //    "Open", "Closed", "Deleted"
        //}
            public CampaignTeam()
        {
           // UserPermission = new HashSet<UserPermission>();
        }
        public int CampaignId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }

        public virtual Campaign Campaign { get; set; }
        public virtual User User { get; set; }  //linked via TeamId

        public string Status { get; set; }
             
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        

        //public virtual UserCredential UserCredential { get; set; }
        //public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
