using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Company")]
        [Display(Name = "Company")]
        [Index("IX_Product_CompanyId_Description", Order = 1, IsUnique = true)]

        public int CompanyId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a category")]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(100, ErrorMessage = "The field must have between {2} and {1} characters", MinimumLength = 3)]
        [Index("IX_Product_CompanyId_Description", Order = 2, IsUnique = true)]

        public string Description { get; set; }

        //[Required(ErrorMessage = "The field {0} is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a Tax")]
        //[Display(Name = "Tax")]

        //public int TaxId { get; set; }


        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, double.MaxValue, ErrorMessage = "You must enter a price between {1} and {2}")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Price { get; set; }

        [DataType(DataType.ImageUrl)]
        public string Image { get; set; }

        [DataType(DataType.MultilineText)]
        public string Remarks { get; set; }

        public virtual Company Company { get; set; }
        public virtual Category Category { get; set; }
        //public virtual Tax Tax { get; set; }
        public virtual ICollection<Inventory> Inventories { get; set; }
        public virtual ICollection<PurchaseDetail> PurchasesDetails { get; set; }
        public virtual ICollection<PurchaseDetailTmp> PurchasesDetailsTmps { get; set; }
    }
}