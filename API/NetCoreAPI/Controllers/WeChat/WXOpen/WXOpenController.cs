using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Cache;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.Sns;
using Senparc.Weixin.WxOpen.Containers;
using Senparc.Weixin.WxOpen.Entities;
using Senparc.Weixin.WxOpen.Entities.Request;
using Senparc.Weixin.WxOpen.Helpers;
using System;
using System.IO;
using Senparc.Weixin.TenPay.V3;//DPBMARK TenPay DPBMARK_END
using Senparc.CO2NET.Utilities;
using System.Threading.Tasks;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp;
using Senparc.Weixin.MP;
using Senparc.Weixin.Entities.TemplateMessage;
using Senparc.CO2NET.AspNet.HttpUtility;
using Senparc.Weixin;
using Senparc.Weixin.WxOpen.AdvancedAPIs;
using Authorization;
using Bussiness.Mangement;
using Entity;
using System.Collections.Generic;
using Bussiness;
using Senparc.Weixin.MP.AdvancedAPIs.TemplateMessage;
using System.Text;
using Senparc.Weixin.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Senparc.CO2NET.Helpers;
using System.Xml.Linq;
using Microsoft.OpenApi.Extensions;

namespace NetCoreAPI.Controllers.WeChat.WXOpen
{
    /// <summary>
    /// 微信小程序Controller
    /// </summary>
    [Route("wx/[controller]/[action]")]
    [ApiController]
    public partial class WxOpenController : Controller
    {
        public static readonly string Token = Config.SenparcWeixinSetting.WxOpenToken;//与微信小程序后台的Token设置保持一致，区分大小写。
        public static readonly string EncodingAESKey = Config.SenparcWeixinSetting.WxOpenEncodingAESKey;//与微信小程序后台的EncodingAESKey设置保持一致，区分大小写。
        public static readonly string WxOpenAppId = Config.SenparcWeixinSetting.WxOpenAppId;//与微信小程序后台的AppId设置保持一致，区分大小写。
        public static readonly string WxOpenAppSecret = Config.SenparcWeixinSetting.WxOpenAppSecret;//与微信小程序账号后台的AppId设置保持一致，区分大小写。
        public static readonly string TenPayV3_Key = Config.SenparcWeixinSetting.TenPayV3_Key;
        public static readonly string TenPayV3_MchId = Config.SenparcWeixinSetting.TenPayV3_MchId;
        public static readonly string TenPayV3_TenpayNotify = Config.SenparcWeixinSetting.TenPayV3_TenpayNotify;


        readonly Func<string> _getRandomFileName = () => SystemTime.Now.ToString("yyyyMMdd-HHmmss") + Guid.NewGuid().ToString("n").Substring(0, 6);

        private readonly IServiceProvider _serviceProvider;
        public WxOpenController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        ///// <summary>
        ///// GET请求用于处理微信小程序后台的URL验证
        ///// </summary>
        ///// <param name="postModel"></param>
        ///// <param name="echostr"></param>
        ///// <returns></returns>
        //[HttpGet]
        //[ActionName("Index")]
        //public ActionResult Get(PostModel postModel, string echostr)
        //{
        //    if (CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
        //    {
        //        return Content(echostr); //返回随机字符串则表示验证通过
        //    }
        //    else
        //    {
        //        return Content("failed:" + postModel.Signature + "," + Senparc.Weixin.MP.CheckSignature.GetSignature(postModel.Timestamp, postModel.Nonce, Token) + "。" +
        //            "如果你在浏览器中看到这句话，说明此地址可以被作为微信小程序后台的Url，请注意保持Token一致。");
        //    }
        //}

        ///// <summary>
        ///// 用户发送消息后，微信平台自动Post一个请求到这里，并等待响应XML。
        ///// </summary>
        //[HttpPost]
        //[ActionName("Index")]
        //public ActionResult Post(PostModel postModel)
        //{
        //    if (!CheckSignature.Check(postModel.Signature, postModel.Timestamp, postModel.Nonce, Token))
        //    {
        //        return Content("参数错误！");
        //    }

        //    postModel.Token = Token;//根据自己后台的设置保持一致
        //    postModel.EncodingAESKey = EncodingAESKey;//根据自己后台的设置保持一致
        //    postModel.AppId = WxOpenAppId;//根据自己后台的设置保持一致（必须提供）

        //    //v4.2.2之后的版本，可以设置每个人上下文消息储存的最大数量，防止内存占用过多，如果该参数小于等于0，则不限制
        //    var maxRecordCount = 10;

        //    var logPath = ServerUtility.ContentRootMapPath(string.Format("~/App_Data/WxOpen/{0}/", SystemTime.Now.ToString("yyyy-MM-dd")));
        //    if (!Directory.Exists(logPath))
        //    {
        //        Directory.CreateDirectory(logPath);
        //    }

        //    //自定义MessageHandler，对微信请求的详细判断操作都在这里面。
        //    var messageHandler = new CustomWxOpenMessageHandler(Request.GetRequestMemoryStream(), postModel, maxRecordCount);


        //    try
        //    {
        //        /* 如果需要添加消息去重功能，只需打开OmitRepeatedMessage功能，SDK会自动处理。
        //         * 收到重复消息通常是因为微信服务器没有及时收到响应，会持续发送2-5条不等的相同内容的RequestMessage*/
        //        messageHandler.OmitRepeatedMessage = true;

        //        //测试时可开启此记录，帮助跟踪数据，使用前请确保App_Data文件夹存在，且有读写权限。
        //        messageHandler.SaveRequestMessageLog();//记录 Request 日志（可选）

        //        messageHandler.Execute();//执行微信处理过程（关键）

