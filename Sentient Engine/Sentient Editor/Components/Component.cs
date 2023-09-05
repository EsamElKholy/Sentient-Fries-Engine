using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sentient_Editor.Components
{
    public interface IMultiSelectionComponent { }
    
    [DataContract]
    public abstract class Component : ViewModelBase
    {

        [DataMember]
        public GameEntity Owner { get; private set; }

        public Component(GameEntity owner)
        {
            Debug.Assert(owner != null);

            Owner = owner;
        }
    }

    public abstract class MultiSelectionComponent<T> : ViewModelBase, IMultiSelectionComponent where T : Component 
    {
    
    }
}
