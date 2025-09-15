using Microsoft.Extensions.WebEncoders;
using System.Text.Encodings.Web;
using System.Text.Unicode;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
//builder.Services.Configure<WebEncoderOptions>(options =>
//{
//    options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
//});
//builder.Services.AddSingleton<HtmlEncoder>(HtmlEncoder.Create(
//    allowedRanges: new[] { UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs }));
//builder.Services.AddRazorPages()
//       .AddJsonOptions(options =>
//       {
//           //原本是 JsonNamingPolicy.CamelCase，強制頭文字轉小寫，我偏好維持原樣，設為null
//           options.JsonSerializerOptions.PropertyNamingPolicy = null;
//           //允許基本拉丁英文及中日韓文字維持原字元
//           options.JsonSerializerOptions.Encoder =
//               JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.CjkUnifiedIdeographs);
//       });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
