using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CRUD.Migrations
{
    /// <inheritdoc />
    public partial class CRUD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Supplier",
                columns: table => new
                {
                    idSupplier = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    namaSupplier = table.Column<string>(type: "nvarchar(50)", nullable: false),
                    alamat = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    tlp = table.Column<string>(type: "nvarchar(20)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supplier", x => x.idSupplier);
                });

            migrationBuilder.CreateTable(
                name: "Produk",
                columns: table => new
                {
                    idProduk = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    idSupplier = table.Column<int>(type: "int", nullable: false),
                    namaProduk = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    deskripsi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    qty = table.Column<int>(type: "int", nullable: false),
                    satuan = table.Column<string>(type: "nvarchar(10)", nullable: false),
                    harga = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produk", x => x.idProduk);
                    table.ForeignKey(
                        name: "FK_Produk_Supplier_idSupplier",
                        column: x => x.idSupplier,
                        principalTable: "Supplier",
                        principalColumn: "idSupplier",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produk_idSupplier",
                table: "Produk",
                column: "idSupplier");
        }
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produk");

            migrationBuilder.DropTable(
                name: "Supplier");
        }
    }
}
