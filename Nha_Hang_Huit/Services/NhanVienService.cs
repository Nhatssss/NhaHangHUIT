using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Helpers;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly dang nhap / phan quyen nhan vien
    /// Schema: tblNhanVien + spDangNhapNhanVien
    /// </summary>
    public class NhanVienService
    {
        /// <summary>
        /// Kiem tra dang nhap bang spDangNhapNhanVien
        /// Mat khau duoc hash SHA256 truoc khi truyen xuong DB
        /// Tra ve thong tin nhan vien neu dung
        /// </summary>
        public NhanVien DangNhap(string taiKhoan, string matKhau)
        {
            string matKhauHash = SecurityHelper.HashMatKhau(matKhau);

            string query = "spDangNhapNhanVien";
            using (var conn = DataService.GetConnection())
            using (var cmd = new SqlCommand(query, conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@TaiKhoan", taiKhoan);
                cmd.Parameters.AddWithValue("@MatKhau", matKhauHash);

                using (var da = new SqlDataAdapter(cmd))
                {
                    var dt = new DataTable();
                    da.Fill(dt);
                    if (dt.Rows.Count == 0) return null;

                    var row = dt.Rows[0];
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
        /// Lay danh sach tat ca nhan vien
        /// </summary>
        public List<NhanVien> GetAll()
        {
            var result = new List<NhanVien>();
            string query = "SELECT * FROM tblNhanVien ORDER BY HoTen";
            DataTable dt = DataService.ExecuteQuery(query);

            foreach (DataRow row in dt.Rows)
            {
                result.Add(new NhanVien
                {
                    MaNhanVien = Convert.ToInt32(row["MaNhanVien"]),
                    HoTen = row["HoTen"].ToString(),
                    TaiKhoan = row["TaiKhoan"].ToString(),
                    ChucVu = row["ChucVu"].ToString(),
                    TrangThai = Convert.ToBoolean(row["TrangThai"])
                });
            }
            return result;
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
