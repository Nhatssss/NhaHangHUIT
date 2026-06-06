Add-Type -AssemblyName System.Data.SqlClient
$conn = New-Object System.Data.SqlClient.SqlConnection('Server=DESKTOP-6UTLKJJ;Database=NhaHang_HaiDiLao_Mini;Trusted_Connection=true;TrustServerCertificate=True;')
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = '-- Them them nha cung cap
IF NOT EXISTS (SELECT 1 FROM tblNhaCungCap)
BEGIN
    INSERT INTO tblNhaCungCap (TenNCC, DiaChi, SoDienThoai, Email, GhiChu)
    VALUES
        (N''Công ty TNHH Thực Phẩm Xanh'', N''123 Nguyễn Văn Linh, Q.7, TP.HCM'', N''0912345678'', N''info@thucphamxanh.vn'', N''Cung cấp rau củ quả''),
        (N''Công ty CP Bia Sài Gòn'', N''128 Lê Lai, Q.1, TP.HCM'', N''0923456789'', N''order@biahcm.vn'', N''Nhà phân phối chính thức''),
        (N''Công ty TNHH Hải Sản Biển Đông'', N''456 Nguyễn Tất Thành, Q.4, TP.HCM'', N''0934567890'', N''hanh@bienhd.vn'', N''Hải sản tươi sống''),
        (N''Công ty CP Chăn Nuôi Việt'', N''789 Quốc Lộ 13, Q.Thủ Đức, TP.HCM'', N''0945678901'', N''info@chanuoiviet.vn'', N''Thịt gia súc, gia cầm''),
        (N''Công ty TNHH Gia Vị Á Âu'', N''321 Lê Văn Sỹ, Q.3, TP.HCM'', N''0956789012'', N''sale@giaviau.vn'', N''Gia vị, sốt, nguyên liệu chế biến''),
        (N''Công ty TNHH Sữa & Sản Phẩm Sữa'', N''555 Trần Hưng Đạo, Q.1, TP.HCM'', N''0967890123'', N''order@suaviet.vn'', N''Sữa, kem, bơ, phô mai''),
        (N''Doanh nghiệp Tư nhân Nước Giải Khát Đông Á'', N''777 Lý Thường Kiệt, Q.10, TP.HCM'', N''0978901234'', N''sale@nuocdk.vn'', N''Nước ngọt, nước suối''),
        (N''Công ty TNHH Đông Lạnh Nam Hải'', N''999 Xa Lộ Hà Nội, Q.9, TP.HCM'', N''0989012345'', N''info@donglanh.vn'', N''Thực phẩm đông lạnh''),
        (N''Công ty CP Rượu Việt'', N''111 Lý Chính Thắng, Q.Phú Nhuận, TP.HCM'', N''0990123456'', N''order@ruouviet.vn'', N''Rượu vang, rượu mạnh''),
        (N''Công ty TNHH Dầu Ăn - Bột Ngọt'', N''222 Nguyễn Thị Minh Khai, Q.1, TP.HCM'', N''0901122334'', N''info@daubuot.vn'', N''Dầu ăn, bột ngọt, hạt nêm'')
END

-- Them them phieu nhap voi ngay khac nhau trong thang 5/2026
IF NOT EXISTS (SELECT 1 FROM tblPhieuNhap WHERE MaPhieuNhap > 5)
BEGIN
    DECLARE @today DATE = CAST(GETDATE() AS DATE);
    
    INSERT INTO tblPhieuNhap (MaNCC, NgayNhap, NguoiGiao, NguoiNhan, TongTien, GhiChu)
    VALUES
        (1, DATEADD(DAY, -15, @today), N''Nguyễn Văn An'', N''Trần Thị Bình'', 12500000, N''Phiếu nhập rau củ tuần 1''),
        (2, DATEADD(DAY, -14, @today), N''Phạm Văn Cường'', N''Lê Thị Dung'', 18500000, N''Phiếu nhập bia''),
        (3, DATEADD(DAY, -12, @today), N''Lê Văn Em'', N''Nguyễn Thị Phương'', 24500000, N''Hải sản tươi sống cuối tuần''),
        (4, DATEADD(DAY, -10, @today), N''Hoàng Văn Giang'', N''Phạm Thị Hoa'', 9800000, N''Thịt gà, thịt heo''),
        (5, DATEADD(DAY, -8, @today), N''Vũ Văn Hưng'', N''Trần Thị Huệ'', 5200000, N''Gia vị nhập bổ sung''),
        (1, DATEADD(DAY, -7, @today), N''Nguyễn Văn An'', N''Lê Thị Dung'', 8200000, N''Rau củ tuần 2''),
        (6, DATEADD(DAY, -6, @today), N''Trần Văn Khải'', N''Nguyễn Thị Lan'', 15400000, N''Sữa, kem, bơ''),
        (7, DATEADD(DAY, -5, @today), N''Đặng Văn Long'', N''Phạm Thị Hoa'', 6700000, N''Nước ngọt, nước suối''),
        (8, DATEADD(DAY, -3, @today), N''Ngô Văn Mạnh'', N''Trần Thị Bình'', 19300000, N''Đông lạnh''),
        (9, DATEADD(DAY, -2, @today), N''Lý Văn Nam'', N''Nguyễn Thị Phương'', 21000000, N''Rượu''),
        (10, DATEADD(DAY, -1, @today), N''Phan Văn Oanh'', N''Lê Thị Dung'', 4300000, N''Dầu ăn, hạt nêm''),
        (3, DATEADD(DAY, 0, @today), N''Lê Văn Em'', N''Trần Thị Huệ'', 28600000, N''Hải sản tươi''),
        (2, DATEADD(DAY, 0, @today), N''Phạm Văn Cường'', N''Nguyễn Thị Lan'', 15900000, N''Bia bổ sung cuối tuần'')
