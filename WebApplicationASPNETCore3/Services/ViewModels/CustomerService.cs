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
    public interface ICustomerService
    {
        Task<CustomerPagination> GetPaginatedResult(string userName, string wKCampaignId, String searchList, int currentPage, int pageSize = 10);
      
        Task<Customer> GetCustomerById(string userName, string wKCampaignId, int id);

        Task<Customer> UpdateCustomer(string userName, string wKCampaignId, Customer model, int? selectedStatusId);
    }
    public class CustomerService : ICustomerService
    {
        private readonly CRMContext _context;
        private readonly CampaignService _campaignService;

        public CustomerService(CRMContext context, CampaignService campaignService)
        {
            _context = context;
            _campaignService = campaignService;
        }

        public  async Task<CustomerPagination> GetPaginatedResult(string userName, string wKCampaignId, String searchString, int currentPage, int pageSize = 10)
        {
            int offset = (currentPage - 1) * pageSize;
            String _SQL = "";
            String _SQLfilterparams="";
            String _SQLlimitparams = "";
            String _SQLfilterusername = "";
            String _searchString;
            List<KeyValuePairCls> _SQLparams = new List<KeyValuePairCls>();

            if (!String.IsNullOrEmpty(searchString))
            {
                _searchString = searchString.Trim(';');
                string[] searchArray = _searchString.Split(";");

                if (searchArray.Length > 0)
                {
                    for (int i = 0; i < searchArray.Length; i++)
                    {
                        String searchparam = searchArray[i];
                        if (!String.IsNullOrEmpty(searchparam))
                        {
                            string[] paramKV = searchparam.Split("=");
                            if (paramKV.Length == 2)
                            {
                                string paramName = paramKV[0];
                                string paramValue = paramKV[1];
                                if (!String.IsNullOrEmpty(paramName) && !String.IsNullOrEmpty(paramValue))
                                {
                                    //_SQLfilterparams += paramName + "='" + paramValue + "' ";
                                    //_SQLfilterparams += "AND ";
                                    KeyValuePairCls item = new KeyValuePairCls();
                                    item.Key = paramName;
                                    item.Value = paramValue;
                                    _SQLparams.Add(item);
                                }
                            }
                            
                        }
                    }
                }
            }
           
            if (_SQLparams.Count > 0)
            {
                _SQLfilterparams += " where ";
                foreach (KeyValuePairCls item in _SQLparams)
                {
                   
                    _SQLfilterparams += item.Key + " like \"%" + item.Value + "%\" ";
                    if (_SQLparams.IndexOf(item) != _SQLparams.Count - 1)
                        _SQLfilterparams += "AND ";
                }

                //_SQLfilterparams = _SQLfilterparams.Replace("\\", string.Empty).Replace(@"\", string.Empty);
            }

            if (string.IsNullOrEmpty(_SQLfilterparams))
                _SQLfilterusername = " where MemberName = '" + userName + "' ";
            else
                _SQLfilterusername = " AND MemberName = '" + userName  + "' ";
            _SQLlimitparams = " limit " + Convert.ToString(offset) + "," + Convert.ToString(pageSize);

            //_SQL = "SELECT @tableName := customerTableName from campaign WHERE Id =" + wKCampaignId + ";";
            //_SQL += "SET @s = CONCAT('SELECT count(*) FROM ', @tableName ";
            //_SQL += String.IsNullOrEmpty(_SQLfilterparams) ? "" : ",' " + _SQLfilterparams + "'";
            //_SQL += " );";
            //_SQL += "PREPARE stmt FROM @s; ";
            //_SQL += "EXECUTE stmt; ";
            //_SQL += "SET @s = CONCAT('SELECT * FROM ', @tableName ";
            //_SQL += String.IsNullOrEmpty(_SQLfilterparams) ? "" : ",' " + _SQLfilterparams + "'";
            //_SQL += String.IsNullOrEmpty(_SQLlimitparams) ? "" : ",' " + _SQLlimitparams + "'";
            //_SQL += " );";
            //_SQL += "PREPARE stmt FROM @s; ";
            //_SQL += "EXECUTE stmt; ";
            //_SQL += "DEALLOCATE PREPARE stmt; ";

            string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(int.Parse(wKCampaignId));
            _SQL = "Select count(*) from " + customerTableName + _SQLfilterparams + _SQLfilterusername + "; ";
            _SQL += "Select * from " + customerTableName + _SQLfilterparams + _SQLfilterusername + _SQLlimitparams;
           
            MySqlConnection conn = (MySqlConnection)_context.Database.GetDbConnection();
            MySqlCommand cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            
            // MySqlDataReader dr = cmd.ExecuteReader();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);
            conn.Close();
            CustomerPagination customerPageModel = new CustomerPagination();
            customerPageModel.Data = ds.Tables[1];
            //customerPageModel.CurrentPage = currentPage;
            //customerPageModel.Count = int.Parse(ds.Tables[0].Rows[0][0].ToString());

            Pager pager = new Pager(int.Parse(ds.Tables[0].Rows[0][0].ToString()), currentPage, pageSize);
            customerPageModel.Pager = pager;
            
            // dt = ds.Tables[1];
            return customerPageModel;
            //return data.OrderBy(d => d.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }



        public async Task<Customer> GetCustomerById(string userName, string wKCampaignId, int id)
        {
            Customer customer = null;
            MySqlConnection conn;
            MySqlCommand cmd;
            String _SQL = "";
            String _SQLfilterusername = "";

            string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(int.Parse(wKCampaignId));

            _SQLfilterusername = " AND MemberName = '" + userName + "' ";
            _SQL = "SELECT * FROM " + customerTableName + " where Id = " + id + _SQLfilterusername;

            //_SQL = "SELECT @tableName := customerTableName from campaign WHERE Id =" + wKCampaignId + ";";
            //_SQL += "SET @s = CONCAT('SELECT * FROM ', @tableName,' where Id= "+ id +"' ";
            //_SQL += " );";
            //_SQL += "PREPARE stmt FROM @s; ";
            //_SQL += "EXECUTE stmt; ";
            //_SQL += "DEALLOCATE PREPARE stmt; ";

            conn = (MySqlConnection)_context.Database.GetDbConnection();
            //cmd = new MySqlCommand("Select* from Customer where Id=" + id, conn);
            cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);
            conn.Close();
            customer = new Customer();
            if (ds.Tables[0].Rows.Count == 1)
            {
                var ret = new List<KeyValuePairCls>();
                foreach (DataColumn dc in ds.Tables[0].Rows[0].Table.Columns)
                {
                    KeyValuePairCls kvp = new KeyValuePairCls();
                    kvp.Key = dc.ColumnName;
                    kvp.Value = ds.Tables[0].Rows[0][dc.ColumnName].ToString();
                    ret.Add(kvp);
                    if (dc.ColumnName == "Status")
                        customer.Status = (int)ds.Tables[0].Rows[0][dc.ColumnName];
                }

                customer.KPV = ret;
            }
            
            return customer;
            //return data.OrderBy(d => d.Id).Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
        }

        public async Task<Customer> UpdateCustomer(string userName, string wKCampaignId, Customer model, int? selectedStatusId)
        {

             MySqlConnection conn;
            MySqlCommand cmd;
            MySqlDataAdapter da;
            String _SQL = "";
            String _SQLfilterusername = "";

            KeyValuePairCls kvp = model.KPV.Find(m => m.Key == "Id");
            string id = kvp.Value;
            
            string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(int.Parse(wKCampaignId));

            _SQLfilterusername = " AND MemberName = '" + userName + "' ";
            _SQL = "SELECT * FROM " + customerTableName + " where Id = " + id + _SQLfilterusername;
            //string _SQLUPDATEVALUES = "";

            //_SQL = "SELECT @tableName := customerTableName from campaign WHERE Id =" + wKCampaignId + ";";
            ////_SQL += "SET @s = CONCAT('update ', @tableName , 'set " + _SQLUPDATEVALUES +  "' ,' where Id= " + id + "' ";
            //_SQL += "SET @s = CONCAT('SELECT * FROM ', @tableName , ' where Id= " + id + "' ";
            //_SQL += " );";
            //_SQL += "PREPARE stmt FROM @s; ";
            //_SQL += "EXECUTE stmt; ";
            //_SQL += "DEALLOCATE PREPARE stmt; ";


            conn = (MySqlConnection)_context.Database.GetDbConnection();
            cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);

            //string tableName = ds.Tables[0].Rows[0][0].ToString();

            DataTable dt = ds.Tables[0];

            //no record to update
            if (dt.Rows.Count == 0)
                return null;
            DataRow drRow  = dt.Rows[0];

            drRow["Status"] = selectedStatusId;
            drRow["UpdatedDate"] = DateTime.Now; //.UtcNow;
            drRow["UpdatedBy"] = userName;
            //dt.AcceptChanges();

            da = new MySqlDataAdapter(cmd);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(da);
            da.UpdateCommand =  builder.GetUpdateCommand();
            da.Update(dt); 
                   
            conn.Close();
            
            return model;
        }

        public async Task<Customer> UpdateStatus(string wKCampaignId, int id, Customer model, int selectedStatusId)
        {

            //foreach (string tableName in new[] { "Table1", "Table2" })
            //{
            //    var result = _context.Database.SqlQuery<Customer>(string.Format("SELECT * FROM {0} WHERE ID=@p0", tableName), 1).FirstOrDefault();
            //}
            //var results = _context.Set<Customer>()..AsQueryable().Where("SomeProperty > @1 and SomeThing < @2", aValue, anotherValue);
            //ModelBuilder modelBuilder = new ModelBuilder();
            //modelBuilder.Entity<Customer>().ToTable("Course");

            var customer = await _context.Customer.SingleOrDefaultAsync(m => m.Id == id);
            customer.Status = selectedStatusId; // model.Status;
            await _context.SaveChangesAsync();
            return model;
        }

    }
}