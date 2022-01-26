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
    public class PurchasesController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Purchases
        public ActionResult Index()
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var purchases = db.Purchases
                .Include(p => p.Company)
                .Include(p => p.Supplier)
                .Include(p => p.Warehouse)
                .Where(p => p.CompanyId == user.CompanyId);

            return View(purchases.ToList());
        }

        // GET: Purchases/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // GET: Purchases/Create
        public ActionResult Create()
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == user.UserName).ToList();

            var purchaseViewModel = new PurchaseViewModel
            {
                Date = DateTime.Now,
                Details = details,
            };

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSuppliers(user.CompanyId), "SupplierId", "FullName");
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(user.CompanyId), "WarehouseId", "Name");
            return View(purchaseViewModel);
        }

        // POST: Purchases/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create (PurchaseViewModel purchaseViewModel)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Purchases.Add(purchase);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", purchase.CompanyId);
            //ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "UserName", purchase.SupplierId);
            //ViewBag.WarehouseId = new SelectList(db.Warehouses, "WarehouseId", "Name", purchase.WarehouseId);
            //return View(purchase);

            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var details = db.PurchaseDetailTmps.Where(pdt => pdt.UserName == user.UserName).ToList();
            purchaseViewModel.Details = details;
            if (details.Count > 0)
            {
                if (ModelState.IsValid)
                {
                    using (var tran = db.Database.BeginTransaction())
                    {
                        try
                        {

                            var purchase = new Purchase
                            {
                                CompanyId = user.CompanyId,
                                Date = purchaseViewModel.Date,
                                Remarks = purchaseViewModel.Remarks,
                                SupplierId = purchaseViewModel.SupplierId,
                                WarehouseId = purchaseViewModel.WarehouseId
                            };
                            db.Purchases.Add(purchase);
                            db.SaveChanges();
                            foreach (var item in purchaseViewModel.Details)
                            {
                                var product = db.Products.Find(item.ProductId);
                                var purchaseDetail = new PurchaseDetail
                                {
                                    Description = product.Description,
                                    Price = item.Price,
                                    ProductId = item.ProductId,
                                    PurchaseId = purchase.PurchaseId,
                                    Quantity = item.Quantity,
                                    //TaxRate = product.Tax.Rate

                                };
                                db.PurchaseDetails.Add(purchaseDetail);
                                var inventory = db.Inventories
                                    .FirstOrDefault(i => i.WarehouseId == purchaseViewModel.WarehouseId && i.ProductId == item.ProductId);
                                if (inventory == null)
                                {
                                    inventory = new Inventory
                                    {
                                        ProductId = item.ProductId,
                                        WarehouseId = purchaseViewModel.WarehouseId,
                                        Stock = item.Quantity
                                    };
                                    db.Inventories.Add(inventory);
                                }
                                else
                                {
                                    inventory.Stock += item.Quantity;
                                    db.Entry(inventory).State = EntityState.Modified;
                                }
                                db.PurchaseDetailTmps.Remove(item);
                            }

                            db.SaveChanges();
                            tran.Commit();
                            return RedirectToAction("Index");


                        }
                        catch (Exception ex)
                        {
                            tran.Rollback();
                            ModelState.AddModelError(string.Empty, ex.Message);
                            ViewBag.SupplierId = new SelectList(CombosHelper.GetSupplier(user.CompanyId), "SupplierId", "FullName", purchaseViewModel.SupplierId);
                            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(user.CompanyId), "WarehouseId", "Name", purchaseViewModel.WarehouseId);
                            return View(purchaseViewModel);
                        }
                    }

                }

            }
            else
            {
                ModelState.AddModelError(string.Empty, "No items added yet");
                ViewBag.SupplierId = new SelectList(CombosHelper.GetSupplier(user.CompanyId), "SupplierId", "FullName", purchaseViewModel.SupplierId);
                ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(user.CompanyId), "WarehouseId", "Name", purchaseViewModel.WarehouseId);
                return View(purchaseViewModel);
            }

            ViewBag.SupplierId = new SelectList(CombosHelper.GetSupplier(user.CompanyId), "SupplierId", "FullName", purchaseViewModel.SupplierId);
            ViewBag.WarehouseId = new SelectList(CombosHelper.GetWarehouses(user.CompanyId), "WarehouseId", "Name", purchaseViewModel.WarehouseId);
            return View(purchaseViewModel);
        }

        // GET: Purchases/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", purchase.CompanyId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "UserName", purchase.SupplierId);
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "WarehouseId", "Name", purchase.WarehouseId);
            return View(purchase);
        }

        // POST: Purchases/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "PurchaseId,CompanyId,SupplierId,WarehouseId,Date,Remarks")] Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                db.Entry(purchase).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", purchase.CompanyId);
            ViewBag.SupplierId = new SelectList(db.Suppliers, "SupplierId", "UserName", purchase.SupplierId);
            ViewBag.WarehouseId = new SelectList(db.Warehouses, "WarehouseId", "Name", purchase.WarehouseId);
            return View(purchase);
        }

        // GET: Purchases/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Purchase purchase = db.Purchases.Find(id);
            if (purchase == null)
            {
                return HttpNotFound();
            }
            return View(purchase);
        }

        // POST: Purchases/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Purchase purchase = db.Purchases.Find(id);
            db.Purchases.Remove(purchase);
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

        public ActionResult AddProduct()
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Index", "Home");
            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description");
            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(0), "ProductId", "Description");

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(AddProductViewModel newProduct)
        {
            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (ModelState.IsValid)
            {
                var product = db.Products.Find(newProduct.ProductId);
                var tmp = db.PurchaseDetailTmps
                    .FirstOrDefault(pdt => pdt.UserName == user.UserName && pdt.ProductId == newProduct.ProductId);
                if (tmp == null)
                {
                    tmp = new PurchaseDetailTmp
                    {
                        Description = product.Description,
                        Price = newProduct.Price,
                        ProductId = newProduct.ProductId,
                        Quantity = newProduct.Quantity,
                        //TaxRate = product.Tax.Rate,
                        UserName = user.UserName
                    };
                    db.PurchaseDetailTmps.Add(tmp);
                }
                else
                {
                    tmp.Quantity += newProduct.Quantity;
                    db.Entry(tmp).State = EntityState.Modified;
                }

                db.SaveChanges();
                return RedirectToAction("Create");
            }
            ViewBag.CategoryId = new SelectList(CombosHelper.GetCategories(user.CompanyId), "CategoryId", "Description", newProduct.CategoryId);
            ViewBag.ProductId = new SelectList(CombosHelper.GetProducts(0), "ProductId", "Description", newProduct.ProductId);

            return View(newProduct);
        }

        public JsonResult GetProducts(int categoryId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var products = db.Products.Where(p => p.CategoryId == categoryId);
            return Json(products);
        }

        public ActionResult DeleteProduct(int? id)
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
            var product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            var tmp = db.PurchaseDetailTmps.FirstOrDefault(pdt => pdt.UserName == user.UserName && pdt.ProductId == id);
            if (tmp != null)
            {
                db.PurchaseDetailTmps.Remove(tmp);
                db.SaveChanges();
            }
            return RedirectToAction("Create");
        }
    }
}
