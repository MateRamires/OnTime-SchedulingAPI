using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class addingScheduleBlocksEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "schedule_blocks",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    professional_id = table.Column<Guid>(type: "uuid", nullable: true),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    start_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_time = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    company_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_schedule_blocks", x => x.id);
                    table.CheckConstraint("ck_schedule_blocks_has_scope", "professional_id IS NOT NULL OR location_id IS NOT NULL");
                    table.CheckConstraint("ck_schedule_blocks_start_before_end", "start_time < end_time");
                    table.ForeignKey(
                        name: "FK_schedule_blocks_companies_company_id",
                        column: x => x.company_id,
                        principalTable: "companies",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_schedule_blocks_locations_location_id",
                        column: x => x.location_id,
                        principalTable: "locations",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_schedule_blocks_users_professional_id",
                        column: x => x.professional_id,
                        principalTable: "users",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_schedule_blocks_company_location_start",
                table: "schedule_blocks",
                columns: new[] { "company_id", "location_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_schedule_blocks_company_professional_start",
                table: "schedule_blocks",
                columns: new[] { "company_id", "professional_id", "start_time" });

            migrationBuilder.CreateIndex(
                name: "ix_schedule_blocks_company_time_range",
                table: "schedule_blocks",
                columns: new[] { "company_id", "start_time", "end_time" });

            migrationBuilder.CreateIndex(
                name: "IX_schedule_blocks_location_id",
                table: "schedule_blocks",
                column: "location_id");

            migrationBuilder.CreateIndex(
                name: "IX_schedule_blocks_professional_id",
                table: "schedule_blocks",
                column: "professional_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "schedule_blocks");
        }
    }
}
