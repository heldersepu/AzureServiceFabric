using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace WebApi1.Controllers
{
    [ServiceRequestActionFilter]
    public class TestController : ApiController
    {
        // GET api/test
        public IHttpActionResult Get()
        {
            var d = new Dictionary<int,string>();
            for (int i = 0; i < 20; i++)
            {
                var x = Guid.NewGuid().ToString();
                x += Guid.NewGuid().ToString();
                d.Add(i, x.Replace("-",""));
            }
            return Json(d.OrderBy(x => x.Value));
        }
    }
}
