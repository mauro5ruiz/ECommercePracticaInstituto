using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VirtualCommerce.Models;

namespace VirtualCommerce.Classes
{
    public class CombosHelper : IDisposable
    {
        private static VirtualCommerceDbContext db = new VirtualCommerceDbContext();

        public static List<Department> GetDepartments()
        {
            var departments = db.Departments.ToList();
            var defaultDepartment = new Department
            {
                DepartmentId = 0,
                Name = "[Select a Department]"
            };

            departments.Add(defaultDepartment);
            departments = departments.OrderBy(d => d.Name).ToList();
            return departments;
        }

        public static List<City> GetCities(int departmentId)
        {
            var cities = db.Cities
                .Where(c => c.DepartmentId == departmentId)
                .ToList();
            var defaultCity = new City
            {
                CityId = 0,
                Name = "[Select a City]"
            };

            cities.Add(defaultCity);
            cities = cities.OrderBy(d => d.Name).ToList();
            return cities;
        }


        public void Dispose()
        {
            db.Dispose();
        }

        public static List<Company> GetCompanies()
        {
            var companies = db.Companies
                .ToList();
            var defaultCompany = new Company
            {
                CompanyId = 0,
                Name = "[Select a Company]"
            };

            companies.Add(defaultCompany);
            companies = companies.OrderBy(d => d.Name).ToList();
            return companies;
        }

        public static List<Category> GetCategories(int userCompanyId)
        {
            var categories = db.Categories
                .Where(c => c.CompanyId == userCompanyId)
                .ToList();
            var defaultCategory = new Category
            {
                CategoryId = 0,
                Description = "[Select a Category]"
            };
            categories.Add(defaultCategory);
            categories = categories.OrderBy(c => c.Description).ToList();
            return categories;
        }

        public static List<Customer>GetCustomers(int userCompanyId)
        {
            var qry = (from cu in db.Customers
                join cc in db.CompanyCustomers on cu.CustomerId equals cc.CustomerId
                join co in db.Companies on cc.CompanyId equals co.CompanyId
            where co.CompanyId == userCompanyId
            select new { cu }).ToList();

            var customers = new List<Customer>();
            foreach (var item in qry)
            {
                customers.Add(item.cu);

            }
            var defaultCustomer = new Customer
            {
                CustomerId = 0,
                FirstName = "[Select a Customer]"
            };
            customers.Add(defaultCustomer);
            customers = customers
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToList();
            return customers;

        }

        public static IEnumerable GetSuppliers(int companyId)
        {
            var qry = (from su in db.Suppliers
                join cs in db.CompanySuppliers on su.SupplierId equals cs.SupplierId
                join co in db.Companies on cs.CompanyId equals co.CompanyId
                where co.CompanyId == companyId
                select new { su }).ToList();

            var suppliers = new List<Supplier>();
            foreach (var item in qry)
            {
                suppliers.Add(item.su);

            }
            var defaultSupplier = new Supplier
            {
                SupplierId = 0,
                FirstName = "[Select a Supplier]"
                

            };
            suppliers.Add(defaultSupplier);
            suppliers = suppliers
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToList();
            return suppliers;
        }

        public static IEnumerable GetWarehouses(int companyId)
        {
            var warehouses = db.Warehouses
                .Where(c => c.CompanyId == companyId)
                .ToList();
            var defaultWarehouse = new Warehouse
            {
                WarehouseId = 0,
                Name = "[Select a Warehouse]"
            };
            warehouses.Add(defaultWarehouse);
            warehouses = warehouses.OrderBy(c => c.Name).ToList();
            return warehouses;
        }

        public static IEnumerable GetProducts(int companyId)
        {
            var products = db.Products.Where(p => p.CompanyId == companyId).ToList();
            var defaultProduct = new Product
            {
                ProductId = 0,
                Description = "[Select a Product]"
            };
            products.Add(defaultProduct);
            products = products.OrderBy(p => p.Description).ToList();
            return products;

        }

        public static IEnumerable GetSupplier(int companyId)
        {
            var qry = (from su in db.Suppliers
                join cs in db.CompanySuppliers on su.SupplierId equals cs.SupplierId
                join co in db.Companies on cs.CompanyId equals co.CompanyId
                where co.CompanyId == companyId
                select new { su }).ToList();

            var suppliers = new List<Supplier>();
            foreach (var item in qry)
            {
                suppliers.Add(item.su);

            }
            var defaultSupplier = new Supplier
            {
                SupplierId = 0,
                FirstName = "[Select a Supplier]"
            };
            suppliers.Add(defaultSupplier);
            suppliers = suppliers
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToList();
            return suppliers;
        }
    }
}