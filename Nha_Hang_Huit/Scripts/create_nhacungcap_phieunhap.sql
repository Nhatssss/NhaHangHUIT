-- ============================================================
-- Script: create_nhacungcap_phieunhap.sql
-- Them bang Nha Cung Cap & Phieu Nhap vao DB NhaHang_HaiDiLao_Mini
-- ============================================================
USE [NhaHang_HaiDiLao_Mini];
GO

-- ==================== BANG NHÀ CUNG CẤP ====================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblNhaCungCap')
BEGIN
    CREATE TABLE tblNhaCungCap (
        MaNhaCungCap INT IDENTITY(1,1) PRIMARY KEY,
        TenNhaCungCap NVARCHAR(200) NOT NULL,
        DiaChi NVARCHAR(500) NULL,
        SoDienThoai NVARCHAR(20) NULL,
        Email NVARCHAR(100) NULL,
        GhiChu NVARCHAR(500) NULL
    );
    PRINT N'Da tao bang tblNhaCungCap';
END
GO

-- ==================== BANG PHIẾU NHẬP ====================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblPhieuNhap')
BEGIN
    CREATE TABLE tblPhieuNhap (
        MaPhieuNhap INT IDENTITY(1,1) PRIMARY KEY,
        MaNhaCungCap INT NOT NULL,
        NgayNhap DATETIME NOT NULL DEFAULT GETDATE(),
        NguoiGiao NVARCHAR(100) NULL,
        NguoiNhan NVARCHAR(100) NULL,
        GhiChu NVARCHAR(500) NULL,
        TongTien DECIMAL(18,2) NULL DEFAULT 0,
		CONSTRAINT FK_PhieuNhap_NhaCungCap FOREIGN KEY (MaNhaCungCap) REFERENCES tblNhaCungCap(MaNhaCungCap)
    );
    PRINT N'Da tao bang tblPhieuNhap';
END
GO

-- ==================== BANG CHI TIẾT PHIẾU NHẬP ====================
IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'tblChiTietPhieuNhap')
BEGIN
    CREATE TABLE tblChiTietPhieuNhap (
        MaChiTiet INT IDENTITY(1,1) PRIMARY KEY,
        MaPhieuNhap INT NOT NULL,
        TenNguyenLieu NVARCHAR(200) NOT NULL,
        DonViTinh NVARCHAR(30) NULL DEFAULT N'Kg',
        SoLuong DECIMAL(18,2) NOT NULL DEFAULT 0,
        DonGia DECIMAL(18,2) NOT NULL DEFAULT 0,
        ThanhTien DECIMAL(18,2) NULL DEFAULT 0,
		CONSTRAINT FK_ChiTietPhieuNhap_PhieuNhap FOREIGN KEY (MaPhieuNhap) REFERENCES tblPhieuNhap(MaPhieuNhap)
    );
    PRINT N'Da tao bang tblChiTietPhieuNhap';
END
GO

-- ==================== DỮ LIỆU MẪU - NHÀ CUNG CẤP ====================
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap)
BEGIN
    INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu)
    VALUES
        (N'Thực Phẩm Xanh', N'123 Nguyễn Huệ, Q.1, TP.HCM', N'0901234567', N'info@thucphamxanh.com', N'Chuyên cung cấp rau củ quả sạch'),
        (N'Thủy Hải Sản Biển Đông', N'456 Võ Văn Kiệt, Q.5, TP.HCM', N'0902345678', N'info@bien-dong.vn', N'Hải sản tươi sống các loại'),
        (N'Thịt Sạch An Phát', N'789 Lê Lợi, Q.Bình Thạnh, TP.HCM', N'0903456789', N'info@anphat.vn', N'Thịt heo, bò, gà sạch'),
        (N'Gia Vị Việt', N'321 Trần Hưng Đạo, Q.10, TP.HCM', N'0904567890', N'info@giaviviet.vn', N'Gia vị, nước chấm, nguyên liệu khô'),
        (N'Nước Giải Khát Sài Gòn', N'654 Hoàng Sa, Q.Tân Bình, TP.HCM', N'0905678901', N'info@nuocgkhaisg.vn', N'Nước ngọt, bia, nước suối');
    PRINT N'Da them 5 nha cung cap mau';
END
GO

