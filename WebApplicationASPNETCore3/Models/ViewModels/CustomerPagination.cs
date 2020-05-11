using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

using CRM.Models.Entities;

namespace CRM.Models.ViewModels
{
    public class CustomerPagination

    {

        public CustomerPagination()
        {
        }

        [BindProperty(SupportsGet = true)]
        //public int CurrentPage { get; set; }
        //public int Count { get; set; }
        //public int PageSize { get; set; } = 10;

        //public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public DataTable Data { get; set; }  //Customer DataSet

        public Pager Pager { get; set; } //Customer Paging setting

        //public List<Campaign> Campaign { get; set; }
        public string SelectedWKCampaignId { get; set;} 

    //public List<KeyValuePair> KPV { get; set; }

}
   
    
}