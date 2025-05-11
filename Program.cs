using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json;
namespace MatchGoalAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

			var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
			builder.Services.AddDbContext<ApplicationDbContext>(options =>
				options.UseSqlServer(connectionString));

			builder.Services.AddIdentity<ApplicationUser,IdentityRole>(options =>
									   options.SignIn.RequireConfirmedAccount = true)
	        .AddEntityFrameworkStores<ApplicationDbContext>();

			// Add Swagger services
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen(c =>
			{
				c.SwaggerDoc("v1", new OpenApiInfo { Title = "MatchGoal API", Version = "v1" });

				// ? ????? ??? Security Scheme
				c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Name = "Authorization",
					Type = SecuritySchemeType.Http,
					Scheme = "Bearer",
					BearerFormat = "JWT",
					In = ParameterLocation.Header,
					Description = "???? ?????? ??? ????? ??????: Bearer {token}"
				});

				// ? ????? ??? Security ??? Endpoints
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
			new string[] {}
		}
	});

			});


			// Add services to the container.

			builder.Services.AddAuthentication()
                .AddCookie("Cookies", options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    options.Cookie.Name = "accessToken";
					options.Cookie.HttpOnly = true;
		            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
                    options.Cookie.SameSite = SameSiteMode.Strict;
					options.AccessDeniedPath = "/Account/AccessDenied";
                    options.ExpireTimeSpan = TimeSpan.FromMinutes(120);
				});

			builder.Services.AddHttpClient();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
					builder =>
					{
						builder.WithOrigins("http://localhost:4200")
							   .AllowCredentials()
							   .AllowAnyMethod()
							   .AllowAnyHeader();
					});
			});

			builder.Services.AddScoped<IBaseRepository<Match>,MatchRepository>();
            builder.Services.AddScoped<IMatchRepository, MatchRepository>();
            builder.Services.AddScoped<IAuthRepository, AuthRepository>();
            builder.Services.AddScoped<ITeamRepository, TeamRepository>();
			builder.Services.AddScoped<IPlayListRepository, PlayListRepository>();

			builder.Services.AddLogging(logging =>
			{
				logging.AddConsole();
				logging.AddDebug();
			});

            // mapping services
			builder.Services.AddAutoMapper(typeof(MatchProfile));
            builder.Services.AddAutoMapper(typeof(TeamMapperProfile));
			builder.Services.AddAutoMapper(typeof(CompetitionMapperProfile));

			builder.Services.AddControllers()
	        .AddJsonOptions(options =>
	        {
		    options.JsonSerializerOptions.Converters.Add(
			new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, allowIntegerValues: true) // allowIntegerValues: true
		    );
		    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
		    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
	        }); ;
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            // JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
					RequireExpirationTime = true,
					ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
                    ClockSkew = TimeSpan.Zero
                };

				options.Events = new JwtBearerEvents
				{
					OnTokenValidated = context =>
					{
						var jwtToken = context.SecurityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
						var tokenString = jwtToken?.RawData;
						var dbContext = context.HttpContext.RequestServices.GetRequiredService<ApplicationDbContext>();
						if (dbContext.blackListedTokens.Any(t => t.Token == tokenString && t.ExpireTime > DateTime.UtcNow))
						{
							context.Fail("Token is blacklisted");
						}
						return Task.CompletedTask;
					}
				};
            });

			var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
				app.UseSwagger();
				app.UseSwaggerUI(c =>
				{
					c.SwaggerEndpoint("/swagger/v1/swagger.json", "MatchGoal API V1");
					c.RoutePrefix = "";
				});
				app.MapOpenApi();
            }

			// if token come in cookies only then add it to the request header 
			app.UseCors("CorsPolicy");

			app.Use(async (context, next) =>
            {
                if (context.Request.Cookies.TryGetValue("accessToken",out string accessToken))
				{
					context.Request.Headers.Add("Authorization", "Bearer " + accessToken);
				}
                await next(context);
			});

            app.UseAuthentication();
			app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
