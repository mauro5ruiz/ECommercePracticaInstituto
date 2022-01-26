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
    public class CustomersController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Customers
        public ActionResult Index()
        {
            //var customers = db.Customers.Include(c => c.City).Include(c => c.Department);
            //return View(customers.ToList());
            var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
            var qry = (from cu in db.Customers
                join cc in db.CompanyCustomers on cu.CustomerId equals cc.CustomerId
                join co in db.Companies on cc.CompanyId equals co.CompanyId
            where co.CompanyId == user.CompanyId
            select new { cu }).ToList();

            var customers = new List<Customer>();
            foreach (var item in qry)
            {
                customers.Add(item.cu);
            }

            return View(customers);


        }

        // GET: Customers/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // GET: Customers/Create
        public ActionResult Create()
        {
            
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            return View();
        }

        // POST: Customers/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CustomerId,UserName,FirstName,LastName,Phone,Address,DepartmentId,CityId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                using (var tran = db.Database.BeginTransaction())
                {
                    try
                    {
                        var user = db.Users.Where(u => u.UserName == User.Identity.Name).FirstOrDefault();
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        UsersHelper.CreateUserASP(customer.UserName, "Customer");
                        var companyCustomer = new CompanyCustomer
                        {
                            CompanyId = user.CompanyId,
                            CustomerId = customer.CustomerId
                        };
                        db.CompanyCustomers.Add(companyCustomer);
                        db.SaveChanges();
                        tran.Commit();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {

                        tran.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", customer.DepartmentId);
                        ViewBag.CityId = new SelectList(CombosHelper.GetCities(customer.CityId), "CityId", "Name", customer.CityId);
                        

                        return View(customer);
                    }
                }
            }

            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", customer.DepartmentId);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(customer.CityId), "CityId", "Name", customer.CityId);
            return View(customer);

        }

        // GET: Customers/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", customer.CityId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", customer.DepartmentId);
            return View(customer);
        }

        // POST: Customers/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CustomerId,UserName,FirstName,LastName,Phone,Address,DepartmentId,CityId")] Customer customer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", customer.CityId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", customer.DepartmentId);
            return View(customer);
        }

        // GET: Customers/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Customer customer = db.Customers.Find(id);
            if (customer == null)
            {
                return HttpNotFound();
            }
            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //Customer customer = db.Customers.Find(id);
            //db.Customers.Remove(customer);
            //db.SaveChanges();
            //return RedirectToAction("Index");

            using (var tran = db.Database.BeginTransaction())
            {
                var user = db.Users.FirstOrDefault(u => u.UserName == User.Identity.Name);
                Customer customer = db.Customers.Find(id);
                try
                {
                    var companyCustomer = db.CompanyCustomers
                        .Where(cc => cc.CustomerId == customer.CustomerId && cc.CompanyId == user.CompanyId).FirstOrDefault();
                    db.CompanyCustomers.Remove(companyCustomer);
                    db.Customers.Remove(customer);
                    db.SaveChanges();
                    tran.Commit();
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {

                    tran.Rollback();
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(customer);
                }
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

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(m => m.DepartmentId == departmentId);
            return Json(cities);
        }
    }
}
