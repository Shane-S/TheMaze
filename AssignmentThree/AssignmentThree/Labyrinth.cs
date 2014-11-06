using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AssignmentThree
{
    class Labyrinth
    {
        int start;
        int finish;
        int width;
        int height;
        public float size;
        Cell[] labyrinth;
        LabyrinthGenerator generator;

        public Labyrinth(int labWidth, int labHeight)
        {
            width = labWidth;
            height = labHeight;

            Initialize();
        }

        private void Initialize()
        {
            int cellIndex = 0;
            labyrinth = new Cell[height * width];

            for (int y = 0; y < height; ++y)
            {
                for (int x = 0; x < width; ++x)
                {
                    cellIndex = y * width + x;
                    labyrinth[cellIndex] = new Cell(cellIndex, width, height);
                }
            }

	        generator = new LabyrinthGenerator(this);
        }

        public void GeneratePaths()
        {
	        if (labyrinth != null)
	        {
		        generator.GenerateLabyrinth(ref labyrinth, width, height);
	        }
        }

        public void Render()
        {
	        int displayWidth = (width * 2 + 1);
	        int displayHeight = (height * 2 + 1);
	        char[] displayArray = new char[displayWidth * displayHeight];

	        int col = 0, row = 0;
	        Cell currentCell;

	        for (int y = 0; y < displayHeight; ++y)
	        {
		        for (int x = 0; x < displayWidth; ++x)
		        {
			        displayArray[y * displayWidth + x] = '#';
		        }
	        }

	        for (int y = 1; y < displayHeight; y += 2)
	        {
		        col = 0;
		        for (int x = 1; x < displayWidth; x += 2)
		        {
			        if (row * width + col == start)
			        {
				        displayArray[y * displayWidth + x] = 'S';
			        }
			        else if (row * width + col == finish)
			        {
				        displayArray[y * displayWidth + x] = 'F';
			        }
			        else
			        {
				        displayArray[y * displayWidth + x] = '0';
			        }

			        currentCell = labyrinth[row * width + col];
			        if (currentCell.northWall)
			        {
				        displayArray[(y - 1) * displayWidth + x] = '#';
			        }
			        else
			        {
				        displayArray[(y - 1) * displayWidth + x] = '0';
			        }

			        if (currentCell.southWall)
			        {
				        displayArray[(y + 1) * displayWidth + x] = '#';
			        }
			        else
			        {
				        displayArray[(y + 1) * displayWidth + x] = '0';
			        }

			        if (currentCell.eastWall)
			        {
				        displayArray[y * displayWidth + (x + 1)] = '#';
			        }
			        else
			        {
				        displayArray[y * displayWidth + (x + 1)] = '0';
			        }

			        if (currentCell.westWall)
			        {
				        displayArray[y * displayWidth + (x - 1)] = '#';
			        }
			        else
			        {
				        displayArray[y * displayWidth + (x - 1)] = '0';
			        }

			        ++col;
		        }
		        ++row;
	        }

	        for (int y = 0; y < displayHeight; ++y)
	        {
		        for (int x = 0; x < displayWidth; ++x)
		        {
			        Console.Write(displayArray[y * displayWidth + x]);
		        }

		        Console.WriteLine();
	        }
        }


        public void SetStart(int spawnIndex)
        {
            start = spawnIndex;
        }

        public Vector3 GetPlayerSpawn()
        {
            Vector3 spawn = new Vector3(0, 0, 0);

            Cell cell = labyrinth[start];

            spawn.X = cell.x * size + size/2;
            spawn.Z = -((cell.y * size) + size / 2);

            return spawn;

        }

        public void SetFinish(int portalIndex)
        {
            finish = portalIndex;
        }

        public int GetSizeOfLabyrinth()
        {
            return width * height;
        }

        public List<Room> GenerateRooms(int size, ref List<Plane> northWalls, ref List<Plane> southWalls, ref List<Plane> eastWalls, ref List<Plane> westWalls, ref List<Plane> ceils, ref List<Plane> floors)
        {
            List<Room> rooms = new List<Room>();
            this.size = size * 2;

            for (int i = 0; i < (height * width); i++)
            {
                Cell cell = labyrinth[i];

                Room room = new Room();
                room.size = new Vector3(size, size, size);
                room.position = new Vector3(cell.x * size, 0, -cell.y * size);
                room.BuildShape(cell.GetNumberOfWalls(), cell.northWall, cell.southWall, cell.eastWall, cell.westWall, ref northWalls, ref southWalls, ref eastWalls, ref westWalls, ref ceils, ref floors);

                rooms.Add(room);
            }

            return rooms;
        }

        public Cell GetCellFromPosition(Vector3 position)
        {
            float z = Math.Abs(position.Z);
            float x = Math.Abs(position.X);

            if (z < 0)
            {
                z = 0;
            }
            else if (z >= height * size)
            {
                z = height * size;
            }

            if (x < 0)
            {
                x = 0;
            }
            else if (x >= width * size)
            {
                x = width * size;
            }

            int index = (int)(Math.Floor(z / size) * width + Math.Floor(x / size));
            
            if (index < 0)
            {
                Console.WriteLine("Y: " + Math.Floor(-position.Z / size) * width);
                Console.WriteLine("X: " + Math.Floor(position.X / size));
                return labyrinth[0];
            }
            else if (index >= width * height)
            {
                Console.WriteLine("Y: " + Math.Floor(-position.Z / size) * width);
                Console.WriteLine("X: " + Math.Floor(position.X / size));
                return labyrinth[(width * height) - 1];
            }
            else
            {
                return labyrinth[index];
            }

        }
    }
}
