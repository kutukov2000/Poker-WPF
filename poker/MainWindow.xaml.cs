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
using System.Numerics;

namespace poker
{
    public enum Combinations
    {
        HightCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        Straight = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        StraightFlush = 9,
        RoyalFlush = 10
    }
    public class Player
    {
        public string Name { get; set; }
        public List<Card> Hand { get; set; }
        //public int CombinationStrength { get; set; }
        public Combinations Combination { get; set; }
        public Player()
        {
            Hand = new List<Card>();
        }
    }
    public class PokerGame
    {
        private List<Card> Cards { get; set; }

        public List<Player> Players { get; set; }
        public List<Card> TableCards { get; set; }
        public List<Card> CardsInGame { get; set; }

        public PokerGame()
        {
            TableCards = new List<Card>();
            CardsInGame = new List<Card>();

            ReadDeckOfCards();
        }
        private void ReadDeckOfCards()
        {
            string json = File.ReadAllText("cards.json");

            Cards = JsonSerializer.Deserialize<List<Card>>(json);
        }
        public void StartGame()
        {
            Players = new List<Player>();

            CardsInGame.Clear();
            TableCards.Clear();
            Players.Clear();

            Players = new List<Player> {
                new Player() {
            Name="Player 1"
            } ,
                 new Player() {
            Name="Player 2"
            } ,
                  new Player() {
            Name="Player 3"
            } ,
                   new Player() {
            Name="Player 4"
            } ,
            };
            CardsToPlayers();

            AddCardToTable(3);
        }
        public void CardsToPlayers()
        {
            foreach (var player in Players)
            {
                for (int i = 0; i < 2; i++)
                {
                    Card card = RandomCard();
                    player.Hand.Add(card);
                    CardsInGame.Add(card);
                }
            }
        }

        private void AddCardToTable(int countOfCards)
        {
            for (int i = 0; i < countOfCards; i++)
            {
                Card card = RandomCard();
                TableCards.Add(card);
                CardsInGame.Add(card);
            }
        }
        public void AddCardToTable()
        {
            if (TableCards.Count == 0)
            {
                AddCardToTable(3);
            }
            else if (TableCards.Count < 5)
            {
                AddCardToTable(1);
            }
            //else перевірка на комбінації
        }

        public bool CheckCard(Card card)
        {
            return !CardsInGame.Contains(card);
        }

        public Card RandomCard()
        {
            Random random = new Random();
            Card card = null;

            while (card == null || !CheckCard(card))
            {
                card = Cards[random.Next(0, Cards.Count - 1)];
            }

            return card;
        }

