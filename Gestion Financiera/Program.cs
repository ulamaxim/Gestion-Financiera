using Gestion_Financiera.Components;
using Gestion_Financiera.Data;
using Gestion_Financiera.Repositories.Implementations;
using Gestion_Financiera.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Añadir servicios al contenedor.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Registrar los repositorios en el contenedor de dependencias
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// ---------------------------------------------------------------------------
// CONFIGURACIÓN DE DAPPER Y SQL SERVER
// ---------------------------------------------------------------------------
// Registramos la fábrica de conexiones como Transient (se crea una nueva por cada petición)
builder.Services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();
// ---------------------------------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
