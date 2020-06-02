using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{

    public enum TeamDistributionTLevel
    {
       
        [Display(Name = "Lead", Order = 0)]
        L,
        [Display(Name = "Member", Order = 1)]
        M
    }

}
