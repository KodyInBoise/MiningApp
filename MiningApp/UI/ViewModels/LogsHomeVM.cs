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
    public class LogsHomeVM
    {
        TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Logs Home", 40, width: 250);

        Grid ViewGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        TextBlock LogTypeTextBlock { get; set; } = ElementHelper.CreateTextBlock("Category", fontSize: 22, height: 30, width: 130);

        ComboBox LogTypeComboBox { get; set; } = ElementHelper.CreateComboBox("Category", width: 200);

        DataGrid DataGrid { get; set; } = ElementHelper.CreateDataGrid("Logs");

        DataGridColumn TimestampColumn { get; set; } = ElementHelper.CreateGridTextColumn("Timestamp", binding: "Timestamp", width: 175);

        DataGridColumn MessageColumn { get; set; } = ElementHelper.CreateGridTextColumn("Message", binding: "Message", width: 740);

        Button ClearLogsButton { get; set; } = ElementHelper.CreateButton("Clear", style: ButtonStyleEnum.Delete);


        double nextLeft = 10;

        double nextTop = 10;

        double padding = 15;

        LogHelper _logHelper { get; set; }

        public LogsHomeVM()
        {
            _logHelper = new LogHelper();

            Show();
        }

        private void Show()
        {
            DisplayElement(TitleTextBlock);

            nextLeft = nextLeft + padding * 2;
            nextTop = 100;
            DisplayElement(LogTypeTextBlock, leftPadding: padding * 17);

            nextTop = LogTypeTextBlock.Margin.Top + 3;
            nextLeft = LogTypeTextBlock.Margin.Left + LogTypeTextBlock.Width;
            DisplayElement(LogTypeComboBox);

            nextLeft = TitleTextBlock.Margin.Left + padding * 2;
            DisplayElement(DataGrid);
            DataGrid.Columns.Add(TimestampColumn);
            DataGrid.Columns.Add(MessageColumn);

            LogTypeComboBox.ItemsSource = LogHelper.LogCategories();
            LogTypeComboBox.DropDownClosed += (s, e) => LogTypeComboBox_DropDownClosed();
            LogTypeComboBox.SelectedIndex = 0;

            ShowLogs(LogType.General);

            nextLeft = DataGrid.Margin.Left + DataGrid.Width - ClearLogsButton.Width;
            DisplayElement(ClearLogsButton);
            ClearLogsButton.Click += (s, e) => ClearLogsButton_Clicked();
        }

        public void Dispose()
        {
            WindowController.Instance.LogsHomeView = null;
        }

        private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
        {
            element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

            ViewGrid.Children.Add(element);

            nextTop = element.Margin.Top + element.Height + padding;
        }

        void ShowLogs(LogType type)
        {
            switch (type)
            {
                case LogType.Error:
                    DataGrid.ItemsSource = LogHelper.ErrorLogEntries;
                    break;
                case LogType.General:
                    DataGrid.ItemsSource = LogHelper.GeneralLogEntries;
                    break;
                case LogType.Session:
                    DataGrid.ItemsSource = LogHelper.SessionLogEntries;
                    break;
                case LogType.Server:
                    DataGrid.ItemsSource = LogHelper.ServerLogEntries;
                    break;
            }
        }

        string _currentCategory = LogHelper.LogCategories()[0];
        void LogTypeComboBox_DropDownClosed()
        {
            var categoryChanged = _currentCategory != (string)LogTypeComboBox.SelectedItem;

            if (categoryChanged)
            {
                ShowLogs((LogType)LogTypeComboBox.SelectedIndex);

                _currentCategory = (string)LogTypeComboBox.SelectedItem;
            }
        }

        void ClearLogsButton_Clicked()
        {
            DataGrid.ItemsSource = null;
            DataGrid.Items.Refresh();

            var type = (LogType)LogTypeComboBox.SelectedIndex;
            Task.Run(() => LogHelper.ClearEntries(type));
        }
    }
}
