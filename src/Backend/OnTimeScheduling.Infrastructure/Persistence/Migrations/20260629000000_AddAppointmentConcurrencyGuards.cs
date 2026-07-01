using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using OnTimeScheduling.Infrastructure.Persistence.DataAccess;

#nullable disable

namespace OnTimeScheduling.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    [DbContext(typeof(AppDbContext))]
    [Migration("20260629000000_AddAppointmentConcurrencyGuards")]
    public partial class AddAppointmentConcurrencyGuards : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.Sql("""
                ALTER TABLE appointments
                ADD CONSTRAINT ex_appointments_no_professional_overlap
                EXCLUDE USING gist (
                    company_id WITH =,
                    professional_id WITH =,
                    tstzrange(start_time, end_time, '[)') WITH &&
                )
                WHERE (status <> 2);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                ALTER TABLE appointments
                DROP CONSTRAINT IF EXISTS ex_appointments_no_professional_overlap;
                """);
        }
    }
}
