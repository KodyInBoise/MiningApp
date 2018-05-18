using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;
using System.Windows;
using System.IO;

namespace MiningApp
{
    public class ElementHelper
    {
        public static ElementHelper Instance { get; set; }


        public ElementHelper()
        {
            Instance = this;
        }

        public static Button CreateButton(string content, string name = "", ButtonStyle style = ButtonStyle.Normal, int fontSize = -1,
            int height = -1, int width = -1)
        {
            if (String.IsNullOrEmpty(name))
            {
                var words = content.Split(' ').ToList();

                if (words.Count > 1)
                {
                    foreach (var word in words) name += word;
                }
                else
                {
                    name = content;
                }
            }

            var backgroundColor = ElementValues.Buttons.Colors.Normal;
            switch(style)
            {
                case ButtonStyle.New:
                    backgroundColor = ElementValues.Buttons.Colors.New;
                    break;
                case ButtonStyle.Delete:
                    backgroundColor = ElementValues.Buttons.Colors.Delete;
                    break;
                case ButtonStyle.Finish:
                    backgroundColor = ElementValues.Buttons.Colors.New;
                    break;
                default:
                    break;
            }

            return new Button()
            {
                Name = $"{name}Button",
                Content = content,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Buttons.FontSize,
                Background = backgroundColor,
                Foreground = ElementValues.Fonts.Color,
                Width = width > 0 ? width : ElementValues.Buttons.Width,
                Height = height > 0 ? height : ElementValues.Buttons.Height,              
                Style = ElementValues.Buttons.Style,
                
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Center,
            };
        }

        public static TextBlock CreateTextBlock(string text, int fontSize = -1, int width = -1, int height = -1)
        {
            string name = "";
            var words = text.Split(' ').ToList();

            if (words.Count > 1)
            {
                foreach (var word in words) name += word;
            }
            else
            {
                name = text;
            }

            return new TextBlock()
            {
                Name = $"{name}TextBlock",
                Text = text,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Fonts.Size,
                Foreground = ElementValues.Fonts.Color,
                MaxWidth = width > 0 ? width : 150,
                MaxHeight = height > 0 ? height : 50,
                
                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
            };
        }

        public static TextBox CreateTextBox(string name, string text = "", int fontSize = -1, int height = -1, int width = -1)
        {
            return new TextBox()
            {
                Name = $"{name}TextBlock",
                Text = text,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Fonts.Size,
                Height = height > 0 ? height : ElementValues.TextBoxs.Height,
                Width = width > 0 ? width : ElementValues.TextBoxs.Width,

                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalContentAlignment = HorizontalAlignment.Left
            };
        }

