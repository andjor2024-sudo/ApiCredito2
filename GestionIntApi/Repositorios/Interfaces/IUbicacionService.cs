using GestionIntApi.DTO;

namespace GestionIntApi.Repositorios.Interfaces
{
    public interface IUbicacionService
    {
        Task<UbicacionDTO> Registrar(double latitud, double longitud, int usuarioId);
        Task<List<UbicacionDTO>> ObtenerPorUsuario(int usuarioId);
        Task<UbicacionDTO> ObtenerUltima(int usuarioId);
    }
}
