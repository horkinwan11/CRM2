using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using CRM.Models.Entities;
using CRM.Models.ViewModels;
using CRM.Services;
using CRM.Services.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;

namespace CRM.Controllers
{
	//[EZAuth(permissions: "Agent")]
	public class CampaignController : Controller
    {
        private readonly CRMContext _context;
        private readonly CampaignService _campaignService;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;

        public CampaignController(EZAuth ezAuth, EZSession ezSession, CRMContext context, CampaignService campaignService)
        {
            _context = context;
            _campaignService = campaignService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
        }
        public async Task<IActionResult> Index()
        {
            int _currentPage = 1;
            int _pageSize = 10;
            CampaignPagination campaignPagination = await _campaignService.GetPaginatedResult(null, _currentPage, _pageSize);

            return View(campaignPagination);
        }
        public async Task<IActionResult> LoadCampaignList(int? currentPage, int? pageSize, string searchText )
        {
            int _currentPage = currentPage == null ? 1 : currentPage.GetValueOrDefault();
            int _pageSize = pageSize == null ? 10 : pageSize.GetValueOrDefault();
           
            CampaignPagination campaignPagination = await _campaignService.GetPaginatedResult(searchText, _currentPage, _pageSize);

            return PartialView("_CampaignListPartial",campaignPagination);
        }
        

        public ActionResult GetCampaignList()
        {
            var campaigns = _context.Campaign.Select(x => new
            {
                Id = x.Id,
                Name = x.Name,

            })
            .ToList();
            return Ok(campaigns);
        }

       [HttpPost]
        public async Task<IActionResult> ChangeWKCampaign(int wkCampaignId)
        {
            var campaign = await _campaignService.GetCampaignById(wkCampaignId);
            if (campaign == null)
                return BadRequest();

            _ezSession.wkCampaignId = campaign.Id.ToString();
            _ezSession.wKCampaignName = campaign.Name.ToString();
            //_httpContext.Session.Set("wKCampaignId", Encoding.UTF8.GetBytes(campaign.Id.ToString()));
            //_httpContext.Session.Set("wKCampaignName", Encoding.UTF8.GetBytes(campaign.Name));
            //TempData["WKCampaignId"] = campaign.Id;
            //TempData["WKCampaignName"] = campaign.Name;
            //TempData.Keep();
            //RedirectToAction("Home", "Index", new { returnUrl = "" });
            return Ok();
        }

        public ActionResult Create(string tabId)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
            ViewData["Title"] = "Campaign: "; 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Campaign model)
        {
            Campaign campaign = null;
            if (ModelState.IsValid)
            {
                campaign = await _campaignService.Create(_ezAuth.UserName, model);

            }

            //Pass to the tab details view
            if (campaign != null)
            {
                ViewBag.CampaignId = campaign.Id;
                ViewBag.CampaignName = campaign.Name;
                ViewData["Title"] = "Campaign: " + campaign.Name;
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id, string tabId)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
           

            if (id == null )
            {
                return  BadRequest("Bad request campaign Id");
            }

           

            Campaign campaign = await _campaignService.GetCampaignById(id.GetValueOrDefault());
            if (campaign == null)
            {
                return NotFound();
            }

            //Pass to the tab details view
            ViewBag.CampaignId = id;
            ViewBag.CampaignName = campaign.Name;
            ViewData["Title"] = "Campaign: " + campaign.Name;

            return View(campaign);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, Campaign model)
        {
            Campaign campaignToUpdate = null;
            if (id == null)
            {
                return  BadRequest("Bad request campaign Id");
            }
            if (ModelState.IsValid)
            {
                campaignToUpdate = await _campaignService.Update(_ezAuth.UserName, id.GetValueOrDefault(), model);
            
            }
            //Pass to the tab details view
            ViewBag.CampaignId = model.Id;
            ViewBag.CampaignName = model.Name;
            ViewData["Title"] = "Campaign: " + model.Name;

            return View(model);
        }

    }
}
