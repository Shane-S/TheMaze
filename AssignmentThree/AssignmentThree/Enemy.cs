using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace AssignmentThree
{
    class Enemy : IUpdateable, IDrawable
    {
        public Vector3 Position { get; set; }
        public Vector3 TargetPos { get; set; }
        public Model Model { get; set; }
        public float Speed { get; set; }
        
        enum States
        {
            moving,
            arrived,
        }

        private States state;
        private Random rand;
        private int speed;

        public Enemy(Vector3 initialPos, Vector3 targetPos, Model model, float moveSpeed)
        {
            Position = initialPos;
            TargetPos = targetPos;
            Model = model;
            Speed = moveSpeed;
            state = States.arrived;
            speed = 1;
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

                Vector3 targetPos = Vector3.Zero;

                List<Walls.dir> directions = new List<Walls.dir>{ Walls.dir.north, Walls.dir.south, Walls.dir.east, Walls.dir.west };
		        
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
                                targetPos = new Vector3(cell.x + 6, 6, cell.y - 18);
                            }
                            break;
                        case Walls.dir.south:
                            if (cell.southWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                targetPos = new Vector3(cell.x + 6, 6, cell.y + 18);
                            }
                            break;
                        case Walls.dir.east:
                            if (cell.eastWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                targetPos = new Vector3(cell.x + 18, 6, cell.y - 6);
                            }
                            break;
                        case Walls.dir.west:
                            if (cell.westWall)
                            {
                                directions.Remove(directions.First());
                            }
                            else
                            {
                                targetPos = new Vector3(cell.x - 18, 6, cell.y - 6);
                            }
                            break;
                        default:
                            break;
                    }

                    if (targetPos != Vector3.Zero)
                    {
                        state = States.moving;
                        TargetPos = targetPos;
                        break;
                    }
                }
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

                Position += toTarget * speed;
            }
        }

        public void Draw (GameTime gameTime)
        {
            Matrix world = rend.GetMatrix();
            Texture2D texture = rend.GetTexture();
            Matrix[] boneTransforms = new Matrix[Model.Bones.Count];
            Model.CopyAbsoluteBoneTransformsTo(boneTransforms);

            foreach (ModelMesh mm in Model.Meshes)
            {
                foreach (ModelMeshPart mmp in mm.MeshParts)
                {
                }
                mm.Draw();
                // Do drawing
            }
        }
    }
}
