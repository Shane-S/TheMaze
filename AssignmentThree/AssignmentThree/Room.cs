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
    class Room : BasicShape
    {
        public void BuildShape(int numfaces, bool northWall, bool southWall, bool eastWall, bool westWall)
        {
            primitiveCount = numfaces * 2;
            vertices = new VertexPositionNormalTexture[6 * numfaces];
            Vector2 Texcoords = new Vector2(0f, 0f);

            Vector3[] face = new Vector3[6];

            //TopLeft
            face[2] = new Vector3(-size.X, size.Y, 0.0f);
            //BottomLeft
            face[3] = new Vector3(-size.X, -size.Y, 0.0f);
            //TopRight
            face[0] = new Vector3(size.X, size.Y, 0.0f);
            //BottomLeft
            face[1] = new Vector3(-size.X, -size.Y, 0.0f);
            //BottomRight
            face[5] = new Vector3(size.X, -size.Y, 0.0f);
            //TopRight
            face[4] = new Vector3(size.X, size.Y, 0.0f);

            Vector2 textureTopLeft = new Vector2(1f, 0.0f);
            Vector2 textureTopRight = new Vector2(0.0f, 0.0f);
            Vector2 textureBottomLeft = new Vector2(1f, 1f);
            Vector2 textureBottomRight = new Vector2(0.0f, 1f);

            int count = 0;

            //front face
            if (northWall)
            {
                vertices[0] = new VertexPositionNormalTexture(face[0] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureTopLeft);
                vertices[1] = new VertexPositionNormalTexture(face[1] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureBottomLeft);
                vertices[2] = new VertexPositionNormalTexture(face[2] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureTopRight);
                vertices[3] = new VertexPositionNormalTexture(face[3] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureBottomLeft);
                vertices[4] = new VertexPositionNormalTexture(face[4] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureBottomRight);
                vertices[5] = new VertexPositionNormalTexture(face[5] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, textureTopRight);

                count += 6;
            }


            //back face
            if (southWall)
            {
                vertices[0 + count] = new VertexPositionNormalTexture(face[2] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureTopLeft);
                vertices[1 + count] = new VertexPositionNormalTexture(face[1] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureTopRight);
                vertices[2 + count] = new VertexPositionNormalTexture(face[0] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureBottomLeft);
                vertices[3 + count] = new VertexPositionNormalTexture(face[5] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureBottomRight);
                vertices[4 + count] = new VertexPositionNormalTexture(face[4] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureTopRight);
                vertices[5 + count] = new VertexPositionNormalTexture(face[3] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, textureBottomLeft);

                count += 6;
            }


            //left face
            Matrix RotY90 = Matrix.CreateRotationY(-(float)Math.PI / 2f);
            if (westWall)
            {
                vertices[0 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[0], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureTopRight);
                vertices[1 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[1], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureBottomLeft);
                vertices[2 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[2], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureBottomRight);
                vertices[count + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[3], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureTopLeft);
                vertices[count + 4] = new VertexPositionNormalTexture(Vector3.Transform(face[4], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureBottomLeft);
                vertices[count + 5] = new VertexPositionNormalTexture(Vector3.Transform(face[5], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, textureTopRight);


                count += 6;
            }


            //Right face
            if (eastWall)
            {
                vertices[0 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[2], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureTopLeft);
                vertices[1 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[1], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureTopRight);
                vertices[2 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[0], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureBottomLeft);
                vertices[count + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureBottomLeft);
                vertices[count + 4] = new VertexPositionNormalTexture(Vector3.Transform(face[4], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureTopRight);
                vertices[count + 5] = new VertexPositionNormalTexture(Vector3.Transform(face[3], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, textureBottomRight);
                
                count += 6;
            }


            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(-(float)Math.PI / 2f);

            vertices[0 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[0], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureBottomLeft);
            vertices[1 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[1], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureTopRight);
            vertices[2 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[2], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureTopLeft);
            vertices[3 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[3], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureBottomLeft);
            vertices[4 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[4], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureBottomRight);
            vertices[5 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[5], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, textureTopRight);
            
            count += 6;


            //Bottom face
            vertices[0 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[2], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureTopLeft);
            vertices[1 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[1], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureTopRight);
            vertices[2 + count] = new VertexPositionNormalTexture(Vector3.Transform(face[0], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureBottomLeft);
            vertices[count + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureBottomLeft);
            vertices[count + 4] = new VertexPositionNormalTexture(Vector3.Transform(face[4], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureTopRight);
            vertices[count + 5] = new VertexPositionNormalTexture(Vector3.Transform(face[3], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, textureBottomRight);

        }

         public override void BuildShape()
        {
            primitiveCount = 12;
            vertices = new VertexPositionNormalTexture[36];
            Vector2 Texcoords = new Vector2(0f, 0f);

            Vector3[] face = new Vector3[6];
            //TopLeft
            face[2] = new Vector3(-size.X, size.Y, 0.0f);
            //BottomLeft
            face[3] = new Vector3(-size.X, -size.Y, 0.0f);
            //TopRight
            face[0] = new Vector3(size.X, size.Y, 0.0f);
            //BottomLeft
            face[1] = new Vector3(-size.X, -size.Y, 0.0f);
            //BottomRight
            face[5] = new Vector3(size.X, -size.Y, 0.0f);
            //TopRight
            face[4] = new Vector3(size.X, size.Y, 0.0f);

            //front face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i] = new VertexPositionNormalTexture(face[i] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, Texcoords);
                vertices[i + 3] = new VertexPositionNormalTexture(face[i + 3] + Vector3.UnitZ * size.Z + position, Vector3.UnitZ, Texcoords);
            }

            //back face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 6] = new VertexPositionNormalTexture(face[2 - i] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, Texcoords);
                vertices[i + 6 + 3] = new VertexPositionNormalTexture(face[5 - i] - Vector3.UnitZ * size.Z + position, -Vector3.UnitZ, Texcoords);
            }

            //left face
            Matrix RotY90 = Matrix.CreateRotationY(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 12] = new VertexPositionNormalTexture(Vector3.Transform(face[i], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, Texcoords);
                vertices[i + 12 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[i + 3], RotY90) - Vector3.UnitX * size.X + position, -Vector3.UnitX, Texcoords);
            }

            //Right face

            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 18] = new VertexPositionNormalTexture(Vector3.Transform(face[2 - i], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, Texcoords);
                vertices[i + 18 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5 - i], RotY90) + Vector3.UnitX * size.X + position, Vector3.UnitX, Texcoords);

            }

            //Top face
            Matrix RotX90 = Matrix.CreateRotationX(-(float)Math.PI / 2f);
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 24] = new VertexPositionNormalTexture(Vector3.Transform(face[i], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, Texcoords);
                vertices[i + 24 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[i + 3], RotX90) + Vector3.UnitY * size.Y + position, Vector3.UnitY, Texcoords);

            }

            //Bottom face
            for (int i = 0; i <= 2; i++)
            {
                vertices[i + 30] = new VertexPositionNormalTexture(Vector3.Transform(face[2 - i], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, Texcoords);
                vertices[i + 30 + 3] = new VertexPositionNormalTexture(Vector3.Transform(face[5 - i], RotX90) - Vector3.UnitY * size.Y + position, -Vector3.UnitY, Texcoords);
            }
        }
    }
}
