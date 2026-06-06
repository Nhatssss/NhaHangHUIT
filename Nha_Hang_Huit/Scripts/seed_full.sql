USE NhaHang_HaiDiLao_Mini
GO

-- Kiem tra data hien tai
SELECT 'Before:', COUNT(*) AS NCC FROM tblNhaCungCap
SELECT COUNT(*) AS PN FROM tblPhieuNhap
SELECT COUNT(*) AS CT FROM tblChiTietPhieuNhap
GO

-- Them NCC mo (chi them khi can)
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap WHERE MaNhaCungCap = 6)
INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu) VALUES
(N'Cong ty TNHH Sua & San Pham Sua', N'555 Tran Hung Dao, Q.1, TP.HCM', N'0967890123', N'order@suaviet.vn', N'Sua, kem, bo, pho mai')
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap WHERE MaNhaCungCap = 7)
INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu) VALUES
(N'DN Nuoc Giai Khat Dong A', N'777 Ly Thuong Kiet, Q.10, TP.HCM', N'0978901234', N'sale@nuocdk.vn', N'Nuoc ngot, nuoc suoi')
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap WHERE MaNhaCungCap = 8)
INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu) VALUES
(N'Cong ty TNHH Dong Lanh Nam Hai', N'999 Xa Lo Ha Noi, Q.9, TP.HCM', N'0989012345', N'info@donglanh.vn', N'Thuc pham dong lanh')
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap WHERE MaNhaCungCap = 9)
INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu) VALUES
(N'Cong ty CP Ruou Viet', N'111 Ly Chinh Thang, Q.Phu Nhuan, TP.HCM', N'0990123456', N'order@ruouviet.vn', N'Ruou vang, ruou manh')
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap WHERE MaNhaCungCap = 10)
INSERT INTO tblNhaCungCap (TenNhaCungCap, DiaChi, SoDienThoai, Email, GhiChu) VALUES
(N'Cong ty TNHH Dau An - Bot Ngot', N'222 Nguyen Thi Minh Khai, Q.1, TP.HCM', N'0901122334', N'info@daubuot.vn', N'Dau an, bot ngot, hat nem')
GO

-- Them phieu nhap moi (ma tu 6 tro di)
DECLARE @today DATE = CAST(GETDATE() AS DATE);
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 6)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (1, DATEADD(DAY, -15, @today), N'Nguyen Van An', N'Tran Thi Binh', 12500000, N'Phieu nhap rau cu tuan 1')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 7)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (2, DATEADD(DAY, -14, @today), N'Pham Van Cuong', N'Le Thi Dung', 18500000, N'Phieu nhap bia')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 8)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (3, DATEADD(DAY, -12, @today), N'Le Van Em', N'Nguyen Thi Phuong', 24500000, N'Hai san tuoi song')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 9)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (4, DATEADD(DAY, -10, @today), N'Hoang Van Giang', N'Pham Thi Hoa', 9800000, N'Thit ga, thit heo')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 10)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (5, DATEADD(DAY, -8, @today), N'Vu Van Hung', N'Tran Thi Hue', 5200000, N'Gia vi nhap bo sung')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 11)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (1, DATEADD(DAY, -7, @today), N'Nguyen Van An', N'Le Thi Dung', 8200000, N'Rau cu tuan 2')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 12)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (6, DATEADD(DAY, -6, @today), N'Tran Van Khai', N'Nguyen Thi Lan', 15400000, N'Sua, kem, bo')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 13)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (7, DATEADD(DAY, -5, @today), N'Dang Van Long', N'Pham Thi Hoa', 6700000, N'Nuoc ngot, nuoc suoi')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 14)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (8, DATEADD(DAY, -3, @today), N'Ngo Van Manh', N'Tran Thi Binh', 19300000, N'Dong lanh')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 15)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (9, DATEADD(DAY, -2, @today), N'Ly Van Nam', N'Nguyen Thi Phuong', 21000000, N'Ruou')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 16)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (10, DATEADD(DAY, -1, @today), N'Phan Van Oanh', N'Le Thi Dung', 4300000, N'Dau an, hat nem')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 17)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (3, @today, N'Le Van Em', N'Tran Thi Hue', 28600000, N'Hai san tuoi')
END
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap = 18)
BEGIN
    INSERT INTO tblPhieuNhap (MaNhaCungCap, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu) VALUES
    (2, @today, N'Pham Van Cuong', N'Nguyen Thi Lan', 15900000, N'Bia bo sung cuoi tuan')
