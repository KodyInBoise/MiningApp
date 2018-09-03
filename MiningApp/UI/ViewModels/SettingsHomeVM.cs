using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using static MiningApp.ServerHelper.VersionHelper;

namespace MiningApp.UI
{
    public enum SettingType
    {
        General,
        Mining,
        Updates,
    }


    public class SettingsHomeVM
    {
        public static SettingsHomeVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private GeneralVM _generalVM { get; set; }

        private MiningVM _miningVM { get; set; }

        private UpdatesVM _updatesVM { get; set; }


        public SettingsHomeVM()
        {
            Instance = this;

            Show();
        }

        private void Show()
        {
            _primaryVM = new PrimaryVM();

            DisplaySecondary(SettingType.General);
        }

        public void Dispose()
        {
            Instance = null;

            WindowController.Instance.PoolSetupView = null;
        }

        public void DisplayPrimary()
        {
            PrimaryGrid.Children.Clear();

            _primaryVM = new PrimaryVM();
        }

        public void DisplaySecondary(SettingType type)
        {
            SecondaryGrid.Children.Clear();

            switch (type)
            {
                case SettingType.General:
                    _generalVM = new GeneralVM();
                    break;
                case SettingType.Mining:
                    _miningVM = new MiningVM();
                    break;
                case SettingType.Updates:
                    _updatesVM = new UpdatesVM();
                    break;
                default:
                    break;
            }
        }

        public class PrimaryVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Settings", 40, width: ElementValues.Grids.PrimarySmall);

            Button GeneralButton { get; set; } = ElementHelper.CreateButton("General", height: buttonHeight);

            Button MiningButton { get; set; } = ElementHelper.CreateButton("Mining", height: buttonHeight);

            Button UpdatesButton { get; set; } = ElementHelper.CreateButton("Updates", height: buttonHeight);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            public PrimaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 10);
                nextTop = 75;

                DisplayElement(GeneralButton, topPadding: padding * 2);
                GeneralButton.Click += (s, e) => GeneralButton_Clicked();

                DisplayElement(MiningButton);
                MiningButton.Click += (s, e) => MiningButton_Clicked();

                DisplayElement(UpdatesButton);
                UpdatesButton.Click += (s, e) => UpdatesButton_Clicked();
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void GeneralButton_Clicked()
            {
                View.DisplaySecondary(SettingType.General);
            }

            void MiningButton_Clicked()
            {
                View.DisplaySecondary(SettingType.Mining);
            }