        public static Label CreateLabel(string content, FrameworkElement element, double topPadding = 0, int fontSize = -1)
        {
            return new Label()
            {
                Name = $"{element.Name}Label",
                Content = content,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Labels.FontSize,
                Foreground = ElementValues.Fonts.Color,

                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
        }

        public static ComboBox CreateComboBox(string name, string text = "", int width = 0, int fontSize = -1, bool isEditable = false)
        {
            return new ComboBox()
            {
                Name = $"{name}ComboBox",
                Text = text,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.ComboBoxs.FontSize,
                Height = ElementValues.TextBoxs.Height,
                Width = width > 0 ? width : ElementValues.TextBoxs.Width,
                IsEditable = isEditable,

                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center,               
            };
        }

        public static RadioButton CreateRadioButton(string name, string content = "", double topPadding = 0, int fontSize = -1)
        {
            return new RadioButton()
            {
                Name = $"{name}RadioButton",
                Content = content,
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Fonts.Size,
                Foreground = ElementValues.Fonts.Color,
                Height = ElementValues.RadioButtons.Height,

                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }

        public static ListBox CreateListBox(string name, int fontSize = -1, int width = -1, int height = -1)
        {
            return new ListBox()
            {
                Name = $"{name}ListBox",
                FontFamily = ElementValues.Fonts.Family,
                FontSize = fontSize > 0 ? fontSize : ElementValues.Fonts.Size,
                Width = width > 0 ? width : ElementValues.ListBoxs.Width,
                Height = height > 0 ? height : ElementValues.ListBoxs.Height,

                VerticalAlignment = VerticalAlignment.Top,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalContentAlignment = VerticalAlignment.Center
            };
        }

        public static string TrimPath(string path, int length = -1)
        {
            try
            {
                var count = path?.Length;
                var pathLength = length > 0 ? length : 30;

                if (count > pathLength)
                {
                    var trimmedPath = string.Empty;
                    var dirs = path.Split('\\').ToList();

                    for (var x = 0; count > pathLength; dirs.RemoveAt(x))
                    {
                        trimmedPath = string.Empty;
                        dirs.ForEach(d => trimmedPath += $"\\{d}");

                        count = trimmedPath.Length;
                    }

                    return $"...{trimmedPath}";
                }
                else
                {
                    return path;
                }
            }
            catch
            {
                return path;
            }
        }

        public static Brush ConvertColorCode(string code)
        {
            try
            {
                var converter = new BrushConverter();
                var brush = (Brush)converter.ConvertFromString(code);

                return brush;
            }
            catch
            {
                return Brushes.White;
            }
        }

        public static string GetFilePath(string dir = "")
        {
            try
            {
                var dialog = new System.Windows.Forms.OpenFileDialog();
                dialog.InitialDirectory = !String.IsNullOrEmpty(dir) ? dir : Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

                var result = dialog.ShowDialog();

                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.FileName;
                }
                else
                {
                    return "";
                }
            }
            catch
            {
                return "";
            }
        }
    }

    public enum DisplayGrid
    {
        Primary,
        Secondary,
    }

    public enum GridSize
    {
        Small,
        Medium,
        Large
    }

    public enum ViewModelType
    {
        Empty,
        Nav,
        Home,
        ConfigsHome,
        MinersHome,
        WalletsHome,
        PoolsHome,
        LogsHome,
        SettingsHome,
        WalletSetup,
        MinerSetup,
        PoolSetup,
        ConfigSetup,
    }

    public class ElementValues
    {
        public static class Windows
        {
            public static Brush BackgroundColor = ElementHelper.ConvertColorCode("#1c1818");

            public class Main
            {
                public static int Width { get; set; } = 1250;
                public static int Height { get; set; } = 850;
            }

            public class Message
            {
                public static int Width { get; set; } = 600;
                public static int Height { get; set; } = 300;
            }
        }

        public static class Grids
        {
            public static int NavWidth { get; set; } = 225;
            public static int PrimaryNormal { get; set; } = 1025;
            public static int PrimarySmall => PrimaryNormal - SecondaryNormal;
            public static int SecondaryNormal { get; set; } = 775;
        }

        public static class Fonts
        {
            public static Brush Color { get; set; } = Brushes.LightGray;
            public static FontFamily Family { get; set; } = new FontFamily("Verdana");
            public static int Size { get; set; } = 16;
        }

        public static class Buttons
        {
            public static Style Style = (Style)MainWindow.Instance.FindResource("RoundButtonTemplate");

            public static int Width { get; set; } = 200;
            public static int Height { get; set; } = 75;
            public static int FontSize { get; set; } = 20;
            
            public static class Colors
            {
                public static Brush Normal = ElementHelper.ConvertColorCode("#444447");
                public static Brush New = ElementHelper.ConvertColorCode("#2b6d17");
                public static Brush Delete = ElementHelper.ConvertColorCode("#b20303");
            }
        }

        public class TextBlocks
        {
        }

        public class TextBoxs
        {
            public static int Width { get; set; } = 450;
            public static int Height { get; set; } = 25;
            public static int FontSize { get; set; } = 18;
        }

        public static class Labels
        {
            public static int FontSize { get; set; } = 20;
        }

        public static class ComboBoxs
        {
            public static int FontSize { get; set; } = 16;
        }

        public static class RadioButtons
        {
            public static int Height { get; set; } = 20;
        }

        public static class ListBoxs
        {
            public static int Width { get; set; } = 450;
            public static int Height { get; set; } = 100;
        }
    }
}
