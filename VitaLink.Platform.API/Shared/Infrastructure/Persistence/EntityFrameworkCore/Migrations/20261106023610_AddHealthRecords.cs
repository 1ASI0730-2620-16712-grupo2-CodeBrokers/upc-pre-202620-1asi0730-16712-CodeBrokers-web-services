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
            migrationBuilder.DropIndex(
                name: "i_x_health_records_older_adult_id_record_type_code_recorded_at",
                schema: "monitoring",
                table: "health_records");

            migrationBuilder.DropIndex(
                name: "i_x_health_records_older_adult_id_recorded_at",
                schema: "monitoring",
                table: "health_records");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "BLOOD_PRESSURE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "BODY_TEMPERATURE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "FALL_DETECTED");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "GENERAL_NOTE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "GLUCOSE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "HEART_RATE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "MEDICATION_INTAKE");

            migrationBuilder.DeleteData(
                schema: "monitoring",
                table: "record_types",
                keyColumn: "code",
                keyValue: "WEIGHT");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_valid_value",
                schema: "monitoring",
                table: "record_types",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "max_valid_value",
                schema: "monitoring",
                table: "record_types",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                schema: "monitoring",
                table: "record_types",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(80)",
                oldMaxLength: 80);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                schema: "monitoring",
                table: "record_types",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "source",
                schema: "monitoring",
                table: "health_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "record_type_code",
                schema: "monitoring",
                table: "health_records",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(30)",
                oldMaxLength: 30);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "monitoring",
                table: "health_records",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "measurement_value",
                schema: "monitoring",
                table: "health_records",
                type: "numeric(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(10,2)",
                oldPrecision: 10,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_older_adult_id",
                schema: "monitoring",
                table: "health_records",
                column: "older_adult_id");

            migrationBuilder.CreateIndex(
                name: "i_x_health_records_recorded_at",
                schema: "monitoring",
                table: "health_records",
                column: "recorded_at");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_health_records_older_adult_id",
                schema: "monitoring",
                table: "health_records");

            migrationBuilder.DropIndex(
                name: "i_x_health_records_recorded_at",
                schema: "monitoring",
                table: "health_records");

            migrationBuilder.AlterColumn<decimal>(
                name: "min_valid_value",
                schema: "monitoring",
                table: "record_types",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "max_valid_value",
                schema: "monitoring",
                table: "record_types",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "display_name",
                schema: "monitoring",
                table: "record_types",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "code",
                schema: "monitoring",
                table: "record_types",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "source",
                schema: "monitoring",
                table: "health_records",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "record_type_code",
                schema: "monitoring",
                table: "health_records",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "note",
                schema: "monitoring",
                table: "health_records",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "measurement_value",
                schema: "monitoring",
                table: "health_records",
                type: "numeric(10,2)",
                precision: 10,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "numeric(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

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
        }
    }
}
