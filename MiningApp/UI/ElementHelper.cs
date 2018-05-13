using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;
using System.Windows;

namespace MiningApp.UI
{
    public class ElementHelper
    {
        public static ElementHelper Instance { get; set; }

        public static NavHelper NavBar => Instance._navBar;


        static Brush DefaultFontColor { get; set; } = Brushes.LightGray;

        static FontFamily DefaultFontFamily { get; set; } = new FontFamily("Verdana");

        static int DefaultFontSize { get; set; } = 16;

        static Style ButtonStyle { get; set; } = (Style)MainWindow.Instance.FindResource("RoundButtonTemplate");

        static Brush ButtonBackgroundColor { get; set; } = ConvertColorCode("#1c1818");

        static int ButtonWidth { get; set; } = 200;

        static int ButtonHeight { get; set; } = 75;


        private NavHelper _navBar { get; set; }


        public ElementHelper()
        {
            Instance = this;

            _navBar = new NavHelper();
        }

        public static Button CreateButton(string content, int fontSize = -1)
        {
            string name = "";
            var words = content.Split(' ').ToList();

            if (words.Count > 1)
            {
                foreach (var word in words) name += word;
            }
            else
            {
                name = content;
            }

            return new Button()
            {
                Name = $"{name}Button",
                Content = content,
                FontFamily = DefaultFontFamily,
                FontSize = fontSize > 0 ? fontSize : DefaultFontSize,
                Foreground = DefaultFontColor,
                Width = ButtonWidth,
                Height = ButtonHeight,
                Style = ButtonStyle,

                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
            };
        }

        public static TextBlock CreateTextBlock(string text, int fontSize = -1)
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
                FontFamily = DefaultFontFamily,
                FontSize = fontSize > 0 ? fontSize : DefaultFontSize,
                Foreground = DefaultFontColor,
                
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
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
    }

    public class NavHelper
    {
        public List<SplitButton> ActiveButtons { get; set; }

        FontFamily _defaultFont = new FontFamily("Verdana");

        int _defaultFontSize = 16;

        int buttonHeight = 70;

        int buttonWidth = 200;

        public NavHelper()
        {
            ActiveButtons = CreateButtons();
        }

        public List<string> NavPageNames = new List<string>()
        {
            "Home",
            "Configurations",
            "Miners",
            "Wallets",
            "Pools",
            "Logs"
        };

        public SplitButton NavButtonTemplate(string content, int height = -1, int width = -1)
        {
            if (height <= 0) height = buttonHeight;
            if (width <= 0) width = buttonWidth;

            var button = new SplitButton
            {
                Name = $"Nav{content}Button",
                Content = content,
                Height = height,
                Width = width,
                FontFamily = _defaultFont,
                FontSize = _defaultFontSize,
                VerticalAlignment = System.Windows.VerticalAlignment.Top,
                HorizontalAlignment = System.Windows.HorizontalAlignment.Left,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Center
            };

            return button;
        }

        private List<SplitButton> CreateButtons()
        {
            var buttons = new List<SplitButton>();

            NavPageNames.ForEach(x => buttons.Add(NavButtonTemplate(x)));

            return buttons;
        }
    }
}
