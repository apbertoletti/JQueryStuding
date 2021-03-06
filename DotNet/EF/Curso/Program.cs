﻿using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Curso.Data;
using Curso.Domain;
using DominadoEFCore.Domain;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;

namespace DominadoEFCore
{
    class Program
    {
        static void Main(string[] args)
        {
            //FiltroGlobal();

            //IgnorarFiltroGlobal();

            //ConsultaProjetada();

            //DivisaoDeConsulta();

            //CriarStoredProcedure();

            //InserirDadosViaProcedure();

            //CriarStoredProcedureConsulta();

            //ConsultarDadosViaProcedure();

            //TestandoTimeout();

            //ExecutarEstrategiaResiliencia();

            //OwnedType();

            //Relacionamento1para1();

            //Relacionamento1paraN();

            RelacionamentoNparaN();
        }

        private static void RelacionamentoNparaN()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente1 = new Cliente()
            {
                Nome = "Nome do cliente 1",
            };

            var cliente2 = new Cliente()
            {
                Nome = "Nome do cliente 2",
            };

            var filme1 = new Filme()
            {
                Nome = "Esqueceram de mim",
                AnoLancamento = 1990
            };

            var filme2 = new Filme()
            {
                Nome = "Matrix",
                AnoLancamento = 1999
            };

            var filme3 = new Filme()
            {
                Nome = "Tomates verdes fritos",
                AnoLancamento = 1991
            };

            cliente1.Filmes.Add(filme1);

            filme2.Clientes.Add(cliente1);
            filme2.Clientes.Add(cliente2);

            filme3.Clientes.Add(cliente2);

            db.AddRange(cliente1, cliente2, filme1, filme2, filme3);
            db.SaveChanges();

            var clientes =
                db.Clientes
                    .Include(c => c.Filmes)                   
                    .AsNoTracking()
                    .ToList();

            clientes.ForEach(c =>
            {
                Console.WriteLine($"Nome: {c.Nome}");

                c.Filmes.ToList().ForEach(f =>
                {
                    Console.WriteLine($"- Filme: ({f.Nome}) | {f.AnoLancamento}");
                });
            });
        }

        private static void Relacionamento1paraN()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente()
            {
                Nome = "Nome do cliente",               
                Endereco = new Endereco
                {
                    Logradouro = "Rua das flores, 124",
                    Bairro = "Jardin",
                    Cidade = "Floricultura",
                    Estado = "Flora do Sul"
                },
                Profissao = new Profissao
                {
                    Nome = "Cortador de galhos"
                }
            };

            cliente.Telefones.Add(new Telefone() { DDD = 19, Numero = 34434533 });
            cliente.Telefones.Add(new Telefone() { DDD = 11, Numero = 93445134 });

            cliente.Contas.Add(new ContasReceber() { Valor = 100, Vencimento = DateTime.Now.AddDays(30) });
            cliente.Contas.Add(new ContasReceber() { Valor = 200, Vencimento = DateTime.Now.AddDays(60) });
            cliente.Contas.Add(new ContasReceber() { Valor = 300, Vencimento = DateTime.Now.AddDays(90) });

            db.Clientes.Add(cliente);
            db.SaveChanges();

            var clientes = 
                db.Clientes
                    .Include(c => c.Telefones)
                    .Include(c => c.Contas)
                    .AsNoTracking()
                    .ToList();

