namespace GestionIntApi.Models.Admin
{
    public class UsuarioAdmin
    {

        public int Id { get; set; }
        public string? NombreApellidos { get; set; }
        public string? Correo { get; set; }
        public int? RolId { get; set; }
        public string? Clave { get; set; }
        public bool? EsActivo { get; set; }
        public DateTime? FechaRegistro { get; set; }
        public Rol Rol { get; set; }



    }
}
