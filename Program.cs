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
    // CORRE��O 1: A assinatura do m�todo Main foi corrigida para usar 'string[] args', 
    // que � o padr�o esperado pelo host da aplica��o web.
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // --- IN�CIO: VALIDA��O DE CONFIGURA��O ESSENCIAL (PRINC�PIO FAIL-FAST) ---
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var jwtSection = builder.Configuration.GetSection("Jwt");

        // CORRE��O 2: O valor da chave 'Secret' agora � obtido corretamente como uma string 
        // a partir da se��o de configura��o 'Jwt'. Isso resolve os erros de convers�o.
        var jwtSecret = jwtSection["Secret"];
        var jwtIssuer = jwtSection["Issuer"];
        var jwtAudience = jwtSection["Audience"];

        // A valida��o agora funciona, pois as vari�veis s�o do tipo string.
        if (string.IsNullOrEmpty(connectionString) ||
            string.IsNullOrEmpty(jwtSecret) ||
            string.IsNullOrEmpty(jwtIssuer) ||
            string.IsNullOrEmpty(jwtAudience))
        {
            throw new InvalidOperationException(
                "Erro Fatal: Configura��es essenciais est�o ausentes. " +
                "Verifique se 'ConnectionStrings:DefaultConnection' e a se��o 'Jwt' (com Secret, Issuer, Audience) " +
                "est�o configuradas corretamente nas vari�veis de ambiente ou no appsettings.json.");
        }
        // --- FIM: VALIDA��O DE CONFIGURA��O ESSENCIAL ---

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
               // CORRE��O 3: A vari�vel 'jwtSecret' agora � uma string, permitindo que 
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