        //        messageHandler.SaveResponseMessageLog();//记录 Response 日志（可选）

        //        //return Content(messageHandler.ResponseDocument.ToString());//v0.7-
        //        return new FixWeixinBugWeixinResult(messageHandler);//为了解决官方微信5.0软件换行bug暂时添加的方法，平时用下面一个方法即可
        //                                                            //return new WeixinResult(messageHandler);//v0.8+
        //    }
        //    catch (Exception ex)
        //    {
        //        using (TextWriter tw = new StreamWriter(ServerUtility.ContentRootMapPath("~/App_Data/Error_WxOpen_" + _getRandomFileName() + ".txt")))
        //        {
        //            tw.WriteLine("ExecptionMessage:" + ex.Message);
        //            tw.WriteLine(ex.Source);
        //            tw.WriteLine(ex.StackTrace);
        //            //tw.WriteLine("InnerExecptionMessage:" + ex.InnerException.Message);

        //            if (messageHandler.ResponseDocument != null)
        //            {
        //                tw.WriteLine(messageHandler.ResponseDocument.ToString());
        //            }

        //            if (ex.InnerException != null)
        //            {
        //                tw.WriteLine("========= InnerException =========");
        //                tw.WriteLine(ex.InnerException.Message);
        //                tw.WriteLine(ex.InnerException.Source);
        //                tw.WriteLine(ex.InnerException.StackTrace);
        //            }

        //            tw.Flush();
        //            tw.Close();
        //        }
        //        return Content("");
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="nickName"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult RequestData(string nickName)
        //{
        //    var data = new
        //    {
        //        msg = string.Format("服务器时间：{0}，昵称：{1}", SystemTime.Now.LocalDateTime, nickName)
        //    };
        //    return Json(data);
        //}

        /// <summary>
        /// wx.login登陆成功之后发送的请求
        /// </summary>
        /// <param name="code"></param>
        /// <returns>sessionId：获取用户资料的key</returns>
        [HttpPost]
        public ActionResult OnLogin(string code)
        {
            try
            {
                var jsonResult = SnsApi.JsCode2Json(WxOpenAppId, WxOpenAppSecret, code);
                if (jsonResult.errcode == ReturnCode.请求成功)
                {
                    //使用SessionContainer管理登录信息（推荐）
                    var unionId = jsonResult.unionid;
                    var sessionBag = SessionContainer.UpdateSession(null, jsonResult.openid, jsonResult.session_key, unionId);

                    //注意：生产环境下SessionKey属于敏感信息，不能进行传输！
                    //return Json(new { success = true, msg = "OK", sessionId = sessionBag.Key });
                    return Json(HttpResult.Success(new { sessionId = sessionBag.Key }));
                }
                else
                {
                    //return Json(new { success = false, msg = jsonResult.errmsg });
                    return Json(HttpResult.WeChatError(jsonResult.errmsg, null));
                }
            }
            catch (Exception ex)
            {
                return Json(HttpResult.WeChatError(ex.Message, null));
            }

        }

        //[HttpPost]
        //public ActionResult CheckWxOpenSignature(string sessionId, string rawData, string signature)
        //{
        //    try
        //    {
        //        var checkSuccess = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.CheckSignature(sessionId, rawData, signature);
        //        return Json(new { success = checkSuccess, msg = checkSuccess ? "签名校验成功" : "签名校验失败" });
        //    }
        //    catch (Exception ex)
        //    {
        //        return Json(new { success = false, msg = ex.Message });
        //    }
        //}


