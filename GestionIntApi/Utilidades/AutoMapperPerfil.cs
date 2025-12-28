using AutoMapper;
using GestionIntApi.DTO;
using GestionIntApi.Models;

namespace GestionIntApi.Utilidades
{
    public class AutoMapperPerfil : Profile
    {


        public AutoMapperPerfil()
        {
            #region Rol
            CreateMap<Rol, RolDTO>().ReverseMap();
            #endregion Rol

            #region Menu
            CreateMap<Menu, MenuDTO>().ReverseMap();
            #endregion Menu

            #region Usuario
            CreateMap<Usuario, UsuarioDTO>()
                 .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                .ForMember(destino =>
                    destino.RolDescripcion,
                    opt => opt.MapFrom(origen => origen.Rol.Descripcion)
                )

                .ForMember(destino =>
                destino.EsActivo,
                opt => opt.MapFrom(origen => origen.EsActivo == true ? 1 : 0)
            );


            CreateMap<Usuario, SesionDTO>()
                .ForMember(destino =>
                    destino.RolDescripcion,
                    opt => opt.MapFrom(origen => origen.Rol.Descripcion)
                );

            CreateMap<UsuarioDTO, Usuario>()
                  .ForMember(dest => dest.Cliente, opt => opt.Ignore())
                 .ForMember(destino =>
                    destino.Rol,
                    opt => opt.Ignore()
                   )
                 .ForMember(destino =>
                    destino.EsActivo,
                    opt => opt.MapFrom(origen => origen.EsActivo == 1 ? true : false)
                   );

            CreateMap<Credito, CreditoMostrarDTO>()
              .ForMember(dest => dest.ProximaCuotaStr,
        opt => opt.MapFrom(src => src.ProximaCuota.ToString("dd/MM/yyyy")))
              .ForMember(dest => dest.FechaCreditoStr,
        opt => opt.MapFrom(src => src.FechaCreacion.ToString("dd/MM/yyyy")));

            #endregion Usuario
            #region DetalleCliente
            CreateMap<DetalleCliente, DetalleClienteDTO>().ReverseMap();
            CreateMap<ClienteDTO, Cliente>()
    .ForMember(dest => dest.Usuario, opt => opt.Ignore())
   // .ForMember(dest => dest.TiendaApps, opt => opt.MapFrom(src => src.TiendasApps))
    .ReverseMap();

            CreateMap<Cliente, ReporteDTO>().ReverseMap();
        //    CreateMap<Cliente, ClienteMostrarAppDTO>()
        //         .ForMember(dest => dest.NombreApellidos,
        //   opt => opt.MapFrom(src => src.DetalleCliente.NombreApellidos))
        //     .ForMember(dest => dest.Correo,
        //opt => opt.MapFrom(src => src.Usuario.Correo));


            #endregion DetalleCliente
            #region Credito
            CreateMap<Credito, CreditoDTO>()
                .ForMember(dest => dest.ProximaCuotaStr,
               opt => opt.MapFrom(src => src.ProximaCuota.ToString("dd/MM/yyyy")));
            CreateMap<CreditoDTO, Credito>()
    .ForMember(dest => dest.ProximaCuota, opt => opt.MapFrom(src => src.ProximaCuota));

            CreateMap<Credito, PagarCreditoDTO>()
            .ForMember(dest => dest.ProximaCuotaStr,
               opt => opt.MapFrom(src => src.ProximaCuota.ToString("dd/MM/yyyy")));

            #endregion Credito
            #region Tienda
            CreateMap<Tienda, TiendaDTO>().ReverseMap();
            CreateMap<Tienda, TiendaMostrarAppDTO>();

            #endregion Tienda


            #region Notificacion

            CreateMap<Notificacion, NotificacionDTO>().ReverseMap();
            #endregion Notificacion



            


            #region Ubicacion

            CreateMap<Ubicacion, UbicacionDTO>().ReverseMap();
            #endregion Ubicacion

            #region TiendaApp
            CreateMap<TiendaApp, TiendaAppDTO>().ReverseMap();
            CreateMap<TiendaAdminDTO, Tienda>().ReverseMap();

            #endregion Ubicacion


            CreateMap<TiendaApp, TiendaMostrarAppVentaDTO>()
    .ForMember(dest => dest.FechaRegistroStr,


               opt => opt.MapFrom(src => src.FechaRegistro.ToString("dd/MM/yyyy")));


            CreateMap<TiendaMostrarAppVentaDTO, TiendaApp>();

        }





    }
}
