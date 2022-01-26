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
    public class CompaniesController : Controller
    {
        private VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        // GET: Companies
        public ActionResult Index()
        {
            var companies = db.Companies.Include(c => c.City).Include(c => c.Department);
            return View(companies.ToList());
        }

        // GET: Companies/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // GET: Companies/Create
        public ActionResult Create()
        {
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(0), "CityId", "Name");
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name");
            
            return View();
        }

        // POST: Companies/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CompanyViewModel companyViewModel)
        {
            if (ModelState.IsValid)
            {
                using (var transaction = db.Database.BeginTransaction())
                {
                    try
                    {
                        /*Tomo el objeto viewmodel y lo paso a objeto
                         para ser persistido*/
                        var company = ToCompany(companyViewModel);
                        //Lo agrego al objeto dbset
                        db.Companies.Add(company);
                        //lo guardo en la tabla
                        db.SaveChanges();
                        //Me fijo si tengo alguna imagen 
                        if (companyViewModel.LogoFile != null)
                        {
                            //Defino la carpeta donde se guarda
                            var folder = "~/Content/Logos";
                            //defino el nombre del archivo tomando el id de la compañía
                            var file = $"{company.CompanyId}.jpg";
                            //Llamo al método UploadPhoto del FileHelper para guardar la imagen
                            var response = FileHelper.UploadPhoto(companyViewModel.LogoFile, folder, file);
                            //Si todo salió bien
                            if (response)
                            {
                                //Armo la ruta donde se guardo la imagen y la 
                                //pongo en la variabe pic
                                var pic = $"{folder}/{file}";
                                //Asigno en el atributo Logo el contenido de la variable pic
                                company.Logo = pic;
                                //Cambio el estado del objeto company para que luego el EF
                                //guarde los cambios
                                db.Entry(company).State = EntityState.Modified;
                                //Guardo los cambios en el objeto modificado
                                db.SaveChanges();

                            }
                        }
                        //Hace la persistencia de los datos
                        transaction.Commit();
                        return RedirectToAction("Index");


                    }
                    catch (Exception ex)
                    {
                        //Tira todo para atras
                        transaction.Rollback();
                        ModelState.AddModelError(string.Empty, ex.Message);
                        ViewBag.CityId = new SelectList(CombosHelper.GetCities(companyViewModel.DepartmentId), "CityId", "Name", companyViewModel.CityId);
                        ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", companyViewModel.DepartmentId);
                        return View(companyViewModel);

                    }
                }

            }

            ViewBag.CityId = new SelectList(CombosHelper.GetCities(companyViewModel.DepartmentId), "CityId", "Name", companyViewModel.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", companyViewModel.DepartmentId);
            return View(companyViewModel);
        }

        private Company ToCompany(CompanyViewModel companyViewModel)
        {
            return new Company
            {
                CompanyId = companyViewModel.CompanyId,
                Name = companyViewModel.Name,
                Phone = companyViewModel.Phone,
                Address = companyViewModel.Address,
                Logo = companyViewModel.Logo,
                DepartmentId = companyViewModel.DepartmentId,
                CityId = companyViewModel.CityId
            };
        }

        // GET: Companies/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }

            var companyViewModel = ToViewModel(company);
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(company.DepartmentId), "CityId", "Name", company.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", company.DepartmentId);
            return View(companyViewModel);
        }

        private CompanyViewModel ToViewModel(Company company)
        {
            return new CompanyViewModel
            {
                CompanyId = company.CompanyId,
                Name = company.Name,
                Phone = company.Phone,
                Address = company.Address,
                Logo = company.Logo,
                DepartmentId = company.DepartmentId,
                CityId = company.CityId,

            };
        }
        // POST: Companies/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que desea enlazarse. Para obtener 
        // más información vea https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CompanyViewModel companyViewModel)
        {
            if (ModelState.IsValid)
            {
                //Convierte el objeto ViewModel en un objeto para ser persistido
                var company = ToCompany(companyViewModel);
                //Se fija si tengo un logo
                if (companyViewModel.LogoFile != null)
                {
                    var pic = string.Empty;
                    var folder = "~/Content/Logos";
                    var file = $"{company.CompanyId}.jpg";
                    var response = FileHelper.UploadPhoto(companyViewModel.LogoFile, folder, file);

                    if (response)
                    {
                        pic = $"{folder}/{file}";
                        company.Logo = pic;
                    }
                }
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CityId = new SelectList(CombosHelper.GetCities(companyViewModel.DepartmentId), "CityId", "Name", companyViewModel.CityId);
            ViewBag.DepartmentId = new SelectList(CombosHelper.GetDepartments(), "DepartmentId", "Name", companyViewModel.DepartmentId);
            return View(companyViewModel);
        }

        // GET: Companies/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Company company = db.Companies.Find(id);
            if (company == null)
            {
                return HttpNotFound();
            }
            return View(company);
        }

        // POST: Companies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Company company = db.Companies.Find(id);
            db.Companies.Remove(company);
            db.SaveChanges();
            var response = FileHelper.DeletePhoto(company.Logo);
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
