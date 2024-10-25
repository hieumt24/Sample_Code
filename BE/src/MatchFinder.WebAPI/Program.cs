using MatchFinder.Application.Installers;
using MatchFinder.Application.Middlewares;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.DataAccess;
using MatchFinder.Infrastructure.Installers;
using MatchFinder.Infrastructure.Services.Core;
using MatchFinder.WebAPI.Backgrounds;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.Configure<HostOptions>(options =>
{
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.Ignore;
});

builder.Services.AddMvcCore().ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState
            .Where(ms => ms.Value.Errors.Count > 0)
            .ToDictionary(
                er => er.Key,
                er => er.Value.Errors.Select(e => e.ErrorMessage).ToArray()
            );

        var result = new InvalidDataResponse
        {
            Message = "One or more validation errors occurred.",
            Errors = errors
        };

        return new BadRequestObjectResult(result);
    };
});
builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));
builder.Services.Configure<FileAzureSettings>(builder.Configuration.GetSection("FileAzureSettings"));
builder.Services.Configure<PayOSSettings>(builder.Configuration.GetSection(PayOSSettings.ConfigName));
builder.Services.AddInfrastructureServices();
builder.Services.AddApplicationServices();
builder.Services.AddDbContext<MatchFinderContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddHostedService<BookingStatusUpdater>();
builder.Services.AddHostedService<OpponentFindingOverdueUpdater>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opt =>
{
    opt.MapType<DateOnly>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "date"
    });
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    opt.IncludeXmlComments(xmlPath);
    var securitySchema = new OpenApiSecurityScheme
    {
        Name = "JWT Authentication",
        Description = "Enter JWT Bearer",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    opt.AddSecurityDefinition("Bearer", securitySchema);
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { securitySchema, new[] { "Bearer" } }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes
                (builder.Configuration["Jwt:Key"]))
        };

        opt.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(builder =>
    {
        builder.WithOrigins("*")
               .AllowAnyHeader()
               .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.UseMiddleware<ExceptionMiddleware>();
app.UseInfrastructureEndpointRoute();
app.Run();