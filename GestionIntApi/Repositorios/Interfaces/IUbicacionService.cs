using GestionIntApi.DTO;

namespace GestionIntApi.Repositorios.Interfaces
{
    public interface IUbicacionService
    {
        void Registrar(int usuarioId, UbicacionDTO dto);
    }
}
