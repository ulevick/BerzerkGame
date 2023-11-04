using System;
using System.Collections.Generic;

namespace BarzerkGame
{
    public class Labyrinth
    {
        public int Seed;
        public int Height;
        public int Width;
        public int NumberOfDirections;
        private int roomNumber;
        private readonly Dictionary<int, int[,]> roomCache;
        private readonly Random rand;
        
        public int RoomNumber
        {
            get => roomNumber;
            set
            {
                roomNumber = value;
                GenerateRoom();
            }
        }
        
        public Labyrinth(int width, int height, int numberOfDirections, int seed = -1)
        {
            roomCache = new Dictionary<int, int[,]>();
            Height = height;
            Width = width;
            NumberOfDirections = numberOfDirections;
            Seed = seed == -1 ? Environment.TickCount : seed;
            rand = new Random(Seed);
            RoomNumber = Seed;
        }

        public int[,] GenerateRoom()
        {
            if (roomCache.TryGetValue(RoomNumber, out int[,] cachedRoom))
            {
                return cachedRoom;
            }

            int[,] newRoom = new int[Height, Width];
            for (int i = 0; i < Height; i++)
            {
                for (int j = 0; j < Width; j++)
                {
                    newRoom[i, j] = rand.Next(NumberOfDirections);
                }
            }
            roomCache.Add(RoomNumber, newRoom);
            return newRoom;
        }
    }
}
