using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using ProjektniMenadzment.Models.Domain;

namespace ProjektniMenadzment.Data;

public partial class PMDbContext : DbContext
{
    public PMDbContext()
    {
    }

    public PMDbContext(DbContextOptions<PMDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ClanoviProjektum> ClanoviProjekta { get; set; }

    public virtual DbSet<KomentariZadatak> KomentariZadataks { get; set; }

    public virtual DbSet<Korisnici> Korisnicis { get; set; }

    public virtual DbSet<Projekti> Projektis { get; set; }

    public virtual DbSet<Resursi> Resursis { get; set; }

    public virtual DbSet<Zadaci> Zadacis { get; set; }

    public virtual DbSet<Zanrovi> Zanrovis { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=ConnectionStrings:PMConnectionString");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ClanoviProjektum>(entity =>
        {
            entity.HasKey(e => new { e.ProjekatId, e.KorisnikId });

            entity.Property(e => e.Uloga).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik)
                .WithMany(p => p.ClanoviProjekta)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClanoviProjekta_Korisnici");

            entity.HasOne(d => d.Projekat)
                .WithMany(p => p.ClanoviProjekta)
                .HasForeignKey(d => d.ProjekatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ClanoviProjekta_Projekti");
        });

        modelBuilder.Entity<KomentariZadatak>(entity =>
        {
            entity.ToTable("KomentariZadatak");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");
            entity.Property(e => e.Sadrzaj).HasMaxLength(150);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.KomentariZadataks)
                .HasForeignKey(d => d.KorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KomentariZadatak_Korisnici");

            entity.HasOne(d => d.Zadatak).WithMany(p => p.KomentariZadataks)
                .HasForeignKey(d => d.ZadatakId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_KomentariZadatak_Zadaci");
        });

        modelBuilder.Entity<Korisnici>(entity =>
        {
            entity.ToTable("Korisnici");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Biografija).HasMaxLength(50);
            entity.Property(e => e.BrojTelefona).HasMaxLength(50);
            entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.Ime).HasMaxLength(50);
            entity.Property(e => e.Prezime).HasMaxLength(50);
        });

        modelBuilder.Entity<Projekti>(entity =>
        {
            entity.ToTable("Projekti");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Budzet).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");
            entity.Property(e => e.DatumPocetka).HasColumnType("datetime");
            entity.Property(e => e.Naziv).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(150);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.Zanr).WithMany(p => p.Projektis)
                .HasForeignKey(d => d.ZanrId)
                .HasConstraintName("FK_Projekti_Zanrovi");
        });

        modelBuilder.Entity<Resursi>(entity =>
        {
            entity.ToTable("Resursi");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Cena).HasColumnType("decimal(18, 2)");
            entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");
            entity.Property(e => e.Naziv).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(150);
            entity.Property(e => e.Tip).HasMaxLength(50);

            entity.HasOne(d => d.DodeljenKorisnikuNavigation).WithMany(p => p.Resursis)
                .HasForeignKey(d => d.DodeljenKorisniku)
                .HasConstraintName("FK_Resursi_Korisnici");

            entity.HasOne(d => d.Projekat).WithMany(p => p.Resursis)
                .HasForeignKey(d => d.ProjekatId)
                .HasConstraintName("FK_Resursi_Projekti");
        });

        modelBuilder.Entity<Zadaci>(entity =>
        {
            entity.ToTable("Zadaci");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DatumKreiranja).HasColumnType("datetime");
            entity.Property(e => e.Naslov).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(150);
            entity.Property(e => e.Prioritet).HasMaxLength(50);
            entity.Property(e => e.Status).HasMaxLength(50);

            entity.HasOne(d => d.DodeljenKorisniku).WithMany(p => p.ZadaciDodeljenKorisnikus)
                .HasForeignKey(d => d.DodeljenKorisnikuId)
                .HasConstraintName("FK_Zadaci_Korisnici");

            entity.HasOne(d => d.KreiraoKorisnik).WithMany(p => p.ZadaciKreiraoKorisniks)
                .HasForeignKey(d => d.KreiraoKorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zadaci_Korisnici1");
        });

        modelBuilder.Entity<Zanrovi>(entity =>
        {
            entity.ToTable("Zanrovi");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
