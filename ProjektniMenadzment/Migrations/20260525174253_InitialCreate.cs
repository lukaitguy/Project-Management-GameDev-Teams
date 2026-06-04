using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProjektniMenadzment.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Korisnici",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Ime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prezime = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BrojTelefona = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Biografija = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime", nullable: false),
                    IdentityUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Korisnici", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Zanrovi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zanrovi", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Projekti",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Budzet = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DatumPocetka = table.Column<DateTime>(type: "datetime", nullable: false),
                    Rok = table.Column<DateOnly>(type: "date", nullable: true),
                    KreiraoKorisnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime", nullable: false),
                    VerzijaIgre = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    Engine = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Platforma = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FazaRazvoja = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    DatumPoslednjegBuilda = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projekti", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projekti_KreiraoKorisnik",
                        column: x => x.KreiraoKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Buildovi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjekatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Verzija = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    NazivBuilda = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TipBuilda = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true),
                    PatchNapomene = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DatumBuilda = table.Column<DateTime>(type: "datetime", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Buildovi__3214EC0721128050", x => x.Id);
                    table.ForeignKey(
                        name: "Fk_Buildovi_Projekat",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClanoviProjekta",
                columns: table => new
                {
                    ProjekatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KorisnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Uloga = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClanoviProjekta", x => new { x.ProjekatId, x.KorisnikId });
                    table.ForeignKey(
                        name: "FK_ClanoviProjekta_Korisnici",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClanoviProjekta_Projekti",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjektiZanrovi",
                columns: table => new
                {
                    ProjekatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ZanrId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjektiZanrovi", x => new { x.ProjekatId, x.ZanrId });
                    table.ForeignKey(
                        name: "FK_ProjektiZanrovi_Projekti",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjektiZanrovi_Zanrovi",
                        column: x => x.ZanrId,
                        principalTable: "Zanrovi",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Resursi",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naziv = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Tip = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Cena = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ProjekatId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DodeljenKorisniku = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resursi", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Resursi_Korisnici",
                        column: x => x.DodeljenKorisniku,
                        principalTable: "Korisnici",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Resursi_Projekti",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Zadaci",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProjekatId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Naslov = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Opis = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: true),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Prioritet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    KreiraoKorisnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DodeljenKorisnikuId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Rok = table.Column<DateOnly>(type: "date", nullable: true),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime", nullable: false),
                    TipZadatka = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Zadaci", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Zadaci_Korisnici",
                        column: x => x.DodeljenKorisnikuId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zadaci_Korisnici1",
                        column: x => x.KreiraoKorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Zadaci_Projekat",
                        column: x => x.ProjekatId,
                        principalTable: "Projekti",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "KomentariZadatak",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Sadrzaj = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    ZadatakId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    KorisnikId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DatumKreiranja = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KomentariZadatak", x => x.Id);
                    table.ForeignKey(
                        name: "FK_KomentariZadatak_Korisnici",
                        column: x => x.KorisnikId,
                        principalTable: "Korisnici",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_KomentariZadatak_Zadaci",
                        column: x => x.ZadatakId,
                        principalTable: "Zadaci",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Buildovi_ProjekatId",
                table: "Buildovi",
                column: "ProjekatId");

            migrationBuilder.CreateIndex(
                name: "IX_ClanoviProjekta_KorisnikId",
                table: "ClanoviProjekta",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_KomentariZadatak_KorisnikId",
                table: "KomentariZadatak",
                column: "KorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_KomentariZadatak_ZadatakId",
                table: "KomentariZadatak",
                column: "ZadatakId");

            migrationBuilder.CreateIndex(
                name: "IX_Projekti_KreiraoKorisnikId",
                table: "Projekti",
                column: "KreiraoKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjektiZanrovi_ZanrId",
                table: "ProjektiZanrovi",
                column: "ZanrId");

            migrationBuilder.CreateIndex(
                name: "IX_Resursi_DodeljenKorisniku",
                table: "Resursi",
                column: "DodeljenKorisniku");

            migrationBuilder.CreateIndex(
                name: "IX_Resursi_ProjekatId",
                table: "Resursi",
                column: "ProjekatId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_DodeljenKorisnikuId",
                table: "Zadaci",
                column: "DodeljenKorisnikuId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_KreiraoKorisnikId",
                table: "Zadaci",
                column: "KreiraoKorisnikId");

            migrationBuilder.CreateIndex(
                name: "IX_Zadaci_ProjekatId",
                table: "Zadaci",
                column: "ProjekatId");

            migrationBuilder.CreateIndex(
                name: "UQ_Zanrovi_Naziv",
                table: "Zanrovi",
                column: "Naziv",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Buildovi");

            migrationBuilder.DropTable(
                name: "ClanoviProjekta");

            migrationBuilder.DropTable(
                name: "KomentariZadatak");

            migrationBuilder.DropTable(
                name: "ProjektiZanrovi");

            migrationBuilder.DropTable(
                name: "Resursi");

            migrationBuilder.DropTable(
                name: "Zadaci");

            migrationBuilder.DropTable(
                name: "Zanrovi");

            migrationBuilder.DropTable(
                name: "Projekti");

            migrationBuilder.DropTable(
                name: "Korisnici");
        }
    }
}
