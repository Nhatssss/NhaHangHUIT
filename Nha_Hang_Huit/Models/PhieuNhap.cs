using System;
using System.Collections.Generic;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model phieu nhap - schema tblPhieuNhap
    /// </summary>
    public class PhieuNhap
    {
        public int MaPhieuNhap { get; set; }
        public int MaNhaCungCap { get; set; }
        public DateTime NgayNhap { get; set; }
        public string NguoiGiao { get; set; }
        public string NguoiNhan { get; set; }
        public string GhiChu { get; set; }
        public decimal TongTien { get; set; }

        // Display helpers
        public string TenNhaCungCap { get; set; }
        public string NgayNhapDisplay => NgayNhap.ToString("dd/MM/yyyy HH:mm");
        public string TongTienDisplay => $"{TongTien:N0} VND";
        public string SoDienThoaiNCC { get; set; }

        public List<ChiTietPhieuNhap> ChiTietList { get; set; } = new List<ChiTietPhieuNhap>();
    }

    /// <summary>
    /// Model chi tiet phieu nhap - schema tblChiTietPhieuNhap
    /// </summary>
    public class ChiTietPhieuNhap
    {
        public int MaChiTiet { get; set; }
        public int MaPhieuNhap { get; set; }
        public string TenNguyenLieu { get; set; }
        public string DonViTinh { get; set; }
        public decimal SoLuong { get; set; }
        public decimal DonGia { get; set; }
        public decimal ThanhTien { get; set; }

        // Display helpers
        public string SoLuongDisplay => SoLuong.ToString("N2");
        public string DonGiaDisplay => $"{DonGia:N0}";
        public string ThanhTienDisplay => $"{ThanhTien:N0} VND";
    }
}
