using MiningApp.LoggingUtil;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public enum SettingType
    {
        General,
        Updates,
    }


    public class SettingsHomeVM
    {
        public static SettingsHomeVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private GeneralVM _generalVM { get; set; }

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

            Button SaveButton { get; set; } = ElementHelper.CreateButton("Save", height: 60, width: 150, style: ButtonStyle.Finish);

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


        public class UpdatesVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Updates", 40, width: ElementValues.Grids.SecondaryNormal);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);

            Button CheckNowButton { get; set; } = ElementHelper.CreateButton("Check Now", height: 60, width: 150, style: ButtonStyle.Finish);

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
                //StatusTextBlock.Visibility = Visibility.Collapsed;

                nextTop = ViewGrid.Height - CheckNowButton.Height - padding;
                DisplayElement(CheckNowButton);
                CheckNowButton.Click += (s, e) => CheckNow_Clicked();
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
                }
                else
                {
                    ShowStatus("You are up to date!");
                }
            }

            void ShowStatus(string message)
            {
                StatusTextBlock.Text = message;
                StatusTextBlock.Visibility = Visibility.Visible;
            }
        }
    }
}
