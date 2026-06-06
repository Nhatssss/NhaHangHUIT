using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;

namespace Nha_Hang_Huit.ViewModels
{
    /// <summary>
    /// Lop co so cho tat ca ViewModel: implement INotifyPropertyChanged
    /// </summary>
    public abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(field, value))
                return false;
            field = value;
            OnPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Tao timer tu dong xoa thong bao sau N giay
        /// </summary>
        private DispatcherTimer _autoClearTimer;

        /// <summary>
        /// Dat thong bao tu dong bien mat sau so mili giay
        /// </summary>
        protected void SetAutoClearMessage(Action<string> setter, string message, int delayMs = 5000)
        {
            setter(message);
            StartAutoClear(() => setter(""), delayMs);
        }

        /// <summary>
        /// Dat thong bao + visibility flag tu dong bien mat
        /// </summary>
        protected void SetAutoClearMessage(Action<string> setText, Action<bool> setVisible, string message, int delayMs = 5000)
        {
            setText(message);
            setVisible(true);
            StartAutoClear(() =>
            {
                setText("");
                setVisible(false);
            }, delayMs);
        }

        private void StartAutoClear(Action clearAction, int delayMs)
        {
            _autoClearTimer?.Stop();
            _autoClearTimer = new DispatcherTimer(
                TimeSpan.FromMilliseconds(delayMs),
                DispatcherPriority.Normal,
                (s, e) =>
                {
                    clearAction();
                    _autoClearTimer.Stop();
                    _autoClearTimer = null;
                },
                Dispatcher.CurrentDispatcher);
            _autoClearTimer.Start();
        }
    }
}
