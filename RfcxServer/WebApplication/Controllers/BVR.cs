using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WebApplication {

    public class BVR : Controller {
        public IActionResult Index() {
            return View();
        }
    }
    
}