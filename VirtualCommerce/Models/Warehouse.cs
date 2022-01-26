using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class Warehouse
    {
        [Key]
        public int WarehouseId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Company.")]
        [Index("IX_Warehouses_CompanyId_Description", Order = 1, IsUnique = true)]
        [Display(Name = "Company")]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Index("IX_Warehouses_CompanyId_Description", Order = 2, IsUnique = true)]
        [StringLength(50, ErrorMessage = "The field must have between {2} and {1} characters", MinimumLength = 3)]
        public string Name { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field must have between {2} and {1} characters", MinimumLength = 3)]
        public string Address { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DataType(DataType.PhoneNumber)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "You must select a department")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, Int32.MaxValue, ErrorMessage = "You must select a city")]
        [Display(Name = "City")]
        public int CityId { get; set; }

        public virtual Company Company { get; set; }
        public virtual Department Department { get; set; }
        public virtual City City { get; set; }

        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<Purchase> Purchases { get; set; }

    }
}