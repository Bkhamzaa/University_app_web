using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using University_web_app.Data;
using University_web_app.Models;
using University_web_app.Repositories;
using University_web_app.Service;
using static University_web_app.Data.UniversityContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<DashboardRepository>();
builder.Services.AddScoped<ExamRepository>();
builder.Services.AddScoped<StudentRepository>();
builder.Services.AddScoped<SubjectRepository>();



// conxection db 
builder.Services.AddDbContext<UniversityContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));
//email service
builder.Services.AddSingleton<EmailService>();

// Add Identity services
builder.Services.AddIdentity<Users, IdentityRole>()
    .AddEntityFrameworkStores<UniversityContext>()
    .AddDefaultTokenProviders();

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

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.Initialize(services);
}


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}");

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UniversityContext>();
    context.Database.Migrate(); 
    DbInitializer.Seed(context);
}

app.Run();
