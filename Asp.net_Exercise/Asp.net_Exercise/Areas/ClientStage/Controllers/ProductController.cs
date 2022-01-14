using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.net_Exercise.Areas.ClientStage.Controllers
{
    public class ProductController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        public ActionResult NewProd()
        {
            if (Convert.ToInt32(Session["Member"].ToString()) != 12 && Session["Member"] == null)//驗證是否為管理員
            {
                Session["Member"] = "";
                return RedirectToAction("Index","home");
            }

            return View();
        }

        public bool UpdataFile(HttpPostedFileBase file, string Type)
        {
            var E = file.FileName.Split('.');//分割字串
            string[] VD = { "jpg", "img", "png", "jpeg" };
            var bl = false;
            //驗證副檔名
            if (E.Length > 2)
            {
                return false;
            }
            else
            {
                foreach (var i in VD)
                {
                    if (E[1] == i)
                    {
                        bl = true;
                    }
                }
                if (bl == false)
                {
                    return false;
                }
                else
                {
                    var PH = Path.Combine(Server.MapPath("~/UpdataFiles/"), file.FileName);//編輯儲存路徑
                    file.SaveAs(PH);//執行儲存
                    var Img = new Img() { Type = Type, FileName = file.FileName };//新增至DB
                    DB.Img.Add(Img);
                    return true;
                }
            }
        }
        [HttpPost]
        public ActionResult NewProd(string name, int price, string type, string Class, HttpPostedFileBase previewed, HttpPostedFileBase title, HttpPostedFileBase content)
        {
            //驗證資料
            if (type == null && Class == null)
            {
                ViewBag.error = "請選擇種類和類別";
                return View();
            }
            if ((UpdataFile(title, "title") & UpdataFile(previewed, "previewed") & UpdataFile(content, "content")) == false)
            {
                ViewBag.error = "新增失敗,只接受圖檔並且檔名不可包含'.'請重新操作";
                return View();
            }
            //新增產品及類別資訊至DB
            var E = previewed.FileName.Split('.');
            var prod = new Product() { Name = name, Price = price };
            int tid = (int)Enum.Parse(typeof(TypeSelect), type);//通過列舉獲得選取的資料
            DB.Product.Add(prod);
            var PC = new Prod_Type() { Product = prod, Tid = tid };
            DB.Prod_Type.Add(PC);
            DB.SaveChanges();
            //新增圖檔和產品之間的連接資料(一對多其實不必額外開Table，多對多才需要)
            var P = DB.Product.Where(m => m.Name == name).FirstOrDefault();
            var Pid = P.Id;
            //三個圖檔用意是封面,預覽,內容
            var I = DB.Img.Where(m => m.FileName == title.FileName).FirstOrDefault();
            var Iid = I.Id;
            var M = DB.Img.Where(m => m.FileName == previewed.FileName).FirstOrDefault();
            var Mid = M.Id;
            var G = DB.Img.Where(m => m.FileName == content.FileName).FirstOrDefault();
            var Gid = G.Id;
            //建立關聯式data
            Prod_Img[] PM = new Prod_Img[3];
            for (int i = 0; PM.Length > i; i++)
            {
                PM[i] = new Prod_Img();
                PM[i].Pid = Pid;
                if (i == 0) { PM[i].Mid = Iid; }
                if (i == 1) { PM[i].Mid = Mid; }
                if (i == 2) { PM[i].Mid = Gid; }
                DB.Prod_Img.Add(PM[i]);
            }
            var p = PM;
            DB.SaveChanges();
            ViewBag.error = "新增成功";
            return View();
        }
    }
}