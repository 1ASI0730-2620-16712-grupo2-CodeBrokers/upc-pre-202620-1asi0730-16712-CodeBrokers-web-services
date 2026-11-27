using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Migrations
{
    /// <inheritdoc />
    public partial class Sprint4_ElderCare_Clinicas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "clinic_id",
                schema: "elder_care",
                table: "care_providers",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "license_number",
                schema: "elder_care",
                table: "care_providers",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "clinics",
                schema: "elder_care",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    district = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_clinics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "license_registries",
                schema: "elder_care",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    license_number = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    holder_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_license_registries", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_care_providers_clinic_id",
                schema: "elder_care",
                table: "care_providers",
                column: "clinic_id");

            migrationBuilder.CreateIndex(
                name: "i_x_care_providers_license_number",
                schema: "elder_care",
                table: "care_providers",
                column: "license_number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_clinics_name",
                schema: "elder_care",
                table: "clinics",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_license_registries_license_number",
                schema: "elder_care",
                table: "license_registries",
                column: "license_number",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "clinics",
                schema: "elder_care");

            migrationBuilder.DropTable(
                name: "license_registries",
                schema: "elder_care");

            migrationBuilder.DropIndex(
                name: "i_x_care_providers_clinic_id",
                schema: "elder_care",
                table: "care_providers");

            migrationBuilder.DropIndex(
                name: "i_x_care_providers_license_number",
                schema: "elder_care",
                table: "care_providers");

            migrationBuilder.DropColumn(
                name: "clinic_id",
                schema: "elder_care",
                table: "care_providers");

            migrationBuilder.DropColumn(
                name: "license_number",
                schema: "elder_care",
                table: "care_providers");
        }
    }
}