        public void CheckPokerHand()
        {
            // Методи перевірки окремих покерних комбінацій

            bool IsRoyalFlush(List<Card> cards)
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

            bool IsStraightFlush(List<Card> cards)
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

            bool IsFourOfAKind(List<Card> cards)
            {
                var groupedCards = cards.GroupBy(card => card.Rank);

                foreach (var group in groupedCards)
                {
                    if (group.Count() == 4)
                        return true;
                }

                return false;
            }

            bool IsFullHouse(List<Card> cards)
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

            bool IsFlush(List<Card> cards)
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

            bool IsStraight(List<Card> cards)
            {
                var distinctRanks = cards.Select(card => card.Rank).Distinct().ToList();
                distinctRanks.Sort();

                // Перевірка на послідовність рангів
                if (distinctRanks.Count < 5)
                    return false;

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

            bool IsThreeOfAKind(List<Card> cards)
            {
                var groupedCards = cards.GroupBy(card => card.Rank);

                foreach (var group in groupedCards)
                {
                    if (group.Count() == 3)
                        return true;
                }

                return false;
            }

            bool IsTwoPair(List<Card> cards)
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

            bool IsOnePair(List<Card> cards)
            {
                var groupedCards = cards.GroupBy(card => card.Rank);

                foreach (var group in groupedCards)
                {
                    if (group.Count() == 2)
                        return true;
                }

                return false;
            }


            foreach (var player in Players)
            {
                List<Card> allCards = new List<Card>();
                allCards.AddRange(player.Hand);
                allCards.AddRange(TableCards);

                // Перевірка комбінацій у порядку від найсильнішої до найслабшої

                if (IsRoyalFlush(allCards))
                    player.Combination = Combinations.RoyalFlush;

                else if (IsStraightFlush(allCards))
                    player.Combination = Combinations.StraightFlush;

                else if (IsFourOfAKind(allCards))
                    player.Combination = Combinations.FourOfAKind;

                else if (IsFullHouse(allCards))
                    player.Combination = Combinations.FullHouse;

                else if (IsFlush(allCards))
                    player.Combination = Combinations.Flush;

                else if (IsStraight(allCards))
                    player.Combination = Combinations.Straight;

                else if (IsThreeOfAKind(allCards))
                    player.Combination = Combinations.ThreeOfAKind;

                else if (IsTwoPair(allCards))
                    player.Combination = Combinations.TwoPair;

                else if (IsOnePair(allCards))
                    player.Combination = Combinations.OnePair;

                else player.Combination = Combinations.HightCard;
            }
        }
        public void ShowWinner()
        {
            int indexOfWinner = ComparePlayers();
         
            MessageBox.Show($"Name: {Players[indexOfWinner].Name}\n" +
                            $"Combination:  {Players[indexOfWinner].Combination}","Winner");
        }
        public int ComparePlayers()
        {
            // Знайти гравця з найсильнішою комбінацією
            Player winner = Players[0]; // Припустимо, що перший гравець - поточний переможець

            for (int i = 1; i < Players.Count; i++)
            {
                // Порівняти комбінації гравців
                if (Players[i].Combination > winner.Combination)
                {
                    winner = Players[i]; // Оновити переможця
                }
                else if (Players[i].Combination == winner.Combination)
                {
                    // Якщо комбінації рівні, порівняти посилення комбінацій
                    if (GetCombinationStrength(Players[i].Combination) > GetCombinationStrength(winner.Combination))
                    {
                        winner = Players[i]; // Оновити переможця
                    }
                }
            }

            // Повернути індекс переможця
            return Players.IndexOf(winner);
        }

        public int GetCombinationStrength(Combinations combination)
        {
            // Повернути силу комбінації відповідно до перерахування Combinations
            return (int)combination;
        }


        public DockPanel DrawTable()
        {
            DockPanel dockPanel = new DockPanel();

            foreach (var player in Players)
            {
                DockPanel playerPanel = new DockPanel();
                //Add Cards Buttons
                foreach (var card in player.Hand)
                {
                    AddButton(playerPanel, card);
                }

                //Add Combination TextBox
                playerPanel.Children.Add(new TextBlock()
                {
                    Foreground = Brushes.White,
                    Text = player.Combination.ToString()
                });

                dockPanel.Children.Add(playerPanel);
            }

            DockPanel tableCardsPanel = new DockPanel();
            tableCardsPanel.HorizontalAlignment = HorizontalAlignment.Center;
            tableCardsPanel.VerticalAlignment = VerticalAlignment.Center;
            foreach (var card in TableCards)
            {
                AddButton(tableCardsPanel, card);
            }

            dockPanel.Children.Add(tableCardsPanel);

            return dockPanel;
        }

        public void AddButton(DockPanel panel, Card card)
        {
            Button button = new Button();

            button.Content = card.Rank;
            button.Tag = card.Suit;

            if (card.Suit == "♥" || card.Suit == "♦")
                button.Foreground = Brushes.Red;
            else
                button.Foreground = Brushes.Black;

            panel.Children.Add(button);
        }
    }
    public class Card
    {
        public string Suit { get; set; }
        public string Rank { get; set; }
    }
    public partial class MainWindow : Window
    {
        PokerGame poker = new PokerGame();
        public MainWindow()
        {
            InitializeComponent();
        }
        public void StartGame()
        {
            poker.StartGame();

            DrawTable();
        }
        public void DrawTable()
        {
            CardsPanel.Children.Clear();

            DockPanel updatedDockPanel = poker.DrawTable();
            updatedDockPanel.HorizontalAlignment = HorizontalAlignment.Stretch;
            updatedDockPanel.VerticalAlignment = VerticalAlignment.Stretch;

            int i = 1;
            foreach (var playerPanel in updatedDockPanel.Children.OfType<DockPanel>())
            {
                playerPanel.Style = (Style)this.Resources["DockStyle" + i++.ToString()];

                foreach (var button in playerPanel.Children.OfType<Button>())
                {
                    button.Style = (Style)this.Resources["CardStyle"];
                }
            }
            CardsPanel.Children.Add(updatedDockPanel);
        }
        private void AddTableCard_Click(object sender, RoutedEventArgs e)
        {
            poker.AddCardToTable();
            DrawTable();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            StartGame();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            poker.CheckPokerHand();
            DrawTable();
            poker.ShowWinner();
        }
    }
}

