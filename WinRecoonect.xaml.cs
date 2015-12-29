using System;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Renci.SshNet;

namespace PUFAS
{
    /// <summary>
    /// Interaction logic for WinRecoonect.xaml
    /// </summary>
    public partial class WinRecoonect : Window
    {
        int _timeout;
        int cpoint;

        SshClient _ssh;
        SftpClient _sftp;

        bool isReconnect;

        Thread tr;

        System.Windows.Threading.DispatcherTimer timer =
            new System.Windows.Threading.DispatcherTimer();

        public WinRecoonect(SshClient ssh, SftpClient sftp, int timeout)
        {
            _ssh = ssh;
            _sftp = sftp;
            _timeout = timeout;

            InitializeComponent();

            tr = new Thread(ThreadM);
            tr.SetApartmentState(ApartmentState.STA);
            tr.Start();

        }

        private void ThreadM()
        {
            if (!isReconnect)
            {
                try
                {
                    _ssh.Connect();
                    _sftp.Connect();
                    isReconnect = true;
                    this.Dispatcher.Invoke(new Action(() => {
                        this.Close();
                    }));
                }
                catch
                {
                    ThreadM();
                }
            }
        }

        private void TimerTick(Object o, EventArgs e)
        {
            if (_timeout == 0)
            {
                try
                {
                    _ssh.Disconnect();
                    _sftp.Disconnect();
                }
                catch
                {
                }
                isReconnect = true;
                this.Close();
            }
            lbl.Content = _timeout.ToString();
            switch (cpoint)
            {
                case 0:
                    cpoint = 1;
                    lbl.Content += " .";
                    break;
                case 1:
                    cpoint = 2;
                    lbl.Content += " ..";
                    break;
                case 2:
                    cpoint = 3;
                    lbl.Content += " ...";
                    break;
                case 3:
                    cpoint = 1;
                    lbl.Content += " .";
                    break;
            }

            _timeout--;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            timer.Tick += new EventHandler(TimerTick);
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!isReconnect)
            {
                e.Cancel = true;
            }
        }
    }
}
