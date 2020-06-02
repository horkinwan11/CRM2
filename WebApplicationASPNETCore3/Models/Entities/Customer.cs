using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;

namespace CRM.Models.Entities
{
    [Serializable]
    public class Customer
    {       
        public Customer()
        {
         }

        public int Id { get; set; }
        public string First_Name { get; set; }
        public string Last_Name { get; set; }
        public string Email { get; set; }

        public int Status { get; set; }
        public DateTime? CreatedDate { get; set; }

       public int? CampaignId { get; set; }
       public int? PackageId { get; set; }
       public int? TLeadId { get; set; }

        public string TLeadName { get; set; }
        public int? TMemberId { get; set; }

        public string TMemberName { get; set; }

        public List<KeyValuePairCls> KPV { get; set; }


        //public Dictionary<string, object> Dc { get; set; }
        //Dictionary<string, object> properties = new Dictionary<string, object>();

        //public override bool TryGetMember(GetMemberBinder binder, out object result)
        //{
        //    if (properties.ContainsKey(binder.Name))
        //    {
        //        result = properties[binder.Name];
        //        return true;
        //    }
        //    else
        //    {
        //        result = "Invalid Property!";
        //        return false;
        //    }
        //}

        //public override bool TrySetMember(SetMemberBinder binder, object value)
        //{
        //    properties[binder.Name] = value;
        //    return true;
        //}

        //public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        //{
        //    dynamic method = properties[binder.Name];
        //    result = method(args[0].ToString(), args[1].ToString());
        //    return true;
        //}

    }
    [Serializable]
    public class KeyPairValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
