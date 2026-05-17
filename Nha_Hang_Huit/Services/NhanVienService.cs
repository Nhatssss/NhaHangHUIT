using System;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly dang nhap / phan quyen nhan vien
    /// Schema: tblNhanVien (giong cu) + spDangNhapNhanVien
    /// </summary>
    public class NhanVienService
    {
        /// <summary>
        /// Kiem tra dang nhap bang spDangNhapNhanVien
        /// Tra ve thong tin nhan vien neu dung
        /// </summary>
        public NhanVien DangNhap(string taiKhoan, string matKhau)
        {
            // Dung stored procedure moi
            string query = "spDangNhapNhanVien";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
                cmd.Parameters.AddWithValue("@MatKhau", matKhau);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return null;

                    var row = dt.Rows[0];
                    // spDangNhapNhanVien chi SELECT MaNhanVien, HoTen, TaiKhoan, ChucVu
                    // (TrangThai da duoc loc = 1 trong SP nen luon true)
                    return new NhanVien
                    {
                        MaNhanVien = Convert.ToInt32(row["MaNhanVien"]),
                        HoTen = row["HoTen"].ToString(),
                        TaiKhoan = row["TaiKhoan"].ToString(),
                        ChucVu = row["ChucVu"].ToString(),
                        TrangThai = true
                    };
                }
            }
        }

        /// <summary>
        /// Lay nhan vien theo MaNhanVien (direct query)
        /// </summary>
        public NhanVien GetById(int maNhanVien)
        {
            string query = "SELECT * FROM tblNhanVien WHERE MaNhanVien = @MaNhanVien";
            var parameters = new SqlParameter[] { new SqlParameter("@MaNhanVien", maNhanVien) };

            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count == 0) return null;

            var row = dt.Rows[0];
            return new NhanVien
            {
                MaNhanVien = Convert.ToInt32(row["MaNhanVien"]),
                HoTen = row["HoTen"].ToString(),
                TaiKhoan = row["TaiKhoan"].ToString(),
                ChucVu = row["ChucVu"].ToString(),
                TrangThai = Convert.ToBoolean(row["TrangThai"])
            };
        }
    }
}
