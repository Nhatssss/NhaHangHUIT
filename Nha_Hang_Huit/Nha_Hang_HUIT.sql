
--USE master;
--GO
--IF EXISTS (SELECT name FROM sys.databases WHERE name = N'NhaHang_HaiDiLao_Mini')
--BEGIN
--    ALTER DATABASE NhaHang_HaiDiLao_Mini SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
--    DROP DATABASE NhaHang_HaiDiLao_Mini;
--END
GO
CREATE DATABASE Nha_Hang_Hiut  COLLATE Vietnamese_CI_AS;
GO
USE Nha_Hang_Huit;
GO

-- ================================================================
-- PHAN 1: TAO BANG
-- ================================================================

-- tblNhomMonAn: Nhom / danh muc mon an
CREATE TABLE tblNhomMonAn (
    MaNhom          INT           NOT NULL IDENTITY(1,1),
    TenNhom         NVARCHAR(100) NOT NULL,
    MoTaNhom        NVARCHAR(255) NULL,
    ThuTuHienThi    INT           NOT NULL DEFAULT 0,
    TrangThai       BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_tblNhomMonAn PRIMARY KEY (MaNhom)
);
GO

-- tblMonAn: Danh sach mon an trong menu
CREATE TABLE tblMonAn (
    MaMonAn         INT             NOT NULL IDENTITY(1,1),
    MaNhom          INT             NOT NULL,
    TenMonAn        NVARCHAR(150)   NOT NULL,
    GiaBan          DECIMAL(18,0)   NOT NULL DEFAULT 0,
    DonVi           NVARCHAR(30)    NOT NULL DEFAULT N'Phan',
    MoTa            NVARCHAR(500)   NULL,
    DuongDanHinh    NVARCHAR(255)   NULL,
    TrangThai       BIT             NOT NULL DEFAULT 1,
    CONSTRAINT PK_tblMonAn        PRIMARY KEY (MaMonAn),
    CONSTRAINT FK_MonAn_NhomMonAn FOREIGN KEY (MaNhom) REFERENCES tblNhomMonAn(MaNhom)
);
GO

-- tblHangThe: Cap bac thanh vien va ty le giam gia
CREATE TABLE tblHangThe (
    MaHang          INT           NOT NULL IDENTITY(1,1),
    TenHang         NVARCHAR(50)  NOT NULL,
    DiemToiThieu    INT           NOT NULL DEFAULT 0,
    DiemToiDa       INT           NOT NULL DEFAULT 0,   -- 0 = khong gioi han
    TyLeGiamGia     DECIMAL(5,2)  NOT NULL DEFAULT 0,   -- 5.00 = 5%
    MoTa            NVARCHAR(255) NULL,
    MauHienThi      NVARCHAR(20)  NULL DEFAULT '#808080',
    CONSTRAINT PK_tblHangThe PRIMARY KEY (MaHang)
);
GO

-- tblKhachHang: Thong tin khach hang thanh vien
CREATE TABLE tblKhachHang (
    MaKhachHang         INT           NOT NULL IDENTITY(1,1),
    TenKhachHang        NVARCHAR(100) NOT NULL,
    SoDienThoai         VARCHAR(15)   NOT NULL,
    NgayDangKy          DATETIME      NOT NULL DEFAULT GETDATE(),
    TongDiemTichLuy     INT           NOT NULL DEFAULT 0,
    MaHang              INT           NOT NULL DEFAULT 1,
    GhiChu              NVARCHAR(255) NULL,
    TrangThai           BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_tblKhachHang        PRIMARY KEY (MaKhachHang),
    CONSTRAINT FK_KhachHang_HangThe   FOREIGN KEY (MaHang) REFERENCES tblHangThe(MaHang),
    CONSTRAINT UQ_KhachHang_DienThoai UNIQUE (SoDienThoai)
);
GO

