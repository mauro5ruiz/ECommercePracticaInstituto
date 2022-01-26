using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PagedList;
using VirtualCommerce.Classes;
using VirtualCommerce.Models;

namespace VirtualCommerce.Controllers
{
    public class CitiesController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Cities
        public ActionResult Index(int? page = 1)
        {
            page = page ?? 1;
            var cities = db.Cities
                .Include(c => c.Department)
                .OrderBy(c => c.Department.Name)
                .ThenBy(c => c.Name).ToPagedList((int)page, 5);
            return View(cities);
        }

        // GET: Cities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: Cities/Create
        public ActionResult Create()
        {
            
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments()
                , "DepartmentId", "Name");
            return View();
        }

        // POST: Cities/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CityId,Name,DepartmentId")] City city)
        {
            if (ModelState.IsValid)
            {
                db.Cities.Add(city);
                try
                {
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

            ViewBag.DepartmentId = new SelectList(
                CombosHelper.GetDepartments(), "DepartmentId", "Name", city.DepartmentId);
            return View(city);
        }

        // GET: Cities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            ViewBag.DepartmentId = new SelectList(
                CombosHelper.GetDepartments(), "DepartmentId", "Name", city.DepartmentId);
            return View(city);
        }

        // POST: Cities/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CityId,Name,DepartmentId")] City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(city).State = EntityState.Modified;
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
            ViewBag.DepartmentId = new SelectList(
                CombosHelper.GetDepartments(), "DepartmentId", "Name", city.DepartmentId);
            return View(city);
        }

        // GET: Cities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            City city = db.Cities.Find(id);
            db.Cities.Remove(city);
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
