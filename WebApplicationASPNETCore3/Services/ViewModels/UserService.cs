using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

using CRM.Models.Entities;
using CRM.Models.ViewModels;


namespace CRM.Services.ViewModels
{
    public interface IUserService
    {
        Task<List<User>> GetUser();
        //Task<int> GetCount();
        Task<User> GetUserById(int id);
    }
    public class UserService : IUserService
    {
        private readonly CRMContext _context;
       
        public UserService(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUser()
        {

            List<User> Users = await _context.User.ToListAsync();  

            return Users;
        }


        public async Task<User> GetUserById(int id)
        {

            User user = await _context.User.SingleOrDefaultAsync(m => m.Id == id);

            return user;
        }

        public async Task<User> GetUserByUserName(string userName)
        {

            User user = await _context.User.SingleOrDefaultAsync(m => m.Email == userName);

            return user;
        }
        public async Task<User> UpdateWKCampaignIdByUserName(string userName, int WKCampaignId)
        {

            User user = await _context.User.SingleOrDefaultAsync(m => m.Email == userName);
            user.LastWKCampaignId = WKCampaignId;
            await _context.SaveChangesAsync();
            return user;
        }




    }
}