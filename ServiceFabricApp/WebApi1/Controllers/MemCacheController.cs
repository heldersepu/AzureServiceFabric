using System;
using System.Dynamic;
using System.Runtime.Caching;
using System.Web.Http;

namespace WebApi1.Controllers
{
    [ServiceRequestActionFilter]
    public class MemCacheController : ApiController
    {
        // GET api/memcache
        public IHttpActionResult Get()
        {
            string servername;
            var memCache = MemoryCache.Default.Get("MemCacheController");
            if (memCache == null)
            {
                servername = Environment.MachineName;
                var policy = new CacheItemPolicy { SlidingExpiration = TimeSpan.FromHours(1) };
                MemoryCache.Default.Add("MemCacheController", servername, policy);
            }
            else
            {
                servername = (string)memCache;
            }

            dynamic o = new ExpandoObject();
            o.MachineName = servername;
            o.MemCache = (memCache != null);
            return Json(o);
        }
    }
}
