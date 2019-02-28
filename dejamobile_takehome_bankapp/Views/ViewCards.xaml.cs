using Prism.Events;
using System.Windows.Controls;
using dejamobile_takehome_sdk;
using System;
using System.Collections.Generic;

namespace dejamobile_takehome_bankapp.Views
{
    /// <summary>
    /// Interaction logic for ViewCards
    /// </summary>
    public partial class ViewCards : UserControl
    {
        private IEventAggregator _eventAggregator;
        public IEventAggregator eventAggregator
        {
            get { return _eventAggregator; }
            set { _eventAggregator = value; }
        }

        List<Views.ViewSingleCard> uiCardList { get; set; }

        public ViewCards(IEventAggregator eventAggregator)
        {
            InitializeComponent();
            this.eventAggregator = eventAggregator;
            uiCardList = new List<ViewSingleCard>();
            initSubscriptions();
        }

        private void initSubscriptions()
        {
            eventAggregator.GetEvent<Events.SdkCommandResultEvent>().Subscribe(onSdkCommandResultEvents);
        }

        private void onSdkCommandResultEvents(TaskResult obj)
        {
            switch (obj.name)
            {
                case TaskResult.TaskName.getCards:
                    if (obj.result)
                    {
                        List<dejamobile_takehome_sdk.Models.CardModel> cards = (List<dejamobile_takehome_sdk.Models.CardModel>)obj.payload;
                        updateCards(cards);
                    }
                    else
                    {
                        // do something ?
                    }
                    break;
                default:
                    break;
            }
        }

        private void updateCards(List<dejamobile_takehome_sdk.Models.CardModel> cards)
        {
            uiCardList.Clear();
            listBoxCardsDisplay.Items.Clear();

            foreach (dejamobile_takehome_sdk.Models.CardModel card in cards)
            {
                ViewSingleCard newCard = new Views.ViewSingleCard(card);
                uiCardList.Add(newCard);
                listBoxCardsDisplay.Items.Add(newCard);
            }


        }
    }
}
