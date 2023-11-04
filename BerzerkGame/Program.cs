using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

namespace BarzerkGame
{
    class Program
    {
        private static List<Robot> robots = new List<Robot>();
        private static int wallWidth = 11;
        public static int wallHeight = 4;
        private static int roomWidth = 4;
        private static int roomHeight = 3;
        private static int mapWidth;
        public static int mapHeight;
        private static char[,] baseMap;
        static void Main(string[] args)
        {
            while (true)
            {
                int menuChoice = Menu.ShowMenu();

                switch (menuChoice)
                {
                    case 1:
                        InitializeGame();
                        break;
                    case 2:
                        Console.Clear();
                        Console.WriteLine("High Scores Placeholder");
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
        }
        
        public static void InitializeGame()
        {
            InitializeMap();
            Player player = new Player();
            EvilOtto evilOtto = new EvilOtto();
            bool playing = true;
            Labyrinth labyrinth = null;
            bool newMap = true;
            char[,] map = null;
            int playerY = 1;
            int playerX = 1;

            CreateMap(ref evilOtto, ref labyrinth, ref newMap, ref playerY, ref playerX, baseMap);
            Console.CursorVisible = false;

            while (playing)
            {

                if (newMap)
                {
                    map = DrawMap(labyrinth);
                    newMap = false;
                }

                player.DrawPlayer(playerY, playerX);
                player.HandlePlayerScore();
                player.HandlePlayerInput(robots, ref evilOtto, ref playerY, ref playerX, ref labyrinth, ref newMap, ref map);
                player.UpdateBullets(map, robots);
                Robot.HandleRobots(player, robots, playerY, playerX, map);
                evilOtto.HandleEvilOtto(ref player, ref playerY, ref playerX, ref playing);
            }
        }
       
        private static void InitializeMap()
        {
            mapWidth = (roomWidth + 1) * (wallWidth + 1) + 1;
            mapHeight = (roomHeight + 1) * (wallHeight + 1) + 1;
            baseMap = new char[mapHeight, mapWidth];
            SetBaseMapBorders(ref baseMap);
        }
       
        private static void SetBaseMapBorders(ref char[,] map)
        {
            int numRows = roomHeight;
            int numCols = roomWidth;
            int midRow = (int)Math.Ceiling(numRows / 2.0);
            int midCol = (int)Math.Ceiling(numCols / 2.0);

            for (int i = 0; i < mapHeight; i++)
            {
                map[i, 0] = 'X';
                map[i, mapWidth - 1] = 'X';
            }
            for (int j = 0; j < mapWidth; j++)
            {
                if (j <= midCol * (wallWidth + 1) || j >= (midCol + 1) * (wallWidth + 1))
                {
                    map[0, j] = 'X';
                    map[mapHeight - 1, j] = 'X';
                }
            }
        }
        
        public static void CreateMap(ref EvilOtto evilOtto, ref Labyrinth labyrinth, ref bool newMap, ref int playerY, ref int playerX, char[,] map)
        {
            robots.Clear();
            evilOtto.ottoTimer.Restart();
            evilOtto.otto = null;
            labyrinth = new Labyrinth(roomWidth, roomHeight, 4);
            newMap = true;
            playerY = 1;
            playerX = 1;

            char[,] mapTemplate = DrawMap(labyrinth);
            List<(int, int)> freeSpots = IdentifyFreeSpots(mapTemplate, playerX, playerY);
            PlaceRobotsRandomly(freeSpots);
        }
        
        private static List<(int, int)> IdentifyFreeSpots(char[,] mapTemplate, int playerX, int playerY)
        {
            List<(int, int)> freeSpots = new List<(int, int)>();
            for (int i = 1; i < mapHeight - 1; i++)
            {
                for (int j = 1; j < mapWidth - 1; j++)
                {
                    if (IsSpotFree(mapTemplate[i, j], i, j, playerX, playerY))
                        freeSpots.Add((i, j));
                }
            }
            return freeSpots;
        }
       
        private static bool IsSpotFree(char spot, int i, int j, int playerX, int playerY)
        {
            const int buffer = 2;
            return spot != '|' && spot != '-' && spot != 'X' && spot != 'O' &&
            Math.Abs(i - playerY) > buffer && Math.Abs(j - playerX) > buffer;
        }
        
        private static void PlaceRobotsRandomly(List<(int, int)> freeSpots)
        {
            Random rnd = new Random();
            Range robotRange = 3..9;
            int numberOfRobots = rnd.Next(robotRange.Start.Value, robotRange.End.Value);

            for (int i = 0; i < numberOfRobots; i++)
            {
                if (freeSpots.Count > 0)
                {
                    var index = rnd.Next(freeSpots.Count);
                    var (robotY, robotX) = freeSpots[index];
                    freeSpots.RemoveAt(index);
                    robots.Add(new Robot(robotX, robotY));
                }
            }
        }
        
        private static char[,] DrawMap(Labyrinth labyrinth)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("                        Berzerk Game \n");
            Console.ForegroundColor = ConsoleColor.Blue;
            int[,] room = labyrinth.GenerateRoom();
            char[,] map = GenerateMap(room);
            
            for (int i = 0; i < mapHeight; i++)
            {
                DrawMapRow(map, i);
            }

            return map;
        }
        
        private static void DrawMapRow(char[,] map, int rowIndex)
        {
            for (int j = 0; j < mapWidth; j++)
            {
                Console.Write(map[rowIndex, j] == '\0' ? ' ' : map[rowIndex, j]);
            }
            Console.WriteLine();
        }
        
        private static char[,] GenerateMap(int[,] room)
        {
            char[,] map = (char[,])baseMap.Clone();

            for (int i = 0; i < roomHeight; i++)
            {
                for (int j = 0; j < roomWidth; j++)
                {
                    Direction dir = (Direction)room[i, j];
                    int mapI = (i + 1) * (wallHeight + 1);
                    int mapJ = (j + 1) * (wallWidth + 1);
                    DrawWalls(dir, mapI, mapJ, map);
                    map[mapI, mapJ] = 'X';
                }
            }
            return map;
        }
        
        private static void DrawWalls(Direction dir, int mapI, int mapJ, char[,] map)
        {
            if (dir == Direction.UP || dir == Direction.DOWN)
            {
                int offset = dir == Direction.DOWN ? 1 : -1;
                for (int length = 1; length <= wallHeight; length++)
                {
                    map[mapI + (length * offset), mapJ] = 'X';
                }
            }
            else if (dir == Direction.RIGHT || dir == Direction.LEFT)
            {
                int offset = dir == Direction.RIGHT ? 1 : -1;
                for (int length = 1; length <= wallWidth; length++)
                {
                    map[mapI, mapJ + (length * offset)] = 'X';
                }
            }
        }
        
        public static bool MoveMap(Labyrinth labyrinth, ref int playerY, ref int playerX)
        {
            bool newMap = false;

            if (playerY == 0)
            {
                newMap = true;
                playerY = mapHeight - 2;
            }
            else if (playerY == mapHeight - 1)
            {
                newMap = true;
                playerY = 1;
            }
            return newMap;
        }
    }
}