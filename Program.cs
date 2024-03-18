using EstoqueWeb.Models;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddDbContext<EstoqueWebContext>( options => {
    options.UseSqlite(builder.Configuration.GetConnectionString("EstoqueWebContext"));
});
var app = builder.Build();

app.UseDeveloperExceptionPage();
app.UseStaticFiles();
app.UseRouting();

app.UseEndpoints( endpoints => {
    endpoints.MapDefaultControllerRoute();
});
app.Run();