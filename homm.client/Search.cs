using System;
using System.CodeDom;
using System.Linq;
using HoMM.Sensors;
using HoMM;
using HoMM.ClientClasses;
using System.Collections.Generic;

namespace Homm.Client
{
	class Search
	{
		//public Search(bool[,] map, Location start, Location target)
		//{
		//	var path = MinPathBetween(start, target, map);

		//	foreach (var point in path)
		//	{
		//		Console.WriteLine("X,Y = " + point.X + "," + point.Y);
		//	}

		//	Console.ReadLine();
		//}

		public Direction[] MinPathBetween(Location start, Location target, bool[,] map)
		{
			var path = new Stack<Location>();
			if (!map[start.Y, start.X])
				return FormatPathToDirection(path);

			var mapPaths = new int[map.GetLength(0), map.GetLength(1)];

			MarkForward(map, mapPaths, start, target, 1);

			//for (int i = 0; i < mapPaths.GetLength(1); i++)
			//{
			//	for (int j = 0; j < mapPaths.GetLength(0); j++)
			//	{
			//		Console.Write("[" + i + "," + j + "]" + mapPaths[i, j] + ", ");
			//	}
			//	Console.WriteLine();
			//}

			SelectPath(map, mapPaths, start, target, path);
			return FormatPathToDirection(path);
		}

		private Direction[] FormatPathToDirection(Stack<Location> path)
		{
			var directionPath = new Direction[path.Count];
			var arrPath = path.ToArray();

			for (int i = 1; i < arrPath.Length; i++)
			{
				var x = arrPath[i].X - arrPath[i-1].X;
				var y = arrPath[i].Y - arrPath[i-1].Y;

				if (x == 1 && y == 1)
					directionPath[i-1] = Direction.RightDown;
				if (x == -1 && y == 1)
					directionPath[i-1] = Direction.LeftDown;
				if (x == 0 && y == 1)
					directionPath[i-1] = Direction.Down;
				if (x == 1 && y == -1)
					directionPath[i-1] = Direction.RightUp;
				if (x == -1 && y == -1)
					directionPath[i-1] = Direction.LeftUp;
				if (x == 0 && y == -1)
					directionPath[i-1] = Direction.Up;
			}

			return directionPath;
		}

		private void MarkForward(bool[,] map, int[,] mapPaths, Location start, Location target, int step)
		{
			if ((mapPaths[start.Y, start.X] == 0 || mapPaths[start.Y, start.X] > step)
				&& map[start.Y, start.X])
				mapPaths[start.Y, start.X] = step;
			else return;

			if (start.X == target.X && start.Y == target.Y)
				return;

			var rows = map.GetLength(1);
			var cols = map.GetLength(0);

			var ydir = new int[] { 1, 1, 1, -1, -1,-1 };
			var xdir = new int[] { 1, -1, 0, 1, -1, 0 };

			for (int i = 0; i < 6; i++)
			{
				if (IsValidPos(start.X + xdir[i], start.Y + ydir[i], rows, cols)
					&& map[start.Y + ydir[i], start.X + xdir[i]])
				{
					MarkForward(map, mapPaths,
						new Location(start.Y + ydir[i], start.X + xdir[i]), target, step + 1);
				}
			}
		}

		private void SelectPath(bool[,] map, int[,] mapPaths, Location start, Location target,
			Stack<Location> path)
		{
			path.Push(target);

			if ((target.X == start.X) && (target.Y == start.Y))
				return;

			var rows = mapPaths.GetLength(1);
			var cols = mapPaths.GetLength(0);
			var ydir = new int[] { 1, 1, 1, -1, -1, -1 };
			var xdir = new int[] { 1, -1, 0, 1, -1, 0 };

			for (int i = 0; i < 6; i++)
			{
				if (IsValidPos(target.X + xdir[i], target.Y + ydir[i], rows, cols)
					&& (mapPaths[target.Y + ydir[i], target.X + xdir[i]] == mapPaths[target.Y, target.X] - 1))
					{
						SelectPath(map, mapPaths, start,
							new Location(target.Y + ydir[i], target.X + xdir[i]), path);
						return;
					}
			}
		}

		private bool IsValidPos(int x, int y, int rows, int cols)
		{
			if ((x >= 0) && (x < rows) && (y >= 0) && (y < cols))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}