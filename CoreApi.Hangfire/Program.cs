// 請於 appsettings.json 加入 HangfireConnection 連線字串，例如：
// "ConnectionStrings": {
//   "HangfireConnection": "Server=localhost;Database=HangfireDb;User Id=sa;Password=your_password;TrustServerCertificate=True;"
// }
// Hangfire 套件引用
// Hangfire 套件引用
using CoreApi.Service.Helper;
using CoreApi.Hangfire.Jobs;
using Hangfire;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Hangfire 設定
// Hangfire SQL Server 設定
builder.Services.AddHangfire(config =>
{
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"));
});
builder.Services.AddHangfireServer();

// 注入 Service 層
ServiceStartUp.StartUp(builder.Services, builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


app.UseHangfireDashboard();

CoreApi.Hangfire.HangfireStartUp.StartUp(app);

app.Run();
