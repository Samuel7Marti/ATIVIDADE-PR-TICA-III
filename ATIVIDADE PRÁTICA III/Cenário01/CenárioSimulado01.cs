using System;

namespace EstacionamentoCenario01
{
    // ENUMS
    public enum TipoVeiculo
    {
        Carro,
        Moto,
        Caminhao
    }

    public enum FormaPagamento
    {
        Dinheiro,
        Cartao
    }

    // CLASSES DE DOMÍNIO
    public class Veiculo
    {
        public string Placa { get; set; }
        public TipoVeiculo Tipo { get; set; }

        public Veiculo(string placa, TipoVeiculo tipo)
        {
            Placa = placa;
            Tipo = tipo;
        }
    }

    public class Ticket
    {
        public Guid Id { get; private set; }
        public Veiculo Veiculo { get; private set; }
        public DateTime HorarioEntrada { get; private set; }
        public DateTime? HorarioSaida { get; private set; }
        public decimal ValorCobrado { get; private set; }
        public bool Pago { get; private set; }
        public FormaPagamento? FormaPagamento { get; private set; }

        public Ticket(Veiculo veiculo)
        {
            Id = Guid.NewGuid();
            Veiculo = veiculo;
            HorarioEntrada = DateTime.Now;
            Pago = false;
        }

        // Método para registrar a saída simulando o tempo decorrido
        public void RegistrarSaida(DateTime horarioSaida, decimal valor)
        {
            HorarioSaida = horarioSaida;
            ValorCobrado = valor;
        }

        public void ProcessarPagamento(FormaPagamento forma)
        {
            FormaPagamento = forma;
            Pago = true;
        }
    }

    // REGRAS DE NEGÓCIO
    public static class CalculadoraTarifa
    {
        // Define o valor da hora para cada tipo de veículo
        public static decimal Calcular(TipoVeiculo tipo, TimeSpan tempoPermanencia)
        {
            decimal valorPorHora = tipo switch
            {
                TipoVeiculo.Moto => 5.00m,
                TipoVeiculo.Carro => 10.00m,
                TipoVeiculo.Caminhao => 20.00m,
                _ => throw new ArgumentException("Tipo de veículo inválido.")
            };

            // Calcula o total de horas, arredondando para cima, regra comum de estacionamento
            double horas = Math.Ceiling(tempoPermanencia.TotalHours);
            
            // Garante pelo menos 1 hora cobrada se o tempo for mínimo
            if (horas < 1) horas = 1; 

            return valorPorHora * (decimal)horas;
        }
    }

    // FLUXO PRINCIPAL 
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SIMULAÇÃO DE ESTACIONAMENTO \n");

            // Entrada do Veículo
            Veiculo meuCarro = new Veiculo("ABC-1234", TipoVeiculo.Carro);
            Ticket ticket = new Ticket(meuCarro);
            
            Console.WriteLine($"[ENTRADA] {meuCarro.Tipo} de Placa {meuCarro.Placa} entrou às: {ticket.HorarioEntrada}");
            Console.WriteLine($"Ticket Gerado: {ticket.Id}\n");

            // Simulando
            DateTime horarioSaidaSimulado = ticket.HorarioEntrada.AddHours(3).AddMinutes(15);
            TimeSpan tempoPermanencia = horarioSaidaSimulado - ticket.HorarioEntrada;

            // Saída e Cálculo do Valor
            decimal valorTotal = CalculadoraTarifa.Calcular(ticket.Veiculo.Tipo, tempoPermanencia);
            ticket.RegistrarSaida(horarioSaidaSimulado, valorTotal);

            Console.WriteLine($"[SAÍDA] Horário de Saída: {ticket.HorarioSaida}");
            Console.WriteLine($"Tempo de Permanência: {tempoPermanencia.Hours}h {tempoPermanencia.Minutes}m");
            Console.WriteLine($"Valor total a pagar: {ticket.ValorCobrado:C}\n");

            // Pagamento
            FormaPagamento formaEscolhida = FormaPagamento.Cartao;
            ticket.ProcessarPagamento(formaEscolhida);

            Console.WriteLine($"[PAGAMENTO] Pago com sucesso via {ticket.FormaPagamento}!");
            Console.WriteLine($"Status do Ticket: {(ticket.Pago ? "CONCLUÍDO / LIBERADO" : "PENDENTE")}");
        }
    }
}