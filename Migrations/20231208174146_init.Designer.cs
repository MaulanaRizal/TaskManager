﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using TaskManager.Data;

#nullable disable

namespace TaskManager.Migrations
{
    [DbContext(typeof(dataContext))]
    [Migration("20231208174146_init")]
    partial class init
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.25")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("TaskManager.Models.Task", b =>
                {
                    b.Property<int>("MyProperty")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("CreateAt")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("UpdateAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("description")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("status")
                        .HasColumnType("int");

                    b.Property<string>("title")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("varchar(255)");

                    b.HasKey("MyProperty");

                    b.ToTable("Task");
                });
#pragma warning restore 612, 618
        }
    }
}
