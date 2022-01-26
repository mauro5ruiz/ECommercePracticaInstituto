﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class PurchaseDetailTmp
    {
        [Key]
        public int PurchaseDetailTmpId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(256, ErrorMessage = "The filed {0} must be maximun {1} characters length")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [MaxLength(100, ErrorMessage = "The filed {0} must be maximun {1} characters length")]
        [Display(Name = "Product")]
        public string Description { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [Range(0, 1, ErrorMessage = "The {0} must be between {1} and {2}")]
        [Display(Name = "Tax rate")]
        public double TaxRate { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        [Range(1, double.MaxValue, ErrorMessage = "You must enter values in {0} between {1} and {2}")]

        public decimal Price { get; set; }

        [Required(ErrorMessage = "The field {0} is required")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        [Range(1, int.MaxValue, ErrorMessage = "You must enter values in {0} between {1} and {2}")]

        public double Quantity { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Value { get { return Price * (decimal)Quantity; } }

        public virtual Product Product { get; set; }
    }
}