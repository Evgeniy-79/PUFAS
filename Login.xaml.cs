using System;
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
using System.Threading;
using Renci.SshNet;

namespace PUFAS
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        SshClient _ssh;
        SftpClient _sftp;

        public Login()
        {
            InitializeComponent();
        }

        private void btnEnter_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Connect();
        }

        private void Connect()
        {
            string _host = txtHost.Text;
            string _user = txtUser.Text;
            string _pass = txtPassword.Password;

            try
            {
                _ssh = new SshClient(_host, _user, _pass);
                _ssh.Connect();
                _sftp = new SftpClient(_host, _user, _pass);
                _sftp.Connect();

                FileManager man = new FileManager(_ssh, _sftp);
                man.Show();
                this.Close();
            }
            catch
            {
                ShowStatus("Connection failed");
            }
        }

        private void ShowStatus(string message)
        {
            lblStatus.Content = message;
        }
    }
}
