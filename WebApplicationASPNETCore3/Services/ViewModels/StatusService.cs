using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MySql.Data.MySqlClient;

using CRM.Models.Entities;
using CRM.Models.ViewModels;


namespace CRM.Services.ViewModels
{
    public interface IStatusService
    {
        Task<Status> GetStatusById(int id);
        Task<Status> GetStatusByCampaignId(int id);
    }
    public class StatusService : IStatusService
    {
        private readonly CRMContext _context;
        public StatusService(CRMContext context)
        {
            _context = context;
        }



        public async Task<Status> GetStatusByCampaignId(int campaignId)
        {
            Status status = null;
                try
                {
                    status = await _context.Status.SingleOrDefaultAsync(m => m.CampaignId == campaignId);
                   
                }
                catch (Exception e)
                {
                    //throw (e);
                }

            return status;
        }


        public async Task<Status> GetStatusById(int id)
        {
            Status status = null;
            try
            {
                status = await _context.Status.SingleOrDefaultAsync(m => m.Id == id);

            }
            catch (Exception e)
            {
                //throw (e);
            }

            return status;
        }

        public async Task<List<Status>> CopyStatusBySelectedList(List<Status> statusList, int newCampaignId)
        {
           
            Status statusitem = null;
            try
                {
                //statusList = await _context.Status
                //           .Where(x => x.CampaignId == campaignId)
                //           .OrderBy(x => x.Code)
                //           .Select(x => new Status { Id = x.Id, Code = x.Code, Title = x.Title })
                //           .ToListAsync();

                DateTime currentDt = DateTime.Now;
                foreach ( Status s in statusList)
                {
                    statusitem = new Status();
                    statusitem.Code = s.Code;
                    statusitem.Title = s.Title;
                    statusitem.CampaignId = newCampaignId;
                    statusitem.CreatedDate = currentDt;
                    _context.Status.Add(statusitem);
                }
                
                await _context.SaveChangesAsync();


                }
                catch (DbUpdateConcurrencyException)
                {
                    //if (!UserExists(id))
                    //{
                    //    return NotFound();
                    //}
                    //else
                    //{
                    //    throw;
                    //}
                }
                catch (Exception e)
                {
                    //throw (e);
                }

         return statusList;
        }

    }
}