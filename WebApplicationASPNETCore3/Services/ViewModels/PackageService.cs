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
    public interface IPackageService
    {
        Task<List<Package>> GetPackage();
        //Task<int> GetCount();
        Task<Package> GetPackageById(int id);
    }
    public class PackageService : IPackageService
    {
        private readonly CRMContext _context;
       
        public PackageService(CRMContext context)
        {
            _context = context;
        }

        public async Task<List<Package>> GetPackage()
        {

            List<Package> Packages = await _context.Package
                .ToListAsync();  

            return Packages;
        }
        
        public async Task<List<Package>> GetPackageByCampaignId(int campaignId)
        {

            List<Package> packages = await _context.Package
                .Where(m => m.CampaignId == campaignId)
                .ToListAsync();
            return packages;
        }
        public async Task<Package> GetPackageById(int id)
        {

            Package Package = await _context.Package
                //.Include(u => u.PackageTeam)
                .SingleOrDefaultAsync(m => m.Id == id);

            return Package;
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

        public async Task<Package> Delete(string userName, int id)
        {

            Package PackageToUpdate = await _context.Package.SingleOrDefaultAsync(m => m.Id == id);
             _context.Package.Remove(PackageToUpdate);
            await _context.SaveChangesAsync();
            return PackageToUpdate;
        }
        public async Task<List<Package>> CopyMultiPackages(string userName, int? campaignId, int? sc, string[] sp)
        {
            int dCampaignId = campaignId.GetValueOrDefault();
            //find all source Campaign Id = sc && package Id in sp list.
            List<Package> sPackages = await _context.Package.Where(m => m.CampaignId == sc.GetValueOrDefault() && sp.Contains(m.Id.ToString())).ToListAsync();
           
            //if (sPackages.Count == 0)
            //    return null; //TODO: return with no record to be saved
            foreach (Package p in sPackages)
            {
                Package packageToCopy = new Package();
                packageToCopy.Name = p.Name;
                packageToCopy.Description = p.Description;
                packageToCopy.Weight = p.Weight;
                packageToCopy.Status = p.Status;
                packageToCopy.CampaignId = dCampaignId;  // assign to destination Campaign Id
                packageToCopy.CreatedBy = userName;
                packageToCopy.CreatedDate = DateTime.Now;
                await _context.Package.AddAsync(packageToCopy);
            }
            await _context.SaveChangesAsync();
            return sPackages;
        }


    }
}