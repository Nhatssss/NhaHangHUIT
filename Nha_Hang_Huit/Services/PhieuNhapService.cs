using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly nha cung cap & phieu nhap
    /// </summary>
    public class PhieuNhapService
    {
        // ==================== NHÀ CUNG CẤP ====================

        public List<NhaCungCap> GetAllSuppliers()
        {
            var list = new List<NhaCungCap>();
            string query = "SELECT * FROM tblNhaCungCap ORDER BY TenNhaCungCap";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToNhaCungCap(row));
            return list;
        }

        public NhaCungCap GetSupplierById(int maNcc)
        {
            string query = "SELECT * FROM tblNhaCungCap WHERE MaNhaCungCap = @MaNCC";
            var p = new SqlParameter[] { new SqlParameter("@MaNCC", maNcc) };
            DataTable dt = DataService.ExecuteQuery(query, p);
            if (dt.Rows.Count > 0)
                return MapRowToNhaCungCap(dt.Rows[0]);
            return null;
        }

        // ==================== PHIẾU NHẬP ====================

        public List<PhieuNhap> GetAllPhieuNhap()
        {
            var list = new List<PhieuNhap>();
            string query = @"SELECT pn.*, ncc.TenNhaCungCap, ncc.SoDienThoai
                             FROM tblPhieuNhap pn
                             INNER JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
                             ORDER BY pn.NgayNhap DESC";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToPhieuNhap(row));
            return list;
        }

        public List<PhieuNhap> GetPhieuNhapByDate(DateTime tuNgay, DateTime denNgay)
        {
            var list = new List<PhieuNhap>();
            string query = @"SELECT pn.*, ncc.TenNhaCungCap, ncc.SoDienThoai
                             FROM tblPhieuNhap pn
                             INNER JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
                             WHERE pn.NgayNhap >= @TuNgay AND pn.NgayNhap < @DenNgay
                             ORDER BY pn.NgayNhap DESC";
            var p = new SqlParameter[] {
                new SqlParameter("@TuNgay", tuNgay),
                new SqlParameter("@DenNgay", denNgay)
            };
            DataTable dt = DataService.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToPhieuNhap(row));
            return list;
        }

        // ==================== CHI TIẾT PHIẾU NHẬP ====================

        public List<ChiTietPhieuNhap> GetChiTietByPhieuNhap(int maPhieuNhap)
        {
            var list = new List<ChiTietPhieuNhap>();
            string query = "SELECT * FROM tblChiTietPhieuNhap WHERE MaPhieuNhap = @MaPN ORDER BY MaChiTiet";
            var p = new SqlParameter[] { new SqlParameter("@MaPN", maPhieuNhap) };
            DataTable dt = DataService.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToChiTiet(row));
            return list;
        }

        /// <summary>
        /// Lay phieu nhap kem chi tiet (full)
        /// </summary>
        public PhieuNhap GetPhieuNhapFull(int maPhieuNhap)
        {
            var pnList = new List<PhieuNhap>();
            string query = @"SELECT pn.*, ncc.TenNhaCungCap, ncc.SoDienThoai
                             FROM tblPhieuNhap pn
                             INNER JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
                             WHERE pn.MaPhieuNhap = @MaPN";
            var p = new SqlParameter[] { new SqlParameter("@MaPN", maPhieuNhap) };
            DataTable dt = DataService.ExecuteQuery(query, p);
            if (dt.Rows.Count == 0) return null;

            var pn = MapRowToPhieuNhap(dt.Rows[0]);
            pn.ChiTietList = GetChiTietByPhieuNhap(maPhieuNhap);
            return pn;
        }

        public List<PhieuNhap> GetPhieuNhapFullByDate(DateTime tuNgay, DateTime denNgay)
        {
            var list = new List<PhieuNhap>();
            string query = @"SELECT pn.*, ncc.TenNhaCungCap, ncc.SoDienThoai
                             FROM tblPhieuNhap pn
                             INNER JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
                             WHERE pn.NgayNhap >= @TuNgay AND pn.NgayNhap < @DenNgay
                             ORDER BY pn.NgayNhap DESC";
            var p = new SqlParameter[] {
                new SqlParameter("@TuNgay", tuNgay),
                new SqlParameter("@DenNgay", denNgay)
            };
            DataTable dt = DataService.ExecuteQuery(query, p);
            foreach (DataRow row in dt.Rows)
            {
                var pn = MapRowToPhieuNhap(row);
                pn.ChiTietList = GetChiTietByPhieuNhap(pn.MaPhieuNhap);
                list.Add(pn);
            }
            return list;
        }

        // ==================== MAP HELPERS ====================

        private NhaCungCap MapRowToNhaCungCap(DataRow row)
        {
            return new NhaCungCap
            {
                MaNhaCungCap = Convert.ToInt32(row["MaNhaCungCap"]),
                TenNhaCungCap = row["TenNhaCungCap"].ToString(),
                DiaChi = row["DiaChi"] != DBNull.Value ? row["DiaChi"].ToString() : null,
                SoDienThoai = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : null,
                Email = row["Email"] != DBNull.Value ? row["Email"].ToString() : null,
                GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : null
            };
        }

        private PhieuNhap MapRowToPhieuNhap(DataRow row)
        {
            return new PhieuNhap
            {
                MaPhieuNhap = Convert.ToInt32(row["MaPhieuNhap"]),
                MaNhaCungCap = Convert.ToInt32(row["MaNhaCungCap"]),
                NgayNhap = Convert.ToDateTime(row["NgayNhap"]),
                NguoiGiao = row["NguoiGiao"] != DBNull.Value ? row["NguoiGiao"].ToString() : null,
                NguoiNhan = row["NguoiNhan"] != DBNull.Value ? row["NguoiNhan"].ToString() : null,
                GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : null,
                TongTien = row["TongTien"] != DBNull.Value ? Convert.ToDecimal(row["TongTien"]) : 0,
                TenNhaCungCap = row["TenNhaCungCap"].ToString(),
                SoDienThoaiNCC = row["SoDienThoai"] != DBNull.Value ? row["SoDienThoai"].ToString() : null
            };
        }

        private ChiTietPhieuNhap MapRowToChiTiet(DataRow row)
        {
            return new ChiTietPhieuNhap
            {
                MaChiTiet = Convert.ToInt32(row["MaChiTiet"]),
                MaPhieuNhap = Convert.ToInt32(row["MaPhieuNhap"]),
                TenNguyenLieu = row["TenNguyenLieu"].ToString(),
                DonViTinh = row["DonViTinh"] != DBNull.Value ? row["DonViTinh"].ToString() : "Kg",
                SoLuong = Convert.ToDecimal(row["SoLuong"]),
                DonGia = Convert.ToDecimal(row["DonGia"]),
                ThanhTien = row["ThanhTien"] != DBNull.Value ? Convert.ToDecimal(row["ThanhTien"]) : 0
            };
        }
    }
}
