using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{

    public enum DataAction
    {

        [Display(Name = "Create")]
        C,
        [Display(Name = "Update")]
        U,
        [Display(Name = "Delete")]
        D
    }

}
