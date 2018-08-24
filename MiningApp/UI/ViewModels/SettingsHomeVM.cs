using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
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

            CheckBox LaunchOnStartupCheckBox { get; set; } = ElementHelper.CreateCheckBox("Launch On Startup", fontSize: 22);

            Button SaveButton { get; set; } = ElementHelper.CreateButton("Save", height: 60, width: 150, style: ButtonStyleEnum.Finish);

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            public GeneralVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextLeft = nextLeft + padding * 4;
                DisplayElement(LaunchOnStartupCheckBox, topPadding: padding * 4);
                LaunchOnStartupCheckBox.IsChecked = Bootstrapper.Settings.General.LaunchOnStartup;

                nextLeft = ElementValues.Grids.SecondaryNormal - SaveButton.Width - padding;
                nextTop = ViewGrid.Height - SaveButton.Height - padding;
                DisplayElement(SaveButton);
                SaveButton.Click += (s, e) => SaveButton_Clicked();
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void SaveButton_Clicked()
            {
                Bootstrapper.Settings.General.LaunchOnStartup = LaunchOnStartupCheckBox.IsChecked ?? false;

                Bootstrapper.Instance.SaveLocalSettings();
            }
        }

        public class MiningVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Mining", 40, width: ElementValues.Grids.SecondaryNormal);

            TextBlock ProcessesTextBlock { get; set; } = ElementHelper.CreateTextBlock("Blacklisted Processes", fontSize: 18, width: ElementValues.Grids.SecondaryNormal, height: 18);

            TextBox NewProcessTextBox { get; set; } = ElementHelper.CreateTextBox("NewProcess", width: 350);

            ListBox ProcessesListBox { get; set; } = ElementHelper.CreateListBox("Processes", fontSize: 14, height: 125, width: 525);

            Button ProcessBrowseButton { get; set; } = ElementHelper.CreateButton("Browse", name: "ProcsBrowse", fontSize: 14, style: ButtonStyleEnum.Normal,
                width: 80, height: ElementValues.TextBoxs.Height);

            Button ProcessRemoveButton { get; set; } = ElementHelper.CreateButton("Remove", name: "ProcsRemove", fontSize: 14, style: ButtonStyleEnum.Delete,
                width: 80, height: ElementValues.TextBoxs.Height);

            Button SaveButton { get; set; } = ElementHelper.CreateButton("Save", height: 60, width: 150, style: ButtonStyleEnum.Finish);

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


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

                nextLeft = ProcessesListBox.Margin.Left + ProcessesListBox.Width + padding;
                nextTop = ProcessesListBox.Margin.Top;
                DisplayElement(ProcessBrowseButton);

                nextTop = ProcessesListBox.Margin.Top + ProcessesListBox.Height - ProcessRemoveButton.Height;
                DisplayElement(ProcessRemoveButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - SaveButton.Width - padding;
                nextTop = ViewGrid.Height - SaveButton.Height - padding;
                DisplayElement(SaveButton);
                SaveButton.Click += (s, e) => SaveButton_Clicked();
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            void SaveButton_Clicked()
            {
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
                //WindowController.Instance.Testing();
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
                StatusTextBlock.Visibility = Visibility.Collapsed;
                UrgencyTextBlock.Visibility = Visibility.Collapsed;
                ReleaseNotesTextBox.Visibility = Visibility.Collapsed;

                VersionTextBlock.Text = $"Current Version: v{Bootstrapper.Settings.App.AppVersion.Number}";
                ReleaseDateTextBlock.Text = $"Release Date: {Bootstrapper.Settings.App.AppVersion.ReleaseTimestamp.ToShortDateString()}";
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