        /// <summary>
        /// 解密用户资料
        /// </summary>
        /// <param name="type">USERINFO//wx.getUserInfo()</param>
        /// <param name="sessionId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> DecodeEncryptedData(string type, string sessionId, string encryptedData, string iv)
        {
            DecodeEntityBase decodedEntity = null;

            try
            {
                switch (type.ToUpper())
                {
                    case "USERINFO"://wx.getUserInfo()
                        decodedEntity = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecodeUserInfoBySessionId(
                            sessionId,
                            encryptedData, iv);
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                WeixinTrace.SendCustomLog("EncryptHelper.DecodeUserInfoBySessionId 方法出错",
                    $@"sessionId: {sessionId}
encryptedData: {encryptedData}
iv: {iv}
sessionKey: { (await SessionContainer.CheckRegisteredAsync(sessionId)
                ? (await SessionContainer.GetSessionAsync(sessionId)).SessionKey
                : "未保存sessionId")}

异常信息：
{ex.ToString()}
");
            }

            //检验水印
            var checkWatermark = false;
            if (decodedEntity != null)
            {
                checkWatermark = decodedEntity.CheckWatermark(WxOpenAppId);

                //保存用户信息（可选）
                if (checkWatermark && decodedEntity is DecodedUserInfo decodedUserInfo)
                {
                    var sessionBag = await SessionContainer.GetSessionAsync(sessionId);
                    if (sessionBag != null)
                    {
                        await SessionContainer.AddDecodedUserInfoAsync(sessionBag, decodedUserInfo);
                    }
                }
            }


            //注意：此处仅为演示，敏感信息请勿传递到客户端！
            return Json(HttpResult.Success(string.Format("水印验证：{0}",checkWatermark ? "通过" : "不通过"),new {decodedEntity}));
        }

        //[HttpPost]
        //public async Task<IActionResult> TemplateTest(string sessionId, string formId)
        //{
        //    //注意：2020年01月10日起，新发布的小程序将不能使用模板消息，请迁移至“订阅消息”功能。

        //    var templateMessageService = new TemplateMessageService();
        //    try
        //    {
        //        var sessionBag = await templateMessageService.RunTemplateTestAsync(WxOpenAppId, sessionId, formId);

        //        return Json(new { success = true, msg = "发送成功，请返回消息列表中的【服务通知】查看模板消息。\r\n点击模板消息还可重新回到小程序内。" });
        //    }
        //    catch (Exception ex)
        //    {
        //        var sessionBag = await SessionContainer.GetSessionAsync(sessionId);
        //        var openId = sessionBag != null ? sessionBag.OpenId : "用户未正确登陆";

        //        return Json(new { success = false, openId = openId, formId = formId, msg = ex.Message });
        //    }
        //}

        /// <summary>
        /// 解密电话号码 并登录
        /// 调用此方法之前需要先进行login和getUserInfo 以及用户信息解密
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DecryptPhoneNumber(string sessionId, string encryptedData, string iv)
        {
            var sessionBag = SessionContainer.GetSession(sessionId);
            try
            {
                var phoneNumber = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecryptPhoneNumber(sessionId, encryptedData, iv);

                //throw new WeixinException("解密PhoneNumber异常测试");//启用这一句，查看客户端返回的异常信息

                //注册并登录
                string errorMessage = string.Empty;
                try
                {
                    User user = new User();
                    var userInfo = sessionBag.DecodedUserInfo;
                    userInfo = userInfo == null ? new DecodedUserInfo() : userInfo;
                    user.Name = userInfo.nickName;
                    user.NickName = userInfo.nickName;
                    user.Sex = (Enums.Sex)userInfo.gender;
                    user.UnionId = string.IsNullOrWhiteSpace(sessionBag.UnionId)? userInfo.unionId : sessionBag.UnionId;
                    user.Mobile = phoneNumber.purePhoneNumber;
                    user.Area = string.Join('-', new List<string>() { userInfo.country, userInfo.province, userInfo.city });
                    var userToken= UserBussiness.Init.WXOpenRLogin(user,(string.IsNullOrWhiteSpace(sessionBag.OpenId) ? userInfo.openId : sessionBag.OpenId), out errorMessage);
                    if (string.IsNullOrWhiteSpace(userToken) || !string.IsNullOrWhiteSpace(errorMessage))
                    {
                        return Json(HttpResult.WeChatError(errorMessage, new { userToken }));
                    }
                    return Json(HttpResult.Success(new { userToken }));
                }
                catch (Exception ex)
                {
                    return Json(HttpResult.WeChatError(ex.Message,null));
                    throw;
                }
            }
            catch (Exception ex)
            {
                //return Json(new { success = false, msg = ex.Message });
                return Json(HttpResult.WeChatError(ex.Message, null));
            }
        }


        /// <summary>
        /// 解密电话号码 并登录
        /// 调用此方法之前需要先进行login和getUserInfo
        /// </summary>
        /// <param name="decrypt"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DecryptPhoneNumberLogin([FromBody] DecryptModel decrypt)
        {
            var sessionBag = SessionContainer.GetSession(decrypt.sessionId);
            DecodedPhoneNumber phoneNumber = null;
            try
            {
                phoneNumber = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecryptPhoneNumber(decrypt.sessionId, decrypt.encryptedData, decrypt.iv);

                //throw new WeixinException("解密PhoneNumber异常测试");//启用这一句，查看客户端返回的异常信息
                if (phoneNumber==null)
                {
                    throw new WeixinException("解密PhoneNumber异常测试,PhoneNumber为空.");
                }
            }
            catch (Exception ex)
            {
                //return Json(new { success = false, msg = ex.Message });
                return Json(HttpResult.WeChatError(ex.Message, null));
            }

            //注册并登录
            string errorMessage = string.Empty;
            try
            {
                User user = new User();
                var userInfo = decrypt.userInfo;
                user.Name = userInfo.nickName;
                user.NickName = userInfo.nickName;
                user.HeadImgUrl = userInfo.avatarUrl;
                user.Sex = (Enums.Sex)userInfo.gender;
                user.UnionId = string.IsNullOrWhiteSpace(sessionBag.UnionId) ? userInfo.unionId : sessionBag.UnionId;
                user.Mobile = phoneNumber.purePhoneNumber;
                user.Area = string.Join('-', new List<string>() { userInfo.country, userInfo.province, userInfo.city });
                var userToken = UserBussiness.Init.WXOpenRLogin(user, (string.IsNullOrWhiteSpace(sessionBag.OpenId) ? userInfo.openId : sessionBag.OpenId), out errorMessage);
                if (string.IsNullOrWhiteSpace(userToken) || !string.IsNullOrWhiteSpace(errorMessage))
                {
                    return Json(HttpResult.Success(HttpResultCode.Fail, errorMessage, new { userToken }));
                }
                return Json(HttpResult.Success(new { userToken }));
            }
            catch (Exception ex)
            {
                return Json(HttpResult.Success( HttpResultCode.Fail,ex.Message, null));
            }
        }

        /// <summary>
        /// 解密运动步数
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="encryptedData"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult DecryptRunData(string sessionId, string encryptedData, string iv)
        {
            var sessionBag = SessionContainer.GetSession(sessionId);
            try
            {
                var runData = Senparc.Weixin.WxOpen.Helpers.EncryptHelper.DecryptRunData(sessionId, encryptedData, iv);

                //throw new WeixinException("解密PhoneNumber异常测试");//启用这一句，查看客户端返回的异常信息

                return Json(new { success = true, runData = runData });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });

            }
        }

        #region DPBMARK TenPay 


