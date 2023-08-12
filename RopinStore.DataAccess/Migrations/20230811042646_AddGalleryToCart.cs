using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RopinStore.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddGalleryToCart : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ShoppingCartId",
                table: "ProductGalleries",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductGalleries_ShoppingCartId",
                table: "ProductGalleries",
                column: "ShoppingCartId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductGalleries_ShoppingCarts_ShoppingCartId",
                table: "ProductGalleries",
                column: "ShoppingCartId",
                principalTable: "ShoppingCarts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductGalleries_ShoppingCarts_ShoppingCartId",
                table: "ProductGalleries");

            migrationBuilder.DropIndex(
                name: "IX_ProductGalleries_ShoppingCartId",
                table: "ProductGalleries");

            migrationBuilder.DropColumn(
                name: "ShoppingCartId",
                table: "ProductGalleries");
        }
    }
}
