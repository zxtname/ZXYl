using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXYL.Model;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace ZXYL.DAL
{
    public partial class Pub_courseDAL
    {
        public List<Pub_course> GetMedicalHomeInfo(int ismedical)
        {
            List<Pub_course> courses = new List<Pub_course>();
            using (IDbConnection db = new SqlConnection(DapperHelper.ConnStr))
            {
                courses = db.Query<Pub_course>("dbo.p_GetHomeInfo",//存储过程的名字
                    new { ismadical = ismedical },//存储过程的参数
                    commandType: CommandType.StoredProcedure
                    ).ToList();
            }
            return courses;
        }

        /// <summary>
        /// 点击触发点击量
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool CourseClickCountAdd(int id)
        {
            var dic = new Dictionary<string, object>() ;
            List<string> proName = new List<string>();
            var sql = "dbo.p_clickAdd";//放存储过程或者sql
            DynamicParameters p = new DynamicParameters();
            p.Add("@courseId", id);
            dic.Add(sql, p);
            proName.Add(sql);
            return DapperHelper.ExecTransaction(dic,proName);
            #region 手动方法
            //using (IDbConnection db = new SqlConnection(DapperHelper.ConnStr))
            //{
            //    db.Open();
            //    using (var transaction = db.BeginTransaction())
            //    {
            //        try
            //        {
            //            db.Execute("dbo.p_clickAdd", new { courseId = id }, transaction: transaction, commandType: CommandType.StoredProcedure);
            //            transaction.Commit();
            //            return true;
            //        }
            //        catch (Exception ex)
            //        {
            //            transaction.Rollback();
            //            return false;
            //        }
            //        finally
            //        {
            //            db.Close();
            //        }
            //    }
            //} 
            #endregion          
        }
    }
}
