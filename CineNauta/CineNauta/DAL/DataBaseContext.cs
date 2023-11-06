﻿using Cine_Nauta.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
namespace Cine_Nauta.DAL
{
    public class DataBaseContext : IdentityDbContext<User>
    {

        /*Constructor*/
        public DataBaseContext(DbContextOptions<DataBaseContext> options) : base(options)
        {
        }

        /*Mapeando la entidad para la Tabla*/
        public DbSet<Country> Countries { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Classification> Classifications { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Seat> Seats { get; set; }
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<TemporalSale> TemporalSales { get; set; }



        /*Indicies para las tablas*/
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            /* Se usa para validar que el nombre sea Unico*/

            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex("Name", "CountryId").IsUnique(); // Para estos casos, debo crear un índice Compuesto
            modelBuilder.Entity<City>().HasIndex("Name", "StateId").IsUnique(); // Para estos casos, debo crear un índice Compuesto
            modelBuilder.Entity<Classification>().HasIndex(c => c.ClassificationName).IsUnique();
            modelBuilder.Entity<Gender>().HasIndex(c => c.GenderName).IsUnique();
            modelBuilder.Entity<Room>().HasIndex(r => r.NumberRoom).IsUnique();
            modelBuilder.Entity<Seat>().HasIndex("NumberSeat", "RoomId").IsUnique();
            modelBuilder.Entity<Movie>().HasIndex(m => m.Title).IsUnique();
            modelBuilder.Entity<Function>().HasIndex("FunctionDate", "RoomId").IsUnique();


        }
    }
}
