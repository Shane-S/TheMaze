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
    class Camera
    {
        Vector3 avatarHeadOffset;
        public Matrix rotationMatrix;
        Vector3 headOffset; // height of the camera
        Vector3 cameraPosition;
        Vector3 cameraLookat; // the position the camera is looking at.
        Vector3 cameraReference; // The direction the camera points without rotation.
        double viewAngle;
        float nearClip;
        float farClip;

        public Matrix view;
        public Matrix proj;

        public Camera(Vector3 avatarPosition, float avatarYaw, float avatarPitch, GraphicsDeviceManager graphics)
        {
            nearClip = 1.0f;
            farClip = 2000.0f;
            viewAngle = Math.PI / 4;

            cameraReference = new Vector3(0, 0, 1);
            avatarHeadOffset = new Vector3(0, 0, 0);

            rotationMatrix = Matrix.CreateRotationY(avatarYaw) * Matrix.CreateRotationX(avatarPitch);
            headOffset = Vector3.Transform(avatarHeadOffset, rotationMatrix);
            cameraPosition = avatarPosition + headOffset;
            Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);
            cameraLookat = transformedReference + cameraPosition;

            view = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            proj = Matrix.CreatePerspectiveFieldOfView((float)viewAngle, aspectRatio,
                nearClip, farClip);
        }

        public void Update(Vector3 avatarPosition, float avatarYaw, float avatarPitch, GraphicsDeviceManager graphics)
        {
            rotationMatrix *= Matrix.CreateRotationY(avatarYaw) * Matrix.CreateRotationX(avatarPitch);
            headOffset = Vector3.Transform(avatarHeadOffset, rotationMatrix);
            cameraPosition = avatarPosition + headOffset;
            Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);
            cameraLookat = transformedReference + cameraPosition;

            view = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));

            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            proj = Matrix.CreatePerspectiveFieldOfView((float)viewAngle, aspectRatio,
                nearClip, farClip);
        }
    }
}
