using AISupportAssist.API.Configuration;
using Azure.AI.Extensions.OpenAI;
using Azure.AI.Projects;
using Azure.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using PaymentAI.API.Middlewares;
using PaymentAI.Core.Entities;
using PaymentAI.Core.Interfaces.Repository;
using PaymentAI.Core.Interfaces.Services;
using PaymentAI.Infrastructure.Configuration;
using PaymentAI.Infrastructure.Data;
using PaymentAI.Infrastructure.Mappings;
using PaymentAI.Infrastructure.Repositories;
using PaymentAI.Infrastructure.Services;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Debug()
    .WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, shared: true)
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers().AddJsonOptions(options =>
    {
        // Serialize enums as strings in JSON responses
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 3,
                maxRetryDelay: TimeSpan.FromSeconds(5),
                errorNumbersToAdd: null);
        }));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));


var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]!))
    };
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPayZenClient",
        policy =>
        {
            policy.WithOrigins("https://payzen-ai.vercel.app")
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

builder.Services.AddScoped<ITransactionRepository,TransactionRepository>();
builder.Services.AddScoped<IRuleRepository,RuleRepository>();
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddScoped<IPaymentService,PaymentService>();
builder.Services.AddScoped<IRiskRuleService,RiskRuleService>();
builder.Services.AddScoped<ITransactionService,TransactionService>();
builder.Services.AddScoped<IRiskRuleService,RiskRuleService>();
builder.Services.AddScoped<IAuditService,AuditService>();
builder.Services.AddScoped<IMerchantService,MerchantService>();
builder.Services.AddScoped<IRiskExplanationService,RiskExplanationService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddHttpClient();

builder.Services.AddAutoMapper(cfg => { }, typeof(RiskRuleProfile).Assembly);

builder.Services.Configure<OpenAISettings>(builder.Configuration.GetSection("OpenAISettings"));

builder.Services.AddSingleton(sp =>
{
    var openAISettings = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
    return new AIProjectClient(new Uri(openAISettings.Endpoint), new DefaultAzureCredential());
});

builder.Services.AddScoped<ProjectResponsesClient>(sp =>
{
    var openAISettings = sp.GetRequiredService<IOptions<OpenAISettings>>().Value;
    var projectClient = sp.GetRequiredService<AIProjectClient>();
    return projectClient.OpenAI.GetProjectResponsesClientForAgent(openAISettings.AgentName);
});


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var app = builder.Build();

app.UseSerilogRequestLogging();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseExceptionHandler("/error");
    app.UseHsts();
}


app.UseHttpsRedirection();
app.UseCors("AllowPayZenClient");

app.UseMiddleware<ApiKeyMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

using (var scope = app.Services.CreateScope())
{
    // Create temporary scope services for the IdentitySeeder
    var services = scope.ServiceProvider;

    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

    await IdentitySeeder.SeedAsync(userManager, roleManager, config);
}
app.MapControllers();

app.Run();

