using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Nha_Hang_Huit.Models;
using Nha_Hang_Huit.Services;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// ViewModel cho danh sach phieu nhap hang
    /// </summary>
    public class PhieuNhapViewModel : BaseViewModel
    {
        private readonly PhieuNhapService _service = new PhieuNhapService();

        // Danh sach phieu nhap
        public ObservableCollection<PhieuNhap> PhieuNhapList { get; } = new ObservableCollection<PhieuNhap>();

        // Phieu nhap duoc chon
        private PhieuNhap _selectedPhieuNhap;
        public PhieuNhap SelectedPhieuNhap
        {
            get => _selectedPhieuNhap;
            set
            {
                if (SetProperty(ref _selectedPhieuNhap, value) && value != null)
                    LoadChiTiet(value.MaPhieuNhap);
                OnPropertyChanged(nameof(HasSelected));
                OnPropertyChanged(nameof(PhieuNhapInfoText));
            }
        }

        public bool HasSelected => SelectedPhieuNhap != null;
        public string PhieuNhapInfoText => SelectedPhieuNhap != null
            ? $"Phieu #{SelectedPhieuNhap.MaPhieuNhap} - {SelectedPhieuNhap.TenNhaCungCap}"
            : "(Chua chon phieu nhap)";

        // Chi tiet phieu nhap
        public ObservableCollection<ChiTietPhieuNhap> ChiTietList { get; } = new ObservableCollection<ChiTietPhieuNhap>();

        // Tong tien
        private decimal _tongTienPhieu;
        public decimal TongTienPhieu
        {
            get => _tongTienPhieu;
            set => SetProperty(ref _tongTienPhieu, value);
        }
        public string TongTienPhieuText => $"{TongTienPhieu:N0} VND";

        // Thong ke
        private int _tongPhieu;
        public int TongPhieu
        {
            get => _tongPhieu;
            set => SetProperty(ref _tongPhieu, value);
        }
        public string TongPhieuText => $"Tong phieu: {TongPhieu}";

        private decimal _tongChiPhi;
        public decimal TongChiPhi
        {
            get => _tongChiPhi;
            set => SetProperty(ref _tongChiPhi, value);
        }
        public string TongChiPhiText => $"{TongChiPhi:N0} VND";

        // Trang thai
        private bool _coDuLieu = true;
        public bool CoDuLieu
        {
            get => _coDuLieu;
            set => SetProperty(ref _coDuLieu, value);
        }

        // Filters
        private bool _filterToday;
        public bool FilterToday
        {
            get => _filterToday;
            set
            {
                if (SetProperty(ref _filterToday, value) && value)
                {
                    FilterWeek = false;
                    FilterMonth = false;
                    FilterAll = false;
                    LoadData();
                }
            }
        }

        private bool _filterWeek;
        public bool FilterWeek
        {
            get => _filterWeek;
            set
            {
                if (SetProperty(ref _filterWeek, value) && value)
                {
                    FilterToday = false;
                    FilterMonth = false;
                    FilterAll = false;
                    LoadData();
                }
            }
        }

        private bool _filterMonth;
        public bool FilterMonth
        {
            get => _filterMonth;
            set
            {
                if (SetProperty(ref _filterMonth, value) && value)
                {
                    FilterToday = false;
                    FilterWeek = false;
                    FilterAll = false;
                    LoadData();
                }
            }
        }

        private bool _filterAll = true;
        public bool FilterAll
        {
            get => _filterAll;
            set
            {
                if (SetProperty(ref _filterAll, value) && value)
                {
                    FilterToday = false;
                    FilterWeek = false;
                    FilterMonth = false;
                    LoadData();
                }
            }
        }

        // Commands
        public ICommand DongCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand FilterTodayCommand { get; }
        public ICommand FilterWeekCommand { get; }
        public ICommand FilterMonthCommand { get; }
        public ICommand FilterAllCommand { get; }
        public ICommand InPhieuCommand { get; }

        public event EventHandler DongYeuCau;
        public event EventHandler<PhieuNhap> InPhieuYeuCau;

        public PhieuNhapViewModel()
        {
            DongCommand = new RelayCommand(_ => DongYeuCau?.Invoke(this, EventArgs.Empty));
            RefreshCommand = new RelayCommand(_ => LoadData());
            FilterTodayCommand = new RelayCommand(_ => FilterToday = true);
            FilterWeekCommand = new RelayCommand(_ => FilterWeek = true);
            FilterMonthCommand = new RelayCommand(_ => FilterMonth = true);
            FilterAllCommand = new RelayCommand(_ => FilterAll = true);
            InPhieuCommand = new RelayCommand(_ =>
            {
                if (SelectedPhieuNhap != null)
                    InPhieuYeuCau?.Invoke(this, SelectedPhieuNhap);
            }, _ => SelectedPhieuNhap != null);

            // Default: show all
            FilterAll = true;
        }

        public void LoadData()
        {
            PhieuNhapList.Clear();
            ChiTietList.Clear();
            SelectedPhieuNhap = null;
            TongTienPhieu = 0;

            var list = GetFilteredData();

            foreach (var pn in list)
                PhieuNhapList.Add(pn);

            TongPhieu = PhieuNhapList.Count;
            TongChiPhi = list.Sum(p => p.TongTien);
            CoDuLieu = TongPhieu > 0;

            // Auto-select first item
            if (PhieuNhapList.Count > 0)
                SelectedPhieuNhap = PhieuNhapList[0];
        }

        private System.Collections.Generic.List<PhieuNhap> GetFilteredData()
        {
            if (FilterToday)
            {
                var today = DateTime.Today;
                return _service.GetPhieuNhapFullByDate(today, today.AddDays(1));
            }
            else if (FilterWeek)
            {
                var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
                return _service.GetPhieuNhapFullByDate(startOfWeek, startOfWeek.AddDays(7));
            }
            else if (FilterMonth)
            {
                var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                return _service.GetPhieuNhapFullByDate(startOfMonth, startOfMonth.AddMonths(1));
            }
            else
            {
                return _service.GetAllPhieuNhap().Select(pn =>
                {
                    pn.ChiTietList = _service.GetChiTietByPhieuNhap(pn.MaPhieuNhap);
                    return pn;
                }).ToList();
            }
        }

        private void LoadChiTiet(int maPhieuNhap)
        {
            ChiTietList.Clear();
            var ctList = _service.GetChiTietByPhieuNhap(maPhieuNhap);
            foreach (var ct in ctList)
                ChiTietList.Add(ct);

            TongTienPhieu = ctList.Sum(c => c.ThanhTien);
            OnPropertyChanged(nameof(PhieuNhapInfoText));
        }
    }
}
