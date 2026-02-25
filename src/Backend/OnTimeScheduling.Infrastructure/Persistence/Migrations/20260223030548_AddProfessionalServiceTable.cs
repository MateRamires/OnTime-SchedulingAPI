using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalServiceTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "professional_services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professional_services", x => x.Id);
                    table.ForeignKey(
                        name: "FK_professional_services_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_professional_services_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_professional_services_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_company_id",
                table: "professional_services",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_service_id",
                table: "professional_services",
                column: "service_id");

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_user_id_service_id_company_id",
                table: "professional_services",
                columns: new[] { "user_id", "service_id", "company_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "professional_services");
        }
    }
}
