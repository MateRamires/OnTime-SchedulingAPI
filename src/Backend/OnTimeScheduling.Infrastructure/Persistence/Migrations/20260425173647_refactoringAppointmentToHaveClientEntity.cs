using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class refactoringAppointmentToHaveClientEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "client_name",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "client_phone",
                table: "appointments");

            migrationBuilder.AddColumn<Guid>(
                name: "client_id",
                table: "appointments",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_appointments_client_id",
                table: "appointments",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_appointments_company_client_start",
                table: "appointments",
                columns: new[] { "company_id", "client_id", "start_time" });

            migrationBuilder.AddForeignKey(
                name: "FK_appointments_clients_client_id",
                table: "appointments",
                column: "client_id",
                principalTable: "clients",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_appointments_clients_client_id",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "IX_appointments_client_id",
                table: "appointments");

            migrationBuilder.DropIndex(
                name: "ix_appointments_company_client_start",
                table: "appointments");

            migrationBuilder.DropColumn(
                name: "client_id",
                table: "appointments");

            migrationBuilder.AddColumn<string>(
                name: "client_name",
                table: "appointments",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "client_phone",
                table: "appointments",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");
        }
    }
}
