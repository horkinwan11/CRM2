using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CRM.Models.Entities
{
    public class KeyValuePairCls
        {
            [Key]
            public string Key { get; set; }
         public string Value { get; set; }
        }
    
}
