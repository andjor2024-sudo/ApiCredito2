using GestionIntApi.DTO;

namespace GestionIntApi.Repositorios.Interfaces
{
    public interface ITiendaService
    {
        Task<List<TiendaDTO>> GetAllTiendas();
        Task<TiendaDTO> GetTiendaById(int id);
        Task<List<TiendaMostrarAppDTO>> GetTiendasApp(int clienteId);

        Task<TiendaDTO> CreateTienda(TiendaDTO tiendaDto);
        Task<bool> UpdateTienda(TiendaDTO tiendaDto);
        Task<bool> DeleteTienda(int id);


    }
}
