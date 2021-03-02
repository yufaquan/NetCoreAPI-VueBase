using Authorization;
using Commons;
using Entity;
using IService;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bussiness.Mangement
{
    public class UserBussiness
    {
        private IUserService _service;
        public UserBussiness()
        {
            _service = ServiceHelp.GetUserService;
        }
        public static UserBussiness Init { get => new UserBussiness(); }

        /// <summary>
        /// 帐号密码登录
        /// </summary>
        /// <param name="name">电话/邮箱/名称</param>
        /// <param name="pwd">MD5一次后的密码</param>
        /// <param name="errorMessage">错误消息</param>
        /// <returns>UserToken</returns>
        public string LoginByPwd(string name,string pwd,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(pwd))
            {
                errorMessage = "请输入帐号和密码！";
                return "";
            }
            pwd = Commonfs.GetMD5_32(pwd);
            User user= ServiceHelp.GetUserService.GetBy(x => x.Name == name || x.Mobile == name || x.Email == name);
            if (user==null)
            {
                errorMessage = "未查询到此帐号.";
                return "";
            }
            if (user.PassWordMD5==pwd)
            {
                //登录成功 获取token
                var usertoken= TokenHelp.WriteUserToken(user.Id);
                if (string.IsNullOrWhiteSpace(usertoken))
                {
                    errorMessage = "登录失败,请稍候重试。";
                    return "";
                }
                else
                {
                    //配置当前登录数据
                    Current.UserId = user.Id;
                    Current.UserToken = usertoken;
                    Current.UserJson = user.ToJsonString();
                    //记录登录日志
                    var task= ServiceHelp.GetLogService.WriteEventLogToLoginAsync();
                    return usertoken;
                }
            }
            else
            {
                errorMessage = "密码错误.";
                return "";
            }
        }

        /// <summary>
        /// 其它地方系统新增、修改用户时名称重复自动添加“_MMddff”
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private User AutoName(User user)
        {
            var result = user;
            var sjk = _service.GetAllList(x => x.Name == user.Name && x.Id != user.Id);
            if (sjk!=null && sjk.Count>0)
            {
                result.Name = user.Name + "_" + DateTime.Now.ToString("MMddff");
            }
            return result;
        }

        /// <summary>
        /// 小程序注册并登录
        /// </summary>
        /// <param name="data"></param>
        /// <param name="errorMessage"></param>
        /// <returns>userToken</returns>
        public string WXOpenRLogin(User data,string openId,out string errorMessage)
        {
            errorMessage = string.Empty;
            //查看有没有注册
            User user = ServiceHelp.GetUserService.GetBy(x=>x.Mobile == data.Mobile );
            if (user==null)
            {
                try
                {
                    _service.BeginTran();
                    //注册
                    data = AutoName(data);
                    var addu = Add(data, out errorMessage);
                    if (addu == null)
                    {
                        if (string.IsNullOrWhiteSpace(errorMessage))
                        {
                            errorMessage = "用户注册失败。";
                        }
                        throw new Exception(errorMessage);
                    }
                    //添加userInfo
                    UserInfo ui = new UserInfo();
                    ui.UserId = addu.Id;
                    ui.OpenId = openId;
                    ui.ProjectType = Enums.ProjectType.WeChatApplet;
                    var addui = ServiceHelp.GetUserInfoService.Add(ui);
                    if (addui == null)
                    {
                        throw new Exception("用户注册失败。");
                    }
                    _service.CommitTran();
                    return Login(addu, ref errorMessage);
                }
                catch (Exception ex)
                {
                    _service.RollbackTran();
                    errorMessage = ex.Message;
                    return null;
                }
            }
            else
            {
                //修改用户头像等
                bool isEdit = false;
                if ((user.HeadImgUrl != data.HeadImgUrl && !string.IsNullOrWhiteSpace(data.HeadImgUrl)))
                {
                    user.HeadImgUrl = data.HeadImgUrl;
                    isEdit = true;
                }
                if ((user.Name != data.Name && !string.IsNullOrWhiteSpace(data.Name)))
                {
                    user.Name = data.Name;
                    isEdit = true;
                }
                if ((user.NickName != data.NickName && !string.IsNullOrWhiteSpace(data.NickName)))
                {
                    user.NickName = data.NickName;
                    isEdit = true;
                }
                if (isEdit)
                {
                    try
                    {
                        user = AutoName(user);
                        var redit= _service.Edit(user);
                        if (redit==null)
                        {
                            throw new Exception("修改用户信息失败");
                        }
                    }
                    catch (Exception ex)
                    {
                        var task = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError(ex, null));
                        errorMessage = ex.Message;
                        return null;
                    }
                }
                //判断有没有其它信息
                var uiService = ServiceHelp.GetUserInfoService;
                var ui= uiService.GetBy(x => x.UserId == user.Id && x.OpenId == openId && x.ProjectType == Enums.ProjectType.WeChatApplet);
                if (ui==null)
                {
                    ui = new UserInfo();
                    ui.UserId = user.Id;
                    ui.OpenId = openId;
                    ui.ProjectType = Enums.ProjectType.WeChatApplet;
                    var addui = ServiceHelp.GetUserInfoService.Add(ui);
                    if (addui == null)
                    {
                        if (string.IsNullOrWhiteSpace(errorMessage))
                        {
                            errorMessage = "用户注册失败。";
                        }
                        return null;
                    }
                }
                return Login(user, ref errorMessage);
            }
        }

        /// <summary>
        /// 进行登录的相关操作
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        private string Login(User user,ref string errorMessage)
        {
            //登录成功 获取token
            var usertoken = TokenHelp.WriteUserToken(user.Id);
            if (string.IsNullOrWhiteSpace(usertoken))
            {
                errorMessage = "登录失败,请稍候重试。";
                return "";
            }
            else
            {
                //配置当前登录数据
                Current.UserId = user.Id;
                Current.UserToken = usertoken;
                Current.UserJson = user.ToJsonString();
                //记录登录日志
                var task = ServiceHelp.GetLogService.WriteEventLogToLoginAsync();
                return usertoken;
            }
        }

        /// <summary>
        /// 退出登录
        /// </summary>
        /// <returns></returns>
        public bool LoginOut()
        {
            if (Current.UserId.HasValue)
            {
                try
                {
                    bool ist = TokenHelp.RemoveUserToken(Current.UserId.Value);
                    if (ist)
                    {
                        //退出登录 取消延长userToken有效期
                        Current.IsUserTokenExtensionTime = false;
                    }
                    //记录日志
                    var task = ServiceHelp.GetLogService.WriteEventLogToLogOutAsync();
                    return ist;
                }
                catch (Exception ex)
                {
                    Task task1 = ServiceHelp.GetLogService.WriteErrorLogAsync(new LogError(ex, null));
                    return false;
                }
                
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 判断但前用户是否用于对应的权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="authorizeList">要判断的权限</param>
        /// <param name="errorMessage"></param>
        /// <returns>True：有权限；</returns>
        public bool IsHaveAuthorize(User user,List<string> authorizeList,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (user==null)
            {
                user = Current.UserJson.JsonToModelOrDefault<User>();
            }
            if (user==null)
            {
                return false;
            }
            var ups = Commonfs.Split(user.Permissions, ',');
            //系统管理员角色和超级管理员拥有全部权限
            if (user.Id == 1 || user.RoleIds.ToList(',').Exists(x => x == "1"))
            {
                return true;
            }
            //获取用户角色的权限
            List<string> rlist = RoleBussiness.Init.GetRolePermissionsByUserId(user.Id, out errorMessage);

            var permissions = ups.Union(rlist);

            ////判断是否有权限
            foreach (var authorize in authorizeList)
            {
                if (permissions.Contains(authorize))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 获取用户拥有的权限
        /// </summary>
        /// <param name="user"></param>
        /// <param name="isAdmin">是否管理员</param>
        /// <returns></returns>
        public List<string> GetAllPermission(User user,out bool isAdmin,out string errorMessage)
        {
            errorMessage = string.Empty;
            isAdmin = false;
            if (user == null)
            {
                user = Current.UserJson.JsonToModelOrDefault<User>();
            }
            if (user == null)
            {
                return new List<string>();
            }
            var ups = Commonfs.Split(user.Permissions, ',');
            //系统管理员角色和超级管理员拥有全部权限
            if (user.Id == 1 || user.RoleIds.ToList(',').Exists(x => x == "1"))
            {
                isAdmin = true;
            }
            //获取用户角色的权限
            List<string> rlist = RoleBussiness.Init.GetRolePermissionsByUserId(user.Id, out errorMessage);

            var permissions = ups.Union(rlist);
            return permissions.ToList();
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="page"></param>
        /// <param name="limit"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<User> GetPageList(User user, int page, int limit, ref int total)
        {
            System.Linq.Expressions.Expression<Func<User, bool>> where = null;
            if (!string.IsNullOrWhiteSpace(user.Name))
            {
                where= where.ExpressionAnd(x => x.Name.Contains(user.Name));
            }
            if (!string.IsNullOrWhiteSpace(user.NickName))
            {
                where = where.ExpressionAnd(x => x.NickName.Contains(user.NickName));
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                where = where.ExpressionAnd(x => x.Email.Contains(user.Email));
            }
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                where = where.ExpressionAnd(x => x.Mobile.Contains(user.Mobile));
            }
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                where = where.ExpressionAnd(x => x.RoleIds.ToList(',').IsSubset(user.RoleIds.ToList(',')));
            }

            return ServiceHelp.GetUserService.GetPageList(where, page, limit, ref total, x => x.CreatedAt, SqlSugar.OrderByType.Desc).ToList();
        }

        /// <summary>
        /// 新增数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public User Add(User user,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (!VerifyData(user,out errorMessage))
            {
                return null;
            }
            //二次加密
            if (!string.IsNullOrWhiteSpace(user.PassWordMD5))
            {
                user.PassWordMD5 = Commonfs.GetMD5_32(user.PassWordMD5);
            }
            //添加默认角色
            var roles = user.RoleIds.ToList(',');
            //不存在则添加
            if (!roles.Exists(x=>x=="2"))
            {
                roles.Add("2");
            }
            user.RoleIds = string.Join(",", roles);
            var r = ServiceHelp.GetUserService.Add(user);
            if (r!=null)
            {
                Task task = ServiceHelp.GetLogService.WriteEventLogCreateAsync(typeof(User), r.Id, r.ToJsonString());
            }
            return r;
        }

        /// <summary>
        /// 密码需要完成第一次加密再传入
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public User Edit(User user, out string errorMessage)
        {
            if (!VerifyData(user, out errorMessage))
            {
                return null;
            }
            if (_service.GetById(user.Id) == null)
            {
                errorMessage = "未查询到数据！";
                return null;
            }
            //二次加密
            if (!string.IsNullOrWhiteSpace(user.PassWordMD5))
            {
                user.PassWordMD5 = Commonfs.GetMD5_32(user.PassWordMD5);
            }
            //添加默认角色
            var roles = user.RoleIds.ToList(',');
            //不存在则添加
            if (!roles.Exists(x => x == "2"))
            {
                roles.Add("2");
            }
            user.RoleIds = string.Join(",", roles);
            var rUser = _service.Edit(user);
            if (rUser == null)
            {
                errorMessage = "修改失败。";
                return null;
            }
            //记录操作日志
            Task task = ServiceHelp.GetLogService.WriteEventLogEditAsync(typeof(User), rUser.Id, rUser.ToJsonString());
            return rUser;
        }

        /// <summary>
        /// 逻辑删除
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="errorMessage"></param>
        /// <returns>True：成功；</returns>
        public bool Delete(int Id, out string errorMessage)
        {
            errorMessage = string.Empty;
            if (Id == 1)
            {
                errorMessage = "系统用户，无法删除！";
                return false;
            }
            var rb = _service.DeleteById(Id);
            if (rb)
            {
                //删除成功，记录日志
                Task task = ServiceHelp.GetLogService.WriteEventLogDeleteAsync(typeof(User), Id);
            }
            return rb;
        }

        /// <summary>
        /// 核查数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public bool VerifyData(User user,out string errorMessage)
        {
            errorMessage = string.Empty;
            if (user == null)
            {
                errorMessage = "未接收到数据。";
                return false;
            }
            if (user.Id==1)
            {
                errorMessage = "系统设置，无法修改！";
                return false;
            }
            if (string.IsNullOrWhiteSpace(user.Name))
            {
                errorMessage = "用户名不能为空。";
                return false;
            }
            else
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Name == user.Name && x.Id!=user.Id);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "此用户名已被注册。";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Email == user.Email && x.Id != user.Id);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "此邮箱已被注册。";
                    return false;
                }
            }
            if (!string.IsNullOrWhiteSpace(user.Mobile))
            {
                var wlist = ServiceHelp.GetUserService.GetAllList(x => x.Mobile == user.Mobile && x.Id != user.Id);
                if (wlist != null && wlist.Count > 0)
                {
                    errorMessage = "该手机号已被注册。";
                    return false;
                }
            }
            return true;
        }

    }
}
