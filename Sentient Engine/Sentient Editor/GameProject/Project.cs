using Sentient_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Sentient_Editor.GameProject
{
    [DataContract(Name = "Game")]
    public class Project : ViewModelBase
    {
        public static string Extension { get; } = ".sentient";

        [DataMember]
        public string ProjectName
        {
            get; private set;
        } = "New Project";

        [DataMember]
        public string ProjectPath
        {
            get; set;
        }

        public string FullPath => $@"{ProjectPath}{ProjectName}{Extension}";

        public static Project Current => Application.Current.MainWindow.DataContext as Project;

        public static UndoRedo UndoRedo { get; } = new UndoRedo();

        public ICommand Undo { get; private set; }
        public ICommand Redo { get; private set; }

        public ICommand AddScene { get; private set; }
        public ICommand RemoveScene { get; private set; }

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; private set; }

        private Scene activeScene;

        [DataMember]
        public Scene ActiveScene
        {
            get { return activeScene; }
            set
            {
                if (activeScene != value)
                {
                    activeScene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                } 
            }
        }

        public Project() { }

        public Project(string name, string path)
        {
            ProjectName = name;
            ProjectPath = path;

            scenes.Add(new Scene("Default Scene", this));
        }

        public void UnloadProject() 
        {
            
        }

        public static Project LoadProject(string file) 
        {
            Debug.Assert(File.Exists(file));

            return Serializer.FromFile<Project>(file);
        }

        public static void SaveProject(Project project)
        {
            Serializer.ToFile<Project>(project, project.FullPath);
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context) 
        {
            if (scenes != null)
            {
                Scenes = new ReadOnlyObservableCollection<Scene>(scenes);
                OnPropertyChanged(nameof(Scenes));
            }

            ActiveScene = Scenes.FirstOrDefault(x => x.IsActive);

            AddScene = new RelayCommand<object>(x => 
            {
                AddSceneInternal($"New Scene {scenes.Count}");
                
                var newScene = scenes.Last();
                int index = scenes.Count - 1;

                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveSceneInternal(newScene), 
                    () => scenes.Insert(index, newScene), 
                    $"Add Scene: {newScene.SceneName}"));
            });

            RemoveScene = new RelayCommand<Scene>(x =>
            {
                int index = scenes.IndexOf(x);
                RemoveSceneInternal(x);

                UndoRedo.Add(new UndoRedoAction(
                    () => scenes.Insert(index, x),
                    () => RemoveSceneInternal(x),
                    $"Remove Scene: {x.SceneName}"));
            }, y => !y.IsActive);

            Undo = new RelayCommand<object>(x =>
            {
                UndoRedo.Undo();
            });

            Redo = new RelayCommand<object>(x =>
            {
                UndoRedo.Redo();
            });
        }

        private void AddSceneInternal(string sceneName) 
        {
            Debug.Assert(!string.IsNullOrEmpty(sceneName.Trim()));

            scenes.Add(new Scene(sceneName, this));
        }

        private void RemoveSceneInternal(Scene scene) 
        {
            Debug.Assert(scene != null && scenes.Contains(scene));

            scenes.Remove(scene);
        }
    }
}
