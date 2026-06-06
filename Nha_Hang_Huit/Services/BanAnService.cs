using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service quan ly ban an - schema: tblBanAn + tblKhuVuc
    /// </summary>
    public class BanAnService
    {
        /// <summary>
        /// Lay danh sach khu vuc
        /// </summary>
        public List<KhuVuc> GetAllKhuVuc()
        {
            var list = new List<KhuVuc>();
            string query = "SELECT * FROM tblKhuVuc ORDER BY ThuTuHienThi";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new KhuVuc
                {
                    MaKhuVuc = Convert.ToInt32(row["MaKhuVuc"]),
                    TenKhuVuc = row["TenKhuVuc"].ToString(),
                    MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
                    ThuTuHienThi = Convert.ToInt32(row["ThuTuHienThi"])
                });
            }
            return list;
        }

        /// <summary>
        /// Lay danh sach ban an theo khu vuc, kem thong tin hoa don dang mo
        /// </summary>
        public List<BanAn> GetBanAnByKhuVuc(int maKhuVuc)
        {
            var list = new List<BanAn>();
            string query = @"
                SELECT b.*, k.TenKhuVuc,
                    (SELECT COUNT(*) FROM tblHoaDon h
                     WHERE h.MaCa IN (SELECT MaCa FROM tblCa WHERE TrangThai = N'DangLam')
                       AND h.SoBan = b.MaBan AND h.TrangThai = N'ChuaTT') AS HoaDonDangMo,
                    ISNULL((
                        SELECT SUM(hd2.TongTienGoc)
                        FROM tblHoaDon hd2
                        WHERE hd2.MaCa IN (SELECT MaCa FROM tblCa WHERE TrangThai = N'DangLam')
                          AND hd2.SoBan = b.MaBan AND hd2.TrangThai = N'ChuaTT'
                    ), 0) AS TongTienTamTinh
                FROM tblBanAn b
                INNER JOIN tblKhuVuc k ON b.MaKhuVuc = k.MaKhuVuc
                WHERE b.MaKhuVuc = @MaKhuVuc
                ORDER BY b.Hang, b.Cot";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@MaKhuVuc", maKhuVuc);
                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    foreach (DataRow row in dt.Rows)
                    {
                        var ban = new BanAn
                        {
                            MaBan = Convert.ToInt32(row["MaBan"]),
                            TenBan = row["TenBan"].ToString(),
                            MaKhuVuc = maKhuVuc,
                            TenKhuVuc = row["TenKhuVuc"].ToString(),
                            SoChoNgoi = Convert.ToInt32(row["SoChoNgoi"]),
                            TrangThai = row["TrangThai"].ToString(),
                            Cot = Convert.ToInt32(row["Cot"]),
                            Hang = Convert.ToInt32(row["Hang"]),
                            SoHoaDonDangMo = Convert.ToInt32(row["HoaDonDangMo"]),
                            TongTienTamTinh = Convert.ToDecimal(row["TongTienTamTinh"])
                        };
                        // Tu dong cap nhat trang thai ban neu co hoa don dang mo
                        if (ban.SoHoaDonDangMo > 0 && ban.TrangThai == "Trong")
                            ban.TrangThai = "DangDung";
                        list.Add(ban);
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// Lay tat ca ban an (tat ca khu vuc)
        /// </summary>
        public List<BanAn> GetAllBanAn()
        {
            var all = new List<BanAn>();
            var kvList = GetAllKhuVuc();
            foreach (var kv in kvList)
                all.AddRange(GetBanAnByKhuVuc(kv.MaKhuVuc));
            return all;
        }

        /// <summary>
        /// Cap nhat trang thai ban
        /// </summary>
        public void CapNhatTrangThai(int maBan, string trangThai)
        {
            string query = "UPDATE tblBanAn SET TrangThai = @TrangThai WHERE MaBan = @MaBan";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaBan", maBan),
                new SqlParameter("@TrangThai", trangThai)
            };
            DataService.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Lay ban an theo MaBan
        /// </summary>
        public BanAn GetById(int maBan)
        {
            string query = @"
                SELECT b.*, k.TenKhuVuc
                FROM tblBanAn b
                INNER JOIN tblKhuVuc k ON b.MaKhuVuc = k.MaKhuVuc
                WHERE b.MaBan = @MaBan";
            var parameters = new SqlParameter[] { new SqlParameter("@MaBan", maBan) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new BanAn
            {
                MaBan = Convert.ToInt32(row["MaBan"]),
                TenBan = row["TenBan"].ToString(),
                MaKhuVuc = Convert.ToInt32(row["MaKhuVuc"]),
                TenKhuVuc = row["TenKhuVuc"].ToString(),
                SoChoNgoi = Convert.ToInt32(row["SoChoNgoi"]),
                TrangThai = row["TrangThai"].ToString(),
                Cot = Convert.ToInt32(row["Cot"]),
                Hang = Convert.ToInt32(row["Hang"])
            };
        }
    }
}
