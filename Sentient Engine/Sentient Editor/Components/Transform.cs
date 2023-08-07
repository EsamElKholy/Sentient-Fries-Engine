using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sentient_Editor.Components
{
    [DataContract]
    public class Transform : Component
    {
        private Vector3 position;
        [DataMember]
        public Vector3 Position 
        {
            get { return position; }
            set
            {
                if (position != value)
                {
                    position = value; 

                    OnPropertyChanged(nameof(Position));
                }
            }
        }

        private Vector3 rotation;
        [DataMember]
        public Vector3 Rotation
        {
            get { return rotation; }
            set
            {
                if (rotation != value)
                {
                    rotation = value;

                    OnPropertyChanged(nameof(Rotation));
                }
            }
        }

        private Vector3 scale;
        [DataMember]
        public Vector3 Scale
        {
            get { return scale; }
            set
            {
                if (scale != value)
                {
                    scale = value;

                    OnPropertyChanged(nameof(Scale));
                }
            }
        }

        public Transform(GameEntity owner) : base(owner)
        {
        }
    }
}
