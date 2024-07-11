var builder = WebApplication.CreateBuilder(args); //cuando ejecuto me dio error por aquello

// Add services to the container.
builder.Services.AddControllersWithViews();


//Builder de las cookies
builder.Services.AddAuthentication("CookieAuthentication").AddCookie("CookieAuthentication",
    config => { config.Cookie.Name = "UserloginCookie"; config.LoginPath = "/Usuarios/Login"; });

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

//Se activa la autenticacion de cookies
app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