        ///// <summary>
        ///// 统一下单
        ///// </summary>
        ///// <param name="sessionId"></param>
        ///// <param name="orderId"></param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult GetPrepayid(string sessionId,int orderId)
        //{
        //    string errorMessage = string.Empty;
        //    //获取订单信息
        //    var order = ServiceHelp.GetOrderService.GetById(orderId);
        //    if (order==null)
        //    {
        //        errorMessage = "未查询的订单信息。";
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail, errorMessage, null));
        //    }
        //    var product = ServiceHelp.GetProductService.GetById(order.ProductId);
        //    if (product == null)
        //    {
        //        errorMessage = "未查询的产品信息。";
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail, errorMessage, null));
        //    }
        //    var group = ServiceHelp.GetGroupBookingService.GetById(order.GroupId);
        //    if (group == null)
        //    {
        //        errorMessage = "未查询的拼团信息。";
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail, errorMessage, null));
        //    }
        //    if (group.Status== Enums.GroupStatus.Success || group.Status== Enums.GroupStatus.Cancel)
        //    {
        //        errorMessage =$"无法参与当前拼团，拼团状态：{group.Status.GetDisplayName()}；您可取消订单后参与其他拼团或自己开团。";
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail, errorMessage, null));
        //    }
        //    try
        //    {
        //        var sessionBag = SessionContainer.GetSession(sessionId);
        //        if (sessionBag==null)
        //        {

        //        }
        //        var openId = sessionBag.OpenId;
        //        //订单1序列号
        //        var sp_billno = order.OrderNumber;

        //        var timeStamp = TenPayV3Util.GetTimestamp();
        //        var nonceStr = TenPayV3Util.GetNoncestr();

        //        var body = product.Title;
        //        var price = (int)(order.Price * 100);//单位：分
        //        var xmlDataInfo = new TenPayV3UnifiedorderRequestData(WxOpenAppId, Config.SenparcWeixinSetting.TenPayV3_MchId, body, sp_billno,
        //            price, HttpContext.UserHostAddress().ToString(), TenPayV3_TenpayNotify, Senparc.Weixin.TenPay.TenPayV3Type.JSAPI, openId, Config.SenparcWeixinSetting.TenPayV3_Key, nonceStr);

        //        var result = TenPayV3.Unifiedorder(xmlDataInfo);//调用统一订单接口
                
        //        WeixinTrace.SendCustomLog("统一订单接口调用结束", "请求：" + xmlDataInfo.ToJson() + "\r\n\r\n返回结果：" + result.ToJson());
        //        if (string.IsNullOrWhiteSpace(result.prepay_id))
        //        {
        //            return new JsonResult(HttpResult.WeChatError(result.err_code_des,null));
        //        }
        //        var packageStr = "prepay_id=" + result.prepay_id;

        //        //记录到缓存

        //        var cacheStrategy = CacheStrategyFactory.GetObjectCacheStrategyInstance();
        //        cacheStrategy.Set($"WxOpenUnifiedorderRequestData-{openId}", xmlDataInfo, TimeSpan.FromDays(4));//3天内可以发送模板消息
        //        cacheStrategy.Set($"WxOpenUnifiedorderResultData-{openId}", result, TimeSpan.FromDays(4));//3天内可以发送模板消息

        //        return new JsonResult(HttpResult.Success(new
        //        {
        //            prepay_id = result.prepay_id,
        //            appId = Config.SenparcWeixinSetting.WxOpenAppId,
        //            timeStamp,
        //            nonceStr,
        //            package = packageStr,
        //            //signType = "MD5",
        //            paySign = TenPayV3.GetJsPaySign(WxOpenAppId, timeStamp, nonceStr, packageStr, Config.SenparcWeixinSetting.TenPayV3_Key)
        //        }));
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(HttpResult.WeChatError(ex.Message,null));
        //    }
        //}


        ///// <summary>
        ///// JS-SDK支付回调地址（在统一下单接口中设置notify_url）
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //[AllowAnonymous]
        //public ActionResult PayNotifyUrl()
        //{
        //    WeixinTrace.Log("进入JSAPI支付回调。"+DateTime.Now.ToLongDateString());
        //    try
        //    {
        //        ResponseHandler resHandler = new ResponseHandler(HttpContext);

        //        string return_code = resHandler.GetParameter("return_code");
        //        string return_msg = resHandler.GetParameter("return_msg");

        //        string res = null;

        //        resHandler.SetKey(Config.SenparcWeixinSetting.TenPayV3_Key);
        //        //验证请求是否从微信发过来（安全）
        //        if (resHandler.IsTenpaySign() && return_code.ToUpper() == "SUCCESS")
        //        {
        //            WeixinTrace.SendCustomLog("PayNotifyUrl被访问", "验证通过");
        //            res = "success";//正确的订单处理
        //                            //直到这里，才能认为交易真正成功了，可以进行数据库操作，但是别忘了返回规定格式的消息！

        //            /* 这里可以进行订单处理的逻辑 */

        //            //获取数据
        //            try
        //            {
        //                WeixinTrace.SendCustomLog("准备进行支付结果处理", "STAER");
        //                //获取接口中需要用到的信息
        //                string transaction_id = resHandler.GetParameter("transaction_id");
        //                string out_trade_no = resHandler.GetParameter("out_trade_no");
        //                string trade_type = resHandler.GetParameter("trade_type");
        //                string result_code = resHandler.GetParameter("result_code");
        //                string time_end = resHandler.GetParameter("time_end");
        //                string nonce_str= resHandler.GetParameter("nonce_str");
        //                string sign = resHandler.GetParameter("sign");
        //                string resource = resHandler.GetParameter("resource");
        //                string openId = resHandler.GetParameter("openid");
        //                var robj = new { transaction_id , out_trade_no , trade_type , result_code, time_end, resource, sign, nonce_str , openId };
        //                WeixinTrace.SendCustomLog("获取到支付结果", robj.ToJson());

        //                if (result_code == "SUCCESS")
        //                {
        //                    string errorMessage;
        //                    //是否处理过了
        //                    bool isCL, isDY, isKT;
        //                    string openId2;
        //                    var isSuccess = OrderBussiness.Init.PayNotify(out_trade_no, transaction_id, out errorMessage, out isCL, out isDY, out openId2, out isKT);

        //                    WeixinTrace.SendCustomLog("=====支付结果操作=====", new { isSuccess, errorMessage, isCL, isDY, isKT, openId2 }.ToJson());
        //                    if (!isCL)
        //                    {
        //                        //通知拼团结果
        //                        string appId = Config.SenparcWeixinSetting.TenPayV3_AppId;//与微信公众账号后台的AppId设置保持一致，区分大小写。
        //                        try
        //                        {
        //                            var order = ServiceHelp.GetOrderService.GetBy(x => x.OrderNumber == out_trade_no);
        //                            if (order == null)
        //                            {
        //                                throw new Exception("未查询到订单信息。");
        //                            }
        //                            var product = ServiceHelp.GetProductService.GetById(order.ProductId);
        //                            if (order == null)
        //                            {
        //                                throw new Exception("未查询到产品信息。");
        //                            }

        //                            if (isDY)
        //                            {
        //                                //拼团成功订阅
        //                                var templateMessageData = new TemplateMessageData();
        //                                var page = "pages/index/index";
        //                                templateMessageData["thing1"] = new TemplateMessageDataValue(product.Title);
        //                                templateMessageData["amount5"] = new TemplateMessageDataValue(order.Price + "元");
        //                                templateMessageData["character_string4"] = new TemplateMessageDataValue(order.OrderNumber);
        //                                if (isSuccess)
        //                                {
        //                                    templateMessageData["phrase2"] = new TemplateMessageDataValue("拼团成功");
        //                                    templateMessageData["thing3"] = new TemplateMessageDataValue("请及时前往店内核销。");
        //                                    page = "pages/group/detail?id=" + order.GroupId;
        //                                }
        //                                else
        //                                {
        //                                    templateMessageData["phrase2"] = new TemplateMessageDataValue("拼团失败");
        //                                    templateMessageData["thing3"] = new TemplateMessageDataValue("已自动退款，请注意查收。");
        //                                    //申请退款
        //                                    string err;
        //                                    RefundF( order, out err);
        //                                }
        //                                var result = MessageApi.SendSubscribeAsync(WxOpenAppId, openId, "rI7K6acoSX046-9sYFveybre0cBoVdrbkxROKBNYdY0", templateMessageData, page);
        //                                //拼团成功同时通知开团人
        //                                if (!string.IsNullOrWhiteSpace(openId2) && isSuccess)
        //                                {
        //                                    var result1 = MessageApi.SendSubscribeAsync(WxOpenAppId, openId2, "rI7K6acoSX046-9sYFveybre0cBoVdrbkxROKBNYdY0", templateMessageData, page);
        //                                }
        //                            }
        //                            if (isKT && isSuccess)
        //                            {
        //                                //开团成功订阅
        //                                var templateMessageData = new TemplateMessageData();
        //                                var page = "pages/index/index";
        //                                templateMessageData["thing5"] = new TemplateMessageDataValue(product.Title);
        //                                templateMessageData["phrase1"] = new TemplateMessageDataValue("等待拼团");
        //                                templateMessageData["character_string6"] = new TemplateMessageDataValue(order.OrderNumber);
        //                                templateMessageData["thing3"] = new TemplateMessageDataValue("还差一人成团");
        //                                templateMessageData["thing4"] = new TemplateMessageDataValue("活动时间有限，请尽快邀请好友拼团哦。");
        //                                page = "pages/group/detail?id=" + order.GroupId;
        //                                var result = MessageApi.SendSubscribeAsync(WxOpenAppId, openId, "u4jw0AXUZKDuFx_zCco8FNNh9JFv_AN8x_lYR06Xv6M", templateMessageData, page);
        //                            }
        //                        }
        //                        catch (Exception ex)
        //                        {
        //                            WeixinTrace.SendCustomLog("订阅消息放失败", ex.Message);
        //                        }
        //                    }
        //                }

        //            }
        //            catch (Exception ex)
        //            {
        //                Senparc.Weixin.WeixinTrace.SendCustomLog("支付结果失败", ex.ToString());
        //            }

        //        }
        //        else
        //        {
        //            res = "wrong";//错误的订单处理
        //        }

                

        //        #region 记录日志

        //        var logDir = ServerUtility.ContentRootMapPath(string.Format("~/App_Data/TenPayNotify/{0}", SystemTime.Now.ToString("yyyyMMdd")));
        //        if (!Directory.Exists(logDir))
        //        {
        //            Directory.CreateDirectory(logDir);
        //        }

        //        var logPath = Path.Combine(logDir, string.Format("{0}-{1}-{2}.txt", SystemTime.Now.ToString("yyyyMMdd"), SystemTime.Now.ToString("HHmmss"), Guid.NewGuid().ToString("n").Substring(0, 8)));

        //        using (var fileStream = System.IO.File.OpenWrite(logPath))
        //        {
        //            var notifyXml = resHandler.ParseXML();
        //            //fileStream.Write(Encoding.Default.GetBytes(res), 0, Encoding.Default.GetByteCount(res));

        //            fileStream.Write(Encoding.Default.GetBytes(notifyXml), 0, Encoding.Default.GetByteCount(notifyXml));
        //            fileStream.Close();
        //        }

        //        #endregion


        //        string xml = string.Format(@"<xml>
        //            <return_code><![CDATA[{0}]]></return_code>
        //            <return_msg><![CDATA[{1}]]></return_msg>
        //        </xml>", return_code, return_msg);
        //        return Content(xml, "text/xml");
        //    }
        //    catch (Exception ex)
        //    {
        //        WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));
        //        throw;
        //    }
        //}



        #region 订单及退款

        /// <summary>
        /// 订单查询
        /// </summary>
        /// <param name="transactionId">微信订单id</param>
        /// <param name="orderNumber">商户订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult OrderQuery(string transactionId,string orderNumber)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            ////设置订单参数
            TenPayV3OrderQueryRequestData data = new TenPayV3OrderQueryRequestData(WxOpenAppId,TenPayV3_MchId,transactionId,nonceStr,orderNumber,TenPayV3_Key);
            OrderQueryResult result = TenPayV3.OrderQuery(data);
            return new JsonResult(HttpResult.Success(result));
        }

        /// <summary>
        /// 关闭订单接口
        /// </summary>
        /// <param name="orderNumber">商户订单id</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CloseOrder(string orderNumber)
        {
            string nonceStr = TenPayV3Util.GetNoncestr();
            //设置package订单参数
            TenPayV3CloseOrderRequestData data = new TenPayV3CloseOrderRequestData(WxOpenAppId, TenPayV3_MchId, orderNumber, TenPayV3_Key, nonceStr);
            CloseOrderResult result = TenPayV3.CloseOrder(data);
            return new JsonResult(HttpResult.Success(result));
        }

        ///// <summary>
        ///// 退款方法
        ///// </summary>
        ///// <param name="order"></param>
        ///// <param name="errorMessage"></param>
        ///// <returns></returns>
        //private bool RefundF(Order order,out string errorMessage)
        //{
        //    errorMessage = string.Empty;
        //    try
        //    {
        //        WeixinTrace.SendCustomLog("进入退款流程", "1");

        //        string nonceStr = TenPayV3Util.GetNoncestr();

        //        string outTradeNo = order.OrderNumber;

        //        WeixinTrace.SendCustomLog("进入退款流程", "2 outTradeNo：" + outTradeNo);

        //        string outRefundNo = "OutRefunNo-" + SystemTime.Now.Ticks;
        //        int totalFee = (int)(order.Price * 100);
        //        //int totalFee = 1;
        //        int refundFee = totalFee;
        //        string opUserId = TenPayV3_MchId;
        //        var notifyUrl = "http://chicface.api.yufaquan.cn/wx/WxOpen/RefundNotifyUrl";
        //        var dataInfo = new TenPayV3RefundRequestData(WxOpenAppId, TenPayV3_MchId, TenPayV3_Key,
        //            null, nonceStr, null, outTradeNo, outRefundNo, totalFee, refundFee, opUserId, null, notifyUrl: notifyUrl);

        //        WeixinTrace.SendCustomLog("进入退款流程", "3 dataInfo：" + dataInfo.ToJson());
        //        #region 旧方法
        //        //var cert = @"D:\cert\apiclient_cert_SenparcRobot.p12";//根据自己的证书位置修改
        //        //var password = TenPayV3_MchId;//默认为商户号，建议修改
        //        //var result = TenPayV3.Refund(dataInfo, cert, password);
        //        #endregion


        //        #region 新方法（Senparc.Weixin v6.4.4+）
        //        var result = TenPayV3.Refund(_serviceProvider, dataInfo);//证书地址、密码，在配置文件中设置，并在注册微信支付信息时自动记录
        //        #endregion

        //        WeixinTrace.SendCustomLog("进入退款流程", "4 Result：" + result.ToJson());

        //        if (result.result_code== "SUCCESS")
        //        {
        //            //更改订单状态为退款中、拼团如果是开团人则将拼团改为开团中
        //            var r= OrderBussiness.Init.ChangeStatus(order.Id,null, Enums.OrderStatus.Refunding, out errorMessage);
        //            if (r==null)
        //            {
        //                errorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "修改失败" : errorMessage;
        //                WeixinTrace.SendCustomLog("进入退款流程 退款状态修改失败", errorMessage);
        //                Task task= ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError() { Message = $"申请退款成功，orderNumber:{outTradeNo}；修改状态失败:"+errorMessage, LogLevel = Enums.LogLevel.Error, MsgType = "ZDY",StackTrace="" });
        //                return false;
        //            }
        //            else
        //            {
        //                return true;
        //            }
        //        }
        //        else
        //        {
        //            //记录失败原因
        //            errorMessage = result.err_code_des;
        //            Task task = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError() { Message = $"申请退款失败，orderNumber:{outTradeNo}；" + errorMessage, LogLevel = Enums.LogLevel.Error, MsgType = "ZDY" , StackTrace =result.ToJson()});
        //            return false;
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));

        //        WeixinTrace.SendCustomLog("退款流程出错",  ex.Message);
        //        errorMessage = ex.Message;
        //        return false;
        //    }
        //}


        ///// <summary>
        ///// 全额退款 
        ///// 不会影响拼团成功的数据
        ///// 如果是开团会将拼团数据变为取消
        ///// </summary>
        ///// <param name="orderId"></param>
        ///// <returns></returns>
        //[MyAuthorize(typeof(Refund<Order>))]
        //[CheckLogin]
        //[HttpPut]
        //public async Task<ActionResult> Refund(int orderId)
        //{
        //    string errorMessage = string.Empty;
        //    if (orderId==0)
        //    {
        //        errorMessage = "未查询到订单";
        //    }
        //    else
        //    {
        //        var order = ServiceHelp.GetOrderService.GetById(orderId);
        //        if (order==null)
        //        {
        //            errorMessage = "未查询到订单";
        //        }
        //        else
        //        {
        //            if (order.Status== Enums.OrderStatus.PaySuccess && !string.IsNullOrWhiteSpace(order.TransactionId))
        //            {
        //                var result = await Task.FromResult(RefundF(order, out errorMessage));
        //                if (result)
        //                {
        //                    return new JsonResult(HttpResult.Success(result));
        //                }
        //            }
        //        }
        //    }
        //    return new JsonResult(HttpResult.Success(HttpResultCode.Fail, errorMessage, null));
        //}

        ///// <summary>
        ///// 退款
        ///// </summary>
        ///// <param name="orderNumber">商家订单号</param>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Refund(string orderNumber)
        //{
        //    if (string.IsNullOrWhiteSpace(orderNumber))
        //    {
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail,"传入信息不完整",""));
        //    }
        //    var order = ServiceHelp.GetOrderService.GetBy(x => x.OrderNumber == orderNumber || x.TransactionId==orderNumber);
        //    if (order==null)
        //    {
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail, "未查询到订单信息", ""));
        //    }
        //    string errorMessage;
        //    var isSuccess= RefundF(order,out errorMessage);
        //    if (isSuccess)
        //    {
        //        return new JsonResult(HttpResult.Success(true));
        //    }
        //    else
        //    {
        //        return new JsonResult(HttpResult.Success(HttpResultCode.Fail,errorMessage,false));
        //    }
        //}

        ///// <summary>
        ///// 退款申请接口
        ///// </summary>
        ///// <returns></returns>
        //[HttpPost]
        //public ActionResult Refund()
        //{
        //    try
        //    {
        //        WeixinTrace.SendCustomLog("进入退款流程", "1");

        //        string nonceStr = TenPayV3Util.GetNoncestr();

        //        string outTradeNo = HttpContext.Session.GetString("BillNo");

        //        WeixinTrace.SendCustomLog("进入退款流程", "2 outTradeNo：" + outTradeNo);

        //        string outRefundNo = "OutRefunNo-" + SystemTime.Now.Ticks;
        //        int totalFee = int.Parse(HttpContext.Session.GetString("BillFee"));
        //        int refundFee = totalFee;
        //        string opUserId = TenPayV3Info.MchId;
        //        var notifyUrl = "https://sdk.weixin.senparc.com/TenPayV3/RefundNotifyUrl";
        //        var dataInfo = new TenPayV3RefundRequestData(TenPayV3Info.AppId, TenPayV3Info.MchId, TenPayV3Info.Key,
        //            null, nonceStr, null, outTradeNo, outRefundNo, totalFee, refundFee, opUserId, null, notifyUrl: notifyUrl);


        //        #region 新方法（Senparc.Weixin v6.4.4+）
        //        var result = TenPayV3.Refund(_serviceProvider, dataInfo);//证书地址、密码，在配置文件中设置，并在注册微信支付信息时自动记录
        //        #endregion

        //        WeixinTrace.SendCustomLog("进入退款流程", "3 Result：" + result.ToJson());
        //        ViewData["Message"] = $"退款结果：{result.result_code} {result.err_code_des}。您可以刷新当前页面查看最新结果。";
        //        return View();
        //        //return Json(result, JsonRequestBehavior.AllowGet);
        //    }
        //    catch (Exception ex)
        //    {
        //        WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));

        //        throw;
        //    }


        //}

        ///// <summary>
        ///// 退款通知地址
        ///// </summary>
        ///// <returns></returns>
