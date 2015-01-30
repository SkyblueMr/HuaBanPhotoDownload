using System;
using System.Collections.Generic;
using System.Configuration;
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

namespace HuaBanPhotoDownload
{
    /// <summary>
    /// DownLoadForm.xaml 的交互逻辑
    /// </summary>
    public partial class DownLoadForm : Window
    {

        public DownLoadForm()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings["ImageDir"].Value = SavePath.Text;
            cfa.Save(ConfigurationSaveMode.Modified); 

            DialogResult = false;
            Close();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
  
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.FolderBrowserDialog Dlg = new System.Windows.Forms.FolderBrowserDialog();
            Dlg.ShowNewFolderButton = false;
            System.Windows.Interop.HwndSource source = PresentationSource.FromVisual(this) as System.Windows.Interop.HwndSource;
            System.Windows.Forms.IWin32Window win = new OldWindow(source.Handle);
            System.Windows.Forms.DialogResult Result = Dlg.ShowDialog(win);
            if (Result == System.Windows.Forms.DialogResult.OK)
            {
                SavePath.Text = Dlg.SelectedPath; 
            }
           
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            SavePath.Text = Config.ImageDir;
        }
    }
}
