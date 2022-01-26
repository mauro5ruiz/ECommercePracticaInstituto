using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using PagedList;
using VirtualCommerce.Models;

namespace VirtualCommerce.Controllers
{
    public class DepartmentsController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Departments
        public ActionResult Index(int? page = null)
        {
            page = page ?? 1;
            return View(db.Departments
                .OrderBy(d => d.Name)
                .ToPagedList((int)page, 5));
           
        }

        // GET: Departments/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // GET: Departments/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Departments/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DepartmentId,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                db.Departments.Add(department);
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

            return View(department);
        }

        // GET: Departments/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DepartmentId,Name")] Department department)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(department).State = EntityState.Modified;
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
            return View(department);
        }

        // GET: Departments/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = db.Departments.Find(id);
            try
            {
                db.Departments.Remove(department);
                db.SaveChanges();
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                if (ex.InnerException != null &&
                    ex.InnerException.InnerException != null &&
                    ex.InnerException.InnerException.Message.Contains("REFERENCE"))
                {
                    ModelState.AddModelError(string.Empty, "The field has related records in other table");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }

                return View(department);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult AddCity(int ? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = db.Departments.Find(id);
            if (department == null)
            {
                return HttpNotFound();

            }

            City city = new City
            {
                DepartmentId = department.DepartmentId,
                Department = department
            };
            return View(city);
        }

        [HttpPost]
        public ActionResult AddCity(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Cities.Add(city);
                    db.SaveChanges();
                    return RedirectToAction($"Details/{city.DepartmentId}");

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("IX"))
                    {
                        ModelState.AddModelError(string.Empty, "The city already exists");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);

                    }
                }

            }
            return View(city);

        }

        public ActionResult EditCity(int? cityId, int? departmentId)
        {
            if (cityId == null || departmentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(cityId);
            if (city == null)
            {
                return HttpNotFound();

            }


            return View(city);
        }

        [HttpPost]
        public ActionResult EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction($"Details/{city.DepartmentId}");

                }
                catch (Exception ex)
                {
                    if (ex.InnerException != null &&
                        ex.InnerException.InnerException != null &&
                        ex.InnerException.InnerException.Message.Contains("IX"))
                    {
                        ModelState.AddModelError(string.Empty, "The city already exists");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, ex.Message);

                    }

                }
            }

            return View(city);
        }


        public ActionResult DeleteCity(int? cityid, int? departmentid)
        {
            if (cityid == null || departmentid == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = db.Cities.Find(cityid);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("DeleteCity")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteCityConfirmed(int id)
        {
            City city = db.Cities.Find(id);
            db.Cities.Remove(city);
            db.SaveChanges();
            //return RedirectToAction("Index");
            return RedirectToAction($"Details/{city.DepartmentId}");
        }
    }
 }
