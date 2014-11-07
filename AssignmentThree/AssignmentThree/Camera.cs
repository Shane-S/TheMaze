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
        public Matrix xRotationMatrix;
        public Matrix rotationMatrix;
        Vector3 headOffset; // height of the camera
        Vector3 cameraPosition;
        Vector3 cameraLookat; // the position the camera is looking at.
        Vector3 cameraReference; // The direction the camera points without rotation.
        double viewAngle;
        float nearClip;
        float farClip;
        float zoomFactor;
        float zoomAdjustment;
        float zoomMax;
        float zoomMin;

        public Matrix view;
        public Matrix proj;

        public Camera()
        {
            nearClip = 1.0f;
            farClip = 2000.0f;
            viewAngle = Math.PI / 4;
            zoomFactor = 1f;
            zoomAdjustment = 0.05f;
            zoomMax = 2;
            zoomMin = 0.40f;

            cameraReference = new Vector3(0, 0, 1);
            avatarHeadOffset = new Vector3(0, 0, 0);

            //xRotationMatrix = Matrix.CreateRotationY(avatarYaw);
            //rotationMatrix = xRotationMatrix * Matrix.CreateRotationX(avatarPitch);
            //headOffset = Vector3.Transform(avatarHeadOffset, rotationMatrix);
            //cameraPosition = avatarPosition + headOffset;
            //Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);
            //cameraLookat = transformedReference + cameraPosition;

            //view = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));

            //Viewport viewport = graphics.GraphicsDevice.Viewport;
            //float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            //proj = Matrix.CreatePerspectiveFieldOfView((float)viewAngle, aspectRatio,
            //    nearClip, farClip);
        }

        public void Update(Vector3 position, float avatarYaw, float avatarPitch, GraphicsDeviceManager graphics)
        {
            //xRotationMatrix = Matrix.CreateRotationY(avatarYaw);
            //rotationMatrix = xRotationMatrix * Matrix.CreateRotationX(avatarPitch);
            //headOffset = Vector3.Transform(avatarHeadOffset, rotationMatrix);
            //cameraPosition = avatarPosition + headOffset;
            //Vector3 transformedReference = Vector3.Transform(cameraReference, rotationMatrix);
            //cameraLookat = transformedReference + cameraPosition;

            //view = Matrix.CreateLookAt(cameraPosition, cameraLookat, new Vector3(0.0f, 1.0f, 0.0f));

            //Viewport viewport = graphics.GraphicsDevice.Viewport;
            //float aspectRatio = (float)viewport.Width / (float)viewport.Height;

            //proj = Matrix.CreatePerspectiveFieldOfView((float)viewAngle, aspectRatio,
            //    nearClip, farClip);

            // Create reference vector
            Vector3 lookAt = new Vector3(0f, 0f, 1f);

            // Rotate the Vector
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationX(MathHelper.ToRadians(avatarPitch)));
            lookAt = Vector3.Transform(lookAt, Matrix.CreateRotationY(MathHelper.ToRadians(avatarYaw)));

            // View
            view = Matrix.CreateLookAt(
                position,
                position + lookAt,
                Vector3.Up
            );

            view *= Matrix.CreateScale(new Vector3(zoomFactor, zoomFactor, 1));

            // Projection
            Viewport viewport = graphics.GraphicsDevice.Viewport;
            float aspectRatio = (float)viewport.Width / (float)viewport.Height;
            proj = Matrix.CreatePerspectiveFieldOfView((float)viewAngle, aspectRatio,
                nearClip, farClip);
        }

        public void ZoomIn()
        {
            zoomFactor += zoomAdjustment;

            if (Math.Abs(zoomFactor) > zoomMax)
            {
                zoomFactor = zoomMax;
            }
        }

        public void ZoomOut()
        {
            zoomFactor -= zoomAdjustment;

            if (Math.Abs(zoomFactor) < zoomMin)
            {
                zoomFactor = zoomMin;
            }
        }

        public void ResetZoom()
        {
            zoomFactor = 1;
        }
    }
}
