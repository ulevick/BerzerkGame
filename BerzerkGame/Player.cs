using System;
using System.Collections.Generic;
using System.Linq;

namespace BarzerkGame
{
    public class Player
    {
        public int playerScore;
        public int playerLives;
        public static Direction LastMoveDirection { get; private set; } = Direction.RIGHT;
        private readonly List<Bullet> bullets = new List<Bullet>();
        private int bulletSpeedCounter = 0;
        private const int BulletSpeedLimit = 100;
        private int lastLifeAwardedAt = 0;

        public Player()
        {
            playerScore = 0;
            playerLives = 1;
        }

        public List<Bullet> Bullets => bullets;

        public void Move(char[,] map, ref int playerY, ref int playerX, ConsoleKeyInfo key)
        {
            int backupY = playerY;
            int backupX = playerX;
            HandlePlayerMovement(map, ref playerY, ref playerX, key);
            HandleWallCollision(map, ref playerY, ref playerX, backupY, backupX);

            if (key.Key == ConsoleKey.Spacebar)
            {
                bullets.Add(new Bullet(playerX, playerY, LastMoveDirection));
            }
        }

        private void HandlePlayerMovement(char[,] map, ref int playerY, ref int playerX, ConsoleKeyInfo key)
        {
            switch (key.Key)
            {
                case ConsoleKey.LeftArrow when playerX > 0:
                    playerX -= 1;
                    LastMoveDirection = Direction.LEFT;
                    break;
                case ConsoleKey.RightArrow when playerX < map.GetLength(1) - 1:
                    playerX += 1;
                    LastMoveDirection = Direction.RIGHT;
                    break;
                case ConsoleKey.DownArrow when playerY < map.GetLength(0) - 1:
                    playerY += 1;
                    LastMoveDirection = Direction.DOWN;
                    break;
                case ConsoleKey.UpArrow when playerY > 0:
                    playerY -= 1;
                    LastMoveDirection = Direction.UP;
                    break;
            }
        }

        private void HandleWallCollision(char[,] map, ref int playerY, ref int playerX, int backupY, int backupX)
        {
            if (map[playerY, playerX] == 'X')
            {
                playerLives--;

                if (playerLives <= 0)
                {
                    Menu.AddScore(playerScore);
                    Menu.GameOver("GAME OVER! You hit an electric wall");
                }
                else
                {
                    playerY = backupY;
                    playerX = backupX;
                    Console.SetCursorPosition(0, Program.mapHeight + 4);
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Don't crash into the walls, be careful!");
                }
            }
        }

        public void UpdateBullets(char[,] map, List<Robot> robots)
        {
            bulletSpeedCounter++;
            if (bulletSpeedCounter >= BulletSpeedLimit)
            {
                foreach (var bullet in bullets.ToList())
                {
                    bullet.Draw(true);
                    if (bullet.Move(map))
                    {
                        bullets.Remove(bullet);
                        continue;
                    }
                    bullet.Draw();

                    Robot hitRobot = robots.FirstOrDefault(r => r.X == bullet.X && r.Y == bullet.Y);
                    if (hitRobot != null)
                    {
                        robots.Remove(hitRobot);
                        hitRobot.Draw(true);
                        bullets.Remove(bullet);
                        playerScore += 20;
                    }
                }
                bulletSpeedCounter = 0;
            }
        }

        public void DrawPlayer(int playerI, int playerJ, bool clear = false)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.SetCursorPosition(playerJ, playerI + 2);
            Console.Write(clear ? ' ' : '@');
        }

        public void HandlePlayerScore()
        {
            int pointsPerLife = 50;
            int livesToAdd = (playerScore - lastLifeAwardedAt) / pointsPerLife;

            if (livesToAdd > 0)
            {
                playerLives += livesToAdd;
                lastLifeAwardedAt = playerScore;
            }
        }

        public void HandlePlayerInput(List<Robot> robots, ref EvilOtto evilOtto, ref int playerY, ref int playerX, ref Labyrinth labyrinth, ref bool newMap, ref char[,] map)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);
                DrawPlayer(playerY, playerX, true);
                Move(map, ref playerY, ref playerX, key);

                foreach (var robot in robots)
                {
                    if (robot.X == playerX && robot.Y == playerY)
                    {
                        playerLives--;
                    }
                }
                if (Program.MoveMap(labyrinth, ref playerY, ref playerX))
                {
                    Program.CreateMap(ref evilOtto, ref labyrinth, ref newMap, ref playerY, ref playerX, map);
                }
            }
        }
    }
}