            void UpdatesButton_Clicked()
            {
                View.DisplaySecondary(SettingType.Updates);
            }
        }
        
        public class GeneralVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("General", 40, width: ElementValues.Grids.SecondaryNormal);

            CheckBox LaunchOnStartupCheckBox { get; set; } = ElementHelper.CreateCheckBox("Launch On Startup", width: 250, fontSize: 20);

            ComboBox LaunchConfigComboBox { get; set; } = ElementHelper.CreateComboBox("Launch", width: 325);

            Button SaveButton { get; set; } = ElementHelper.CreateButton("Save", height: 60, width: 150, style: ButtonStyleEnum.Finish);

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;

            private List<SessionConfigModel> _sessionConfigs { get; set; }


            public GeneralVM()
            {
                Show();
            }

            private async void Show()
            {
                DisplayElement(TitleTextBlock);

                nextLeft = nextLeft + padding * 4;
                DisplayElement(LaunchOnStartupCheckBox, topPadding: padding * 4);
                LaunchOnStartupCheckBox.IsChecked = Bootstrapper.Settings.General.LaunchOnStartup;
                LaunchOnStartupCheckBox.Click += (s, e) => LaunchOnStartupCheckBox_Clicked();

                nextLeft = LaunchOnStartupCheckBox.Margin.Left + LaunchOnStartupCheckBox.Width + padding * 4;
                nextTop = LaunchOnStartupCheckBox.Margin.Top;
                DisplayElement(LaunchConfigComboBox, topPadding: 7);
                LaunchConfigComboBox.Visibility = LaunchOnStartupCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;

                nextLeft = ElementValues.Grids.SecondaryNormal - SaveButton.Width - padding;
                nextTop = ViewGrid.Height - SaveButton.Height - padding;
                DisplayElement(SaveButton);
                SaveButton.Click += (s, e) => SaveButton_Clicked();

                _sessionConfigs = await DataHelper.Instance.GetAllConfigs();
                LaunchConfigComboBox.ItemsSource = _sessionConfigs;

                if (_sessionConfigs.Any())
                {
                    var configID = Bootstrapper.Settings.General.LaunchConfigID;
                    if (configID > 0)
                    {
                        var selectedConfig = _sessionConfigs.FirstOrDefault(x => x.ID == configID);
                        LaunchConfigComboBox.SelectedItem = selectedConfig;
                    }
                    else
                    {
                        LaunchConfigComboBox.SelectedIndex = 0;
                    }
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void LaunchOnStartupCheckBox_Clicked()
            {
                LaunchConfigComboBox.Visibility = LaunchOnStartupCheckBox.IsChecked == true ? Visibility.Visible : Visibility.Collapsed;
            }

            void SaveButton_Clicked()
            {
                Bootstrapper.Settings.General.LaunchOnStartup = LaunchOnStartupCheckBox.IsChecked ?? false;
                if (LaunchOnStartupCheckBox.IsChecked == true)
                {
                    var selectedConfig = (SessionConfigModel)LaunchConfigComboBox.SelectedItem;

                    Bootstrapper.Settings.General.LaunchOnStartup = true;
                    Bootstrapper.Settings.General.LaunchConfigID = selectedConfig != null ? selectedConfig.ID : -1;
                }
                else
                {
                    Bootstrapper.Settings.General.LaunchOnStartup = false;
                    Bootstrapper.Settings.General.LaunchConfigID = -1;
                }

                Bootstrapper.Instance.SaveLocalSettings();
            }
        }

        public class MiningVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Mining", 40, width: ElementValues.Grids.SecondaryNormal);

            TextBlock ProcessesTextBlock { get; set; } = ElementHelper.CreateTextBlock("Blacklisted Processes", fontSize: 20, width: ElementValues.Grids.SecondaryNormal, height: 20);

            TextBox NewProcessTextBox { get; set; } = ElementHelper.CreateTextBox("NewProcess", width: 350);

            TextBox PreviewBlacklistTextBox { get; set; } = ElementHelper.CreateTextBox("PreviewBlacklist", width: 525, height: 225, 
                readOnly: true, contentVerticalAlignment: VerticalAlignment.Top);

            ListBox ProcessesListBox { get; set; } = ElementHelper.CreateListBox("Processes", fontSize: 14, height: 125, width: 525);

            Button BrowseFilesButton { get; set; } = ElementHelper.CreateButton("Add File", name: "BrowseFiles", fontSize: 14, style: ButtonStyleEnum.New,
                width: 100, height: ElementValues.TextBoxs.Height);

            Button BrowseFoldersButton { get; set; } = ElementHelper.CreateButton("Add Folder", name: "BrowseFolders", fontSize: 14, style: ButtonStyleEnum.New,
                width: 100, height: ElementValues.TextBoxs.Height);

            Button ProcessRemoveButton { get; set; } = ElementHelper.CreateButton("Remove", name: "ProcsRemove", fontSize: 14, style: ButtonStyleEnum.Delete,
                width: 100, height: ElementValues.TextBoxs.Height);

            Button PreviewBlacklistButton { get; set; } = ElementHelper.CreateButton("Preview", name: "PreviewBlacklist", fontSize: 14, style: ButtonStyleEnum.Normal,
                width: 100, height: ElementValues.TextBoxs.Height);

            CheckBox UseBlacklistCheckBox { get; set; } = ElementHelper.CreateCheckBox("Use Blacklist to start and stop miners");

            Button SaveButton { get; set; } = ElementHelper.CreateButton("Save", height: 60, width: 150, style: ButtonStyleEnum.Finish);

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;

            private List<BlacklistedItem> _blacklistedItems { get; set; } = Bootstrapper.Settings.Mining.BlacklistedItems.ToList();


            public MiningVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextLeft = nextLeft + padding * 4;
                DisplayElement(ProcessesTextBlock, topPadding: padding * 4);

                DisplayElement(ProcessesListBox);
                ProcessesListBox.ItemsSource = _blacklistedItems;

                DisplayElement(UseBlacklistCheckBox);
                UseBlacklistCheckBox.IsChecked = Bootstrapper.Settings.Mining.UseBlackList;

                nextLeft = ProcessesListBox.Margin.Left + ProcessesListBox.Width + padding;
                nextTop = ProcessesListBox.Margin.Top;
                DisplayElement(BrowseFilesButton);
                BrowseFilesButton.Click += (s, e) => BrowseFilesButtonButton_Clicked();

                nextTop = BrowseFilesButton.Margin.Top + BrowseFilesButton.Height + padding;
                DisplayElement(BrowseFoldersButton);
                BrowseFoldersButton.Click += (s, e) => BrowseFoldersButton_Clicked();

                nextTop = ProcessesListBox.Margin.Top + ProcessesListBox.Height - ProcessRemoveButton.Height;
                DisplayElement(ProcessRemoveButton);
                ProcessRemoveButton.Click += (s, e) => ProcessRemoveButton_Clicked();

                nextLeft = ProcessesListBox.Margin.Left + ProcessesListBox.Width - PreviewBlacklistButton.Width;
                nextTop = UseBlacklistCheckBox.Margin.Top;
                DisplayElement(PreviewBlacklistButton, topPadding: 7);
                PreviewBlacklistButton.Click += (s, e) => PreviewBlacklistButton_Clicked();

                nextLeft = ElementValues.Grids.SecondaryNormal - SaveButton.Width - padding;
                nextTop = ViewGrid.Height - SaveButton.Height - padding;
                DisplayElement(SaveButton);
                SaveButton.Click += (s, e) => SaveButton_Clicked();
            }

            void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void BrowseFilesButtonButton_Clicked()
            {
                var processPath = ElementHelper.GetFilePath();

                if (!String.IsNullOrEmpty(processPath))
                {
                    _blacklistedItems.Add(new BlacklistedItem(BlacklistedItemType.Executable, processPath));

                    ProcessesListBox.Items.Refresh();
                }
            }

            void BrowseFoldersButton_Clicked()
            {
                var path = ElementHelper.GetFolderPath();

                if (!String.IsNullOrEmpty(path))
                {
                    _blacklistedItems.Add(new BlacklistedItem(BlacklistedItemType.Directory, path));

                    ProcessesListBox.Items.Refresh();
                }
            }

            void ProcessRemoveButton_Clicked()
            {
                var blacklistedItem = (BlacklistedItem)ProcessesListBox.SelectedItem;
                _blacklistedItems.Remove(blacklistedItem);

                ProcessesListBox.Items.Refresh();
            }

            async void PreviewBlacklistButton_Clicked()
            {
                try
                {
                    if (ViewGrid.Children.Contains(PreviewBlacklistTextBox))
                    {
                        ViewGrid.Children.Remove(PreviewBlacklistTextBox);
                    }
                    else
                    {
                        nextLeft = ProcessesListBox.Margin.Left;
                        nextTop = PreviewBlacklistButton.Margin.Top + PreviewBlacklistButton.Height + padding * 2;
                        DisplayElement(PreviewBlacklistTextBox);

                        PreviewBlacklistTextBox.Text = "Checking...";
                        var previewBody = string.Empty;

                        var allBlacklistProcs = await Bootstrapper.Settings.Mining.GetAllBlacklistedProcesses();
                        foreach (var proc in allBlacklistProcs)
                        {
                            previewBody += $"\"{proc.ItemName}\" - Running: {proc.IsRunning} \r";
                        }

                        previewBody = !String.IsNullOrEmpty(previewBody) ? previewBody : "No processes to show!";
                        PreviewBlacklistTextBox.Text = previewBody;
                    }
                }
                catch (Exception ex)
                {
                    LogHelper.AddEntry(ex);
                }
            }

            void SaveButton_Clicked()
            {
                Bootstrapper.Settings.Mining.UseBlackList = UseBlacklistCheckBox.IsChecked == true;
                Bootstrapper.Settings.Mining.BlacklistedItems = _blacklistedItems.ToList();

                Bootstrapper.Instance.SaveLocalSettings();
            }
        }


        public class UpdatesVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Updates", 40, width: ElementValues.Grids.SecondaryNormal);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 250);

            TextBlock VersionTextBlock { get; set; } = ElementHelper.CreateTextBlock("Version: ", width: 700, height: 20, fontSize: 18);

            TextBlock ReleaseDateTextBlock { get; set; } = ElementHelper.CreateTextBlock("Release Date: ", width: 700, height: 20, fontSize: 18);

            TextBlock UrgencyTextBlock { get; set; } = ElementHelper.CreateTextBlock("Urgency: ", width: 700, height: 30, fontSize: 18);

            TextBox ReleaseNotesTextBox { get; set; } = ElementHelper.CreateTextBox("Output", height: 300, width: 500, fontSize: 12, readOnly: true);

            Button CheckNowButton { get; set; } = ElementHelper.CreateButton("Check Now", height: 60, width: 150, style: ButtonStyleEnum.Finish);

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            public UpdatesVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextTop = 75;
                DisplayElement(StatusTextBlock, leftPadding: 10);

                nextTop = 200;
                nextLeft = nextLeft + padding * 8;
                DisplayElement(VersionTextBlock, topPadding: padding * 2);

                DisplayElement(ReleaseDateTextBlock);

                DisplayElement(UrgencyTextBlock);

                DisplayElement(ReleaseNotesTextBox);

                nextTop = ViewGrid.Height - CheckNowButton.Height - padding;
                nextLeft = StatusTextBlock.Margin.Left;
                DisplayElement(CheckNowButton);
                CheckNowButton.Click += (s, e) => CheckNow_Clicked();

                DisplayCurrent();
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            async void CheckNow_Clicked()
            {
                var updatesAvailable = await Task.Run(ServerHelper.Instance.CheckForUpdates);

                if (updatesAvailable)
                {
                    ShowStatus($"New version available for download!");
                    DisplayUpdate(ServerHelper.Instance.GetUpdateVersion());
                }
                else
                {
                    DisplayCurrent();
                    ShowStatus("You are up to date!");
                }
            }

            void ShowStatus(string message)
            {
                StatusTextBlock.Text = message;
                StatusTextBlock.Visibility = Visibility.Visible;
            }

            void DisplayCurrent()
            {
                try
                {
                    StatusTextBlock.Visibility = Visibility.Collapsed;
                    UrgencyTextBlock.Visibility = Visibility.Collapsed;
                    ReleaseNotesTextBox.Visibility = Visibility.Collapsed;

                    VersionTextBlock.Text = $"Current Version: v{Bootstrapper.Settings.App.AppVersion.Number}";
                    ReleaseDateTextBlock.Text = $"Release Date: {Bootstrapper.Settings.App.AppVersion.ReleaseTimestamp.ToShortDateString()}";
                }
                catch (Exception ex)
                {
                    LogHelper.AddEntry(ex);
                }
            }

            void DisplayUpdate(VersionModel version)
            {
                UrgencyTextBlock.Visibility = Visibility.Visible;
                ReleaseNotesTextBox.Visibility = Visibility.Visible;

                VersionTextBlock.Text = $"New Version: v{version.Number}";
                ReleaseDateTextBlock.Text = $"Release Date: {version.ReleaseTimestamp.ToShortDateString()}";
                UrgencyTextBlock.Text = $"Urgency: {version.Urgency}";
                ReleaseNotesTextBox.Text = "";
            }
        }
    }
}
