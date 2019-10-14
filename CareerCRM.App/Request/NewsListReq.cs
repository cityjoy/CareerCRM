using System;
using System.ComponentModel;
namespace CareerCRM.App.Request
{
    public class NewsListReq : PageReq
    {


        /// <summary>
        ///名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        ///起始时间
        /// </summary>
        [Description("起始时间")]
        public DateTime StartCreateTime { get; set; }
        /// <summary>
        ///结束时间
        /// </summary>
        [Description("结束时间")]
        public DateTime EndCreateTime { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [Description("是否发布")]
        public bool? IsPublish { get; set; }

    }
}