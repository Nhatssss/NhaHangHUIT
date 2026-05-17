using System.ComponentModel;

namespace Nha_Hang_Huit.Models
{
    /// <summary>
    /// Model chi tiet hoa don - schema tblChiTietHoaDon
    /// </summary>
    public class ChiTietHoaDon : INotifyPropertyChanged
    {
        public int MaChiTiet { get; set; }
        public int MaHoaDon { get; set; }
        public int MaMonAn { get; set; }
        public string TenMonAn { get; set; }
        public string GhiChu { get; set; }

        private int _soLuong;
        public int SoLuong
        {
            get => _soLuong;
            set
            {
                if (_soLuong != value)
                {
                    _soLuong = value;
                    OnPropertyChanged(nameof(SoLuong));
                    OnPropertyChanged(nameof(ThanhTien));
                }
            }
        }

        private decimal _donGia;
        public decimal DonGia
        {
            get => _donGia;
            set
            {
                if (_donGia != value)
                {
                    _donGia = value;
                    OnPropertyChanged(nameof(DonGia));
                    OnPropertyChanged(nameof(ThanhTien));
                }
            }
        }

        /// <summary>Thanh tien = DonGia * SoLuong (computed, khong set)</summary>
        public decimal ThanhTien => DonGia * SoLuong;

        /// <summary>Danh dau hang tam trong gio hang (chua luu DB)</summary>
        public bool IsTempItem { get; set; } = true;

        /// <summary>Hien thi trong UI</summary>
        public string HienThi => $"{TenMonAn} x{SoLuong} = {ThanhTien:N0} VND";

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
