using Gestion_Financiera.Components;
using Gestion_Financiera.Data;
using Gestion_Financiera.Repositories.Implementations;
using Gestion_Financiera.Repositories.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// 1. SERVICIOS DE INTERFAZ DE USUARIO (Blazor)
// =============================================================================
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// =============================================================================
// 2. CONFIGURACIÓN DE ACCESO A DATOS (Dapper & SQL Server)
// =============================================================================
// Fábrica de conexiones (Transient: se genera una nueva instancia por petición)
builder.Services.AddTransient<IDbConnectionFactory, DbConnectionFactory>();

// =============================================================================
// 3. INYECCIÓN DE DEPENDENCIAS (Repositorios)
// =============================================================================
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();
builder.Services.AddScoped<IFinancialMetricsRepository, FinancialMetricsRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();

// =============================================================================
// 4. CONSTRUCCIÓN Y PIPELINE DE PETICIONES HTTP
// =============================================================================
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();
app.UseAntiforgery();
app.MapStaticAssets();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
