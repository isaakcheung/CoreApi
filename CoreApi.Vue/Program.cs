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
//           //�쥻�O JsonNamingPolicy.CamelCase�A�j���Y��r��p�g�A�ڰ��n������ˡA�]��null
//           options.JsonSerializerOptions.PropertyNamingPolicy = null;
//           //���\�򥻩ԤB�^��Τ�������r������r��
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
