using System;
using System.Diagnostics;

namespace BarzerkGame
{
    public class EvilOtto : GameObject
    {
        private int targetX;
        private int targetY;
        static int ottoSpeedCounter = 0;
        static int ottoSpeedLimit = 700;
        public Stopwatch ottoTimer = new Stopwatch();
        public EvilOtto otto = null;

        public EvilOtto() : base(1, 1) {}

        public override void Draw(bool clear = false)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.SetCursorPosition(X, Y + 2);
            Console.Write(clear ? ' ' : 'O');
        }

        public void MoveTowardsPlayer(int playerX, int playerY)
        {
            Draw(true);
            targetX = playerX;
            targetY = playerY;
            if (X < targetX) X++;
            else if (X > targetX) X--;
            if (Y < targetY) Y++;
            else if (Y > targetY) Y--;
        }
        public void HandleEvilOtto(ref Player player, ref int playerY, ref int playerX, ref bool playing)
        {
            int millisecods = 60000;
            if (ottoTimer.ElapsedMilliseconds > millisecods && otto == null)
            {
                otto = new EvilOtto();
            }

            if (otto != null && ottoSpeedCounter >= ottoSpeedLimit)
            {
                otto.Draw(true);
                otto.MoveTowardsPlayer(playerX, playerY);
                otto.Draw();

                if (otto.X == playerX && otto.Y == playerY)
                {
                    player.playerLives--;

                    if (player.playerLives > 0)
                    {
                        Console.SetCursorPosition(0, Program.mapHeight + 4);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Run faster!");
                    }
                    if (player.playerLives <= 0)
                    {
                        Menu.AddScore(player.playerScore);
                        Menu.GameOver("GAME OVER! Evil Otto killed you");
                        playing = false;
                    }
                }
                ottoSpeedCounter = 7;
            }
            ottoSpeedCounter++;
        }
    }
}
