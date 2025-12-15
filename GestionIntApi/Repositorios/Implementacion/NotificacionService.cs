using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;
using GestionIntApi.Repositorios.Contrato;
using GestionIntApi.Repositorios.Interfaces;

namespace GestionIntApi.Repositorios.Implementacion
{
    

    public class NotificacionService: INotificacionServicio
    {

        private readonly IGenericRepository<Credito> _CreditoRepositorio;
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IMapper _mapper;
        public NotificacionService(IGenericRepository<Credito> CreditoRepositorio,
                           INotificacionRepository notificacionRepository,
                           IMapper mapper)
        {
            _CreditoRepositorio = CreditoRepositorio;
            _notificacionRepository = notificacionRepository;
            _mapper = mapper;
        }


        public async Task GenerarNotificaciones()
        {
            var listaCreditos = await _CreditoRepositorio.Consultar();
            var creditos = listaCreditos.ToList();

            foreach (var credito in creditos)
            {
                // 1. PAGO MAÑANA
                if (credito.ProximaCuota.Date == DateTime.Now.AddDays(1).Date)
                {
                    await CrearNotificacion(credito.ClienteId, "PagoMañana",
                        $"El cliente debe pagar mañana: {credito.ProximaCuota:dd/MM/yyyy}");
                }

                // 2. CUOTA VENCIDA
                if (credito.ProximaCuota.Date < DateTime.Now.Date)
                {
                    await CrearNotificacion(credito.ClienteId, "CuotaVencida",
                        $"La cuota venció el {credito.ProximaCuota:dd/MM/yyyy}");
                }

                // 3. CLIENTE MOROSO (más de 5 días)
                var diasAtraso = (DateTime.Now.Date - credito.ProximaCuota.Date).Days;

                if (diasAtraso >= 5)
                {
                    await CrearNotificacion(credito.ClienteId, "ClienteMoroso",
                        $"El cliente tiene {diasAtraso} días de atraso en el pago.");
                }
            }
        }

        public async Task CrearNotificacion(int clienteId, string tipo, string mensaje)
        {
            var notificacion = new Notificacion
            {
                ClienteId = clienteId,
                Mensaje = mensaje,
                Tipo = tipo,
                Fecha = DateTime.UtcNow
            };

            await _notificacionRepository.Crear(notificacion);
        }

        public async Task<List<NotificacionDTO>> GetNotificaciones()
        {
            var query = await _notificacionRepository.Consultar();
            return _mapper.Map<List<NotificacionDTO>>(query.ToList());
        }
    }
}
