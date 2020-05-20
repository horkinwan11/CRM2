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
        private readonly Cryptography _cryptography;


        public UserService(CRMContext context, Cryptography cryptography)
        {
            _context = context;
            _cryptography = cryptography;
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


        public async Task<User> Create(string userName, User model, string password)
        {

            User userToUpdate = new User();
            userToUpdate.Email = model.Email;
            userToUpdate.FirstName = model.FirstName;
            userToUpdate.LastName = model.LastName;
            userToUpdate.IsTeamLead = model.IsTeamLead;
            userToUpdate.Status = model.Status;
            userToUpdate.CreatedBy = userName;
            userToUpdate.CreatedDate = DateTime.Now;

            var passwordSalt = Guid.NewGuid().ToString();
            var userCredential = new UserCredential()
            {
                PasswordSalt = passwordSalt,
                HashedPassword = _cryptography.HashSHA256(password + passwordSalt),
            };
            userToUpdate.UserCredential = userCredential;

            await _context.User.AddAsync(userToUpdate);
            await _context.SaveChangesAsync();
            return userToUpdate;
        }
        public async Task<User> Update(string userName, int id, User model)
        {

            User userToUpdate = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            //userToUpdate.Email = model.Email;
            userToUpdate.FirstName = model.FirstName;
            userToUpdate.LastName = model.LastName;
            userToUpdate.IsTeamLead = model.IsTeamLead;
            userToUpdate.Status = model.Status;
            userToUpdate.UpdatedBy = userName;
            userToUpdate.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return userToUpdate;
        }
        public async Task<User> Delete(string userName, int id)
        {

            User UserToUpdate = await _context.User.SingleOrDefaultAsync(m => m.Id == id);
            _context.User.Remove(UserToUpdate);
            await _context.SaveChangesAsync();
            return UserToUpdate;
        }
        public async Task<UserPagination> GetPaginatedResult(String searchString, int currentPage, int pageSize = 10)
        {
            int offset = (currentPage - 1) * pageSize;
            String _searchString = "";
            UserPagination userPagination = new UserPagination();

            if (!String.IsNullOrEmpty(searchString))
            {
                _searchString = searchString.Trim();
            }
            int numRecords = await _context.User
                  .Where(m => m.FirstName.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1
                      || m.LastName.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1
                      || m.Email.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1)
                  .CountAsync();

            List<User> users = await _context.User
                 .Where(m => m.FirstName.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1 
                     || m.LastName.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1
                     || m.Email.IndexOf(_searchString, StringComparison.InvariantCultureIgnoreCase) > -1
                  ).Skip(offset).Take(pageSize).ToListAsync();


            userPagination.Users = users;

            Pager pager = new Pager(numRecords, currentPage, pageSize);
            userPagination.Pager = pager;

            return userPagination;
        }

        public async Task<UserCredential> ChangePassword(string userName, int id, string password)
        {
            
            UserCredential userCredential = await _context.UserCredential.SingleOrDefaultAsync(m => m.Id == id);
            var passwordSalt = Guid.NewGuid().ToString();
            userCredential.PasswordSalt = passwordSalt;
            userCredential.HashedPassword = _cryptography.HashSHA256(password + passwordSalt);
            await _context.SaveChangesAsync();

            return userCredential;
        }



    }
}