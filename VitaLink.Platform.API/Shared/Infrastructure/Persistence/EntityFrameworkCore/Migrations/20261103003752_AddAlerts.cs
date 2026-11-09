using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddAlerts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "alerting");

            migrationBuilder.CreateTable(
                name: "alerts",
                schema: "alerting",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    older_adult_id = table.Column<Guid>(type: "uuid", nullable: false),
                    health_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    monitoring_rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assigned_provider_id = table.Column<Guid>(type: "uuid", nullable: true),
                    severity = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    raised_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_status_change_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alerts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "alert_status_changes",
                schema: "alerting",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    to_status = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    changed_by = table.Column<Guid>(type: "uuid", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    alert_id = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("p_k_alert_status_changes", x => x.id);
                    table.ForeignKey(
                        name: "f_k_alert_status_changes_alerts_alert_id",
                        column: x => x.alert_id,
                        principalSchema: "alerting",
                        principalTable: "alerts",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "i_x_alert_status_changes_alert_id",
                schema: "alerting",
                table: "alert_status_changes",
                column: "alert_id");

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_assigned_provider_id_status_severity",
                schema: "alerting",
                table: "alerts",
                columns: new[] { "assigned_provider_id", "status", "severity" });

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_health_record_id",
                schema: "alerting",
                table: "alerts",
                column: "health_record_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "i_x_alerts_older_adult_id_raised_at",
                schema: "alerting",
                table: "alerts",
                columns: new[] { "older_adult_id", "raised_at" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "alert_status_changes",
                schema: "alerting");

            migrationBuilder.DropTable(
                name: "alerts",
                schema: "alerting");
        }
    }
}
