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
        public MainWindow()
        {
            InitializeComponent();

            string json = File.ReadAllText("cards.json");

            // Розбір JSON-рядка і перетворення на об'єкт типу Card
            cards = JsonSerializer.Deserialize<List<Card>>(json);

            foreach (var card in cards)
            {
                Button button = new Button();
                button.Style = (Style)this.Resources["CardStyle"];
                button.Content = card.Rank;
                button.Tag = card.Suit;
                CardsPanel.Children.Add(button);
            }
        }

        //    public void GenerateDeckOfCard()
        //    {
        //        HashSet<Card> cards = new HashSet<Card>();
        //        for (int i = 1; i < 11; i++)
        //        {
        //            cards.Add(new Card
        //            {
        //                Suit = "♠",
        //                Rank = i.ToString()
        //            }) ;
        //        }
        //        for (int i = 1; i < 11; i++)
        //        {
        //            cards.Add(new Card
        //            {
        //                Suit = "♥",
        //                Rank = i.ToString()
        //            });
        //        }
        //        for (int i = 1; i < 11; i++)
        //        {
        //            cards.Add(new Card
        //            {
        //                Suit = "♦",
        //                Rank = i.ToString()
        //            });
        //        }
        //        for (int i = 1; i < 11; i++)
        //        {
        //            cards.Add(new Card
        //            {
        //                Suit = "♣",
        //                Rank = i.ToString()
        //            });
        //        }
        //        cards.Add(new Card
        //        {
        //            Suit = "♣",
        //            Rank = "J"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♣",
        //            Rank = "Q"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♣",
        //            Rank = "K"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♣",
        //            Rank = "A"
        //        });

        //        cards.Add(new Card
        //        {
        //            Suit = "♠",
        //            Rank = "J"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♠",
        //            Rank = "Q"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♠",
        //            Rank = "K"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♠",
        //            Rank = "A"
        //        });

        //        cards.Add(new Card
        //        {
        //            Suit = "♥",
        //            Rank = "J"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♥",
        //            Rank = "Q"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♥",
        //            Rank = "K"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♥",
        //            Rank = "A"
        //        });

        //        cards.Add(new Card
        //        {
        //            Suit = "♦",
        //            Rank = "J"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♦",
        //            Rank = "Q"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♦",
        //            Rank = "K"
        //        });
        //        cards.Add(new Card
        //        {
        //            Suit = "♦",
        //            Rank = "A"
        //        });
        //        foreach (var card in cards)
        //        {
        //            Button button = new Button();
        //            button.Style = (Style)this.Resources["CardStyle"];
        //            button.Content = card.Rank;
        //            button.Tag = card.Suit;
        //            CardsPanel.Children.Add(button);
        //        }

        //        string jsonString=JsonSerializer.Serialize(cards);

        //        File.WriteAllText("cards.json", jsonString);
        //    //}
        //}
    }
}
