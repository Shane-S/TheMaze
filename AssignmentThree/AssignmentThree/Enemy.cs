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

        public Enemy(Vector3 initialPos, Vector3 targetPos, Model model, Texture2D texture, float moveSpeed)
        {
            Position = initialPos;
            TargetPos = targetPos;
            EnemyModel = model;
            Texture = texture;
            Speed = moveSpeed;
        }

        public void Update(GameTime gameTime)
        {
            // Do updating
        }
    }
}
