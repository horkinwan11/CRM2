using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{

    public enum ItemStatus
    {
       
        [Display(Name = "Inactive", Order = 1)]
        I,
        [Display(Name = "Active", Order = 0)]
        A
    }

}
