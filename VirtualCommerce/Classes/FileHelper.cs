using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace VirtualCommerce.Classes
{
    public class FileHelper
    {
        public static bool UploadPhoto(HttpPostedFileBase file, string folder, string name)
        {
            if (file == null || string.IsNullOrEmpty(folder) || string.IsNullOrEmpty(name))
            {
                return false;
            }

            try
            {
                string path = string.Empty;

                path = Path.Combine(HttpContext.Current.Server.MapPath(folder), name);
                file.SaveAs(path);
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }

        public static bool DeletePhoto(string name)
        {
            try
            {
                var archiv = new FileInfo(HttpContext.Current.Server.MapPath(name));
                if (!archiv.Exists)
                {
                    return false;
                }
                archiv.Delete();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}