-- tblNhanVien: Nhan vien he thong
CREATE TABLE tblNhanVien (
    MaNhanVien  INT           NOT NULL IDENTITY(1,1),
    HoTen       NVARCHAR(100) NOT NULL,
    TaiKhoan    VARCHAR(50)   NOT NULL,
    MatKhau     VARCHAR(100)  NOT NULL,
    ChucVu      NVARCHAR(50)  NOT NULL DEFAULT N'Phuc Vu',
    TrangThai   BIT           NOT NULL DEFAULT 1,
    CONSTRAINT PK_tblNhanVien PRIMARY KEY (MaNhanVien),
    CONSTRAINT UQ_NhanVien_TK UNIQUE (TaiKhoan)
);
GO

-- tblCa: Lich su mo ca / dong ca
CREATE TABLE tblCa (
    MaCa                INT           NOT NULL IDENTITY(1,1),
    MaNhanVien          INT           NOT NULL,
    ThoiGianMoCa        DATETIME      NOT NULL DEFAULT GETDATE(),
    ThoiGianDongCa      DATETIME      NULL,
    TongHoaDon          INT           NOT NULL DEFAULT 0,
    TongDoanhThu        DECIMAL(18,0) NOT NULL DEFAULT 0,
    TongGiamGia         DECIMAL(18,0) NOT NULL DEFAULT 0,
    DoanhThuThucNhan    DECIMAL(18,0) NOT NULL DEFAULT 0,
    GhiChu              NVARCHAR(500) NULL,
    TrangThai           NVARCHAR(20)  NOT NULL DEFAULT 'DangLam',
    CONSTRAINT PK_tblCa       PRIMARY KEY (MaCa),
    CONSTRAINT FK_Ca_NhanVien FOREIGN KEY (MaNhanVien) REFERENCES tblNhanVien(MaNhanVien)
);
GO

-- tblHoaDon: Thong tin hoa don
CREATE TABLE tblHoaDon (
    MaHoaDon            INT           NOT NULL IDENTITY(1,1),
    MaCa                INT           NOT NULL,
    MaKhachHang         INT           NULL,
    SoBan               INT           NOT NULL DEFAULT 0,
    ThoiGianTao         DATETIME      NOT NULL DEFAULT GETDATE(),
    ThoiGianThanhToan   DATETIME      NULL,
    TongTienGoc         DECIMAL(18,0) NOT NULL DEFAULT 0,
    SoTienGiamGia       DECIMAL(18,0) NOT NULL DEFAULT 0,
    TongThanhToan       DECIMAL(18,0) NOT NULL DEFAULT 0,
    SoTienKhachDua      DECIMAL(18,0) NOT NULL DEFAULT 0,
    TienThua            DECIMAL(18,0) NOT NULL DEFAULT 0,
    HinhThucTT          NVARCHAR(20)  NOT NULL DEFAULT 'TienMat',
    DiemTichLuy         INT           NOT NULL DEFAULT 0,
    TrangThai           NVARCHAR(20)  NOT NULL DEFAULT 'ChuaTT',
    CONSTRAINT PK_tblHoaDon        PRIMARY KEY (MaHoaDon),
    CONSTRAINT FK_HoaDon_Ca        FOREIGN KEY (MaCa)        REFERENCES tblCa(MaCa),
    CONSTRAINT FK_HoaDon_KhachHang FOREIGN KEY (MaKhachHang) REFERENCES tblKhachHang(MaKhachHang)
);
GO

