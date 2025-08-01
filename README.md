# CadastroEmpresaApi

API minimal em ASP.NET Core usando JWT e SQLite.

---

## Configuração do User Secrets para dados sensíveis

Para não deixar dados sensíveis no `appsettings.json`, como a chave secreta do JWT, utilize o recurso **User Secrets** do .NET.

### Como usar

1. No diretório do projeto, inicialize os User Secrets (caso ainda não tenha feito):

```bash
dotnet user-secrets init
dotnet user-secrets set "Jwt:Key" "SuaChaveSecretaSuperSegura"
dotnet user-secrets set "Jwt:Issuer" "SeuIssuer"
dotnet user-secrets set "Jwt:Audience" "SeuAudience"