//        [AllowAnonymous]
//        [HttpPost]
//        public ActionResult RefundNotifyUrl()
//        {
//            WeixinTrace.SendCustomLog("RefundNotifyUrl被访问", "IP" + HttpContext.UserHostAddress()?.ToString());

//            string responseCode = "FAIL";
//            string responseMsg = "FAIL";
//            try
//            {
//                ResponseHandler resHandler = new ResponseHandler(null);

//                string return_code = resHandler.GetParameter("return_code");
//                string return_msg = resHandler.GetParameter("return_msg");

//                WeixinTrace.SendCustomLog("跟踪RefundNotifyUrl信息", resHandler.ParseXML());

//                if (return_code == "SUCCESS")
//                {
//                    responseCode = "SUCCESS";
//                    responseMsg = "OK";

//                    string appId = resHandler.GetParameter("appid");
//                    string mch_id = resHandler.GetParameter("mch_id");
//                    string nonce_str = resHandler.GetParameter("nonce_str");
//                    string req_info = resHandler.GetParameter("req_info");

//                    var decodeReqInfo = TenPayV3Util.DecodeRefundReqInfo(req_info, TenPayV3_Key);
//                    var decodeDoc = XDocument.Parse(decodeReqInfo);

//                    //获取接口中需要用到的信息
//                    string transaction_id = decodeDoc.Root.Element("transaction_id").Value;
//                    string out_trade_no = decodeDoc.Root.Element("out_trade_no").Value;
//                    string refund_id = decodeDoc.Root.Element("refund_id").Value;
//                    string out_refund_no = decodeDoc.Root.Element("out_refund_no").Value;
//                    int total_fee = int.Parse(decodeDoc.Root.Element("total_fee").Value);
//                    int? settlement_total_fee = decodeDoc.Root.Element("settlement_total_fee") != null
//                            ? int.Parse(decodeDoc.Root.Element("settlement_total_fee").Value)
//                            : null as int?;
//                    int refund_fee = int.Parse(decodeDoc.Root.Element("refund_fee").Value);
//                    int tosettlement_refund_feetal_fee = int.Parse(decodeDoc.Root.Element("settlement_refund_fee").Value);
//                    string refund_status = decodeDoc.Root.Element("refund_status").Value;
//                    string success_time = decodeDoc.Root.Element("success_time").Value;
//                    string refund_recv_accout = decodeDoc.Root.Element("refund_recv_accout").Value;
//                    string refund_account = decodeDoc.Root.Element("refund_account").Value;
//                    string refund_request_source = decodeDoc.Root.Element("refund_request_source").Value;


