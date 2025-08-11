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
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- INÍCIO: DEBUGGING DE CONFIGURAÇÃO ---
        // Este bloco irá imprimir os valores que a aplicação está lendo.
        Console.WriteLine("--- LENDO CONFIGURAÇÕES ---");
        var connectionStringForLog = builder.Configuration.GetConnectionString("DefaultConnection");
        var jwtSecretForLog = builder.Configuration.GetSection("Jwt")["Secret"];
        var jwtIssuerForLog = builder.Configuration.GetSection("Jwt")["Issuer"];
        var jwtAudienceForLog = builder.Configuration.GetSection("Jwt")["Audience"];

        Console.WriteLine($"ConnectionString: '{connectionStringForLog}'");
        Console.WriteLine($"Jwt:Secret: '{jwtSecretForLog}'");
        Console.WriteLine($"Jwt:Issuer: '{jwtIssuerForLog}'");
        Console.WriteLine($"Jwt:Audience: '{jwtAudienceForLog}'");
        Console.WriteLine("--- FIM DA LEITURA DE CONFIGURAÇÕES ---");
        // --- FIM: DEBUGGING DE CONFIGURAÇÃO ---

        // --- INÍCIO: VALIDAÇÃO DE CONFIGURAÇÃO ESSENCIAL (PRINCÍPIO FAIL-FAST) ---
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var jwtSection = builder.Configuration.GetSection("Jwt");

        var jwtSecret = jwtSection["Secret"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];

        if (string.IsNullOrEmpty(connectionString) ||
            string.IsNullOrEmpty(jwtSecret) ||
            string.IsNullOrEmpty(jwtIssuer) ||
            string.IsNullOrEmpty(jwtAudience))
        {
            // Adicionando um log de erro mais explícito
            Console.WriteLine("ERRO FATAL: UMA OU MAIS CONFIGURAÇÕES ESSENCIAIS ESTÃO AUSENTES OU VAZIAS.");
            throw new InvalidOperationException(
                "Erro Fatal: Configurações essenciais estão ausentes. " +
                "Verifique se 'ConnectionStrings:DefaultConnection' e a seção 'Jwt' (com Secret, Issuer, Audience) " +
                "estão configuradas corretamente nas variáveis de ambiente.");
        }
        // --- FIM: VALIDAÇÃO DE CONFIGURAÇÃO ESSENCIAL ---

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(connectionString));

        // ... resto do seu código ...
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