-- ==================== DỮ LIỆU MẪU - PHIẾU NHẬP (biến SCOPE_IDENTITY) ====================
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap)
BEGIN
    DECLARE @pn1 INT, @pn2 INT, @pn3 INT, @pn4 INT, @pn5 INT;

    -- Phiếu nhập 1 - Thực Phẩm Xanh (MaNhaCungCap = 1)
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, GhiChu)
    VALUES (1, '2026-06-01 08:30:00', N'Nguyễn Văn An', N'Trần Thị Bình', N'Nhập rau củ tuần 1');
    SET @pn1 = SCOPE_IDENTITY();
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn1, N'Rau muống', N'Kg', 20, 15000, 300000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn1, N'Xà lách', N'Kg', 15, 20000, 300000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn1, N'Cà chua', N'Kg', 10, 25000, 250000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn1, N'Hành tím', N'Kg', 5, 35000, 175000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn1, N'Tỏi', N'Kg', 3, 40000, 120000);
    UPDATE tblPhieuNhap SET TongTien = 1145000 WHERE MaPhieuNhap = @pn1;

    -- Phiếu nhập 2 - Thủy Hải Sản Biển Đông (MaNhaCungCap = 2)
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, GhiChu)
    VALUES (2, '2026-06-01 09:00:00', N'Phạm Văn Cường', N'Lê Thị Dung', N'Nhập hải sản tươi');
    SET @pn2 = SCOPE_IDENTITY();
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn2, N'Tôm sú', N'Kg', 10, 180000, 1800000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn2, N'Mực ống', N'Kg', 8, 150000, 1200000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn2, N'Cá hồi', N'Kg', 5, 220000, 1100000);
    UPDATE tblPhieuNhap SET TongTien = 4100000 WHERE MaPhieuNhap = @pn2;

    -- Phiếu nhập 3 - Thịt Sạch An Phát (MaNhaCungCap = 3)
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, GhiChu)
    VALUES (3, '2026-06-01 10:00:00', N'Hoàng Văn Em', N'Nguyễn Thị Phương', N'Nhập thịt tươi');
    SET @pn3 = SCOPE_IDENTITY();
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn3, N'Thịt heo ba chỉ', N'Kg', 15, 80000, 1200000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn3, N'Thịt bò', N'Kg', 10, 150000, 1500000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn3, N'Thịt gà', N'Kg', 12, 70000, 840000);
    UPDATE tblPhieuNhap SET TongTien = 3540000 WHERE MaPhieuNhap = @pn3;

    -- Phiếu nhập 4 - Gia Vị Việt (MaNhaCungCap = 4)
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, GhiChu)
    VALUES (4, '2026-05-31 14:00:00', N'Mai Văn Gia', N'Trần Văn Hoàng', N'Nhập gia vị tồn kho');
    SET @pn4 = SCOPE_IDENTITY();
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn4, N'Nước mắm', N'Lít', 20, 30000, 600000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn4, N'Dầu ăn', N'Lít', 15, 35000, 525000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn4, N'Hạt nêm', N'Kg', 10, 25000, 250000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn4, N'Bột ngọt', N'Kg', 5, 20000, 100000);
    UPDATE tblPhieuNhap SET TongTien = 1475000 WHERE MaPhieuNhap = @pn4;

    -- Phiếu nhập 5 - Nước Giải Khát Sài Gòn (MaNhaCungCap = 5)
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, GhiChu)
    VALUES (5, '2026-05-30 09:30:00', N'Nguyễn Văn In', N'Phạm Thị Khánh', N'Nhập nước giải khát');
    SET @pn5 = SCOPE_IDENTITY();
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn5, N'Coca Cola', N'Thùng', 20, 120000, 2400000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn5, N'Nước suối', N'Thùng', 15, 80000, 1200000);
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia, ThanhTien) VALUES (@pn5, N'Bia Tiger', N'Thùng', 10, 180000, 1800000);
    UPDATE tblPhieuNhap SET TongTien = 5400000 WHERE MaPhieuNhap = @pn5;

    PRINT N'Da them 5 phieu nhap mau';
END
GO

-- Verify
SELECT N'VERIFY - NhaCungCap' AS CheckPoint, COUNT(*) AS SoLuong FROM tblNhaCungCap
UNION ALL
SELECT N'PhieuNhap', COUNT(*) FROM tblPhieuNhap
UNION ALL
SELECT N'ChiTietPhieuNhap', COUNT(*) FROM tblChiTietPhieuNhap
GO
