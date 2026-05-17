/*
  Database Setup Script for Nha_Hang_Huit
  .NET Framework 4.8 - WPF App
  SQL Server (LocalDB or Express)
*/

-- Tao Database
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = 'NhaHangHuit')
BEGIN
    CREATE DATABASE NhaHangHuit;
END
GO

USE NhaHangHuit;
GO

-- ============================================
-- Bang tblMonAn: Menu mon an
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblMonAn]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblMonAn] (
        [MaMonAn]       INT IDENTITY(1,1) PRIMARY KEY,
        [TenMonAn]      NVARCHAR(200)   NOT NULL,
        [Gia]           DECIMAL(18,2)   NOT NULL,
        [NhomMon]       NVARCHAR(50)    NOT NULL,   -- Lo Nuong, Mon Nhung, Mon Them, Do Uong
        [HinhAnh]       NVARCHAR(500)   NULL,
        [MoTa]          NVARCHAR(500)   NULL,
        [TrangThai]     NVARCHAR(20)    NOT NULL DEFAULT N'Con',  -- Con / Het
        [NgayTao]       DATETIME2       NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ============================================
-- Bang tblKhachHang: Thong tin khach hang + tich diem
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblKhachHang]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblKhachHang] (
        [MaKhachHang]       INT IDENTITY(1,1) PRIMARY KEY,
        [TenKhachHang]      NVARCHAR(200)   NOT NULL,
        [SoDienThoai]       NVARCHAR(20)    NOT NULL,
        [NgayDangKy]        DATETIME2       NOT NULL DEFAULT GETDATE(),
        [TongDiemTichLuy]   INT             NOT NULL DEFAULT 0,
        [HangThe]           NVARCHAR(50)    NOT NULL DEFAULT N'Thuong'
    );
END
GO

-- ============================================
-- Bang tblHoaDon: Thong tin hoa don
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblHoaDon]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblHoaDon] (
        [MaHoaDon]          INT IDENTITY(1,1) PRIMARY KEY,
        [MaKhachHang]       INT             NULL,
        [NgayTao]           DATETIME2       NOT NULL DEFAULT GETDATE(),
        [TongTien]          DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [TienGiamGia]       DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [ThanhTien]         DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [PhuongThucThanhToan] NVARCHAR(50)  NULL,    -- TienMat, QRCode
        [TrangThai]         NVARCHAR(50)    NOT NULL DEFAULT N'ChoThanhToan',  -- ChoThanhToan, DaThanhToan, DaHuy
        [MaCa]              INT             NULL,
        [GhiChu]            NVARCHAR(500)   NULL,
        FOREIGN KEY ([MaKhachHang]) REFERENCES [dbo].[tblKhachHang]([MaKhachHang])
    );
END
GO

-- ============================================
-- Bang tblChiTietHoaDon: Chi tiet tung mon trong hoa don
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblChiTietHoaDon]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblChiTietHoaDon] (
        [MaChiTiet]     INT IDENTITY(1,1) PRIMARY KEY,
        [MaHoaDon]      INT             NOT NULL,
        [MaMonAn]       INT             NOT NULL,
        [TenMonAn]      NVARCHAR(200)   NOT NULL,
        [SoLuong]       INT             NOT NULL DEFAULT 1,
        [DonGia]        DECIMAL(18,2)   NOT NULL,
        [ThanhTien]     DECIMAL(18,2)   NOT NULL,
        FOREIGN KEY ([MaHoaDon]) REFERENCES [dbo].[tblHoaDon]([MaHoaDon]) ON DELETE CASCADE,
        FOREIGN KEY ([MaMonAn]) REFERENCES [dbo].[tblMonAn]([MaMonAn])
    );
END
GO

