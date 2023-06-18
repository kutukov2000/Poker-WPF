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

namespace poker
{
    public class Card
    {
        public string Suit { get; set; }//Масть
        public string Rank { get; set; }//Значення карти

    }
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Button button=new Button();
            button.Style = (Style)this.Resources["CardStyle"];
            button.Content = "K";
            button.Tag = "♥";
            CardsPanel.Children.Add(button);
        }
    }
}
