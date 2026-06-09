using System;
using System.Collections.Generic;

namespace PlataformaStreamingCenario04
{
    // Categorias de planos
    public enum TipoPlano
    {
        Basico,
        Premium
    }

    // Modalidades de pagamento
    public enum MetodoPagamento
    {
        Cartao,
        Pix
    }

    // Tipo de conteúdo do catálogo
    public enum TipoConteudo
    {
        Filme,
        Serie
    }

    // Dados do catálogo de vídeos
    public class Conteudo
    {
        public string Titulo { get; private set; }
        public TipoConteudo Tipo { get; private set; }
        public bool EhExclusivoPremium { get; private set; }

        public Conteudo(string titulo, TipoConteudo tipo, bool ehExclusivoPremium)
        {
            Titulo = titulo;
            Tipo = tipo;
            EhExclusivoPremium = ehExclusivoPremium;
        }
    }

    // Configurações e limites de cada assinatura
    public class PlanoAssinatura
    {
        public TipoPlano Tipo { get; private set; }
        public decimal ValorMensal { get; private set; }
        public int QuantidadeTelas { get; private set; }
        public string QualidadeVideo { get; private set; }

        public PlanoAssinatura(TipoPlano tipo)
        {
            Tipo = tipo;
            ConfigurarPlano();
        }

        // Define os benefícios de acordo com o plano escolhido
        private void ConfigurarPlano()
        {
            if (Tipo == TipoPlano.Basico)
            {
                ValorMensal = 29.90m;
                QuantidadeTelas = 1;
                QualidadeVideo = "HD (720p)";
            }
            else if (Tipo == TipoPlano.Premium)
            {
                ValorMensal = 55.90m;
                QuantidadeTelas = 4;
                QualidadeVideo = "Ultra HD (4K) + HDR";
            }
        }
    }

    // Dados do assinante
    public class Usuario
    {
        public string Nome { get; private set; }
        public string Email { get; private set; }

        public Usuario(string nome, string email)
        {
            Nome = nome;
            Email = email;
        }
    }

    // Controle da assinatura mensal e acessos
    public class Assinatura
    {
        public Usuario Usuario { get; private set; }
        public PlanoAssinatura Plano { get; private set; }
        public bool Ativa { get; private set; }
        public DateTime DataProximaCobranca { get; private set; }

        public Assinatura(Usuario usuario, PlanoAssinatura plano)
        {
            Usuario = usuario;
            Plano = plano;
            Ativa = false; // Começa inativa até o primeiro pagamento
        }

        // Processa o pagamento recorrente mensal
        public void ProcessarPagamentoMensal(MetodoPagamento metodo)
        {
            Ativa = true;
            DataProximaCobranca = DateTime.Now.AddMonths(1);
            Console.WriteLine($"[PAGAMENTO] Sucesso via {metodo}! Valor: {Plano.ValorMensal:C}. Assinatura renovada até {DataProximaCobranca.ToShortDateString()}.");
        }

        // Valida se o usuário pode assistir ao vídeo solicitado
        public void AssistirConteudo(Conteudo conteudo)
        {
            if (!Ativa)
                throw new InvalidOperationException("Acesso negado: Assinatura inadimplente ou inativa.");

            if (conteudo.EhExclusivoPremium && Plano.Tipo != TipoPlano.Premium)
                throw new InvalidOperationException($"Acesso negado: O conteúdo '{conteudo.Titulo}' é exclusivo do plano Premium.");

            Console.WriteLine($"[STREAMING] {Usuario.Nome} está assistindo '{conteudo.Titulo}' em {Plano.QualidadeVideo}. (Telas disponíveis: {Plano.QuantidadeTelas})");
        }
    }

    // Execução da simulação do sistema
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SISTEMA DE PLATAFORMA DE STREAMING ===\n");

            // Criação do catálogo de conteúdos
            Conteudo filmeComum = new Conteudo("Spirit: O Corcel Indomável", TipoConteudo.Filme, ehExclusivoPremium: false);
            Conteudo serieLancamento = new Conteudo("Bridgerton (Nova Temporada)", TipoConteudo.Serie, ehExclusivoPremium: true);

            // Criação do usuário cliente
            Usuario usuario = new Usuario("Samuel Martins", "samuel@gmail.com");
            Console.WriteLine($"Usuário cadastrado: {usuario.Nome}\n");

            // Escolha do plano e abertura da assinatura (Plano Básico)
            PlanoAssinatura planoEscolhido = new PlanoAssinatura(TipoPlano.Basico);
            Assinatura assinatura = new Assinatura(usuario, planoEscolhido);
            Console.WriteLine($"Plano escolhido: {planoEscolhido.Tipo} - {planoEscolhido.QualidadeVideo}");

            // Tentativa de acesso antes do pagamento (Gera erro controlado no fluxo real)
            try
            {
                assinatura.AssistirConteudo(filmeComum);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRÍTICA] {ex.Message}");
            }

            // Realização do pagamento mensal recorrente via PIX
            Console.WriteLine("\nEfetuando pagamento recorrente...");
            assinatura.ProcessarPagamentoMensal(MetodoPagamento.Pix);

            // Acesso liberado ao conteúdo comum
            Console.WriteLine();
            assinatura.AssistirConteudo(filmeComum);

            // Tentativa de acesso a conteúdo Premium com plano Básico
            try
            {
                Console.WriteLine("\nTentando acessar conteúdo exclusivo...");
                assinatura.AssistirConteudo(serieLancamento);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CRÍTICA] {ex.Message}");
            }

            // Upgrade de plano simulado para o cliente
            Console.WriteLine("\n[UPGRADE] Usuário mudou para o Plano Premium.");
            PlanoAssinatura planoPremium = new PlanoAssinatura(TipoPlano.Premium);
            Assinatura assinaturaPremium = new Assinatura(usuario, planoPremium);
            
            // Novo pagamento do novo plano
            assinaturaPremium.ProcessarPagamentoMensal(MetodoPagamento.Cartao);

            // Acesso liberado ao conteúdo Premium após o upgrade
            Console.WriteLine();
            assinaturaPremium.AssistirConteudo(serieLancamento);
        }
    }
}