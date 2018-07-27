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
    public class PoolSetupVM
    {
        public static PoolSetupVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public PoolSetupVM()
        {
            Instance = this;

            Show();
        }

        private void Show()
        {
            _primaryVM = new PrimaryVM();
            _secondaryVM = new SecondaryVM();
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

        public void DisplaySecondary(PoolConfigModel pool = null)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(pool);
        }

        public class PrimaryVM
        {
            PoolSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Pools", 40);

            Button NewButton { get; set; } = ElementHelper.CreateButton("New Pool", height: buttonHeight, style: ButtonStyleEnum.New);


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
                DisplayElement(TitleTextBlock, leftPadding: 45);
                nextTop = 75;

                DisplayElement(NewButton);
                NewButton.Click += (s, e) => NewButton_Clicked();

                DisplayExisting();
            }

            private async void DisplayExisting()
            {
                _pools = await DataHelper.Instance.GetPools();

                nextTop = NewButton.Margin.Top + NewButton.Height + padding * 2;

                foreach (var pool in _pools)
                {
                    var button = ElementHelper.CreateButton(pool.Name);
                    _poolButtons.Add(button);
                    _buttonDictionary.Add(button, pool.ID);

                    DisplayElement(button);

                    button.Click += ExistingPoolClicked;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingPoolClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var pool = _pools.Find(x => x.ID == _buttonDictionary[button]);

                Instance.DisplaySecondary(pool);
            }

            public void ShowNewPool(PoolConfigModel pool)
            {
                var button = ElementHelper.CreateButton(pool.Name);
                _poolButtons.Add(button);
                _buttonDictionary.Add(button, pool.ID);

                DisplayElement(button);

                button.Click += ExistingPoolClicked;

                _pools.Add(pool);
            }

            private void NewButton_Clicked()
            {
                Instance.DisplaySecondary();
            }
        }

        public class SecondaryVM
        {
            PoolSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("New Mining Pool", fontSize: 40, width: 400);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);


            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            TextBox AddressTextBox { get; set; } = ElementHelper.CreateTextBox("Address");

            TextBox NewTagTextBox { get; set; } = ElementHelper.CreateTextBox("NewTag", width: 350);


            ListBox TagsListBox { get; set; } = ElementHelper.CreateListBox("Pools", fontSize: 14);


            Button TagsAddButton { get; set; } = ElementHelper.CreateButton("+", name: "PoolsAdd", fontSize: 14, style: ButtonStyleEnum.New,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button TagsRemoveButton { get; set; } = ElementHelper.CreateButton("-", name: "PoolsAdd", fontSize: 14, style: ButtonStyleEnum.Delete,
                width: 40, height: ElementValues.TextBoxs.Height);



            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyleEnum.Delete);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyleEnum.Finish);



            Label NameLabel { get; set; }

            Label AddressLabel { get; set; }

            Label TagsLabel { get; set; }


            private static int buttonWidth = 150;

            private static int buttonHeight = 60;

            private double nextLeft = 15;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;

            private PoolConfigModel _pool { get; set; }


            public SecondaryVM(PoolConfigModel pool = null)
            {
                _pool = pool;

                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock);

                nextTop = 75;
                DisplayElement(StatusTextBlock, leftPadding: 10);
                StatusTextBlock.Visibility = Visibility.Collapsed;

                nextLeft = 200;
                nextTop = 250;
                DisplayElement(NameTextBox);

                DisplayElement(AddressTextBox);

                DisplayElement(NewTagTextBox, topPadding: padding);

                DisplayElement(TagsListBox);


                nextLeft = NewTagTextBox.Margin.Left + NewTagTextBox.Width + padding;
                nextTop = NewTagTextBox.Margin.Top;
                DisplayElement(TagsAddButton);

                nextLeft = TagsAddButton.Margin.Left + TagsAddButton.Width + 5;
                nextTop = NewTagTextBox.Margin.Top;
                DisplayElement(TagsRemoveButton);


                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = AddressTextBox.Margin.Top;
                AddressLabel = ElementHelper.CreateLabel("Address", AddressTextBox);
                AddressLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(AddressLabel, ignoreMargin: true);

                nextTop = NewTagTextBox.Margin.Top;
                TagsLabel = ElementHelper.CreateLabel("Tags", NewTagTextBox);
                TagsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(TagsLabel, ignoreMargin: true);


                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                TagsAddButton.Click += (s, e) => TagsAddButton_Clicked();
                TagsRemoveButton.Click += (s, e) => TagsRemoveButton_Clicked();
                DeleteButton.Click += (s, e) => DeleteButton_Clicked();
                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_pool == null)
                {
                    _pool = new PoolConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Mining Pool";

                    NameTextBox.Text = _pool.Name;
                    AddressTextBox.Text = _pool.Address;
                    TagsListBox.ItemsSource = _pool.Tags;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0, bool ignoreMargin = false)
            {
                if (!ignoreMargin)
                {
                    element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);
                }

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void RadioButton_Toggled()
            {

            }

            void TagsAddButton_Clicked()
            {
                string value = NewTagTextBox.Text;

                if (!String.IsNullOrEmpty(value) && !TagsListBox.Items.Contains(value))
                {
                    TagsListBox.Items.Add(value);
                    NewTagTextBox.Text = "";
                }
            }

            void TagsRemoveButton_Clicked()
            {
                try
                {
                    string value = (string)TagsListBox.SelectedItem;

                    TagsListBox.Items.Remove(TagsListBox.SelectedItem);
                    NewTagTextBox.Text = value;
                }
                catch
                {

                }
            }

            private void DeleteButton_Clicked()
            {
                Delete();
            }

            private void FinishButton_Clicked()
            {
                SetPoolInfo();

                Save();
            }

            private void Delete()
            {
                DataHelper.Instance.DeletePoolConfig(_pool);

                Task.Run(() => LogHelper.AddEntry(LogType.General, $"Deleted Pool Config: \"{_pool.Name}\""));

                View.DisplayPrimary();
                View.DisplaySecondary();
            }

            private void Save()
            {
                DataHelper.Instance.SavePoolConfig(_pool);

                Task.Run(() => LogHelper.AddEntry(LogType.General, $"Saved Pool Config: \"{_pool.Name}\""));

                StatusTextBlock.Text = "Mining pool config saved successfully!";
                TitleTextBlock.Text = "Edit Mining Pool";

                View.DisplayPrimary();
            }

            public void SetPoolInfo()
            {
                _pool.CreatedTimestamp = _pool.ID > 0 ? _pool.CreatedTimestamp : DateTime.Now;
                _pool.Name = NameTextBox.Text;
                _pool.Address = AddressTextBox.Text;
                _pool.Tags = TagsListBox.Items.Cast<string>().ToList();
            }
        }
    }
}
