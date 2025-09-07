// Serilog 結構化日誌設定
using Serilog;
using CoreApi.Service;
using CoreApi.Service.Interfaces;
using CoreApi.Common;
using CoreApi.Common.Interfaces;
using CoreApi.Common.Helpers;
using CoreApi.Repository.Interfaces;
using CoreApi.Repository;
using Microsoft.EntityFrameworkCore;
using CoreApi.Entity.Entities;
using CoreApi.Common.Middlewares;
using Microsoft.OpenApi.Models;
var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, lc) => lc
    .ReadFrom.Configuration(ctx.Configuration)
    .WriteTo.Console()
    .WriteTo.File("Logs/log-.txt", rollingInterval: RollingInterval.Day));
    
// JwtAuthentication 設定
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", UserInfoHelper.JwtBearerOptionsAction(builder.Configuration));

builder.Services.AddAuthorization();

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<SqlConnectionHelper>();
builder.Services.AddScoped<IUserInfoHelper, UserInfoHelper>();

builder.Services.AddDbContext<ReadWriteCoreApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(Constants.DbConnectionKeys.ReadWriteConnection)));
builder.Services.AddDbContext<ReadOnlyCoreApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(Constants.DbConnectionKeys.ReadOnlyConnection)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "請於此處輸入 JWT Bearer Token，格式為：Bearer {token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.Filters.Add<ProcessResultFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<ProcessResultExceptionMiddlewares>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();

