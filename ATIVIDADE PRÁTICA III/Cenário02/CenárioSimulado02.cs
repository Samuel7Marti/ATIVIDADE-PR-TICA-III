using System;
using System.Collections.Generic;
using System.Linq;

namespace PlataformaCursosCenario02
{
    // ENUMS
    public enum TipoCurso
    {
        Gratuito,
        Pago
    }

    public enum FormaPagamento
    {
        Cartao,
        Pix
    }

    // CLASSES DE DOMÍNIO 
    public class Aula
    {
        public string Id { get; private set; }
        public string Titulo { get; private set; }
        public int DuracaoMinutos { get; private set; }

        public Aula(string titulo, int duracaoMinutos)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Titulo = titulo;
            DuracaoMinutos = duracaoMinutos;
        }
    }

    public class Curso
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public TipoCurso Tipo { get; private set; }
        public decimal Preco { get; private set; }
        public List<Aula> Aulas { get; private set; }

        public Curso(string nome, TipoCurso tipo, decimal preco = 0)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Nome = nome;
            Tipo = tipo;
            Preco = tipo == TipoCurso.Gratuito ? 0 : preco;
            Aulas = new List<Aula>();
        }

        public void AdicionarAula(Aula aula)
        {
            Aulas.Add(aula);
        }
    }

    public class Aluno
    {
        public string Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }

        public Aluno(string nome, string email)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Nome = nome;
            Email = email;
        }
    }

    public class Matricula
    {
        public string Id { get; private set; }
        public Aluno Aluno { get; private set; }
        public Curso Curso { get; private set; }
        public DateTime DataMatricula { get; private set; }
        public bool Pago { get; private set; }
        public FormaPagamento? FormaPagamento { get; private set; }
        
        // Registro de progresso: Guarda o ID das aulas que o aluno já concluiu neste curso
        public List<string> AulasConcluidasIds { get; private set; }

        public Matricula(Aluno aluno, Curso curso)
        {
            Id = Guid.NewGuid().ToString().Substring(0, 8);
            Aluno = aluno;
            Curso = curso;
            DataMatricula = DateTime.Now;
            AulasConcluidasIds = new List<string>();
            
            // Se o curso for gratuito, já nasce aprovado/pago
            Pago = curso.Tipo == TipoCurso.Gratuito;
        }

        public void ProcessarPagamento(FormaPagamento forma)
        {
            if (Curso.Tipo == TipoCurso.Gratuito)
                throw new InvalidOperationException("Cursos gratuitos não exigem pagamento.");

            FormaPagamento = forma;
            Pago = true;
        }

        public void RegistrarProgresso(string aulaId)
        {
            // Valida se a aula pertence ao curso matriculado
            bool aulaExisteNoCurso = Curso.Aulas.Any(a => a.Id == aulaId);
            if (!aulaExisteNoCurso)
                throw new ArgumentException("Esta aula não pertence a este curso.");

            // Valida se o curso pago foi devidamente quitado antes de permitir o progresso
            if (!Pago)
                throw new InvalidOperationException("Não é possível progredir em um curso pago não liquidado.");

            if (!AulasConcluidasIds.Contains(aulaId))
            {
                AulasConcluidasIds.Add(aulaId);
            }
        }

        public double CalcularPercentualProgresso()
        {
            if (Curso.Aulas.Count == 0) return 0;
            
            double percentual = ((double)AulasConcluidasIds.Count / Curso.Aulas.Count) * 100;
            return Math.Round(percentual, 2);
        }
    }

    // FLUXO PRINCIPAL
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(" PLATAFORMA DE CURSOS ONLINE \n");

            // Cadastro de Aluno e Cursos
            Aluno aluno = new Aluno("Samuel Martins", "samuel@gmail.com");

            Curso cursoDotNet = new Curso("Formação C# Completa", TipoCurso.Pago, 299.90m);
            Aula aula1 = new Aula("Introdução ao C#", 45);
            Aula aula2 = new Aula("Orientação a Objetos", 60);
            cursoDotNet.AdicionarAula(aula1);
            cursoDotNet.AdicionarAula(aula2);

            Console.WriteLine($"Aluno Cadastrado: {aluno.Nome}");
            Console.WriteLine($"Curso Criado: {cursoDotNet.Nome} ({cursoDotNet.Tipo}) - Valor: {cursoDotNet.Preco:C}\n");

            // Criação da Matrícula
            Matricula matricula = new Matricula(aluno, cursoDotNet);
            Console.WriteLine($"[MATRÍCULA] Aluno matriculado no curso: {matricula.Curso.Nome}");
            Console.WriteLine($"Status Inicial de Acesso: {(matricula.Pago ? "Liberado" : "Bloqueado (Aguardando Pagamento)")}");

            // Pagamento do Curso Pago
            Console.WriteLine("\nRealizando pagamento via PIX...");
            matricula.ProcessarPagamento(FormaPagamento.Pix);
            Console.WriteLine($"Status de Acesso Após Pagamento: {(matricula.Pago ? "Liberado" : "Bloqueado")}");

            // Registro de Progresso do Aluno
            Console.WriteLine("\n[PROGRESSO] Aluno assistindo as aulas...");
            
            // Concluindo a primeira aula
            matricula.RegistrarProgresso(aula1.Id);
            Console.WriteLine($"Aula Concluída: {aula1.Titulo}");
            Console.WriteLine($"Progresso Atual: {matricula.CalcularPercentualProgresso()}%");

            // Concluindo a segunda aula
            matricula.RegistrarProgresso(aula2.Id);
            Console.WriteLine($"Aula Concluída: {aula2.Titulo}");
            Console.WriteLine($"Progresso Final do Curso: {matricula.CalcularPercentualProgresso()}%");
        }
    }
}