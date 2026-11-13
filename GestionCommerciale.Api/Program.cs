using System.Text;
using GestionCommerciale.Application;
using GestionCommerciale.Infrastructure;
using GestionCommerciale.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using GestionCommerciale.Api.Middleware;
var builder = WebApplication.CreateBuilder(args);

// ───── Services des couches ─────
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
// ───── Authentification JWT : comment vérifier les tokens reçus ─────
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.Zero // le token expire à l'heure exacte
        };
    });
builder.Services.AddAuthorization();
// CORS : autorise le front Angular à appeler l'API
const string FrontendCorsPolicy = "FrontendPolicy";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

builder.Services.AddCors(options =>
    options.AddPolicy(FrontendCorsPolicy, policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()     // dont l'en-tête Authorization (le token)
              .AllowAnyMethod()));  // GET, POST, PUT, DELETE
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ───── Swagger avec le bouton « Authorize » ─────
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Collez uniquement le token (sans écrire « Bearer »)"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();
app.UseExceptionHandler();   // ← en PREMIER : il doit entourer tout le reste pour tout attraper
// ───── Création du compte admin au démarrage ─────
using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DbSeeder>();
    await seeder.SeedAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();   // 1. qui es-tu ? (lit et vérifie le token)
app.UseAuthorization();    // 2. as-tu le droit ? (applique [Authorize])
app.MapControllers();

app.Run();