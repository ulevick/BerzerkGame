using System;

namespace BarzerkGame
{
    public abstract class GameObject
    {
        public int X { get; set; }
        public int Y { get; set; }

        protected GameObject(int x, int y)
        {
            X = x;
            Y = y;
        }
        
        protected bool IsMoveValid(int x, int y, char[,] map)
        {
            return !(x < 0 || x >= map.GetLength(1) || y < 0 || y >= map.GetLength(0) || map[y, x] == 'X');
        }
        
        public abstract void Draw(bool clear = false);
    }
}