using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CRM.Models.ViewModels;
using CRM.Services;

namespace CRM.Controllers
{
	public class AccountController : Controller
	{
		private readonly EZAuth _ezAuth;
		public AccountController(EZAuth ezAuth)
		{
			_ezAuth = ezAuth;
		}

		public IActionResult Login(string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> LoginAsync(AccountLogin model, string returnUrl = null)
		{
			ViewData["ReturnUrl"] = returnUrl;
			if (ModelState.IsValid)
			{
				var loggedIn = await _ezAuth.SignIn(model.Email, model.Password);
				if (loggedIn)
				{
					if (IsUrlValid(returnUrl))
					{
						return Redirect(returnUrl);
					}

					return RedirectToAction("Index", "Home");
					//return RedirectToLocal(returnUrl);
				}
				else
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					return View(model);
				}
			}

			return View(model);
		}

		public async Task<IActionResult> Logout()
		{
			//TempData.Clear();
			HttpContext.Session.Clear();
			await _ezAuth.SignOut();
			return RedirectToAction("Login", "Account");
		}

		private IActionResult RedirectToLocal(string returnUrl)
		{
			if (Url.IsLocalUrl(returnUrl))
			{
				return Redirect(returnUrl);
			}
			else
			{
				return RedirectToAction(nameof(HomeController.Index), "Home");
			}
		}
		private static bool IsUrlValid(string returnUrl)
		{
			return !string.IsNullOrWhiteSpace(returnUrl)
				   && Uri.IsWellFormedUriString(returnUrl, UriKind.Relative);
		}

	}
}