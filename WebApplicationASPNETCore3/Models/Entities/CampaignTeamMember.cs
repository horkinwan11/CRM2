using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace CRM.Models.Entities
{
    public partial class CampaignTeamMember
    {


        //public enum Grade
        //{
        //    "Open", "Closed", "Deleted"
        //}
            public CampaignTeamMember()
        {
           
        }

        public int CampaignId { get; set; }
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; }
        public string Status { get; set; }

        [ForeignKey("Id")]
        public virtual Campaign Campaign { get; set; }
        public virtual User User { get; set; }//linked via MemberId

        
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        

        //public virtual UserCredential UserCredential { get; set; }
        //public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
