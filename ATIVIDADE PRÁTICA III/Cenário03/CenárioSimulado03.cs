using System;
using System.Collections.Generic;

namespace ServicosTecnicosCenario03
{
    // Contrato para os tipos de serviço
    public interface ITipoServico
    {
        string Nome { get; }
        decimal PrecoBase { get; }
        void Executar(string localCliente);
    }

    // Regras do serviço de manutenção
    public class ServicoManutencao : ITipoServico
    {
        public string Nome => "Manutenção Corretiva/Preventiva";
        public decimal PrecoBase => 150.00m;

        public void Executar(string localCliente)
        {
            Console.WriteLine($"[EXECUTANDO MANUTENÇÃO] em '{localCliente}': Abrindo equipamento, realizando testes de circuito, limpando componentes e trocando peças desgastadas.");
        }
    }

    // Regras do serviço de instalação
    public class ServicoInstalacao : ITipoServico
    {
        public string Nome => "Instalação de Equipamento";
        public decimal PrecoBase => 250.00m;

        public void Executar(string localCliente)
        {
            Console.WriteLine($"[EXECUTANDO INSTALAÇÃO] em '{localCliente}': Fixando suportes, passando cabeamento estruturado, conectando à rede elétrica e realizando a primeira calibração.");
        }
    }

    // Regras do serviço de suporte
    public class ServicoSuporte : ITipoServico
    {
        public string Nome => "Suporte Técnico";
        public decimal PrecoBase => 90.00m;

        public void Executar(string localCliente)
        {
            Console.WriteLine($"[EXECUTANDO SUPORTE] em '{localCliente}': Atualizando softwares, analisando logs de erro no sistema e prestando treinamento operacional ao usuário.");
        }
    }

    // Estados possíveis da ordem de serviço
    public enum StatusOrdem
    {
        Aberta,
        EmExecucao,
        Finalizada
    }

    // Dados do cliente
    public class Cliente
    {
        public string Nome { get; private set; }
        public string Endereco { get; private set; }

        public Cliente(string nome, string endereco)
        {
            Nome = nome;
            Endereco = endereco;
        }
    }

    // Dados do técnico
    public class Tecnico
    {
        public string Nome { get; private set; }
        public string Especialidade { get; private set; }

        public Tecnico(string nome, string especialidade)
        {
            Nome = nome;
            Especialidade = especialidade;
        }
    }

    // Gerenciamento do ciclo do atendimento
    public class OrdemServico
    {
        public string Protocolo { get; private set; }
        public Cliente Cliente { get; private set; }
        public Tecnico TecnicoAlocado { get; private set; }
        public ITipoServico ServicoSolicitado { get; private set; }
        public StatusOrdem Status { get; private set; }
        public decimal ValorFinal { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public DateTime? DataFinalizacao { get; private set; }

        public OrdemServico(Cliente cliente, ITipoServico servico)
        {
            Protocolo = "OS-" + Guid.NewGuid().ToString().Substring(0, 6).ToUpper();
            Cliente = cliente;
            ServicoSolicitado = servico;
            Status = StatusOrdem.Aberta;
            DataAbertura = DateTime.Now;
            ValorFinal = 0;
        }

        public void AtribuirTecnico(Tecnico tecnico)
        {
            if (Status == StatusOrdem.Finalizada)
                throw new InvalidOperationException("Não é possível alterar o técnico de uma OS já finalizada.");

            TecnicoAlocado = tecnico;
            Status = StatusOrdem.EmExecucao;
            Console.WriteLine($"[STATUS: {Status}] Técnico {tecnico.Nome} foi alocado para a OS {Protocolo}.");
        }

        public void ExecutarServico()
        {
            if (TecnicoAlocado == null)
                throw new InvalidOperationException("O serviço não pode ser executado sem um técnico alocado.");
            
            if (Status != StatusOrdem.EmExecucao)
                throw new InvalidOperationException("A ordem precisa estar em execução para rodar o serviço.");

            Console.WriteLine($"\n-> O técnico {TecnicoAlocado.Nome} chegou ao local.");
            // Executa a lógica específica do serviço por polimorfismo
            ServicoSolicitado.Executar(Cliente.Endereco);
        }

        public void FinalizarOrdem(int horasTrabalhadas)
        {
            if (Status != StatusOrdem.EmExecucao)
                throw new InvalidOperationException("Apenas ordens em execução podem ser finalizadas.");

            Status = StatusOrdem.Finalizada;
            DataFinalizacao = DateTime.Now;

            // Calcula o valor final por horas trabalhadas
            ValorFinal = ServicoSolicitado.PrecoBase * horasTrabalhadas;

            Console.WriteLine($"\n[STATUS: {Status}] Ordem {Protocolo} concluída com sucesso!");
            Console.WriteLine($"Resumo Financeiro: {horasTrabalhadas}h de esforço técnico x {ServicoSolicitado.PrecoBase:C} (Tarifa do serviço) = Total: {ValorFinal:C}\n");
        }
    }

    // Execução da simulação do sistema
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== SISTEMA DE GESTÃO DE SERVIÇOS TÉCNICOS ===\n");

            // Criação dos objetos de teste
            Cliente cliente = new Cliente("QuantumTech Informática", "Av. Rio Branco, 777 - Galpão 7");
            Tecnico tecnicoEspecialista = new Tecnico("Samuel Martins", "Eletromecânica e Redes");

            // Abertura do chamado pelo cliente
            ITipoServico servicoDesejado = new ServicoInstalacao(); 
            OrdemServico os = new OrdemServico(cliente, servicoDesejado);
            
            Console.WriteLine($"[ABERTURA] Cliente {cliente.Nome} abriu uma OS para o serviço: {servicoDesejado.Nome}.");
            Console.WriteLine($"Protocolo Gerado: {os.Protocolo}\n");

            // Alocação do profissional
            os.AtribuirTecnico(tecnicoEspecialista);

            // Realização do trabalho em campo
            os.ExecutarServico();

            // Encerramento e cálculo final
            int esforcoHoras = 3;
            os.FinalizarOrdem(esforcoHoras);
            
            Console.WriteLine("=============================================");
            Console.WriteLine($"FIM DA SIMULAÇÃO: OS {os.Protocolo} Pronta para faturamento.");
        }
    }
}