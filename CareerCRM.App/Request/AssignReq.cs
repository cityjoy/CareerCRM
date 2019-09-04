﻿namespace CareerCRM.App.Request
{
    /// <summary>
    /// 比如给用户分配资源，那么firstId就是用户ID，secIds就是资源ID列表
    /// </summary>
    public class AssignReq
    {
        /// <summary>
        /// 分配的关键字，比如：UserRole
        /// </summary>
        public string type { get; set; }
        public string firstId { get; set; }
        public string[] secIds { get; set; }
    }
}