using CareerCRM.Repository.Core;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace CareerCRM.Repository.Domain
{
      [Table("News")]
    public class News : Entity
    {
        public News() {

            CreateTime = DateTime.Now;
        }

        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }
        /// <summary>
        /// 简称
        /// </summary>
        [Description("内容")]
        public string Content { get; set; }
        /// <summary>
        /// 时间
        /// </summary>
        [Description("时间")]
        public DateTime CreateTime { get; set; }


        /// <summary>
        /// 是否发布
        /// </summary>
        [Description("是否发布")]
        public bool IsPublish { get; set; }


    }
}
