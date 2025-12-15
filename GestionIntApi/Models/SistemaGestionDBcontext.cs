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
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<DetalleCliente> DetallesCliente { get; set; }
        public DbSet<Credito> Creditos { get; set; }

        public DbSet<Notificacion> Notificacions { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seed Roles
            modelBuilder.Entity<Rol>().HasData(
                new Rol { Id = 1, Descripcion = "Administrador", FechaRegistro = new DateTime(2025, 12, 12, 0, 0, 0, DateTimeKind.Utc) },
                new Rol { Id = 2, Descripcion = "Cliente", FechaRegistro = new DateTime(2025, 12, 12, 0, 0, 0, DateTimeKind.Utc) }
            );

            // Seed Menus
            modelBuilder.Entity<Menu>().HasData(
                new Menu { Id = 1, Nombre = "DashBoard", Icono = "dashboard", Url = "/pages/dashboard" },
                new Menu { Id = 2, Nombre = "Usuarios", Icono = "group", Url = "/pages/usuarios" },
                new Menu { Id = 3, Nombre = "Clientes", Icono = "fa-user", Url = "/clientes" },
                new Menu { Id = 4, Nombre = "Venta", Icono = "currency_exchange", Url = "/pages/venta" },
                new Menu { Id = 5, Nombre = "Historial", Icono = "edit_note", Url = "/pages/historial_venta" },
                new Menu { Id = 6, Nombre = "Reportes", Icono = "receipt", Url = "/pages/reportes" }
            );

            // Seed MenuRols
            modelBuilder.Entity<MenuRol>().HasData(
                new MenuRol { Id = 1, MenuId = 1, RolId = 1 },
                new MenuRol { Id = 2, MenuId = 2, RolId = 1 },
                new MenuRol { Id = 3, MenuId = 3, RolId = 1 },
                new MenuRol { Id = 4, MenuId = 4, RolId = 1 },
                new MenuRol { Id = 5, MenuId = 5, RolId = 1 },
                new MenuRol { Id = 6, MenuId = 6, RolId = 1 }
            );
        }
    }

}

