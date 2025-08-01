using CadastroEmpresaApi.Data;
using CadastroEmpresaApi.DTOs;
using CadastroEmpresaApi.Helpers;
using CadastroEmpresaApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace WebApplication1
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    if (builder.Environment.IsDevelopment())
                    {

                        policy.WithOrigins("http://localhost:3000", "http://192.168.15.114:3000")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                    else
                    {

                        policy.WithOrigins("https://meuwebapp.com") // exemplo
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .AllowCredentials();
                    }
                });
            });

            builder.Services.AddDbContext<AppDbContext>(opt =>
                opt.UseSqlite("Data Source=users.db"));

            builder.Services.AddAuthentication("Bearer")
               .AddJwtBearer("Bearer", opt =>
               {
                   opt.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidateAudience = true,
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       ValidIssuer = builder.Configuration["Jwt:Issuer"],
                       ValidAudience = builder.Configuration["Jwt:Audience"],
                       IssuerSigningKey = new SymmetricSecurityKey(
                           Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                   };

                   opt.Events = new JwtBearerEvents
                   {
                       OnMessageReceived = context =>
                       {
                           if (context.Request.Cookies.ContainsKey("access_token"))
                           {
                               context.Token = context.Request.Cookies["access_token"];
                           }
                           return Task.CompletedTask;
                       }
                   };
               });

            builder.Services.AddAuthorization();

            var app = builder.Build();

            app.UseHttpsRedirection();

            app.UseCors("AllowFrontend");

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapPost("/registrar", async (UsuarioDTO dto, AppDbContext db) =>
            {
                if (await db.Users.AnyAsync(u => u.Email == dto.Email))
                    return Results.BadRequest("E-mail ja foi cadastrado");

                PasswordHasher.CreatePasswordHash(dto.Senha, out byte[] hash, out byte[] salt);

                var usario = new Usuario
                {
                    Nome = dto.Nome,
                    Email = dto.Email,
                    SenhaHash = hash,
                    SenhaSalt = salt
                };

                db.Users.Add(usario);
                await db.SaveChangesAsync();

                return Results.Ok("Usuario Criado");
            });

            app.MapPost("/login", async (
                UsuarioDTO dto,
                AppDbContext db,
                IConfiguration config,
                HttpResponse response,
                IWebHostEnvironment env) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Senha))
                    return Results.BadRequest(new { message = "Email e senha são obrigatórios" });

                var usuario = await db.Users
                    .FirstOrDefaultAsync(u => u.Email.ToLower() == dto.Email.Trim().ToLower());

                if (usuario == null)
                {
                    Console.WriteLine("Usuário não encontrado");
                    return Results.Unauthorized();
                }

                var isValid = PasswordHasher.VerifyPasswordHash(dto.Senha, usuario.SenhaHash, usuario.SenhaSalt);
                if (!isValid)
                    return Results.Unauthorized();

                var token = JwtHelper.GenerateToken(usuario, config);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.None,
                    Expires = DateTime.UtcNow.AddHours(1),
                    Path = "/"
                };

                response.Cookies.Append("access_token", token, cookieOptions);

                return Results.Ok(new { message = "Login efetuado com sucesso" });
            });


            app.MapPost("/empresas", [Authorize] async (EmpresaCreateDTO dto, AppDbContext db, ClaimsPrincipal usuario) =>
            {
                var idUsuario = int.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                DateTime aberturaDate = DateTime.Now;
                if (!string.IsNullOrWhiteSpace(dto.Abertura))
                {
                    DateTime.TryParseExact(dto.Abertura, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out aberturaDate);
                }

                var atividadePrincipal = dto.AtividadePrincipal != null && dto.AtividadePrincipal.Count > 0
                    ? dto.AtividadePrincipal[0].Text
                    : string.Empty;

                var empresa = new Empresa
                {
                    NomeEmpresa = dto.NomeEmpresa,
                    NomeFantasia = dto.NomeFantasia,
                    CNPJ = dto.CNPJ,
                    Situacao = dto.Situacao,
                    Abertura = aberturaDate,
                    Tipo = dto.Tipo,
                    NaturezaPolitica = dto.NaturezaPolitica,
                    AtividadePrincipal = atividadePrincipal,
                    Logradouro = dto.Logradouro,
                    Numero = dto.Numero,
                    Complemento = dto.Complemento,
                    Bairro = dto.Bairro,
                    Municipio = dto.Municipio,
                    UF = dto.UF,
                    CEP = dto.CEP,
                    UsuarioId = idUsuario
                };

                db.Empresas.Add(empresa);
                await db.SaveChangesAsync();

                return Results.Created($"/empresas/{empresa.Id}", empresa);
            });

            app.MapGet("/minhas-empresas", [Authorize] async (AppDbContext db, ClaimsPrincipal usuario) =>
            {
                var IdUsuario = int.Parse(usuario.FindFirst(ClaimTypes.NameIdentifier)!.Value);

                var empresas = await db.Empresas
                    .Where(e => e.UsuarioId == IdUsuario)
                    .ToListAsync();

                return Results.Ok(empresas);
            });

            app.MapGet("receitaws/cnpj/{cnpj}", async (string cnpj) =>
            {
                using var http = new HttpClient();
                var url = $"https://www.receitaws.com.br/v1/cnpj/{cnpj}";
                var dados = await http.GetFromJsonAsync<object>(url);

                return Results.Ok(dados);
            }).RequireCors("AllowFrontend");

            app.Run();
        }
    }
}
