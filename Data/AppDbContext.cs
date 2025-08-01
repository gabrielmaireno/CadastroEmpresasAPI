using CadastroEmpresaApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CadastroEmpresaApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Usuario> Users => Set<Usuario>();
        public DbSet<Empresa> Empresas => Set<Empresa>();
    }


}
