using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly du lieu khach hang - schema: tblKhachHang + tblHangThe
    /// </summary>
    public class KhachHangService
    {
        /// <summary>
        /// Co so cau query: JOIN voi tblHangThe de lay TenHang
        /// </summary>
        private const string SELECT_KH = @"
            SELECT kh.*, ht.TenHang
            FROM tblKhachHang kh
            LEFT JOIN tblHangThe ht ON kh.MaHang = ht.MaHang";

        public List<KhachHang> GetAll()
        {
            var list = new List<KhachHang>();
            string query = SELECT_KH + " ORDER BY kh.TenKhachHang";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        public KhachHang GetByPhone(string soDienThoai)
        {
            string query = SELECT_KH + " WHERE kh.SoDienThoai = @SoDienThoai";
            var parameters = new SqlParameter[] { new SqlParameter("@SoDienThoai", soDienThoai) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapRow(dt.Rows[0]);
            return null;
        }

        public KhachHang GetById(int maKhachHang)
        {
            string query = SELECT_KH + " WHERE kh.MaKhachHang = @MaKhachHang";
            var parameters = new SqlParameter[] { new SqlParameter("@MaKhachHang", maKhachHang) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapRow(dt.Rows[0]);
            return null;
        }

        public int Insert(KhachHang kh)
        {
            // Mac dinh MaHang = 1 (Thuong)
            string query = @"INSERT INTO tblKhachHang (TenKhachHang, SoDienThoai, NgayDangKy, TongDiemTichLuy, MaHang)
                             VALUES (@TenKhachHang, @SoDienThoai, GETDATE(), 0, 1);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@TenKhachHang", kh.TenKhachHang),
                new SqlParameter("@SoDienThoai", kh.SoDienThoai)
            };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Dang ky khach hang moi bang spDangKyKhachHangMoi
        /// </summary>
        public int DangKyKhachHangMoi(string ten, string soDienThoai)
        {
            string query = "spDangKyKhachHangMoi";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TenKhachHang", ten);
                cmd.Parameters.AddWithValue("@SoDienThoai", soDienThoai);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return Convert.ToInt32(dt.Rows[0]["MaKhachHang"]);
                    return 0;
                }
            }
        }

        public void CapNhatDiem(int maKhachHang, decimal tongTien)
        {
            // Dung spDongCa + logic tich diem ben SQL stored procedure
            // Hoac dung query cap nhat thu cong
            int diemCong = (int)Math.Floor(tongTien / 10000);

            string query = @"UPDATE tblKhachHang SET 
                             TongDiemTichLuy = TongDiemTichLuy + @DiemCong,
                             MaHang = CASE
                                 WHEN (TongDiemTichLuy + @DiemCong) >= 1000 THEN 4  -- KimCuong
                                 WHEN (TongDiemTichLuy + @DiemCong) >= 500  THEN 3  -- Vang
                                 WHEN (TongDiemTichLuy + @DiemCong) >= 100  THEN 2  -- Bac
                                 ELSE 1  -- Thuong
                             END
                             WHERE MaKhachHang = @MaKhachHang";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaKhachHang", maKhachHang),
                new SqlParameter("@DiemCong", diemCong)
            };
            DataService.ExecuteNonQuery(query, parameters);
        }

        public int Update(KhachHang kh)
        {
            string query = @"UPDATE tblKhachHang SET 
                             TenKhachHang=@TenKhachHang, SoDienThoai=@SoDienThoai,
                             TongDiemTichLuy=@TongDiem, MaHang=(SELECT MaHang FROM tblHangThe WHERE TenHang=@HangThe)
                             WHERE MaKhachHang=@MaKhachHang";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaKhachHang", kh.MaKhachHang),
                new SqlParameter("@TenKhachHang", kh.TenKhachHang),
                new SqlParameter("@SoDienThoai", kh.SoDienThoai),
                new SqlParameter("@TongDiem", kh.TongDiemTichLuy),
                new SqlParameter("@HangThe", kh.HangThe)
            };
            return DataService.ExecuteNonQuery(query, parameters);
        }

        public int DemKhachMoiTrongCa(DateTime gioBatDau, DateTime gioKetThuc)
        {
            string query = "SELECT COUNT(*) FROM tblKhachHang WHERE NgayDangKy >= @Tu AND NgayDangKy <= @Den";
            var parameters = new SqlParameter[] {
                new SqlParameter("@Tu", gioBatDau),
                new SqlParameter("@Den", gioKetThuc)
            };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Tim kiem khach hang theo so dien thoai (LIKE keyword%)
        /// </summary>
        public List<KhachHang> SearchByPhone(string keyword)
        {
            var list = new List<KhachHang>();
            if (string.IsNullOrWhiteSpace(keyword)) return list;
            string query = SELECT_KH + " WHERE kh.SoDienThoai LIKE @Keyword + '%' ORDER BY kh.SoDienThoai";
            var parameters = new SqlParameter[] { new SqlParameter("@Keyword", keyword) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        /// <summary>
        /// Tinh tien giam gia dua tren hang the khach hang
        /// </summary>
        public decimal TinhTienGiamGia(KhachHang kh, decimal tongTien)
        {
            if (kh == null) return 0;
            double tiLeGiam = kh.LayTiLeGiamGia();
            return Math.Round(tongTien * (decimal)tiLeGiam, 0);
        }

        private KhachHang MapRow(DataRow row)
        {
            return new KhachHang
            {
                MaKhachHang = Convert.ToInt32(row["MaKhachHang"]),
                TenKhachHang = row["TenKhachHang"].ToString(),
                SoDienThoai = row["SoDienThoai"].ToString(),
                NgayDangKy = Convert.ToDateTime(row["NgayDangKy"]),
                TongDiemTichLuy = Convert.ToInt32(row["TongDiemTichLuy"]),
                MaHang = row["MaHang"] != DBNull.Value ? Convert.ToInt32(row["MaHang"]) : (int?)null,
                HangThe = row.Table.Columns.Contains("TenHang") && row["TenHang"] != DBNull.Value
                    ? row["TenHang"].ToString()
                    : "Thuong"
            };
        }
    }
}
