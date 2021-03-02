using Senparc.Weixin.TenPay.V3;//DPBMARK TenPay DPBMARK_END
using Senparc.Weixin;
using Senparc.Weixin.Exceptions;
using System;
using Entity;
using Senparc.CO2NET.Extensions;

namespace WeChatRelated.WXOpen
{
    public class WXOrderService
    {
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.WxOpenAppId;//与微信小程序后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。
        public static readonly string TenPayV3_Key = Config.SenparcWeixinSetting.TenPayV3_Key;
        public static readonly string TenPayV3_MchId = Config.SenparcWeixinSetting.TenPayV3_MchId;
        public static readonly string TenPayV3_TenpayNotify = Config.SenparcWeixinSetting.TenPayV3_TenpayNotify;

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="transactionId">微信订单id</param>
        /// <param name="orderNumber">商户订单id</param>
        /// <returns></returns>
        public OrderQueryResult OrderQuery(string transactionId, string orderNumber)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            ////设置订单参数
            TenPayV3OrderQueryRequestData data = new TenPayV3OrderQueryRequestData(WxOpenAppId, TenPayV3_MchId, transactionId, nonceStr, orderNumber, TenPayV3_Key);
            OrderQueryResult result = TenPayV3.OrderQuery(data);
            return result;
        }

        /// <summary>
        /// 关闭订单接口
        /// </summary>
        /// <param name="orderNumber">商户订单id</param>
        /// <returns></returns>
        public CloseOrderResult CloseOrder(string orderNumber)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            //设置package订单参数
            TenPayV3CloseOrderRequestData data = new TenPayV3CloseOrderRequestData(WxOpenAppId, TenPayV3_MchId, orderNumber, TenPayV3_Key, nonceStr);
            CloseOrderResult result = TenPayV3.CloseOrder(data);
            return result;
        }

        ///// <summary>
        ///// 退款申请接口
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult Refund(Order order)
        //{
        //    try
        //    {
        //        WeixinTrace.SendCustomLog("进入退款流程", "1");

        //        string nonceStr = TenPayV3Util.GetNoncestr();

        //        string outTradeNo = order.OrderNumber;

        //        Senparc.CO2NET.Trace.SenparcTrace.SendCustomLog("进入退款流程", "2 outTradeNo：" + outTradeNo);

        //        string outRefundNo = "OutRefunNo-" + SystemTime.Now.Ticks;
        //        int totalFee = (int)(order.Price * 100);
        //        int refundFee = totalFee;
        //        string opUserId = TenPayV3_MchId;
        //        var notifyUrl = "https://sdk.weixin.senparc.com/TenPayV3/RefundNotifyUrl";
        //        var dataInfo = new TenPayV3RefundRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, TenPayV3Info.Key,
        //            null, nonceStr, null, outTradeNo, outRefundNo, totalFee, refundFee, opUserId, null, notifyUrl: notifyUrl);


        //        #region 新方法（Senparc.Weixin v6.4.4+）
        //        var result = TenPayV3.Refund(_serviceProvider, dataInfo);//证书地址、密码，在配置文件中设置，并在注册微信支付信息时自动记录
        //        #endregion

        //        WeixinTrace.SendCustomLog("进入退款流程", "3 Result：" + result.ToJson());
        //        //return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));

        //        throw;
        //    }


        //}

    }
}
