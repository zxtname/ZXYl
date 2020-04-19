using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZXYL.DAL;
using ZXYL.Model;

namespace ZXYL.BLL
{
    public partial class Pub_courseBLL
    {
        Pub_courseDAL dal = new Pub_courseDAL();
        public bool ChangeSotpStatus(string where, object pms)
        {
            return dal.ChangeSotpStatus(where);
        }
        public Tuple<bool, string> Edit(Pub_course model)
        {
            var r = Update(model);
            if (!r)
            {
                return Tuple.Create(false, "保存用户失败！");
            }

            return Tuple.Create(true, "保存成功");
        }
        public List<Pub_course> GetMedicalHomeInfo(int ismedical)
        {
            return dal.GetMedicalHomeInfo(ismedical);
        }
        public bool CourseClickCountAdd(int courseId)
        {
            return dal.CourseClickCountAdd(courseId);
        }


    }
}
