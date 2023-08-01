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

namespace Sentient_Editor.GameProject
{
    /// <summary>
    /// Interaction logic for OpenProjectView.xaml
    /// </summary>
    public partial class OpenProjectView : UserControl
    {
        public OpenProjectView()
        {
            InitializeComponent();
        }

        private void OnOpen_Button_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedProject();
        }

        private void OpenSelectedProject() 
        {
            var project = OpenProjectModel.OpenProject(projectsListBox.SelectedItem as ProjectData);

            var window = Window.GetWindow(this);
            bool dialogueResult = false;

            if (project != null)
            {
                dialogueResult = true;

                window.DataContext = project;
            }

            window.DialogResult = dialogueResult;
            window.Close();
        }

        private void OnListBoxItem_Mouse_Double_Click(object sender, MouseButtonEventArgs e) 
        {
            OpenSelectedProject();
        }
    }
}