//                    WeixinTrace.SendCustomLog("RefundNotifyUrl被访问", "验证通过");

//                    //进行后续业务处理
//                    //更改订单状态为已退款、拼团如果是开团人则将拼团改为开团中
//                    string errorMessage;
//                    var r = OrderBussiness.Init.ChangeStatus(0,out_trade_no, Enums.OrderStatus.Refund, out errorMessage);
//                    if (r == null)
//                    {
//                        errorMessage = string.IsNullOrWhiteSpace(errorMessage) ? "修改失败" : errorMessage;
//                        WeixinTrace.SendCustomLog("RefundNotifyUrl被访问 退款状态修改失败", errorMessage);
//                        //记录失败
//                        Task task= ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError() { Message = $"退款通知成功，orderNumber:{out_trade_no}；修改状态失败:"+errorMessage, LogLevel = Enums.LogLevel.Error, MsgType = "ZDY",StackTrace="" });
//                    }

//                }
//                else
//                {

//                    Task task = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError() { Message = $"退款通知失败，" + return_msg, LogLevel = Enums.LogLevel.Error, MsgType = "ZDY", StackTrace =new  { return_code, return_msg }.ToJson() });
//                }
//            }
//            catch (Exception ex)
//            {
//                responseMsg = ex.Message;
//                WeixinTrace.WeixinExceptionLog(new WeixinException(ex.Message, ex));
//            }

