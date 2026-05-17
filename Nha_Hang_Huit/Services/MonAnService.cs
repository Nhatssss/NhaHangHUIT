using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Nha_Hang_Huit.Models;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Service xu ly du lieu mon an - schema: tblMonAn + tblNhomMonAn
    /// </summary>
    public class MonAnService
    {
        /// <summary>
        /// Lay tat ca mon an dang ban (TrangThai = 1), JOIN voi NhomMonAn de lay TenNhom
        /// </summary>
        public List<MonAn> GetAll()
        {
            var list = new List<MonAn>();
            string query = @"SELECT ma.*, n.TenNhom 
                             FROM tblMonAn ma 
                             INNER JOIN tblNhomMonAn n ON ma.MaNhom = n.MaNhom 
                             WHERE n.TrangThai = 1 AND ma.TrangThai = 1
                             ORDER BY n.ThuTuHienThi, ma.TenMonAn";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        /// <summary>
        /// Lay mon an theo nhom (loc bang TenNhom de tuong thich nguoc)
        /// </summary>
        public List<MonAn> GetByNhom(string tenNhom)
        {
            var list = new List<MonAn>();
            string query = @"SELECT ma.*, n.TenNhom 
                             FROM tblMonAn ma 
                             INNER JOIN tblNhomMonAn n ON ma.MaNhom = n.MaNhom 
                             WHERE n.TenNhom = @TenNhom AND ma.TrangThai = 1
                             ORDER BY ma.TenMonAn";
            var parameters = new SqlParameter[] { new SqlParameter("@TenNhom", tenNhom) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            foreach (DataRow row in dt.Rows)
                list.Add(MapRow(row));
            return list;
        }

        /// <summary>
        /// Lay danh sach nhom mon tu tblNhomMonAn (thay cho SELECT DISTINCT NhomMon)
        /// </summary>
        public List<string> GetAllNhomMon()
        {
            var list = new List<string>();
            string query = "SELECT TenNhom FROM tblNhomMonAn WHERE TrangThai = 1 ORDER BY ThuTuHienThi";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
                list.Add(row["TenNhom"].ToString());
            return list;
        }

        /// <summary>
        /// Lay chi tiet nhom mon de dung combo box
        /// </summary>
        public List<NhomMonAn> GetAllNhomMonAn()
        {
            var list = new List<NhomMonAn>();
            string query = "SELECT * FROM tblNhomMonAn WHERE TrangThai = 1 ORDER BY ThuTuHienThi";
            DataTable dt = DataService.ExecuteQuery(query);
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new NhomMonAn
                {
                    MaNhom = Convert.ToInt32(row["MaNhom"]),
                    TenNhom = row["TenNhom"].ToString(),
                    MoTaNhom = row["MoTaNhom"] != DBNull.Value ? row["MoTaNhom"].ToString() : null,
                    ThuTuHienThi = Convert.ToInt32(row["ThuTuHienThi"]),
                    TrangThai = Convert.ToBoolean(row["TrangThai"])
                });
            }
            return list;
        }

        public MonAn GetById(int maMonAn)
        {
            string query = @"SELECT ma.*, n.TenNhom 
                             FROM tblMonAn ma 
                             INNER JOIN tblNhomMonAn n ON ma.MaNhom = n.MaNhom 
                             WHERE ma.MaMonAn = @MaMonAn";
            var parameters = new SqlParameter[] { new SqlParameter("@MaMonAn", maMonAn) };
            DataTable dt = DataService.ExecuteQuery(query, parameters);
            if (dt.Rows.Count > 0)
                return MapRow(dt.Rows[0]);
            return null;
        }

        public int Insert(MonAn monAn)
        {
            // Tim MaNhom tu TenNhom
            int maNhom = GetMaNhomByTen(monAn.NhomMon);
            if (maNhom == 0)
            {
                // Them nhom moi neu chua co
                maNhom = InsertNhomMonAn(monAn.NhomMon);
            }

            string query = @"INSERT INTO tblMonAn (MaNhom, TenMonAn, GiaBan, DonVi, MoTa, DuongDanHinh, TrangThai)
                             VALUES (@MaNhom, @TenMonAn, @GiaBan, N'Phan', @MoTa, @DuongDanHinh, @TrangThai);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaNhom", maNhom),
                new SqlParameter("@TenMonAn", monAn.TenMonAn),
                new SqlParameter("@GiaBan", monAn.Gia),
                new SqlParameter("@MoTa", (object)monAn.MoTa ?? DBNull.Value),
                new SqlParameter("@DuongDanHinh", (object)monAn.HinhAnh ?? DBNull.Value),
                new SqlParameter("@TrangThai", monAn.TrangThai == "Con" ? 1 : 0)
            };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        public int Update(MonAn monAn)
        {
            int maNhom = GetMaNhomByTen(monAn.NhomMon);
            if (maNhom == 0) maNhom = InsertNhomMonAn(monAn.NhomMon);

            string query = @"UPDATE tblMonAn SET 
                             MaNhom=@MaNhom, TenMonAn=@TenMonAn, GiaBan=@GiaBan,
                             MoTa=@MoTa, DuongDanHinh=@DuongDanHinh, TrangThai=@TrangThai
                             WHERE MaMonAn=@MaMonAn";
            var parameters = new SqlParameter[] {
                new SqlParameter("@MaMonAn", monAn.MaMonAn),
                new SqlParameter("@MaNhom", maNhom),
                new SqlParameter("@TenMonAn", monAn.TenMonAn),
                new SqlParameter("@GiaBan", monAn.Gia),
                new SqlParameter("@MoTa", (object)monAn.MoTa ?? DBNull.Value),
                new SqlParameter("@DuongDanHinh", (object)monAn.HinhAnh ?? DBNull.Value),
                new SqlParameter("@TrangThai", monAn.TrangThai == "Con" ? 1 : 0)
            };
            return DataService.ExecuteNonQuery(query, parameters);
        }

        public int Delete(int maMonAn)
        {
            // Soft delete: set TrangThai = 0 (Het)
            string query = "UPDATE tblMonAn SET TrangThai = 0 WHERE MaMonAn = @MaMonAn";
            var parameters = new SqlParameter[] { new SqlParameter("@MaMonAn", maMonAn) };
            return DataService.ExecuteNonQuery(query, parameters);
        }

        /// <summary>
        /// Lay MaNhom tu TenNhom
        /// </summary>
        private int GetMaNhomByTen(string tenNhom)
        {
            string query = "SELECT MaNhom FROM tblNhomMonAn WHERE TenNhom = @TenNhom";
            var parameters = new SqlParameter[] { new SqlParameter("@TenNhom", tenNhom) };
            object result = DataService.ExecuteScalar(query, parameters);
            return result != null ? Convert.ToInt32(result) : 0;
        }

        /// <summary>
        /// Them nhom mon an moi
        /// </summary>
        private int InsertNhomMonAn(string tenNhom)
        {
            string query = @"INSERT INTO tblNhomMonAn (TenNhom, ThuTuHienThi) VALUES (@TenNhom, 99);
                             SELECT SCOPE_IDENTITY();";
            var parameters = new SqlParameter[] { new SqlParameter("@TenNhom", tenNhom) };
            return Convert.ToInt32(DataService.ExecuteScalar(query, parameters));
        }

        /// <summary>
        /// Map DataRow sang MonAn model
        /// </summary>
        private MonAn MapRow(DataRow row)
        {
            return new MonAn
            {
                MaMonAn = Convert.ToInt32(row["MaMonAn"]),
                TenMonAn = row["TenMonAn"].ToString(),
                Gia = Convert.ToDecimal(row["GiaBan"]),
                NhomMon = row["TenNhom"].ToString(),
                HinhAnh = row["DuongDanHinh"] != DBNull.Value ? row["DuongDanHinh"].ToString() : null,
                MoTa = row["MoTa"] != DBNull.Value ? row["MoTa"].ToString() : null,
                TrangThai = Convert.ToBoolean(row["TrangThai"]) ? "Con" : "Het",
                // NgayTao: field not in new schema, keep default (DateTime.MinValue)
            };
        }
    }
}
