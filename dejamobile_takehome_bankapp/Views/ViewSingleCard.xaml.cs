using System;
using System.Windows.Controls;

namespace dejamobile_takehome_bankapp.Views
{
    /// <summary>
    /// Interaction logic for ViewSingleCard
    /// </summary>
    public partial class ViewSingleCard : UserControl
    {
        public ViewSingleCard(dejamobile_takehome_sdk.Models.CardModel card)
        {
            InitializeComponent();
            ownerName.Content = card.ownerName;
            cardNumber.Content = reformatCardNumber(card.cardNumber);
            expDate.Content = card.expirationDate;
        }

        private string reformatCardNumber(string cardNumber)
        {
            string first4Digits = cardNumber.Substring(0, 4);
            string last4Digits = cardNumber.Substring(cardNumber.Length - 4, 4);

            string reformattedNumber = first4Digits + " **** **** " + last4Digits;
            return reformattedNumber;
                
        }
    }
}