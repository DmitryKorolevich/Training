using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Migrations.Builders;
using Microsoft.Data.Entity.Migrations.Model;
using System;

namespace VitalChoice_vNext.Migrations
{
    public partial class FirstMigration : Migration
    {
        public override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("AspNetRoleClaims", "FK_AspNetRoleClaims_AspNetRoles_RoleId");
            
            migrationBuilder.DropForeignKey("AspNetUserClaims", "FK_AspNetUserClaims_AspNetUsers_UserId");
            
            migrationBuilder.DropForeignKey("AspNetUserLogins", "FK_AspNetUserLogins_AspNetUsers_UserId");
            
            migrationBuilder.DropPrimaryKey("AspNetRoles", "PK_AspNetRoles");
            
            migrationBuilder.DropPrimaryKey("AspNetUsers", "PK_AspNetUsers");
            
            migrationBuilder.AlterColumn("AspNetRoles", "Id", c => c.String());
            
            migrationBuilder.AlterColumn("AspNetUsers", "Id", c => c.String());
            
            migrationBuilder.AlterColumn("AspNetUsers", "LockoutEnd", c => c.DateTimeOffset());
            
            migrationBuilder.AddColumn("AspNetUsers", "CustomerId", c => c.Int(nullable: false));
            
            migrationBuilder.AddPrimaryKey("AspNetRoles", "PK_AspNetRoles", new[] { "Id" }, isClustered: true);
            
            migrationBuilder.AddPrimaryKey("AspNetUsers", "PK_AspNetUsers", new[] { "Id" }, isClustered: true);
            
            migrationBuilder.AddForeignKey(
                "AspNetRoleClaims",
                "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                new[] { "RoleId" },
                "AspNetRoles",
                new[] { "Id" },
                cascadeDelete: false);
            
            migrationBuilder.AddForeignKey(
                "AspNetUserClaims",
                "FK_AspNetUserClaims_AspNetUsers_UserId",
                new[] { "UserId" },
                "AspNetUsers",
                new[] { "Id" },
                cascadeDelete: false);
            
            migrationBuilder.AddForeignKey(
                "AspNetUserLogins",
                "FK_AspNetUserLogins_AspNetUsers_UserId",
                new[] { "UserId" },
                "AspNetUsers",
                new[] { "Id" },
                cascadeDelete: false);
        }
        
        public override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey("AspNetRoleClaims", "FK_AspNetRoleClaims_AspNetRoles_RoleId");
            
            migrationBuilder.DropForeignKey("AspNetUserClaims", "FK_AspNetUserClaims_AspNetUsers_UserId");
            
            migrationBuilder.DropForeignKey("AspNetUserLogins", "FK_AspNetUserLogins_AspNetUsers_UserId");
            
            migrationBuilder.DropPrimaryKey("AspNetRoles", "PK_AspNetRoles");
            
            migrationBuilder.DropPrimaryKey("AspNetUsers", "PK_AspNetUsers");
            
            migrationBuilder.DropColumn("AspNetUsers", "CustomerId");
            
            migrationBuilder.AlterColumn("AspNetRoles", "Id", c => c.String());
            
            migrationBuilder.AlterColumn("AspNetUsers", "Id", c => c.String());
            
            migrationBuilder.AlterColumn("AspNetUsers", "LockoutEnd", c => c.DateTimeOffset(nullable: false));
            
            migrationBuilder.AddPrimaryKey("AspNetRoles", "PK_AspNetRoles", new[] { "Id" }, isClustered: true);
            
            migrationBuilder.AddPrimaryKey("AspNetUsers", "PK_AspNetUsers", new[] { "Id" }, isClustered: true);
            
            migrationBuilder.AddForeignKey(
                "AspNetRoleClaims",
                "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                new[] { "RoleId" },
                "AspNetRoles",
                new[] { "Id" },
                cascadeDelete: false);
            
            migrationBuilder.AddForeignKey(
                "AspNetUserClaims",
                "FK_AspNetUserClaims_AspNetUsers_UserId",
                new[] { "UserId" },
                "AspNetUsers",
                new[] { "Id" },
                cascadeDelete: false);
            
            migrationBuilder.AddForeignKey(
                "AspNetUserLogins",
                "FK_AspNetUserLogins_AspNetUsers_UserId",
                new[] { "UserId" },
                "AspNetUsers",
                new[] { "Id" },
                cascadeDelete: false);
        }
    }
}