-- tblChiTietHoaDon: Tung dong mon an trong hoa don
CREATE TABLE tblChiTietHoaDon (
    MaChiTiet   INT             NOT NULL IDENTITY(1,1),
    MaHoaDon    INT             NOT NULL,
    MaMonAn     INT             NOT NULL,
    TenMonAn    NVARCHAR(150)   NOT NULL,       -- Snapshot ten luc dat
    DonGia      DECIMAL(18,0)   NOT NULL,       -- Snapshot gia luc dat
    SoLuong     INT             NOT NULL DEFAULT 1,
    ThanhTien   AS (DonGia * SoLuong) PERSISTED,
    GhiChu      NVARCHAR(200)   NULL,
    CONSTRAINT PK_tblChiTietHoaDon PRIMARY KEY (MaChiTiet),
    CONSTRAINT FK_ChiTiet_HoaDon   FOREIGN KEY (MaHoaDon) REFERENCES tblHoaDon(MaHoaDon),
    CONSTRAINT FK_ChiTiet_MonAn    FOREIGN KEY (MaMonAn)  REFERENCES tblMonAn(MaMonAn)
);
GO

-- tblLichSuDiemKhachHang: Log tich / doi diem
CREATE TABLE tblLichSuDiemKhachHang (
    MaLichSu        INT           NOT NULL IDENTITY(1,1),
    MaKhachHang     INT           NOT NULL,
    MaHoaDon        INT           NULL,
    LoaiGiaoDich    NVARCHAR(20)  NOT NULL,  -- TichDiem | DoiDiem | DieuChinh
    SoDiem          INT           NOT NULL,
    DiemSauGiaoDich INT           NOT NULL,
    ThoiGian        DATETIME      NOT NULL DEFAULT GETDATE(),
    GhiChu          NVARCHAR(255) NULL,
    CONSTRAINT PK_tblLichSuDiem  PRIMARY KEY (MaLichSu),
    CONSTRAINT FK_LichSuDiem_KH  FOREIGN KEY (MaKhachHang) REFERENCES tblKhachHang(MaKhachHang),
    CONSTRAINT FK_LichSuDiem_HD  FOREIGN KEY (MaHoaDon)    REFERENCES tblHoaDon(MaHoaDon)
);
GO

-- ================================================================
-- PHAN 2: INDEX
-- ================================================================
CREATE INDEX IX_MonAn_MaNhom        ON tblMonAn(MaNhom);
CREATE INDEX IX_MonAn_TrangThai     ON tblMonAn(TrangThai);
CREATE INDEX IX_KhachHang_DienThoai ON tblKhachHang(SoDienThoai);
CREATE INDEX IX_HoaDon_MaCa         ON tblHoaDon(MaCa);
CREATE INDEX IX_HoaDon_TrangThai    ON tblHoaDon(TrangThai);
CREATE INDEX IX_ChiTiet_MaHoaDon    ON tblChiTietHoaDon(MaHoaDon);
GO

-- ================================================================
-- PHAN 3: DU LIEU MAU
-- ================================================================

INSERT INTO tblHangThe (TenHang, DiemToiThieu, DiemToiDa, TyLeGiamGia, MoTa, MauHienThi) VALUES
('Thuong',    0,    99,   0.00, N'Khach moi - Khong giam gia',      '#808080'),
('Bac',       100,  499,  3.00, N'Tu 100 diem - Giam 3%',           '#C0C0C0'),
('Vang',      500,  999,  5.00, N'Tu 500 diem - Giam 5%',           '#FFD700'),
('Kim Cuong', 1000, 0,   10.00, N'Tu 1000 diem - Giam 10%',         '#00BFFF');
GO

INSERT INTO tblNhomMonAn (TenNhom, MoTaNhom, ThuTuHienThi) VALUES
(N'Lo Nuong',   N'Cac loai lo lau dac trung cua Haidilao', 1),
(N'Thit Bo',    N'Thit bo cao cap nhuong va nhung lau',    2),
(N'Thit Heo',   N'Cac loai thit heo tuoi ngon',            3),
(N'Hai San',    N'Hai san tuoi song nhung lau va nuong',    4),
(N'Rau Cu Nam', N'Rau xanh, cu qua va cac loai nam tuoi',  5),
(N'Vien & Bot', N'Cac loai vien, banh va san pham tu bot', 6),
(N'Mon Them',   N'Com, mi, chao, banh trang an kem',       7),
(N'Do Uong',    N'Nuoc ngot, nuoc ep, tra, bia',           8);
GO

