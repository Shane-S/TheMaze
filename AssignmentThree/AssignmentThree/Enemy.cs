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

        public Enemy(Vector3 initialPos, Vector3 targetPos, Model model, float moveSpeed)
        {
            Position = initialPos;
            TargetPos = targetPos;
            Model = model;
            Speed = moveSpeed;
        }

        public void Update(GameTime gameTime)
        {
            // Do updating
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
