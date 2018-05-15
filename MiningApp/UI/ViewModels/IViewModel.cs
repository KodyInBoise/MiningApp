using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MiningApp.UI
{
    interface IViewModel
    {
        PrimaryViewModel PrimaryView { get; set; }
        SecondaryViewModel SecondaryView { get; set; }

        List<FrameworkElement> ActiveElements { get; set; }

        Label TitleLabel { get; set; }

        Label NameLabel { get; set; }
        TextBox NameTextBox { get; set; }

        Label PathLabel { get; set; }
        TextBox PathTextBox { get; set; }

        RadioButton UserRadioButton { get; set; }
        RadioButton AllRadioButton { get; set; }

        Label CryptoLabel { get; set; }
        ComboBox CryptoComboBox { get; set; }

        Label AddressLabel { get; set; }
        TextBox AddressTextBox { get; set; }

        Label VerifyLabel { get; set; }
        TextBox VerifyTextBox { get; set; }

        Button NewButton { get; set; }
        Button FinishButton { get; set; }
        Button DeleteButton { get; set; }

        int NextLeft { get; set; }
        int NextTop { get; set; }
        int Padding { get; set; }

        void Show();
        void Dispose();
    }
}
