using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Authorization;
using Commons;
using Commons.Cache;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NetCoreAPI.AuthHelp;
using Senparc.CO2NET;
using Senparc.CO2NET.RegisterServices;
using Senparc.WebSocket;
using Senparc.Weixin;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.Containers;
using Senparc.Weixin.RegisterServices;

namespace NetCoreAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            #region Config
            //ע��appsettings��ȡ��
            services.AddSingleton(new Appsettings(Configuration));
            //��ȡ�������
            var vtos = Appsettings.app<Dictionary<string, string>>(new string[] { "VisitToken", "Tos" });
            Commons.Config.VisitTos = vtos.FirstOrDefault();
            Commons.Config.IsOpenRedis = Appsettings.app(new string[] { "Config", "IsOpenRedis" });
            Commons.Config.MysqlConnectionStrng = Appsettings.app(new string[] { "Config", "MysqlConnectionStrng" });
            Commons.Config.RedisConnectionString = Appsettings.app(new string[] { "Config", "RedisConnectionString" });
            Commons.Config.CorsUrl = Appsettings.app(new string[] { "Config", "CorsUrl" }).ToList(',');
            #endregion

            services.AddControllers();
            
            //����
            services.AddCors(options =>
            {
                options.AddPolicy("allCors", builder =>
                {
                    //builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(new[] {
                    //    "http://chicface.yufaquan.cn","https://chicface.yufaquan.cn"
                    //});
                    builder.AllowAnyHeader().AllowAnyMethod().AllowCredentials().WithOrigins(Commons.Config.CorsUrl.ToArray());
                });
            });

            //������
            services.AddMvc(options =>
            {
                options.Filters.Add<ExceptionActionFilter>();
                options.Filters.Add<ManageVerifyAttribute>();
            });

            #region Swagger
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Version = "v1",
                    Title = "YuFaquan API",
                    Description = "desc",
                    TermsOfService = new Uri("https://yufaquan.cn"),
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact() { 
                        Email="13235601859@163.com",
                        Name="YuFaquan",
                        Url=new Uri("https://yufaquan.cn")
                    }
                });
                //��Ӷ�ȡע�ͷ���
                dynamic type = new Program().GetType();
                string basePath = Path.GetDirectoryName(type.Assembly.Location);
                Current.ServerPath = basePath;
                var xmlPath = Path.Combine(basePath, "NetCoreAPI.xml");
                var xmlPath2 =  Path.Combine(basePath, "Entity.xml");
                //c.IncludeXmlComments(xmlPath);

                //��ӿ�������ע�ͣ�true��ʾ��ʾ������ע�ͣ�
                c.IncludeXmlComments(xmlPath2);
                c.IncludeXmlComments(xmlPath, true);

                //���header��֤��Ϣ
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "JWT",
                    Scheme = "Bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                            }
                        },
                            new string[] { }
                        }
                });
                //����û�usertoken
                c.AddSecurityDefinition("UserToken", new OpenApiSecurityScheme
                {
                    Description = "�û���Ϣ��Ȩ(���ݽ�������ͷ�н��д���) �����ṹ: \"UserToken:{token}\"",
                    Name = "UserToken",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    BearerFormat = "UT",
                    Scheme = "UserToken"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                 {
                   {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference {
                            Type = ReferenceType.SecurityScheme,
                            Id = "UserToken"
                            }
                        },
                            new string[] { }
                        }
                });

            });
            #endregion

            #region ע�Ỻ��
            if (Commons.Config.IsOpenRedis.ToLower() == "true")
            {
                services.AddSingleton<ICacheManager, RedisCacheManager>();
            }
            else
            {
                services.AddSingleton<ICacheManager, MyMemoryCache>();
            } 
            #endregion

            #region �����֤
            //�����֤
            //services.AddAuthentication(options =>
            //{
            //    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            //    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            //})
            ////���jwt��֤��
            //.AddJwtBearer(options =>
            //{
            //    options.TokenValidationParameters = new TokenValidationParameters
            //    {
            //        ValidateLifetime = true,//�Ƿ���֤ʧЧʱ��
            //        ClockSkew = TimeSpan.FromMinutes(30),
            //        ValidateActor = false,
            //        ValidateTokenReplay = false,
            //        IgnoreTrailingSlashWhenValidatingAudience = false,


            //        ValidateAudience = false,//�Ƿ���֤Audience
            //        //ValidAudience = Const.GetValidudience(),//Audience
            //        //������ö�̬��֤�ķ�ʽ�������µ�½ʱ��ˢ��token����token��ǿ��ʧЧ��
            //        //AudienceValidator = (m, n, z) =>
            //        //{
            //        //    return m != null && m.FirstOrDefault().Equals(Const.ValidAudience);
            //        //},
            //        ValidateIssuer = true,//�Ƿ���֤Issuer
            //        ValidIssuer = Config.JWTInfo.Issuer,//Issuer���������ǰ��ǩ��jwt������һ��

            //        ValidateIssuerSigningKey = false,//�Ƿ���֤SecurityKey
            //        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Config.JWTInfo.SigningKey))//�õ�SecurityKey
            //    };
            //    // ------------------------�Զ���ָ���-------------------------

            //    options.SecurityTokenValidators.Clear();//���Ĭ�ϵ�����
            //    options.SecurityTokenValidators.Add(new MyTokenValidata(new RedisCacheManager()));//����Լ��趨�������֤����
            //    options.Events = new JwtBearerEvents
            //    {
            //        //������������ڼ������쳣������á����Ǳ����ƣ������ڴ��¼������������쳣
            //        OnAuthenticationFailed = context =>
            //        {
            //            //Token expired
            //            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            //            {
            //                context.Response.Headers.Add("Token-Expired", "true");
            //            }
            //            return Task.CompletedTask;
            //        }
            //        //�˴�ΪȨ����֤ʧ�ܺ󴥷����¼�
            //        ,
            //        OnChallenge = context =>
            //       {
            //           //�˴�����Ϊ��ֹ.Net CoreĬ�ϵķ������ͺ����ݽ�����������ҪŶ������
            //           context.HandleResponse();
            //           //�Զ��巵�ص���������
            //           context.Response.AuthFailed();
            //           return Task.FromResult(0);
            //       }
            //       //,OnForbidden = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}
            //       //,
            //       //OnMessageReceived = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}
            //       //,
            //       //OnTokenValidated = context =>
            //       //{
            //       //    return Task.CompletedTask;
            //       //}

            //    };
            //});

            #endregion

            #region ��Ӳ��Լ�Ȩģʽ
            //��Ӳ��Լ�Ȩģʽ
            //ֻ����Default����
            //services.AddAuthorization(o =>
            //{
            //    o.AddPolicy("Default", policy => policy.RequireClaim("DefaultType").Build());
            //});
            //services.AddAuthorization(options =>
            //{
            //    options.AddPolicy("Permission", policy => policy.Requirements.Add(new PolicyRequirement()));
            //})
            //��Ȩ 
            #endregion

            services.AddAuthorization();

            //��֤����
            //services.AddSingleton<IAuthorizationHandler, PolicyHandler>();

            //��Ҫ���ڻ�ȡ�ͻ���ip
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            //ע��Ȩ��
            AuthorizationService.LoadAuthorize();

            #region ΢��ģ��
            services.AddSenparcGlobalServices(Configuration)//Senparc.CO2NET ȫ��ע��
                    .AddSenparcWeixinServices(Configuration);//Senparc.Weixin ע��
            //�˿��֤���ַ
            services.AddCertHttpClient("1603559615" + "_", "1603559615", "C:\\WeChat\\chicface\\apiclient_cert.p12");
            //Senparc.WebSocket ע�ᣨ���裩
            services.AddSenparcWebSocket<WXOpenSocketMessageHandler>();
            

            #endregion


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IOptions<SenparcSetting> senparcSetting, IOptions<SenparcWeixinSetting> senparcWeixinSetting)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use(next => context =>
            {
                context.Request.EnableBuffering();
                return next(context);
            });
            //app.UseMiddleware<TokenAuth>();

            app.UseHttpsRedirection();

            app.UseRouting();

            #region ���ÿ���
            app.UseCors("allCors");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.Map("/", context =>
                {
                    context.Response.Redirect("/swagger");
                    return Task.CompletedTask;
                });
            }); 
            #endregion

            #region ΢��ģ��
            // ���� CO2NET ȫ��ע�ᣬ���룡
            IRegisterService register = RegisterService.Start(senparcSetting.Value)
                                                        .UseSenparcGlobal(false, null);
            //��ʼע��΢����Ϣ�����룡
            register.UseSenparcWeixin(senparcWeixinSetting.Value, senparcSetting.Value);


            //ȫ��ע��appid
            AccessTokenContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppSecret);
            JsApiTicketContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WeixinAppSecret);
            //С����
            AccessTokenContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WxOpenAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WxOpenAppSecret);
            JsApiTicketContainer.RegisterAsync(Senparc.Weixin.Config.SenparcWeixinSetting.WxOpenAppId, Senparc.Weixin.Config.SenparcWeixinSetting.WxOpenAppSecret);
            #endregion

            #region Swagger
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApiHellp V1");
                c.DocumentTitle = "Api�����ĵ�";
                
            });
            #endregion

            #region ���þ�̬�ļ�·��
            //��ȡ��ǰ�������е�·��
            dynamic type = new Program().GetType();
            string basePath = Path.GetDirectoryName(type.Assembly.Location);
            string StaticFilePath = Path.Combine(basePath, "StaticFiles");
            app.UseStaticFiles(new StaticFileOptions
            {
                //���ó���Ĭ�ϵ�wwwroot�ļ��еľ�̬�ļ�������ļ���  �ṩ Web ��Ŀ¼����ļ�  �����������Ժ󣬾Ϳ��Է���StaticFiles�ļ��µ��ļ�
                FileProvider = new PhysicalFileProvider(StaticFilePath),
                RequestPath = "/StaticFiles",
            });
            #endregion

            #region ʹ��SignalR
            //ʹ�� SignalR
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapHub<YvanHub>("/yvanHub");
            }); 
            #endregion

        }
    }
}
