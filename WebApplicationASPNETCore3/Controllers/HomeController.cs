using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CRM.Models;
using CRM.Services;

   

namespace CRM.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly EZAuth _ezAuth;
      
        public HomeController(EZAuth ezAuth, ILogger<HomeController> logger)
        {
            _logger = logger;
            _ezAuth = ezAuth;
        }

        public IActionResult Index()
        {
            //if (!_ezAuth.ScopeAuthInfo.IsAuthenticated )
            //{
            //    return new RedirectToActionResult("Login", "Account", new { returnUrl = "" });
            //}
            if (!_ezAuth.IsAuthenticated)
            {
                return new RedirectToActionResult("Login", "Account", new { returnUrl = "" });
            }
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
