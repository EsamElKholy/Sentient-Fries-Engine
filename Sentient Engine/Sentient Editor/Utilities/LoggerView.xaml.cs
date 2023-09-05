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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Sentient_Editor.Utilities
{
    /// <summary>
    /// Interaction logic for LoggerView.xaml
    /// </summary>
    public partial class LoggerView : UserControl
    {
        public LoggerView()
        {
            InitializeComponent();

            Loaded += (s, e) =>
            {
                Logger.Log(MessageType.Info, "Info Message");
                Logger.Log(MessageType.Warning, "Warning Message");
                Logger.Log(MessageType.Error, "Error Message");
            };
        }

        private void OnClick_Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.Clear();
        }

        private void MessageFilter_Button_Click(object sender, RoutedEventArgs e)
        {
            Logger.SetMessageFilter(toggleInfo.IsChecked.Value, toggleWarning.IsChecked.Value, toggleError.IsChecked.Value);
        }
    }
}
