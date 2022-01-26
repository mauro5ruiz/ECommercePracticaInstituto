using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using VirtualCommerce.Models;

namespace VirtualCommerce.ViewModels
{
    [NotMapped]
    public class UserViewModel : User
    {
        public HttpPostedFileBase PhotoFile { get; set; }

    }
}