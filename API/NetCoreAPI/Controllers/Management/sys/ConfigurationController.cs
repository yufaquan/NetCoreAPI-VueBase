﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Bussiness.Mangement;
using Entity;
using Microsoft.AspNetCore.Authorization;
using Authorization;

namespace NetCoreAPI.Controllers.Management
{
    /// <summary>
    /// 系统配置
    /// </summary>
    public class ConfigurationController : ManagementApiController
    {
        /// <summary>
        /// 获取
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [MyAuthorize(typeof(Read<Configuration>))]
        public async Task<JsonResult> Get()
        {
            var res = await Task.FromResult(HttpResult.Success(ConfigurationBussiness.Init.Get(HttpContext)));
            return new JsonResult(res);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <returns></returns>
        [HttpPut]
        [MyAuthorize(typeof(Update<Configuration>))]
        public async Task<JsonResult> Set([FromBody] Configuration configuration)
        {
            bool isSuccess= await Task.FromResult(ConfigurationBussiness.Init.Set(configuration,HttpContext));
            if (isSuccess)
            {
                return new JsonResult(HttpResult.Success(null));
            }
            else
            {
                return new JsonResult(HttpResult.Success(HttpResultCode.EditFail,null));
            }
        }
    }
}
