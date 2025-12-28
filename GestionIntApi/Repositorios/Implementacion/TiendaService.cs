using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Implementacion;
using GestionIntApi.Repositorios.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace GestionIntApi.Repositorios.Implementacion
{
    public class TiendaService: ITiendaService
    {

        private readonly IGenericRepository<TiendaApp> _tiendaAppRepository;
        private readonly IGenericRepository<TiendaMostrarAppVentaDTO> _tiendaAppRepositoryFecha;
        private readonly IMapper _mapper;
        private readonly SistemaGestionDBcontext _context;
        private readonly IGenericRepository<Tienda> _tiendaRepository;




        public TiendaService(
            IGenericRepository<Tienda> tiendaRepository,
            IGenericRepository<TiendaApp> tiendaAppRepository,
            IMapper mapper,
            IGenericRepository<TiendaMostrarAppVentaDTO> tiendaAppRepositoryFecha)
        {
            _tiendaRepository = tiendaRepository;
            _tiendaAppRepository = tiendaAppRepository;
            _mapper = mapper;
            _tiendaAppRepositoryFecha = tiendaAppRepositoryFecha;
        }


        public async Task<TiendaAdminDTO> CrearTiendaAdmin(TiendaAdminDTO dto)
        {
            var tienda = _mapper.Map<Tienda>(dto);

            var creada = await _tiendaRepository.Crear(tienda);

            if (creada.Id == 0)
                throw new Exception("No se pudo crear la tienda");

            return _mapper.Map<TiendaAdminDTO>(creada);
        }

        // ===============================
        // 2️⃣ ADMIN → LISTAR TIENDAS
        // ===============================
        public async Task<List<TiendaAdminDTO>> GetTiendasAdmin()
        {
            var query = await _tiendaRepository.Consultar();
            return _mapper.Map<List<TiendaAdminDTO>>(query.ToList());
        }
        public async Task<TiendaAdminDTO> GetTiendaAdminById(int id)
        {
            var tienda = await _tiendaRepository.Obtener(t => t.Id == id);

            if (tienda == null)
                throw new Exception("La tienda no existe");

            return _mapper.Map<TiendaAdminDTO>(tienda);
        }
        // 3️⃣ APP → ASOCIAR TIENDA A CLIENTE
        // ===============================
        public async Task<bool> AsociarTiendaCliente(TiendaAppDTO dto)
        {
            // Buscar tienda por cédula del encargado
            var tienda = await _tiendaRepository.Obtener(
                t => t.CedulaEncargado == dto.CedulaEncargado
            );

            if (tienda == null)
                throw new Exception("No existe una tienda con esa cédula");

            // Validar que no esté asociada ya al cliente
            var existeRelacion = await _tiendaAppRepository.Obtener(
                ta => ta.ClienteId == dto.ClienteId &&
                      ta.CedulaEncargado == dto.CedulaEncargado
            );

            if (existeRelacion != null)
                throw new Exception("La tienda ya está asociada a este cliente");

            var tiendaApp = new TiendaApp
            {
                TiendaId= tienda.Id,
                CedulaEncargado = tienda.CedulaEncargado,
                ClienteId = dto.ClienteId,
               EstadoDeComision = dto.EstadoDeComision,
                FechaRegistro = DateTime.UtcNow
            };

            await _tiendaAppRepository.Crear(tiendaApp);

            return true;
        }

        // ===============================
        // 4️⃣ APP → OBTENER TIENDAS DEL CLIENTE
        // ===============================
        public async Task<List<TiendaAppDTO>> GetTiendasCliente(int clienteId)
        {
            var query = await _tiendaAppRepository.Consultar(
                t => t.ClienteId == clienteId
            );

            return _mapper.Map<List<TiendaAppDTO>>(query.ToList());
        }


        public async Task<List<TiendaMostrarAppVentaDTO>> GetFechaVenta(int clienteId)
        {
            var query = await _tiendaAppRepository.Consultar(
                t => t.ClienteId == clienteId
            );

            return _mapper.Map<List<TiendaMostrarAppVentaDTO>>(query.ToList());
        }
        // ===============================
        // 5️⃣ ADMIN → ELIMINAR TIENDA
        // ===============================
        public async Task<bool> EliminarTienda(int id)
        {
            var tienda = await _tiendaRepository.Obtener(t => t.Id == id);

            if (tienda == null)
                throw new Exception("La tienda no existe");

            return await _tiendaRepository.Eliminar(tienda);
        }
    }
    /*
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

    */




}


