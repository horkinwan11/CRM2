using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

using CRM.Models.Entities;
using CRM.Models.ViewModels;
using CRM.Services;
using CRM.Services.ViewModels;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.AspNetCore.Authorization;
using System.IO;
using MySql.Data.MySqlClient;

namespace CRM.Controllers
{
    //[EZAuth(permissions: "Agent")]
    public class CustomerFileUploadController : Controller
    {
        private readonly CRMContext _context;
        private readonly EZAuth _ezAuth;
        private readonly EZSession _ezSession;

        public CustomerFileUploadController(EZAuth ezAuth, EZSession ezSession, CRMContext context)
        {
            _context = context;
            _ezAuth = ezAuth;
            _ezSession = ezSession;
        }

        public IActionResult Index(bool hasError, string tabId, int campaignId, string campaignName)
        {
            ViewBag.tabId = String.IsNullOrEmpty(tabId) ? "0" : tabId;
            ViewBag.CampaignId = campaignId;
            ViewBag.CampaignName = campaignName;
            ViewData["Title"] = "Campaign: " + campaignName;

            CustomerFileUploadViewModel model = new CustomerFileUploadViewModel { FileAttach = null, Data = new System.Data.DataTable() };
            model.CampaignId = campaignId;
            try
            {

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            // Info.
            return this.View(model);
            
        }
        private static string connStr = "Server=localhost;Database=crm;Uid=root;Pwd=1q2w3e4r!;AllowLoadLocalInfile=true";

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        //upload csv with SQLBulkLoad and create table based on csv column names
        public async Task<ActionResult> Index(CustomerFileUploadViewModel model)
        {
            //CSV FIle upload
            //try
            //{
            // Verification
            if (ModelState.IsValid)
            {
                // Initialization.
                string folderPath = "~/Content/temp_upload_files/";
                string filename = "download.csv";

                // Initialization. 
                string importFilePath = string.Empty;
                string exportFilePath = string.Empty;

                if (model.FileAttach == null || model.FileAttach.Length == 0)
                    return Content("file not selected");

                //var path = Path.Combine(
                //            Directory.GetCurrentDirectory(), "\\wwwroot\\Content\\temp_upload_files",
                //            model.FileAttach.FileName);

                var path = Directory.GetCurrentDirectory() + "\\wwwroot\\Content\\temp_upload_files\\" +
                            model.FileAttach.FileName;

                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await model.FileAttach.CopyToAsync(stream);
                }

                
                importFilePath = path;
                string headerRow = String.Empty;
                using (StreamReader sr = System.IO.File.OpenText(importFilePath))
                {
                    headerRow = sr.ReadLine();
                }
                String[] headerList = headerRow.Split(',');


                MySqlConnection conn = new MySqlConnection(connStr);
                String sCreateTable = "CREATE  TABLE IF NOT EXISTS Customer_" + model.CampaignId.ToString() + " ( ";
                //sCreateTable += " ID int NOT NULL, ";
                List<String> headerListWOQ = new List<String>();

                for (int i = 0; i < headerList.Length; i++)
                {
                    headerListWOQ.Add(headerList[i].Trim('\"'));
                    sCreateTable += "`" + headerList[i].Trim('\"') + "`" + " varchar(255) ";
                    if (i != headerList.Length - 1)
                        sCreateTable += ",";

                }
                //sCreateTable += " PRIMARY KEY(ID) ";
                sCreateTable += " ); update campaign set customerTableName='Customer_" + model.CampaignId.ToString()  + "' WHERE Id=" + model.CampaignId.ToString();

                //string json = JsonSerializer.Serialize(headerListWOQ);

                MySqlBulkLoader bl = new MySqlBulkLoader(conn);
                bl.Local = true;
                bl.TableName = "Customer_" + model.CampaignId.ToString();
                bl.FieldTerminator = ",";
                bl.LineTerminator = "\r\n";
                bl.FieldQuotationCharacter = '"';
                bl.FieldQuotationOptional = false;
                bl.EscapeCharacter = '\\';
                // bl.Expressions.Add("created_at = utc_timestamp()");
                bl.FileName = importFilePath;
                bl.NumberOfLinesToSkip = 1;
                //bl.Columns.AddRange(new string[] { "first_name", "last_name", "email" });
                Console.WriteLine("Connecting to MySQL...");
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(sCreateTable, conn);
                cmd.ExecuteNonQuery();

                //cmd.CommandText = sqlstring;
                //cmd.ExecuteNonQuery();

                //Upload data from file
                int count = bl.Load();
                Console.WriteLine(count + " lines uploaded.");

               
                if (count > 0)
                {
                    String sIdColumn = "SELECT count(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE ";
                    sIdColumn += "table_name = 'customer_" + model.CampaignId.ToString() + "'";
                    sIdColumn += "AND table_schema = 'crm'"; //TODO: change to parameterised
                    sIdColumn += "AND column_name = 'Id'";

                    cmd = new MySqlCommand(sIdColumn, conn);
                    int idColumnExist = (int)(long)(cmd.ExecuteScalar());
                   
                    if (idColumnExist == 0 )
                    { 
                        String sAlterTable = "ALTER TABLE Customer_" + model.CampaignId.ToString() + "  ";
                        sAlterTable += " ADD COLUMN Id int NOT NULL AUTO_INCREMENT PRIMARY KEY,";
                        sAlterTable += " ADD COLUMN CampaignId int,";
                        sAlterTable += " ADD COLUMN PackageId int,";
                        sAlterTable += " ADD COLUMN TLeadId int,";
                        sAlterTable += " ADD COLUMN TMemberId int,";
                        sAlterTable += " ADD COLUMN TLeadName VARCHAR(50),";
                        sAlterTable += " ADD COLUMN TMemberName VARCHAR(50),";
                        sAlterTable += " ADD COLUMN Created DateTime,";
                        sAlterTable += " ADD COLUMN CreatedBy  VARCHAR(50),";
                        sAlterTable += " ADD COLUMN Updated DateTime,";
                        sAlterTable += " ADD COLUMN UpdatedBy VARCHAR(50),";
                        sAlterTable += " ADD COLUMN Status int ";
                        sAlterTable += " ;";

                        cmd = new MySqlCommand(sAlterTable, conn);
                        cmd.ExecuteNonQuery();
                    }
                }
                

                conn.Close();
                //}
                //catch (MySqlException ex)
                //{
                //    Console.WriteLine(ex.ToString());
                //}
                // Deleting Extra files.
                System.IO.File.Delete(importFilePath);
                Console.WriteLine("Done.");

            }
            return this.View(model);
        }

    }

}