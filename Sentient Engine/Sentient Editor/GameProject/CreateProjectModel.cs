using Sentient_Editor;
using Sentient_Editor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Sentient_Editor.GameProject
{
    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string? ProjectType { get; set; }
        [DataMember]
        public string? ProjectFile { get; set; }
        [DataMember]
        public List<string>? Folders { get; set; }

        [DataMember]
        public byte[]? Icon { get; set; }
        [DataMember]
        public byte[]? PreviewImage { get; set; }

        [DataMember]
        public string? IconFilePath { get; set; }
        [DataMember]
        public string? PreviewImageFilePath { get; set; }
        [DataMember]
        public string? ProjectFilePath { get; set; }
    }

    public class CreateProjectModel : ViewModelBase
    {
        private readonly string projectTemplatePath = @"..\..\..\Sentient Editor\Project Templates\";
        
        private ObservableCollection<ProjectTemplate> projectTemplates = new ObservableCollection<ProjectTemplate>();

        private string projectName = "New Project";
        private string projectPath = $@"..\..\Sentient Projects\";
        private bool isValid = false;
        private string errorMessage;

        public string ProjectName 
        {
            get { return projectName; } 
            set 
            {
                if (projectName != value)
                {
                    projectName = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(projectName));
                }
            }
        }

        public string ProjectPath
        {
            get { return projectPath; }
            set
            {
                if (projectPath != value)
                {
                    projectPath = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        public bool IsValid
        {
            get { return isValid; }
            set
            {
                if (isValid != value)
                {
                    isValid = value;
                    OnPropertyChanged(nameof(IsValid));
                }
            }
        }

        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if (errorMessage != value)
                {
                    errorMessage = value;
                    OnPropertyChanged(nameof(ErrorMessage));
                }
            }
        }

        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public CreateProjectModel()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(projectTemplates);

            try
            {
                var templates = Directory.GetFiles(projectTemplatePath, "template.xml", SearchOption.AllDirectories);

                Debug.Assert(templates.Any());

                foreach (var file in templates)
                {
                    //var template = new ProjectTemplate() 
                    //{
                    //    ProjectType = "Empty Project",
                    //    ProjectFile = "project.sentient",
                    //    Folders = new List<string>() 
                    //    {
                    //        ".Sentient",
                    //        "Content",
                    //        "Code"
                    //    }
                    //};

                    //Serializer.ToFile(template, file);
                    var template = Serializer.FromFile<ProjectTemplate>(file);

                    if (template != null)
                    {
                        template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                        template.PreviewImageFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "preview.png"));

                        template.Icon = File.ReadAllBytes(template.IconFilePath);
                        template.PreviewImage = File.ReadAllBytes(template.PreviewImageFilePath);

                        template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));

                        projectTemplates.Add(template);
                    }
                    else
                    {
                        Debug.WriteLine("Template was null");
                    }
                }

                ValidateProjectPath();
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                //throw;
            }
        }

        private bool ValidateProjectPath() 
        {
            var path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path))
            {
                path += @"\";
            }

            path += $@"{ProjectName}\";

            IsValid = false;

            if (string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMessage = "Please type in a project name."; 
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used!";
            }
            if (string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMessage = "Please type in a project path.";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMessage = "Invalid character(s) used!";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMessage = "Selected path already exists and is not empty.";
            }
            else
            {
                ErrorMessage = String.Empty;
                IsValid = true;
            }

            return IsValid;
        }

        public string CreateProject(ProjectTemplate projectTemplate) 
        {
            ValidateProjectPath();

            if (!IsValid)
            {
                return string.Empty;
            }

            if (!Path.EndsInDirectorySeparator(ProjectPath))
            {
                ProjectPath += @"\";
            }

            var path = $@"{ProjectPath}{ProjectName}\";

            try
            {
                if (!Directory.Exists(ProjectPath))
                {
                    Directory.CreateDirectory(ProjectPath);                   
                }

                string hiddenFolder = string.Empty;
                foreach (var folder in projectTemplate.Folders)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path), folder)));
                    if (folder[0] == '.')
                    {
                        hiddenFolder = folder;
                    }
                }

                var directoryInfo = new DirectoryInfo(path + hiddenFolder);
                directoryInfo.Attributes |= FileAttributes.Hidden;

                File.Copy(projectTemplate.IconFilePath, Path.GetFullPath(Path.Combine(directoryInfo.FullName, "icon.png")));
                File.Copy(projectTemplate.PreviewImageFilePath, Path.GetFullPath(Path.Combine(directoryInfo.FullName, "preview.png")));

                var projectXML = File.ReadAllText(projectTemplate.ProjectFilePath);
                projectXML = String.Format(projectXML, ProjectName, Path.GetFullPath(ProjectPath));
                var projectPath = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(projectPath, projectXML);

                //Project project = new Project(ProjectName, path);

                //Serializer.ToFile(project, path + $"{ProjectName}" + Project.Extension);

                return Path.GetFullPath(path);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
                //throw;
            }

            return String.Empty;
        }        
    }
}
