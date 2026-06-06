using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service quan ly gio hang (shared state giua cac ViewModel)
    /// Dung nhu singleton toan cuc
    /// </summary>
    public class CartService
    {
        private static readonly Lazy<CartService> _instance = new Lazy<CartService>(() => new CartService());
        public static CartService Instance => _instance.Value;

        public List<ChiTietHoaDon> GioHang { get; private set; } = new List<ChiTietHoaDon>();
        public int? MaKhachHangHienTai { get; set; }
        public decimal TienGiamGiaHienTai { get; set; }

        /// <summary>Discount percentage stored as C# decimal 0-1 range (e.g., 0.05 = 5%)</summary>
        public decimal TiLeGiamGiaHienTai { get; set; }

        // Thong tin ban hien tai
        public int? SoBanHienTai { get; set; }
        public string TenBanHienTai { get; set; }

        // Event thong bao gio hang thay doi
        public event EventHandler GioHangChanged;

        private CartService() { }

        private void NotifyGioHangChanged() => GioHangChanged?.Invoke(this, EventArgs.Empty);

        /// <summary>
        /// Them mon an vao gio hang
        /// </summary>
        public void ThemVaoGioHang(MonAn monAn, int soLuong)
        {
            var item = GioHang.Find(i => i.MaMonAn == monAn.MaMonAn);
            if (item != null)
            {
                item.SoLuong += soLuong;
            }
            else
            {
                GioHang.Add(new ChiTietHoaDon
                {
                    MaMonAn = monAn.MaMonAn,
                    TenMonAn = monAn.TenMonAn,
                    SoLuong = soLuong,
                    DonGia = monAn.Gia,
                    IsTempItem = true
                });
            }
            NotifyGioHangChanged();
        }

        public void CapNhatSoLuong(int maMonAn, int soLuongMoi)
        {
            var item = GioHang.Find(i => i.MaMonAn == maMonAn);
            if (item != null)
            {
                if (soLuongMoi <= 0)
                    GioHang.Remove(item);
                else
                    item.SoLuong = soLuongMoi;
            }
            NotifyGioHangChanged();
        }

        public void XoaKhoiGioHang(int maMonAn)
        {
            GioHang.RemoveAll(i => i.MaMonAn == maMonAn);
            NotifyGioHangChanged();
        }

        public void XoaGioHang()
        {
            GioHang.Clear();
            MaKhachHangHienTai = null;
            TienGiamGiaHienTai = 0;
            TiLeGiamGiaHienTai = 0;
            SoBanHienTai = null;
            TenBanHienTai = null;
            NotifyGioHangChanged();
        }

        public decimal TinhTongTien()
        {
            return GioHang.Sum(i => i.ThanhTien);
        }

        public decimal TinhThanhTien()
        {
            return TinhTongTien() - TienGiamGiaHienTai;
        }

        /// <summary>
        /// Tao hoa don trong DB va tra ve MaHoaDon
        /// </summary>
        public int? TaoHoaDon(HoaDonService hoaDonService, BanAnService banAnService = null)
        {
            if (GioHang.Count == 0) return null;

            int? maCa = hoaDonService.GetCurrentCa();
            if (!maCa.HasValue) return null;

            var hd = new HoaDon
            {
                MaKhachHang = MaKhachHangHienTai,
                MaCa = maCa.Value
            };

            int maHoaDon = hoaDonService.Insert(hd, SoBanHienTai ?? 0);
            if (maHoaDon <= 0) return null;

            // Luu chi tiet hoa don
            foreach (var ct in GioHang)
            {
                ct.MaHoaDon = maHoaDon;
                hoaDonService.InsertChiTiet(ct);
            }

            // Cap nhat trang thai ban: DangDung
            if (banAnService != null && SoBanHienTai.HasValue)
                banAnService.CapNhatTrangThai(SoBanHienTai.Value, "DangDung");

            return maHoaDon;
        }

        /// <summary>
        /// Xac nhan thanh toan bang spXacNhanThanhToan
        /// </summary>
        public bool XacNhanThanhToan(int maHoaDon, string phuongThuc, decimal tienKhachDua, HoaDonService hoaDonService, KhachHangService khachHangService, BanAnService banAnService = null)
        {
            try
            {
                // spXacNhanThanhToan tinh toan tu dong:
                // - TongTienGoc = SUM(ct.ThanhTien)
                // - SoTienGiamGia = ROUND(TongTienGoc * TyLeGiamGia / 100, 0)
                // - Cap nhat diem + hang the khach hang tu dong
                // - Ghi vao tblLichSuDiemKhachHang
                hoaDonService.XacNhanThanhToan(
                    maHoaDon,
                    tienKhachDua,  // SoTienKhachDua thuc te khach dua
                    phuongThuc,
                    TiLeGiamGiaHienTai);

                // Giai phong ban: set trang thai lai = Trong
                if (banAnService != null && SoBanHienTai.HasValue)
                    banAnService.CapNhatTrangThai(SoBanHienTai.Value, "Trong");

                // Xoa gio hang
                XoaGioHang();
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("XacNhanThanhToan error: " + ex.Message);
                return false;
            }
        }
    }
}
