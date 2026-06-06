using System;
using System.Data;
using System.IO;
using System.Diagnostics;
using Nha_Hang_Huit.Helpers;

namespace Nha_Hang_Huit.Services
{
    /// <summary>
    /// Dich vu quan ly bao cao Crystal Reports cho nha hang.
    /// Dam bao pre-built .rpt files da co san tables + fields,
    /// chi can SetDataSource + Export/Print.
    /// </summary>
    public static class ReportService
    {
        private static readonly string ReportDir;
        private static readonly string CompiledDir;
        private static readonly string TemplatePath;

        static ReportService()
        {
            ReportDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report");
            CompiledDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CompiledReports");
            TemplatePath = FindTemplatePath();
        }

        private static string FindTemplatePath()
        {
            // Try multiple locations
            string[] candidates =
            {
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Report", "PhieuNhapReport.rpt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\Report\PhieuNhapReport.rpt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\Report\PhieuNhapReport.rpt"),
            };

            foreach (var c in candidates)
            {
                var full = Path.GetFullPath(c);
                if (File.Exists(full))
                    return full;
            }

            // Fallback: assume debug mode path
            var fallback = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                @"..\..\Report\PhieuNhapReport.rpt"));
            return File.Exists(fallback) ? fallback : candidates[0];
        }

        /// <summary>
        /// Kiem tra template co ton tai khong
        /// </summary>
        public static bool TemplateExists => File.Exists(TemplatePath);

        /// <summary>
        /// Pre-build .rpt voi table + fields tu DataTable schema.
        /// Tra ve path toi .rpt file da build.
        /// </summary>
        public static string BuildReportFromSchema(string reportName, DataTable schemaTable)
        {
            if (!Directory.Exists(CompiledDir))
                Directory.CreateDirectory(CompiledDir);

            string outputPath = Path.Combine(CompiledDir, reportName + ".rpt");

            // Check if already built
            if (File.Exists(outputPath))
            {
                Debug.WriteLine($"[ReportService] Reusing pre-built: {outputPath}");
                return outputPath;
            }

            if (!TemplateExists)
                throw new FileNotFoundException("Khong tim thay template .rpt", TemplatePath);

            using (var builder = new CrystalReportBuilder(TemplatePath))
            {
                builder.AddTable(schemaTable);
                builder.AddFields(schemaTable);
                builder.SaveRpt(outputPath);
            }

            Debug.WriteLine($"[ReportService] Built: {outputPath}");
            return outputPath;
        }

        /// <summary>
        /// Tao report cho PhieuNhap (don table).
        /// </summary>
        public static CrystalReportBuilder CreatePhieuNhapReport(DataTable chiTietTable)
        {
            var builder = new CrystalReportBuilder(TemplatePath);
            try
            {
                builder.AddTable(chiTietTable);
                builder.AddFields(chiTietTable);
                builder.AddText("NHÀ HÀNG HUIT", 0, 100, 10, 6000, 400);
                builder.AddText("01 Võ Văn Ngân, P. Linh Chiểu, TP. Thủ Đức", 0, 100, 50, 6000, 200);
                builder.AddText("PHIẾU NHẬP HÀNG", 0, 100, 100, 6000, 300);

                return builder;
            }
            catch
            {
                builder.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Tao report cho HoaDon (don table).
        /// </summary>
        public static CrystalReportBuilder CreateHoaDonReport(DataTable chiTietTable,
            string restaurantName = "NHÀ HÀNG HUIT",
            string address = "01 Võ Văn Ngân, P. Linh Chiểu, TP. Thủ Đức")
        {
            var builder = new CrystalReportBuilder(TemplatePath);
            try
            {
                builder.AddTable(chiTietTable);
                builder.AddFields(chiTietTable);
                builder.AddText(restaurantName, 0, 100, 10, 6000, 400);
                builder.AddText(address, 0, 100, 50, 6000, 200);
                builder.AddText("HÓA ĐƠN THANH TOÁN", 0, 100, 100, 6000, 300);

                return builder;
            }
            catch
            {
                builder.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Tao report cho PhieuNhap master-detail (2 tables).
        /// Master table: header info (single row)
        /// Detail table: line items
        /// </summary>
        public static CrystalReportBuilder CreatePhieuNhapMasterDetail(DataTable headerTable, DataTable detailTable)
        {
            var builder = new CrystalReportBuilder(TemplatePath);
            try
            {
                // Add both tables
                builder.AddTable(headerTable);
                builder.AddTable(detailTable);

                // Add header fields
                foreach (DataColumn col in headerTable.Columns)
                    builder.AddField($"{{{headerTable.TableName}.{col.ColumnName}}}", col.ColumnName);

                // Add detail fields
                foreach (DataColumn col in detailTable.Columns)
                    builder.AddField($"{{{detailTable.TableName}.{col.ColumnName}}}", col.ColumnName);

                builder.AddText("NHÀ HÀNG HUIT", 0, 100, 10, 6000, 400);
                builder.AddText("01 Võ Văn Ngân, P. Linh Chiểu, TP. Thủ Đức", 0, 100, 50, 6000, 200);
                builder.AddText("PHIẾU NHẬP HÀNG", 0, 100, 100, 6000, 300);

                return builder;
            }
            catch
            {
                builder.Dispose();
                throw;
            }
        }

        /// <summary>
        /// Xuat PDF truc tiep
        /// </summary>
        public static string ExportToPdf(CrystalReportBuilder builder, string outputDir, string fileName)
        {
            if (!Directory.Exists(outputDir))
                Directory.CreateDirectory(outputDir);

            string outputPath = Path.Combine(outputDir, fileName);
            builder.ExportToPdf(outputPath);
            return outputPath;
        }
    }
}
