using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddProfessionalScheduleAndIndexes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_services_company_id",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_professional_services_company_id",
                table: "professional_services");

            migrationBuilder.DropIndex(
                name: "IX_professional_services_user_id_service_id_company_id",
                table: "professional_services");

            migrationBuilder.CreateTable(
                name: "professional_schedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    day_of_week = table.Column<int>(type: "integer", nullable: false),
                    start_time = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    end_time = table.Column<TimeSpan>(type: "time without time zone", nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_professional_schedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_professional_schedules_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_professional_schedules_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_professional_schedules_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id_role",
                table: "users",
                columns: new[] { "company_id", "role" });

            migrationBuilder.CreateIndex(
                name: "IX_users_company_id_status",
                table: "users",
                columns: new[] { "company_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_services_company_id_name",
                table: "services",
                columns: new[] { "company_id", "name" });

            migrationBuilder.CreateIndex(
                name: "IX_services_company_id_status",
                table: "services",
                columns: new[] { "company_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_company_id_service_id",
                table: "professional_services",
                columns: new[] { "company_id", "service_id" });

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_company_id_user_id_service_id",
                table: "professional_services",
                columns: new[] { "company_id", "user_id", "service_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_user_id",
                table: "professional_services",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_locations_company_id_name",
                table: "locations",
                columns: new[] { "company_id", "name" });

            migrationBuilder.CreateIndex(
                name: "IX_locations_company_id_status",
                table: "locations",
                columns: new[] { "company_id", "status" });

            migrationBuilder.CreateIndex(
                name: "IX_professional_schedules_company_id_location_id_day_of_week",
                table: "professional_schedules",
                columns: new[] { "company_id", "location_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "IX_professional_schedules_company_id_user_id_day_of_week",
                table: "professional_schedules",
                columns: new[] { "company_id", "user_id", "day_of_week" });

            migrationBuilder.CreateIndex(
                name: "IX_professional_schedules_location_id",
                table: "professional_schedules",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_professional_schedules_user_id",
                table: "professional_schedules",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "professional_schedules");

            migrationBuilder.DropIndex(
                name: "IX_users_company_id_role",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_users_company_id_status",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_services_company_id_name",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_services_company_id_status",
                table: "services");

            migrationBuilder.DropIndex(
                name: "IX_professional_services_company_id_service_id",
                table: "professional_services");

            migrationBuilder.DropIndex(
                name: "IX_professional_services_company_id_user_id_service_id",
                table: "professional_services");

            migrationBuilder.DropIndex(
                name: "IX_professional_services_user_id",
                table: "professional_services");

            migrationBuilder.DropIndex(
                name: "IX_locations_company_id_name",
                table: "locations");

            migrationBuilder.DropIndex(
                name: "IX_locations_company_id_status",
                table: "locations");

            migrationBuilder.CreateIndex(
                name: "IX_services_company_id",
                table: "services",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_company_id",
                table: "professional_services",
                column: "company_id");

            migrationBuilder.CreateIndex(
                name: "IX_professional_services_user_id_service_id_company_id",
                table: "professional_services",
                columns: new[] { "user_id", "service_id", "company_id" },
                unique: true);
        }
    }
}