            clientes.ForEach(c =>
            {
                Console.WriteLine($"Nome: {c.Nome} | Profissão: {c.Profissao.Nome }");

                c.Telefones.ToList().ForEach(t =>
                {
                    Console.WriteLine($"- Telefone ({t.DDD}) {t.Numero}");
                });

                Console.WriteLine("------------CONTAS-------------");
                c.Contas.ToList().ForEach(r =>
                {
                    Console.WriteLine($"- Valor: ({r.Valor.ToString("C")}) | Vencimento: {r.Vencimento}");
                });
            });
        }

        private static void Relacionamento1para1()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente()
            {
                Nome = "Nome do cliente",
                //Telefone = "Telefone do cliente",
                Endereco = new Endereco
                {
                    Logradouro = "Rua das flores, 124",
                    Bairro = "Jardin",
                    Cidade = "Floricultura",
                    Estado = "Flora do Sul"
                },
                Profissao = new Profissao
                {
                    Nome = "Cortador de galhos"
                }
            };

            db.Clientes.Add(cliente);
            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            clientes.ForEach(c =>
            {
                Console.WriteLine($"Nome: {c.Nome} | Profissão: {c.Profissao.Nome }");
            });
        }

        private static void OwnedType()
        {
            using var db = new Curso.Data.ApplicationContext();
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            var cliente = new Cliente()
            {
                Nome = "Nome do cliente",
                //Telefone = "Telefone do cliente",
                Endereco = new Endereco 
                { 
                    Logradouro = "Rua das flores, 124", 
                    Bairro = "Jardin", 
                    Cidade = "Floricultura",
                    Estado = "Flora do Sul"
                }
            };

            db.Clientes.Add(cliente);
            db.SaveChanges();

            var clientes = db.Clientes.AsNoTracking().ToList();

            var options = new JsonSerializerOptions { WriteIndented = true };
            clientes.ForEach(c =>
            {
                var json = JsonSerializer.Serialize(clientes, options);
                Console.WriteLine(json);
            });
        }

        private static void ExecutarEstrategiaResiliencia()
        {
            using var db = new Curso.Data.ApplicationContext();

            var strategy = db.Database.CreateExecutionStrategy();
            strategy.Execute(() =>
            {
                using var transacation = db.Database.BeginTransaction();

                var newDep = db.Departamentos.Add(new Departamento { Descricao = "Novo departamento adicionado" });
                db.SaveChanges();

                db.Funcionarios.Add(new Funcionario { DepartamentoId = newDep.Entity.Id, Nome = "Novo funcionario adicionado" });
                db.SaveChanges();

                transacation.Commit();
            });
        }

        static void TestandoTimeout()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.SetCommandTimeout(21);

            db.Database.ExecuteSqlRaw("WAITFOR DELAY '00:00:20';SELECT 1;");
        }

        static void ConsultarDadosViaProcedure()
        {
            using var db = new Curso.Data.ApplicationContext();

            var desc = new SqlParameter("@desc", "Dep");

            var departamentos =
                db.Departamentos
                //.FromSqlRaw("EXECUTE SelecionarDepartamento @p0", "Dep")
                //.FromSqlRaw("EXECUTE SelecionarDepartamento @desc", desc)
                .FromSqlInterpolated($"EXECUTE SelecionarDepartamento {desc}")
                .ToList();
            
            foreach (var dep in departamentos)
            {
                Console.WriteLine(dep.Descricao);
            }
        }

        static void CriarStoredProcedureConsulta()
        {
            var scriptSP = @"
            CREATE OR ALTER PROCEDURE SelecionarDepartamento
                @Descricao VARCHAR(50)
            AS
            BEGIN
                SELECT * FROM Departamentos WHERE Descricao LIKE @Descricao + '%'
            END";

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(scriptSP);
        }

        static void InserirDadosViaProcedure()
        {
            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw("EXECUTE CriarDepartamento @p0, @p1", "Departamento novo", true);
        }
        
        static void CriarStoredProcedure()
        {
            var scriptSP = @"
            CREATE OR ALTER PROCEDURE CriarDepartamento
                @Descricao VARCHAR(50),
                @ATivo BIT
            AS
            BEGIN
                INSERT INTO Departamentos (Descricao, Ativo, Excluido)
                VALUES (@Descricao, @Ativo, 0)
            END";

            using var db = new Curso.Data.ApplicationContext();

            db.Database.ExecuteSqlRaw(scriptSP);
        }

        static void DivisaoDeConsulta()
        {
            using var db = new Curso.Data.ApplicationContext();
            SetupDb(db);

            var departamentos = db.Departamentos
                .Include(p => p.Funcionarios)
                .AsSingleQuery()
                .Where(p => p.Id < 3)
                .ToList();

            foreach (var dep in departamentos)
            {
                Console.WriteLine($"Descrição: {dep.Descricao}");
                foreach(var func in dep.Funcionarios)
                {
                    Console.WriteLine($"\tNome: {func.Nome}");
                }
            }   
        }
        static void ConsultaProjetada()
        {
            using var db = new ApplicationContext();
            SetupDb(db);

            var departamentos = db.Departamentos
                .Where(p => p.Id > 0)
                .Select(p => new {
                     p.Descricao, 
                     FuncionarioNomes = p.Funcionarios.Select(f => f.Nome)})
                .ToList();

            foreach(var dep in departamentos)
            {
                Console.WriteLine($"Descrição: {dep.Descricao}");

                foreach(var func in dep.FuncionarioNomes)
                {
                    Console.WriteLine($"\t Nome: {func}");
                }
            }
        }

        static void IgnorarFiltroGlobal()
        {
            using var db = new ApplicationContext();
            SetupDb(db);

            var departamentos = db.Departamentos.IgnoreQueryFilters().Where(p => p.Id > 0).ToList();

            foreach(var dep in departamentos)
            {
                Console.WriteLine($"Descrição: {dep.Descricao} \t Excluido: {dep.Excluido}");
            }
        }

        static void FiltroGlobal()
        {
            using var db = new ApplicationContext();
            SetupDb(db);

            var departamentos = db.Departamentos.Where(p => p.Id > 0).ToList();

            foreach(var dep in departamentos)
            {
                Console.WriteLine($"Descrição: {dep.Descricao} \t Excluido: {dep.Excluido}");
            }
        }
        static void SetupDb(Curso.Data.ApplicationContext db)
        {
            if (db.Database.EnsureCreated())
            {
                if (!db.Departamentos.Any())
                {
                    db.Departamentos.AddRange(
                        new Curso.Domain.Departamento
                        {
                            Ativo = true,
                            Descricao = "Departamento 01",
                            Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                            {
                                new Curso.Domain.Funcionario
                                {
                                    Nome = "Rafael Almeida",
                                    CPF = "99999999911",
                                    Rg= "2100062"
                                }
                            },
                            Excluido = true            
                        },
                        new Curso.Domain.Departamento
                        {
                            Ativo = true,
                            Descricao = "Departamento 02",
                            Funcionarios = new System.Collections.Generic.List<Curso.Domain.Funcionario>
                            {
                                new Curso.Domain.Funcionario
                                {
                                    Nome = "Bruno Brito",
                                    CPF = "88888888811",
                                    Rg = "3100062"
                                },
                                new Curso.Domain.Funcionario
                                {
                                    Nome = "Eduardo Pires",
                                    CPF = "77777777711",
                                    Rg = "1100062"
                                }
                            }
                        },
                        new Curso.Domain.Departamento
                        {
                            Ativo = true,
                            Descricao = "Departamento 03",
                        });

                    db.SaveChanges();
                    db.ChangeTracker.Clear();
                }
            }
        }
    }
}
