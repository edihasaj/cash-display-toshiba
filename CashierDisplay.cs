using OposLineDisplay_CCO;
using System;
using System.Threading;

namespace CashierDisplayToshiba
{
    public sealed class CashierDisplay : IDisposable
    {
        private OPOSLineDisplay _display;
        private string _textToDisplay;
        private static Thread _displayThread;

        public CashierDisplay()
        {
            _display = new OPOSLineDisplay();
            OpenDisplayConnection();
        }

        public void SendTextToDisplay(string textToDisplay, bool isTotal = false)
        {
            /*if (isTotal) // Uncomment this to let the display display things on it's own, and abort when isTotal
                _displayThread.Abort();*/
            _displayThread.Abort();

            if (_displayThread.ThreadState != ThreadState.Stopped &&
                _displayThread.ThreadState != ThreadState.Aborted) return;
            
            _displayThread = new Thread(() =>
            {
                _display.ClearText();
                _textToDisplay = textToDisplay;
                _display.DisplayText(_textToDisplay, 1);
            });
            _displayThread.Start();
        }

        public void OpenDisplayConnection()
        {
            _displayThread = new Thread(OpenConnection);
            _displayThread.Start();
        }

        private void OpenConnection()
        {
            _display.Open("DISPLAY");
            _display.ClaimDevice(200);
            _display.DeviceEnabled = true;
        }

        public void CloseDisplayConnection()
        {
            _displayThread.Abort();
            _displayThread = new Thread(CloseConnection);
            _displayThread.Start();
        }

        private void CloseConnection()
        {
            _display.DeviceEnabled = false;
            _display.ReleaseDevice();
            _display.Close();
        }

        public bool ConnectionIsOpen()
        {
            return _display.DeviceEnabled;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            CloseDisplayConnection();

            if (disposing)
            {
                ReleaseManagedResources();
            }

            ReleaseUnmanagedResources();
        }

        ~CashierDisplay()
        {
            Dispose(false);
        }

        private void ReleaseManagedResources()
        {
            if (_display == null) return;
            _display = null;
            _textToDisplay = null;
        }

        private static void ReleaseUnmanagedResources()
        { }
    }
}
