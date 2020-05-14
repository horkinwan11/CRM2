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
    public interface ICampaignService
    {
        Task<List<Campaign>> GetCampaign();
        //Task<int> GetCount();
        Task<Campaign> GetCampaignById(int id);
    }
    public class CampaignService : ICampaignService
    {
        private readonly CRMContext _context;
       
        public CampaignService(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<Campaign>> GetCampaign()
        {

            List<Campaign> campaigns = await _context.Campaign
                .ToListAsync();  

            return campaigns;
        }
        public async Task<dynamic>  GetCampaignByUserName(string userName)
        {
            var query =  await (from ctm in _context.CampaignTeamMember
                          join c in _context.Campaign
                          on ctm.CampaignId equals c.Id
                          where ctm.MemberName == userName
                          select c).ToListAsync();
            //select new
            //{
            //    Id = c.Id,
            //    Name = c.Name,
            //}).ToList();
            //var query = await _context.CampaignTeamMember.Join(_context.Campaign,
            //                     ctm => ctm.CampaignId, // Primary Key
            //                     c => c., // Foreign Key
            //                     (ctm, c) => new { Id = ctm.CampaignId, Name = ctm. });

            //var query = await _context.Campaign.Join(_context.CampaignTeam,
            //                    ctm => ctm.Id, // Primary Key
            //                    c => c., // Foreign Key
            //                    (ctm, c) => new { Id = ctm.Id, Name = ctm.Name });
            //List<CampaignTeamMember> campaigns = await _context.CampaignTeamMember
            //     .Include<Campaign>()
            //     .Where(m => m.MemberName == userName)
            //     .Select(g => new { MemberName = g.Key, Count = g. });
            //     .ToListAsync();

            return query;
        }

        public async Task<Campaign> GetCampaignById(int id)
        {

            Campaign campaign = await _context.Campaign
                //.Include(u => u.CampaignTeam)
                .SingleOrDefaultAsync(m => m.Id == id);

            return campaign;
        }
        public async Task<Campaign> Create(string userName, Campaign model)
        {

            Campaign campaignToUpdate = model;
            campaignToUpdate.Description = model.Description;
            campaignToUpdate.Name = model.Name;
            campaignToUpdate.StartDate = model.StartDate;
            campaignToUpdate.EndDate = model.EndDate;
            campaignToUpdate.Status = model.Status;
            campaignToUpdate.CreatedBy = userName;
            campaignToUpdate.CreatedDate = DateTime.Now;
            await _context.Campaign.AddAsync(campaignToUpdate);
            await _context.SaveChangesAsync();
            return campaignToUpdate;
        }
        public async Task<Campaign> Update(string userName, int id, Campaign model)
        {

            Campaign campaignToUpdate = await _context.Campaign.SingleOrDefaultAsync(m => m.Id == id);
            campaignToUpdate.Description = model.Description;
            campaignToUpdate.Name = model.Name;
            campaignToUpdate.StartDate = model.StartDate;
            campaignToUpdate.EndDate = model.EndDate;
            campaignToUpdate.Status = model.Status;
            campaignToUpdate.UpdatedBy = userName;
            campaignToUpdate.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return campaignToUpdate;
        }

        public async Task<string> GetCustomerTableNameByCampaignId(int id)
        {

            Campaign campaign = await _context.Campaign.SingleOrDefaultAsync(m => m.Id == id);

            
            return campaign.customerTableName; 
        }

        public async Task<CampaignPagination> GetPaginatedResult( String searchString, int currentPage, int pageSize = 10)
        {
            int offset = (currentPage - 1) * pageSize;
            String _searchString="";
            CampaignPagination campaignPagination = new CampaignPagination();

            if (!String.IsNullOrEmpty(searchString))
            {
                _searchString = searchString.Trim();
            }
            int numRecords = await _context.Campaign
                 // .Where(m=> m.Name.Contains(_searchString)
                 .CountAsync();

            List<Campaign> campaigns = await _context.Campaign
                // .Where(m=> m.Name.Contains(_searchString)
                .Skip(offset).Take(pageSize).ToListAsync();


            campaignPagination.Campaigns = campaigns;

            Pager pager = new Pager(numRecords, currentPage, pageSize);
            campaignPagination.Pager = pager;

             return campaignPagination;
        }

    }
}