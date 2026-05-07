using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using taxi.Data;
using taxi.Interfaces.Repositories;
using taxi.Interfaces.Services;
using taxi.Repositories;
using taxi.Services;
public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

        // Add CORS
        builder.Services.AddCors(options =>
            options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

        builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Taxi API", Version = "v1" });
    
    // Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT"
    });
    
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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
            Array.Empty<string>()
        }
    });
});

//ajout de la connexion a la base de donnee
var cnx = builder.Configuration.GetConnectionString("cnx");
//injection de dependance pour le context
builder.Services.AddDbContext<ApplicationContext>(
    options => options.UseSqlServer(cnx));

builder.Services.AddScoped<IUserAuthService, UserAuthService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaxiRepository, TaxiRepository>();
        builder.Services.AddScoped<IRideRepository, RideRepository>();   
        builder.Services.AddScoped<IAssignmentRepository, AssignmentRepository>();
        builder.Services.   AddScoped<IExpenseRepository, ExpenseRepository>();
        builder.Services.   AddScoped<ICommissionRepository, CommissionRepository>();
        builder.Services.AddScoped<ReportService>();


        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                ),
                // Map role claim correctly
                RoleClaimType = System.Security.Claims.ClaimTypes.Role
            };
        });

        builder.Services.AddAuthorization();



        var app = builder.Build();



        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

        // CORS avant UseHttpsRedirection : sinon le preflight OPTIONS reçoit une 307 vers HTTPS
        // et le navigateur bloque (« Redirect is not allowed for a preflight request »).
        app.UseCors("AllowAll");

        // En dev, pas de redirection HTTP → HTTPS : le front en http://localhost:4200
        // appelle l’API en http://localhost:5067 sans certificat ni 307 vers :7067.
        if (!app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
        }

        // Auth après CORS (inchangé)


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
        }
}