-- ============================================
-- Bang tblNhanVien: Nhan vien he thong (phan quyen)
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblNhanVien]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblNhanVien] (
        [MaNhanVien]    INT IDENTITY(1,1) PRIMARY KEY,
        [HoTen]         NVARCHAR(100)   NOT NULL,
        [TaiKhoan]      VARCHAR(50)     NOT NULL,
        [MatKhau]       VARCHAR(100)    NOT NULL,
        [ChucVu]        NVARCHAR(50)    NOT NULL DEFAULT N'Nhan Vien',
        [TrangThai]     BIT             NOT NULL DEFAULT 1
    );
END
GO

IF NOT EXISTS (SELECT * FROM tblNhanVien WHERE TaiKhoan = 'admin')
BEGIN
    INSERT INTO tblNhanVien (HoTen, TaiKhoan, MatKhau, ChucVu) VALUES
    (N'Admin',  'admin',  '123456', N'Quan Ly'),
    (N'Nhan Vien 1', 'nv01', '123456', N'Nhan Vien'),
    (N'Nhan Vien 2', 'nv02', '123456', N'Nhan Vien');
END
GO

-- ============================================
-- Bang tblLichSuCa: Lich su dong ca
-- ============================================
IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[tblLichSuCa]') AND type = 'U')
BEGIN
    CREATE TABLE [dbo].[tblLichSuCa] (
        [MaCa]              INT IDENTITY(1,1) PRIMARY KEY,
        [GioBatDau]         DATETIME2       NOT NULL DEFAULT GETDATE(),
        [GioKetThuc]        DATETIME2       NULL,
        [NhanVien]          NVARCHAR(200)   NOT NULL DEFAULT N'NhanVien',
        [TongSoHoaDon]      INT             NOT NULL DEFAULT 0,
        [TongDoanhThuTruocGiamGia] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [TongTienGiamGia]   DECIMAL(18,2)   NOT NULL DEFAULT 0,
        [TongDoanhThuThucNhan] DECIMAL(18,2) NOT NULL DEFAULT 0,
        [SoKhachMoi]        INT             NOT NULL DEFAULT 0,
        [GhiChu]            NVARCHAR(500)   NULL
    );
END
GO

-- ============================================
-- Stored Procedure: spLayHoaDonTheoCa
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spLayHoaDonTheoCa]') AND type = 'P')
    DROP PROCEDURE [dbo].[spLayHoaDonTheoCa]
GO

CREATE PROCEDURE [dbo].[spLayHoaDonTheoCa]
    @MaCa INT
AS
BEGIN
    SELECT * FROM tblHoaDon WHERE MaCa = @MaCa ORDER BY NgayTao;
END
GO

-- ============================================
-- Stored Procedure: spCapNhatDiemKhachHang
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spCapNhatDiemKhachHang]') AND type = 'P')
    DROP PROCEDURE [dbo].[spCapNhatDiemKhachHang]
GO

CREATE PROCEDURE [dbo].[spCapNhatDiemKhachHang]
    @MaKhachHang INT,
    @TongTien DECIMAL(18,2)
AS
BEGIN
    DECLARE @DiemCong INT;
    SET @DiemCong = FLOOR(@TongTien / 10000);
    
    UPDATE tblKhachHang
    SET TongDiemTichLuy = TongDiemTichLuy + @DiemCong
    WHERE MaKhachHang = @MaKhachHang;
    
    -- Cap nhat hang the
    UPDATE tblKhachHang
    SET HangThe = CASE
        WHEN TongDiemTichLuy >= 1000 THEN N'KimCuong'
        WHEN TongDiemTichLuy >= 500 THEN N'Vang'
        WHEN TongDiemTichLuy >= 100 THEN N'Bac'
        ELSE N'Thuong'
    END
    WHERE MaKhachHang = @MaKhachHang;
END
GO

-- ============================================
-- Stored Procedure: spThongKeTopMonBanChay
-- ============================================
IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[spThongKeTopMonBanChay]') AND type = 'P')
    DROP PROCEDURE [dbo].[spThongKeTopMonBanChay]
GO

