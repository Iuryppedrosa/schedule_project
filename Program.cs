using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using scheduler.Business;
using scheduler.Configuration;
using scheduler.Data;
using scheduler.Repositories;
using System.Text;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtConfig>();

        // Add services to the container.
        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection"))));

        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IUserBusiness, UserBusiness>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(jwtConfig.Secret)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
