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
    public class TestSideBarController : Controller
    {
        private readonly ILogger<TestSideBarController> _logger;
        private readonly EZAuth _ezAuth;

        public TestSideBarController(EZAuth ezAuth, ILogger<TestSideBarController> logger)
        {
            _logger = logger;
            _ezAuth = ezAuth;
        }

        public IActionResult Index()
        {
            ViewBag.DD = "{\"1\": \"Black Widow\", \"2\": \"Captain America\", \"3\": \"Iron Man\"}";

            return View();
        }
        public IActionResult Index1()
        {

            return View();
        }
    }
}
