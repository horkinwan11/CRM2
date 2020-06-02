using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using CRM.Models.Entities;
using CRM.Models.ViewModels;
using CRM.Services;
using CRM.Services.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;

namespace CRM.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly CRMContext _context;
        private readonly UserService _userService;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;
        private readonly Cryptography _cryptography;

        public UserController(EZAuth ezAuth, EZSession ezSession, CRMContext context, UserService userService,  Cryptography cryptography)
        {
            _context = context;
            _userService = userService;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
            _cryptography = cryptography;
        }
        public async Task<IActionResult> Index()
        {
            int _currentPage = 1;
            int _pageSize = 10;
            UserPagination userPagination = await _userService.GetPaginatedResult(null, _currentPage, _pageSize);

            return View(userPagination);
        }
        public async Task<IActionResult> LoadUserList(int? currentPage, int? pageSize, string searchText )
        {
            int _currentPage = currentPage == null ? 1 : currentPage.GetValueOrDefault();
            int _pageSize = pageSize == null ? 10 : pageSize.GetValueOrDefault();
           
            UserPagination userPagination = await _userService.GetPaginatedResult(searchText, _currentPage, _pageSize);

            return PartialView("_UserListPartial", userPagination);
        }

        public async Task<IActionResult> UserNewEditModalPage(int? id)
        {
            UserManageViewModel userManageViewModel = new UserManageViewModel();
            if (id == null)
            {
                return BadRequest("Bad request User Id");
            }
            if (id > 0)
            {    //load data to model for update form
                User user = await _userService.GetUserById(id.GetValueOrDefault());
                userManageViewModel.Id = user.Id;
                userManageViewModel.Email = user.Email;
                userManageViewModel.FirstName = user.FirstName;
                userManageViewModel.LastName = user.LastName;
                userManageViewModel.Status = user.Status;
                userManageViewModel.IsTeamLead = user.IsTeamLead;
                userManageViewModel.RoleId = user.RoleId;
            }
            else
            {  //preset data to model for create form
                //pre-setting fields for new Create Form
                userManageViewModel.Id = 0;  // duplicate assignment
                userManageViewModel.Email = "";
                userManageViewModel.Password = "";
                userManageViewModel.IsTeamLead = false;
            }
            //List<SelectListItem> packageList = packages.Select(m => new SelectListItem { Text = m.Name, Value = m.Id.ToString() }).ToList();
            ViewData["Roles"] = await _context.Role.Select(m => new SelectListItem { Text = m.Code, Value = m.Id.ToString() }).ToListAsync();
            return PartialView("_UserDetailModalPartial", userManageViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessUserModalPage(UserManageViewModel model)
        {

            User userToUpdate = null;
            if (model == null)
            {
                return BadRequest("Bad request User Form");
            }
            //if (ModelState.IsValid || true)
            //{
            //mapping with updateable fields in the form including Id
            User user = new User()
            {
                Id = model.Id,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Status = model.Status,
                IsTeamLead = model.IsTeamLead,
                RoleId = model.RoleId
            };

            if (model.Id > 0)
            {
                userToUpdate = await _userService.Update(_ezAuth.UserName, user.Id, user);
                if (userToUpdate == null)
                    return NotFound("No Record updated.");
            }
            else //Id=0  , New record
            {
                user.Email = model.Email; //

                userToUpdate = await _userService.Create(_ezAuth.UserName, user, model.Password);

            }

            return Ok("save completed.");

            //}


            // return StatusCode(304);


        }

        
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return BadRequest("Bad request User Id");
            }
            if (ModelState.IsValid)
            {
                await _userService.Delete(_ezAuth.UserName, id.GetValueOrDefault());

            }

            return Ok();
        }


        public async Task<IActionResult> UserChangePasswordModalPage(int? id)
        {
            
            if (id == null)
            {
                return BadRequest("Bad request User Id");
            }
            
            User user = await _userService.GetUserById(id.GetValueOrDefault());
            ViewData["User"] = user;
            return PartialView("_UserPasswordModalPartial");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(int id, string password)
        {
            if (ModelState.IsValid)
            {
               await _userService.ChangePassword(_ezAuth.UserName, id, password);
            }
             
            return Ok();
        }





    }
}
