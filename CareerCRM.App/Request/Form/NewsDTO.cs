using CareerCRM.Repository.Core;
using System.ComponentModel;
namespace CareerCRM.App.Request
{
    public class NewsFormDTO : Entity
    {


        /// <summary>
        /// 名称
        /// </summary>
        [Description("名称")]
        public string Name { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Description("内容")]
        public string Content { get; set; }

        /// <summary>
        /// 是否发布
        /// </summary>
        [Description("是否发布")]
        public bool IsPublish { get; set; }
    }
}



