﻿using Sentient_Editor.GameProject;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace Sentient_Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += OnMainWindowLoaded;
            Closing += OnMainWindowClosing;
        }

        private void OnMainWindowClosing(object sender, CancelEventArgs e)
        {
            Closing -= OnMainWindowClosing;

            Project.Current?.UnloadProject();
        }

        private void OnMainWindowLoaded(object sender, RoutedEventArgs e) 
        {
            Loaded -= OnMainWindowLoaded;

            OpenProjectBrowserDialogue();
        }

        private void OpenProjectBrowserDialogue()
        {
            var projectBrowser = new GameProject.ProjectBrowserDialogue();
           
            if (projectBrowser.ShowDialog() == false || projectBrowser.DataContext == null) 
            {
                Application.Current.Shutdown();
            }
            else
            {
                Project.Current?.UnloadProject();

                DataContext = projectBrowser.DataContext;
            }
        }
    }
}
