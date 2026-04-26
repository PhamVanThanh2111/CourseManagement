using AutoMapper;
using CourseManagement.API.Data;
using CourseManagement.API.Mappings;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;
using System.Text;

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
                    document.Components ??= new Microsoft.OpenApi.OpenApiComponents();
                    document.Components.SecuritySchemes ??= new Dictionary<string, Microsoft.OpenApi.IOpenApiSecurityScheme>();

                    var securityScheme = new Microsoft.OpenApi.OpenApiSecurityScheme
                    {
                        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
                        Scheme = "bearer",
                        BearerFormat = "JWT",
                        Description = "Dán Token vào đây (Scalar sẽ tự thêm chữ Bearer)"
                    };

                    if (!document.Components.SecuritySchemes.ContainsKey("Bearer"))
                    {
                        document.Components.SecuritySchemes.Add("Bearer", securityScheme);
                    }

                    // 2. Áp dụng Security Requirement lên toàn bộ Document
                    var requirement = new Microsoft.OpenApi.OpenApiSecurityRequirement();
                    requirement.Add(new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer"), new List<string>());

                    document.Security ??= new List<Microsoft.OpenApi.OpenApiSecurityRequirement>();
                    document.Security.Add(requirement);

                    return Task.CompletedTask;
                });
            }); // OpenAPI chính chủ của Microsoft

            // --- 2. Database & AutoMapper ---
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

            // --- 3. JWT Authentication ---
            var jwtKey = builder.Configuration["Jwt:Key"] ?? "SecretKeyMacDinhCuaThanh2026";
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

            // --- 4. Cấu hình Middleware ---
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi(); // Tạo file JSON /openapi/v1.json

                // Truy cập qua /scalar/v1
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
