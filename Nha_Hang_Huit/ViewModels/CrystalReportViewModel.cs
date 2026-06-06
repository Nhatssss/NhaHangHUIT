using System;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Input;
using Nha_Hang_Huit.Helpers;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho in hoa don / phieu nhap bang Crystal Reports builder.
    /// Hỗ trợ: xem truoc trong viewer, xuat PDF, in truc tiep.
    /// </summary>
    public class CrystalReportViewModel : BaseViewModel
    {
        private readonly CrystalReportBuilder _builder;
        private string _reportTitle;
        private string _pdfPath;
        private bool _dataLoaded;

        public CrystalReportBuilder Builder => _builder;
        public bool HasData => _dataLoaded;

        public string ReportTitle
        {
            get => _reportTitle;
            set => SetProperty(ref _reportTitle, value);
        }

        public string PdfPath
        {
            get => _pdfPath;
            set => SetProperty(ref _pdfPath, value);
        }

        /// <summary>
        /// Khoi tao ViewModel voi builder da co data.
        /// </summary>
        public CrystalReportViewModel(CrystalReportBuilder builder)
        {
            _builder = builder ?? throw new ArgumentNullException(nameof(builder));
            _dataLoaded = true;
            ReportTitle = "Crystal Report";
        }

        /// <summary>
        /// Tao PhieuNhap report tu ma phieu
        /// </summary>
        public static CrystalReportViewModel FromPhieuNhap(int maPhieuNhap, string connectionString)
        {
            var dt = LoadChiTietPhieuNhap(maPhieuNhap, connectionString);
            if (dt == null || dt.Rows.Count == 0)
                throw new InvalidOperationException($"Không có dữ liệu cho phiếu nhập PN-{maPhieuNhap:D5}");

            // Tao builder + add table + fields + text
            var builder = new CrystalReportBuilder(ReportService.TemplatePath);
            try
            {
                builder.AddTable(dt);
                builder.AddFields(dt);
                builder.AddText("NHÀ HÀNG HUIT", 0, 100, 10, 6000, 400);
                builder.AddText($"PHIẾU NHẬP PN-{maPhieuNhap:D5}", 0, 100, 100, 6000, 300);
                builder.SetDataSource(dt);

                var vm = new CrystalReportViewModel(builder)
                {
                    ReportTitle = $"Phiếu nhập PN-{maPhieuNhap:D5}"
                };
                return vm;
            }
            catch
            {
                builder.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Load data, SetDataSource, ReadRecords
        /// </summary>
        public void LoadData(DataTable dataTable)
        {
            if (dataTable == null) return;
            _builder.SetDataSource(dataTable);
            _builder.ReadRecords();
            _dataLoaded = true;
        }

        /// <summary>
        /// Xuat PDF
        /// </summary>
        public string ExportToPdf(string outputDir = null)
        {
            if (!_dataLoaded)
                throw new InvalidOperationException("Chua co data. Goi LoadData truoc.");

            if (outputDir == null)
                outputDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                    "HUIT_Reports");

            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
            string path = Path.Combine(outputDir, fileName);
            _builder.ExportToPdf(path);
            PdfPath = path;
            return path;
        }

        /// <summary>
        /// In truc tiep
        /// </summary>
        public void Print(int copies = 1)
        {
            if (!_dataLoaded) return;
            _builder.Print(copies);
        }

        private static DataTable LoadChiTietPhieuNhap(int maPhieuNhap, string connectionString)
        {
            var dt = new DataTable("ChiTietPhieuNhap");
            dt.Columns.Add("STT", typeof(int));
            dt.Columns.Add("TenNguyenLieu", typeof(string));
            dt.Columns.Add("DonViTinh", typeof(string));
            dt.Columns.Add("SoLuong", typeof(decimal));
            dt.Columns.Add("DonGia", typeof(decimal));
            dt.Columns.Add("ThanhTien", typeof(decimal));

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = @"
                        SELECT ROW_NUMBER() OVER (ORDER BY ct.MaChiTiet) AS STT,
                               ct.TenNguyenLieu, ct.DonViTinh, ct.SoLuong, ct.DonGia,
                               (ct.SoLuong * ct.DonGia) AS ThanhTien
                        FROM tblChiTietPhieuNhap ct
                        WHERE ct.MaPhieuNhap = @MaPN
                        ORDER BY ct.MaChiTiet";

                    using (var cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPhieuNhap);
                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var row = dt.NewRow();
                                row["STT"] = Convert.ToInt32(reader["STT"]);
                                row["TenNguyenLieu"] = reader["TenNguyenLieu"] as string ?? "";
                                row["DonViTinh"] = reader["DonViTinh"] as string ?? "";
                                row["SoLuong"] = Convert.ToDecimal(reader["SoLuong"]);
                                row["DonGia"] = Convert.ToDecimal(reader["DonGia"]);
                                row["ThanhTien"] = Convert.ToDecimal(reader["ThanhTien"]);
                                dt.Rows.Add(row);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CrystalReportViewModel] Load error: {ex.Message}");
            }

            return dt;
        }
    }
}
