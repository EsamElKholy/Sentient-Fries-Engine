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
    /// Interaction logic for CreateProjectView.xaml
    /// </summary>
    public partial class CreateProjectView : UserControl
    {
        public CreateProjectView()
        {
            InitializeComponent();
        }

        private void OnCreate_Button_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as CreateProjectModel;

            var projectPath = viewModel.CreateProject(templateListBox.SelectedItem as ProjectTemplate);

            var window = Window.GetWindow(this);
            bool dialogueResult = false;

            if (!string.IsNullOrEmpty(projectPath))
            {
                dialogueResult = true;

                var project = OpenProjectModel.OpenProject(new ProjectData() { ProjectName = viewModel.ProjectName, ProjectPath = projectPath });
            }
            window.DialogResult = dialogueResult;
            window.Close();
        }
    }
}
