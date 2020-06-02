using System;
using System.Linq;
using System.Text.Json;
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
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MySql.Data.MySqlClient;

namespace CRM.Controllers
{
    //[EZAuth(permissions: "Agent")]
    public class TeamDistributionController : Controller
    {
        private readonly CRMContext _context;
        private readonly TeamDistributionService _teamDistributionService;
        private readonly CustomerService _customerService;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;
        private readonly UserService _userService;

        public TeamDistributionController(EZAuth ezAuth, EZSession ezSession, CRMContext context, TeamDistributionService teamDistributionService, CustomerService customerService, UserService userService)
        {
            _context = context;
            _teamDistributionService = teamDistributionService;
            _customerService = customerService;
            _userService = userService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;

        }

        public async Task<IActionResult> Index(bool hasError, string tabId, int campaignId, string campaignName)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
            ViewBag.CampaignId = campaignId;
            ViewBag.CampaignName = campaignName;
            ViewData["Title"] = "Campaign: " + campaignName;

            TeamDistributionViewModel model = null;
            try
            {
                model =  await _customerService.GetTeamDistributionList(_ezAuth.UserName, campaignId, _ezAuth.GetRole());
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            //ViewData["TeamLeads"] = users.Select(m => new SelectListItem { Text = m.Email, Value = m.Id.ToString() }).ToList();

            // Info.
            return View(model); 
            
        }
       
        
        public async Task<IActionResult> LoadTeamDistributionList(int? campaignId)
        {
            TeamDistributionViewModel model = null;

            if (campaignId == null)
            {
                return BadRequest("Could not load due to bad input campaign.");
            }

            model = await  _customerService.GetTeamDistributionList(_ezAuth.UserName, campaignId.GetValueOrDefault(), _ezAuth.GetRole());

            return PartialView("_TeamDistributionListPartial", model);
        }

        public async Task<IActionResult> TeamDistributionNewEditModalPage(int? id, int campaignId)
        {
            TeamDistribution teamdist = null;
            if (id == null)
            {
                return BadRequest("Bad request Id");
            }
            if (id > 0)
                teamdist = await _customerService.GetTeamDistribution(_ezAuth.UserName, campaignId, _ezAuth.GetRole());
            else
            {
                teamdist = new TeamDistribution();
                
            }
            List<User> users = null;
            switch (_ezAuth.GetRole().ToUpperInvariant())
            {
                case "ADMIN":
                    users = await _userService.GetTLead();
                    ViewBag.TUsers = users.Select(m => new SelectListItem { Text = m.Email, Value = m.Id.ToString() }).ToList() ;
                    break;
                case "LEADER":
                    users = await _userService.GetTMember();
                    ViewBag.TUsers = users.Select(m => new SelectListItem { Text = m.Email, Value = m.Id.ToString() }).ToList();
                    break;
                default:
                    break;
            }
            
            return PartialView("_TeamDistributionDetailModalPartial", teamdist);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessTeamDistributionModalPage(int campaignId, TeamDistribution model)
        {

            if (campaignId.Equals(null))
            {
                return BadRequest("Bad Request Campaign Id in Package Form");
            }


            
            TeamDistribution teamdist = await _customerService.UpdateTeamDistribution(_ezAuth.UserName, campaignId, model);
            if (teamdist == null)
                return NotFound("No team to be distributed."); //TODO: replace with no create status
            return Ok();
        }

        public async Task<IActionResult> Delete(int campaignId, int? id, TeamDistributionTLevel tLevel)
        {
            if (id == null)
            {
                return BadRequest("Bad request Package Id");
            }
            
            await _customerService.DeleteTeamDistribution(_ezAuth.UserName, campaignId, id.GetValueOrDefault(), tLevel);

            return Ok();
        }
    }

}