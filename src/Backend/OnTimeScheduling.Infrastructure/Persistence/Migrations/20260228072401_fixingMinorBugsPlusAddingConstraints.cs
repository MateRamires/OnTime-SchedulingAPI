using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class fixingMinorBugsPlusAddingConstraints : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_services_company_id_name",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_locations_company_id_name",
                table: "locations");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "users",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "services",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "professional_services",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "professional_schedules",
                newName: "id");

            migrationBuilder.CreateIndex(
                name: "IX_services_company_id_name",
                table: "services",
                columns: new[] { "company_id", "name" },
                unique: true);

            migrationBuilder.AddCheckConstraint(
                name: "ck_services_duration_positive",
                table: "services",
                sql: "duration_in_minutes > 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_services_price_non_negative",
                table: "services",
                sql: "price >= 0");

            migrationBuilder.AddCheckConstraint(
                name: "ck_professional_schedules_start_before_end",
                table: "professional_schedules",
                sql: "start_time < end_time");

            migrationBuilder.CreateIndex(
                name: "IX_locations_company_id_name",
                table: "locations",
                columns: new[] { "company_id", "name" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_locations_companies_company_id",
                table: "locations",
                column: "company_id",
                principalTable: "companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_locations_companies_company_id",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_services_company_id_name",
                table: "services");

            migrationBuilder.DropCheckConstraint(
                name: "ck_services_duration_positive",
                table: "services");

            migrationBuilder.DropCheckConstraint(
                name: "ck_services_price_non_negative",
                table: "services");

            migrationBuilder.DropCheckConstraint(
                name: "ck_professional_schedules_start_before_end",
                table: "professional_schedules");

            migrationBuilder.DropIndex(
                name: "IX_locations_company_id_name",
                table: "locations");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "users",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "professional_services",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "professional_schedules",
                newName: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_services_company_id_name",
                table: "services",
                columns: new[] { "company_id", "name" });

            migrationBuilder.CreateIndex(
                name: "IX_locations_company_id_name",
                table: "locations",
                columns: new[] { "company_id", "name" });
        }
    }
}
