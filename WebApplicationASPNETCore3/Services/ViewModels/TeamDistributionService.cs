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
    public interface ITeamDistributionService
    {
        Task<List<TeamDistribution>> GetTeamDistribution();
     
        
    }
    public class TeamDistributionService : ITeamDistributionService
    {
        private readonly CRMContext _context;
       
        public TeamDistributionService(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<TeamDistribution>> GetTeamDistribution()
        {

            //var customers = _context.Customer
            //    .GroupBy(item => item.TeamId);

            List<TeamDistribution> results = await _context.Customer
            .GroupBy(s => new { s.TLeadId, s.TLeadName })
            .Select(g => new TeamDistribution()
            {
                TLeadId = g.Key.TLeadId.GetValueOrDefault(),  //TODO: test whether int?
                TLeadName = g.Key.TLeadName,
                Quantity = g.Count(x => x.Status > 0)
            }).ToListAsync();


            return results;
        }


        public async Task<Package> Create(string userName, Package model)
        {

            Package PackageToUpdate = model;
            PackageToUpdate.Description = model.Description;
            PackageToUpdate.Name = model.Name; 
            PackageToUpdate.Description = model.Description;
            PackageToUpdate.Weight = model.Weight;
            PackageToUpdate.Status = model.Status;
            PackageToUpdate.CampaignId = model.CampaignId;  // assign to Campaign Id
            PackageToUpdate.CreatedBy = userName;
            PackageToUpdate.CreatedDate = DateTime.Now;
            await _context.Package.AddAsync(PackageToUpdate);
            await _context.SaveChangesAsync();
            return PackageToUpdate;
        }
        public async Task<Package> Update(string userName, int id, Package model)
        {

            Package PackageToUpdate = await _context.Package.SingleOrDefaultAsync(m => m.Id == id);
            PackageToUpdate.Name = model.Name;
            PackageToUpdate.Description = model.Description;
            PackageToUpdate.Weight = model.Weight;
            PackageToUpdate.Status = model.Status;
            PackageToUpdate.UpdatedBy = userName;
            PackageToUpdate.UpdatedDate = DateTime.Now;
            await _context.SaveChangesAsync();
            return PackageToUpdate;
        }

        public async Task<Package> Delete(int id)
        {

            Package PackageToUpdate = await _context.Package.SingleOrDefaultAsync(m => m.Id == id);
             _context.Package.Remove(PackageToUpdate);
            await _context.SaveChangesAsync();
            return PackageToUpdate;
        }
    

    }
}