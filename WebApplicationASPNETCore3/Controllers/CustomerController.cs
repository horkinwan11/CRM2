using System;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

using CRM.Models.Entities;
using CRM.Models.ViewModels;
using CRM.Services;
using CRM.Services.ViewModels;

namespace CRM.Controllers
{
    
    public class CustomerController : Controller
    { 
        private readonly CRMContext _context;
        private readonly CustomerService _customerService;
        private readonly CampaignService _campaignService;
        private readonly UserService _userService;
        //private readonly HttpContext _httpContext;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;
        
        public CustomerController(EZAuth ezAuth, EZSession ezSession, CRMContext context, CustomerService customerService, CampaignService campaignService, UserService userService)
        {
            _context = context;
            _customerService = customerService;
            _campaignService = campaignService;
            _userService = userService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
            
        }

        [HttpPost]
        public async Task<IActionResult> Index(string SelectedWKCampaignId,bool dummy)
        {
            int wkCampaignId;
           //SelectedWKCampaignId = "";
            bool hasError = false;
            //Retrieve Last Working Campaign
            if (!String.IsNullOrEmpty(SelectedWKCampaignId))
            {
                wkCampaignId = int.Parse(SelectedWKCampaignId);
                User user = await _userService.UpdateWKCampaignIdByUserName(_ezAuth.UserName, wkCampaignId);

            }
            else
                hasError = true;

            return RedirectToAction("Index", new { hasError = hasError });
            
        }


        public async Task<IActionResult> Index(bool hasError)
        {
            if (hasError)
                ModelState.AddModelError("", "Couldn't update campaign into database.");
            //int _currentPage = currentPage.Equals(null) ? currentPage : 1;
            //int _pageSize = pageSize.Equals(null) ? pageSize : 10;
            int currentPage = 1;
            int pageSize = 10;
            string wkCampaignId="";
            CustomerPagination customerPagination= null;
            

            //Populate Campaign Dropdown list  
            //List<Campaign> campaigns = await _campaignService.GetCampaign();
            List<Campaign> campaigns = await _campaignService.GetCampaignByUserName(_ezAuth.UserName);
            
            if (campaigns.Count == 0)
            {
                ModelState.AddModelError("", "No campaign available for this user");
                //return BadRequest("No campaign available for this user");
            }
            ViewBag.CampaignList = campaigns.Select(m => new SelectListItem {  Value = m.Id.ToString(), Text = m.Name });

            //Retrieve Last Working Campaign
            User user = await _userService.GetUserByUserName(_ezAuth.UserName);
            if (user == null)
            {
                ModelState.AddModelError("", "This user not found to retrieve last working campaign");
                ////return View();
                //return BadRequest("This user not found to retrieve last working campaign");
            }
            else
            {
                if (user.LastWKCampaignId != null)
                {
                    int iLastWKCampaignId = user.LastWKCampaignId.GetValueOrDefault();
                    wkCampaignId = iLastWKCampaignId.ToString();
                    //Campaign wkCampaign = campaigns.SingleOrDefault(m => m.Id == iLastWKCampaignId);
                    //wkCampaignTableName = wkCampaign.customerTableName;
                } //TODO: else just get 1st record from campaign list
            }
            if (!string.IsNullOrEmpty(wkCampaignId))
            {
                customerPagination = await _customerService.GetPaginatedResult(_ezAuth.UserName, _ezAuth.GetRole(), wkCampaignId, null, currentPage, pageSize);
                customerPagination.SelectedWKCampaignId = wkCampaignId;
            }
         
            return View(customerPagination);

        }
      
        public async Task<IActionResult> LoadCustomerList(string SelectedWKCampaignId, string currentPage, string searchString)
        {
           
            int _currentPage = String.IsNullOrEmpty(currentPage)? 1: int.Parse(currentPage);
            int _pageSize = 10;
           
            if (String.IsNullOrEmpty(SelectedWKCampaignId))
            {
                return BadRequest("Could not load due to bad input campaign.");
            }
            
                CustomerPagination customerPagination = await _customerService.GetPaginatedResult(_ezAuth.UserName,_ezAuth.GetRole(), SelectedWKCampaignId,  searchString, _currentPage, _pageSize);
           
            return PartialView("_CustomerListPartial", customerPagination);
        }



     

        public async Task<IActionResult> ViewCustomerModalPage(int id, string SelectedWKCampaignId)
        {
            
            CustomerViewModel model = null;
            int wkCampaignId = int.Parse(SelectedWKCampaignId);

                 // Initialise view model
                 model = new CustomerViewModel();
            model.SelectedWKCampaignId = SelectedWKCampaignId;
           Customer customer = await _customerService.GetCustomerById(_ezAuth.UserName, SelectedWKCampaignId, id);
               model.Customer = customer;
               
                IEnumerable<SelectListItem> selectList =
                             from s in _context.Status
                             where s.CampaignId == wkCampaignId
                             select new SelectListItem
                               {
                                   Selected = (s.Id == model.Customer.Status),
                                   Text = s.Code,
                                   Value = s.Id.ToString()
                               };
                model.StatusList = selectList;
        
            return PartialView("_CustomerDetailModalPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> ProcessCustomerModalPage(CustomerViewModel model, string SelectedWKCampaignId) //(CustomerViewModel model)
        {
          
            KeyValuePairCls kvp = model.Customer.KPV.Find(m => m.Key == "Id");
            if (kvp == null || string.IsNullOrEmpty(kvp.Value))
                return BadRequest("Couldn't save customer with bad input Id");

            int? statusId = null;
            if ( ! string.IsNullOrEmpty(model.SelectedStatusId))
                statusId =  int.Parse(model.SelectedStatusId);

             Customer customer = await _customerService.UpdateCustomer(_ezAuth.UserName, SelectedWKCampaignId, model.Customer, statusId);

            if (customer == null)
                return NotFound("No Record updated.");
            //await _customerService.UpdateStatus(SelectedWKCampaignId, int.Parse(kvp.Value), model.Customer, int.Parse(model.SelectedStatusId)); 

            return Ok();
        }
       
        //[HttpPost]
        //public async Task<ActionResult> UpdateStatus(int Id, Customer model, int selectedStatusId)
        //{
        //    try
        //    {
        //        await _customerService.UpdateStatus(Id, model, selectedStatusId);
            
        //    }
        //    catch (MySqlException me)
        //    {
        //        //throw (me);

        //    }
        //    catch (Exception e)
        //    {
        //        //throw (e);
        //    }
           
        //    return RedirectToAction("Index");
        //}


    }
}
