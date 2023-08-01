using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Sentient_Editor.GameProject
{
    [DataContract]
    public class Scene : ViewModelBase
    {
        private string sceneName;
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

        public Scene(string name, Project project)
        {
            Debug.Assert(project != null);
            SceneName = name;
            Project = project;
        }
    }
}
