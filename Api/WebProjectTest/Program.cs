


using System.Configuration;
using WebProjectTest;

var builder = WebApplication.CreateBuilder(args);
//builder.Services.Configure<AppSettingsSection>(builder.Configuration.GetSection("SqlServer"));
// �ֶ�ģ��Startup�������߼�
// ע�⣺���´������Ϊ��ʾ���ģ������̣�ʵ��Ӧ����Ӧ���ȿ���ֱ����Program.cs������
var startup = new Startup(builder.Configuration);

startup.ConfigureServices(builder.Services);



// Add services to the container.
builder.Logging.AddConsole();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
startup.Configure(app,builder.Environment,app.Services);
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
