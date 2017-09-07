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
    using System.Collections;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.IO;
    using System.Net;
    using System.Net.Sockets;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;

    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        bool customProject = false;
        bool optOut = false;
        ObservableCollection<string> projectRankings = new ObservableCollection<string>();

        public event PropertyChangedEventHandler PropertyChanged;

        public MainWindow()
        {
            InitializeComponent();

            this.DataContext = this;

            this.LoadProjects();
        }

        #region Properties
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

        // Read the list of project names from the embedded text file
        private void LoadProjects()
        {
            string[] projectNames = Properties.Resources.ProjectList.Split(new string[] { Environment.NewLine }, StringSplitOptions.None);

            foreach (string project in projectNames)
            {
                this.ProjectsListBox.Items.Add(project);
            }
        }

        // Create a text file for the user to submit, and optionally send anonymized file for data analysis
        private void Export()
        {
            string message = string.Empty;

            using (StreamWriter outputFile = new StreamWriter(@"./COP4934ProjectRankings.txt"))
            {
                outputFile.WriteLine(this.UserName.Text);

                int count = 0;
                foreach (string line in ProjectRankings)
                {
                    outputFile.WriteLine(count + ": " + line);
                    count++;
                }

                message += "Successfully wrote " + count + " rankings to " + Directory.GetCurrentDirectory() + "/COP4934ProjectRankings.txt";
            }

            if (!OptOut)
            {
                using (StreamWriter outputFile = new StreamWriter(@"./AnonymizedProjectRankings.txt"))
                {
                    int count = 0;
                    foreach (string line in ProjectRankings)
                    {
                        outputFile.WriteLine(count + ": " + line);
                        count++;
                    }
                }

                StreamReader sr = new StreamReader(@"./AnonymizedProjectRankings.txt");

                TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(new IPEndPoint(IPAddress.Parse("192.168.1.83"), 5442));

                byte[] buffer = new byte[1500];
                long bytesSent = 0;

                while (bytesSent < sr.BaseStream.Length)
                {
                    int bytesRead = sr.BaseStream.Read(buffer, 0, 1500);
                    tcpClient.GetStream().Write(buffer, 0, bytesRead);
                    Console.WriteLine(bytesRead + " bytes sent.");

                    bytesSent += bytesRead;
                }

                tcpClient.Close();

                message += "\r\nSuccessfully sent " + bytesSent + " bytes.";
            }

            MessageBox.Show(message);
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
            // Copy the selected items to an array because we modify the collection in the foreach
            string[] selectedItems = new string[this.ProjectsListBox.SelectedItems.Count];
            this.ProjectsListBox.SelectedItems.CopyTo(selectedItems, 0);

            foreach (string project in selectedItems)
            {
                this.ProjectsListBox.Items.Remove(project);
                this.ProjectRankings.Add(project);
            }
        }

        private void RemoveProjectButton_Click(object sender, RoutedEventArgs e)
        {
            // Copy the selected items to an array because we modify the collection in the foreach
            string[] selectedItems = new string[this.ProjectRankingsListBox.SelectedItems.Count];
            this.ProjectRankingsListBox.SelectedItems.CopyTo(selectedItems, 0);

            foreach (string project in selectedItems)
            {
                this.ProjectRankings.Remove(project);
                this.ProjectsListBox.Items.Add(project);
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
                ProjectsListBox.Items.Add("Custom: " + customProjectName);
                ProjectRankings.Insert(0, "Custom: " + customProjectName);
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
