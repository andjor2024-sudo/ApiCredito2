namespace GestionIntApi.Models
{
    public class Credito
    {

        public int Id { get; set; }


        // Valor total del celular
        public decimal MontoTotal { get; set; }

        public decimal MontoPendiente { get; set; }
        public decimal Entrada { get; set; }
        public int PlazoCuotas { get; set; }
        public string FrecuenciaPago { get; set; } // semanal, quincenal, mensual
        public DateTime DiaPago { get; set; }

        public decimal ValorPorCuota { get; set; }
        public decimal TotalPagar { get; set; }
        public DateTime ProximaCuota { get; set; }

        public string Estado { get; set; }

        public DateTime FechaCreacion { get; set; }

        // FK a Cliente
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
        // public ICollection<Cliente> Clientes { get; set; }
    }
}
