﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Xceed.Wpf.Toolkit;

namespace MiningApp.UI
{
    public class NavBarVM
    {
        public static NavBarVM Instance { get; set; }


        Grid ViewGrid => MainWindow.Instance.NavGrid;

        List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();

        Button HomeButton { get; set; } = ElementHelper.CreateButton("Home");

        Button ConfigurationsButton { get; set; } = ElementHelper.CreateButton("Configurations");

        Button MinersButton { get; set; } = ElementHelper.CreateButton("Miners");

        Button WalletsButton { get; set; } = ElementHelper.CreateButton("Wallets");

        Button PoolsButton { get; set; } = ElementHelper.CreateButton("Pools");

        Button LogsButton { get; set; } = ElementHelper.CreateButton("Logs");

        Button SettingsButton { get; set; } = ElementHelper.CreateButton("Settings");


        double nextLeft = 12;

        double nextTop = 13;

        double padding = 25;


        public NavBarVM()
        {
            Instance = this;

            Show();

            WindowController.Instance.ShowHome();
        }

        private void Show()
        {
            DisplayElement(HomeButton);
            HomeButton.Click += (s, e) => HomeButton_Clicked();

            DisplayElement(ConfigurationsButton, topPadding: padding * 2);
            ConfigurationsButton.Click += (s, e) => ConfigurationsButton_Clicked();

            DisplayElement(MinersButton);
            MinersButton.Click += (s, e) => MinersButton_Clicked();

            DisplayElement(WalletsButton);
            WalletsButton.Click += (s, e) => WalletsButton_Clicked();

            DisplayElement(PoolsButton);
            PoolsButton.Click += (s, e) => PoolsButton_Clicked();

            DisplayElement(LogsButton);
            LogsButton.Click += (s, e) => LogsButton_Clicked();

            DisplayElement(SettingsButton, topPadding: padding * 2);
            SettingsButton.Click += (s, e) => SettingsButton_Clicked();
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);
            ActiveElements.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        private void HomeButton_Clicked()
        {
            WindowController.Instance.ShowHome();
        }

        private void ConfigurationsButton_Clicked()
        {
            WindowController.Instance.ShowConfigurationsHome();
        }

        private void MinersButton_Clicked()
        {
            WindowController.Instance.ShowMinersHome();
        }

        private void WalletsButton_Clicked()
        {
            WindowController.Instance.ShowWalletsHome();
        }

        private void PoolsButton_Clicked()
        {
            WindowController.Instance.ShowPoolsHome();
        }

        private async void LogsButton_Clicked()
        {
            WindowController.Instance.ShowLogsHome();
        }

        private void SettingsButton_Clicked()
        {
            WindowController.Instance.ShowSettingsHome();
        }
    }
}
