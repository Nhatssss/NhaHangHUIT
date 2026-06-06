using CrystalDecisions.CrystalReports.Engine;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace Nha_Hang_Huit.Views
{
    public partial class PhieuNhapCrystalReportView : UserControl
    {
        private int _maPhieuNhap;
        private string _connectionString;

        public PhieuNhapCrystalReportView(int maPhieuNhap)
        {
            InitializeComponent();
            _maPhieuNhap = maPhieuNhap;
            _connectionString = DataService.GetConnectionString();
            Loaded += OnLoaded;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // Build the path to the .rpt in Report folder
                string reportPath = System.IO.Path.Combine(
                    AppDomain.CurrentDomain.BaseDirectory,
                    @"..\..\Report\PhieuNhapReport.rpt");
                reportPath = System.IO.Path.GetFullPath(reportPath);

                if (!File.Exists(reportPath))
                {
                    MessageBox.Show("Không tìm thấy file báo cáo: " + reportPath,
                        "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Load report
                ReportDocument report = new ReportDocument();
                report.Load(reportPath);

                // Create DataSet from database and set as report data source
                DataSet ds = LoadPhieuNhapData();
                if (ds.Tables.Count > 0)
                    report.SetDataSource(ds);

                // Assign to viewer
                crystalReportViewer.ReportSource = report;
                crystalReportViewer.Refresh();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tạo báo cáo: " + ex.Message + "\n\n" + ex.StackTrace,
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DataSet LoadPhieuNhapData()
        {
            DataSet ds = new DataSet();

            try
            {
                // 1. Header table
                DataTable headerTable = new DataTable("PhieuNhapHeader");
                headerTable.Columns.Add("MaPhieuNhap", typeof(int));
                headerTable.Columns.Add("NgayNhap", typeof(string));
                headerTable.Columns.Add("NguoiGiao", typeof(string));
                headerTable.Columns.Add("NguoiNhan", typeof(string));
                headerTable.Columns.Add("TongTien", typeof(decimal));
                headerTable.Columns.Add("TenNCC", typeof(string));
                headerTable.Columns.Add("SoDienThoaiNCC", typeof(string));
                headerTable.Columns.Add("TongTienBangChu", typeof(string));

                // 2. Detail items table
                DataTable detailTable = new DataTable("ChiTietPhieuNhap");
                detailTable.Columns.Add("STT", typeof(int));
                detailTable.Columns.Add("TenNguyenLieu", typeof(string));
                detailTable.Columns.Add("DonViTinh", typeof(string));
                detailTable.Columns.Add("SoLuong", typeof(decimal));
                detailTable.Columns.Add("DonGia", typeof(decimal));
                detailTable.Columns.Add("ThanhTien", typeof(decimal));

                using (SqlConnection conn = new SqlConnection(_connectionString))
                {
                    conn.Open();

                    // Header query
                    string sqlHeader = @"
                        SELECT pn.MaPhieuNhap, pn.NgayNhap, pn.NguoiGiao, pn.NguoiNhan, pn.TongTien,
                               ncc.TenNhaCungCap, ncc.SoDienThoai
                        FROM tblPhieuNhap pn
                        LEFT JOIN tblNhaCungCap ncc ON pn.MaNhaCungCap = ncc.MaNhaCungCap
                        WHERE pn.MaPhieuNhap = @MaPhieuNhap";

                    using (SqlCommand cmd = new SqlCommand(sqlHeader, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieuNhap", _maPhieuNhap);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                DataRow row = headerTable.NewRow();
                                row["MaPhieuNhap"] = Convert.ToInt32(reader["MaPhieuNhap"]);
                                row["NgayNhap"] = Convert.ToDateTime(reader["NgayNhap"]).ToString("dd/MM/yyyy HH:mm");
                                row["NguoiGiao"] = reader["NguoiGiao"] as string ?? "";
                                row["NguoiNhan"] = reader["NguoiNhan"] as string ?? "";
                                row["TongTien"] = Convert.ToDecimal(reader["TongTien"]);
                                row["TenNCC"] = reader["TenNhaCungCap"] as string ?? "";
                                row["SoDienThoaiNCC"] = reader["SoDienThoai"] as string ?? "";
                                row["TongTienBangChu"] = NumberToText(Convert.ToDecimal(reader["TongTien"]));
                                headerTable.Rows.Add(row);
                            }
                        }
                    }

                    // Detail items
                    string sqlDetail = @"
                        SELECT ROW_NUMBER() OVER (ORDER BY ct.MaChiTiet) AS STT,
                               ct.TenNguyenLieu, ct.DonViTinh, ct.SoLuong, ct.DonGia,
                               (ct.SoLuong * ct.DonGia) AS ThanhTien
                        FROM tblChiTietPhieuNhap ct
                        WHERE ct.MaPhieuNhap = @MaPhieuNhap
                        ORDER BY ct.MaChiTiet";

                    using (SqlCommand cmd = new SqlCommand(sqlDetail, conn))
                    {
                        cmd.Parameters.AddWithValue("@MaPhieuNhap", _maPhieuNhap);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                DataRow row = detailTable.NewRow();
                                row["STT"] = Convert.ToInt32(reader["STT"]);
                                row["TenNguyenLieu"] = reader["TenNguyenLieu"] as string ?? "";
                                row["DonViTinh"] = reader["DonViTinh"] as string ?? "";
                                row["SoLuong"] = Convert.ToDecimal(reader["SoLuong"]);
                                row["DonGia"] = Convert.ToDecimal(reader["DonGia"]);
                                row["ThanhTien"] = Convert.ToDecimal(reader["ThanhTien"]);
                                detailTable.Rows.Add(row);
                            }
                        }
                    }
                }

                ds.Tables.Add(headerTable);
                ds.Tables.Add(detailTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi tải dữ liệu: " + ex.Message);
            }

            return ds;
        }

        /// <summary>
        /// Convert number to Vietnamese text
        /// </summary>
        private string NumberToText(decimal number)
        {
            if (number == 0) return "Không đồng";

            string[] ones = { "", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };
            string[] tens = { "", "mười", "hai mươi", "ba mươi", "bốn mươi", "năm mươi", "sáu mươi", "bảy mươi", "tám mươi", "chín mươi" };
            string[] groups = { "", "nghìn", "triệu", "tỷ" };

            long integerPart = (long)Math.Floor(number);
            if (integerPart == 0) return "Không đồng";

            string result = "";
            int groupIndex = 0;

            while (integerPart > 0)
            {
                int groupValue = (int)(integerPart % 1000);
                integerPart /= 1000;

                if (groupValue > 0 || groupIndex == 0)
                {
                    string groupText = "";
                    int hundreds = groupValue / 100;
                    int ten = (groupValue % 100) / 10;
                    int one = groupValue % 10;

                    if (hundreds > 0)
                        groupText += ones[hundreds] + " trăm ";
                    else if (groupIndex > 0)
                        groupText += "không trăm ";

                    if (ten > 1)
                    {
                        groupText += tens[ten] + " ";
                        if (one > 0)
                            groupText += ones[one] + " ";
                    }
                    else if (ten == 1)
                    {
                        groupText += "mười ";
                        if (one > 0)
                            groupText += ones[one] + " ";
                    }
                    else if (groupIndex > 0 && one > 0)
                    {
                        groupText += "lẻ " + ones[one] + " ";
                    }
                    else if (one > 0)
                    {
                        groupText += ones[one] + " ";
                    }

                    if (groupValue > 0 && groups.Length > groupIndex)
                        result = groupText + groups[groupIndex] + " " + result;
                }

                groupIndex++;
            }

            result = result.Trim() + " đồng";
            result = char.ToUpper(result[0]) + result.Substring(1);
            return result;
        }
    }
}