//            string xml = string.Format(@"<xml>
//<return_code><![CDATA[{0}]]></return_code>
//<return_msg><![CDATA[{1}]]></return_msg>
//</xml>", responseCode, responseMsg);
//            return Content(xml, "text/xml");
//        }

        #endregion

        #endregion DPBMARK_END


        /// <summary>
        /// 获取二维码
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="useBase64"></param>
        /// <param name="codeType"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> GetQrCode(string sessionId, string useBase64, string codeType = "1")
        {
            var sessionBag = SessionContainer.GetSession(sessionId);
            if (sessionBag == null)
            {
                return Json(new { success = false, msg = "请先登录！" });
            }

            var ms = new MemoryStream();
            var openId = sessionBag.OpenId;
            var page = "pages/QrCode/QrCode";//此接口不可以带参数，如果需要加参数，必须加到scene中
            var scene = $"OpenIdSuffix:{openId.Substring(openId.Length - 10, 10)}#{codeType}";//储存OpenId后缀，以及codeType。scene最多允许32个字符
            LineColor lineColor = null;//线条颜色
            if (codeType == "2")
            {
                lineColor = new LineColor(221, 51, 238);
            }

            var result = await Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppApi
                .GetWxaCodeUnlimitAsync(WxOpenAppId, ms, scene, page, lineColor: lineColor);
            ms.Position = 0;

            if (!useBase64.IsNullOrEmpty())
            {
                //转base64
                var imgBase64 = Convert.ToBase64String(ms.GetBuffer());
                return Json(new { success = true, msg = imgBase64, page = page });
            }
            else
            {
                //返回文件流
                return File(ms, "image/jpeg");
            }
        }

        /// <summary>
        /// 订阅消息
        /// </summary>
        /// <param name="sessionId"></param>
        /// <param name="templateId"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SubscribeMessage(string sessionId, string templateId = "xWclWkOqDrxEgWF4DExmb9yUe10pfmSSt2KM6pY7ZlU")
        {
            var sessionBag = SessionContainer.GetSession(sessionId);
            if (sessionBag == null)
            {
                return Json(new { success = false, msg = "请先登录！" });
            }

            await Task.Delay(1000);//停1秒钟，实际开发过程中可以将权限存入数据库，任意时间发送。

            var templateMessageData = new TemplateMessageData();
            templateMessageData["thing1"] = new TemplateMessageDataValue("微信公众号+小程序快速开发");
            templateMessageData["time5"] = new TemplateMessageDataValue(SystemTime.Now.ToString("yyyy年MM月dd日 HH:mm"));
            templateMessageData["thing6"] = new TemplateMessageDataValue("盛派网络研究院");
            templateMessageData["thing7"] = new TemplateMessageDataValue("第二部分课程正在准备中，尽情期待");

            var page = "pages/index/index";
            //templateId也可以由后端指定

            try
            {
                var result = await MessageApi.SendSubscribeAsync(WxOpenAppId, sessionBag.OpenId, templateId, templateMessageData, page);
                if (result.errcode == ReturnCode.请求成功)
                {
                    return Json(new { success = true, msg = "消息已发送，请注意查收" });
                }
                else
                {
                    return Json(new { success = false, msg = result.errmsg });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, msg = ex.Message });
            }
        }
    }

    /// <summary>
    /// 微信小程序解密数据
    /// </summary>
    public class DecryptModel
    {
        public string sessionId { get; set; }
        /// <summary>
        /// 敏感信息密文
        /// </summary>
        public string encryptedData { get; set; }
        /// <summary>
        /// 解密向量
        /// </summary>
        public string iv { get; set; }

        /// <summary>
        /// 用户信息
        /// </summary>
        public DecodedUserInfo userInfo { get; set; }
    }

}
