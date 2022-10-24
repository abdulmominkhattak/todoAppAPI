using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Any;
using System.Text;
using toDoApp.Configuration;
using toDoApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtConfig>(builder.Configuration.GetSection("JwtConfig"));
//link the appsetting JwtConfig with the Class JwtConfig on the secret // Feature introduce in dot net core 3.1
//this will allow us to read the secret anywhere in the application. // Use IOC 

builder.Services.AddDbContext<ApiDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnectionString")));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme; // incase first one fails
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    // Now we add JWT TOKEN Configuration.
}).AddJwtBearer(jwt => 
{
    //define the key we are using. The secret Key from congfig
    var key = Encoding.ASCII.GetBytes(builder.Configuration["JwtConfig:Secret"]); //reading key 
    //addding the setting that we want for out JWT Token.
    jwt.SaveToken = true;
    jwt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true, //Validate the Key 
        IssuerSigningKey = new SymmetricSecurityKey(key), //key that will be used to encrpyt
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        RequireExpirationTime = false,
    };

}
);
builder.Services.AddDefaultIdentity<IdentityUser>(options =>
options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApiDbContext>();
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
