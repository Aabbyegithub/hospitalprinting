


using System.Configuration;
using WebProjectTest;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.Configure<AppSettingsSection>(builder.Configuration.GetSection("SqlServer"));
// 手动模拟Startup的配置逻辑
// 注意：以下代码仅作为演示如何模拟旧流程，实际应用中应优先考虑直接在Program.cs中配置
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);



// Add services to the container.
builder.Logging.AddConsole();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
startup.Configure(app,builder.Environment);
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
