using Client.DataAccess;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Client.Models;

namespace Client.Controllers {
    public class HomeController : Controller {
        [HttpPost]
        public ActionResult Index(string queueName) {
            RabbitMQAccess dataAccess = new RabbitMQAccess();
            string jsonData = dataAccess.GetData(queueName);
            var data = JsonConvert.DeserializeObject<List<Message>>(jsonData);
            return View(data);
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}