using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Models
{
    public class Status
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(20)]
        public string Description { get; set; }
    }
}