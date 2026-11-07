using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VitaLink.Platform.API.Shared.Infrastructure.Persistence.EntityFrameworkCore.Migrations
{
    /// <inheritdoc />
    public partial class AddElderCareForeignKeys : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // CareProvider -> IAM User
            migrationBuilder.AddForeignKey(
                name: "fk_care_providers_users_user_id",
                schema: "elder_care",
                table: "care_providers",
                column: "user_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // OlderAdult -> IAM User
            migrationBuilder.AddForeignKey(
                name: "fk_older_adults_users_user_id",
                schema: "elder_care",
                table: "older_adults",
                column: "user_id",
                principalSchema: "iam",
                principalTable: "users",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            // OlderAdult -> CareProvider
            migrationBuilder.AddForeignKey(
                name: "fk_older_adults_care_providers_assigned_provider_id",
                schema: "elder_care",
                table: "older_adults",
                column: "assigned_provider_id",
                principalSchema: "elder_care",
                principalTable: "care_providers",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_older_adults_care_providers_assigned_provider_id",
                schema: "elder_care",
                table: "older_adults");

            migrationBuilder.DropForeignKey(
                name: "fk_older_adults_users_user_id",
                schema: "elder_care",
                table: "older_adults");

            migrationBuilder.DropForeignKey(
                name: "fk_care_providers_users_user_id",
                schema: "elder_care",
                table: "care_providers");
        }
    }
}
