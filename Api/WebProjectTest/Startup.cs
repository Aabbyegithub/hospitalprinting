using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using SqlSugar;
using SqlSugar.Extensions;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using System.Text;
using WebIServices;
using WebIServices.IBase;
using WebProjectTest.Common;
using WebServiceClass.Base;
using WebServiceClass.Helper;
using WebServiceClass.QuartzTask;
using WebTaskClass.SampleJob;

namespace WebProjectTest
{
    public class Startup
    {
        private static readonly string _windowsBasePath = AppSettings.GetConfig("UpFile:Windows");
        private static readonly string _linuxBasePath = AppSettings.GetConfig("UpFile:Linux");
        private static readonly bool _swaggerEnabled  = AppSettings.GetConfig("Swagger:Enabled").ObjToBool();
        public Startup(IConfiguration configuration) 
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //为了在 ASP.NET Core 中使用 MVC 模式，我们必须在 Startup 类中的 ConfigureServices 方法中添加 AddMvc 服务
            services.AddMvc();

            services.AddControllers();

            //配置数据库链接
            services.AddSingleton(new AppSettings(Configuration));
            SqlsugarSetup.AddSqlsugarSetup(services);

            // 确保注册依赖服务
            services.AddHttpContextAccessor();

            // 配置 Redis
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = AppSettings.GetConfig("Redis:Redis");
                options.InstanceName = "RedisInstance";
            });

            //注册获取配置文件
            services.AddScoped<IAppSettinghelper, WebAppConfig>();

            //注入Service
            services.Scan(scan =>
            scan.FromAssemblies(Assembly.Load("WebIServices"),Assembly.Load("WebServiceClass"),Assembly.Load("ModelClassLibrary"), typeof(Class1).Assembly) // 添加更多程序集
            .AddClasses(classes => classes.AssignableTo(typeof(IBaseService)))
            .AsImplementedInterfaces()
            .WithScopedLifetime());

            if (AppSettings.GetConfig("Quartz:StartOnStartup").ObjToBool())
            {
                //配置定时服务
                services.AddSingleton<IHostedService, QuartzHostedService>();
                // 注册所有 Job 类型
                var jobAssembly = Assembly.Load("WebTaskClass");
                var jobTypes = jobAssembly.GetTypes()
                    .Where(t => typeof(IJob).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

                foreach (var jobType in jobTypes)
                {
                    services.AddTransient(jobType);
                }
            }

            // 配置JWT认证
            var key = Encoding.UTF8.GetBytes(SecureKeyGenerator.GenerateSecureKey()); // 请替换为安全的密钥
            var symmetricSecurityKey = new SymmetricSecurityKey(key);
            services.AddSingleton(symmetricSecurityKey);
            services.AddSingleton<TokenService>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = true; // 开发环境下可设为false
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false, // 可根据实际情况调整
                    ValidateAudience = false, // 同上
                    ClockSkew = TimeSpan.Zero // 允许的时间偏差
                };
            });
            // services.AddSingleton<TokenService>();
            // 添加CORS服务
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder => builder.AllowAnyOrigin()
                                      .AllowAnyHeader()
                                      .AllowAnyMethod()
                                      );
            });

            services.AddSwaggerGen(c =>
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var controllerTypes = assembly.GetTypes().Where(type =>
                        type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract);

                    var addedControllers = new HashSet<string>();

                    foreach (var controllerType in controllerTypes)
                    {
                        var apiVersion = controllerType.Name.Replace("Controller", "");
                        if (!addedControllers.Contains(apiVersion))
                        {
                            c.SwaggerDoc(apiVersion, new OpenApiInfo { Title = $"{apiVersion}", Version = apiVersion });
                            addedControllers.Add(apiVersion);
                        }
                    }
                    // 设置 Swagger 根据控制器名称分组
                    c.DocInclusionPredicate((docName, apiDesc) =>
                    {
                        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return false;

                        var controllerName = methodInfo.DeclaringType.Name.Replace("Controller", "");
                        return docName == controllerName;
                    });

                    // 使用控制器名称作为分组
                    c.TagActionsBy(apiDesc =>
                    {
                        if (!apiDesc.TryGetMethodInfo(out var methodInfo)) return new[] { "default" };

                        var controllerName = methodInfo.DeclaringType.Name.Replace("Controller", "");
                        return new[] { controllerName };
                    });

                    c.OrderActionsBy((apiDesc) => $"{apiDesc.GroupName}_{apiDesc.HttpMethod}");

                    // 添加支持Bearer Token的授权方案
                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.ApiKey,
                        Name = "Authorization", // Bearer Token将放到这个header中
                        In = ParameterLocation.Header,
                        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                        Scheme = "Bearer"
                    });

                    // 为所有API路径添加授权要求（可选，也可以针对特定API设置）
                    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "oauth2"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,

                        },
                        new List<string>()
                    }
                    });
                    // 获取XML注释文件的路径
                    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                    c.IncludeXmlComments(xmlPath);
                });
            
        }


        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
             app.UseCors("AllowAllOrigins");
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // 启用认证
            //app.UseAuthorization(); 
            app.UseAuthorization();// 启用授权

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    var assembly = Assembly.GetExecutingAssembly();
                    var controllerTypes = assembly.GetTypes().Where(type =>
                        type.IsSubclassOf(typeof(ControllerBase)) && !type.IsAbstract);

                    foreach (var controllerType in controllerTypes)
                    {
                        var apiVersion = controllerType.Name.Replace("Controller", "");
                        c.SwaggerEndpoint($"/swagger/{apiVersion}/swagger.json", $"{apiVersion}");
                    }
                    //c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    // 可以在这里添加其他Swagger UI的配置
                });
            
            string basePath = OperatingSystem.IsWindows() ? _windowsBasePath : _linuxBasePath;
            if (!Directory.Exists(basePath))
            {
                Directory.CreateDirectory(basePath);
            }
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(basePath),
                RequestPath = "" // 直接映射到根路径，根据实际需求调整
            });
            // 配置全局异常处理
            app.Use(async (context, next) =>
            {
                try
                {
                    await next();
                }
                catch (Exception ex)
                {
                    //logger.LogError(ex, "An unhandled exception has occurred.");
                    using (var Logger = new LoggerHelper(moduleName:"全局异常管理",logFileName:"Errer"))
                    {
                        Logger.LogError(ex.Message);
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    }
                    
                }
            });
        }
    }
}