-- Nhom 1: Lo Nuong
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(1, N'Lo Lau Cay Dac Biet',   199000, N'Lo',  N'Lo lau cay ky dau, vi sau dam dac trung Haidilao'),
(1, N'Lo Lau Hai San',        219000, N'Lo',  N'Lo lau vi ngot tu xuong ham hai san trong lanh'),
(1, N'Lo Lau Nam Tong Hop',   189000, N'Lo',  N'Lo lau vi ngot tu 5 loai nam rung tuoi'),
(1, N'Lo Lau Tom Chua Cay',   209000, N'Lo',  N'Lo lau chua nhe cay vua, thom mui sa la chanh');

-- Nhom 2: Thit Bo
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(2, N'Thit Bo My Nhung Lau',  299000, N'Dia', N'Thit bo nguyen ban My, cat lat mong, mem tan'),
(2, N'Bo Wagyu Cat Lat',      459000, N'Dia', N'Bo Wagyu A5 thuong hang, beo ngon vo cung'),
(2, N'Bo Ba Chi Cuon La Lot', 189000, N'Dia', N'Ba chi cuon la lot, tham chat nuoc lau'),
(2, N'Bo Non Xao La Giang',   169000, N'Dia', N'Bo non xao chua nhe, an kem bun tuoi');

-- Nhom 3: Thit Heo
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(3, N'Heo Ba Chi Cat Lat',    139000, N'Dia', N'Ba chi heo tuoi, cat mong, cuon rau rat ngon'),
(3, N'Heo Vai Nuong Gia Vi',  149000, N'Dia', N'Thit vai heo uop gia vi dac biet, nuong vang xu'),
(3, N'Nem Nuong Heo',         129000, N'Dia', N'Nem heo thom ngon, cuon banh trang voi rau song');

-- Nhom 4: Hai San
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(4, N'Tom Su Tuoi Song',      249000, N'Dia', N'Tom su co con, tuoi song, thit gion ngon ngot'),
(4, N'Muc Ong Cat Khoanh',    219000, N'Dia', N'Muc ong cat khoanh trang tron, gion ngon'),
(4, N'Ca Phi Le Nhung Lau',   199000, N'Dia', N'Ca tuoi phi le mong, nhung lau chin nhanh'),
(4, N'So Diep Nuong Mo Hanh', 269000, N'Dia', N'So diep nuong tren lo, pho mo hanh la thom ngay');

-- Nhom 5: Rau Cu Nam
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(5, N'Nam Tong Hop 5 Loai',   89000,  N'Dia', N'Nam Huong, Nam Kim, Nam Dau Hu, Nam Bao Ngu, Nam Truffle'),
(5, N'Rau Muong Nhung Lau',   45000,  N'Dia', N'Rau muong tuoi rua sach, nhung lau chin gion'),
(5, N'Dau Phu Non Nhat Ban',  55000,  N'Dia', N'Dau phu non tan chay, hut vi nuoc lau'),
(5, N'Khoai Mon Cat Lat',     59000,  N'Dia', N'Khoai mon cat lat nhung lau beo bot deo ngon');

-- Nhom 6: Vien & Bot
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(6, N'Vien Bo Giay Bac',      99000,  N'Dia', N'Vien bo hinh dong xu bac, dai gion tham vi lau'),
(6, N'Vien Tom Thit',         89000,  N'Dia', N'Vien mix tom tuoi va thit heo bam, gion ngot'),
(6, N'Banh Gao Han Quoc',     79000,  N'Dia', N'Banh gao dai nhung lau hoac xao sot cay');

-- Nhom 7: Mon Them
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(7, N'Mi Trung Nhung Lau',    35000,  N'Phan', N'Mi trung vang uom, nhung lau chin deo ngon'),
(7, N'Com Trang',             20000,  N'Chen', N'Com hat dai thom, nau trong nieu dat truyen thong'),
(7, N'Banh Trang Nuong Gion', 45000,  N'Dia',  N'Banh trang nuong gion, an kem thit va rau song');

