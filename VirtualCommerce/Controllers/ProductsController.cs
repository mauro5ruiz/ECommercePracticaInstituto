using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VirtualCommerce.Classes;
using VirtualCommerce.Models;
using VirtualCommerce.ViewModels;

namespace VirtualCommerce.Controllers
{
    [Authorize(Roles = "User")]
    public class ProductsController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        private List<Product> _personas;

        [HttpPost]
        public PartialViewResult CambioSlider(int min, int max)
        {
            using (VirtualCommerceDbContext db = new VirtualCommerceDbContext())
            {
                var query = db.Products.Where(x => x.Price >= min);
                if (max < 61)
                {
                    query = query.Where(x => x.Price <= max);
                }

                var personas = query.ToList();

                return PartialView("_personas", personas);
            }
        }

        // GET: Products
        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var products = db.Products
                .Include(p => p.Category).
                Include(p => p.Company)
                .Where(p => p.CompanyId == user.CompanyId);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        public ActionResult Create()
        {

            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var productViewModel = new ProductViewModel
            {
                CompanyId = user.CompanyId

            };
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description");
            //ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name");
            return View(productViewModel);
        }

        // POST: Products/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel productViewModel)
        {
           
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {

                    try
                    {
                        var product = ToModel(productViewModel);
                        db.Products.Add(product);

                        db.SaveChanges();
                        if (productViewModel.ImageFile != null)
                        {
                            var folder = "~/Content/Products";
                            var file = $"{product.ProductId}.jpg";

                            var response = FileHelper.UploadPhoto(productViewModel.ImageFile, folder, file);
                            if (response)
                            {
                                var pic = $"{folder}/{file}";
                                product.Image = pic;
                                db.Entry(product).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                        }
                        transaction.Commit();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {

                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(productViewModel.CompanyId), "CategoryId", "Description", productViewModel.CategoryId);
                        

                        return View(productViewModel);
                    }
                }

            }

            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(productViewModel.CompanyId), "CategoryId", "Description", productViewModel.CategoryId);
          
            return View(productViewModel);
        }

        private Product ToModel(ProductViewModel productViewModel)
        {
            return new Product
            {
                ProductId = productViewModel.ProductId,
                CompanyId = productViewModel.CompanyId,
                Description = productViewModel.Description,
                CategoryId = productViewModel.CategoryId,
                Image = productViewModel.Image,
                Price = productViewModel.Price,
                Remarks = productViewModel.Remarks
            };
        }

        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {

            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            var productViewModel = ToViewModel(product);
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description", product.CategoryId);
            return View(productViewModel);
        }

        private ProductViewModel ToViewModel(Product product)
        {
            return new ProductViewModel
            {
                ProductId = product.ProductId,
                CompanyId = product.CompanyId,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Image = product.Image,
                Price = product.Price,
                Remarks = product.Remarks

            };
        }

        // POST: Products/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel productViewModel)
        {
      
            if (ModelState.IsValid)
            {
                try
                {
                    var product = ToModel(productViewModel);
                    if (productViewModel.ImageFile != null)
                    {
                        var pic = string.Empty;
                        var folder = "~/Content/Products";
                        var file = $"{product.ProductId}.jpg";
                        var response = FileHelper.UploadPhoto(productViewModel.ImageFile, folder, file);

                        if (response)
                        {
                            pic = $"{folder}/{file}";
                            product.Image = pic;
                        }
                    }
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {


                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("IX"))
                    {
                        ModelState.AddModelError(string.Empty, "The field already exists");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);
                    }
                }

            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(productViewModel.CompanyId), "CategoryId", "Description", productViewModel.CategoryId);
            return View(productViewModel);
        }

        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
