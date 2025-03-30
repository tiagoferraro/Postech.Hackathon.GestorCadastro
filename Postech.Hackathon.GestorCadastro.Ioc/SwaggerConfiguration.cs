using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System;
using Swashbuckle.AspNetCore.SwaggerGen;
using Microsoft.AspNetCore.Authorization;

namespace Postech.Hackathon.GestorCadastro.Ioc
{
    public static class SwaggerConfiguration
    {
        public static IServiceCollection AddOpenApi(this IServiceCollection services)
        {
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(static options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Postech Hackathon - GestorCadastro API",
                    Version = "v1",
                    Description = "API de autorização para o Hackathon da Postech",
                    Contact = new OpenApiContact
                    {
                        Name = "Equipe Postech",
                        Email = "contato@postech.com"
                    }
                });

                // Configuração para autenticação JWT no Swagger
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header usando o esquema Bearer. Exemplo: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<string>()
                    }
                });

                // Adiciona operação filter para identificar endpoints com [Authorize]
                options.OperationFilter<SecurityRequirementsOperationFilter>();
                
                // Inclui comentários XML se disponíveis
                var xmlFile = $"{Assembly.GetEntryAssembly()?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    options.IncludeXmlComments(xmlPath);
                }
            });

            return services;
        }

        public static WebApplication MapOpenApi(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Postech Hackathon GestorCadastro API v1");
                options.RoutePrefix = string.Empty; // Para servir a UI do Swagger na raiz
                options.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
                options.DefaultModelsExpandDepth(-1); // Oculta a seção de modelos por padrão
            });

            return app;
        }
    }

    // Filtro para identificar endpoints com [Authorize]
    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Verifica se o endpoint tem o atributo [Authorize]
            var authAttributes = context.MethodInfo.DeclaringType!.GetCustomAttributes(true)
                .Union(context.MethodInfo.GetCustomAttributes(true))
                .OfType<AuthorizeAttribute>();

            if (authAttributes.Any())
            {
                // Adiciona o requisito de segurança para endpoints com [Authorize]
                operation.Security =
                [
                    new OpenApiSecurityRequirement
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
                            Array.Empty<string>()
                        }
                    }
                ];

                // Adiciona um ícone de cadeado na descrição
                operation.Description = string.IsNullOrEmpty(operation.Description)
                    ? "🔒 Endpoint protegido - Requer autenticação"
                    : $"🔒 Endpoint protegido - Requer autenticação\n{operation.Description}";
            }
            else
            {
                // Adiciona indicação para endpoints sem [Authorize]
                operation.Description = string.IsNullOrEmpty(operation.Description)
                    ? "🔓 Endpoint público - Não requer autenticação"
                    : $"🔓 Endpoint público - Não requer autenticação\n{operation.Description}";
            }
        }
    }
} 