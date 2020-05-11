using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{
    public partial class CampaignViewModel
    {


        //public enum Grade
        //{
        //    "Open", "Closed", "Deleted"
        //}
            public CampaignViewModel()
        {
          
           
        }

        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
       
        
    }
}
