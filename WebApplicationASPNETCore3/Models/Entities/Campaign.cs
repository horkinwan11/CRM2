using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{
    public partial class Campaign
    {


        //public enum Grade
        //{
        //    "Open", "Closed", "Deleted"
        //}
            public Campaign()
        {
            CampaignTeam = new HashSet<CampaignTeam>();
           
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public ItemStatus Status { get; set; }
        
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }
        public virtual ICollection<CampaignTeam> CampaignTeam { get; set; }
        
        //[DisplayFormat(NullDisplayText = "No grade")]
        //public Grade? Grade { get; set; }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public string customerTableName { get; set; }
        

        //public virtual UserCredential UserCredential { get; set; }
        //public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
