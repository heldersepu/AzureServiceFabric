using System;
using System.Collections.Generic;
using System.Web.Http;

namespace WebApi1.Controllers
{
    [ServiceRequestActionFilter]
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            var list = new List<string>();
            for (int i = 0; i < 20; i++)
            {
                list.Add(Guid.NewGuid().ToString());
            }
            return list.ToArray();
        }

        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
