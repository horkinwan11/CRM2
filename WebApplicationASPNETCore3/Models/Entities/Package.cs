using System;
using System.Collections.Generic;

namespace CRM.Models.Entities
{
    public partial class Package
    {
        public Package()
        {
           // UserPermission = new HashSet<UserPermission>();
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedDate { get; set; }

        //public virtual UserCredential UserCredential { get; set; }
        //public virtual ICollection<UserPermission> UserPermission { get; set; }
    }
}
