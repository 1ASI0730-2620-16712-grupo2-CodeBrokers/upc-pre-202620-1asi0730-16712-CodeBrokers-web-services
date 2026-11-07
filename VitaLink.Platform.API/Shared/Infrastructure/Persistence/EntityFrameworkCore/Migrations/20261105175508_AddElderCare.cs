using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddElderCare : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "elder_care");

            migrationBuilder.CreateTable(
                name: "care_providers",
                schema: "elder_care",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    specialty = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    institution = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    is_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_care_providers", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "older_adults",
                schema: "elder_care",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    first_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    last_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    birth_date = table.Column<DateOnly>(type: "date", nullable: false),
                    address_street = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    address_district = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    address_city = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    emergency_contact_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    emergency_contact_phone = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    assigned_provider_id = table.Column<Guid>(type: "uuid", nullable: true),
                    is_monitoring_active = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_older_adults", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_care_providers_user_id",
                schema: "elder_care",
                table: "care_providers",
                column: "user_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_older_adults_assigned_provider_id",
                schema: "elder_care",
                table: "older_adults",
                column: "assigned_provider_id");

            migrationBuilder.CreateIndex(
                name: "i_x_older_adults_user_id",
                schema: "elder_care",
                table: "older_adults",
                column: "user_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "care_providers",
                schema: "elder_care");

            migrationBuilder.DropTable(
                name: "older_adults",
                schema: "elder_care");
        }
    }
}
