using Microsoft.OpenApi.Models;
using ToffApi.AuthenticationService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Cors;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using AutoMapper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using ToffApi;
using ToffApi.Models;
using ToffApi.DataAccess;
using ToffApi.DtoModels;
using ToffApi.Hubs;


var builder = WebApplication.CreateBuilder(args);

// read from appsettings.json
var mongoDbSettings = builder.Configuration.GetSection(nameof(MongoDbConfig)).Get<MongoDbConfig>();
var mongoDbName = mongoDbSettings.Name;

// Mapper config
var mapperConfiguration = new MapperConfiguration(cfg => 
{
    cfg.CreateMap<MessageDto, Message>();
    cfg.CreateMap<UserDto, User>();
});
var mapper = new Mapper(mapperConfiguration);

// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services
    .AddIdentity<User, ApplicationRole>()
    .AddMongoDbStores<User, ApplicationRole, Guid>(mongoDbSettings.ConnectionString, mongoDbSettings.Name);
builder.Services.AddSingleton<IAccessTokenManager, AccessTokenManager>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddSingleton<IMessageDataAccess, MessageDataAccess>(provider => new MessageDataAccess(mongoDbSettings.ConnectionString,
    mongoDbName));
builder.Services.AddSingleton<IUserDataAccess, UserDataAccess>(provider => new UserDataAccess(mongoDbSettings.ConnectionString,
    mongoDbName));
builder.Services.AddSingleton<JwtSecurityTokenHandler>();
builder.Services.AddSingleton<IMapper, Mapper>(_ => mapper);
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSignalR();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(o =>
{
    o.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey
        (Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = true
    };
    o.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("X-Access-Token"))
            {
                context.Token = context.Request.Cookies["X-Access-Token"];
            }
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddSwaggerGen(setup => {
    setup.SwaggerDoc(
        "v1",
        new OpenApiInfo
        {
            Title = "Toff API"
        });
    setup.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Description = "Bearer Authentication with JWT Token",
        Type = SecuritySchemeType.Http
    });
    setup.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            new List<string>()
        }
    });
});
builder.Services.AddAuthorization();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllHeaders",
        b =>
        {
            b.WithOrigins("http://127.0.0.1:5173", "https://chat.pancho.moe", "http://localhost:5173")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials();
        });
});



var app = builder.Build();

app.UseRouting();
app.UseCors("AllowAllHeaders");

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

app.MapHub<MessageHub>("/message-hub");

app.Run();
