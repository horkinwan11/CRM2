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
    public class PackageController : Controller
    {
        private readonly CRMContext _context;
        private readonly PackageService _packageService;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;

        public PackageController(EZAuth ezAuth, EZSession ezSession, CRMContext context, PackageService packageService)
        {
            _context = context;
            _packageService = packageService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
        }

        public async Task<IActionResult> Index(bool hasError, string tabId, int campaignId, string campaignName)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
            ViewBag.CampaignId = campaignId;
            ViewBag.CampaignName = campaignName;
            ViewData["Title"] = "Campaign: " + campaignName;

            List<Package> packages = await _packageService.GetPackageByCampaignId(campaignId);

            return View(packages);
        }



        public ActionResult Create(string tabId)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(Package model)
        {
            if (ModelState.IsValid)
            {
                Package Package = await _packageService.Create(_ezAuth.UserName, model);

            }
            else
            {
                //TODO: return error save
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(int? id, string tabId)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;

            if (id == null)
            {
                return BadRequest("Bad request Package Id");
            }
            Package Package = await _packageService.GetPackageById(id.GetValueOrDefault());
            if (Package == null)
            {
                return NotFound();
            }
            return View(Package);
        }

        [HttpPost, ActionName("Edit")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int? id, Package model)
        {
            Package PackageToUpdate = null;
            if (id == null)
            {
                return BadRequest("Bad request Package Id");
            }
            if (ModelState.IsValid)
            {
                PackageToUpdate = await _packageService.Update(_ezAuth.UserName, id.GetValueOrDefault(), model);

            }

            return View(model);
        }

        public async Task<IActionResult> LoadPackageList(int? campaignId)
        {

            if (campaignId == null)
            {
                return BadRequest("Could not load due to bad input campaign.");
            }

            List<Package> packages = await _packageService.GetPackageByCampaignId(campaignId.GetValueOrDefault());

            return PartialView("_PackageListPartial", packages);
        }

        public async Task<IActionResult> PackageNewEditModalPage(int? id, int campaignId)
        {
            Package package = null;
            if (id == null)
            {
                return BadRequest("Bad request Package Id");
            }
            if (id > 0)
                package = await _packageService.GetPackageById(id.GetValueOrDefault());
            else
            {
                package = new Package();
                package.Status = ItemStatus.A;  //pre-select Status 
            }
                //package.CampaignId = campaignId; //TODO: verify this is not null
            //if (package == null)
            //{
            //    return NotFound();
            //}

            return PartialView("_PackageDetailModalPartial", package);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ProcessPackageModalPage(Package model) 
        {

            Package PackageToUpdate = null;
            if (model == null)
            {
                return BadRequest("Bad request Package Form");
            }
            if (ModelState.IsValid)
            {
                if (model.Id > 0)
                {
                    PackageToUpdate = await _packageService.Update(_ezAuth.UserName, model.Id, model);
                    if (PackageToUpdate == null)
                        return NotFound("No Record updated.");
                }
                else //Id=0
                {
                    PackageToUpdate = await _packageService.Create(_ezAuth.UserName, model);

                }
            }

            
            
            return Ok();
        }

    }
}
