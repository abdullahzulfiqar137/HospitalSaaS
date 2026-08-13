using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalSaaS.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class CompleteHospitalModules : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "RadiologyTestId",
                table: "RadiologyOrders",
                type: "uniqueidentifier",
                nullable: true
            );

            migrationBuilder.AddColumn<Guid>(
                name: "LabTestId",
                table: "LabOrderItems",
                type: "uniqueidentifier",
                nullable: true
            );

            migrationBuilder.CreateTable(
                name: "AdmissionHistories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AdmissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromWardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FromBedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToWardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToRoomId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToBedId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(
                        type: "nvarchar(500)",
                        maxLength: 500,
                        nullable: true
                    ),
                    TransferredAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdmissionHistories", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdmissionHistories_Admissions_AdmissionId",
                        column: x => x.AdmissionId,
                        principalTable: "Admissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LabResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LabOrderItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<string>(
                        type: "nvarchar(4000)",
                        maxLength: 4000,
                        nullable: false
                    ),
                    Unit = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    ReferenceRange = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    Version = table.Column<int>(type: "int", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LabResults_LabOrderItems_LabOrderItemId",
                        column: x => x.LabOrderItemId,
                        principalTable: "LabOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_LabResults_LabOrders_LabOrderId",
                        column: x => x.LabOrderId,
                        principalTable: "LabOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "LabTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Code = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Category = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Price = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    NormalRange = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: true
                    ),
                    Unit = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: true
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTests", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "PurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderNumber = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    SupplierId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpectedDeliveryDate = table.Column<DateOnly>(type: "date", nullable: true),
                    Notes = table.Column<string>(
                        type: "nvarchar(2000)",
                        maxLength: 2000,
                        nullable: true
                    ),
                    Status = table.Column<int>(type: "int", nullable: false),
                    ReceivedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrders_Suppliers_SupplierId",
                        column: x => x.SupplierId,
                        principalTable: "Suppliers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "RadiologyResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RadiologyOrderId = table.Column<Guid>(
                        type: "uniqueidentifier",
                        nullable: false
                    ),
                    Report = table.Column<string>(
                        type: "nvarchar(max)",
                        maxLength: 16000,
                        nullable: false
                    ),
                    Version = table.Column<int>(type: "int", nullable: false),
                    RecordedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RadiologyResults_RadiologyOrders_RadiologyOrderId",
                        column: x => x.RadiologyOrderId,
                        principalTable: "RadiologyOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "RadiologyTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Code = table.Column<string>(
                        type: "nvarchar(50)",
                        maxLength: 50,
                        nullable: false
                    ),
                    Name = table.Column<string>(
                        type: "nvarchar(200)",
                        maxLength: 200,
                        nullable: false
                    ),
                    Modality = table.Column<string>(
                        type: "nvarchar(100)",
                        maxLength: 100,
                        nullable: false
                    ),
                    Price = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    UpdatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DeletedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RadiologyTests", x => x.Id);
                }
            );

            migrationBuilder.CreateTable(
                name: "PurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PurchaseOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InventoryItemId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitCost = table.Column<decimal>(
                        type: "decimal(18,2)",
                        precision: 18,
                        scale: 2,
                        nullable: false
                    ),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_InventoryItems_InventoryItemId",
                        column: x => x.InventoryItemId,
                        principalTable: "InventoryItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_PurchaseOrderItems_PurchaseOrders_PurchaseOrderId",
                        column: x => x.PurchaseOrderId,
                        principalTable: "PurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyOrders_RadiologyTestId",
                table: "RadiologyOrders",
                column: "RadiologyTestId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabOrderItems_LabTestId",
                table: "LabOrderItems",
                column: "LabTestId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Diagnoses_MedicalRecordId",
                table: "Diagnoses",
                column: "MedicalRecordId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionHistories_AdmissionId",
                table: "AdmissionHistories",
                column: "AdmissionId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_AdmissionHistories_TenantId_AdmissionId_TransferredAtUtc",
                table: "AdmissionHistories",
                columns: new[] { "TenantId", "AdmissionId", "TransferredAtUtc" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabOrderId",
                table: "LabResults",
                column: "LabOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_LabOrderItemId",
                table: "LabResults",
                column: "LabOrderItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabResults_TenantId_LabOrderItemId_Version",
                table: "LabResults",
                columns: new[] { "TenantId", "LabOrderItemId", "Version" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_LabTests_TenantId_Code",
                table: "LabTests",
                columns: new[] { "TenantId", "Code" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_InventoryItemId",
                table: "PurchaseOrderItems",
                column: "InventoryItemId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrderItems_PurchaseOrderId",
                table: "PurchaseOrderItems",
                column: "PurchaseOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_SupplierId",
                table: "PurchaseOrders",
                column: "SupplierId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseOrders_TenantId_OrderNumber",
                table: "PurchaseOrders",
                columns: new[] { "TenantId", "OrderNumber" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_RadiologyOrderId",
                table: "RadiologyResults",
                column: "RadiologyOrderId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyResults_TenantId_RadiologyOrderId_Version",
                table: "RadiologyResults",
                columns: new[] { "TenantId", "RadiologyOrderId", "Version" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_RadiologyTests_TenantId_Code",
                table: "RadiologyTests",
                columns: new[] { "TenantId", "Code" },
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_Diagnoses_MedicalRecords_MedicalRecordId",
                table: "Diagnoses",
                column: "MedicalRecordId",
                principalTable: "MedicalRecords",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade
            );

            migrationBuilder.AddForeignKey(
                name: "FK_LabOrderItems_LabTests_LabTestId",
                table: "LabOrderItems",
                column: "LabTestId",
                principalTable: "LabTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );

            migrationBuilder.AddForeignKey(
                name: "FK_RadiologyOrders_RadiologyTests_RadiologyTestId",
                table: "RadiologyOrders",
                column: "RadiologyTestId",
                principalTable: "RadiologyTests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Diagnoses_MedicalRecords_MedicalRecordId",
                table: "Diagnoses"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_LabOrderItems_LabTests_LabTestId",
                table: "LabOrderItems"
            );

            migrationBuilder.DropForeignKey(
                name: "FK_RadiologyOrders_RadiologyTests_RadiologyTestId",
                table: "RadiologyOrders"
            );

            migrationBuilder.DropTable(name: "AdmissionHistories");

            migrationBuilder.DropTable(name: "LabResults");

            migrationBuilder.DropTable(name: "LabTests");

            migrationBuilder.DropTable(name: "PurchaseOrderItems");

            migrationBuilder.DropTable(name: "RadiologyResults");

            migrationBuilder.DropTable(name: "RadiologyTests");

            migrationBuilder.DropTable(name: "PurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_RadiologyOrders_RadiologyTestId",
                table: "RadiologyOrders"
            );

            migrationBuilder.DropIndex(name: "IX_LabOrderItems_LabTestId", table: "LabOrderItems");

            migrationBuilder.DropIndex(name: "IX_Diagnoses_MedicalRecordId", table: "Diagnoses");

            migrationBuilder.DropColumn(name: "RadiologyTestId", table: "RadiologyOrders");

            migrationBuilder.DropColumn(name: "LabTestId", table: "LabOrderItems");
        }
    }
}
