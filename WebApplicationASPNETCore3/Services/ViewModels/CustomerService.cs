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
        Task<CustomerPagination> GetPaginatedResult(string userName, string role, string wKCampaignId, String searchList, int currentPage, int pageSize = 10);
      
        Task<Customer> GetCustomerById(string userName, string wKCampaignId, int id);

        Task<Customer> UpdateCustomer(string userName, string wKCampaignId, Customer model, int? selectedStatusId);
    }
    public class CustomerService : ICustomerService
    {
        private readonly CRMContext _context;
        private readonly CampaignService _campaignService;
        private readonly UserService _userService;

        public CustomerService(CRMContext context, CampaignService campaignService, UserService userService)
        {
            _context = context;
            _campaignService = campaignService;
            _userService = userService;
           
        }

        public  async Task<CustomerPagination> GetPaginatedResult(string userName, String role, string wKCampaignId, String searchString, int currentPage, int pageSize = 10)
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
            //TODO: if tmember login is filter by tmember, but if admin or Team lead member ??
            if (string.IsNullOrEmpty(_SQLfilterparams))
                _SQLfilterusername = " where ";
            else
                _SQLfilterusername = " AND ";
            switch (role.ToUpperInvariant())
            {
                case "ADMIN":
                    //no filter
                    _SQLfilterusername =  ""; //reset where clause or logical AND
                    break;
                case "LEADER":
                    _SQLfilterusername += " TLeadName = '" + userName + "' ";
                    break;
                case "AGENT":
                    _SQLfilterusername += " TMemberName = '" + userName + "' ";
                    break;
                default:
                    _SQLfilterusername += " TLeadName is null and TMemberName is null ";
                    break;
            }

            

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

           // _SQLfilterusername = " AND MemberName = '" + userName + "' ";
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

            //_SQLfilterusername = " AND MemberName = '" + userName + "' ";
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

       


        public async Task<TeamDistributionViewModel> GetTeamDistributionList(string userName, int campaignId, string role)
        {
           TeamDistributionViewModel model = new TeamDistributionViewModel();
            List<TeamDistribution> teamdists = new List<TeamDistribution>();
            MySqlConnection conn;
            MySqlCommand cmd;
            String _SQL = "";
            String _SQLfilterusername = "";
            String _SQLgrouping = "";
            String _SQLprojection = "";
            String _SQLfiltertotalquantity = "";
            //String _SQLfilterunusedquantity = "";
            Int64 accTotal = 0;

            string customerTableName = await  _campaignService.GetCustomerTableNameByCampaignId(campaignId);

            switch (role.ToUpperInvariant())
            {
                case "ADMIN":
                    //no filter
                    _SQLfilterusername = " where TLeadName is not null ";  
                    _SQLgrouping = " group by TLeadId, TLeadName";
                    _SQLprojection = "  TLeadId, TLeadName , count(*) as Quantity ";
                    _SQLfiltertotalquantity = "";  // no filter , all records
                    //_SQLfilterunusedquantity = " where TLeadId is null ";
                    break;
                case "LEADER":
                    _SQLfilterusername = " where TLeadName = '" + userName + "' AND TMemberName is not null";
                    _SQLgrouping = " group by TMemberId, TMemberName";
                    _SQLprojection = "  TMemberId, TMemberName, count(*) as Quantity  ";
                    _SQLfiltertotalquantity = " where TLeadName = '" + userName + "'";
                    //_SQLfilterunusedquantity = " where TMemberId is null ";
                    break;
                default:
                    _SQLfilterusername = " where 1 = 2 ";  // select nothing
                    _SQLfiltertotalquantity = " where 1 = 2 ";
                    //_SQLfilterunusedquantity = " where 1 = 2 ";
                    break;
            }

            _SQL = "SELECT " + _SQLprojection  + " FROM " + customerTableName +  _SQLfilterusername + _SQLgrouping + ";";
            _SQL += "SELECT count(*) FROM " + customerTableName + _SQLfiltertotalquantity + ";"; //total count for the user login
           // _SQL += "SELECT count(*) FROM " + customerTableName + _SQLfilterunusedquantity + ";"; //TODO: may not need this request, use totalquantity - total used


            conn = (MySqlConnection)_context.Database.GetDbConnection();
            cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);
            conn.Close();
            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    TeamDistribution td = new TeamDistribution()
                    {
                        Quantity = (Int64)row["Quantity"],
                        TLeadName = role.ToUpperInvariant() == "ADMIN" ? row["TLeadName"].ToString() : null,
                        TMemberName = role.ToUpperInvariant() == "LEADER" ? row["TMemberName"].ToString() : null,
                    };
                    switch (role.ToUpperInvariant())
                    {
                        case "ADMIN":
                            td.TId = (int) row["TLeadId"];
                            td.TName =  row["TLeadName"].ToString();
                            td.TLevel = TeamDistributionTLevel.L;
                            break;
                        case "LEADER":
                            td.TId = (int)row["TMemberId"];
                            td.TName = row["TMemberName"].ToString();
                            td.TLevel = TeamDistributionTLevel.M;
                            break;
                        default:
                            break;
                    }

                    teamdists.Add(td);
                    accTotal += td.Quantity;
                }
                model.TeamDistributions = teamdists;


               model.TotalQuantity = (Int64)ds.Tables[1].Rows[0][0];
                //model.UnusedQuantity = (Int64)ds.Tables[2].Rows[0][0];
                //model.UsedQuantity = model.TotalQuantity - model.UnusedQuantity;
                model.UsedQuantity = accTotal;
                model.UnusedQuantity = model.TotalQuantity - model.UsedQuantity;
                model.UsedQuantityPerc =  ( model.UsedQuantity * 100 ) / model.TotalQuantity ;
            }
            
            return model;
           
        }
        public async Task<TeamDistribution> GetTeamDistribution(string userName, int campaignId, string role)
        {
            TeamDistribution model = new TeamDistribution();
            //MySqlConnection conn;
            //MySqlCommand cmd;
            //String _SQL = "";
            //String _SQLfilterusername = "";
            //String _SQLgrouping = "";
            //String _SQLprojection = "";

            //string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(campaignId);
            
            //switch (role.ToUpperInvariant())
            //{
            //    case "ADMIN":
            //        //no filter
            //        _SQLfilterusername = " where TLeadName is not null "; //reset where clause or logical AND
            //        _SQLgrouping = " group by TLeadId, TLeadName";
            //        _SQLprojection = "  TLeadId, TLeadName , count(*) as Quantity ";
                   
            //        break;
            //    case "LEADER":
            //        _SQLfilterusername += " where TLeadName = '" + userName + "' AND TMemberName is not null";
            //        _SQLgrouping = " group by TMemberId, TMemberName";
            //        _SQLprojection = "  TMemberId, TMemberName, count(*) as Quantity  ";
                   
            //        break;
            //    default:
            //        _SQLfilterusername += " where 1 = 2 ";  // select nothing
            //        break;
            //}

            //_SQL = "SELECT " + _SQLprojection + " FROM " + customerTableName + _SQLfilterusername + _SQLgrouping + ";";


            //conn = (MySqlConnection)_context.Database.GetDbConnection();
            //cmd = new MySqlCommand(_SQL, conn);
            //conn.Open();
            //MySqlDataAdapter da = new MySqlDataAdapter(cmd);
            //System.Data.DataSet ds = new System.Data.DataSet();
            //da.Fill(ds);
            //conn.Close();
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    foreach (DataRow row in ds.Tables[0].Rows)
            //    {
            //        teamdists.Add(new TeamDistribution()
            //        {
            //            Quantity = (Int64)row["Quantity"],
            //            TLeadName = role.ToUpperInvariant() == "ADMIN" ? row["TLeadName"].ToString() : null,
            //            TMemberName = role.ToUpperInvariant() == "LEADER" ? row["TMemberName"].ToString() : null,


            //        });
            //    }
            //    model.TeamDistributions = teamdists;


            //    model.TotalQuantity = (Int64)ds.Tables[1].Rows[0][0];
            //    model.UnusedQuantity = (Int64)ds.Tables[2].Rows[0][0];
            //    model.UsedQuantity = model.TotalQuantity - model.UnusedQuantity;
            //    model.UsedQuantityPerc = (model.UsedQuantity * 100) / model.TotalQuantity;
            //}

            return model;

        }
        //public async Task<TeamDistribution> CreateTeamDistribution(string userName, int campaignId, TeamDistribution model)
        //{
        //    return await UpdateTeamDistribution(userName, campaignId, model, DataAction.C);
        //}
        //public async Task<TeamDistribution> DeleteTeamDistribution(string userName, int campaignId, TeamDistribution model)
        //{
        //    return await UpdateTeamDistribution(userName, campaignId, model, DataAction.D);
        //}

        public async Task<TeamDistribution> UpdateTeamDistribution(string userName, int campaignId, TeamDistribution model)
        {
            
            MySqlConnection conn;
            MySqlCommand cmd;
            MySqlDataAdapter da;
            String _SQL = "";
            String tName = "";
            int tId = model.TId;
            Int64 tQty = model.Quantity;

            string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(campaignId);

            User user = await _userService.GetUserById(model.TId);
            tName = user.Email;
           
            switch (model.TLevel)
            {
                case TeamDistributionTLevel.L:
                    // _SQL = "UPDATE " + customerTableName + " SET TLeadId=" + model.TId + " , TLeadName='" + model.TName + "'  Where TLeadId is null and TLeadName is null limit "+ tQty.ToString();
                    _SQL = "SELECT * FROM " + customerTableName + " Where TLeadId is null and TLeadName is null LIMIT " + tQty.ToString();
                    break;
                case TeamDistributionTLevel.M:
                    // _SQL = "UPDATE " + customerTableName + " SET TMemberId=" + model.TId + " , TMemberName='" + model.TName + "'  Where TMemberId is null and TMemberName is null limit " + tQty.ToString();
                    _SQL = "SELECT * FROM " + customerTableName + " Where TMemberId is null and TMemberName is null LIMIT " + tQty.ToString();
                    break;
                default:
                    break;
            }
           

            conn = (MySqlConnection)_context.Database.GetDbConnection();
            cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);


            DataTable dt = ds.Tables[0];

            //no record to update
            if (dt.Rows.Count == 0)
                return null;



            //dt.AcceptChanges();
            DateTime updatedDate = DateTime.Now;
            
            
            switch (model.TLevel)
            {
                case TeamDistributionTLevel.L:
                    foreach (DataRow drRow in dt.Rows)
                    {
                        drRow["TLeadId"] = tId; 
                        drRow["TLeadName"] = tName; 
                        drRow["UpdatedDate"] = updatedDate; //.UtcNow;
                        drRow["UpdatedBy"] = userName;
                    }
                    break;
                case TeamDistributionTLevel.M:
                    foreach (DataRow drRow in dt.Rows)
                    {
                        drRow["TMemberId"] = tId; 
                        drRow["TMemberName"] = tName; 
                        drRow["UpdatedDate"] = updatedDate; //.UtcNow;
                        drRow["UpdatedBy"] = userName;
                    }
                    break;
                default:
                    break;
            }

            da = new MySqlDataAdapter(cmd);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(da);
            da.UpdateCommand = builder.GetUpdateCommand();
            da.Update(dt);

            conn.Close();

            return model;  //TODO: should return number of records updated?
        }

        public async Task<int> DeleteTeamDistribution(string userName, int campaignId, int id, TeamDistributionTLevel tLevel)
        {

            MySqlConnection conn;
            MySqlCommand cmd;
            MySqlDataAdapter da;
            String _SQL = "";

            string customerTableName = await _campaignService.GetCustomerTableNameByCampaignId(campaignId);

            switch (tLevel)
            {
                case TeamDistributionTLevel.L:
                    _SQL = "SELECT * FROM " + customerTableName + " Where TLeadId = " + id + " ";
                    break;
                case TeamDistributionTLevel.M:
                    _SQL = "SELECT * FROM " + customerTableName + "  Where TMemberId = " + id + " ";
                    break;
                default:
                    break;
            }


            conn = (MySqlConnection)_context.Database.GetDbConnection();
            cmd = new MySqlCommand(_SQL, conn);
            conn.Open();
            da = new MySqlDataAdapter(cmd);
            System.Data.DataSet ds = new System.Data.DataSet();
            da.Fill(ds);


            DataTable dt = ds.Tables[0];

            //no record to update
            if (dt.Rows.Count == 0)
                return 0;

            DateTime updatedDate = DateTime.Now;

            switch (tLevel)
            {
                case TeamDistributionTLevel.L:
                    foreach (DataRow drRow in dt.Rows)
                    {
                        drRow["TLeadId"] = DBNull.Value;
                        drRow["TLeadName"] = DBNull.Value;
                        drRow["UpdatedDate"] = updatedDate; //.UtcNow;
                        drRow["UpdatedBy"] = userName;
                    }
                    break;
                case TeamDistributionTLevel.M:
                    foreach (DataRow drRow in dt.Rows)
                    {
                        drRow["TMemberId"] = DBNull.Value;
                        drRow["TMemberName"] = DBNull.Value;
                        drRow["UpdatedDate"] = updatedDate; //.UtcNow;
                        drRow["UpdatedBy"] = userName;
                    }
                    break;
                default:
                    break;
            }

            da = new MySqlDataAdapter(cmd);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(da);
            da.UpdateCommand = builder.GetUpdateCommand();
            int nDeletedRecord = da.Update(dt);

            conn.Close();

            return nDeletedRecord;  //TODO: should return number of records updated?
        }

    }
}