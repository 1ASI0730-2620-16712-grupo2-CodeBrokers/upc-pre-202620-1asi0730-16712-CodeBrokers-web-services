using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddHealthRecords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "monitoring");

            migrationBuilder.CreateTable(
                name: "record_types",
                schema: "monitoring",
                columns: table => new
                {
                    code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    display_name = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    min_valid_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    max_valid_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    requires_value = table.Column<bool>(type: "boolean", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_record_types", x => x.code);
                    table.CheckConstraint("ck_record_types_range", "min_valid_value IS NULL OR max_valid_value IS NULL OR min_valid_value < max_valid_value");
                    table.CheckConstraint("ck_record_types_unit", "requires_value = FALSE OR unit IS NOT NULL");
                });

            migrationBuilder.CreateTable(
                name: "health_records",
                schema: "monitoring",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    older_adult_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_type_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    measurement_value = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    note = table.Column<string>(type: "text", nullable: true),
                    source = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    recorded_by = table.Column<Guid>(type: "uuid", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_health_records", x => x.id);
                    table.CheckConstraint("ck_hr_source", "source IN ('OLDER_ADULT','FAMILY_CAREGIVER','CARE_PROVIDER')");
                    table.CheckConstraint("ck_hr_value_non_negative", "measurement_value IS NULL OR measurement_value >= 0");
                    table.CheckConstraint("ck_hr_value_or_note", "measurement_value IS NOT NULL OR note IS NOT NULL");
                    table.ForeignKey(
                        name: "f_k_health_records_record_types_record_type_code",
                        column: x => x.record_type_code,
                        principalSchema: "monitoring",
                        principalTable: "record_types",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "monitoring",
                table: "record_types",
                columns: new[] { "code", "display_name", "is_active", "max_valid_value", "min_valid_value", "requires_value", "unit" },
                values: new object[,]
                {
                    { "BLOOD_PRESSURE", "Blood pressure (systolic)", true, 300m, 40m, true, "mmHg" },
                    { "BODY_TEMPERATURE", "Body temperature", true, 45m, 30m, true, "°C" },
                    { "FALL_DETECTED", "Fall detected", true, null, null, false, null },
                    { "GENERAL_NOTE", "General note", true, null, null, false, null },
                    { "GLUCOSE", "Blood glucose", true, 600m, 20m, true, "mg/dL" },
                    { "HEART_RATE", "Heart rate", true, 250m, 20m, true, "bpm" },
                    { "MEDICATION_INTAKE", "Medication intake", true, null, null, false, null },
                    { "WEIGHT", "Weight", true, 300m, 1m, true, "kg" }
                });

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_older_adult_id_record_type_code_recorded_at",
                schema: "monitoring",
                table: "health_records",
                columns: new[] { "older_adult_id", "record_type_code", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_older_adult_id_recorded_at",
                schema: "monitoring",
                table: "health_records",
                columns: new[] { "older_adult_id", "recorded_at" });

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_record_type_code",
                schema: "monitoring",
                table: "health_records",
                column: "record_type_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "health_records",
                schema: "monitoring");

            migrationBuilder.DropTable(
                name: "record_types",
                schema: "monitoring");
        }
    }
}