END

-- Them chi tiet cho cac phieu nhap moi
IF NOT EXISTS (SELECT 1 FROM tblChiTietPhieuNhap WHERE MaChiTiet > 18)
BEGIN
    -- Phieu 6: Rau cu tuan 2 (NCC 1)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (6, N''Xà lách'', ''kg'', 20, 35000),
        (6, N''Cà chua'', ''kg'', 30, 25000),
        (6, N''Hành tây'', ''kg'', 25, 42000),
        (6, N''Ớt chuông'', ''kg'', 10, 55000),
        (6, N''Rau thơm'', ''kg'', 8, 60000),
        (6, N''Khoai tây'', ''kg'', 40, 28000);

    -- Phieu 7: Sua (NCC 6)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (7, N''Sữa tươi Vinamilk'', ''lít'', 50, 32000),
        (7, N''Kem tươi'', ''lít'', 15, 85000),
        (7, N''Bơ lạt'', ''kg'', 10, 120000),
        (7, N''Phô mai Con BÒ Cười'', ''hộp'', 30, 45000),
        (7, N''Sữa đặc'', ''hộp'', 24, 28000);

    -- Phieu 8: Nuoc ngot (NCC 7)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (8, N''Coca Cola'', ''thùng'', 20, 165000),
        (8, N''Pepsi'', ''thùng'', 15, 155000),
        (8, N''Seven Up'', ''thùng'', 10, 155000),
        (8, N''Sting'', ''thùng'', 12, 120000);

    -- Phieu 9: Dong lanh (NCC 8)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (9, N''Cá hồi phi lê'', ''kg'', 15, 280000),
        (9, N''Tôm sú'', ''kg'', 20, 220000),
        (9, N''Mực ống'', ''kg'', 18, 190000),
        (9, N''Chả cá'', ''kg'', 12, 95000);

    -- Phieu 10: Ruou (NCC 9)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (10, N''Rượu vang đỏ'', ''chai'', 12, 350000),
        (10, N''Rượu vang trắng'', ''chai'', 8, 320000),
        (10, N''Rượu Vodka'', ''chai'', 6, 450000),
        (10, N''Rượu trái cây'', ''chai'', 15, 180000);

    -- Phieu 11: Dau an (NCC 10)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (11, N''Dầu ăn Neptune'', ''can 5L'', 10, 135000),
        (11, N''Dầu ăn Simply'', ''can 5L'', 8, 125000),
        (11, N''Hạt nêm Knorr'', ''gói 1kg'', 20, 48000),
        (11, N''Bột ngọt Ajinomoto'', ''gói 500g'', 15, 24000);

    -- Phieu 12: Hai san tuoi (NCC 3)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (12, N''Tôm hùm'', ''kg'', 8, 550000),
        (12, N''Cua biển'', ''kg'', 10, 350000),
        (12, N''Nghêu'', ''kg'', 20, 55000),
        (12, N''Sò huyết'', ''kg'', 15, 75000),
        (12, N''Ốc hương'', ''kg'', 12, 130000),
        (12, N''Mực tươi'', ''kg'', 15, 220000);

    -- Phieu 13: Bo sung bia cuoi tuan (NCC 2)
    INSERT INTO tblChiTietPhieuNhap (MaPhieuNhap, TenNguyenLieu, DonViTinh, SoLuong, DonGia)
    VALUES 
        (13, N''Bia Sài Gòn đặc biệt'', ''thùng'', 30, 195000),
        (13, N''Bia Heineken'', ''thùng'', 20, 290000),
        (13, N''Bia Tiger'', ''thùng'', 15, 240000);
END

-- Update TongTien for all purchase orders
UPDATE pn SET TongTien = (
    SELECT ISNULL(SUM(ct.SoLuong * ct.DonGia), 0)
    FROM tblChiTietPhieuNhap ct
    WHERE ct.MaPhieuNhap = pn.MaPhieuNhap
)
FROM tblPhieuNhap pn

SELECT ''Data inserted!'' AS Status;
SELECT COUNT(*) AS NCC_Count FROM tblNhaCungCap;
SELECT COUNT(*) AS PhieuNhap_Count FROM tblPhieuNhap;
SELECT COUNT(*) AS ChiTiet_Count FROM tblChiTietPhieuNhap;'
$rdr = $cmd.ExecuteReader()
while ($rdr.Read()) {
    if ($rdr.FieldCount -gt 0) {
         = @()
        for ($i = 0; $i -lt $rdr.FieldCount; $i++) { $vals += $rdr.GetValue($i) }
        Write-Output ($vals -join ' | ')
    }
}
$rdr.Close()
$conn.Close()