CREATE PROCEDURE [dbo].[spThongKeTopMonBanChay]
    @MaCa INT
AS
BEGIN
    SELECT TOP 5
        ctd.TenMonAn,
        SUM(ctd.SoLuong) AS TongSoLuong,
        SUM(ctd.ThanhTien) AS TongDoanhThu
    FROM tblChiTietHoaDon ctd
    INNER JOIN tblHoaDon hd ON ctd.MaHoaDon = hd.MaHoaDon
    WHERE hd.MaCa = @MaCa AND hd.TrangThai = N'DaThanhToan'
    GROUP BY ctd.TenMonAn
    ORDER BY TongSoLuong DESC;
END
GO

-- ============================================
-- Du lieu mau: Them 20 mon an phong cach Haidilao
-- ============================================
IF NOT EXISTS (SELECT 1 FROM tblMonAn)
BEGIN
    -- Nhom Lo Nuong
    INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    (N'Lau Bo Nhung Giat', 299000, N'Lo Nuong', N'Lau bo nhung gat Haidilao chinh goc', N'Con'),
    (N'Lau Cay Szechuan', 259000, N'Lo Nuong', N'Lau cay phong cach Tu Xuyen', N'Con'),
    (N'Lau Hai San Tong Hop', 399000, N'Lo Nuong', N'Lau hai san tom, muc, ngao, ca', N'Con'),
    (N'Lau Nam Huong', 239000, N'Lo Nuong', N'Lau nam huong thanh mat', N'Con'),
    (N'Lau Ca Chua', 219000, N'Lo Nuong', N'Lau ca chua chua ngot', N'Con');

    -- Nhom Mon Nhung
    INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    (N'Thit Bo My Nhung Giat', 199000, N'Mon Nhung', N'Thit bo My nhung gat tuoi ngon', N'Con'),
    (N'Thit Heo Nhung Giat', 149000, N'Mon Nhung', N'Thit heo nhung gat thom ngon', N'Con'),
    (N'Tom Su Nhung Giat', 179000, N'Mon Nhung', N'Tom su tuoi nhung gat', N'Con'),
    (N'Muc Nhung Giat', 169000, N'Mon Nhung', N'Muc tuoi nhung gat gion ngot', N'Con'),
    (N'Rau Cu Tong Hop', 89000, N'Mon Nhung', N'Rau cu cac loai nhung lau', N'Con'),
    (N'Vien Ca Hoi', 99000, N'Mon Nhung', N'Vien ca hoi tuoi ngon', N'Con');

    -- Nhom Mon Them
    INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    (N'Com Trang', 10000, N'Mon Them', N'Com trang nau mem', N'Con'),
    (N'Mi Udon', 35000, N'Mon Them', N'Mi Udon Nhat Ban', N'Con'),
    (N'Mi Trung', 25000, N'Mon Them', N'Mi trung tuoi', N'Con'),
    (N'Banh Trang Cuon', 29000, N'Mon Them', N'Banh trang cuon thit luon', N'Con'),
    (N'Khoai Tay Chien', 49000, N'Mon Them', N'Khoai tay chien gion', N'Con');

    -- Nhom Do Uong
    INSERT INTO tblMonAn (TenMonAn, Gia, NhomMon, MoTa, TrangThai) VALUES
    (N'Coca Cola', 15000, N'Do Uong', N'Coca Cola lon 330ml', N'Con'),
    (N'Nuoc Ep Cam', 35000, N'Do Uong', N'Nuoc ep cam tuoi nguyen chat', N'Con'),
    (N'Tra Dao', 29000, N'Do Uong', N'Tra dao house made', N'Con'),
    (N'Bia Tiger', 25000, N'Do Uong', N'Bia Tiger lon 330ml', N'Con'),
    (N'Nuoc Suoi', 10000, N'Do Uong', N'Nuoc suoi dong chai 500ml', N'Con');
END
GO
