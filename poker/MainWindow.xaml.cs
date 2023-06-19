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
    public class Suits
    {
        public const string Spades = "♠";
        public const string Hearts = "♥";
        public const string Diamonds = "♦";
        public const string Clubs = "♣";
    }
    enum ErrorCode : ushort
    {
        None = 0,
        Unknown = 1,
        ConnectionLost = 100,
        OutlierReading = 200
    }
    public partial class MainWindow : Window
    {
        List<Card> cards;
        HashSet<Card> selectedCards = new HashSet<Card>();
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
            selectedCards.Clear();

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
        public string CheckPokerHand(List<Card> playerCards, List<Card> tableCards)
        {
            List<Card> allCards = new List<Card>(playerCards);
            allCards.AddRange(tableCards);

            // Перевірка комбінацій у порядку від найсильнішої до найслабшої

            if (IsRoyalFlush(allCards))
                return "Royal Flush";

            if (IsStraightFlush(allCards))
                return "Straight Flush";

            if (IsFourOfAKind(allCards))
                return "Four of a Kind";

            if (IsFullHouse(allCards))
                return "Full House";

            if (IsFlush(allCards))
                return "Flush";

            if (IsStraight(allCards))
                return "Straight";

            if (IsThreeOfAKind(allCards))
                return "Three of a Kind";

            if (IsTwoPair(allCards))
                return "Two Pair";

            if (IsOnePair(allCards))
                return "One Pair";

            return "High Card";
        }

        // Методи перевірки окремих покерних комбінацій

        private bool IsRoyalFlush(List<Card> cards)
        {
            var royalRanks = new List<string> { "10", "J", "Q", "K", "A" };
            var suits = cards.Select(card => card.Suit).Distinct();

            foreach (var suit in suits)
            {
                var suitCards = cards.Where(card => card.Suit == suit);
                var royalFlushCards = suitCards.Where(card => royalRanks.Contains(card.Rank));

                if (royalFlushCards.Count() == 5)
                    return true;
            }

            return false;
        }

        private bool IsStraightFlush(List<Card> cards)
        {
            var suits = cards.Select(card => card.Suit).Distinct();

            foreach (var suit in suits)
            {
                var suitCards = cards.Where(card => card.Suit == suit);
                var sortedCards = suitCards.OrderBy(card => card.Rank).ToList();

                bool isStraight = true;
                for (int i = 0; i < sortedCards.Count - 1; i++)
                {
                    int currentRank, nextRank;
                    if (!int.TryParse(sortedCards[i].Rank, out currentRank) ||
                        !int.TryParse(sortedCards[i + 1].Rank, out nextRank) ||
                        currentRank != nextRank - 1)
                    {
                        isStraight = false;
                        break;
                    }
                }

                // Перевірка, чи всі карти мають одну масті та є послідовні
                if (isStraight && sortedCards.Count == 5)
                    return true;
            }

            return false;
        }

        private bool IsFourOfAKind(List<Card> cards)
        {
            var groupedCards = cards.GroupBy(card => card.Rank);

            foreach (var group in groupedCards)
            {
                if (group.Count() == 4)
                    return true;
            }

            return false;
        }

        private bool IsFullHouse(List<Card> cards)
        {
            var groupedCards = cards.GroupBy(card => card.Rank);

            bool hasThreeOfAKind = false;
            bool hasPair = false;

            foreach (var group in groupedCards)
            {
                if (group.Count() == 3)
                {
                    hasThreeOfAKind = true;
                }
                else if (group.Count() == 2)
                {
                    hasPair = true;
                }
            }

            return hasThreeOfAKind && hasPair;
        }

        private bool IsFlush(List<Card> cards)
        {
            var suits = cards.Select(card => card.Suit).Distinct();

            foreach (var suit in suits)
            {
                var suitCards = cards.Where(card => card.Suit == suit);

                if (suitCards.Count() >= 5)
                    return true;
            }

            return false;
        }

        private bool IsStraight(List<Card> cards)
        {
            var distinctRanks = cards.Select(card => card.Rank).Distinct().ToList();
            distinctRanks.Sort();

            // Перевірка на послідовність рангів
            if (distinctRanks.Count < 5)
                return false;

            //for (int i = 0; i < distinctRanks.Count - 4; i++)
            //{
            //    if (int.Parse(distinctRanks[i + 4]) - int.Parse(distinctRanks[i]) == 4)
            //        return true;
            //}
            for (int i = 0; i < distinctRanks.Count - 4; i++)
            {
                int rank1, rank2;
                if (int.TryParse(distinctRanks[i], out rank1) && int.TryParse(distinctRanks[i + 4], out rank2))
                {
                    if (rank2 - rank1 == 4)
                        return true;
                }
            }

            // Перевірка на випадок "A, 2, 3, 4, 5" (стріт з тузом в кінці)
            if (distinctRanks.Contains("A") && distinctRanks.Contains("2") && distinctRanks.Contains("3") && distinctRanks.Contains("4") && distinctRanks.Contains("5"))
                return true;

            return false;
        }

        private bool IsThreeOfAKind(List<Card> cards)
        {
            var groupedCards = cards.GroupBy(card => card.Rank);

            foreach (var group in groupedCards)
            {
                if (group.Count() == 3)
                    return true;
            }

            return false;
        }

        private bool IsTwoPair(List<Card> cards)
        {
            var groupedCards = cards.GroupBy(card => card.Rank);

            int pairCount = 0;

            foreach (var group in groupedCards)
            {
                if (group.Count() == 2)
                {
                    pairCount++;
                }
            }

            return pairCount == 2;
        }

        private bool IsOnePair(List<Card> cards)
        {
            var groupedCards = cards.GroupBy(card => card.Rank);

            foreach (var group in groupedCards)
            {
                if (group.Count() == 2)
                    return true;
            }

            return false;
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
            if (gamesCardDock.Children.Count < 5)
                CardsToPlayer(gamesCardDock, 1);
            else
                playerCombination.Text=CheckPokerHand(ButtonsToCards(playerDock.Children.OfType<Button>().ToList()), ButtonsToCards(gamesCardDock.Children.OfType<Button>().ToList()));
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
        private List<Card> ButtonsToCards(List<Button> buttons)
        {
            List<Card> cards = new List<Card>();
            foreach (Button button in buttons)
            {
                cards.Add(new Card
                {
                    Rank = button.Content.ToString(),
                    Suit = button.Tag.ToString()
                });
            }
            return cards;
        }
    }
}

