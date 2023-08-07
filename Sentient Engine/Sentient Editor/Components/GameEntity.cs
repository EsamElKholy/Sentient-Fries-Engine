using Sentient_Editor.GameProject;
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

namespace Sentient_Editor.Components
{
    [DataContract]
    [KnownType(typeof(Transform))]
    public class GameEntity : ViewModelBase
    {
        private bool isEnabled = true;

        [DataMember]
        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (isEnabled != value)
                {
                    isEnabled = value;
                    OnPropertyChanged(nameof(IsEnabled));
                }
            }
        }

        private string name;

        [DataMember]
        public string Name 
        {
            get { return name; }
            set 
            {
                if (name != value)
                {
                    name = value; 
                    OnPropertyChanged(nameof(Name));
                }
            }
        }

        [DataMember]
        public Scene ParentScene { get; private set; }

        [DataMember(Name = nameof(Components))]
        private  ObservableCollection<Component> components = new ObservableCollection<Component>();
        public ReadOnlyObservableCollection<Component> Components { get; private set; }

        public ICommand RenameCommand { get; private set; }
        public ICommand EnableCommand { get; private set; }

        public GameEntity(Scene scene)
        {
            Debug.Assert(scene != null);

            ParentScene = scene;

            components.Add(new Transform(this));

            OnDeserialized(new StreamingContext());
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (components != null)
            {
                Components = new ReadOnlyObservableCollection<Component>(components);
                OnPropertyChanged(nameof(Components));
            }

            RenameCommand = new RelayCommand<string>(x =>
            {
                var oldName = Name;
                Name = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(Name), this, oldName, x, $"Rename entity {oldName} to {x}"));
            }, y => y != name);

            EnableCommand = new RelayCommand<bool>(x =>
            {
                var oldEnabled = Name;
                IsEnabled = x;
                Project.UndoRedo.Add(new UndoRedoAction(nameof(IsEnabled), this, oldEnabled, x, $"Set entity enabled from {oldEnabled} to {x}"));
            });

            //AddComponentCommand = new RelayCommand<Component>(x =>
            //{
            //    AddComponent(x);

            //    int index = components.Count - 1;

            //    Project.UndoRedo.Add(new UndoRedoAction(
            //        () => RemoveComponent(x),
            //        () => components.Insert(index, x),
            //        $"Add Component: {nameof(x)} to game entity {Name}"));
            //});

            //RemoveComponentCommand = new RelayCommand<Component>(x =>
            //{
            //    int index = components.IndexOf(x);
            //    RemoveComponent(x);

            //    Project.UndoRedo.Add(new UndoRedoAction(
            //        () => components.Insert(index, x),
            //        () => RemoveComponent(x),
            //        $"Remove Component: {nameof(x)} to game entity {Name}"));
            //});
        }

        private void AddComponent(Component component) 
        {
            Debug.Assert(component != null);

            components.Add(component);
        }

        private void RemoveComponent(Component component) 
        {
            Debug.Assert(component != null);

            components.Remove(component);
        }
    }
}
