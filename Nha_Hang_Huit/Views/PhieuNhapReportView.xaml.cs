using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.ViewModels;

namespace Nha_Hang_Huit.Views
{
    /// <summary>
    /// Window in phieu nhap hang (FixedDocument + DocumentViewer)
    /// </summary>
    public partial class PhieuNhapReportView : Window
    {
        private readonly PhieuNhap _phieuNhap;

        public PhieuNhapReportView(PhieuNhap phieuNhap)
        {
            InitializeComponent();
            _phieuNhap = phieuNhap ?? throw new ArgumentNullException(nameof(phieuNhap));

            var vm = new PhieuNhapReportViewModel(phieuNhap);
            DataContext = vm;

            this.Title = $"Phiếu nhập PN-{phieuNhap.MaPhieuNhap:D5}";

            Loaded += (s, e) => BuildReport();
        }

        private void BuildReport()
        {
            var doc = new FixedDocument();
            doc.DocumentPaginator.PageSize = new Size(96 * 8.27, 96 * 11.69); // A4

            var page = new FixedPage();
            page.Width = doc.DocumentPaginator.PageSize.Width;
            page.Height = doc.DocumentPaginator.PageSize.Height;

            // Margins
            double margin = 48; // 0.5 inch
            double x = margin;
            double y = margin;
            double w = page.Width - 2 * margin;


            // === HEADER ===
            // Restaurant name
            var tbTitle = MakeText("NHÀ HÀNG HUIT", 24, FontWeights.Bold, Brushes.DarkGreen);
            FixedPage.SetLeft(tbTitle, x);
            FixedPage.SetTop(tbTitle, y);
            page.Children.Add(tbTitle);
            y += 30;

            var tbAddr = MakeText("01 Võ Văn Ngân, P. Linh Chiểu, TP. Thủ Đức", 11, FontWeights.Normal, Brushes.Gray);
            FixedPage.SetLeft(tbAddr, x);
            FixedPage.SetTop(tbAddr, y);
            page.Children.Add(tbAddr);
            y += 16;

            var tbPhone = MakeText("ĐT: 0901 234 567 - 0909 888 777", 11, FontWeights.Normal, Brushes.Gray);
            FixedPage.SetLeft(tbPhone, x);
            FixedPage.SetTop(tbPhone, y);
            page.Children.Add(tbPhone);
            y += 26;

            // Divider line
            page.Children.Add(MakeLine(x, y, w));
            y += 12;

            // Report title
            var tbReportTitle = MakeText("PHIẾU NHẬP HÀNG", 20, FontWeights.Bold, Brushes.DarkGreen, TextAlignment.Center);
            FixedPage.SetLeft(tbReportTitle, (page.Width - tbReportTitle.DesiredSize.Width) / 2);
            FixedPage.SetTop(tbReportTitle, y);
            page.Children.Add(tbReportTitle);
            y += 28;

            // So phieu
            var tbSoPhieu = MakeText($"Số phiếu: PN-{_phieuNhap.MaPhieuNhap:D5}", 13, FontWeights.SemiBold, Brushes.Black);
            FixedPage.SetLeft(tbSoPhieu, x);
            FixedPage.SetTop(tbSoPhieu, y);
            page.Children.Add(tbSoPhieu);

            var tbNgay = MakeText($"Ngày nhập: {_phieuNhap.NgayNhap:dd/MM/yyyy HH:mm}", 13, FontWeights.Normal, Brushes.Black);
            FixedPage.SetLeft(tbNgay, x + w - 200);
            FixedPage.SetTop(tbNgay, y);
            page.Children.Add(tbNgay);
            y += 24;

            // === SUPPLIER INFO ===
            page.Children.Add(MakeLine(x, y, w));
            y += 8;

            var tbNCC = MakeText($"Nhà cung cấp: {_phieuNhap.TenNhaCungCap}", 13, FontWeights.Bold, Brushes.Black);
            FixedPage.SetLeft(tbNCC, x);
            FixedPage.SetTop(tbNCC, y);
            page.Children.Add(tbNCC);
            y += 20;

            if (!string.IsNullOrEmpty(_phieuNhap.SoDienThoaiNCC))
            {
                var tbSDT = MakeText($"SĐT: {_phieuNhap.SoDienThoaiNCC}", 12, FontWeights.Normal, Brushes.Gray);
                FixedPage.SetLeft(tbSDT, x);
                FixedPage.SetTop(tbSDT, y);
                page.Children.Add(tbSDT);
                y += 18;
            }

            if (!string.IsNullOrEmpty(_phieuNhap.NguoiGiao))
            {
                var tbGiao = MakeText($"Người giao: {_phieuNhap.NguoiGiao}", 12, FontWeights.Normal, Brushes.Gray);
                FixedPage.SetLeft(tbGiao, x);
                FixedPage.SetTop(tbGiao, y);
                page.Children.Add(tbGiao);
                y += 18;
            }

            if (!string.IsNullOrEmpty(_phieuNhap.NguoiNhan))
            {
                var tbNhan = MakeText($"Người nhận: {_phieuNhap.NguoiNhan}", 12, FontWeights.Normal, Brushes.Gray);
                FixedPage.SetLeft(tbNhan, x);
                FixedPage.SetTop(tbNhan, y);
                page.Children.Add(tbNhan);
                y += 18;
            }

            y += 6;
            page.Children.Add(MakeLine(x, y, w));
            y += 10;

            // === TABLE HEADER ===
            double[] colWidths = { 40, w * 0.32, 50, 65, 80, 100 };
            string[] colHeaders = { "STT", "Nguyên liệu", "ĐVT", "Số lượng", "Đơn giá", "Thành tiền" };
            double tableX = x + 10;

            var headerBg = new System.Windows.Shapes.Rectangle
            {
                Width = w - 20,
                Height = 28,
                Fill = new SolidColorBrush(Color.FromRgb(46, 125, 50))
            };
            FixedPage.SetLeft(headerBg, x + 10);
            FixedPage.SetTop(headerBg, y);
            page.Children.Add(headerBg);

            double cx = tableX;
            for (int i = 0; i < colHeaders.Length; i++)
            {
                var tb = MakeText(colHeaders[i], 10, FontWeights.Bold, Brushes.White);
                FixedPage.SetLeft(tb, cx + 4);
                FixedPage.SetTop(tb, y + 4);
                page.Children.Add(tb);
                cx += colWidths[i];
            }
            y += 28;

            // === TABLE ROWS ===
            double startY = y;
            int stt = 0;
            foreach (var ct in _phieuNhap.ChiTietList)
            {
                stt++;
                double rowH = 22;
                cx = tableX;

                if (y + rowH > page.Height - margin)
                {
                    // Add page to doc and create new page
                    doc.Pages.Add(new PageContent { Child = page });
                    page = new FixedPage();
                    page.Width = doc.DocumentPaginator.PageSize.Width;
                    page.Height = doc.DocumentPaginator.PageSize.Height;
                    y = margin;

                    // Reprint header on new page
                    var hdrBg = new System.Windows.Shapes.Rectangle { Width = w - 20, Height = 28, Fill = new SolidColorBrush(Color.FromRgb(46, 125, 50)) };
                    FixedPage.SetLeft(hdrBg, x + 10); FixedPage.SetTop(hdrBg, y);
                    page.Children.Add(hdrBg);

                    cx = tableX;
                    for (int i = 0; i < colHeaders.Length; i++)
                    {
                        var tb = MakeText(colHeaders[i], 10, FontWeights.Bold, Brushes.White);
                        FixedPage.SetLeft(tb, cx + 4); FixedPage.SetTop(tb, y + 4);
                        page.Children.Add(tb);
                        cx += colWidths[i];
                    }
                    y += 28;
                }

                // Alternating row color
                if (stt % 2 == 0)
                {
                    var rowBg = new System.Windows.Shapes.Rectangle { Width = w - 20, Height = rowH, Fill = new SolidColorBrush(Color.FromRgb(245, 245, 245)) };
                    FixedPage.SetLeft(rowBg, x + 10); FixedPage.SetTop(rowBg, y);
                    page.Children.Add(rowBg);
                }

                // Border line
                var lineEl = MakeLine(x + 10, y + rowH - 1, w - 20, 0.5, Brushes.LightGray);
                page.Children.Add(lineEl);

                string[] values = {
                    stt.ToString(),
                    ct.TenNguyenLieu,
                    ct.DonViTinh,
                    ct.SoLuong.ToString("N2"),
                    ct.DonGia.ToString("N0"),
                    ct.ThanhTien.ToString("N0")
                };

                cx = tableX;
                for (int i = 0; i < values.Length; i++)
                {
                    var tb = MakeText(values[i], 10, FontWeights.Normal, Brushes.Black,
                        i == values.Length - 1 ? TextAlignment.Right : TextAlignment.Left);
                    FixedPage.SetLeft(tb, cx + 4);
                    FixedPage.SetTop(tb, y + 2);
                    page.Children.Add(tb);
                    cx += colWidths[i];
                }
                y += rowH;
            }

            // === TOTAL ===
            y += 6;
            page.Children.Add(MakeLine(x, y, w));
            y += 10;

            var tbTotalLabel = MakeText("TỔNG CỘNG:", 14, FontWeights.Bold, Brushes.DarkRed);
            FixedPage.SetLeft(tbTotalLabel, x);
            FixedPage.SetTop(tbTotalLabel, y);
            page.Children.Add(tbTotalLabel);

            var tbTotal = MakeText($"{_phieuNhap.TongTien:N0} VND", 14, FontWeights.Bold, Brushes.DarkRed, TextAlignment.Right);
            FixedPage.SetLeft(tbTotal, x + w - 150);
            FixedPage.SetTop(tbTotal, y);
            page.Children.Add(tbTotal);
            y += 30;

            // Currency words
            string words = NumberToWords((long)_phieuNhap.TongTien);
            if (!string.IsNullOrEmpty(words))
            {
                var tbWords = MakeText($"Bằng chữ: {words} đồng", 11, FontWeights.Normal, Brushes.Gray, TextAlignment.Left);
                tbWords.TextWrapping = System.Windows.TextWrapping.Wrap;
                tbWords.Width = w;
                FixedPage.SetLeft(tbWords, x);
                FixedPage.SetTop(tbWords, y);
                page.Children.Add(tbWords);
                y += 30;
            }

            // Note
            if (!string.IsNullOrEmpty(_phieuNhap.GhiChu))
            {
                var tbNote = MakeText($"Ghi chú: {_phieuNhap.GhiChu}", 11, FontWeights.Normal, Brushes.Gray, TextAlignment.Left);
                FixedPage.SetLeft(tbNote, x);
                FixedPage.SetTop(tbNote, y);
                page.Children.Add(tbNote);
                y += 24;
            }

            // Signatures
            y = page.Height - margin - 80;
            double sigW = 160;
            double spacing = (w - 3 * sigW) / 4;

            string[] sigLabels = { "Người giao hàng", "Người nhận hàng", "Thủ kho" };
            for (int i = 0; i < 3; i++)
            {
                double sx = x + spacing + i * (sigW + spacing);
                var sig = MakeText("........................", 12, FontWeights.Normal, Brushes.Black, TextAlignment.Center);
                FixedPage.SetLeft(sig, sx);
                FixedPage.SetTop(sig, y);
                page.Children.Add(sig);

                var lbl = MakeText(sigLabels[i], 11, FontWeights.Normal, Brushes.Gray, TextAlignment.Center);
                FixedPage.SetLeft(lbl, sx);
                FixedPage.SetTop(lbl, y + 18);
                page.Children.Add(lbl);

                var date = MakeText($"(Ngày {DateTime.Now:dd/MM/yyyy})", 9, FontWeights.Normal, Brushes.Gray, TextAlignment.Center);
                FixedPage.SetLeft(date, sx);
                FixedPage.SetTop(date, y + 34);
                page.Children.Add(date);
            }

            doc.Pages.Add(new PageContent { Child = page });

            ReportViewer.Document = doc;
        }

