using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class appointmentsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "appointments",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: false),
                    service_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    client_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_appointments", x => x.id);
                    table.CheckConstraint("ck_appointments_start_before_end", "start_time < end_time");
                    table.ForeignKey(
                        name: "FK_appointments_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_services_service_id",
                        column: x => x.service_id,
                        principalTable: "services",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_appointments_users_professional_id",
                        column: x => x.professional_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_company_location_start",
                table: "appointments",
                columns: new[] { "company_id", "location_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_company_professional_start",
                table: "appointments",
                columns: new[] { "company_id", "professional_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_appointments_company_start",
                table: "appointments",
                columns: new[] { "company_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "IX_appointments_location_id",
                table: "appointments",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_professional_id",
                table: "appointments",
                column: "professional_id");

            migrationBuilder.CreateIndex(
                name: "IX_appointments_service_id",
                table: "appointments",
                column: "service_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "appointments");
        }
    }
}
