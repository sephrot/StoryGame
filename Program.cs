using Microsoft.EntityFrameworkCore;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StoryDbContext>(options =>
{
    options
        .UseSqlite(builder.Configuration["ConnectionStrings:StoryDbContextConnection"])
        .LogTo(_ => { }); // disables EF logging
});

builder.Services.AddScoped<IStoryRepository, StoryRepository>();
builder.Services.AddScoped<ISceneRepository, SceneRepository>();
builder.Services.AddScoped<IChoiceRepository, ChoiceRepository>();

var app = builder.Build();

// Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

DBInit.Seed((IApplicationBuilder)app);

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthorization();

// Enable attribute-routed controllers (REQUIRED for the [Route] attributes above)
app.MapControllers();

// Keep conventional route for the rest of your app
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Title}/{id?}"
);

app.Run();
