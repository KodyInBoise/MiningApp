using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MiningApp.LoggingUtil
{
    public class ViewLogsViewModel
    {
        public static ViewLogsViewModel Instance { get; set; }

        public CollectionViewSource GridItems { get; set; }


        private ViewLogsWindow _window { get; set; }

        private List<LogEntry> _entries { get; set; }

        private LogType _viewingType { get; set; }


        public ViewLogsViewModel(ViewLogsWindow window)
        {
            _window = window;

            ShowWindow();
        }

        private void ShowWindow()
        {
            _window.Left = WindowController.Instance?.WindowLeft ?? 50;
            _window.Top = WindowController.Instance?.WindowTop ?? 50;

            _window.GeneralRadioButton.Checked += (s, e) => LogType_Toggled();
            _window.ErrorsRadioButton.Checked += (s, e) => LogType_Toggled();

            DisplayGrid(LogType.General);

            _window.Show();
        }

        public void Dispose()
        {
            Instance = null;

            _window.Close();
        }

        private void DisplayGrid(LogType type)
        {
            switch (type)
            {
                case LogType.General:
                    _entries = LogHelper.Instance.GeneralLogEntries;
                    break;
                case LogType.Error:
                    _entries = LogHelper.Instance.ErrorLogEntries;
                    break;
                default:
                    _entries = new List<LogEntry>();
                    break;
            }
            _viewingType = type;

            GridItems = (CollectionViewSource)(_window.FindResource("GridItems"));
            GridItems.Source = _entries;

            _window.LogsDataGrid.Items.Refresh();
        }

        private LogEntry GetSelectedEntry()
        {
            DataGridCellInfo cellInfo = _window.LogsDataGrid.SelectedCells[0];
            if (cellInfo == null) return null;

            DataGridBoundColumn column = cellInfo.Column as DataGridBoundColumn;
            if (column == null) return null;

            FrameworkElement element = new FrameworkElement() { DataContext = cellInfo.Item };
            BindingOperations.SetBinding(element, FrameworkElement.TagProperty, column.Binding);
            var id = element.Tag.ToString();

            foreach (LogEntry entry in _entries)
            {
                if (entry.ID == Convert.ToInt32(id))
                {
                    return entry;
                }
            }

            return null;
        }

        private void LogType_Toggled()
        {
            if (_window.GeneralRadioButton.IsChecked == true)
            {
                DisplayGrid(LogType.General);
            }
            else if (_window.ErrorsRadioButton.IsChecked == true)
            {
                DisplayGrid(LogType.Error);
            }
        }
    }

    public partial class ViewLogsWindow : Window
    {
        public CollectionViewSource GridItems => _view.GridItems;


        private ViewLogsViewModel _view { get; set; }


        public ViewLogsWindow()
        {
            InitializeComponent();

            ViewLogsViewModel.Instance?.Dispose();

            _view = new ViewLogsViewModel(this);
        }
    }
}
