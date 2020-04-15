using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZXYL.DAL
{
    public partial class Pub_courseDAL
    {
        /// <summary>
        /// 修改删除状态
        /// </summary>
        /// <param name="where"></param>
        /// <returns></returns>
        public bool ChangeSotpStatus(string where, object pms)
        {
            string sql = "UPDATE Pub_course SET StopFlag =1 ";
            if (string.IsNullOrWhiteSpace(where))
            {
                return false;
            }

            sql += " where " + where;

            return DapperHelper.Excute(sql, pms) > 0;
        }
    }
}
