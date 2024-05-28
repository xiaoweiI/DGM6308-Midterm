using System;
using System.Collections.Generic;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        bool playAgain = true;
        while (playAgain)
        {
            playAgain = StartGame();
        }
    }

    static bool StartGame()
    {
        int playerChips = 5;  // Initial bet chips
        const int winningChips = 50;  // Winning condition reduced to 50 chips

        while (playerChips < winningChips && playerChips > 0)
        {
            int predictedWins = GetPlayerPrediction();

            // Add betting system
            string betRankPrompt = "Choose a rank to bet on (A, 2, 3, ..., 10, J, Q, K), or enter R to read the rules:";
            Console.WriteLine(betRankPrompt);
            string betRank = GetInputWithRules(betRankPrompt).ToUpper();
            int betRankValue = GetRankValue(betRank);

            string betChipsPrompt = "Enter the number of chips to bet:";
            Console.WriteLine(betChipsPrompt);
            int betChips = GetChipsInput(playerChips, betChipsPrompt);

            Deck deck = new Deck();
            var playerCards = deck.DealCards(5); // Deal 5 cards to the player
            var computerCards = deck.DealCards(5); // Deal 5 cards to the computer

            int playerWins = 0;
            bool betWon = false;
            bool playerDrewAce = false;
            bool playerDrewRedJoker = false;
            bool playerDrewBlackJoker = false;

            Console.WriteLine("Game starting...\n");

            while (playerCards.Count > 0 && computerCards.Count > 0)
            {
                Console.WriteLine("Player's turn:");
                var playerCard = PlayTurn(ref playerCards, "Player");
                if (playerCard.Rank == "A")
                {
                    playerDrewAce = true;
                }
                if (playerCard.Suit == "Joker" && playerCard.Rank == "Red")
                {
                    playerDrewRedJoker = true;
                }
                if (playerCard.Suit == "Joker" && playerCard.Rank == "Black")
                {
                    playerDrewBlackJoker = true;
                }
                if (playerCard.Rank == betRank)
                {
                    betWon = true;
                }
                Console.WriteLine("\nComputer's turn:");
                var computerCard = PlayTurn(ref computerCards, "Computer", isComputer: true);

                // Compare cards
                int comparisonResult = CompareCards(playerCard, computerCard, playerDrewBlackJoker);
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
            bool predictionCorrect = predictedWins == playerWins;

            if (playerDrewAce)
            {
                Console.WriteLine($"You drew an Ace! You earn {betRankValue} extra chips regardless of your prediction.");
                playerChips += betRankValue;
            }

            if (playerDrewRedJoker)
            {
                Console.WriteLine($"You drew a Red Joker! Your prediction is considered correct. You earn {betChips} extra chips!");
                playerChips += betChips;
            }
            else if (predictionCorrect)
            {
                if (betWon)
                {
                    int totalChipsWon = betChips + betRankValue;
                    Console.WriteLine($"Congratulations! Your bet on {betRank} won and your prediction was correct. You earn {betChips} + {betRankValue} extra chips for a total of {totalChipsWon} chips!");
                    playerChips += totalChipsWon;
                }
                else
                {
                    Console.WriteLine($"Your prediction was correct, but your bet on {betRank} did not win. You earn {betChips} chips.");
                    playerChips += betChips;
                }
            }
            else
            {
                Console.WriteLine("Your prediction was incorrect, You lose the bet chips.");
                playerChips -= betChips; // Deduct chips if prediction is incorrect
            }

            Console.WriteLine($"You now have {playerChips} chips.");

            if (playerChips <= 0)
            {
                Console.WriteLine("You have lost all your chips. Game Over.");
                return AskToPlayAgain();
            }

            if (playerChips >= winningChips)
            {
                Console.WriteLine("Congratulations! You have accumulated 50 chips and won the game!");
                return AskToPlayAgain();
            }
        }

        return AskToPlayAgain();
    }

    static void ShowRules()
    {
        Console.WriteLine("\nRULES:");
        Console.WriteLine("1. Choose a rank to bet on (A, 2, 3, ..., 10, J, Q, K).");
        Console.WriteLine("2. Enter the number of chips to bet.");
        Console.WriteLine("3. The game will deal 5 cards to you and 5 cards to the computer.");
        Console.WriteLine("4. Players take turns to play a card.");
        Console.WriteLine("5. The card with the higher rank wins the round.");
        Console.WriteLine("6. Special rules for Aces and Jokers:");
        Console.WriteLine("   - If you draw an Ace, you earn extra chips equal to the rank you bet on.");
        Console.WriteLine("   - If you draw a Red Joker, your prediction is considered correct.");
        Console.WriteLine("   - If you draw a Black Joker, all your played cards' values are doubled for that round.");
        Console.WriteLine("7. If your prediction is correct, you earn chips based on your bet.");
        Console.WriteLine("8. If your prediction is incorrect, you lose the chips you bet.");
        Console.WriteLine("9. The game ends when you have 0 chips or accumulate 50 chips.");
        Console.WriteLine("\n");
    }

    static int GetPlayerPrediction()
    {
        string prompt = "Predict how many rounds you will win (0-5):";
        Console.WriteLine(prompt);
        int prediction;
        while (!int.TryParse(GetInputWithRules(prompt), out prediction) || prediction < 0 || prediction > 5)
        {
            Console.WriteLine("Invalid input. Please enter a number between 0 and 5:");
        }
        return prediction;
    }

    static int GetChipsInput(int maxChips, string prompt)
    {
        int chips;
        while (!int.TryParse(GetInputWithRules(prompt), out chips) || chips <= 0 || chips > maxChips)
        {
            Console.WriteLine("Invalid input. Please enter a number between 1 and your current chip count:");
        }
        return chips;
    }

    static string GetInputWithRules(string prompt)
    {
        string input = Console.ReadLine().ToUpper();
        while (input == "R")
        {
            ShowRules();
            Console.WriteLine("Enter B to go back to the game:");
            while (Console.ReadLine().ToUpper() != "B")
            {
                Console.WriteLine("Invalid input. Enter B to go back to the game:");
            }
            Console.WriteLine(prompt);
            input = Console.ReadLine().ToUpper();
        }
        return input;
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
            string prompt = "Choose a card to play (enter number): ";
            Console.WriteLine(prompt);
            int choice;
            while (!int.TryParse(GetInputWithRules(prompt), out choice) || choice < 1 || choice > cards.Count)
            {
                Console.WriteLine("Invalid input. Please enter a valid card number:");
            }
            selectedCard = cards[choice - 1];
            Console.WriteLine($"You played: {selectedCard.Rank} of {selectedCard.Suit}");
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

    static int CompareCards(Card playerCard, Card computerCard, bool doublePlayerValues)
    {
        // Compare based on rank first
        var rankOrder = new List<string> { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
        int playerRankIndex = rankOrder.IndexOf(playerCard.Rank);
        int computerRankIndex = rankOrder.IndexOf(computerCard.Rank);

        if (playerCard.Rank == "A")
        {
            playerRankIndex = 0; // Representing Ace as 1 (index 0)
        }

        if (computerCard.Rank == "A")
        {
            computerRankIndex = 0; // Representing Ace as 1 (index 0)
        }

        if (doublePlayerValues)
        {
            playerRankIndex *= 2;
        }

        return playerRankIndex.CompareTo(computerRankIndex);
    }

    static int GetRankValue(string rank)
    {
        return rank switch
        {
            "A" => 1,
            "2" => 2,
            "3" => 3,
            "4" => 4,
            "5" => 5,
            "6" => 6,
            "7" => 7,
            "8" => 8,
            "9" => 9,
            "10" => 10,
            "J" => 11,
            "Q" => 12,
            "K" => 13,
            _ => 0
        };
    }

    static bool AskToPlayAgain()
    {
        Console.WriteLine("Do you want to play again? (yes/no)");
        string input = Console.ReadLine().ToUpper();
        while (input != "YES" && input != "NO")
        {
            Console.WriteLine("Invalid input. Please enter 'yes' or 'no':");
            input = Console.ReadLine().ToUpper();
        }
        return input == "YES";
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
        if (Suit == "Joker")
        {
            string jokerType = Rank == "Black" ? "Black Joker" : "Red Joker";
            return
                "┌─────────┐\n" +
                $"│{jokerType}│\n" +
                "│         │\n" +
                "│   JOKER  │\n" +
                "│         │\n" +
                $"│{jokerType}│\n" +
                "└─────────┘";
        }
        else
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
    private static Random random = new Random(); // Static random instance

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

        // Add Joker cards
        cards.Add(new Card("Joker", "Black"));
        cards.Add(new Card("Joker", "Red"));
    }

    public void Shuffle()
    {
        cards = cards.OrderBy(x => random.Next()).ToList();
    }

    public List<Card> DealCards(int count)
    {
        var dealtCards = cards.Take(count).ToList();
        cards.RemoveRange(0, Math.Min(count, cards.Count));
        return dealtCards;
    }
}
