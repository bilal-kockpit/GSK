using AspNetCoreHero.ToastNotification.Abstractions;
using GSKPOC.Models;
using GSKPOC.ORM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace GSKPOC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly INotyfService _notyf;
        Data oData;
        DataSet LocalDS = new DataSet();
        string sConn = string.Empty;
        IConfiguration _conf;

        public HomeController(ILogger<HomeController> logger, INotyfService notyf, IConfiguration configuration)
        {
            this._conf = configuration;
            _logger = logger;
            _notyf = notyf;
            sConn = this._conf["ConnectionString"].ToString().Trim();
            oData = new Data(sConn, LocalDS);
        }

        public IActionResult Index()
        {
            ViewBag.Items = GetItems();
            ViewBag.Requests = GetRequest();
            return View();
        }

        public JsonResult SubmitRequest(string sRecord)
        {
            bool lretval = false;
            string msg = "";
            try
            {
                ItemPrice oModel = JsonConvert.DeserializeObject<ItemPrice>(sRecord);
                var query = $@"INSERT INTO ItemPrice(ItemId,CurrentPrice,NewPrice,IsSupervisorApproved,IsAdminApproved) 
                                VALUES(@ItemId, @CurrentPrice, @NewPrice, @IsSupervisorApproved, @IsAdminApproved) RETURNING WorkOrderNo;";
                Npgsql.NpgsqlParameter[] npgsqlParameters = {
                    new Npgsql.NpgsqlParameter("@ItemId",oModel.ItemId),
                    new Npgsql.NpgsqlParameter("@CurrentPrice",oModel.CurrentPrice),
                    new Npgsql.NpgsqlParameter("@NewPrice",oModel.NewPrice),
                    new Npgsql.NpgsqlParameter("@IsSupervisorApproved",DBNull.Value),
                    new Npgsql.NpgsqlParameter("@IsAdminApproved",DBNull.Value),
                };

                oData.ExecuteSelectCommand(query, npgsqlParameters, "tItemPrice");
                if (this.LocalDS.Tables["tItemPrice"] != null
                    && this.LocalDS.Tables["tItemPrice"].Rows.Count > 0)
                {
                    lretval = true;
                    msg = this.LocalDS.Tables["tItemPrice"].Rows[0][0].ToString().Trim();
                }
                _notyf.Success("Record added successfully");
            }
            catch (Exception ex)
            {
                msg = $"Error : {ex.Message}";
            }
            return Json(new Tuple<bool, string>(lretval, msg));
        }

        public IActionResult Remove(int OrderNo)
        {
            try
            {
                var query = $@"DELETE FROM ItemPrice WHERE WorkOrderNo = @WorkOrderNo";
                Npgsql.NpgsqlParameter[] npgsqlParameters = {
                new Npgsql.NpgsqlParameter("@WorkOrderNo",OrderNo),
                };

                var res = oData.ExecuteNonQuery(query, npgsqlParameters);
                if (res)
                    TempData["success"] = "Request Removed";
            }
            catch (Exception ex)
            {
                TempData["error"] = $"Error : {ex.Message}";
            }
            return RedirectToAction("Index");
        }



        List<Item> GetItems()
        {
            var query = "SELECT * FROM Item";
            oData.ExecuteSelectCommand(query, "tItems");
            if(this.LocalDS.Tables["tItems"]!= null
                && this.LocalDS.Tables["tItems"].Rows.Count > 0)
            {
                return this.LocalDS.Tables["tItems"].AsEnumerable().Select(r => new Item {
                    ItemId = Convert.ToInt32(r["ItemId"].ToString()),
                    ItemName = r.Field<string>("ItemName"),
                    Price = Convert.ToDecimal(r["Price"])
                }).ToList();
            }
            else
            {
                return null;
            }
        }

        List<ItemPrice> GetRequest()
        {
            var query = $@"SELECT a.*, b.ItemName 
                            FROM ItemPrice a
                            JOIN Item b ON a.ItemId = b.ItemId";

            oData.ExecuteSelectCommand(query, "tItemRequest");
            if (this.LocalDS.Tables["tItemRequest"] != null
                && this.LocalDS.Tables["tItemRequest"].Rows.Count > 0)
            {
                return this.LocalDS.Tables["tItemRequest"].AsEnumerable().Select(r => new ItemPrice
                {
                    WorkOrderNo = Convert.ToInt32(r["WorkOrderNo"].ToString()),
                    ItemId = Convert.ToInt32(r["ItemId"].ToString()),
                    ItemName = r.Field<string>("ItemName"),
                    CurrentPrice = Convert.ToDecimal(r["CurrentPrice"]),
                    NewPrice = Convert.ToDecimal(r["NewPrice"]),
                }).ToList();
            }
            else
            {
                return null;
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
