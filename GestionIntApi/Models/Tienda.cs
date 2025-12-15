namespace GestionIntApi.Models
{
    public class Tienda
    {
        public int Id { get; set; }
        public string NombreTienda { get; set; }
        public string NombreEncargado { get; set; }
        public string Telefono { get; set; }
        public string Direccion { get; set; }
        public DateTime FechaRegistro { get; set; } = DateTime.Now;

        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }
    }
}
