using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using scheduler.Business;
using scheduler.Business.Interfaces;
using scheduler.Configuration;
using scheduler.Data;
using scheduler.Repositories;
using scheduler.Repositories.Interfaces;
using System.Text;

public class Program
{
    // CORREÇÃO 1: A assinatura do método Main foi corrigida para usar 'string[] args', 
    // que é o padrão esperado pelo host da aplicação web.
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- INÍCIO: VALIDAÇÃO DE CONFIGURAÇÃO ESSENCIAL (PRINCÍPIO FAIL-FAST) ---
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var jwtSection = builder.Configuration.GetSection("Jwt");

        // CORREÇÃO 2: O valor da chave 'Secret' agora é obtido corretamente como uma string 
        // a partir da seção de configuração 'Jwt'. Isso resolve os erros de conversão.
        var jwtSecret = jwtSection["Secret"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];

        // A validação agora funciona, pois as variáveis são do tipo string.
        if (string.IsNullOrEmpty(connectionString) ||
            string.IsNullOrEmpty(jwtSecret) ||
            string.IsNullOrEmpty(jwtIssuer) ||
            string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException(
                "Erro Fatal: Configurações essenciais estão ausentes. " +
                "Verifique se 'ConnectionStrings:DefaultConnection' e a seção 'Jwt' (com Secret, Issuer, Audience) " +
                "estão configuradas corretamente nas variáveis de ambiente ou no appsettings.json.");
        }
        // --- FIM: VALIDAÇÃO DE CONFIGURAÇÃO ESSENCIAL ---

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserBusiness, UserBusiness>();
        builder.Services.AddScoped<ICourtRepository, CourtRepository>();
        builder.Services.AddScoped<ICourtBusiness, CourtBusiness>();
        builder.Services.AddScoped<IEventRepository, EventRepository>();
        builder.Services.AddScoped<IEventBusiness, EventBusiness>();

        builder.Services.Configure<JwtConfig>(jwtSection);

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options =>
       {
           options.TokenValidationParameters = new TokenValidationParameters
           {
               ValidateIssuerSigningKey = true,
               // CORREÇÃO 3: A variável 'jwtSecret' agora é uma string, permitindo que 
               // 'GetBytes' funcione corretamente para criar a chave de assinatura.
               IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSecret)),
               ValidateIssuer = true,
               ValidIssuer = jwtIssuer,
               ValidateAudience = true,
               ValidAudience = jwtAudience,
               ValidateLifetime = true,
               ClockSkew = TimeSpan.Zero
           };
       });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAllOrigins", policy =>
            {
                policy.AllowAnyOrigin()
                      .AllowAnyMethod()
                      .AllowAnyHeader();
            });
        });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        app.UseCors("AllowAllOrigins");
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
