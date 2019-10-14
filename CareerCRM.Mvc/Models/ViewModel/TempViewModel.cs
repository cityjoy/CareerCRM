using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CareerCRM.Mvc.Models.ViewModel
{
    public class TempViewModel
    {

        /// <summary>
        /// 项目
        /// </summary>
        public string ProjectName { get; set; }
        /// <summary>
        /// 模型名称
        /// </summary>
        public string ModelName { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }
        /// <summary>
        /// 数据
        /// </summary>
        public List<string> DataList { get; set; }

        /// <summary>
        /// 列表视图数据
        /// </summary>
        public List<string> ListViewDataList { get; set; }

        /// <summary>
        /// 搜索字段数据
        /// </summary>
        public List<string> SearchDataList { get; set; }
    }
}
