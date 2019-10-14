using CareerCRM.Repository.Core;
using System;
using System.ComponentModel;
namespace CareerCRM.App.Response
{
    public class NewsListVM 
    {
        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        ///
        /// </summary>
        [Description("")]
        public string Name { get; set; }

        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 是否发布
        ///</summary>
        [Description("是否发布")]
        public bool IsPublish { get; set; }
    }
}