using MongoDB.Driver;
using MyApi.Services;
using MyApi.Security;
using MyApi.Settings;
using MyApi.Models; // Agregar esta línea
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuración de servicios
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDB"));

builder.Services.AddSingleton<UserService>();

// Configuración de TokenService y JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton<MyApi.Security.TokenService>(); // Usar espacio de nombres completo para evitar ambigüedad

var jwtSettings = builder.Configuration.GetSection("Jwt");
var secretKey = jwtSettings["Key"];
var issuer = jwtSettings["Issuer"];
var audience = jwtSettings["Audience"];

if (string.IsNullOrEmpty(secretKey))
{
    throw new InvalidOperationException("La clave secreta de JWT no está configurada.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = issuer,
            ValidAudience = audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
        };
    });

// Configuración de servicios de controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter JWT token"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// Comprobar conexión a MongoDB
CheckMongoDbConnection(app.Services);

static void CheckMongoDbConnection(IServiceProvider services)
{
    using (var scope = services.CreateScope())
    {
        var mongoDbSettings = scope.ServiceProvider.GetRequiredService<IOptions<MongoDbSettings>>().Value;
        var client = new MongoClient(mongoDbSettings.ConnectionString);
        var database = client.GetDatabase(mongoDbSettings.DatabaseName);

        Console.WriteLine("Comprobando conexión a MongoDB Atlas...");

        try
        {
            var collections = database.ListCollectionNames().ToList();
            Console.WriteLine("Colecciones en la base de datos:");
            collections.ForEach(collection => Console.WriteLine(collection));
            Console.WriteLine("Conexión exitosa");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error al conectar a MongoDB Atlas: " + ex.Message);
        }
    }
}

// Configuración del pipeline de HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Registrar middleware JWT
app.UseMiddleware<JWTMiddleware>();

app.MapControllers();

app.Run();
