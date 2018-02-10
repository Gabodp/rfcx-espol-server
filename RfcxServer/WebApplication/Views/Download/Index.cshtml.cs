using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApplication.Views.Download
{
    public class IndexModel : PageModel
    {
        private List<String> devices;

        public void OnGet()
        {
            devices = new List<String>();
            DirectoryInfo DI = new DirectoryInfo("files");
            foreach(var item in DI.GetFiles())
            {
                devices.Add(item.Name);
            }

        }
    }
}