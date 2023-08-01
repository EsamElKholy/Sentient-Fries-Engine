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

namespace Sentient_Editor.GameProject
{
    [DataContract]
    public class ProjectData 
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        [DataMember]
        public DateTime Date { get; set; }

        public string FullPath { get { return $@"{ProjectPath}{ProjectName}{Project.Extension}"; } }

        public byte[]? Icon { get; set; }
        public byte[]? PreviewImage { get; set; } 
    }

    [DataContract]
    public class ProjectDataList
    {
        [DataMember]
        public List<ProjectData> Projects { get; set; }
    }

    public class OpenProjectModel 
    {
        private static readonly string applicationDataPath = $@"..\..\Sentient Projects Data\";
        private static readonly string projectDataPath;
        private static readonly ObservableCollection<ProjectData> projects = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProjectModel()
        {
            try
            {
                if (!Directory.Exists(applicationDataPath))
                {
                    Directory.CreateDirectory(applicationDataPath);
                }

                OpenProjectModel.projectDataPath = $@"{applicationDataPath}ProjectData.xml";

                Projects = new ReadOnlyObservableCollection<ProjectData>(projects);

                ReadProjectData();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }

        private static void ReadProjectData() 
        {
            if (File.Exists(projectDataPath))
            {
                var orderedProjects = Serializer.FromFile<ProjectDataList>(projectDataPath).Projects.OrderByDescending(x => x.Date).ToList();

                projects.Clear();

                foreach (var project in orderedProjects) 
                {
                    if (Directory.Exists(project.ProjectPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\.Sentient\icon.png");
                        project.PreviewImage = File.ReadAllBytes($@"{project.ProjectPath}\.Sentient\preview.png");

                        projects.Add(project);
                    }
                }
            }
        }

        private static void WriteProjectData()
        {
            var orderedProjects = projects.OrderBy(x => x.Date).ToList();

            Serializer.ToFile(new ProjectDataList() { Projects = orderedProjects }, projectDataPath);
        }

        public static Project OpenProject(ProjectData projectData) 
        {
            ReadProjectData();

            var project = projects.FirstOrDefault(x => x.ProjectPath == projectData.ProjectPath);

            if (project == null) 
            {
                project = projectData;
                project.Date = DateTime.Now;
                projects.Add(project);
            }
            else
            {
                project.Date = DateTime.Now;
            }

            WriteProjectData();

            return Project.LoadProject(project.FullPath);
        }
    }
}