-- Nhom 8: Do Uong
INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa) VALUES
(8, N'Nuoc Ep Dua Hau Tuoi',  49000,  N'Ly',  N'Nuoc ep dua hau tuoi ngot, uop lanh giai nhiet'),
(8, N'Tra Sua Truyen Thong',  55000,  N'Ly',  N'Tra sua vi goc beo ngam mo, uong lanh'),
(8, N'Coca Cola Lon 330ml',   35000,  N'Lon', N'Coca Cola 330ml uop lanh suc khoan'),
(8, N'Bia Tiger Lon 330ml',   45000,  N'Lon', N'Bia Tiger 330ml uop lanh, thuong thuc bua an');
GO

-- Nhan Vien mac dinh (mat khau test: 123456)
INSERT INTO tblNhanVien (HoTen, TaiKhoan, MatKhau, ChucVu) VALUES
(N'Nguyen Van An',  'admin',   '123456', N'Quan Ly'),
(N'Tran Thi Bich',  'bich01',  '123456', N'Thu Ngan'),
(N'Le Minh Cuong',  'cuong02', '123456', N'Phuc Vu');
GO

-- Khach Hang mau (test cac hang the)
INSERT INTO tblKhachHang (TenKhachHang, SoDienThoai, TongDiemTichLuy, MaHang) VALUES
(N'Nguyen Thi Mai', '0901234567',    0, 1),  -- Hang Thuong
(N'Tran Van Hung',  '0912345678',  150, 2),  -- Hang Bac
(N'Le Thanh Tam',   '0923456789',  560, 3),  -- Hang Vang
(N'Pham Minh Duc',  '0934567890', 1250, 4);  -- Hang Kim Cuong
GO

-- ================================================================
-- PHAN 4: STORED PROCEDURES
-- ================================================================

-- spDangNhapNhanVien
CREATE PROCEDURE spDangNhapNhanVien
    @TaiKhoan VARCHAR(50), @MatKhau VARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT MaNhanVien, HoTen, TaiKhoan, ChucVu
    FROM tblNhanVien
    WHERE TaiKhoan = @TaiKhoan AND MatKhau = @MatKhau AND TrangThai = 1;
END
GO

-- spLayDanhSachMonAn: Lay menu, @MaNhom = NULL thi lay tat ca
CREATE PROCEDURE spLayDanhSachMonAn
    @MaNhom INT = NULL
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ma.MaMonAn, ma.MaNhom, n.TenNhom, ma.TenMonAn,
           ma.GiaBan, ma.DonVi, ma.MoTa, ma.DuongDanHinh, ma.TrangThai
    FROM tblMonAn ma
    INNER JOIN tblNhomMonAn n ON ma.MaNhom = n.MaNhom
    WHERE ma.TrangThai = 1
      AND (@MaNhom IS NULL OR ma.MaNhom = @MaNhom)
    ORDER BY n.ThuTuHienThi, ma.TenMonAn;
END
GO

-- spTimKhachHangTheoDienThoai
CREATE PROCEDURE spTimKhachHangTheoDienThoai
    @SoDienThoai VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT kh.MaKhachHang, kh.TenKhachHang, kh.SoDienThoai,
           kh.NgayDangKy, kh.TongDiemTichLuy,
           ht.MaHang, ht.TenHang, ht.TyLeGiamGia, ht.MauHienThi
    FROM tblKhachHang kh
    INNER JOIN tblHangThe ht ON kh.MaHang = ht.MaHang
    WHERE kh.SoDienThoai = @SoDienThoai AND kh.TrangThai = 1;
END
GO

-- spDangKyKhachHangMoi
CREATE PROCEDURE spDangKyKhachHangMoi
    @TenKhachHang NVARCHAR(100), @SoDienThoai VARCHAR(15)
AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM tblKhachHang WHERE SoDienThoai = @SoDienThoai)
    BEGIN
        SELECT -1 AS MaKhachHang, N'So dien thoai da duoc dang ky' AS ThongBao;
        RETURN;
    END
    INSERT INTO tblKhachHang (TenKhachHang, SoDienThoai, MaHang)
    VALUES (@TenKhachHang, @SoDienThoai, 1);
    SELECT SCOPE_IDENTITY() AS MaKhachHang, N'Dang ky thanh cong' AS ThongBao;
END
GO

-- spMoCa
CREATE PROCEDURE spMoCa
    @MaNhanVien INT, @MaCaMoi INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblCa (MaNhanVien, TrangThai) VALUES (@MaNhanVien, 'DangLam');
    SET @MaCaMoi = SCOPE_IDENTITY();
END
GO

-- spTaoHoaDonMoi
CREATE PROCEDURE spTaoHoaDonMoi
    @MaCa INT, @MaKhachHang INT = NULL, @SoBan INT = 0, @MaHoaDonMoi INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    INSERT INTO tblHoaDon (MaCa, MaKhachHang, SoBan, TrangThai)
    VALUES (@MaCa, @MaKhachHang, @SoBan, 'ChuaTT');
    SET @MaHoaDonMoi = SCOPE_IDENTITY();
END
GO

-- spThemChiTietHoaDon: Them mon vao hoa don (cong so luong neu mon da co)
CREATE PROCEDURE spThemChiTietHoaDon
    @MaHoaDon INT, @MaMonAn INT, @SoLuong INT, @GhiChu NVARCHAR(200) = NULL
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TenMonAn NVARCHAR(150);
    DECLARE @DonGia   DECIMAL(18,0);

    SELECT @TenMonAn = TenMonAn, @DonGia = GiaBan
    FROM tblMonAn WHERE MaMonAn = @MaMonAn;

    IF EXISTS (SELECT 1 FROM tblChiTietHoaDon
               WHERE MaHoaDon = @MaHoaDon AND MaMonAn = @MaMonAn)
        UPDATE tblChiTietHoaDon
        SET SoLuong = SoLuong + @SoLuong
        WHERE MaHoaDon = @MaHoaDon AND MaMonAn = @MaMonAn;
    ELSE
        INSERT INTO tblChiTietHoaDon (MaHoaDon, MaMonAn, TenMonAn, DonGia, SoLuong, GhiChu)
        VALUES (@MaHoaDon, @MaMonAn, @TenMonAn, @DonGia, @SoLuong, @GhiChu);
END
GO

-- spLayChiTietHoaDon
CREATE PROCEDURE spLayChiTietHoaDon
    @MaHoaDon INT
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ct.MaChiTiet, ct.MaMonAn, ct.TenMonAn, n.TenNhom,
           ct.DonGia, ct.SoLuong, ct.ThanhTien, ct.GhiChu
    FROM tblChiTietHoaDon ct
    INNER JOIN tblMonAn     ma ON ct.MaMonAn = ma.MaMonAn
    INNER JOIN tblNhomMonAn  n ON ma.MaNhom  = n.MaNhom
    WHERE ct.MaHoaDon = @MaHoaDon;
END
GO

-- spXacNhanThanhToan: Tinh toan + cap nhat hoa don + tich diem
CREATE PROCEDURE spXacNhanThanhToan
    @MaHoaDon       INT,
    @HinhThucTT     NVARCHAR(20),
    @SoTienKhachDua DECIMAL(18,0),
    @TyLeGiamGia    DECIMAL(5,2) = 0.00
