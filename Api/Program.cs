using Application.Interfaces;
using Application.Services;
using Domain.Interfaces;
using Infrastructure.Context;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Registo dos Serviços (Injeção de Dependência) ---

// Adiciona o serviço de Controllers
builder.Services.AddControllers();

// Adiciona o Swagger (documentação da API)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Configuração para que o Swagger possa enviar o Token JWT nos testes
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira 'Bearer' [espaço] e depois o seu token no campo abaixo."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configura o DbContext com a Connection String
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<FluxoDeNotasDbContext>(options =>
    options.UseSqlServer(connectionString));

// Configura o CORS (para permitir que o seu frontend chame a API)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configura a Autenticação JWT
var jwtKey = builder.Configuration["Jwt:Key"];
if (string.IsNullOrEmpty(jwtKey))
{
    throw new InvalidOperationException("Chave JWT ('Jwt:Key') não encontrada ou está vazia no appsettings.json.");
}
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(auth =>
{
    auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(jwt =>
{
    jwt.RequireHttpsMetadata = false; 
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        // Valida o tempo de vida do token
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero // Remove o tempo de tolerância na expiração
    };
});

// Adiciona o serviço de Autorização
builder.Services.AddAuthorization();


builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();
builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<ICodigoTemporarioRepository, CodigoTemporarioRepository>();
builder.Services.AddScoped<IProjetoRepository, ProjetoRepository>();
builder.Services.AddScoped<IProjetoService, ProjetoService>();
builder.Services.AddScoped<IAnotacaoRepository, AnotacaoRepository>();
builder.Services.AddScoped<IAnotacaoService, AnotacaoService>();
builder.Services.AddScoped<ITarefaRepository, TarefaRepository>();
builder.Services.AddScoped<ITarefaService, TarefaService>();


// --- 2. Configuração do Pipeline HTTP  ---

// Esta linha "fecha" o registo de serviços.
var app = builder.Build();

// Em ambiente de desenvolvimento, mostra o Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redireciona HTTP para HTTPS
app.UseHttpsRedirection();

// Adiciona o CORS
app.UseCors("AllowAll");

// Adiciona Autenticação (IMPORTANTE: antes da Autorização)
app.UseAuthentication();

// Adiciona Autorização
app.UseAuthorization();

// Mapeia os Controllers para as rotas
app.MapControllers();

// Inicia a aplicação
app.Run();