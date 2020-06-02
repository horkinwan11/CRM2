using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace CRM.Models.ViewModels
{
    public class CustomerFileUploadViewModel
    {
        [Required]
        [Display(Name = "Import CSV File")]
        public IFormFile FileAttach { get; set; }
        public int CampaignId { get; set; }
        public DataTable Data { get; set; }
    }
}