AS
BEGIN
    SET NOCOUNT ON;
    BEGIN TRY
        BEGIN TRANSACTION;

        DECLARE @TongTienGoc    DECIMAL(18,0);
        DECLARE @MaKhachHang    INT;

        SELECT @TongTienGoc = SUM(ct.ThanhTien),
               @MaKhachHang = hd.MaKhachHang
        FROM tblChiTietHoaDon ct
        INNER JOIN tblHoaDon hd ON ct.MaHoaDon = hd.MaHoaDon
        WHERE ct.MaHoaDon = @MaHoaDon
        GROUP BY hd.MaKhachHang;

        DECLARE @SoTienGiamGia DECIMAL(18,0) = ROUND(@TongTienGoc * @TyLeGiamGia / 100.0, 0);
        DECLARE @TongThanhToan DECIMAL(18,0) = @TongTienGoc - @SoTienGiamGia;
        DECLARE @TienThua      DECIMAL(18,0) = @SoTienKhachDua - @TongThanhToan;
        DECLARE @DiemTichLuy   INT           = FLOOR(@TongThanhToan / 10000);

        UPDATE tblHoaDon SET
            ThoiGianThanhToan = GETDATE(),
            TongTienGoc       = @TongTienGoc,
            SoTienGiamGia     = @SoTienGiamGia,
            TongThanhToan     = @TongThanhToan,
            SoTienKhachDua    = @SoTienKhachDua,
            TienThua          = @TienThua,
            HinhThucTT        = @HinhThucTT,
            DiemTichLuy       = @DiemTichLuy,
            TrangThai         = 'DaThanhToan'
        WHERE MaHoaDon = @MaHoaDon;

        IF @MaKhachHang IS NOT NULL AND @DiemTichLuy > 0
        BEGIN
            DECLARE @DiemHienTai INT;
            SELECT @DiemHienTai = TongDiemTichLuy FROM tblKhachHang WHERE MaKhachHang = @MaKhachHang;
            DECLARE @DiemMoi INT = @DiemHienTai + @DiemTichLuy;

            UPDATE tblKhachHang SET
                TongDiemTichLuy = @DiemMoi,
                MaHang = (
                    SELECT TOP 1 MaHang FROM tblHangThe
                    WHERE DiemToiThieu <= @DiemMoi
                      AND (DiemToiDa = 0 OR DiemToiDa >= @DiemMoi)
                    ORDER BY DiemToiThieu DESC
                )
            WHERE MaKhachHang = @MaKhachHang;

            INSERT INTO tblLichSuDiemKhachHang
                (MaKhachHang, MaHoaDon, LoaiGiaoDich, SoDiem, DiemSauGiaoDich, GhiChu)
            VALUES
                (@MaKhachHang, @MaHoaDon, 'TichDiem', @DiemTichLuy, @DiemMoi,
                 N'Tich diem tu hoa don #' + CAST(@MaHoaDon AS NVARCHAR(10)));
        END

        COMMIT TRANSACTION;

        SELECT @TongTienGoc   AS TongTienGoc,  @SoTienGiamGia AS SoTienGiamGia,
               @TongThanhToan AS TongThanhToan, @TienThua      AS TienThua,
               @DiemTichLuy   AS DiemTichLuy;
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        THROW;
    END CATCH
END
GO

