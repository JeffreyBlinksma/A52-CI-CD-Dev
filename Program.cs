using Area52Entertainment.Data;
using Area52Entertainment.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();

// Register repositories
builder.Services.AddScoped<SqlConnectionFactory>();
builder.Services.AddScoped<IActiviteitRepository, ActiviteitRepository>();
builder.Services.AddScoped<IDeelnemerRepository, DeelnemerRepository>();
builder.Services.AddScoped<IReserveringRepository, ReserveringRepository>();
builder.Services.AddScoped<IAnnuleringsBeleidRepository, AnnuleringsBeleidRepository>();

// Register services
builder.Services.AddScoped<ActiviteitService>();
builder.Services.AddScoped<ReserveringService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();