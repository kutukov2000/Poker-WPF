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
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Text.Json;
using System.Text.Json.Serialization;
using System.IO;
using System.Globalization;

namespace poker
{
    //    Піки(Spades) : ♠
    //Хрести(Hearts) : ♥
    //Бубни(Diamonds) : ♦
    //Трефи(Clubs) : ♣
    public class Card
    {
        public string Suit { get; set; }//Масть
        public string Rank { get; set; }//Значення карти
    }
    public partial class MainWindow : Window
    {
        List<Card> cards;
        HashSet<Card> selectedCards=new HashSet<Card>();
        Random random = new Random();
        public MainWindow()
        {
            InitializeComponent();

            //GenerateDeckOfCard();

            ReadDeckOfCards();

            StartGame();
        }
        public void StartGame()
        {
            playerDock.Children.Clear();
            playerDock2.Children.Clear();
            playerDock3.Children.Clear();
            playerDock4.Children.Clear();
            gamesCardDock.Children.Clear(); 

            CardsToPlayer(playerDock, 2);
            CardsToPlayer(playerDock2, 2);
            CardsToPlayer(playerDock3, 2);
            CardsToPlayer(playerDock4, 2);
            CardsToPlayer(gamesCardDock, 3);
        }
        public void CardsToPlayer(DockPanel panel, int countOfCards)
        {
            for (int i = 0; i < countOfCards; i++)
            {
                Card card = SelectCard();
                AddButton(panel, card);
                selectedCards.Add(card);
            }
        }
        public Card SelectCard()
        {
            Card card = cards[random.Next(0, cards.Count - 1)];
            if (CheckCard(card))
                return card;

            return SelectCard();
        }
        public bool CheckCard(Card card)
        {
            if (selectedCards.Count != 0)
            {
                foreach (var item in selectedCards)
                {
                    if (card == item)
                        return false;
                }
            }
            return true;
        }
        public void AddButton(DockPanel panel, Card card)
        {
            Button button = new Button();
            button.Style = (Style)this.Resources["CardStyle"];

            button.Content = card.Rank;
            button.Tag = card.Suit;

            if (card.Suit == "♥" || card.Suit == "♦")
                button.Foreground = Brushes.Red;
            else
                button.Foreground = Brushes.Black;

            panel.Children.Add(button);
        }
        public void ReadDeckOfCards()
        {
            string json = File.ReadAllText("cards.json");

            cards = JsonSerializer.Deserialize<List<Card>>(json);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if(gamesCardDock.Children.Count<5)
                CardsToPlayer(gamesCardDock, 1);
        }
        public void GenerateDeckOfCard()
        {
            HashSet<Card> cards = new HashSet<Card>();
            for (int i = 2; i < 11; i++)
            {
                cards.Add(new Card
                {
                    Suit = "♠",
                    Rank = i.ToString(),
                });
            }
            cards.Add(new Card
            {
                Suit = "♠",
                Rank = "J",
            });
            cards.Add(new Card
            {
                Suit = "♠",
                Rank = "Q",
            });
            cards.Add(new Card
            {
                Suit = "♠",
                Rank = "K",
            });
            cards.Add(new Card
            {
                Suit = "♠",
                Rank = "A",
            });

            for (int i = 2; i < 11; i++)
            {
                cards.Add(new Card
                {
                    Suit = "♥",
                    Rank = i.ToString(),
                });
            }
            cards.Add(new Card
            {
                Suit = "♥",
                Rank = "J",
            });
            cards.Add(new Card
            {
                Suit = "♥",
                Rank = "Q",
            });
            cards.Add(new Card
            {
                Suit = "♥",
                Rank = "K",
            });
            cards.Add(new Card
            {
                Suit = "♥",
                Rank = "A",
            });

            for (int i = 2; i < 11; i++)
            {
                cards.Add(new Card
                {
                    Suit = "♦",
                    Rank = i.ToString(),
                });
            }
            cards.Add(new Card
            {
                Suit = "♦",
                Rank = "J",
            });
            cards.Add(new Card
            {
                Suit = "♦",
                Rank = "Q",
            });
            cards.Add(new Card
            {
                Suit = "♦",
                Rank = "K",
            });
            cards.Add(new Card
            {
                Suit = "♦",
                Rank = "A",
            });

            for (int i = 2; i < 11; i++)
            {
                cards.Add(new Card
                {
                    Suit = "♣",
                    Rank = i.ToString(),
                });
            }
            cards.Add(new Card
            {
                Suit = "♣",
                Rank = "J",
            });
            cards.Add(new Card
            {
                Suit = "♣",
                Rank = "Q",
            });
            cards.Add(new Card
            {
                Suit = "♣",
                Rank = "K",
            });
            cards.Add(new Card
            {
                Suit = "♣",
                Rank = "A",
            });

            foreach (var card in cards)
            {
                Button button = new Button();
                button.Style = (Style)this.Resources["CardStyle"];
                button.Content = card.Rank;
                button.Tag = card.Suit;
                CardsPanel.Children.Add(button);
            }

            string jsonString = JsonSerializer.Serialize(cards);

            File.WriteAllText("cards.json", jsonString);
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StartGame();
        }
    }
}