-- spDongCa: Tong ket va dong ca lam viec
-- Tra ve 2 result set: (1) Tong hop ca  (2) Top 5 mon ban chay
CREATE PROCEDURE spDongCa
    @MaCa INT
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @TongHoaDon   INT;
    DECLARE @TongDT       DECIMAL(18,0);
    DECLARE @TongGG       DECIMAL(18,0);
    DECLARE @ThucNhan     DECIMAL(18,0);

    SELECT @TongHoaDon = COUNT(*),
           @TongDT     = ISNULL(SUM(TongTienGoc),   0),
           @TongGG     = ISNULL(SUM(SoTienGiamGia), 0),
           @ThucNhan   = ISNULL(SUM(TongThanhToan),  0)
    FROM tblHoaDon
    WHERE MaCa = @MaCa AND TrangThai = 'DaThanhToan';

    UPDATE tblCa SET
        ThoiGianDongCa   = GETDATE(),
        TongHoaDon       = ISNULL(@TongHoaDon, 0),
        TongDoanhThu     = ISNULL(@TongDT,     0),
        TongGiamGia      = ISNULL(@TongGG,     0),
        DoanhThuThucNhan = ISNULL(@ThucNhan,   0),
        TrangThai        = 'DaDong'
    WHERE MaCa = @MaCa;

    -- Result set 1: Tong hop ca
    SELECT ca.MaCa, nv.HoTen AS TenNhanVien,
           ca.ThoiGianMoCa,  GETDATE() AS ThoiGianDongCa,
           ISNULL(@TongHoaDon, 0) AS TongHoaDon,
           ISNULL(@TongDT,     0) AS TongDoanhThu,
           ISNULL(@TongGG,     0) AS TongGiamGia,
           ISNULL(@ThucNhan,   0) AS DoanhThuThucNhan,
           (SELECT COUNT(*) FROM tblKhachHang
            WHERE CAST(NgayDangKy AS DATE) = CAST(ca.ThoiGianMoCa AS DATE)
           ) AS KhachMoiTrongNgay
    FROM tblCa ca
    INNER JOIN tblNhanVien nv ON ca.MaNhanVien = nv.MaNhanVien
    WHERE ca.MaCa = @MaCa;

    -- Result set 2: Top 5 mon ban chay
    SELECT TOP 5
        ct.TenMonAn,
        SUM(ct.SoLuong)   AS TongSoLuong,
        SUM(ct.ThanhTien) AS TongDoanhThuMon
    FROM tblChiTietHoaDon ct
    INNER JOIN tblHoaDon hd ON ct.MaHoaDon = hd.MaHoaDon
    WHERE hd.MaCa = @MaCa AND hd.TrangThai = 'DaThanhToan'
    GROUP BY ct.TenMonAn
    ORDER BY TongSoLuong DESC;
END
GO

-- ================================================================
-- PHAN 5: VIEW
-- ================================================================

CREATE VIEW vwHoaDonHomNay AS
SELECT hd.MaHoaDon, hd.SoBan, hd.ThoiGianTao, hd.ThoiGianThanhToan,
       kh.TenKhachHang, kh.SoDienThoai,
       hd.TongTienGoc, hd.SoTienGiamGia, hd.TongThanhToan,
       hd.HinhThucTT, hd.DiemTichLuy, hd.TrangThai
FROM tblHoaDon hd
LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
WHERE CAST(hd.ThoiGianTao AS DATE) = CAST(GETDATE() AS DATE);
GO

CREATE VIEW vwChiTietHoaDonDayDu AS
SELECT ct.MaChiTiet, ct.MaHoaDon, ct.MaMonAn,
       ct.TenMonAn,  n.TenNhom,   ct.DonGia,
       ct.SoLuong,   ct.ThanhTien, ct.GhiChu
FROM tblChiTietHoaDon ct
INNER JOIN tblMonAn     ma ON ct.MaMonAn = ma.MaMonAn
INNER JOIN tblNhomMonAn  n ON ma.MaNhom  = n.MaNhom;
GO

-- ================================================================
-- KIEM TRA CUOI
-- ================================================================
SELECT 'tblNhomMonAn' AS TenBang, COUNT(*) AS SoBanGhi FROM tblNhomMonAn UNION ALL
SELECT 'tblMonAn',               COUNT(*)              FROM tblMonAn      UNION ALL
SELECT 'tblHangThe',             COUNT(*)              FROM tblHangThe    UNION ALL
SELECT 'tblNhanVien',            COUNT(*)              FROM tblNhanVien   UNION ALL
SELECT 'tblKhachHang',           COUNT(*)              FROM tblKhachHang;
GO

PRINT N'=== HOAN THANH: NhaHang_HaiDiLao_Mini tao thanh cong! ===';
GO