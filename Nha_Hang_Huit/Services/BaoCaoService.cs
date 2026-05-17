using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly bao cao dong ca - schema: tblCa + spMoCa/spDongCa
    /// </summary>
    public class BaoCaoService
    {
        private readonly HoaDonService _hoaDonService = new HoaDonService();
        private readonly KhachHangService _khachHangService = new KhachHangService();

        /// <summary>
        /// Mo ca moi bang spMoCa
        /// spMoCa co @MaCaMoi INT OUTPUT, khong tra ve result set
        /// </summary>
        public int MoCa(int maNhanVien)
        {
            int? caHienTai = _hoaDonService.GetCurrentCa();
            if (caHienTai.HasValue)
                return caHienTai.Value;

            string query = "spMoCa";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaNhanVien", maNhanVien);
                var maCaParam = cmd.Parameters.Add("@MaCaMoi", SqlDbType.Int);
                maCaParam.Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                if (maCaParam.Value != DBNull.Value)
                    return Convert.ToInt32(maCaParam.Value);
                return 0;
            }
        }

        /// <summary>
        /// Dong ca bang spDongCa (tra ve 2 result set)
        /// </summary>
        public LichSuCa DongCa(int maCa)
        {
            var caInfo = GetCaInfo(maCa);
            if (caInfo == null) return null;

            // Goi spDongCa de thuc hien dong ca
            string query = "spDongCa";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCa", maCa);

                using (var reader = cmd.ExecuteReader())
                {
                    // Result set 1: Tong hop ca
                    if (reader.Read())
                    {
                        caInfo.GioKetThuc = reader["ThoiGianDongCa"] != DBNull.Value
                            ? Convert.ToDateTime(reader["ThoiGianDongCa"]) : (DateTime?)null;
                        caInfo.TongSoHoaDon = Convert.ToInt32(reader["TongHoaDon"]);
                        caInfo.TongDoanhThuTruocGiamGia = Convert.ToDecimal(reader["TongDoanhThu"]);
                        caInfo.TongTienGiamGia = Convert.ToDecimal(reader["TongGiamGia"]);
                        caInfo.TongDoanhThuThucNhan = Convert.ToDecimal(reader["DoanhThuThucNhan"]);
                        caInfo.SoKhachMoi = Convert.ToInt32(reader["KhachMoiTrongNgay"]);
                        caInfo.NhanVien = reader["TenNhanVien"].ToString();
                    }
                }
            }

            return caInfo;
        }

        /// <summary>
        /// Lay thong tin ca tu tblCa + JOIN NhanVien
        /// </summary>
        public LichSuCa GetCaInfo(int maCa)
        {
            string query = @"SELECT ca.*, nv.HoTen AS TenNhanVien
                             FROM tblCa ca
                             INNER JOIN tblNhanVien nv ON ca.MaNhanVien = nv.MaNhanVien
                             WHERE ca.MaCa = @MaCa";
            var parameters = new SqlParameter[] { new SqlParameter("@MaCa", maCa) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new LichSuCa
            {
                MaCa = Convert.ToInt32(row["MaCa"]),
                GioBatDau = Convert.ToDateTime(row["ThoiGianMoCa"]),
                GioKetThuc = row["ThoiGianDongCa"] != DBNull.Value
                    ? (DateTime?)Convert.ToDateTime(row["ThoiGianDongCa"]) : null,
                NhanVien = row["TenNhanVien"].ToString(),
                TongSoHoaDon = Convert.ToInt32(row["TongHoaDon"]),
                TongDoanhThuTruocGiamGia = Convert.ToDecimal(row["TongDoanhThu"]),
                TongTienGiamGia = Convert.ToDecimal(row["TongGiamGia"]),
                TongDoanhThuThucNhan = Convert.ToDecimal(row["DoanhThuThucNhan"]),
                SoKhachMoi = 0, // se cap nhat sau khi dong ca
                GhiChu = row["GhiChu"] != DBNull.Value ? row["GhiChu"].ToString() : null
            };
        }

        /// <summary>
        /// Lay top mon ban chay (su dung spDongCa result set 2 hoac spThongKeTopMonBanChay)
        /// </summary>
        public DataTable GetTopMonBanChay(int maCa)
        {
            string query = @"SELECT TOP 5
                ct.TenMonAn,
                SUM(ct.SoLuong) AS TongSoLuong,
                SUM(ct.ThanhTien) AS TongDoanhThu
                FROM tblChiTietHoaDon ct
                INNER JOIN tblHoaDon hd ON ct.MaHoaDon = hd.MaHoaDon
                WHERE hd.MaCa = @MaCa AND hd.TrangThai = N'DaThanhToan'
                GROUP BY ct.TenMonAn
                ORDER BY TongSoLuong DESC";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaCa", maCa);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    return dt;
                }
            }
        }

        /// <summary>
        /// GetTopMon tu result set 2 cua spDongCa
        /// </summary>
        public DataTable GetTopMonTuSpDongCa(int maCa)
        {
            string query = "spDongCa";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@MaCa", maCa);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var ds = new DataSet();
                    da.Fill(ds);
                    // Result set 1 = ca summary, Result set 2 = top 5
                    if (ds.Tables.Count >= 2)
                        return ds.Tables[1];
                    return new DataTable();
                }
            }
        }

        public bool XuatBaoCaoTxt(LichSuCa ca, DataTable topMon, string filePath)
        {
            using (var writer = new StreamWriter(filePath, false, System.Text.Encoding.UTF8))
            {
                writer.WriteLine("============================================");
                writer.WriteLine("     BAO CAO DONG CA - NHA HANG HUIT");
                writer.WriteLine("============================================");
                writer.WriteLine();
                writer.WriteLine($"Ngay:            {ca.GioBatDau:dd/MM/yyyy}");
                writer.WriteLine($"Gio bat dau:     {ca.GioBatDau:HH:mm:ss}");
                writer.WriteLine($"Gio ket thuc:    {ca.GioKetThuc:HH:mm:ss}");
                writer.WriteLine($"Nhan vien:       {ca.NhanVien}");
                writer.WriteLine($"Ma Ca:           #{ca.MaCa}");
                writer.WriteLine();
                writer.WriteLine("--- THONG KE ---");
                writer.WriteLine($"Tong so hoa don:               {ca.TongSoHoaDon}");
                writer.WriteLine($"Doanh thu truoc giam gia:      {ca.TongDoanhThuTruocGiamGia:N0} VND");
                writer.WriteLine($"Tong tien giam gia:            {ca.TongTienGiamGia:N0} VND");
                writer.WriteLine($"Doanh thu thuc nhan:           {ca.TongDoanhThuThucNhan:N0} VND");
                writer.WriteLine($"So khach hang moi dang ky:     {ca.SoKhachMoi}");
                writer.WriteLine();
                writer.WriteLine("--- TOP 5 MON BAN CHAY ---");
                if (topMon != null && topMon.Rows.Count > 0)
                {
                    int stt = 1;
                    foreach (DataRow row in topMon.Rows)
                    {
                        string tenMon = row["TenMonAn"].ToString();
                        int soLuong = Convert.ToInt32(row["TongSoLuong"]);
                        decimal doanhThu = Convert.ToDecimal(row["TongDoanhThu"]);
                        writer.WriteLine($"  {stt}. {tenMon} - {soLuong} phan - {doanhThu:N0} VND");
                        stt++;
                    }
                }
                else
                    writer.WriteLine("  (Khong co du lieu)");
                writer.WriteLine();
                writer.WriteLine("============================================");
                writer.WriteLine("  Phan mem quan ly nha hang Nha_Hang_Huit");
                writer.WriteLine("============================================");
            }
            return true;
        }
    }
}
