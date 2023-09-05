using Sentient_Editor.Components;
using Sentient_Editor.GameProject;
using Sentient_Editor.Utilities;
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

namespace Sentient_Editor.Editors
{
    /// <summary>
    /// Interaction logic for GameEntityView.xaml
    /// </summary>
    public partial class GameEntityView : UserControl
    {
        private Action undoAction;
        private string propertyName;
        public static GameEntityView Instance { get; private set; }

        public GameEntityView()
        {
            InitializeComponent();

            DataContext = null;

            Instance = this;

            DataContextChanged += (_, __) =>
            {
                if (DataContext != null)
                {
                    (DataContext as MultiSelectionEntity).PropertyChanged += (s, e) => 
                    {
                        propertyName = e.PropertyName;
                    };
                }
            };
        }

        private void OnName_TextBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            undoAction = GetRenameAction();
        }

        private Action GetRenameAction() 
        {
            var vm = DataContext as MultiSelectionGameEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.Name)).ToList();

            var action = new Action(() =>
            {
                selection.ForEach(e => e.entity.Name = e.Name);
                (DataContext as MultiSelectionEntity).Refresh();
            });

            return action;
        }

        private Action GetIsEnabledAction()
        {
            var vm = DataContext as MultiSelectionGameEntity;
            var selection = vm.SelectedEntities.Select(entity => (entity, entity.IsEnabled)).ToList();

            var action = new Action(() =>
            {
                selection.ForEach(e => e.entity.IsEnabled = e.IsEnabled);
                (DataContext as MultiSelectionEntity).Refresh();
            });

            return action;
        }

        private void OnName_TextBox_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            if (propertyName == nameof(MultiSelectionEntity.Name) && undoAction != null)
            {                
                Project.UndoRedo.Add(new UndoRedoAction(undoAction, GetRenameAction(), "Rename game entity"));
                propertyName = null;
            }

            undoAction = null;
        }

        private void IsEnabled_CheckBox_Click(object sender, RoutedEventArgs e)
        {
            var undo = GetIsEnabledAction();

            var vm = DataContext as MultiSelectionGameEntity;
            vm.IsEnabled = (sender as CheckBox).IsChecked == true;
            
            var redo = GetIsEnabledAction();

            Project.UndoRedo.Add(new UndoRedoAction(undo, redo, vm.IsEnabled == true ? "Enable game entity" : "Disable game entity"));
        }
    }
}
