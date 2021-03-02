using System;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using Commons;
using SqlSugar;

namespace Entity
{
    //需要

    ///<summary>
    ///系统配置
    ///</summary>
    [YDisplay("系统配置")]
    [KnownType(typeof(Configuration))]
    public class Configuration
    {
        /// <summary>
        /// 附件上传大小限制（兆）
        /// 为0无限制
        /// </summary>
        [YDisplay("附件上传大小限制（兆）")]
        public int LimitUpFileSize { get; set; }
        /// <summary>
        /// 产品拼团上限
        /// </summary>
        [YDisplay("产品拼团上限")]
        public int ProductGroupMax { get; set; }

        /// <summary>
        /// 预售金
        /// </summary>
        [YDisplay("预售金")]
        public decimal BookingPrice { get; set; }

        /// <summary>
        /// 是否记录API日志
        /// </summary>
        [YDisplay("是否记录API日志")]
        public bool IsWriteAPILog { get; set; }

    }


    [SugarTable("sys_configuration")]
    public class SysConfiguration : BaseModel
    {
        public string Key { get; set; }
        public object Value { get; set; }
        public string Description { get; set; }
    }
}
