﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SisºFut_SistemaOrganizacionalJogosdeFutsal.Data;

namespace SisºFut_SistemaOrganizacionalJogosdeFutsal.Migrations
{
    [DbContext(typeof(BancoContext))]
    partial class BancoContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 64)
                .HasAnnotation("ProductVersion", "5.0.17");

            modelBuilder.Entity("SisºFut_SistemaOrganizacionalJogosdeFutsal.Models.AgendamentosModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DS_Descricao")
                        .HasColumnType("longtext");

                    b.Property<DateTime>("DT_Agendamento")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("DT_Atualizacao")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("HR_Agendamento")
                        .HasColumnType("longtext");

                    b.Property<string>("Quadra")
                        .HasColumnType("longtext");

                    b.Property<int>("id_Quadra")
                        .HasColumnType("int");

                    b.Property<int>("id_Time1")
                        .HasColumnType("int");

                    b.Property<int?>("id_Time2")
                        .HasColumnType("int");

                    b.Property<bool>("st_ativo")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Agendamentos");
                });

            modelBuilder.Entity("SisºFut_SistemaOrganizacionalJogosdeFutsal.Models.ContatoModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Categoria")
                        .HasColumnType("int");

                    b.Property<string>("Celular")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("UsuarioId")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("Contatos");
                });

            modelBuilder.Entity("SisºFut_SistemaOrganizacionalJogosdeFutsal.Models.JogosEncerradosModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime>("DataEncerramento")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Descricao")
                        .HasColumnType("longtext");

                    b.Property<int>("GolsTime1")
                        .HasColumnType("int");

                    b.Property<int>("GolsTime2")
                        .HasColumnType("int");

                    b.Property<int>("IdQuadra")
                        .HasColumnType("int");

                    b.Property<int>("IdTime1")
                        .HasColumnType("int");

                    b.Property<int>("IdTime2")
                        .HasColumnType("int");

                    b.HasKey("Id");

                    b.ToTable("JogosEncerrados");
                });

            modelBuilder.Entity("SisºFut_SistemaOrganizacionalJogosdeFutsal.Models.QuadrasModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("DS_Endereco")
                        .HasColumnType("longtext");

                    b.Property<string>("NM_Quadra")
                        .HasColumnType("longtext");

                    b.Property<int>("id_Time")
                        .HasColumnType("int");

                    b.Property<bool>("st_ativo")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Quadras");
                });

            modelBuilder.Entity("SisºFut_SistemaOrganizacionalJogosdeFutsal.Models.UsuarioModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<DateTime?>("DataAtualização")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime>("DataCadastro")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Foto")
                        .HasColumnType("longtext");

                    b.Property<string>("Login")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<int>("Perfil")
                        .HasColumnType("int");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("TokenExpiracao")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TokenRedefinicaoSenha")
                        .HasColumnType("longtext");

                    b.Property<bool>("st_ativo")
                        .HasColumnType("tinyint(1)");

                    b.HasKey("Id");

                    b.ToTable("Usuarios");
                });
#pragma warning restore 612, 618
        }
    }
}
