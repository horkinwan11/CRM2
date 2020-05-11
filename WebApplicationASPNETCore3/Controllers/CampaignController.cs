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
        private readonly EZSession _ezSession;

        public CampaignController(EZSession ezSession, CRMContext context, CampaignService campaignService)
        {
            _context = context;
            _campaignService = campaignService;
            _ezSession = ezSession;
        }

        public async Task<IActionResult> Index(bool hasError, int? currentPage, int? pageSize)
        {
            int _currentPage = currentPage == null ? 1 : currentPage.GetValueOrDefault();
            int _pageSize = pageSize == null ? 10 : pageSize.GetValueOrDefault();


            CampaignPagination campaignPagination = await _campaignService.GetPaginatedResult(null, _currentPage, _pageSize);

            return View(campaignPagination);
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


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return  BadRequest("Bad request campaign Id");
            }
            Campaign campaign = await _campaignService.GetCampaignById(id.GetValueOrDefault());
            if (campaign == null)
            {
                return NotFound();
            }
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
                campaignToUpdate = await _campaignService.Update(id.GetValueOrDefault(), model);
            
            }

            return View(model);
        }

    }
}
