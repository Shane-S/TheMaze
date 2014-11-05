using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssignmentThree
{
    class LabyrinthGenerator
    {
	    private Labyrinth owner;

	    private int ChooseIndex(int ceil)
        {
            return ceil - 1; // choose newest
        }

	    public LabyrinthGenerator(Labyrinth  myOwner)
        {
            owner = myOwner;
        }

        private void Shuffle<T>(IList<T> list)
        {
            Random rng = new Random();
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public void GenerateLabyrinth(ref Cell[] lab, int width, int height)
        {
            int index;
	        int count = 1;
            Random rand = new Random();

	        List<int> C = new List<int>();

	        int[] grid = new int[width * height];

	        for (int row = 0; row < height; ++row)
	        {
		        for (int col = 0; col < width; ++col)
		        {
			        grid[row * width + col] = 0;
		        }
	        }

	        int x = rand.Next(0, width);
	        int y = rand.Next(0, height);
	        int ny = y;
	        int nx = x;
	        int cellCoord = y * width + x;

	        grid[cellCoord] = count;
	        ++count;

	        owner.SetStart(cellCoord);
            
            C.Add(cellCoord);

	        while (C.Count != 0)
	        {
		        index = ChooseIndex(C.Count);
		        cellCoord = C[index];
		        nx = x = cellCoord % width;
		        ny = y = (cellCoord - x) / width;

		        List<Walls.dir> directions = new List<Walls.dir>{ Walls.dir.north, Walls.dir.south, Walls.dir.east, Walls.dir.west };
		        index = -1;

		        while (directions.Count != 0)
		        {
                    Shuffle(directions);

			        switch (directions.First())
			        {
			        case Walls.dir.north:
				        ny--;
				        break;
			        case Walls.dir.south:
				        ny++;
				        break;
			        case Walls.dir.east:
				        nx++;
				        break;
			        case Walls.dir.west:
				        nx--;
				        break;
			        default:
				        break;
			        }

			        int status;

                    if (nx >= 0 
                        && ny >= 0 
                        && nx < width 
                        && ny < height 
                        && (status = grid[ny * width + nx]) == 0)
			        {
				        cellCoord = ny * width + nx;
				        C.Add(cellCoord);
				        grid[cellCoord] = count;

				        if (count == width * height)
				        {
					        owner.SetFinish(cellCoord);
				        }

				        ++count;

				        switch (directions.First())
				        {
				        case Walls.dir.north:
					        lab[y * width + x].RemoveWall(Walls.dir.north);
					
					        if ((y - 1) >= 0)
					        {
						        lab[(y - 1) * width + x].RemoveWall(Walls.dir.south);
					        }
					        break;
				        case Walls.dir.south:
					        lab[y * width + x].RemoveWall(Walls.dir.south);

					        if ((y + 1) < height)
					        {
						        lab[(y + 1) * width + x].RemoveWall(Walls.dir.north);
					        }
					        break;
				        case Walls.dir.east:
					        lab[y * width + x].RemoveWall(Walls.dir.east);

					        if ((x + 1) < width)
					        {
						        lab[y * width + (x + 1)].RemoveWall(Walls.dir.west);
					        }
					        break;
				        case Walls.dir.west:
					        lab[y * width + x].RemoveWall(Walls.dir.west);

					        if ((x - 1) >= 0)
					        {
						        lab[y * width + (x - 1)].RemoveWall(Walls.dir.east);
					        }
					        break;
				        default:
					        break;
				        }

				        index = 0;
				        break;
			        }
			        else
			        {
				        nx = x;
				        ny = y;

				        directions.Remove(directions.First());
			        }
		        }

		        if (index < 0)
		        {
			        C.Remove(C.Last());
		        }
	        }

	        for (int row = 0; row < height; ++row)
	        {
		        for (int col = 0; col < width; ++col)
		        {
			        Console.Write(grid[row * width + col] + ", ");
		        }

		        Console.WriteLine();
	        }

            Console.WriteLine();
            Console.WriteLine();

        }
    }
}
