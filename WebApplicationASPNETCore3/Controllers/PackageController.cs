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
        private readonly PackageService _PackageService;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;

        public PackageController(EZAuth ezAuth, EZSession ezSession, CRMContext context, PackageService PackageService)
        {
            _context = context;
            _PackageService = PackageService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
        }

        public async Task<IActionResult> Index(bool hasError, string tabId, int campaignId)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
            ViewBag.CampaignId = campaignId;
            
            List<Package> packages = await _PackageService.GetPackageByCampaignId(campaignId);

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
                Package Package = await _PackageService.Create(_ezAuth.UserName, model);

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
                return  BadRequest("Bad request Package Id");
            }
            Package Package = await _PackageService.GetPackageById(id.GetValueOrDefault());
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
                return  BadRequest("Bad request Package Id");
            }
            if (ModelState.IsValid)
            {
                PackageToUpdate = await _PackageService.Update(_ezAuth.UserName, id.GetValueOrDefault(), model);
            
            }

            return View(model);
        }

    }
}
