﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using WebApi.Jwt.Filters;

namespace ZXYL.Web.Api.Controllers
{
     [JwtAuthentication]
    public class ValuesController : ApiController
    {
        // GET api/values
        [AllowAnonymous]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        /// <summary>
        /// 输出id值
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string Get(int id)
        {
            int a = 0;
            int b = 1 / a;
            //throw new Exception("aaa");
            return "value:"+id;
        }

        // POST api/values
        /// <summary>
        /// 测试
        /// </summary>
        /// <param name="value"></param>
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
