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

namespace VirtualCommerce.Controllers
{
    [Authorize(Roles = "User")]
    public class SuppliersController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Suppliers
        public ActionResult Index()
        {
            //var suppliers = db.Suppliers.Include(s => s.City).Include(s => s.Department);
            //return View(suppliers.ToList());

            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            var qry = (from su in db.Suppliers
                join cs in db.CompanySuppliers on su.SupplierId equals cs.SupplierId
                join co in db.Companies on cs.CompanyId equals co.CompanyId
                where co.CompanyId == user.CompanyId
                select new { su }).ToList();

            var suppliers = new List<Supplier>();
            foreach (var item in qry)
            {
                suppliers.Add(item.su);
            }

            return View(suppliers);
        }

        // GET: Suppliers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // GET: Suppliers/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            return View();
        }

        // POST: Suppliers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "SupplierId,UserName,FirstName,LastName,Phone,Address,DepartmentId,CityId")] Supplier supplier)
        {
            //if (ModelState.IsValid)
            //{
            //    db.Suppliers.Add(supplier);
            //    db.SaveChanges();
            //    return RedirectToAction("Index");
            //}

            //ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", supplier.CityId);
            //ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", supplier.DepartmentId);
            //return View(supplier);

            var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (ModelState.IsValid)
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        db.Suppliers.Add(supplier);
                        db.SaveChanges();
                        UsersHelper.CreateUserASP(user.UserName, "Supplier");
                        var companySupplier = new CompanySupplier
                        {
                            CompanyId = user.CompanyId,
                            SupplierId = supplier.SupplierId
                        };
                        db.CompanySuppliers.Add(companySupplier);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", supplier.CityId);
                        ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", supplier.DepartmentId);
                        return View(supplier);

                    }
                }
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", supplier.CityId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", supplier.DepartmentId);
            return View(supplier);

        }

        // GET: Suppliers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", supplier.CityId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", supplier.DepartmentId);
            return View(supplier);
        }

        // POST: Suppliers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "SupplierId,UserName,FirstName,LastName,Phone,Address,DepartmentId,CityId")] Supplier supplier)
        {
            if (ModelState.IsValid)
            {
                db.Entry(supplier).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", supplier.CityId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", supplier.DepartmentId);
            return View(supplier);
        }

        // GET: Suppliers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Supplier supplier = db.Suppliers.Find(id);
            if (supplier == null)
            {
                return HttpNotFound();
            }
            return View(supplier);
        }

        // POST: Suppliers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Supplier supplier = db.Suppliers.Find(id);
            db.Suppliers.Remove(supplier);
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

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(m => m.DepartmentId == departmentId);
            return Json(cities);
        }

    }
}
