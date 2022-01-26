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
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Users
        public ActionResult Index()
        {
            var users = db.Users.Include(u => u.City).Include(u => u.Company).Include(u => u.Department);
            return View(users.ToList());
        }

        // GET: Users/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: Users/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            return View();
        }

        // POST: Users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UserViewModel userViewModel)
        {

            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var user = ToModel(userViewModel);
                        db.Users.Add(user);
                        db.SaveChanges();

                        UsersHelper.CreateUserASP(user.UserName, "User");

                        if (userViewModel.PhotoFile != null)
                        {
                            var folder = "~/Content/Users";
                            var file = $"{user.UserId}.jpg";
                            var response = FileHelper.UploadPhoto(userViewModel.PhotoFile, folder, file);
                            if (response)
                            {
                                var pic = $"{folder}/{file}";
                                user.Photo = pic;
                                db.Entry(user).State = EntityState.Modified;
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
                        ViewBag.CityId = new SelectList(CombosHelper.GetCities(userViewModel.DepartmentId), "CityId", "Name", userViewModel.CityId);
                        ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", userViewModel.CompanyId);
                        ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", userViewModel.DepartmentId);

                        return View(userViewModel);
                    }
                }

            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(userViewModel.DepartmentId), "CityId", "Name", userViewModel.CityId);
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", userViewModel.CompanyId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", userViewModel.DepartmentId);
            return View(userViewModel);
        }

        private User ToModel(UserViewModel userViewModel)
        {
            return new User
            {
                UserId = userViewModel.UserId,
                UserName = userViewModel.UserName,
                FirstName = userViewModel.FirstName,
                LastName = userViewModel.LastName,
                Phone = userViewModel.Phone,
                Photo = userViewModel.Photo,
                Address = userViewModel.Address,
                DepartmentId = userViewModel.DepartmentId,
                CityId = userViewModel.CityId,
                CompanyId = userViewModel.CompanyId

            };
        }

        // GET: Users/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }

            var userViewModel = ToViewModel(user);

            ViewBag.CityId = new SelectList(db.Cities, "CityId", "Name", user.CityId);
            ViewBag.CompanyId = new SelectList(db.Companies, "CompanyId", "Name", user.CompanyId);
            ViewBag.DepartmentId = new SelectList(db.Departments, "DepartmentId", "Name", user.DepartmentId);
            return View(userViewModel);
        }

        private UserViewModel ToViewModel(User user)
        {
            return new UserViewModel
            {
                UserId = user.UserId,
                UserName = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Phone = user.Phone,
                Photo = user.Photo,
                Address = user.Address,
                DepartmentId = user.DepartmentId,
                CityId = user.CityId,
                CompanyId = user.CompanyId
            };
        }

        // POST: Users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UserViewModel userViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        var user = ToModel(userViewModel);
                        if (userViewModel.PhotoFile != null)
                        {
                            var folder = "~/Content/Users";
                            var file = $"{user.UserId}.jpg";

                            var response = FileHelper.UploadPhoto(userViewModel.PhotoFile, folder, file);
                            if (response)
                            {
                                var pic = $"{folder}/{file}";
                                user.Photo = pic;
                            }
                        }

                        var db2 = new VirtualCommerceDbContext();
                        var currentUser = db2.Users.Find(user.UserId);
                        if (currentUser.UserName != user.UserName)
                        {
                            UsersHelper.UpdateUserName(currentUser.UserName, user.UserName);
                        }

                        db2.Dispose();
                        db.Entry(user).State = EntityState.Modified;
                        db.SaveChanges();

                        transaction.Commit();
                        return RedirectToAction("Index");

                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        ViewBag.CityId = new SelectList(CombosHelper.GetCities(userViewModel.DepartmentId), "CityId", "Name", userViewModel.CityId);
                        ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", userViewModel.CompanyId);
                        ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", userViewModel.DepartmentId);
                        return View(userViewModel);
                    }
                }

            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(userViewModel.DepartmentId), "CityId", "Name", userViewModel.CityId);
            ViewBag.CompanyId = new SelectList(CombosHelper.GetCompanies(), "CompanyId", "Name", userViewModel.CompanyId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", userViewModel.DepartmentId);
            return View(userViewModel);
        }

        // GET: Users/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            User user = db.Users.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: Users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            //using (var transaction = db.Database.BeginTransaction())
            //{
            //    User user = db.Users.Find(id);

            //    try
            //    {
            //        db.Users.Remove(user);
            //        db.SaveChanges();
            //        var response = FileHelper.DeletePhoto(user.Photo);

            //        UsersHelper.DeleteUser(user.UserName);
            //        transaction.Commit();
            //        return RedirectToAction("Index");

            //    }
            //    catch (Exception ex)
            //    {

            //        transaction.Rollback();
            //        ModelState.AddModelError(string.Empty, ex.Message);
            //        return View("Delete", user);
            //    }

            //}

            User user = db.Users.Find(id);
            using (var tran = db.Database.BeginTransaction())
            {
                try
                {
                    db.Users.Remove(user);
                    db.SaveChanges();
                    UsersHelper.DeleteUser(user.UserName, "User");
                    tran.Commit();
                    return RedirectToAction("Index");

                }
                catch (Exception ex)
                {

                    tran.Rollback();
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return View(user);
                }
            }
        }

        public JsonResult GetCities(int departmentId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var cities = db.Cities.Where(m => m.DepartmentId == departmentId);
            return Json(cities);
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
