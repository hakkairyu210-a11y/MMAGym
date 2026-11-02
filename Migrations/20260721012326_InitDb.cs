using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MMA.Migrations
{
    /// <inheritdoc />
    public partial class InitDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BoMon",
                columns: table => new
                {
                    MaBoMon = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenBoMon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BoMon", x => x.MaBoMon);
                });

            migrationBuilder.CreateTable(
                name: "ChiNhanh",
                columns: table => new
                {
                    MaCN = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenCN = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DiaChi = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Hotline = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    HinhAnh = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChiNhanh", x => x.MaCN);
                });

            migrationBuilder.CreateTable(
                name: "LichTap",
                columns: table => new
                {
                    MaLich = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaCN = table.Column<int>(type: "int", nullable: false),
                    ThuTrongTuan = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    KhungGio = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TenLop = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TenHLV = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ClassType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ColorClass = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ThoiGianChiTiet = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LichTap", x => x.MaLich);
                });

            migrationBuilder.CreateTable(
                name: "PhuongThucThanhToan",
                columns: table => new
                {
                    MaPT = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenPhuongThuc = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhuongThucThanhToan", x => x.MaPT);
                });

            migrationBuilder.CreateTable(
                name: "VaiTro",
                columns: table => new
                {
                    MaVaiTro = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenVaiTro = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VaiTro", x => x.MaVaiTro);
                });

            migrationBuilder.CreateTable(
                name: "GoiTap",
                columns: table => new
                {
                    MaGoi = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TenGoi = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MaBoMon = table.Column<int>(type: "int", nullable: false),
                    ThoiHanThang = table.Column<int>(type: "int", nullable: false),
                    GiaTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoTa = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GoiTap", x => x.MaGoi);
                    table.ForeignKey(
                        name: "FK_GoiTap_BoMon_MaBoMon",
                        column: x => x.MaBoMon,
                        principalTable: "BoMon",
                        principalColumn: "MaBoMon",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "NguoiDung",
                columns: table => new
                {
                    MaND = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    HoTen = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SoDT = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    MaVaiTro = table.Column<int>(type: "int", nullable: false),
                    MaCN_TrucThuoc = table.Column<int>(type: "int", nullable: true),
                    NgayTao = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrangThai = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NguoiDung", x => x.MaND);
                    table.ForeignKey(
                        name: "FK_NguoiDung_ChiNhanh_MaCN_TrucThuoc",
                        column: x => x.MaCN_TrucThuoc,
                        principalTable: "ChiNhanh",
                        principalColumn: "MaCN");
                    table.ForeignKey(
                        name: "FK_NguoiDung_VaiTro_MaVaiTro",
                        column: x => x.MaVaiTro,
                        principalTable: "VaiTro",
                        principalColumn: "MaVaiTro",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DangKyGoi",
                columns: table => new
                {
                    MaDK = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaHD = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    MaND = table.Column<int>(type: "int", nullable: false),
                    MaCN = table.Column<int>(type: "int", nullable: false),
                    MaGoi = table.Column<int>(type: "int", nullable: false),
                    NgayDK = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayBatDau = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NgayKetThuc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TongTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TrangThai = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DangKyGoi", x => x.MaDK);
                    table.ForeignKey(
                        name: "FK_DangKyGoi_ChiNhanh_MaCN",
                        column: x => x.MaCN,
                        principalTable: "ChiNhanh",
                        principalColumn: "MaCN",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyGoi_GoiTap_MaGoi",
                        column: x => x.MaGoi,
                        principalTable: "GoiTap",
                        principalColumn: "MaGoi",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DangKyGoi_NguoiDung_MaND",
                        column: x => x.MaND,
                        principalTable: "NguoiDung",
                        principalColumn: "MaND",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GiaoDichThanhToan",
                columns: table => new
                {
                    MaGD = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MaDK = table.Column<int>(type: "int", nullable: false),
                    MaPT_ThanhToan = table.Column<int>(type: "int", nullable: false),
                    SoTien = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ThoiGianThanhToan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GiaoDichThanhToan", x => x.MaGD);
                    table.ForeignKey(
                        name: "FK_GiaoDichThanhToan_DangKyGoi_MaDK",
                        column: x => x.MaDK,
                        principalTable: "DangKyGoi",
                        principalColumn: "MaDK",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GiaoDichThanhToan_PhuongThucThanhToan_MaPT_ThanhToan",
                        column: x => x.MaPT_ThanhToan,
                        principalTable: "PhuongThucThanhToan",
                        principalColumn: "MaPT",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoi_MaCN",
                table: "DangKyGoi",
                column: "MaCN");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoi_MaGoi",
                table: "DangKyGoi",
                column: "MaGoi");

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoi_MaHD",
                table: "DangKyGoi",
                column: "MaHD",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DangKyGoi_MaND",
                table: "DangKyGoi",
                column: "MaND");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToan_MaDK",
                table: "GiaoDichThanhToan",
                column: "MaDK");

            migrationBuilder.CreateIndex(
                name: "IX_GiaoDichThanhToan_MaPT_ThanhToan",
                table: "GiaoDichThanhToan",
                column: "MaPT_ThanhToan");

            migrationBuilder.CreateIndex(
                name: "IX_GoiTap_MaBoMon",
                table: "GoiTap",
                column: "MaBoMon");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_Email",
                table: "NguoiDung",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaCN_TrucThuoc",
                table: "NguoiDung",
                column: "MaCN_TrucThuoc");

            migrationBuilder.CreateIndex(
                name: "IX_NguoiDung_MaVaiTro",
                table: "NguoiDung",
                column: "MaVaiTro");

            migrationBuilder.CreateIndex(
                name: "IX_PhuongThucThanhToan_TenPhuongThuc",
                table: "PhuongThucThanhToan",
                column: "TenPhuongThuc",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VaiTro_TenVaiTro",
                table: "VaiTro",
                column: "TenVaiTro",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GiaoDichThanhToan");

            migrationBuilder.DropTable(
                name: "LichTap");

            migrationBuilder.DropTable(
                name: "DangKyGoi");

            migrationBuilder.DropTable(
                name: "PhuongThucThanhToan");

            migrationBuilder.DropTable(
                name: "GoiTap");

            migrationBuilder.DropTable(
                name: "NguoiDung");

            migrationBuilder.DropTable(
                name: "BoMon");

            migrationBuilder.DropTable(
                name: "ChiNhanh");

            migrationBuilder.DropTable(
                name: "VaiTro");
        }
    }
}
