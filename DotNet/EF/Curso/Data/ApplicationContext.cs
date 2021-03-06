using System;
using System.IO;
using Curso.Domain;
using DominadoEFCore.Configurations;
using DominadoEFCore.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Curso.Data
{
    public class ApplicationContext : DbContext
    {                
        public DbSet<Funcionario> Funcionarios { get; set; }
        public DbSet<Departamento> Departamentos { get; set; }
        public DbSet<Cliente> Clientes { get; set; }


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string strConnection = "Data source=(localdb)\\mssqllocaldb; Initial Catalog=MyCourseEFCore-New;Integrated Security=true; Pooling=true";

            optionsBuilder
                .UseSqlServer(strConnection, o =>
                {
                    o.CommandTimeout(16);
                    o.EnableRetryOnFailure(4, TimeSpan.FromSeconds(10), null);
                })
                .EnableSensitiveDataLogging()
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
        }

        public override void Dispose()
        {
            base.Dispose();
        }
    }
}