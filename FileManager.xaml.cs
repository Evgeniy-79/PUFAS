using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for FileManager.xaml
    /// </summary>
    public partial class FileManager : Window
    {
        SshClient _ssh;
        SftpClient _sftp;
        // Домашний каталог
        string local_dir;
        // Удаленный каталог
        string remote_dir;
        // Буфер
        cFile copy_file;
        // Коллекция файлов в текущем каталоге
        ObservableCollection<pFile> file_localcollection;
        // Коллекция файлов в удаленном каталоге
        ObservableCollection<pFile> file_remotecollection;

        List<string> listlocal_path;
        List<string> listremote_path;

        bool flback;
        bool frback;
        bool isLocalDir;

        string LocalDir
        {
            get
            {
                return local_dir;
            }
            set
            {
                local_dir = value;
                lblPathOne.Content = local_dir;
            }
        }
        string RemoteDir
        {
            get
            {
                return remote_dir;
            }
            set
            {
                remote_dir = value;
                lblPathTwo.Content = remote_dir;
            }
        }

        public FileManager(SshClient ssh, SftpClient sftp)
        {
            InitializeComponent(); 
            _ssh = ssh;
            _sftp = sftp;

            LocalDir = @"C:\";
            RemoteDir = "../..";

            listlocal_path = new List<string>();
            listremote_path = new List<string>();

            listlocal_path.Add(LocalDir);
            listremote_path.Add(RemoteDir);

            // Инициализируем коллекции
            file_localcollection = new ObservableCollection<pFile>();
            file_remotecollection = new ObservableCollection<pFile>();
            // Привяжем коллекции
            lsLocal.ItemsSource = file_localcollection;
            lsRemove.ItemsSource = file_remotecollection;

            UpdateLocalDir();
            UpdateRemoteDir();

            lsLocal.ContextMenu = c_menu();
            lsRemove.ContextMenu = c_menu();
        }

        #region Events

        private void lsOne_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                pFile file = (pFile)lsLocal.SelectedItem;
                if (file != null)
                    GoToDirectory(file.Name);
            }
            catch { }
            
        }
        private void lsRemove_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            try
            {
                pFile file = (pFile)lsRemove.SelectedItem;
                if (file != null)
                    GoToRemDir(file.Name);
            }
            catch { }
        }
        private void btnNewFile_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CreateNewDir();
            }
            catch { }
            
        }
        private void btnRemove_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //try
            //{
            //    RemoveFile();
            //}
            //catch { }

            RemoveFile();
        }
        private void Rename_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                RenameFile();
            }
            catch { }
        }
        private void Button_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            try
            {
                CreateNewFile();
            }
            catch { }
            
        }
        private void btnBack_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isLocalDir)
                BackLocalPatch();
            else
                BackRemotePatch();
        }
        private void btnForward_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (isLocalDir)
                ForwdlocalPath();
            else
                ForwdRemotePath();
        }
        private void lsLocal_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            
        }
        private void btnCopy_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CopyFile(false);
        }
        private void btnMove_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            PasteFile();
        }
        private void btnCut_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CopyFile(true);
        }
        private void lsLocal_GotFocus(object sender, RoutedEventArgs e)
        {
            isLocalDir = true;
        }
        private void lsRemove_GotFocus(object sender, RoutedEventArgs e)
        {
            isLocalDir = false;
        }
        #endregion

        #region LocalNavigation

        private void GoToDirectory(string name_dir)
        {
            if (flback)
            {
                flback = false;
                ClearListLocal();
            }

            LocalDir = LocalDir + @"\" + name_dir;
            listlocal_path.Add(LocalDir);

            UpdateLocalDir();
        }
        private void BackLocalPatch()
        {
            string _path = LocalDir;
            foreach (string str in listlocal_path)
            {
                if (str == LocalDir)
                {
                    LocalDir = _path;
                    UpdateLocalDir();
                    flback = true;
                    break;
                }
                _path = str;
            }
        }
        private void ForwdlocalPath()
        {
            bool find = false;
            foreach (string str in listlocal_path)
            {
                if (find)
                {
                    LocalDir = str;
                    UpdateLocalDir();
                    break;
                }

                if (str == LocalDir)
                {
                    find = true;
                }
            }
        }
        private void ClearListLocal()
        {
            string _path = LocalDir;
            for (int i = 0; i < listlocal_path.Count; i++)
            {
                if (listlocal_path[i] == LocalDir)
                {
                    listlocal_path.RemoveRange(i + 1, listlocal_path.Count - i - 1);
                    break;
                }
            }
        }
        #endregion

        #region RemoteNavigation

        private void GoToRemDir(string dir)
        {
            if (frback)
            {
                frback = false;
                ClearListRemote();
            }
            if (RemoteDir == "../.." || RemoteDir == "..")
                RemoteDir = "";
            RemoteDir = RemoteDir + "/" + dir;
            listremote_path.Add(RemoteDir);

            UpdateRemoteDir();
        }
        private void BackRemotePatch()
        {
            string _path = RemoteDir;
            foreach (string str in listremote_path)
            {
                if (str == RemoteDir)
                {
                    RemoteDir = _path;
                    UpdateRemoteDir();
                    frback = true;
                    break;
                }
                _path = str;
            }
        }
        private void ForwdRemotePath()
        {
            bool find = false;
            foreach (string str in listremote_path)
            {
                if (find)
                {
                    RemoteDir = str;
                    UpdateRemoteDir();
                    break;
                }

                if (str == RemoteDir)
                {
                    find = true;
                }
            }
        }
        private void ClearListRemote()
        {
            string _path = RemoteDir;
            for (int i = 0; i < listremote_path.Count; i++)
            {
                if (listremote_path[i] == RemoteDir)
                {
                    listremote_path.RemoveRange(i + 1, listremote_path.Count - i - 1);
                    break;
                }
            }
        }
        #endregion

        /// <summary>
        /// Создание нового файла
        /// </summary>
        private void CreateNewFile()
        {
            string _name = GetName(1);

            if (isLocalDir)
            {
                File.Create(LocalDir + @"\" + _name);
                UpdateLocalDir();
            }
            else
            {
                string _command = "touch " + RemoteDir + "/" + _name;
                _ssh.RunCommand(_command);
                UpdateRemoteDir();
            }
            
        }

        /// <summary>
        /// Создание новой директории
        /// </summary>
        private void CreateNewDir()
        {
            string _name = GetName(0);

            if (isLocalDir)
            {
                DirectoryInfo dir = new DirectoryInfo(LocalDir + @"\" + _name);
                dir.Create();
                UpdateLocalDir();
            }
            else
            {
                string _command = "mkdir " + RemoteDir + "/" + _name;
                _ssh.RunCommand(_command);
                UpdateRemoteDir();
            }
        }

        /// <summary>
        /// Удаление файла
        /// </summary>
        private void RemoveFile()
        {
            if (isLocalDir)
            {
                //try
                //{
                //    pFile file = (pFile)lsLocal.SelectedItem;
                //    if (file != null)
                //    {
                //        if (file.Size == null)
                //            Directory.Delete(LocalDir + @"\" + file.Name);
                //        else
                //            File.Delete(local_dir + @"\" + file.Name);
                //    }
                //    UpdateLocalDir();
                //}
                //catch { }

                pFile file = (pFile)lsLocal.SelectedItem;
                if (file != null)
                {
                    if (file.Size == null)
                        Directory.Delete(LocalDir + @"\" + file.Name, true);
                    else
                        File.Delete(local_dir + @"\" + file.Name);
                }
                UpdateLocalDir();

            }
            else
            {
                pFile file = (pFile)lsRemove.SelectedItem;
                if (file != null)
                {
                    string _commandF = "rm " + RemoteDir + "/" + file.Name;
                    string _commandD = "rm -rf " + RemoteDir + "/" + file.Name;
                    _ssh.RunCommand(_commandF);
                    _ssh.RunCommand(_commandD);
                }
                UpdateRemoteDir();
            }
        }

        /// <summary>
        /// Переименование файла
        /// </summary>
        private void RenameFile()
        {
            string _name = GetName(2);

            if (isLocalDir)
            {
                try
                {
                    pFile file = (pFile)lsLocal.SelectedItem;
                    if (file != null)
                    {
                        if (file.Size == null)
                            Directory.Move(LocalDir + @"\" + file.Name, LocalDir + @"\" + _name);
                        else
                        {
                            File.Move(LocalDir + @"\" + file.Name, LocalDir + @"\" + _name);
                        }

                    }
                    UpdateLocalDir();
                }
                catch { }
            }
            else
            {
                pFile file = (pFile)lsRemove.SelectedItem;
                if (file != null)
                {
                    string _command = "mv " + RemoteDir + "/" + file.Name + " " + RemoteDir + "/" + _name;
                    _ssh.RunCommand(_command);
                }
                UpdateRemoteDir();
            }
        }

        /// <summary>
        /// Копирование файла
        /// </summary>
        private void CopyFile(bool iscut)
        {
            if (isLocalDir)
            {
                pFile file = (pFile)lsLocal.SelectedItem;
                if (file != null)
                {
                    copy_file = new cFile
                    {
                        Name = file.Name,
                        Local = true,
                        IsCut = iscut,
                        Path = LocalDir + @"\" + file.Name
                    };

                }
                    
            }
            else
            {
                pFile file = (pFile)lsRemove.SelectedItem;
                if (file != null)
                    copy_file = new cFile
                    {
                        Name = file.Name,
                        Local = false,
                        IsCut = iscut,
                        Path = RemoteDir + "/" + file.Name
                    };
            }
        }
        
        /// <summary>
        /// Вставка файла
        /// </summary>
        private void PasteFile()
        {
            if (isLocalDir)
            {
                if(copy_file.Local)
                {
                    File.Copy(copy_file.Path, LocalDir + @"\" + copy_file.Name);
                    if (copy_file.IsCut)
                        File.Delete(copy_file.Path);
                    UpdateLocalDir();
                    UpdateRemoteDir();
                }
                else
                {
                    // Из удаленки на локалку
                    try
                    {
                        FileStream f = new FileStream(LocalDir + @"\" + copy_file.Name, FileMode.Create);
                        if (copy_file.IsCut)
                            _sftp.DeleteFile(copy_file.Path);
                        _sftp.DownloadFile(copy_file.Path, f);
                        f.Close();
                    }
                    catch
                    {
                    }
                    UpdateLocalDir();
                    UpdateRemoteDir();
                }
            }
            else
            {
                if (copy_file.Local)
                {
                    FileStream f = new FileStream(copy_file.Path, FileMode.OpenOrCreate);
                    _sftp.BufferSize = 4 * 1024;
                    _sftp.UploadFile(f, RemoteDir + "/" + copy_file.Name, null);
                    f.Close();
                    if (copy_file.IsCut)
                        File.Delete(copy_file.Path);
                    UpdateRemoteDir();
                    UpdateLocalDir();
                }
                else
                {
                    string _command = "cp " + copy_file.Path + " " + RemoteDir + "/";
                    string res = _ssh.RunCommand(_command).Result;
                    if (copy_file.IsCut)
                        _sftp.DeleteFile(copy_file.Path);
                    UpdateLocalDir();
                    UpdateRemoteDir();
                }
            }
        }

        /// <summary>
        /// Обновление локальной директории
        /// </summary>
        private void UpdateLocalDir()
        {
            file_localcollection.Clear();

            GetDirectories(0);
            GetFiles();
        }

        /// <summary>
        /// Обновление удаленной директории
        /// </summary>
        private void UpdateRemoteDir()
        {
            file_remotecollection.Clear();

            GetDirectories(1);
        }

        /// <summary>
        /// Получение списка файлов в текущем каталоге
        /// </summary>
        private void GetFiles()
        {
            DirectoryInfo dinfo = new DirectoryInfo(LocalDir);
            FileInfo[] filearr = dinfo.GetFiles();
            foreach(FileInfo file in filearr)
            {
                file_localcollection.Add(new pFile()
                {
                    Name = file.Name,
                    Size = file.Length.ToString(),
                    Date = file.LastWriteTime
                });
            }
        }

        /// <summary>
        /// Получение списка директорий в текущем каталоге
        /// </summary>
        private void GetDirectories(int type)
        {
            if (type == 0){
                DirectoryInfo dinfo = new DirectoryInfo(LocalDir);
                foreach (var dir in dinfo.GetDirectories())
                {
                    file_localcollection.Add(new pFile()
                    {
                        Name = dir.Name,
                        Date = dir.LastWriteTime
                    });
                }
            }
            else {
                IEnumerable<Renci.SshNet.Sftp.SftpFile> ls = _sftp.ListDirectory(RemoteDir);
                List<pFile> lsp = new List<pFile>();

                foreach (Renci.SshNet.Sftp.SftpFile f in ls)
                {
                    pFile _pf = new pFile { 
                        Name = f.Name,
                        Date = f.LastWriteTime
                    };

                    if (f.IsDirectory)
                    {
                        _pf.Size = null;
                        file_remotecollection.Add(_pf);
                    } 
                    else
                    {
                        _pf.Size = f.Length.ToString();
                        lsp.Add(_pf);
                    }
                }

                foreach (pFile pfl in lsp)
                    file_remotecollection.Add(pfl);
            }
        }
        
        /// <summary>
        /// Получение имени через окно
        /// </summary>
        private string GetName(int index)
        {
            Window win = new Window
            {
                Width = 300,
                Height = 150,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };
            Label lbl = new Label();
            switch(index)
            {
                case 0:
                    lbl.Content = "Новая папка";
                    break;
                case 1:
                    lbl.Content = "Новый файл";
                    break;
                case 2:
                    lbl.Content = "Введите новое имя";
                    break;
            }

            StackPanel sp = new StackPanel
            {
                Margin = new Thickness(5),
                Background = new SolidColorBrush(Colors.Gray)
            };
            TextBox txb = new TextBox
            {
                
            };
            Button btn = new Button
            {
                Width = 100,
                Height = 30,
                Content = "Ok",
                Margin = new Thickness(0, 10, 0, 0)
            };
            btn.PreviewMouseLeftButtonDown += (o, i) => { win.Close(); };
            win.Content = sp;
            sp.Children.Add(lbl);
            sp.Children.Add(txb);
            sp.Children.Add(btn);
            win.ShowDialog();

            return txb.Text;
        }

        /// <summary>
        /// Возвращает контекстное меню
        /// </summary>
        /// <returns></returns>
        private ContextMenu c_menu()
        {
            ContextMenu _menu = new ContextMenu();
            
            Label l_newF = new Label();
            l_newF.Content = "Новый файл";
            l_newF.Background = new SolidColorBrush(Colors.Transparent);
            l_newF.Foreground = new SolidColorBrush(Colors.White);
            l_newF.PreviewMouseLeftButtonDown += 
                new MouseButtonEventHandler((s, o) => { CreateNewFile(); });
            Label l_newD = new Label();
            l_newD.Content = "Новая папка";
            l_newD.Background = new SolidColorBrush(Colors.Transparent);
            l_newD.Foreground = new SolidColorBrush(Colors.White);
            l_newD.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { CreateNewDir(); });
            Label l_rename = new Label();
            l_rename.Content = "Переименовать";
            l_rename.Background = new SolidColorBrush(Colors.Transparent);
            l_rename.Foreground = new SolidColorBrush(Colors.White);
            l_rename.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { RenameFile(); });
            Label l_cop = new Label();
            l_cop.Content = "Копировать";
            l_cop.Background = new SolidColorBrush(Colors.Transparent);
            l_cop.Foreground = new SolidColorBrush(Colors.White);
            l_cop.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { CopyFile(false); });
            Label l_cut = new Label();
            l_cut.Content = "Вырезать";
            l_cut.Background = new SolidColorBrush(Colors.Transparent);
            l_cut.Foreground = new SolidColorBrush(Colors.White);
            l_cut.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { CopyFile(true); });
            Label l_move = new Label();
            l_move.Content = "Вставить";
            l_move.Background = new SolidColorBrush(Colors.Transparent);
            l_move.Foreground = new SolidColorBrush(Colors.White);
            l_move.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { PasteFile(); });
            Label l_del = new Label();
            l_del.Content = "Удалить";
            l_del.Background = new SolidColorBrush(Colors.Transparent);
            l_del.Foreground = new SolidColorBrush(Colors.White);
            l_del.PreviewMouseLeftButtonDown +=
                new MouseButtonEventHandler((s, o) => { RemoveFile(); });

            _menu.Items.Add(l_newF);
            _menu.Items.Add(l_newD);
            _menu.Items.Add(l_rename);
            _menu.Items.Add(l_cop);
            _menu.Items.Add(l_move);
            _menu.Items.Add(l_del);

            return _menu;
        }
    }
}
