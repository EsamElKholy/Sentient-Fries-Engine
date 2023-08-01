using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

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
        }

        [DataMember]
        public string ProjectPath
        {
            get; set;
        }

        public string FullPath => $@"{ProjectPath}{ProjectName}{Extension}";

        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> scenes = new ObservableCollection<Scene>();
        public ReadOnlyObservableCollection<Scene> Scenes { get; }

        public Project(string name, string path)
        {
            ProjectName = name;
            ProjectPath = path;

            scenes.Add(new Scene("Default Scene", this));
        }
    }
}
