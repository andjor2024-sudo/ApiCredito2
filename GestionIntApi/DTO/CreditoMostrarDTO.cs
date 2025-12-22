namespace GestionIntApi.DTO
{
    public class CreditoMostrarDTO
    {
        public int Id { get; set; }
        public decimal MontoTotal { get; set; }// Id del crédito        
        public decimal MontoPendiente { get; set; }
        public string ProximaCuotaStr { get; set; }
        public int PlazoCuotas { get; set; }
        public decimal ValorPorCuota { get; set; }
        public string Estado { get; set; }
        public int ClienteId { get; set; }
        public int? TiendaId { get; set; }// Opcional según necesidad
    }

}
