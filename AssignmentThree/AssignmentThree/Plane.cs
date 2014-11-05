using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace AssignmentThree
{
    class Plane : BasicShape
    {
        public Plane(Vector3 size, Vector3 position, VertexPositionNormalTexture[] vertices)
        {
            primitiveCount = 2;
            this.size = size;
            this.position = position;
            this.vertices = vertices;
        }
    }
}
