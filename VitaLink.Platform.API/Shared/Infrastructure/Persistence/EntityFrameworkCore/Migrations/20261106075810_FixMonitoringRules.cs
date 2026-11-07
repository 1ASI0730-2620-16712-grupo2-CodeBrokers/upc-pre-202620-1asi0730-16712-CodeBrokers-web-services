using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class FixMonitoringRules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "i_x_monitoring_rules_older_adult_id_record_type_code",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropCheckConstraint(
                name: "ck_rules_min_max",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropCheckConstraint(
                name: "ck_rules_threshold",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropColumn(
                name: "description",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropColumn(
                name: "severity_level",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.RenameColumn(
                name: "min_threshold",
                schema: "monitoring",
                table: "monitoring_rules",
                newName: "min_value");

            migrationBuilder.RenameColumn(
                name: "max_threshold",
                schema: "monitoring",
                table: "monitoring_rules",
                newName: "max_value");

            migrationBuilder.AddColumn<string>(
                name: "severity",
                schema: "monitoring",
                table: "monitoring_rules",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "uq_mr_active_rule",
                schema: "monitoring",
                table: "monitoring_rules",
                columns: new[] { "older_adult_id", "record_type_code", "severity" },
                unique: true,
                filter: "is_active");

            migrationBuilder.AddCheckConstraint(
                name: "ck_mr_at_least_one_bound",
                schema: "monitoring",
                table: "monitoring_rules",
                sql: "min_value IS NOT NULL OR max_value IS NOT NULL");

            migrationBuilder.AddCheckConstraint(
                name: "ck_mr_bounds_consistent",
                schema: "monitoring",
                table: "monitoring_rules",
                sql: "min_value IS NULL OR max_value IS NULL OR min_value <= max_value");

            migrationBuilder.AddCheckConstraint(
                name: "ck_mr_severity",
                schema: "monitoring",
                table: "monitoring_rules",
                sql: "severity IN ('HIGH','MEDIUM','LOW')");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_mr_active_rule",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropCheckConstraint(
                name: "ck_mr_at_least_one_bound",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropCheckConstraint(
                name: "ck_mr_bounds_consistent",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropCheckConstraint(
                name: "ck_mr_severity",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.DropColumn(
                name: "severity",
                schema: "monitoring",
                table: "monitoring_rules");

            migrationBuilder.RenameColumn(
                name: "min_value",
                schema: "monitoring",
                table: "monitoring_rules",
                newName: "min_threshold");

            migrationBuilder.RenameColumn(
                name: "max_value",
                schema: "monitoring",
                table: "monitoring_rules",
                newName: "max_threshold");

            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "monitoring",
                table: "monitoring_rules",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "severity_level",
                schema: "monitoring",
                table: "monitoring_rules",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "i_x_monitoring_rules_older_adult_id_record_type_code",
                schema: "monitoring",
                table: "monitoring_rules",
                columns: new[] { "older_adult_id", "record_type_code" });

            migrationBuilder.AddCheckConstraint(
                name: "ck_rules_min_max",
                schema: "monitoring",
                table: "monitoring_rules",
                sql: "min_threshold IS NULL OR max_threshold IS NULL OR min_threshold <= max_threshold");

            migrationBuilder.AddCheckConstraint(
                name: "ck_rules_threshold",
                schema: "monitoring",
                table: "monitoring_rules",
                sql: "min_threshold IS NOT NULL OR max_threshold IS NOT NULL");
        }
    }
}
