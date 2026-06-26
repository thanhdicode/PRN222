using MangaWorkflow.Application;
using MangaWorkflow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add MVC + Razor Pages + Blazor (Blazor/SignalR wired in later phases)
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
builder.Services.AddSignalR();

// Database context & Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Application Services
builder.Services.AddApplicationServices();

// --- Cookie Authentication (Phase 2) ---
builder.Services.AddAuthentication("MangaWorkflowCookie")
    .AddCookie("MangaWorkflowCookie", options =>
    {
        options.LoginPath = "/Auth/Login";
        options.LogoutPath = "/Auth/Logout";
        options.AccessDeniedPath = "/Auth/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = Microsoft.AspNetCore.Http.CookieSecurePolicy.SameAsRequest;
    });

// Authorization policies per role
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("MangakaOnly", policy => policy.RequireRole("Mangaka"));
    options.AddPolicy("AssistantOnly", policy => policy.RequireRole("Assistant"));
    options.AddPolicy("TantouEditorOnly", policy => policy.RequireRole("TantouEditor"));
    options.AddPolicy("EditorialBoardOnly", policy => policy.RequireRole("EditorialBoard"));
    options.AddPolicy("AdminOrBoard", policy => policy.RequireRole("Admin", "EditorialBoard"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

// Authentication must come before Authorization
app.UseAuthentication();
app.UseAuthorization();

// Area routes — areas must be mapped before the default route
app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
// app.MapHub<NotificationHub>("/hubs/notifications"); // Phase 4 SignalR

app.Run();

