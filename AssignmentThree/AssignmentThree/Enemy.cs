using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AssignmentThree
{
    class Enemy
    {
        public Vector3 Position { get; set; }
        public Vector3 TargetPos { get; set; }
        public Model EnemyModel { get; set; }
        public Texture2D Texture { get; set; }
        public float Speed { get; set; }
        
        enum States
        {
            moving,
            arrived,
        }

        private States state;
        private Random rand;
        private Walls.dir lastDir;
        private Walls.dir selectedDir;

        public Enemy(Vector3 initialPos, Vector3 targetPos, Model model, Texture2D texture, float moveSpeed)
        {
            Position = initialPos;
            TargetPos = targetPos;
            EnemyModel = model;
            Texture = texture;
            Speed = moveSpeed;
            state = States.arrived;
            rand = new Random();
        }

        private IList<T> Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rand.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }

            return list;
        }

        public void Update(GameTime gameTime, Labyrinth world)
        {
            if (state == States.arrived)
            {
                Cell cell = world.GetCellFromPosition(Position);
                float cellX = cell.x * world.size;
                float cellY = cell.y * world.size;

                Vector3 targetPos = Vector3.Zero;

                List<Walls.dir> directions = new List<Walls.dir>{ Walls.dir.north, Walls.dir.south, Walls.dir.east, Walls.dir.west };

                List<Walls.dir> possibleDirections = new List<Walls.dir>();
                
                while (directions.Count != 0)
                {
                    directions = (List<Walls.dir>)Shuffle(directions);

                    switch (directions.First())
                    {
                        case Walls.dir.north:
                            if (cell.northWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                possibleDirections.Add(Walls.dir.north);
                                directions.Remove(directions.First());
                            }
                            break;
                        case Walls.dir.south:
                            if (cell.southWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                possibleDirections.Add(Walls.dir.south);
                                directions.Remove(directions.First());
                            }
                            break;
                        case Walls.dir.east:
                            if (cell.eastWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                possibleDirections.Add(Walls.dir.east);
                                directions.Remove(directions.First());
                            }
                            break;
                        case Walls.dir.west:
                            if (cell.westWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                possibleDirections.Add(Walls.dir.west);
                                directions.Remove(directions.First());
                            }
                            break;
                        default:
                            break;
                    }

                }

                if (selectedDir != null)
                {
                    lastDir = selectedDir;
                }
                else
                {
                    lastDir = possibleDirections.First();
                }

                selectedDir = possibleDirections.First();

                foreach (Walls.dir direct in possibleDirections)
                {
                    switch(lastDir)
                    {
                        case Walls.dir.north:
                            if (direct != Walls.dir.south)
                            {
                                selectedDir = direct;
                            }
                            break;
                        case Walls.dir.south:
                            if (direct != Walls.dir.north)
                            {
                                selectedDir = direct;
                            }
                            break;
                        case Walls.dir.east:
                            if (direct != Walls.dir.west)
                            {
                                selectedDir = direct;
                            }
                            break;
                        case Walls.dir.west:
                            if (direct != Walls.dir.east)
                            {
                                selectedDir = direct;
                            }
                            break;
                        default:
                            break;
                    }
                }

                switch (selectedDir)
                {
                    case Walls.dir.north:
                        targetPos = new Vector3(cellX + 6, 0, -cellY + 6);
                        break;
                    case Walls.dir.south:
                        targetPos = new Vector3(cellX + 6, 0, -cellY - 18);
                        break;
                    case Walls.dir.east:
                        targetPos = new Vector3(cellX + 18, 0, -cellY - 6);
                        break;
                    case Walls.dir.west:
                        targetPos = new Vector3(cellX - 6, 0, -cellY - 6);
                        break;
                    default:
                        break;
                }
                TargetPos = targetPos;
                state = States.moving;
            }
            else if (state == States.moving)
            {
                Vector3 toTarget = TargetPos - Position;
                float length = toTarget.Length();
                toTarget = Vector3.Normalize(toTarget);

                if (length < 1.5)
                {
                    state = States.arrived;
                }

                Position += toTarget * Speed;
            }
        }
    }
}
