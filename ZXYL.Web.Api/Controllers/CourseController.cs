using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ZXYL.Model;
using ZXYL.BLL;
using System.Text;

namespace ZXYL.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Course")]
    public class CourseController : BaseController
    {
        private string preImgUrl = "http://localhost:80/image/";
        private Pub_courseBLL courseBll = new Pub_courseBLL();
        private V_PubCourse_ClassfyBLL CourseClassfyBLL = new V_PubCourse_ClassfyBLL();
        /// <summary>
        /// 获取首页信息
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet, Route("GetHomePageInfo")]
        public DataRes<dynamic> GetHomePageInfo()
        {
            var result = new DataRes<dynamic>();
            var getInfo = CourseClassfyBLL.GetList("ismadical=1 and stopFlag=0", limits: 8, sort: "clickCount Desc");
            var medicalCourse = getInfo.MapTo<List<V_PubCourse_Classfy>, List<Pub_course>>();
            var getInfo2 = CourseClassfyBLL.GetList("ismadical=0 and stopFlag=0", limits: 8, sort: "clickCount Desc");
            var noMedical = getInfo2.MapTo<List<V_PubCourse_Classfy>, List<Pub_course>>();
            result = new DataRes<dynamic>
            {
                data = new
                {
                    medicalCourse = medicalCourse,
                    noMedical = noMedical
                }
            };
            return result;
        }
        /// <summary>
        /// 根据课程页数获取课程不包括停用的
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("GetCourseInfo")]
        public PageDateRes<Pub_course> GetCourseInfo([FromBody]PageDataReq pageReq)
        {

            var whereStr = GetWhereStr(1);
            if (whereStr == "-1")
            {
                return new PageDateRes<Pub_course>() { code = ResCode.Error, msg = "查询参数有误！", data = null };
            }
            var courses = courseBll.GetPage(whereStr, (pageReq.field + " " + pageReq.order), pageReq.pageNum, pageReq.pageSize);
            courses.data.ForEach(p =>
            {
                if (p.imgUrl == null)
                {
                    p.imgUrl = preImgUrl + p.imgName;
                }

            });
            return courses;
        }
        /// <summary>
        /// 根据课程页数获取课程，包括括停用的
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost, Route("GetCourseInfoAdmin")]
        public PageDateRes<Pub_course> GetCourseInfoAdmin([FromBody]PageDataReq pageReq)
        {

            var whereStr = GetWhereStr(2);
            if (whereStr == "-1")
            {
                return new PageDateRes<Pub_course>() { code = ResCode.Error, msg = "查询参数有误！", data = null };
            }
            var courses = courseBll.GetPage(whereStr, (pageReq.field + " " + pageReq.order), pageReq.pageNum, pageReq.pageSize);
            courses.data.ForEach(p =>
            {
                if (p.imgUrl == null)
                {
                    p.imgUrl = preImgUrl + p.imgName;
                }

            });
            return courses;
        }

        /// <summary>
        /// 修改课程的内容
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost,Route("EditCourse")]
        public DataRes<bool> EditCourse([FromBody]Pub_course model)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };
            if (model.id == 0)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "参数错误";
                return res;
            }
            var preModel = courseBll.Get(model.id);         
            //用来遍历model
            preModel = GetUpdateModel<Pub_course>(model, preModel);

            var r = courseBll.Edit(preModel);
            if (!r.Item1)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "修改失败";
            }
            return res;

        }
        #region 私有方法
        /// <summary>
        /// 将model中改变的值赋值到premodel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="model"></param>
        /// <param name="preModel"></param>
        /// <returns></returns>
        private dynamic GetUpdateModel<T>(T model, T preModel)
        {
            List<System.Reflection.PropertyInfo> properties = model.GetType().GetProperties(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public).ToList();
            properties.ForEach(p =>
            {
                var mValue = p.GetValue(model);
                if (mValue != null)
                {
                    p.SetValue(preModel, mValue);
                }

            });
            return preModel;
        }

        private string GetWhereStr(int i)
        {
            StringBuilder sb = new StringBuilder(" 1=1 ");
            if (i==1)            
            sb.Append(" and StopFlag=0 ");
            var query = this.ControllerContext.GetWhereStr();
            if (query == "-1")
            {
                return query;
            }
            sb.AppendFormat(" and {0} ", query);

            return sb.ToString();
        }
        #endregion

        /// <summary>
        /// 获取首页中医学专区和非医学专区的数据
        /// </summary>
        /// <param name="ismedical"></param>
        /// <returns></returns>
        [HttpGet, Route("GetMedicalHomeInfo")]
        public DataRes<dynamic> GetMedicalHomeInfo(int ismedical)
        {
            var res = new DataRes<dynamic>();
            List<Pub_course> courses = courseBll.GetMedicalHomeInfo(ismedical);
            if (courses.Count == 0)
            {
                res.code = ResCode.Error;
                res.msg = "查询数据为空";
                res.data = null;
                return res;

            }
            courses.ForEach(p =>
            {
                if (string.IsNullOrWhiteSpace(p.imgUrl)) { p.imgUrl = preImgUrl + p.imgName; };
            });
            res.data = courses;
            return res;
        }

        /// <summary>
        /// 根据id删除课程
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>

        [HttpPost, Route("DeleteCourse")]
        public DataRes<bool> Delete(long id)
        {
            DataRes<bool> res = new DataRes<bool>() { code = ResCode.Success, data = true };

            var r = courseBll.ChangeSotpStatus(string.Format("id={0}", id), null);
            if (!r)
            {
                res.code = ResCode.Error;
                res.data = false;
                res.msg = "删除失败";
            }

            return res;
        }
        /// <summary>
        /// 测试用例
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost,Route("test")]
        public DataRes<dynamic> test(Pub_course model)
        {
            //courseBll.CourseClickCountAdd(model.id);
            return new DataRes<dynamic>
            {
                data = courseBll.CourseClickCountAdd(model.id)
            };
        }

    }
}
