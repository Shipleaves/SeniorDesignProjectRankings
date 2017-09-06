// Austin Shipley
// 9/6/2017
// Senior Design Project Ranking

/* This project was created with the intent to assist UCF students in COP 4934 with
 * ranking the available Senior Design projects, as well as collect data on these
 * rankings to anonymize, analyze, and visualize.
 * The data will be shown to future project presenters/companies that don't know 
 * what project to do so that they can see what kind of projects had positive 
 * responses, as well as provide some solace for students who don't get their 
 * first choice because 50% of the class also had the same project as their 
 * first choice. 
 */

namespace Senior_Design_Project_Ranking
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        bool customProject = false;
        bool optOut = false;
        ObservableCollection<string> projectRankings = new ObservableCollection<string>();
        ObservableCollection<string> projects = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        #region Properties
        public ObservableCollection<string> Projects
        {
            get
            {
                return projects;
            }

            set
            {
                projects = value;
                OnPropertyChanged("Projects");
            }
        }

        public ObservableCollection<string> ProjectRankings
        {
            get
            {
                return projectRankings;
            }

            set
            {
                projectRankings = value;
                OnPropertyChanged("ProjectRankings");
            }
        }

        public bool CustomProject
        {
            get
            {
                return customProject;
            }

            set
            {
                customProject = value;
                OnPropertyChanged("CustomProject");
            }
        }

        public bool OptOut
        {
            get
            {
                return optOut;
            }

            set
            {
                optOut = value;
            }
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.LoadProjects();
        }

        // Read the list of project names from the embedded text file
        private void LoadProjects()
        {
            string[] projectNames = Properties.Resources.ProjectList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string project in projectNames)
            {
                this.Projects.Add(project);
            }
        }

        // Create a text file for the user to submit, and optionally send anonymized file for data analysis
        private void Export()
        {

        }

        /* UI Event handlers */

        private void IncreaseRankButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string project in this.ProjectRankingsListBox.SelectedItems)
            {
                int currentIndex = ProjectRankings.IndexOf(project);
                if (currentIndex > 0)
                {
                    ProjectRankings.Move(currentIndex, currentIndex - 1);
                }
            }
        }

        private void DecreaseRankButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string project in this.ProjectRankingsListBox.SelectedItems)
            {
                int currentIndex = ProjectRankings.IndexOf(project);
                if (currentIndex < ProjectRankings.Count - 1)
                {
                    ProjectRankings.Move(currentIndex, currentIndex + 1);
                }
            }
        }

        private void AddProjectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string project in this.ProjectsListBox.SelectedItems)
            {
                this.Projects.Remove(project);
                this.ProjectRankings.Add(project);
            }
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            foreach (string project in this.ProjectRankingsListBox.SelectedItems)
            {
                this.ProjectRankings.Remove(project);
                this.Projects.Add(project);
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            this.Export();
        }

        private void ProjectsListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Load project description
        }

        private void CustomProjectName_LostFocus(object sender, RoutedEventArgs e)
        {
            string customProjectName = ((TextBox)sender).Text;

            if (!string.IsNullOrWhiteSpace(customProjectName))
            {
                Projects.Add("0: " + customProjectName);
                ProjectRankings.Insert(0, "0: " + customProjectName);
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
