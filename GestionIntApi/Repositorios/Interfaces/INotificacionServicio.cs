using GestionIntApi.DTO;

namespace GestionIntApi.Repositorios.Interfaces
{
    public interface INotificacionServicio
    {
        Task GenerarNotificaciones();
        Task<List<NotificacionDTO>> GetNotificaciones();
    }
}
