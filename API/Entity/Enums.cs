using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Entity
{
    public partial class Enums
    {

        /// <summary>
        /// 拼团状态
        /// 开团之后变成开团中
        /// 付款之后变成等待拼团中
        /// 加入团队并付款则拼团成功
        /// </summary>
        public enum GroupStatus
        {
            /// <summary>
            /// Null
            /// </summary>
            [Display(Name = "Null")]
            Null,
            /// <summary>
            /// 开团中
            /// </summary>
            [Display(Name = "开团中")]
            Starting,
            /// <summary>
            /// 等待拼团中
            /// </summary>
            [Display(Name = "等待拼团中")]
            Waiting,
            /// <summary>
            /// 拼团成功
            /// </summary>
            [Display(Name = "拼团成功")]
            Success,
            /// <summary>
            /// 已取消
            /// </summary>
            [Display(Name = "已取消")]
            Cancel
        }


    }
}
