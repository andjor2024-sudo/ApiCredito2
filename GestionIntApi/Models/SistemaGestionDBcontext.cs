using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace GestionIntApi.Models
{
    public class SistemaGestionDBcontext: DbContext
    {


        public SistemaGestionDBcontext(DbContextOptions<SistemaGestionDBcontext> options)
            : base(options)
        {
        }


        public DbSet<Usuario> Usuarios { get; set; }
       public virtual DbSet<Rol> Rol { get; set; } = null!;
        public virtual DbSet<MenuRol> MenuRols { get; set; } = null!;
        public virtual DbSet<Menu> Menus { get; set; } = null!;
        public DbSet<EmailSettings> EmailSettings { get; set; }

        public DbSet<VerificationCode> VerificationCodes { get; set; }

        public DbSet<VerificationCode> CodigosVerificacion { get; set; }
        public DbSet<Tienda> Tiendas { get; set; }

        public DbSet<TiendaApp> TiendaApps { get; set; }
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetalleCliente> DetallesCliente { get; set; }
        public DbSet<Credito> Creditos { get; set; }

        public DbSet<Notificacion> Notificacions { get; set; }

        public DbSet<Ubicacion> Ubicacions { get; set; }

        public DbSet<RegistrarPago> RegistrosPagos { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Descripcion = "Administrador", FechaRegistro = new DateTime(2025, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                new Rol { Id = 2, Descripcion = "Cliente", FechaRegistro = new DateTime(2025, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                   new Rol { Id = 3, Descripcion = "Cajera", FechaRegistro = new DateTime(2025, 12, 12, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Seed Menus
            modelBuilder.Entity<Menu>().HasData(
       new Menu { Id = 1, Nombre = "DashBoard", Icono = "dashboard", Url = "/pages/dashboard" },

       // 1. Corregido: Pagos debe ir a una ruta de pagos, no de usuarios
       new Menu { Id = 2, Nombre = "Pagos", Icono = "payments", Url = "/pages/pagos" },

       // 2. Corregido: Si el menú es Bodega, la URL debe ser sobre bodega
       new Menu { Id = 3, Nombre = "RegistrarBodega", Icono = "inventory", Url = "/pages/bodega/registrar" },

       // 3. Corregido: Agrupamos las acciones de bodega bajo la misma ruta base
       new Menu { Id = 4, Nombre = "EditarBodega", Icono = "edit_attributes", Url = "/pages/bodega/editar" },

       // 4. Corregido: Tiendas debe tener su propia ruta
       new Menu { Id = 5, Nombre = "RegistrarTienda", Icono = "storefront", Url = "/pages/tienda/registrar" },

       // 5. Corregido: Movimientos es una sección de historial o logs
       new Menu { Id = 6, Nombre = "Movimientos", Icono = "history", Url = "/pages/movimientos" },

       new Menu { Id = 7, Nombre = "Reportes", Icono = "assessment", Url = "/pages/reportes" },

       // 6. Corregido: Ubicación ahora tiene su propia ruta única
       new Menu { Id = 8, Nombre = "Ubicacion", Icono = "location_on", Url = "/pages/ubicacion" }
   );

            // Seed MenuRols
            modelBuilder.Entity<MenuRol>().HasData(
    // PERMISOS PARA EL ADMINISTRADOR (RolId = 1) - Tiene acceso a todo
    new MenuRol { Id = 1, MenuId = 1, RolId = 1 },
    new MenuRol { Id = 2, MenuId = 2, RolId = 1 },
    new MenuRol { Id = 3, MenuId = 3, RolId = 1 },
    new MenuRol { Id = 4, MenuId = 4, RolId = 1 }, // El Admin sí edita
    new MenuRol { Id = 5, MenuId = 5, RolId = 1 },
    new MenuRol { Id = 6, MenuId = 6, RolId = 1 },
    new MenuRol { Id = 7, MenuId = 7, RolId = 1 },

    // PERMISOS PARA LA CAJERA (RolId = 3) - Se excluye el MenuId 4
    new MenuRol { Id = 8, MenuId = 1, RolId = 3 }, // DashBoard
    new MenuRol { Id = 9, MenuId = 2, RolId = 3 }, // Pagos
    new MenuRol { Id = 10, MenuId = 3, RolId = 3 }, // RegistrarBodega
                                                    // El MenuId 4 (EditarBodega) NO SE AGREGA PARA EL ROL 3
    new MenuRol { Id = 11, MenuId = 5, RolId = 3 }, // RegistrarTienda
    new MenuRol { Id = 12, MenuId = 6, RolId = 3 }, // Movimientos
    new MenuRol { Id = 13, MenuId = 7, RolId = 3 } 
            );
        }
    }

}

