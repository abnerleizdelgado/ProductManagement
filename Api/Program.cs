using Domain.Interfaces;
using Domain.Interfaces.EmployeeManagement.Domain.Interfaces.Repositories;
using Domain.Services;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Net.Sockets;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to listen on port 80 inside the container
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(80);
});


// Configuração do CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins", policy =>
    {
        policy.AllowAnyOrigin() 
              .AllowAnyMethod() 
              .AllowAnyHeader();
    });
});


// Configuração do JWT, banco de dados e outros serviços
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });

// Add DbContext with SQL Server configuration
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
     sqlOptions => sqlOptions.MigrationsAssembly("Infrastructure")));

builder.Services.AddAutoMapper(typeof(MappingProfileData));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
await app.MigrateDatabaseAsync();

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowAllOrigins");
}
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();


public static class WebApplicationExtensions
{
    public static async Task<IHost> MigrateDatabaseAsync(this IHost webHost)
    {
        using (var scope = webHost.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var context = services.GetRequiredService<AppDbContext>();

            var retryCount = 0;
            var maxRetryCount = 10;
            var delay = TimeSpan.FromSeconds(10);

            while (retryCount < maxRetryCount)
            {
                try
                {
                    Console.WriteLine("Verificando a disponibilidade da porta 1433...");

                    var connectionString = context.Database.GetConnectionString();
                    var builder = new SqlConnectionStringBuilder(connectionString);
                    var host = builder.DataSource.Split(',')[0];
                    var port = 1433; 

                    using (var tcpClient = new TcpClient())
                    {
                        var connectTask = tcpClient.ConnectAsync(host, port);
                        var completedTask = await Task.WhenAny(connectTask, Task.Delay(TimeSpan.FromSeconds(5)));

                        if (completedTask != connectTask)
                        {
                            Console.WriteLine("Porta 1433 não está disponível. Tentando novamente...");
                            retryCount++;
                            if (retryCount >= maxRetryCount)
                                throw new Exception("Número máximo de tentativas alcançado. Não foi possível conectar à porta 1433.");
                            await Task.Delay(delay); 
                            continue;
                        }
                    }

                    Console.WriteLine("Porta 1433 está disponível.");

                    using (var connection = new SqlConnection(connectionString))
                    {
                        await connection.OpenAsync();

                        using (var command = connection.CreateCommand())
                        {
                            command.CommandText = @"
                                SELECT COUNT(*) 
                                FROM sys.databases 
                                WHERE name = @databaseName";
                            command.Parameters.AddWithValue("@databaseName", builder.InitialCatalog);

                            var result = (int)await command.ExecuteScalarAsync();
                            if (result > 0)
                            {
                                Console.WriteLine($"O banco de dados '{builder.InitialCatalog}' já existe. Migração ignorada.");
                                return webHost;
                            }
                        }
                    }

                    Console.WriteLine($"O banco de dados '{builder.InitialCatalog}' não existe. Executando migrações...");
                    await context.Database.MigrateAsync();
                    break;
                }
                catch (SqlException ex)
                {
                    retryCount++;
                    Console.WriteLine($"Erro ao conectar ao banco de dados: {ex.Message}");
                    if (retryCount >= maxRetryCount)
                        throw;

                    Console.WriteLine("Tentando novamente em alguns segundos...");
                    await Task.Delay(delay); 
                }
                catch (Exception ex)
                {
                    retryCount++;
                    Console.WriteLine($"Erro genérico: {ex.Message}");
                    if (retryCount >= maxRetryCount)
                        throw;

                    Console.WriteLine("Tentando novamente em alguns segundos...");
                    await Task.Delay(delay); 
                }
            }
        }

        return webHost;
    }
}



