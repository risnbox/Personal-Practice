using Asp.net_Exercise.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Asp.net_Exercise.Controllers
{
    public class ProductController : Controller
    {
        DatabaseEntities DB = new DatabaseEntities();
        public ActionResult NewProd()
        {
            if (Convert.ToInt32(Session["Member"].ToString()) != 12 && Session["Member"] == null)
            {
                Session["Member"] = "";
                return RedirectToAction("Index","home");
            }

            return View();
        }

        public bool UpdataFile(HttpPostedFileBase file, string Type)
        {
            var E = file.FileName.Split('.');//分割字串
            string[] VD = { "jpg", "img", "png", "jpeg" };//驗證副檔名
            var bl = false;
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
                    var Img = new Img() { Type = Type, FileName = file.FileName };
                    DB.Img.Add(Img);
                    return true;
                }
            }
        }
        [HttpPost]
        public ActionResult NewProd(string name, int price, string type, string Class, HttpPostedFileBase previewed, HttpPostedFileBase title, HttpPostedFileBase content)
        {
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
            var E = previewed.FileName.Split('.');
            var prod = new Product() { Name = name, Price = price };
            var cid = (int)Enum.Parse(typeof(ClassSelect), Class);
            int tid = (int)Enum.Parse(typeof(TypeSelect), type);
            DB.Product.Add(prod);
            var PC = new Prod_Class_Type() { Cid = cid, Product = prod, Tid = tid };
            DB.Prod_Class_Type.Add(PC);
            DB.SaveChanges();
            var P = DB.Product.Where(m => m.Name == name).FirstOrDefault();
            var Pid = P.Id;
            var I = DB.Img.Where(m => m.FileName == title.FileName).FirstOrDefault();
            var Iid = I.Id;
            var M = DB.Img.Where(m => m.FileName == previewed.FileName).FirstOrDefault();
            var Mid = M.Id;
            var G = DB.Img.Where(m => m.FileName == content.FileName).FirstOrDefault();
            var Gid = G.Id;
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
        public string SearchProd(string Stype, string SClass)
        {
            var data = (from PCT in DB.Prod_Class_Type
                        join C in DB.Class on PCT.Class.ClassName equals SClass
                        join T in DB.Type on PCT.Type.TypeName equals Stype
                        join P in DB.Product on PCT.Pid equals P.Id
                        where PCT.Cid == C.Id && PCT.Tid == T.Id && PCT.Pid == P.Id
                        select new
                        {
                            Prod = PCT.Product,
                            Clas = PCT.Class,
                            type = PCT.Type
                        }
                        ).ToList();

            var json = JsonConvert.SerializeObject(data);
            return json;
        }

        public int DelProd(string name)
        {
            int? data = null;
            foreach (var i in DB.Prod_Class_Type)
            {
                if (i.Product.Name == name)
                {
                    data = i.Pid;
                }
            }
            var Delete1 = DB.Product.Where(m => m.Id == data).FirstOrDefault();
            var Delete2 = DB.Prod_Class_Type.Where(m => m.Pid == data).FirstOrDefault();
            try
            {
                DB.Product.Remove(Delete1);
                DB.Prod_Class_Type.Remove(Delete2);
                DB.SaveChanges();
                return 1;
            }
            catch
            {
                return 0;
            }
        }

    }
}