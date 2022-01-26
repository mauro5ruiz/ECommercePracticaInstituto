using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class Inventory
    {
        [Key]
        public int InventoryId { get; set; }

        [Required]
        public int WarehouseId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]

        public double Stock { get; set; }

        public virtual Warehouse Warehouse { get; set; }
        public virtual Product Product { get; set; }

    }
}