﻿using Sentient_Editor.Components;
using Sentient_Editor.GameProject;
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
    /// Interaction logic for ProjectLayoutView.xaml
    /// </summary>
    public partial class ProjectLayoutView : UserControl
    {
        public ProjectLayoutView()
        {
            InitializeComponent();
        }

        private void OnAddGameEntity_Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var scene = button.DataContext as Scene;
            scene.AddGameEntityCommand.Execute(new GameEntity(scene) { Name = "Empty Game Entity"});
        }

        private void OnGameEntities_Listbox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var entity = (sender as ListBox).SelectedItems[0];

            GameEntityView.Instance.DataContext = entity;
        }
    }
}
