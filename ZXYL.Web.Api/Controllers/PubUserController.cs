﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ZXYL.BLL;
using ZXYL.Model;
using System.Web.Http;

namespace ZXYL.Web.Api.Controllers
{

    [RoutePrefix("api/PubUser")]
    [JwtAuthentication]
    public class PubUserController : BaseController
    {
        private Pub_UserBLL bll = new Pub_UserBLL();
        private V_PubUser_DeptBLL userDeptBLL = new V_PubUser_DeptBLL();
        private Pub_UserRoleBLL userRoleBLL = new Pub_UserRoleBLL();
        Pub_UserFunctionBLL userFunctionBLL = new Pub_UserFunctionBLL();

        [HttpGet,Route("GetAccess")]
        public dynamic GetAccess()
        {
            var user = new ZXYLUser(User);
            var userName = user.UserName;
            var userCode = user.UserCode;
            var access = user.Access;
            return new
            {
                access = access,
                avatar = "https://file.iviewui.com/dist/a0e88e83800f138b94d2414621bd9704.png",
                name = userName,
                user_id = userCode
            };
        }

        /// <summary>
        /// 获取用户分页
        /// </summary>
        /// <returns></returns>
        [Route("GetPage")]
        [HttpPost]
        public PageDateRes<V_PubUser_Dept> GetPage([FromBody]PageDataReq pageReq)
        {
            var whereStr = GetWhereStr();
            if (whereStr=="-1")
            {
                return new PageDateRes<V_PubUser_Dept>() {code=ResCode.Error,msg="查询参数有误！",data=null };
            }
            var users = userDeptBLL.GetPage(whereStr, (pageReq.field + " " + pageReq.order), pageReq.pageNum, pageReq.pageSize);

            //  PageDateRes<V_PubUser_DeptExt> users = usersPage.MapTo<PageDateRes <V_PubUser_Dept>,PageDateRes <V_PubUser_DeptExt>>();
            var userCodes = string.Join("','", users.data.Select(p => p.UserCode));
            List<Pub_UserRole> userRoles = userRoleBLL.GetList(string.Format("userCode in ('{0}')",userCodes));
            users.data.ForEach(p =>
            {
                p.RoleCodes = userRoles.Where(c => c.UserCode == p.UserCode).Select(d => d.RoleCode);
            });

            return users;
        }

        private string GetWhereStr()
        {
            StringBuilder sb = new StringBuilder(" 1=1 ");
            sb.Append(" and StopFlag=0 ");
            var query = this.ControllerContext.GetWhereStr();
            if (query=="-1")
            {
                return query;
            }
            sb.AppendFormat(" and {0} ", query);

            return sb.ToString();
        }


        /// <summary>
        /// 添加用户
        /// </summary>
        /// <returns></returns>
        [Route("Add")]
        [HttpPost]
        public DataRes<bool> Add([FromBody]V_PubUser_Dept model)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            model.Crdt = model.Lmdt = DateTime.Now;
            var user = User.GetZXYLUser();
            model.Crid=model.Lmid = string.Format("{0}-{1}",user.UserCode,user.UserName);
            var r = bll.Add(model);
            if (!r.Item1)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = r.Item2;
            }

            return res;
        }

        /// <summary>
        /// 编辑用户
        /// </summary>
        /// <returns></returns>
        [Route("Edit")]
        [HttpPost]
        public DataRes<bool> Edit([FromBody]V_PubUser_Dept model)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            model.Lmdt = DateTime.Now;
           var user = User.GetZXYLUser();
            model.Lmid = string.Format("{0}-{1}",user.UserCode,user.UserName);
            var r = bll.Edit(model);
            if (!r.Item1)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = r.Item2;
            }

            return res;
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <returns></returns>
        [Route("Delete/{id}")]
        [HttpPost]
        public DataRes<bool> Delete(long id)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            var r = bll.ChangeSotpStatus(string.Format("id={0}",id), null);
            if (!r)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "删除失败";
            }

            return res;
        }


        /// <summary>
        /// 获取用户权限
        /// </summary>
        /// <returns></returns>
        [Route("GetFunctions/{code}")]
        [HttpPost]
        public DataRes<IEnumerable<string>> GetFunctions(string code)
        {
            DataRes<IEnumerable<string>> res = new DataRes<IEnumerable<string>>() { code = ResCode.Success };

            var list = userFunctionBLL.GetList(string.Format("userCode='{0}'",code));
            res.data = list.Select(p=>p.FunctionCode);

            return res;
        }

        /// <summary>
        /// 保存用户权限
        /// </summary>
        /// <returns></returns>
        [Route("SaveFunctions/{code}")]
        [HttpPost]
        public DataRes<bool> SaveFunctions(string code, [FromBody]List<string> functions)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            List<Pub_UserFunction> list = new List<Pub_UserFunction>();
            functions.ForEach(p => { list.Add(new Pub_UserFunction() { FunctionCode = p,UserCode = code }); });
            var r = bll.SaveFunctions(code, list);
            if (!r.Item1)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "保存失败";
            }

            return res;
        }

    }
}