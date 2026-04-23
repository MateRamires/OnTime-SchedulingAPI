using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixingClientIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clients_company_id_phone",
                table: "clients");

            migrationBuilder.CreateIndex(
                name: "IX_clients_company_id_phone",
                table: "clients",
                columns: new[] { "company_id", "phone" },
                unique: true,
                filter: "status = 1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_clients_company_id_phone",
                table: "clients");

            migrationBuilder.CreateIndex(
                name: "IX_clients_company_id_phone",
                table: "clients",
                columns: new[] { "company_id", "phone" },
                unique: true);
        }
    }
}
