using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UndeFacemRevelionul.Migrations
{
    public partial class partypartiers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyPartierModel_Partiers_PartierId",
                table: "PartyPartierModel");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyPartierModel_Parties_PartyId",
                table: "PartyPartierModel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyPartierModel",
                table: "PartyPartierModel");

            migrationBuilder.RenameTable(
                name: "PartyPartierModel",
                newName: "PartyPartiers");

            migrationBuilder.RenameIndex(
                name: "IX_PartyPartierModel_PartierId",
                table: "PartyPartiers",
                newName: "IX_PartyPartiers_PartierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyPartiers",
                table: "PartyPartiers",
                columns: new[] { "PartyId", "PartierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPartiers_Partiers_PartierId",
                table: "PartyPartiers",
                column: "PartierId",
                principalTable: "Partiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPartiers_Parties_PartyId",
                table: "PartyPartiers",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyPartiers_Partiers_PartierId",
                table: "PartyPartiers");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyPartiers_Parties_PartyId",
                table: "PartyPartiers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PartyPartiers",
                table: "PartyPartiers");

            migrationBuilder.RenameTable(
                name: "PartyPartiers",
                newName: "PartyPartierModel");

            migrationBuilder.RenameIndex(
                name: "IX_PartyPartiers_PartierId",
                table: "PartyPartierModel",
                newName: "IX_PartyPartierModel_PartierId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PartyPartierModel",
                table: "PartyPartierModel",
                columns: new[] { "PartyId", "PartierId" });

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPartierModel_Partiers_PartierId",
                table: "PartyPartierModel",
                column: "PartierId",
                principalTable: "Partiers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PartyPartierModel_Parties_PartyId",
                table: "PartyPartierModel",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id");
        }
    }
}
