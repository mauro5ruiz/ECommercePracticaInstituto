using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class Tax
    {
        //[Key]
        //public int TaxId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required")]
        //[StringLength(50, ErrorMessage = "The field must be between {2} and {1} characters.")]
        ////[Display(Name = "Description")]
        //[Index("IX_Taxes_CompanyId_Description", Order = 2, IsUnique = true)]
        //public string Description { get; set; }


        //[Required(ErrorMessage = "The field {0} is required")]
        //[Range(1, int.MaxValue, ErrorMessage = "You must select a Company.")]
        //[Index("IX_Taxes_CompanyId_Description", Order = 1, IsUnique = true)]
        //[Display(Name = "Company")]
        //public int CompanyId { get; set; }

        //[Required(ErrorMessage = "The field {0} is required")]
        //[Range(0,100,ErrorMessage = "You must enter a value between {1} and {2}.")]
        //[DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        //public double Rate { get; set; }

        //public virtual Company Company { get; set; }
        //public virtual ICollection<Product> Products { get; set; }

        [Key]
        public int TaxId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(1, int.MaxValue, ErrorMessage = "You must select a Company")]
        [Display(Name = "Company")]
        [Index("IX_Taxes_CompanyId_Description", Order = 1, IsUnique = true)]
        public int CompanyId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [StringLength(50, ErrorMessage = "The field must be between {2} and {1} characters", MinimumLength = 3)]
        [Index("IX_Taxes_CompanyId_Description", Order = 2, IsUnique = true)]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(0, 1, ErrorMessage = "You must enter a value between {1} and {2}")]
        [DisplayFormat(DataFormatString = "{0:P2}", ApplyFormatInEditMode = false)]
        public double Rate { get; set; }

        public virtual Company Company { get; set; }
        //public virtual ICollection<Product> Products { get; set; }
    }
}