END
GO

-- Chi tiet phieu 6 (rau cu)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 6)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (6, N'Xa lach', 'kg', 20, 35000),
    (6, N'Ca chua', 'kg', 30, 25000),
    (6, N'Hanh tay', 'kg', 25, 42000),
    (6, N'Ot chuong', 'kg', 10, 55000),
    (6, N'Rau thom', 'kg', 8, 60000),
    (6, N'Khoai tay', 'kg', 40, 28000)
END
GO

-- Chi tiet phieu 7 (bia)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 7)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (7, N'Bia Sai Gon dac biet', 'thung', 30, 195000),
    (7, N'Bia Heineken', 'thung', 20, 290000),
    (7, N'Bia Tiger', 'thung', 15, 240000)
END
GO

-- Chi tiet phieu 8 (hai san)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 8)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (8, N'Tom hum', 'kg', 8, 550000),
    (8, N'Cua bien', 'kg', 10, 350000),
    (8, N'Ngheu', 'kg', 20, 55000),
    (8, N'So huyet', 'kg', 15, 75000)
END
GO

-- Chi tiet phieu 9 (thit)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 9)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (9, N'Thit ga ta', 'kg', 30, 85000),
    (9, N'Thit heo ba chi', 'kg', 25, 95000),
    (9, N'Thit bo', 'kg', 15, 150000)
END
GO

-- Chi tiet phieu 10 (gia vi)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 10)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (10, N'Tieu xay', 'kg', 5, 120000),
    (10, N'Bot ngot', 'kg', 10, 45000),
    (10, N'Dau an', 'lit', 20, 35000)
END
GO

-- Chi tiet phieu 11 (rau cu tuan 2)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 11)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (11, N'Rau muong', 'kg', 25, 12000),
    (11, N'Gia do', 'kg', 15, 15000),
    (11, N'Ca rot', 'kg', 20, 25000),
    (11, N'Bap cai', 'kg', 15, 18000)
END
GO

-- Chi tiet phieu 12 (sua)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 12)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (12, N'Sua tuoi Vinamilk', 'lit', 50, 32000),
    (12, N'Kem tuoi', 'lit', 15, 85000),
    (12, N'Bo lat', 'kg', 10, 120000),
    (12, N'Pho mai Con Bo Cuoi', 'hop', 30, 45000)
END
GO

-- Chi tiet phieu 13 (nuoc ngot)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 13)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (13, N'Coca Cola', 'thung', 15, 165000),
    (13, N'Pepsi', 'thung', 10, 155000)
END
GO

-- Chi tiet phieu 14 (dong lanh)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 14)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (14, N'Ca hoi phi le', 'kg', 15, 280000),
    (14, N'Tom su', 'kg', 20, 220000),
    (14, N'Chao ca', 'kg', 12, 95000)
END
GO

-- Chi tiet phieu 15 (ruou)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 15)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (15, N'Ruou vang do', 'chai', 12, 350000),
    (15, N'Ruou vang trang', 'chai', 8, 320000),
    (15, N'Ruou Vodka', 'chai', 6, 450000)
END
GO

-- Chi tiet phieu 16 (dau an)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 16)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (16, N'Dau an Neptune', 'can 5L', 10, 135000),
    (16, N'Hat nem Knorr', 'goi 1kg', 20, 48000)
END
GO

-- Chi tiet phieu 17 (hai san tuoi)
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = 17)
BEGIN
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia) VALUES
    (17, N'Tom hum', 'kg', 5, 550000),
    (17, N'Oc huong', 'kg', 12, 130000),
    (17, N'Cua bien', 'kg', 8, 350000),
    (17, N'Muc tuoi', 'kg', 15, 220000),
    (17, N'Ngheu', 'kg', 20, 55000)
END
GO

-- Cap nhat TongTien
UPDATE pn SET TongTien = (
    SELECT ISNULL(SUM(ct.SoLuong * ct.DonGia), 0)
    FROM tblChiTietPhieuNhap ct WHERE ct.MaPhieuNhap = pn.MaPhieuNhap
)
FROM tblPhieuNhap pn
GO

-- Ket qua
SELECT 'After:' AS '', COUNT(*) AS NCC FROM tblNhaCungCap
GO
SELECT COUNT(*) AS Phieu FROM tblPhieuNhap
GO
SELECT COUNT(*) AS ChiTiet FROM tblChiTietPhieuNhap
GO
