using System;

namespace BarzerkGame
{
    public class Bullet : GameObject
    {
        private readonly Direction direction;

        public Bullet(int x, int y, Direction direction) : base(x, y)
        {
            this.direction = direction;
        }

        public bool Move(char[,] map)
        {
            int newX = X, newY = Y;

            switch (direction)
            {
                case Direction.UP:
                    newY--;
                    break;
                case Direction.DOWN:
                    newY++;
                    break;
                case Direction.RIGHT:
                    newX++;
                    break;
                case Direction.LEFT:
                    newX--;
                    break;
            }

            if (IsMoveValid(newX, newY, map))
            {
                X = newX;
                Y = newY;
                return false;
            }

            return true;
        }

        public bool Move(char[,] map)
        {
            int newX = X, newY = Y;

            switch (direction)
            {
                case Direction.UP:
                    newY--;
                    break;
                case Direction.DOWN:
                    newY++;
                    break;
                case Direction.RIGHT:
                    newX++;
                    break;
                case Direction.LEFT:
                    newX--;
                    break;
            }

            if (IsMoveValid(newX, newY, map))
            {
                X = newX;
                Y = newY;
                return false;
            }

            return true;
        }

        public override void Draw(bool clear = false)
        {
            Console.SetCursorPosition(X, Y + 2); // Y + 2 because of some offset handling, which could be specific to the game's UI.
            Console.Write(clear ? ' ' : '*');
        }
    }
}
