using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DatabaseAccess;

namespace Core.Controllers
{
    public class SendController : ApiController {
        IDataAccess dataAccess = new DataAccess();

    }
    
    
}
