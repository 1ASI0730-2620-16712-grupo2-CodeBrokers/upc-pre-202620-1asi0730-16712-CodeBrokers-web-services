using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VitaLink.Platform.API.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4_CaregiverRegistration_LinksAndRecordTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "family_caregiver_links",
                schema: "elder_care",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    older_adult_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_family_caregiver_links", x => x.id);
                });

            migrationBuilder.InsertData(
                schema: "monitoring",
                table: "record_types",
                columns: new[] { "code", "display_name", "is_active", "max_valid_value", "min_valid_value", "requires_value", "unit" },
                values: new object[,]
                {
                    { "BLOOD_PRESSURE_DIASTOLIC", "Blood pressure (diastolic)", true, 200m, 20m, true, "mmHg" },
                    { "OXYGEN_SATURATION", "Oxygen saturation", true, 100m, 50m, true, "%" }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_family_caregiver_links_older_adult_id",
                schema: "elder_care",
                table: "family_caregiver_links",
                column: "older_adult_id");

            migrationBuilder.CreateIndex(
                name: "i_x_family_caregiver_links_user_id_older_adult_id",
                schema: "elder_care",
                table: "family_caregiver_links",
                columns: new[] { "user_id", "older_adult_id" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "family_caregiver_links",
                schema: "elder_care");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "BLOOD_PRESSURE_DIASTOLIC");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "OXYGEN_SATURATION");
        }
    }
}
