using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;
using Microsoft.OpenApi;
using CourseManagement.API.Services.Interfaces;

namespace CourseManagement.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- 1. Đăng ký Services căn bản ---
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddOpenApi(options =>
            {
                options.AddDocumentTransformer((document, context, cancellationToken) =>
                {
                    // 1. Khai báo cấu trúc Bearer Token
                    document.Components ??= new OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();

                    var securityScheme = new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Dán Token vào đây (Scalar sẽ tự thêm chữ Bearer)"
                    };

                    if (!document.Components.SecuritySchemes.ContainsKey("Bearer"))
                    {
                        document.Components.SecuritySchemes.Add("Bearer", securityScheme);
                    }

                    // 2. Áp dụng Security Requirement lên toàn bộ Document
                    var requirement = new OpenApiSecurityRequirement();
                    requirement.Add(new OpenApiSecuritySchemeReference("Bearer"), new List<string>());

                    document.Security ??= new List<OpenApiSecurityRequirement>();
                    document.Security.Add(requirement);

                    return Task.CompletedTask;
                });
            }); // OpenAPI chính chủ của Microsoft

            // --- 2. Database & AutoMapper ---
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection"),
                    sqlOptions => sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null)
                ));

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // Register Services
            builder.Services.AddScoped<IAuthService, Services.AuthService>();
            builder.Services.AddScoped<IUserService, Services.UserService>();
            builder.Services.AddScoped<ICourseService, Services.CourseService>();
            builder.Services.AddScoped<Services.ICloudinaryService, Services.CloudinaryService>();

            // --- 3. JWT Authentication ---
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "SecretKeyMacDinh";
            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
                    };
                });

            builder.Services.AddAuthorization();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("RedisConnection") ?? "localhost:6379";
                options.InstanceName = "CourseCatalog_"; // Tiền tố để phân biệt các app khác nhau
            });

            var app = builder.Build();

            // --- Tự động tạo Database và Apply Migration khi khởi động ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<AppDbContext>();
                    // Thực thi các file Migration để tạo Database (CourseDb) và các bảng
                    context.Database.Migrate(); 
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "Lỗi khi apply migration cho database.");
                }
            }

            // --- 4. Cấu hình Middleware ---
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Tạo file JSON /openapi/v1.json

                // Truy cập qua /scalar/v1
                app.MapScalarApiReference();
                app.UseHttpsRedirection(); // Chỉ ép HTTPS trong môi trường Development
            }

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
