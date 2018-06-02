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

    }


    public class SettingsHomeVM
    {
        public static SettingsHomeVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private GeneralVM _generalVM { get; set; }


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


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<PoolConfigModel> _pools { get; set; } = new List<PoolConfigModel>();

            private List<Button> _poolButtons { get; set; } = new List<Button>();

            private Dictionary<Button, int> _buttonDictionary { get; set; } = new Dictionary<Button, int>();


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

                DisplayExisting();
            }

            private async void DisplayExisting()
            {

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
        }
        
        public class GeneralVM
        {
            SettingsHomeVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("General", 40, width: ElementValues.Grids.SecondaryNormal);

            CheckBox LaunchOnStartupCheckBox { get; set; } = ElementHelper.CreateCheckBox("Launch On Startup", fontSize: 22); 


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
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }
        }

    }
}
