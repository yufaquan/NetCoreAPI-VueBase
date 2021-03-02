//using Common;
//using Entity;
//using IService;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace Bussiness.Mangement
//{
//    public class GroupBookingBussiness
//    {
//        private IGroupBookingService _service;
//        public GroupBookingBussiness()
//        {
//            _service = ServiceHelp.GetGroupBookingService;
//        }
//        public static GroupBookingBussiness Init { get => new GroupBookingBussiness(); }


//        /// <summary>
//        /// 获取分页数据
//        /// </summary>
//        /// <param name="filter">过滤条件</param>
//        /// <param name="page">第几页</param>
//        /// <param name="limit">每页多少个</param>
//        /// <param name="total">总数</param>
//        /// <returns></returns>
//        public List<GroupBooking> GetPageList(GroupBooking filter, int page, int limit, ref int total)
//        {
//            System.Linq.Expressions.Expression<Func<GroupBooking, bool>> where = null;
//            //if (filter.UserId>0)
//            //{
//            //    where = where.ExpressionAnd(x => x.UserId==filter.UserId);
//            //}
//            return ServiceHelp.GetGroupBookingService.GetPageList(where, page, limit, ref total, x => x.CreatedAt, SqlSugar.OrderByType.Desc).ToList();
//        }

//        /// <summary>
//        /// 创建
//        /// </summary>
//        /// <param name="data"></param>
//        /// <param name="errorMessage"></param>
//        /// <returns>Null：失败；</returns>
//        public GroupBooking Add(GroupBooking data, out string errorMessage)
//        {
//            if (!VerifyData(data, out errorMessage))
//            {
//                return null;
//            }
//            var rGroupBooking = _service.Add(data);
//            if (rGroupBooking == null)
//            {
//                errorMessage = "创建失败。";
//                return null;
//            }
//            //记录操作日志
//            Task task = ServiceHelp.GetLogService.WriteEventLogCreateAsync(typeof(GroupBooking), rGroupBooking.Id, rGroupBooking.ToJsonString());
//            return rGroupBooking;
//        }

//        /// <summary>
//        /// 修改
//        /// </summary>
//        /// <param name="data"></param>
//        /// <param name="errorMessage"></param>
//        /// <returns>Null：失败；</returns>
//        public GroupBooking Edit(GroupBooking data, out string errorMessage)
//        {
//            if (!VerifyData(data, out errorMessage))
//            {
//                return null;
//            }
//            if (_service.GetById(data.Id) == null)
//            {
//                errorMessage = "未查询到数据！可能已被删除。";
//                return null;
//            }
//            var rGroupBooking = _service.Edit(data);
//            if (rGroupBooking == null)
//            {
//                errorMessage = "修改失败。";
//                return null;
//            }
//            //记录操作日志
//            Task task = ServiceHelp.GetLogService.WriteEventLogEditAsync(typeof(GroupBooking), rGroupBooking.Id, rGroupBooking.ToJsonString());
//            return rGroupBooking;
//        }

//        /// <summary>
//        /// 逻辑删除
//        /// </summary>
//        /// <param name="id"></param>
//        /// <param name="errorMessage"></param>
//        /// <returns>True：成功；</returns>
//        public bool Delete(int id, out string errorMessage)
//        {
//            errorMessage = string.Empty;
//            if (_service.GetById(id) == null)
//            {
//                errorMessage = "未查询到数据！可能已被删除。";
//                return false;
//            }
//            var rb = _service.DeleteById(id);
//            if (rb)
//            {
//                //删除成功，记录日志
//                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(GroupBooking), id);
//            }
//            return rb;
//        }

//        /// <summary>
//        /// 检测数据
//        /// </summary>
//        /// <param name="data">数据</param>
//        /// <param name="errorMessage">错误信息</param>
//        /// <returns>True：校验通过；</returns>
//        private bool VerifyData(GroupBooking data, out string errorMessage)
//        {
//            errorMessage = string.Empty;
//            if (data == null)
//            {
//                errorMessage = "请检查是否传入数据。";
//                return false;
//            }
//            return true;
//        }
//    }
//}
