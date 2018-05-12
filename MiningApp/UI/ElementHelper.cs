using System;
using System.Collections.Generic;
using System.Windows.Media;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xceed.Wpf.Toolkit;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class ElementHelper
    {
        public static ElementHelper Instance { get; set; }

        public static NavHelper NavBar => Instance._navBar;


        static Brush _fontColor { get; set; } = Brushes.LightGray;

        static FontFamily _fontFamily { get; set; } = new FontFamily("Verdana");

        static int _fontSize { get; set; } = 16;

       
        private NavHelper _navBar { get; set; }


        public ElementHelper()
        {
            Instance = this;

            _navBar = new NavHelper();
        }

        public static TextBlock CreateTextBlock(string text, int fontSize = -1)
        {
            if (fontSize <= 0) fontSize = _fontSize;

            return new TextBlock()
            {
                Name = $"{text}TextBlock",
                Text = text,
                FontFamily = _fontFamily,
                FontSize = fontSize,
                Foreground = _fontColor,
                
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
    }

    public class NavHelper
    {
        public List<SplitButton> ActiveButtons { get; set; }

        FontFamily _defaultFont = new FontFamily("Verdana");

        int _defaultFontSize = 16;

        int buttonHeight = 70;

        int buttonWidth = 215;

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
