﻿using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using System.Web.Http.Controllers;

namespace ZXYL.Web.Api
{

    /// <summary>
    /// 搜索条件
    /// </summary>
    public static class QueryHelper
    {

        /// <summary>
        /// 获取搜索query
        /// </summary>
        public static string GetWhereStr(this HttpControllerContext context)
        {
            var request = context.Request;
            var body = request.Content;
            var query =  GetQuery(context);
            if (query==null)
            {
                return "-1";
            }
          

            return context.GetWhereStr(query);
        }

        public static  Dictionary<string, object> GetQuery(this HttpControllerContext context)
        {
            try
            {
                var postData = string.Empty;
                var request = context.Request;
                var body = request.Content;

				 //posData为空bug： Action参数存在[FromBody]等读取内容的方法时，会被[FromBody]“吃掉”，指针重定向或继承BaseController重新 Initialize
               // request.Content.ReadAsStreamAsync().Result.Seek(0, System.IO.SeekOrigin.Begin);
                postData = body.ReadAsStringAsync().Result;
                var query = new Dictionary<string, object>();
                if (string.IsNullOrEmpty(postData))
                {
                    return null;
                }

                var postDataJson = JsonConvert.DeserializeObject<dynamic>(postData);
                query = JsonConvert.DeserializeObject<Dictionary<string, object>>(postDataJson.query.ToString());
                return query;
                

            }
            catch (Exception)
            {

                return null;
            }
        }

        public static string GetWhereStr(this HttpControllerContext context, Dictionary<string, object> query)
        {
            string strWhere = " 1=1 ";
            if (query == null)
            {
                return strWhere;
            }
            var keys = query.Select(p => p.Key);
            var parms = keys.Where(p => (p.Contains("SL_")
                || p.Contains("SEB_")) || p.Contains("SES_") || p.Contains("SEGT_") || p.Contains("SELT_") || p.Contains("SEI_") || p.Contains("SENE_") || p.Contains("SLL_") || p.Contains("SLR_"));
            foreach (var parm in parms)
            {
                var name = parm.Split('_');
                var keyPosition = name[0].Length + 1;
                var fieldName = parm.Substring(keyPosition, parm.Length - keyPosition);

                var value = query[parm].ToString().Trim();
                if (string.IsNullOrWhiteSpace(value))
                {
                    continue;
                }

                value = SqlFilter(value);

                switch (name[0])
                {
                    case "SL": strWhere += string.Format(" And {0} like '%{1}%' ", fieldName, value); break;
                    case "SLL": strWhere += string.Format(" And {0} like '%{1}' ", fieldName, value); break;
                    case "SLR": strWhere += string.Format(" And {0} like '{1}%' ", fieldName, value); break;
                    case "SEB": strWhere += string.Format(" And {0}={1} ", fieldName, value); break;
                    case "SEI": strWhere += string.Format(" And {0} in({1})", fieldName, value); break;
                    case "SES": strWhere += string.Format(" And {0}='{1}'", fieldName, value); break;
                    case "SEGT": strWhere += string.Format(" And {0}>='{1}' ", fieldName, value); break;
                    case "SELT": strWhere += string.Format(" And {0}<='{1}' ", fieldName, value); break;
                    case "SENE": strWhere += string.Format(" And {0}<>'{1}' ", fieldName, value); break;
                    default:
                        break;
                }
            }

            return strWhere;
        }
        /// <summary>
        /// 过滤危险的字符（SQL注入）
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string SqlFilter(this string str)
        {
            var ext = new[] { "and", "exec", "insert", "select", "delete", "update", "chr", "mid", "master", "or", "truncate", "char", "declare", "join", "\r", "\n", "'" };

            if (str.Contains("'"))
            {
                str = str.Replace("'", "''");
            }
            else
            {
                if (!string.IsNullOrEmpty(str) && str.Length >= 3)
                {
                    foreach (var e in ext.Where(e => str.ToLower().IndexOf(e, StringComparison.Ordinal) != -1))
                    {
                        str = Regex.Replace(str, e, "", RegexOptions.IgnoreCase);
                    }
                }

                if (str.Length >= 128)
                    str = "";
            }


            return str;
        }
    }
}
