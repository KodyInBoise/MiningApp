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
using System.Threading;

namespace MiningApp.UI
{
    /// <summary>
    /// Interaction logic for MessageWindow.xaml
    /// </summary>
    public partial class MessageWindow : Window
    {
        TextBlock MessageTextBlock { get; set; } 

        public enum Result
        {
            Yes,
            No,
            Cancel
        }

        public MessageWindow()
        {
            Background = ElementValues.Windows.BackgroundColor;
            Width = ElementValues.Windows.Message.Width;
            Height = ElementValues.Windows.Message.Height;
        }

        public void ShowMessage()
        {

        }

    }
}
