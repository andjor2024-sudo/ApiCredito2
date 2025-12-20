using GestionIntApi.DTO;
using GestionIntApi.Models;

namespace GestionIntApi.Repositorios.Implementacion
{
    public class UbicacionService
    {
        private readonly SistemaGestionDBcontext _context;

        public UbicacionService(SistemaGestionDBcontext context)
        {
            _context = context;
        }

        public void Registrar(int usuarioId, UbicacionDTO dto)
        {
            // Crear modelo desde DTO
            var ubicacion = new Ubicacion
            {
                UsuarioId = usuarioId,
                Latitud = dto.Latitud,
                Longitud = dto.Longitud,
                Fecha = DateTime.UtcNow
            };

            _context.Ubicacions.Add(ubicacion);
            _context.SaveChanges();
        }
    }
}
