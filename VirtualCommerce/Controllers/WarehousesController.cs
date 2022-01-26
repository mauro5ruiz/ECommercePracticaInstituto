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
    public class WarehousesController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Warehouses
        public ActionResult Index()
        {
            var user = db.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefault();

            if (user == null)
            {
                RedirectToAction("Index", "Home");
            }

            var warehouses = db.Warehouses
                .Include(w => w.City).
                Include(w => w.Company).
                Include(w => w.Department)
                .Where(ca => ca.CompanyId == user.CompanyId);   
            return View(warehouses.ToList());
        }

        // GET: Warehouses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        // GET: Warehouses/Create
        public ActionResult Create()
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                RedirectToAction("Index", "Home");
            }

            var  warehouse = new Warehouse
            {
                CompanyId = user.CompanyId,

            };
            

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            return View(warehouse);
        }

        // POST: Warehouses/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WarehouseId,CompanyId,Name,Address,Phone,DepartmentId,CityId")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                db.Warehouses.Add(warehouse);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", warehouse.CityId);
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", warehouse.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", warehouse.DepartmentId);
            return View(warehouse);
        }

        // GET: Warehouses/Edit/5
        public ActionResult Edit(int? id)
        {
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();

            if (user == null)
            {
                RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            Category category = db.Categories.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
           
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(user.DepartmentId), "CityId", "Name", warehouse.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", warehouse.DepartmentId);
            return View(warehouse);
        }

        // POST: Warehouses/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "WarehouseId,CompanyId,Name,Address,Phone,DepartmentId,CityId")] Warehouse warehouse)
        {
            if (ModelState.IsValid)
            {
                db.Entry(warehouse).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", warehouse.CityId);
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", warehouse.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", warehouse.DepartmentId);
            return View(warehouse);
        }

        // GET: Warehouses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Warehouse warehouse = db.Warehouses.Find(id);
            if (warehouse == null)
            {
                return HttpNotFound();
            }
            return View(warehouse);
        }

        // POST: Warehouses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Warehouse warehouse = db.Warehouses.Find(id);
            db.Warehouses.Remove(warehouse);
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
