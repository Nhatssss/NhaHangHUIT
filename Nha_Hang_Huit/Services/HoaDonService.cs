using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly hoa don - schema: tblHoaDon (TongTienGoc, SoTienGiamGia, TongThanhToan,...)
    /// </summary>
    public class HoaDonService
    {
        // ==================== HOA DON ====================

        public HoaDon GetById(int maHoaDon)
        {
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.MaHoaDon = @MaHoaDon";
            var parameters = new SqlParameter[] { new SqlParameter("@MaHoaDon", maHoaDon) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapRowToHoaDon(dt.Rows[0]);
            return null;
        }

        public List<HoaDon> GetByCa(int maCa)
        {
            var list = new List<HoaDon>();
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.MaCa = @MaCa ORDER BY hd.ThoiGianTao";
            var parameters = new SqlParameter[] { new SqlParameter("@MaCa", maCa) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        public List<HoaDon> GetDaThanhToanByCa(int maCa)
        {
            var list = new List<HoaDon>();
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.MaCa = @MaCa AND hd.TrangThai = N'DaThanhToan'
                             ORDER BY hd.ThoiGianTao";
            var parameters = new SqlParameter[] { new SqlParameter("@MaCa", maCa) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Lay tat ca hoa don da thanh toan (khong phan biet ca)
        /// </summary>
        public List<HoaDon> GetAllDaThanhToan()
        {
            var list = new List<HoaDon>();
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.TrangThai = N'DaThanhToan'
                             ORDER BY hd.ThoiGianTao DESC";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Lay hoa don chua thanh toan cua ca hien tai
        /// </summary>
        public List<HoaDon> GetChuaThanhToanByCa(int maCa)
        {
            var list = new List<HoaDon>();
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.MaCa = @MaCa AND hd.TrangThai = N'ChuaTT'
                             ORDER BY hd.ThoiGianTao";
            var parameters = new SqlParameter[] { new SqlParameter("@MaCa", maCa) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Tao hoa don moi bang spTaoHoaDonMoi
        /// sp co @MaHoaDonMoi INT OUTPUT, khong tra ve result set
        /// </summary>
        public int Insert(HoaDon hd)
        {
            return Insert(hd, 0);
        }

        public int Insert(HoaDon hd, int soBan)
        {
            string query = "spTaoHoaDonMoi";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCa", hd.MaCa ?? 0);
                cmd.Parameters.AddWithValue("@MaKhachHang", (object)hd.MaKhachHang ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@SoBan", soBan);

                var maHdParam = cmd.Parameters.Add("@MaHoaDonMoi", SqlDbType.Int);
                maHdParam.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                if (maHdParam.Value != DBNull.Value)
                    return Convert.ToInt32(maHdParam.Value);
                return 0;
            }
        }

        /// <summary>
        /// Tao hoa don moi (direct INSERT - fallback)
        /// </summary>
        public int InsertDirect(HoaDon hd)
        {
            return InsertDirect(hd, 0);
        }

        public int InsertDirect(HoaDon hd, int soBan)
        {
            string query = @"INSERT INTO tblHoaDon (MaCa, MaKhachHang, SoBan, ThoiGianTao, TrangThai)
                             VALUES (@MaCa, @MaKhachHang, @SoBan, GETDATE(), N'ChuaTT');
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaCa", hd.MaCa ?? 0),
                new SqlParameter("@MaKhachHang", (object)hd.MaKhachHang ?? DBNull.Value),
                new SqlParameter("@SoBan", soBan)
            };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Xac nhan thanh toan bang spXacNhanThanhToan
        /// </summary>
        public int XacNhanThanhToan(int maHoaDon, decimal soTienKhachDua, string hinhThucTT = "TienMat", decimal tyLeGiamGia = 0)
        {
            string query = "spXacNhanThanhToan";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                cmd.Parameters.AddWithValue("@SoTienKhachDua", soTienKhachDua);
                cmd.Parameters.AddWithValue("@HinhThucTT", hinhThucTT);
                // Convert C# decimal 0.05 -> SP expects 5.00
                cmd.Parameters.AddWithValue("@TyLeGiamGia", tyLeGiamGia * 100);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                    {
                        // Ket qua tra ve: TongTienGoc, SoTienGiamGia, TongThanhToan, TienThua, DiemTichLuy
                        return 1;
                    }
                    return 0;
                }
            }
        }

        public int UpdateTrangThai(int maHoaDon, string trangThai, string phuongThucThanhToan)
        {
            string query = @"UPDATE tblHoaDon SET 
                             TrangThai=@TrangThai, HinhThucTT=@HinhThucTT,
                             ThoiGianThanhToan=GETDATE()
                             WHERE MaHoaDon=@MaHoaDon";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", maHoaDon),
                new SqlParameter("@TrangThai", trangThai),
                new SqlParameter("@HinhThucTT", (object)phuongThucThanhToan ?? DBNull.Value)
            };
            return DataService.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Lay hoa don chua thanh toan theo so ban
        /// </summary>
        public List<HoaDon> GetChuaThanhToanByBan(int soBan)
        {
            var list = new List<HoaDon>();
            string query = @"SELECT hd.*, kh.TenKhachHang, kh.SoDienThoai
                             FROM tblHoaDon hd
                             LEFT JOIN tblKhachHang kh ON hd.MaKhachHang = kh.MaKhachHang
                             WHERE hd.SoBan = @SoBan AND hd.TrangThai = N'ChuaTT'
                             ORDER BY hd.ThoiGianTao";
            var parameters = new SqlParameter[] { new SqlParameter("@SoBan", soBan) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToHoaDon(row));
            return list;
        }

        /// <summary>
        /// Tim ca dang mo trong tblCa (thay cho tblLichSuCa cu)
        /// </summary>
        public int? GetCurrentCa()
        {
            string query = "SELECT TOP 1 MaCa FROM tblCa WHERE TrangThai = N'DangLam' ORDER BY MaCa DESC";
            DataTable dt = DataService.ExecuteQuery(query);
            if (dt.Rows.Count > 0)
                return Convert.ToInt32(dt.Rows[0]["MaCa"]);
            return null;
        }

        // ==================== CHI TIET HOA DON ====================

        public List<ChiTietHoaDon> GetChiTietByHoaDon(int maHoaDon)
        {
            var list = new List<ChiTietHoaDon>();
            string query = "SELECT * FROM tblChiTietHoaDon WHERE MaHoaDon = @MaHoaDon";
            var parameters = new SqlParameter[] { new SqlParameter("@MaHoaDon", maHoaDon) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRowToChiTiet(row));
            return list;
        }

        /// <summary>
        /// Them chi tiet hoa don bang spThemChiTietHoaDon
        /// </summary>
        public int InsertChiTiet(ChiTietHoaDon ct)
        {
            string query = "spThemChiTietHoaDon";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaHoaDon", ct.MaHoaDon);
                cmd.Parameters.AddWithValue("@MaMonAn", ct.MaMonAn);
                cmd.Parameters.AddWithValue("@SoLuong", ct.SoLuong);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count > 0)
                        return Convert.ToInt32(dt.Rows[0]["MaChiTiet"]);
                    return 0;
                }
            }
        }

        /// <summary>
        /// Them chi tiet hoa don (direct INSERT - fallback)
        /// </summary>
        public int InsertChiTietDirect(ChiTietHoaDon ct)
        {
            string query = @"INSERT INTO tblChiTietHoaDon (MaHoaDon, MaMonAn, TenMonAn, SoLuong, DonGia)
                             VALUES (@MaHoaDon, @MaMonAn, @TenMonAn, @SoLuong, @DonGia);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaHoaDon", ct.MaHoaDon),
                new SqlParameter("@MaMonAn", ct.MaMonAn),
                new SqlParameter("@TenMonAn", ct.TenMonAn),
                new SqlParameter("@SoLuong", ct.SoLuong),
                new SqlParameter("@DonGia", ct.DonGia)
            };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Lay chi tiet qua spLayChiTietHoaDon
        /// </summary>
        public DataTable LayChiTietHoaDon(int maHoaDon)
        {
            string query = "spLayChiTietHoaDon";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaHoaDon", maHoaDon);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        private HoaDon MapRowToHoaDon(DataRow row)
        {
            return new HoaDon
            {
                MaHoaDon = Convert.ToInt32(row["MaHoaDon"]),
                MaKhachHang = row["MaKhachHang"] != DBNull.Value ? Convert.ToInt32(row["MaKhachHang"]) : (int?)null,
                NgayTao = Convert.ToDateTime(row["ThoiGianTao"]),
                TongTien = Convert.ToDecimal(row["TongTienGoc"]),
                TienGiamGia = Convert.ToDecimal(row["SoTienGiamGia"]),
                ThanhTien = Convert.ToDecimal(row["TongThanhToan"]),
                PhuongThucThanhToan = row["HinhThucTT"] != DBNull.Value ? row["HinhThucTT"].ToString() : null,
                TrangThai = row["TrangThai"].ToString(),
                MaCa = Convert.ToInt32(row["MaCa"]),
                // GhiChu = khong co trong DB
            };
        }

        private ChiTietHoaDon MapRowToChiTiet(DataRow row)
        {
            return new ChiTietHoaDon
            {
                MaChiTiet = Convert.ToInt32(row["MaChiTiet"]),
                MaHoaDon = Convert.ToInt32(row["MaHoaDon"]),
                MaMonAn = Convert.ToInt32(row["MaMonAn"]),
                TenMonAn = row["TenMonAn"].ToString(),
                SoLuong = Convert.ToInt32(row["SoLuong"]),
                DonGia = Convert.ToDecimal(row["DonGia"])
                // ThanhTien = computed (DonGia * SoLuong) - khong can load tu DB
            };
        }
    }
}
