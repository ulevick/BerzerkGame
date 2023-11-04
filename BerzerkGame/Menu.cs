using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using static BarzerkGame.Program;

namespace BarzerkGame
{
    class Menu
    {

        static List<int> scores = new List<int>();
        static int maxScoresToKeep = 10;

        public static int ShowMenu()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Barzerk Game Menu");
            Console.WriteLine("1. Start New Game");
            Console.WriteLine("2. View High Scores");  // Placeholder for now
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");

            int choice;
            if (int.TryParse(Console.ReadLine(), out choice) && choice >= 1 && choice <= 3)
            {
                return choice;
            }
            else
            {
                return 0;  // Invalid choice
            }
        }

        public static void GameOver(string message)
        {
            Player player = new Player();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(0, Program.mapHeight + 2);
            Console.WriteLine(message + ". Press R to restart or E to return to the main menu.");

            while (true)
            {
                var key = Console.ReadKey(true).Key;

                if (key == ConsoleKey.R)
                {
                    player.playerScore = 0;
                    player.playerLives = 1;
                    Program.InitializeGame(); // Restart the game
                    return;
                }
                else if (key == ConsoleKey.E)
                {
                    player.playerScore = 0;
                    player.playerLives = 1;
                    while (true)
                    {
                        int menuChoice = ShowMenu();

                        switch (menuChoice)
                        {
                            case 1:
                                Program.InitializeGame();
                                break;
                            case 2:
                                Console.Clear();
                                Console.WriteLine("High Scores Placeholder");
                                DisplayScores();
                                Console.WriteLine("Press any key to return to the menu.");
                                Console.ReadKey();
                                break;
                            case 3:
                                Environment.Exit(0);
                                break;
                            default:
                                Console.WriteLine("Invalid choice. Press any key to try again.");
                                Console.ReadKey();
                                break;
                        }
                    }
                    return;  // Exit the GameOver method after handling the menu choice
                }
            }
        }
        public static void AddScore(int score)
        {
            scores.Add(score);
            scores = scores.OrderByDescending(x => x).ToList();

            if (scores.Count > maxScoresToKeep)
            {
                scores.RemoveRange(maxScoresToKeep, scores.Count - maxScoresToKeep);
            }
        }
        public static void DisplayScores()
        {
            Console.Clear();
            Console.WriteLine("---- HIGH SCORES ----");
            foreach (var score in scores)
            {
                Console.WriteLine(score);
            }
        }
        public static void displayScore(Player player)
        {
            Console.SetCursorPosition(0, mapHeight + 3);
            Console.WriteLine($"Score: {player.playerScore} Lives: {player.playerLives}");
        }
    }
}