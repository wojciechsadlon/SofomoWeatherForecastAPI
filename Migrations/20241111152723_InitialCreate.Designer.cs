﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SofomoWeatherForecastAPI.Data;

#nullable disable

namespace SofomoWeatherForecastAPI.Migrations
{
    [DbContext(typeof(WeatherDbContext))]
    [Migration("20241111152723_InitialCreate")]
    partial class InitialCreate
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("SofomoWeatherForecastAPI.Entities.Location", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<double>("Latitude")
                        .HasColumnType("float");

                    b.Property<double>("Longitude")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.ToTable("Locations");
                });

            modelBuilder.Entity("SofomoWeatherForecastAPI.Entities.WeatherForecast", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ForecastDate")
                        .HasColumnType("datetime2");

                    b.Property<int>("LocationId")
                        .HasColumnType("int");

                    b.Property<double?>("RainSum")
                        .HasColumnType("float");

                    b.Property<double?>("TemperatureMax")
                        .HasColumnType("float");

                    b.Property<double?>("TemperatureMin")
                        .HasColumnType("float");

                    b.Property<double?>("WindSpeedMax")
                        .HasColumnType("float");

                    b.HasKey("Id");

                    b.HasIndex("LocationId");

                    b.ToTable("WeatherForecasts");
                });

            modelBuilder.Entity("SofomoWeatherForecastAPI.Entities.WeatherForecast", b =>
                {
                    b.HasOne("SofomoWeatherForecastAPI.Entities.Location", "Location")
                        .WithMany("WeatherForecasts")
                        .HasForeignKey("LocationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Location");
                });

            modelBuilder.Entity("SofomoWeatherForecastAPI.Entities.Location", b =>
                {
                    b.Navigation("WeatherForecasts");
                });
#pragma warning restore 612, 618
        }
    }
}
