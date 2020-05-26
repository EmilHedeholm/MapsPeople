﻿using Client.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Client.Controllers {
    public class QueueController : Controller {
        public ActionResult Index() {
            return View();
        }
        [HttpPost]
        // GET: Queue
        public ActionResult Index(string userQueue, string queueID) {
            RabbitMQAccess dataAccess = new RabbitMQAccess();
            dataAccess.ReceiveDataFromRabbitMQ(userQueue, queueID);
            //ViewBag.Situation = 2;
            return View(dataAccess.Data);
        }
    }
}