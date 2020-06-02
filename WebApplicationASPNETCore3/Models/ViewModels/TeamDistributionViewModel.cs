using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Data;
using CRM.Models.Entities;

namespace CRM.Models.ViewModels
{
    public class TeamDistributionViewModel
    {
        public int TLeadId { get; set; }

        public Int64 TotalQuantity { get; set; }

        public Int64 UnusedQuantity { get; set; }

        public Int64 UsedQuantity { get; set; }

        public Int64 UsedQuantityPerc { get; set; }
        public List<TeamDistribution> TeamDistributions { get; set; }
    }
}
   
