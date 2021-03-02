using System;
using System.Threading.Tasks;
using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.AdvancedAPIs;

namespace WeChatRelated.WXOpen
{
    /// <summary>
    /// 订阅消息
    /// </summary>
    public class SubscribeMessage
    {
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.WxOpenAppId;//与微信小程序后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。
        public static readonly string TenPayV3_Key = Config.SenparcWeixinSetting.TenPayV3_Key;
        public static readonly string TenPayV3_MchId = Config.SenparcWeixinSetting.TenPayV3_MchId;
        public static readonly string TenPayV3_TenpayNotify = Config.SenparcWeixinSetting.TenPayV3_TenpayNotify;
        //public async Task<HttpResult> SubscribeGroupMessage(string openId, string templateId = "rI7K6acoSX046-9sYFveybre0cBoVdrbkxROKBNYdY0")
        //{
            
        //    await Task.Delay(1000);//停1秒钟，实际开发过程中可以将权限存入数据库，任意时间发送。

        //    var templateMessageData = new TemplateMessageData();
        //    templateMessageData["thing1"] = new TemplateMessageDataValue("微信公众号+小程序快速开发");
        //    templateMessageData["time5"] = new TemplateMessageDataValue(SystemTime.Now.ToString("yyyy年MM月dd日 HH:mm"));
        //    templateMessageData["thing6"] = new TemplateMessageDataValue("盛派网络研究院");
        //    templateMessageData["thing7"] = new TemplateMessageDataValue("第二部分课程正在准备中，尽情期待");

        //    var page = "pages/index/index";
        //    //templateId也可以由后端指定

        //    try
        //    {
        //        var result = await MessageApi.SendSubscribeAsync(WxOpenAppId, openId, templateId, templateMessageData, page);
        //        if (result.errcode == ReturnCode.请求成功)
        //        {
        //            return Json(new { success = true, msg = "消息已发送，请注意查收" });
        //        }
        //        else
        //        {
        //            return Json(new { success = false, msg = result.errmsg });
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, msg = ex.Message });
        //    }
        //}
    }
}
