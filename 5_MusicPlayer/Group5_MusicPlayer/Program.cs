using Group5_MusicPlayer;
using Group5_MusicPlayer.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);



//////////////////////////////////////////////////////////////////////////////////
//Add sSignalR
builder.Services.AddSignalR();

// Add services to the container.
builder.Services.AddControllersWithViews();

//Configure Session 
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromSeconds(60 * 60 * 24);
    options.Cookie.HttpOnly = true;
});
builder.Services.AddHttpContextAccessor();

//Configure database context
builder.Services.AddDbContext<MusicPlayerDbContext>(options =>
    options.UseSqlServer(builder.Configuration
    .GetConnectionString("DbConnection")));


var app = builder.Build();
app.UseSession();
//////////////////////////////////////////////////////////////////////////////////



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

//Configure SinalR
app.UseEndpoints(endpoints =>
{
    endpoints.MapHub<SignalrServer>("/signalrServer");
});

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.Run();
