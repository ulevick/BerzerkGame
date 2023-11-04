using System;
using System.Collections.Generic;
using System.Linq;

namespace BarzerkGame
{
    public class Robot : GameObject
    {
        private static int robotSpeedCounter = 0;
        private static int robotSpeedLimit = 800;

        public Robot(int x, int y) : base(x, y) {}

        public override void Draw(bool clear = false)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.SetCursorPosition(X, Y + 2);
            Console.Write(clear ? ' ' : 'R');
            Console.ForegroundColor = ConsoleColor.White;
        }

        public static void HandleRobots(Player player, List<Robot> robots, int playerY, int playerX, char[,] map)
        {
            robotSpeedCounter++;

            if (robotSpeedCounter >= robotSpeedLimit)
            {
                List<Robot> robotsToRemove = new List<Robot>();

                foreach (var robot in robots)
                {
                    robot.Draw(true);
                    bool shouldRemove = robot.MoveTowardsPlayer(playerX, playerY, map);
                    robot.Draw();

                    if (shouldRemove)
                    {
                        robot.Draw(true);
                        robotsToRemove.Add(robot);
                    }
                    if (robot.X == playerX && robot.Y == playerY)
                    {
                        HandlePlayerCollision(player, robot, robotsToRemove);
                    }
                }

                HandleCollisions(robots, robotsToRemove, player);
                Menu.displayScore(player);
                robotSpeedCounter = 0;
            }
        }

        private bool MoveTowardsPlayer(int playerX, int playerY, char[,] map)
        {
            int oldX = X;
            int oldY = Y;

            double distance = CalculateDistance(playerX, playerY);

            if (distance <= 8)
            {
                TryMoveTowardsPlayer(playerX, playerY, map);
            }
            else
            {
                RandomlyMove(map);
            }

            if (oldX == X && oldY == Y)
            {
                return true;
            }

            return false;
        }

        private double CalculateDistance(int playerX, int playerY)
        {
            return Math.Sqrt(Math.Pow(playerX - X, 2) + Math.Pow(playerY - Y, 2));
        }

        private void TryMoveTowardsPlayer(int playerX, int playerY, char[,] map)
        {
            int deltaX = playerX - X;
            int deltaY = playerY - Y;

            int newX = X;
            int newY = Y;

            if (Math.Abs(deltaX) > Math.Abs(deltaY))
            {
                newX += Math.Sign(deltaX);
            }
            else
            {
                newY += Math.Sign(deltaY);
            }

            if (IsMoveValid(newX, newY, map))
            {
                X = newX;
                Y = newY;
            }
        }
        private void RandomlyMove(char[,] map)
        {
            Random rnd = new Random();
            List<int> directions = new List<int> { 0, 1, 2, 3 };

            while (directions.Count > 0)
            {
                int randomDirection = directions[rnd.Next(directions.Count)];
                directions.Remove(randomDirection);
                int tempX = X;
                int tempY = Y;

                switch (randomDirection)
                {
                    case 0:
                        tempX += 1;
                        break;
                    case 1:
                        tempX -= 1;
                        break;
                    case 2:
                        tempY += 1;
                        break;
                    case 3:
                        tempY -= 1;
                        break;
                }

                if (IsMoveValid(tempX, tempY, map))
                {
                    X = tempX;
                    Y = tempY;
                    break;
                }
            }
        }
        
        private static void HandlePlayerCollision(Player player, Robot robot, List<Robot> robotsToRemove)
        {
            player.playerLives--;
            if (player.playerLives > 0)
            {
                robotsToRemove.Add(robot);
                Console.SetCursorPosition(0, Program.mapHeight + 4);
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Run faster!");
            }
            else if (player.playerLives <= 0)
            {
                Menu.AddScore(player.playerScore);
                Menu.GameOver("GAME OVER! You were caught by a robot");
            }
        }
        
        private static void HandleCollisions(List<Robot> robots, List<Robot> robotsToRemove, Player player)
        {
            List<Robot> collidedRobots = new List<Robot>();
            foreach (var robot1 in robots)
            {
                foreach (var robot2 in robots)
                {
                    if (robot1 != robot2 && robot1.X == robot2.X && robot1.Y == robot2.Y)
                    {
                        collidedRobots.Add(robot1);
                        collidedRobots.Add(robot2);
                    }
                }
            }
            foreach (var robot in collidedRobots.Distinct())
            {
                robot.Draw(true);
                robots.Remove(robot);
                player.playerScore += 5;
            }
            foreach (var robot in robotsToRemove)
            {
                robot.Draw(true);
                robots.Remove(robot);
                player.playerScore += 10;
            }
        }
    }
}
