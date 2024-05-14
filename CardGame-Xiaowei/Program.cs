using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        int predictedWins = GetPlayerPrediction();

        Deck deck = new Deck();
        var playerCards = deck.DealCards(5); // Deal 5 cards to the player
        var computerCards = deck.DealCards(5); // Deal 5 cards to the computer

        int playerWins = 0;

        Console.WriteLine("Game starting...\n");

        while (playerCards.Count > 0 && computerCards.Count > 0)
        {
            Console.WriteLine("Player's turn:");
            var playerCard = PlayTurn(ref playerCards, "Player");
            Console.WriteLine("\nComputer's turn:");
            var computerCard = PlayTurn(ref computerCards, "Computer", isComputer: true);

            // Compare cards
            int comparisonResult = CompareCards(playerCard, computerCard);
            if (comparisonResult > 0)
            {
                Console.WriteLine("\nPlayer wins this round!\n");
                playerWins++;
            }
            else if (comparisonResult < 0)
            {
                Console.WriteLine("\nComputer wins this round!\n");
            }
            else
            {
                Console.WriteLine("\nIt's a tie this round!\n");
            }
        }

        // Compare prediction with actual wins
        Console.WriteLine($"\nYou predicted {predictedWins} wins. You won {playerWins} times.");
        if (predictedWins == playerWins)
        {
            Console.WriteLine("Your prediction was correct! You win the Game");
        }
        else
        {
            Console.WriteLine("Your prediction was incorrect, You lose the Game");
        }
    }
    static int GetPlayerPrediction()
    {
        Console.WriteLine("Predict how many rounds you will win (0-5):");
        int prediction;
        while (!int.TryParse(Console.ReadLine(), out prediction) || prediction < 0 || prediction > 5)
        {
            Console.WriteLine("Invalid input. Please enter a number between 0 and 5:");
        }
        return prediction;
    }
    static Card PlayTurn(ref List<Card> cards, string playerName, bool isComputer = false)
    {
        Card selectedCard;
        if (isComputer)
        {
            Random rand = new Random();
            selectedCard = cards[rand.Next(cards.Count)];
            Console.WriteLine($"Computer plays: {selectedCard.Rank} of {selectedCard.Suit}");
        }
        else
        {
            PrintCardsHorizontally(cards);
            Console.WriteLine("Choose a card to play (enter number): ");
            int choice = Convert.ToInt32(Console.ReadLine()) - 1;
            if (choice >= 0 && choice < cards.Count)
            {
                selectedCard = cards[choice];
                Console.WriteLine($"You played: {selectedCard.Rank} of {selectedCard.Suit}");
            }
            else
            {
                throw new Exception("Invalid choice. Game Over.");
            }
        }

        cards.Remove(selectedCard);
        return selectedCard;
    }

    static void PrintCardsHorizontally(List<Card> cards)
    {
        string[] lines = new string[7];

        foreach (var card in cards)
        {
            string[] cardLines = card.GetAsciiArt().Split('\n');
            for (int i = 0; i < 7; i++)
            {
                lines[i] += cardLines[i] + "  ";
            }
        }

        foreach (string line in lines)
        {
            Console.WriteLine(line);
        }
    }

    static int CompareCards(Card playerCard, Card computerCard)
    {
        // Compare based on rank first
        var rankOrder = new List<string> { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };
        int playerRankIndex = rankOrder.IndexOf(playerCard.Rank);
        int computerRankIndex = rankOrder.IndexOf(computerCard.Rank);

        return playerRankIndex.CompareTo(computerRankIndex);
    }
}

public class Card
{
    public string Suit { get; set; }
    public string Rank { get; set; }

    public Card(string suit, string rank)
    {
        Suit = suit;
        Rank = rank;
    }

    public string GetAsciiArt()
    {
        string topRank = Rank.Length > 1 ? Rank : Rank + " ";
        string bottomRank = Rank.Length > 1 ? Rank : " " + Rank;
        return
            "┌─────────┐\n" +
            $"│{topRank}       │\n" +
            "│         │\n" +
            $"│    {GetSuitSymbol()}    │\n" +
            "│         │\n" +
            $"│       {bottomRank}│\n" +
            "└─────────┘";
    }

    private string GetSuitSymbol()
    {
        return Suit switch
        {
            "Hearts" => "H",
            "Diamonds" => "D",
            "Clubs" => "C",
            "Spades" => "S",
            _ => " "
        };
    }
}

public class Deck
{
    private List<Card> cards = new List<Card>();

    public Deck()
    {
        Initialize();
        Shuffle();
    }

    private void Initialize()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" };
        string[] ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cards.Add(new Card(suit, rank));
            }
        }
    }

    public void Shuffle()
    {
        Random random = new Random();
        cards = cards.OrderBy(x => random.Next()).ToList();
    }

    public List<Card> DealCards(int count)
    {
        var dealtCards = cards.Take(count).ToList();
        cards.RemoveRange(0, Math.Min(count, cards.Count));
        return dealtCards;
    }
}