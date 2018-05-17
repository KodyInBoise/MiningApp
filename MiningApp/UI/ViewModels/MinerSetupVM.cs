using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    public class MinerSetupVM
    {
        public static MinerSetupVM Instance { get; set; }


        Grid PrimaryGrid { get; set; } = MainWindow.Instance.PrimaryGrid;

        Grid SecondaryGrid { get; set; } = MainWindow.Instance.SecondaryGrid;


        private PrimaryVM _primaryVM { get; set; }

        private SecondaryVM _secondaryVM { get; set; }


        public MinerSetupVM()
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

            WindowController.Instance.MinerSetupView = null;
        }

        public void DisplayPrimary()
        {
            PrimaryGrid.Children.Clear();

            _primaryVM = new PrimaryVM();
        }

        public void DisplaySecondary(MinerConfigModel miner = null)
        {
            SecondaryGrid.Children.Clear();

            _secondaryVM = new SecondaryVM(miner);
        }

        public class PrimaryVM
        {
            MinerSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.PrimaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("Miners", 40);

            Button NewButton { get; set; } = ElementHelper.CreateButton("New Miner", height: buttonHeight, style: ButtonStyle.New);


            private static int buttonHeight = 60;

            private double nextLeft = 20;

            private double nextTop = 12;

            private double padding = 15;


            private List<MinerConfigModel> _miners { get; set; } = new List<MinerConfigModel>();

            private List<Button> _minerButtons { get; set; } = new List<Button>();

            private Dictionary<Button, int> _buttonDictionary { get; set; } = new Dictionary<Button, int>();


            public PrimaryVM()
            {
                Show();
            }

            private void Show()
            {
                DisplayElement(TitleTextBlock, leftPadding: 25);
                nextTop = 75;

                DisplayElement(NewButton);
                NewButton.Click += (s, e) => NewButton_Clicked();

                DisplayExisting();
            }

            private async void DisplayExisting()
            {
                _miners = await DataHelper.Instance.GetMiners();

                nextTop = NewButton.Margin.Top + NewButton.Height + padding * 2;

                foreach (var miner in _miners)
                {
                    var button = ElementHelper.CreateButton(miner.Name);
                    _minerButtons.Add(button);
                    _buttonDictionary.Add(button, miner.ID);

                    DisplayElement(button);

                    button.Click += ExistingMinerClicked;
                }
            }

            private void DisplayElement(FrameworkElement element, double leftPadding = 0, double topPadding = 0)
            {
                element.Margin = new Thickness((nextLeft + leftPadding), nextTop + topPadding, 0, 0);

                ViewGrid.Children.Add(element);
                ActiveElements.Add(element);

                nextTop = element.Margin.Top + element.Height + padding;
            }

            private void ExistingMinerClicked(object sender, EventArgs e)
            {
                var button = (Button)sender;

                var miner = _miners.Find(x => x.ID == _buttonDictionary[button]);

                Instance.DisplaySecondary(miner);
            }

            public void ShowNewMiner(MinerConfigModel miner)
            {
                var button = ElementHelper.CreateButton(miner.Name);
                _minerButtons.Add(button);
                _buttonDictionary.Add(button, miner.ID);

                DisplayElement(button);

                button.Click += ExistingMinerClicked;

                _miners.Add(miner);
            }

            private void NewButton_Clicked()
            {
                Instance.DisplaySecondary();
            }
        }

        public class SecondaryVM
        {
            MinerSetupVM View { get; set; } = Instance;

            Grid ViewGrid { get; set; } = Instance.SecondaryGrid;

            List<FrameworkElement> ActiveElements { get; set; } = new List<FrameworkElement>();


            TextBlock TitleTextBlock { get; set; } = ElementHelper.CreateTextBlock("New Miner", fontSize: 40, width: 400);

            TextBlock StatusTextBlock { get; set; } = ElementHelper.CreateTextBlock("Status", fontSize: 22, width: 725, height: 400);

            TextBox NameTextBox { get; set; } = ElementHelper.CreateTextBox("Name");

            TextBox PathTextBox { get; set; } = ElementHelper.CreateTextBox("Path", width: 350);


            ComboBox PoolsComboBox { get; set; } = ElementHelper.CreateComboBox("Pools", width: 350, isEditable: true);

            ComboBox CryptosComboBox { get; set; } = ElementHelper.CreateComboBox("Cryptos", width: 350);


            ListBox PoolsListBox { get; set; } = ElementHelper.CreateListBox("Pools", fontSize: 14);

            ListBox CryptosListBox { get; set; } = ElementHelper.CreateListBox("Cryptos", fontSize: 14);


            Button PoolsAddButton { get; set; } = ElementHelper.CreateButton("+", name: "PoolsAdd", fontSize: 14, style: ButtonStyle.New, 
                width: 40, height: ElementValues.TextBoxs.Height);

            Button PoolsRemoveButton { get; set; } = ElementHelper.CreateButton("-", name: "PoolsAdd", fontSize: 14, style: ButtonStyle.Delete,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button CryptosAddButton { get; set; } = ElementHelper.CreateButton("+", name: "CryptosAdd", fontSize: 14, style: ButtonStyle.New,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button CryptosRemoveButton { get; set; } = ElementHelper.CreateButton("-", name: "CryptosRemove", fontSize: 14, style: ButtonStyle.Delete,
                width: 40, height: ElementValues.TextBoxs.Height);

            Button BrowseButton { get; set; } = ElementHelper.CreateButton("Browse", width: 85, fontSize: 14, height: ElementValues.TextBoxs.Height);

            Button DeleteButton { get; set; } = ElementHelper.CreateButton("Delete", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Delete);

            Button FinishButton { get; set; } = ElementHelper.CreateButton("Finish", height: buttonHeight,
                width: buttonWidth, style: ButtonStyle.Finish);



            Label NameLabel { get; set; }

            Label PathLabel { get; set; }

            Label PoolsLabel { get; set; }

            Label CryptosLabel { get; set; }


            private static int buttonWidth = 150;

            private static int buttonHeight = 60;

            private double nextLeft = 15;

            private double nextTop = 12;

            private double padding = 15;

            private double labelRight = 600;

            private double labelOffset = -5;

            private List<string> ViewingCryptos { get; set; } = CryptoHelper.Instance.GetCryptoNames();

            private List<PoolConfigModel> ViewingPools { get; set; } = DataHelper.Instance.GetPools().Result;


            private MinerConfigModel _miner { get; set; }


            public SecondaryVM(MinerConfigModel miner = null)
            {
                _miner = miner;

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

                DisplayElement(PathTextBox);

                DisplayElement(PoolsComboBox, topPadding: padding);
                PoolsComboBox.ItemsSource = ViewingPools;
            
                DisplayElement(PoolsListBox);

                DisplayElement(CryptosComboBox, topPadding: padding);
                CryptosComboBox.ItemsSource = ViewingCryptos;

                DisplayElement(CryptosListBox);


                nextLeft = PathTextBox.Margin.Left + PathTextBox.Width + padding;
                nextTop = PathTextBox.Margin.Top;
                DisplayElement(BrowseButton);

                nextLeft = PoolsComboBox.Margin.Left + PoolsComboBox.Width + padding;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsAddButton);

                nextLeft = PoolsAddButton.Margin.Left + PoolsAddButton.Width + 5;
                nextTop = PoolsComboBox.Margin.Top;
                DisplayElement(PoolsRemoveButton);

                nextLeft = CryptosComboBox.Margin.Left + CryptosComboBox.Width + padding;
                nextTop = CryptosComboBox.Margin.Top;
                DisplayElement(CryptosAddButton);

                nextLeft = CryptosAddButton.Margin.Left + CryptosAddButton.Width + 5;
                nextTop = CryptosComboBox.Margin.Top;
                DisplayElement(CryptosRemoveButton);

                nextTop = NameTextBox.Margin.Top;
                NameLabel = ElementHelper.CreateLabel("Name", NameTextBox);
                NameLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(NameLabel, ignoreMargin: true);

                nextTop = PathTextBox.Margin.Top;
                PathLabel = ElementHelper.CreateLabel("Path", PathTextBox);
                PathLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PathLabel, ignoreMargin: true);

                nextTop = PoolsComboBox.Margin.Top;
                PoolsLabel = ElementHelper.CreateLabel("Pools", PoolsComboBox);
                PoolsLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(PoolsLabel, ignoreMargin: true);

                nextTop = CryptosComboBox.Margin.Top;
                CryptosLabel = ElementHelper.CreateLabel("Cryptos", CryptosComboBox);
                CryptosLabel.Margin = new Thickness(0, nextTop + labelOffset, labelRight, 0);
                DisplayElement(CryptosLabel, ignoreMargin: true);

                nextLeft = 15;
                nextTop = ViewGrid.Height - DeleteButton.Height - padding;
                DisplayElement(DeleteButton);

                nextLeft = ElementValues.Grids.SecondaryNormal - FinishButton.Width - padding;
                nextTop = DeleteButton.Margin.Top;
                DisplayElement(FinishButton);

                PoolsAddButton.Click += (s, e) => PoolsAddButton_Clicked();
                PoolsRemoveButton.Click += (s, e) => PoolsRemoveButton_Clicked();
                CryptosAddButton.Click += (s, e) => CryptosAddButton_Clicked();
                CryptosRemoveButton.Click += (s, e) => CryptosRemoveButton_Clicked();
                DeleteButton.Click += (s, e) => DeleteButton_Clicked();
                FinishButton.Click += (s, e) => FinishButton_Clicked();

                if (_miner == null)
                {
                    _miner = new MinerConfigModel();
                }
                else
                {
                    TitleTextBlock.Text = "Edit Miner";

                    NameTextBox.Text = _miner.Name;
                    PathTextBox.Text = _miner.Path;

                    PoolsListBox.ItemsSource = _miner.Pools;
                    CryptosListBox.ItemsSource = _miner.Cryptos;
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
                /*
                if (UserCryptosRadioButton.IsChecked == true)
                {
                    ViewingCryptos = WindowController.User.WatchingCryptos;
                }
                else
                {
                    ViewingCryptos = CryptoHelper.Instance.GetCryptoNames();
                }

                CryptoComboBox.ItemsSource = ViewingCryptos;
                */
            }

            void PoolsAddButton_Clicked()
            {
                var pool = (PoolConfigModel)PoolsComboBox.SelectedItem ?? new PoolConfigModel(PoolsComboBox.Text);

                if (!String.IsNullOrEmpty(PoolsComboBox.Text) && !PoolsListBox.Items.Contains(pool))
                {
                    if (!ViewingPools.Contains(pool))
                    {
                        ViewingPools.Add(pool);
                        PoolsComboBox.Items.Refresh();
                    }
                    PoolsListBox.Items.Add(pool);
                    PoolsComboBox.Text = "";
                }
            }

            void PoolsRemoveButton_Clicked()
            {
                try
                {
                    var pool = (PoolConfigModel)PoolsListBox.SelectedItem;

                    PoolsListBox.Items.Remove(PoolsListBox.SelectedItem);
                    PoolsComboBox.SelectedItem = pool;
                }
                catch
                {

                }
            }

            void CryptosAddButton_Clicked()
            {
                string value = CryptosComboBox.Text;

                if (!String.IsNullOrEmpty(value) && !CryptosListBox.Items.Contains(value))
                {
                    CryptosListBox.Items.Add(value);
                    CryptosComboBox.Text = "";
                }
            }

            void CryptosRemoveButton_Clicked()
            {
                try
                {
                    string value = (string)CryptosListBox.SelectedItem;

                    CryptosListBox.Items.Remove(CryptosListBox.SelectedItem);
                    CryptosComboBox.Text = value;
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
                SetMinerInfo();

                Save();
            }

            private void Delete()
            {
                DataHelper.Instance.DeleteMinerConfig(_miner);

                View.DisplayPrimary();
                View.DisplaySecondary();
            }

            private void Save()
            {
                DataHelper.Instance.SaveMiner(_miner);

                StatusTextBlock.Text = "Miner config saved successfully!";
                TitleTextBlock.Text = "Edit Miner";

                View.DisplayPrimary();
            }

            public void SetMinerInfo()
            {               
                _miner.CreatedTimestamp = _miner.ID > 0 ? _miner.CreatedTimestamp : DateTime.Now;
                _miner.Name = NameTextBox.Text;
                _miner.Path = PathTextBox.Text;
                _miner.Pools = PoolsListBox.Items.Cast<PoolConfigModel>().ToList();
                _miner.Cryptos = CryptosListBox.Items.Cast<string>().ToList();
                _miner.Status = MinerStatus.Inactive;                
            }

            private void DisplayCryptos()
            {

            }
        }
    }
}
