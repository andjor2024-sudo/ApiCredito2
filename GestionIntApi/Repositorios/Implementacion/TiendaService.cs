using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Implementacion;
using GestionIntApi.Repositorios.Interfaces;
namespace GestionIntApi.Repositorios.Implementacion
{
    public class TiendaService: ITiendaService
    {

        private readonly IGenericRepository<Tienda> _tiendaRepository;

        private readonly IMapper _mapper;
        private readonly SistemaGestionDBcontext _context;


        public TiendaService(IGenericRepository<Tienda> tiendaRepository, IMapper mapper, SistemaGestionDBcontext context) {

        _tiendaRepository= tiendaRepository;
            _mapper = mapper;
            _context = context;
        
        
        }



        public async Task<List<TiendaDTO>> GetAllTiendas() {
            try
            {
                var queryTienda = await _tiendaRepository.Consultar();

                var listaDetalleCliente = queryTienda.ToList();
                // Recorremos la lista de usuarios y reemplazamos el hash de la contraseña por el texto plano
                return _mapper.Map<List<TiendaDTO>>(listaDetalleCliente);
            }
            catch
            {

                throw;
            }



        }
        public async Task<TiendaDTO> GetTiendaById(int id)
        {
            try
            {
                var detalleEncontrado = await _tiendaRepository.Obtener(u => u.Id == id);


                if (detalleEncontrado == null)
                    throw new TaskCanceledException("Tienda de cliente no encontrado");
                return _mapper.Map<TiendaDTO>(detalleEncontrado);
            }
            catch
            {
                throw;
            }
        }


        public async Task<List<TiendaMostrarAppDTO>> GetTiendasApp(int clienteId)
        {
            try
            {
                var detalleEncontrado = await _tiendaRepository.Consultar(u => u.ClienteId == clienteId);


                if (detalleEncontrado == null)
                    throw new TaskCanceledException("Tienda de cliente no encontrado");
                return _mapper.Map<List<TiendaMostrarAppDTO>>(detalleEncontrado);
            }
            catch
            {
                throw;
            }
        }
       

        public async Task<TiendaDTO> CreateTienda(TiendaDTO modelo)
        {
            try
            {
                var UsuarioCreado = await _tiendaRepository.Crear(_mapper.Map<Tienda>(modelo));

                if (UsuarioCreado.Id == 0)
                    throw new TaskCanceledException("No se pudo crear la tienda");

                return _mapper.Map<TiendaDTO>(UsuarioCreado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> UpdateTienda(TiendaDTO modelo)
        {
            try
            {


                var TiendaModelo = _mapper.Map<TiendaDTO>(modelo);

                var TiendaEncontrado = await _tiendaRepository.Obtener(u => u.Id == TiendaModelo.Id);
                if (TiendaEncontrado == null)
                    throw new TaskCanceledException("La tienda no existe");
                TiendaEncontrado.NombreTienda = TiendaModelo.NombreTienda;
                TiendaEncontrado.NombreEncargado = TiendaModelo.NombreEncargado;
                 TiendaEncontrado.Telefono = TiendaModelo.Telefono;
                TiendaEncontrado.Direccion = TiendaModelo.Direccion;



                bool respuesta = await _tiendaRepository.Editar(TiendaEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> DeleteTienda(int id)
        {
            try
            {
                var tiendaEncontrado = await _tiendaRepository.Obtener(u => u.Id == id);
                if (tiendaEncontrado == null)
                    throw new TaskCanceledException("Tienda no existe");
                bool respuesta = await _tiendaRepository.Eliminar(tiendaEncontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }



        }



    }
}
