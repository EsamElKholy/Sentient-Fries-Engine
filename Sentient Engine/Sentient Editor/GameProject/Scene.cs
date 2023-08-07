using Sentient_Editor.Components;
using Sentient_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Sentient_Editor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private string sceneName;

        [DataMember(Name = nameof(GameEntities))]
        private ObservableCollection<GameEntity> gameEntities = new ObservableCollection<GameEntity>();
        public ReadOnlyObservableCollection<GameEntity> GameEntities { get; private set; }

        public ICommand AddGameEntityCommand { get; set; } 
        public ICommand RemoveGameEntityCommand { get; set; } 

        [DataMember]
        public string SceneName
        {
            get { return sceneName; }
            set
            {
                if (sceneName != value)
                {
                    sceneName = value;
                    OnPropertyChanged(nameof(SceneName));
                }
            }
        }

        [DataMember]
        public Project Project { get; private set; }
        
        private bool isActive;
        
        [DataMember]
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                if (isActive != value)
                {
                    isActive = value;
                    OnPropertyChanged(nameof(IsActive));
                }
            }
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (gameEntities != null)
            {
                GameEntities = new ReadOnlyObservableCollection<GameEntity>(gameEntities);
                OnPropertyChanged(nameof(GameEntities));
            }

            AddGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                AddEntity(x);

                int index = gameEntities.Count - 1;

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => RemoveEntity(x),
                    () => gameEntities.Insert(index, x),
                    $"Add Game Entity: {x.Name} to scene {SceneName}"));
            });

            RemoveGameEntityCommand = new RelayCommand<GameEntity>(x =>
            {
                int index = gameEntities.IndexOf(x);
                RemoveEntity(x);

                Project.UndoRedo.Add(new UndoRedoAction(
                    () => gameEntities.Insert(index, x),
                    () => RemoveEntity(x),
                    $"Remove Game Entity: {x.Name} from scene {SceneName}"));
            });            
        }

        public Scene(string name, Project project)
        {
            Debug.Assert(project != null);
            SceneName = name;
            Project = project;

            OnDeserialized(new StreamingContext());
        }

        private void AddEntity(GameEntity gameEntity) 
        {
            Debug.Assert(gameEntity != null && !gameEntities.Contains(gameEntity));

            gameEntities.Add(gameEntity);
        }

        private void RemoveEntity(GameEntity gameEntity) 
        {
            Debug.Assert(gameEntity != null && gameEntities.Contains(gameEntity));

            gameEntities.Remove(gameEntity);
        }
    }
}
