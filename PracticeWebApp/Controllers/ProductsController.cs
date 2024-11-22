using PracticeWebApp.Models;
using PracticeWebApp.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PracticeWebApp.Controllers
{
    public class ProductsController : Controller
    {
        readonly ProductDBContext db=new ProductDBContext();
        // GET: Products
        public ActionResult Index()
        {
            var data = db.Products.ToList();
            return View(data);
        }
        public ActionResult Create()
        {
            var data = new ProductInputModel();
            data.Sales.Add(new Sale { });
            ViewBag.Sellers=db.Sellers.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Create(ProductInputModel model, string operation="")
        {
            if (operation == "add")
            {
                model.Sales.Add(new Sale { });
                ModelState.Clear();
            }
            if (operation.StartsWith("del"))
            {
                int index= int.Parse(operation.Split('_')[1]);
                model.Sales.RemoveAt(index);
                ModelState.Clear();
            }
            if(ModelState.IsValid && operation ==("insert"))
            {
                var p = new Product
                {
                    ProductName = model.ProductName,
                    Price = model.Price,
                    MfgDate = model.MfgDate,
                    InStock = model.InStock
                };
                string ext= Path.GetExtension(model.Picture.FileName);
                string f=Path.GetFileNameWithoutExtension(Path.GetRandomFileName())+ext;
                string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                model.Picture.SaveAs(savePath);
                p.Picture = f;
                foreach (var s in model.Sales) 
                { 
                    p.Sales.Add(new Sale { Date = s.Date,SellerId=s.SellerId });
                }
               db.Products.Add(p);
                db.SaveChanges();
                RedirectToAction("Index");
            }
           
            ViewBag.Sellers = db.Sellers.ToList();
            return View(model);
        }


        public ActionResult Edit(int id)
        {
            var p = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (p == null) return new HttpNotFoundResult();
            var data = new ProductEditModel
            {
                ProductId=p.ProductId,
                ProductName = p.ProductName,
                Price = p.Price,
                MfgDate = p.MfgDate,
                InStock = p.InStock.Value
            };
            data.Sales = p.Sales.ToList();

            ViewBag.CurrentPic = p.Picture;
            ViewBag.Sellers = db.Sellers.ToList();
            return View(data);
        }
        [HttpPost]
        public ActionResult Edit(ProductEditModel model, string operation = "")
        {
            var p = db.Products.FirstOrDefault(x => x.ProductId == model.ProductId);
            if (p == null) return new HttpNotFoundResult();
            if (operation == "add")
            {
                model.Sales.Add(new Sale { });
                ModelState.Clear();
            }
            if (operation.StartsWith("del"))
            {
                int index = int.Parse(operation.Split('_')[1]);
                model.Sales.RemoveAt(index);
                ModelState.Clear();
            }
            if (ModelState.IsValid && operation == ("update"))
            {


                p.ProductName = model.ProductName;
                   p. Price = model.Price;
                   p. MfgDate = model.MfgDate;
                   p. InStock = model.InStock;

                if (model.Picture != null)
                {
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Pictures"), f);
                    model.Picture.SaveAs(savePath);
                    p.Picture = f;
                }
                db.Database.ExecuteSqlCommand($"Delete Sales where productid={p.ProductId}");
                foreach (var s in model.Sales)
                {
                    db.Sales.Add(new Sale { Date = s.Date, SellerId = s.SellerId, ProductId=p.ProductId });
                }
                
                db.SaveChanges();
               return RedirectToAction("Index");
            }
            ViewBag.CurrentPic = p.Picture;
            ViewBag.Sellers = db.Sellers.ToList();

            return View(model);
        }

        public ActionResult Delete(int id)
        {
            var p=db.Products.FirstOrDefault(x=> x.ProductId==id);
            if (p==null) return new HttpNotFoundResult();
            return View(p);
        }
        [HttpPost,ActionName("Delete")]
        public ActionResult DoDelete(int id)
        {
            var p = db.Products.FirstOrDefault(x => x.ProductId == id);
            if (p == null) return new HttpNotFoundResult();
            db.Sales.RemoveRange(p.Sales.ToList());
            db.Products.Remove(p);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}