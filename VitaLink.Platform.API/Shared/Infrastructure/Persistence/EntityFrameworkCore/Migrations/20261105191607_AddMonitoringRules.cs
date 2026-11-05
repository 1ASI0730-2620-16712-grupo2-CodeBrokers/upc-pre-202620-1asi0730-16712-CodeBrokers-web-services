using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddMonitoringRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "monitoring_rules",
                schema: "monitoring",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    older_adult_id = table.Column<Guid>(type: "uuid", nullable: false),
                    record_type_code = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    min_threshold = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    max_threshold = table.Column<decimal>(type: "numeric(10,2)", precision: 10, scale: 2, nullable: true),
                    severity_level = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    description = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_monitoring_rules", x => x.id);
                    table.CheckConstraint("ck_rules_min_max", "min_threshold IS NULL OR max_threshold IS NULL OR min_threshold <= max_threshold");
                    table.CheckConstraint("ck_rules_threshold", "min_threshold IS NOT NULL OR max_threshold IS NOT NULL");
                    table.ForeignKey(
                        name: "f_k_monitoring_rules_record_types_record_type_code",
                        column: x => x.record_type_code,
                        principalSchema: "monitoring",
                        principalTable: "record_types",
                        principalColumn: "code",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_monitoring_rules_older_adult_id_record_type_code",
                schema: "monitoring",
                table: "monitoring_rules",
                columns: new[] { "older_adult_id", "record_type_code" });

            migrationBuilder.CreateIndex(
                name: "i_x_monitoring_rules_record_type_code",
                schema: "monitoring",
                table: "monitoring_rules",
                column: "record_type_code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "monitoring_rules",
                schema: "monitoring");
        }
    }
}
