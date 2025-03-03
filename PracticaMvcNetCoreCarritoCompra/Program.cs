using Microsoft.EntityFrameworkCore;
using PracticaMvcNetCoreCarritoCompra.Data;
using PracticaMvcNetCoreCarritoCompra.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddMemoryCache();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();

builder.Services.AddTransient<IRepositoryCubos, RepositoryCubos>();
builder.Services.AddTransient<IRepositoryCompras, RepositoryCompras>();

string connectionString = builder.Configuration.GetConnectionString("MySqlCubos");
builder.Services.AddDbContext<CuboContext>(options => options.UseMySQL(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();
app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Cubos}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