        private TextBlock MakeText(string text, double fontSize, FontWeight weight, Brush color, TextAlignment align = TextAlignment.Left)
        {
            var tb = new TextBlock
            {
                Text = text,
                FontSize = fontSize,
                FontWeight = weight,
                Foreground = color,
                TextAlignment = align,
                FontFamily = new FontFamily("Segoe UI")
            };
            tb.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            tb.Arrange(new Rect(tb.DesiredSize));
            return tb;
        }

        private System.Windows.Shapes.Line MakeLine(double x, double y, double width, double thickness = 1, Brush brush = null)
        {
            return new System.Windows.Shapes.Line
            {
                X1 = x, Y1 = y, X2 = x + width, Y2 = y,
                Stroke = brush ?? Brushes.DarkGray,
                StrokeThickness = thickness
            };
        }

        /// <summary>
        /// Chuyen so thanh chu (Vietnamese)
        /// </summary>
        private static string NumberToWords(long number)
        {
            if (number == 0) return "không";

            string[] units = { "", "nghìn", "triệu", "tỷ" };
            string[] digits = { "không", "một", "hai", "ba", "bốn", "năm", "sáu", "bảy", "tám", "chín" };

            string result = "";
            int unitIndex = 0;

            while (number > 0)
            {
                int chunk = (int)(number % 1000);
                number /= 1000;

                if (chunk == 0)
                {
                    unitIndex++;
                    continue;
                }

                string chunkStr = "";
                int hundreds = chunk / 100;
                int tens = (chunk % 100) / 10;
                int ones = chunk % 10;

                if (hundreds > 0)
                    chunkStr += digits[hundreds] + " trăm ";
                else if (unitIndex > 0)
                    chunkStr += "không trăm ";

                if (tens > 1)
                {
                    chunkStr += digits[tens] + " mươi ";
                    if (ones == 1) chunkStr += "mốt";
                    else if (ones == 5) chunkStr += "lăm";
                    else if (ones > 0) chunkStr += digits[ones];
                }
                else if (tens == 1)
                {
                    chunkStr += "mười ";
                    if (ones == 5) chunkStr += "lăm";
                    else if (ones > 0) chunkStr += digits[ones];
                }
                else if (tens == 0 && ones > 0)
                {
                    if (hundreds > 0 || unitIndex > 0)
                        chunkStr += "lẻ ";
                    chunkStr += digits[ones];
                }

                if (!string.IsNullOrEmpty(chunkStr))
                {
                    chunkStr = chunkStr.Trim() + " " + units[unitIndex] + " ";
                    result = chunkStr + result;
                }

                unitIndex++;
            }

            return result.Trim();
        }
    }
}
