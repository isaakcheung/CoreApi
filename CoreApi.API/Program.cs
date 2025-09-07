// Serilog 結構化日誌設定
using Serilog;
using CoreApi.Service;
using CoreApi.Common;
using CoreApi.Entity;
using CoreApi.Service.Interfaces;
using CoreApi.Common.Interfaces;
using CoreApi.Common.Helpers;
using Microsoft.EntityFrameworkCore;
using CoreApi.Common.Middlewares;
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
builder.Services.AddScoped<CoreApi.Repository.Interfaces.IUserRepository, CoreApi.Repository.UserRepository>();

builder.Services.AddDbContext<CoreApi.Entity.Entities.ReadWriteCoreApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(CoreApi.Common.Constants.DbConnectionKeys.ReadWriteConnection)));
builder.Services.AddDbContext<CoreApi.Entity.Entities.ReadOnlyCoreApiDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString(CoreApi.Common.Constants.DbConnectionKeys.ReadOnlyConnection)));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "請於此處輸入 JWT Bearer Token，格式為：Bearer {token}"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
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
    options.Filters.Add<CoreApi.Common.Helpers.ProcessResultFilter>();
});

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseMiddleware<CoreApi.Common.Middlewares.ProcessResultExceptionHandler>();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();



app.Run();

