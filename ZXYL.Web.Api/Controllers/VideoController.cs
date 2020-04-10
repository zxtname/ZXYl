using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using ZXYL.BLL;
using ZXYL.Model;

namespace ZXYL.Web.Api.Controllers
{
    [AllowAnonymous]
    [RoutePrefix("api/Video")]
    public class VideoController : ApiController
    {       
        private string _secretKey = "123456";//上传视频的密钥

        /// <summary>
        /// 修改Video中的数据
        /// </summary>
        /// <param name="changeVideo"></param>
        /// <returns></returns>
        public DataRes<dynamic> PutVideo(Pub_video changeVideo)
        {
            var result = new Pub_videoBLL().Update(changeVideo);
            return new DataRes<dynamic>
            {
                data = result ? "修改成功" : "修改失败"
            };
        }

        /// <summary>
        /// 根据课程id获取相应的视频
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //[HttpGet, Route("GetVideoByCourseId")]
        public Result GetVideoByCourseId(string id)
        {
            int ID;
            if (!int.TryParse(id,out ID)) { return new Result { Code = -1, Message = "error", Data = "参数错误" }; }
            string whereSql = $"id={id}";
            var videos = new Pub_videoBLL().GetList(whereSql);
            if (videos.Count==0)
            {
                return new Result { Code = 1, Message = "success", Data = "该用户没有课程或者不存在该用户" };
            }
            return new Result { Code = 1,Message="success",Data=videos };
        }
        
        /// <summary>
        /// 上传视频，可自己调节存放目录
        /// </summary>
        /// <returns></returns>
        [HttpPost,Route("UploadVideo")]
        public IHttpActionResult UploadVideo()
        {
            try
            {
                var secretKey = HttpContext.Current.Request["SecretKey"];
                if (secretKey == null || !_secretKey.Equals(secretKey))
                    return Ok(new Result(-1, "验证身份失败"));
                var files = HttpContext.Current.Request.Files;
                if (files == null || files.Count == 0)
                    return Ok(new Result(-1, "请选择视频"));
                var file = files[0];
                if (file == null)
                    return Ok(new Result(-1, "请选择上传的视频"));
                //存储的路径             
                var foldPath = HttpContext.Current.Request["FoldPath"];
                if (foldPath == null)
                    foldPath = "/Upload";
                foldPath = "/UploadFile" + "/" + foldPath;
                if (foldPath.Contains("../"))
                    foldPath = foldPath.Replace("../", "");
                //校验是否有该文件夹 
                var mapPath = AppDomain.CurrentDomain.BaseDirectory + foldPath;
                if (!Directory.Exists(mapPath))
                    Directory.CreateDirectory(mapPath);
                //获取文件名和文件扩展名
                var extension = Path.GetExtension(file.FileName);
                if (extension == null || !".ogg|.flv|.avi|.wmv|.rmvb|.mov|.mp4".Contains(extension.ToLower()))
                    return Ok(new Result(-1, "格式错误"));

                string newFileName = Guid.NewGuid() + extension;
                string absolutePath = string.Format("{0}/{1}", foldPath, newFileName);
                file.SaveAs(AppDomain.CurrentDomain.BaseDirectory + absolutePath);

                string fileUrl = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Authority, absolutePath);
                return Json(new Result(0, "success", fileUrl));
            }
            catch (Exception e)
            {
                //Logger.Error("UploadVideo is error", GetType(), e);
                return Json(new Result(-1, "上传失败"));
            }
        }

        
        /// <summary>
        /// 返回的结果
        /// </summary>
        public class Result
        {
            public Result() { }
            public Result(int c, string m) {
                Code = c;
                Message = m;
            }
            public Result(int c, string m,dynamic d)
            {
                Code = c;
                Message = m;
                Data = d;
            }
            /// <summary>
            /// 0请求数据错误 1 成功 -1失败
            /// </summary>
            public int Code { get; set; }

            public string Message { get; set; }

            public dynamic Data { get; set; }
        }
    }
}
