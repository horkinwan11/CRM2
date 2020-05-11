using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{

    public enum ItemStatus
    {
        [Display(Name = "Active")]
        A,
        [Display(Name = "Inactive")]
        I
    }

}
