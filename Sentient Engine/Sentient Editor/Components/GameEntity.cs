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

    public abstract class MultiSelectionEntity : ViewModelBase 
    {
        private bool enableUpdates = true;

        private bool? isEnabled = true;

        public bool? IsEnabled
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

        private ObservableCollection<IMultiSelectionComponent> components = new ObservableCollection<IMultiSelectionComponent>();
        public ReadOnlyObservableCollection<IMultiSelectionComponent> Components { get; private set; }

        public List<GameEntity> SelectedEntities { get; } = new List<GameEntity>();

        public MultiSelectionEntity(List<GameEntity> entities)
        {
            Debug.Assert(entities?.Any() == true);

            Components = new ReadOnlyObservableCollection<IMultiSelectionComponent>(components);
            SelectedEntities = entities;
            PropertyChanged += (s, e) => 
            {
                if (enableUpdates) 
                {
                    UpdateGameEntities(e.PropertyName);
                }
            };
        }

        protected virtual bool UpdateGameEntities(string name) 
        {
            switch (name)
            {
                case nameof(Name): SelectedEntities.ForEach(e => e.Name = Name); return true;
                case nameof(IsEnabled): SelectedEntities.ForEach(e => e.IsEnabled = IsEnabled.Value); return true;
            }

            return false;
        }

        public void Refresh() 
        {
            enableUpdates = false;
            UpdateGameEntity();
            enableUpdates = true;
        }

        protected virtual bool UpdateGameEntity() 
        {
            IsEnabled = GetMixedValue(SelectedEntities, new Func<GameEntity, bool>(x => x.IsEnabled));
            Name = GetMixedValue(SelectedEntities, new Func<GameEntity, string>(x => x.Name));

            return true;
        }

        public static float? GetMixedValue(List<GameEntity> gameEntities, Func<GameEntity, float> GetProperty) 
        {
            var firstVal = GetProperty(gameEntities.First());

            foreach (var entity in gameEntities.Skip(1))
            {
                if (!firstVal.IsTheSameAs(GetProperty(entity)))
                {
                    return null;
                }
            }

            return firstVal;
        }

        public static bool? GetMixedValue(List<GameEntity> gameEntities, Func<GameEntity, bool> GetProperty)
        {
            var firstVal = GetProperty(gameEntities.First());

            foreach (var entity in gameEntities.Skip(1))
            {
                if (firstVal != GetProperty(entity))
                {
                    return null;
                }
            }

            return firstVal;
        }

        public static string GetMixedValue(List<GameEntity> gameEntities, Func<GameEntity, string> GetProperty)
        {
            var firstVal = GetProperty(gameEntities.First());

            foreach (var entity in gameEntities.Skip(1))
            {
                if (!firstVal.Equals(GetProperty(entity)))
                {
                    return null;
                }
            }

            return firstVal;
        }
    }

    public class MultiSelectionGameEntity : MultiSelectionEntity
    {
        public MultiSelectionGameEntity(List<GameEntity> entities) : base(entities)
        {
            Refresh();
        }
    }
}
