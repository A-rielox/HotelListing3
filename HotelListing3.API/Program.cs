using HotelListing3.API.Configurations;
using HotelListing3.API.Contracts;
using HotelListing3.API.Data;
using HotelListing3.API.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

/////////////////////////////////////////////////////
// Add services to the container.



// para q la app sepa q se tiene q conectar a la BD a traves de EF y para agarrar el connection string con la config
// el hecho de registrarlo ( con builder.Services. ... ) aca es lo que me permite inyectarlo en los componentes
var connectionString = builder.Configuration.GetConnectionString("HotelListingDbConnectionString");
builder.Services.AddDbContext<HotelListingDbContext>(options => {
    options.UseSqlServer(connectionString);
});




// para Security
// necesito intalar nuGet " Microsoft.AspNetCore.Identity.EntityFrameworkCore "
// builder.Services.AddIdentityCore<IdentityUser>()
    builder.Services.AddIdentityCore<ApiUser>()
        .AddRoles<IdentityRole>()
        //.AddTokenProvider<DataProtectorTokenProvider<ApiUser>>("HotelListingApi")
        .AddEntityFrameworkStores<HotelListingDbContext>();
        //.AddDefaultTokenProviders();



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



// cors para q se pueda acceder desde otros servidores distintos al donde esta la api
// construyo la policy q permite todo lo q le especifique
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", b => b.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod());
});




// ctx -> context, lc -> logger configuration, la configuracion esta en appsettings.json
// ctx.Configuration -> los archivos de configuracion, incluye appsettings.json
builder.Host.UseSerilog((ctx, lc) => lc.WriteTo.Console().ReadFrom.Configuration(ctx.Configuration));


builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
builder.Services.AddScoped<IHotelsRepository, HotelsRepository>();
builder.Services.AddScoped<IAuthManager, AuthManager>();





// configuracion para ocupar JWT, estas opciones son parametros que se activan aqui
// no van a buscar un archivo con options en otro lado.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // verifica mi secret-key
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
            // estas ( abajo ) son las unicas q se buscan en "appsettings.json"
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(builder.Configuration["JwtSettings:Key"])),
        };
    });




builder.Services.AddAutoMapper(typeof(MapperConfig));


var app = builder.Build();


/////////////////////////////////////////////////////
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();



// pongo la policy en el pipeline, la cors policy va a permitir que usuarios q no estan
// en el mismo server q mi app puedan acceder a ella.
app.UseCors("AllowAll");




app.UseAuthorization();

app.MapControllers();

app.Run();
