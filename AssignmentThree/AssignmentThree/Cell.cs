using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentThree
{
    class Cell
    {
        public int x;
        public int y;
        public bool northWall;
        public bool southWall;
        public bool eastWall;
        public bool westWall;

        private int numWalls;

        public Cell(int cellIndex, int width, int height)
        {
            x = cellIndex % width;
            y = (cellIndex - x) / width;

            northWall = true;
            southWall = true;
            eastWall = true;
            westWall = true;

            numWalls = 6; // include floor and ceiling
        }

        public void RemoveWall(Walls.dir wall)
        {
            switch (wall)
            {
                case Walls.dir.north:
                    northWall = false;
                    numWalls--;
                    break;
                case Walls.dir.south:
                    southWall = false;
                    numWalls--;
                    break;
                case Walls.dir.east:
                    eastWall = false;
                    numWalls--;
                    break;
                case Walls.dir.west:
                    westWall = false;
                    numWalls--;
                    break;
            }
        }

        public int GetNumberOfWalls()
        {
            return numWalls;
        }
    }
}
