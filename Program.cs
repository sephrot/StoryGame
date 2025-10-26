using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Events;
using StoryGame.DAL;
using StoryGame.Models;
using StoryGame.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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

var loggerConfiguration = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/app_{DateTime.Now:yyyyMMdd_HHmmss}.log");



loggerConfiguration.Filter.ByExcluding(e =>
    e.Properties.TryGetValue("SourceContext", out var value)
    && e.Level == LogEventLevel.Information
    && e.MessageTemplate.Text.Contains("Executed DbCommand")
);
var logger = loggerConfiguration.CreateLogger();
builder.Logging.AddSerilog(logger);
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

DBInit.Seed((IApplicationBuilder)app);

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.UseStaticFiles();

app.MapDefaultControllerRoute();
app.Run();
