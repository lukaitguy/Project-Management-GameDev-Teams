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

    public virtual DbSet<Buildovi> Buildovis { get; set; }

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
        modelBuilder.Entity<Buildovi>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Buildovi__3214EC0721128050");

            entity.ToTable("Buildovi");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.DatumBuilda)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.NazivBuilda).HasMaxLength(100);
            entity.Property(e => e.PatchNapomene).HasMaxLength(500);
            entity.Property(e => e.TipBuilda).HasMaxLength(30);
            entity.Property(e => e.Verzija).HasMaxLength(20);

            entity.HasOne(d => d.Projekat).WithMany(p => p.Buildovis)
                .HasForeignKey(d => d.ProjekatId)
                .HasConstraintName("Fk_Buildovi_Projekat");
        });

        modelBuilder.Entity<ClanoviProjektum>(entity =>
        {
            entity.HasKey(e => new { e.ProjekatId, e.KorisnikId });

            entity.Property(e => e.Uloga).HasMaxLength(50);

            entity.HasOne(d => d.Korisnik).WithMany(p => p.ClanoviProjekta)
                .HasForeignKey(d => d.KorisnikId)
                .HasConstraintName("FK_ClanoviProjekta_Korisnici");

            entity.HasOne(d => d.Projekat).WithMany(p => p.ClanoviProjekta)
                .HasForeignKey(d => d.ProjekatId)
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
            entity.Property(e => e.IdentityUserId).HasMaxLength(450);
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
            entity.Property(e => e.DatumPoslednjegBuilda).HasColumnType("datetime");
            entity.Property(e => e.Engine).HasMaxLength(50);
            entity.Property(e => e.FazaRazvoja).HasMaxLength(30);
            entity.Property(e => e.Naziv).HasMaxLength(50);
            entity.Property(e => e.Opis).HasMaxLength(150);
            entity.Property(e => e.Platforma).HasMaxLength(100);
            entity.Property(e => e.Status).HasMaxLength(50);
            entity.Property(e => e.VerzijaIgre).HasMaxLength(20);

            entity.HasOne(d => d.KreiraoKorisnik).WithMany(p => p.Projektis)
                .HasForeignKey(d => d.KreiraoKorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Projekti_KreiraoKorisnik");

            entity.HasMany(d => d.Zanrs).WithMany(p => p.Projekats)
                .UsingEntity<Dictionary<string, object>>(
                    "ProjektiZanrovi",
                    r => r.HasOne<Zanrovi>().WithMany()
                        .HasForeignKey("ZanrId")
                        .HasConstraintName("FK_ProjektiZanrovi_Zanrovi"),
                    l => l.HasOne<Projekti>().WithMany()
                        .HasForeignKey("ProjekatId")
                        .HasConstraintName("FK_ProjektiZanrovi_Projekti"),
                    j =>
                    {
                        j.HasKey("ProjekatId", "ZanrId");
                        j.ToTable("ProjektiZanrovi");
                    });
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
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Resursi_Korisnici");

            entity.HasOne(d => d.Projekat).WithMany(p => p.Resursis)
                .HasForeignKey(d => d.ProjekatId)
                .OnDelete(DeleteBehavior.Cascade)
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
            entity.Property(e => e.TipZadatka).HasMaxLength(30);

            entity.HasOne(d => d.DodeljenKorisniku).WithMany(p => p.ZadaciDodeljenKorisnikus)
                .HasForeignKey(d => d.DodeljenKorisnikuId)
                .HasConstraintName("FK_Zadaci_Korisnici");

            entity.HasOne(d => d.KreiraoKorisnik).WithMany(p => p.ZadaciKreiraoKorisniks)
                .HasForeignKey(d => d.KreiraoKorisnikId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Zadaci_Korisnici1");

            entity.HasOne(d => d.Projekat).WithMany(p => p.Zadacis)
                .HasForeignKey(d => d.ProjekatId)
                .HasConstraintName("FK_Zadaci_Projekat");
        });

        modelBuilder.Entity<Zanrovi>(entity =>
        {
            entity.ToTable("Zanrovi");

            entity.HasIndex(e => e.Naziv, "UQ_Zanrovi_Naziv").IsUnique();

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Naziv).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
