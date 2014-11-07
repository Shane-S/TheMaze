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
        public const float FOG_FAR_CLIP = 70.0f;
        public const float NORMAL_FAR_CLIP = 2000f;

        public Vector3 AvatarHeadOffset;
        public Vector3 CameraPosition;

        /// <summary>
        /// The direction in which the camera is looking (unit vector).
        /// </summary>
        public Vector3 CameraLookAt;

        /// <summary>
        /// The direction the camera points without rotation.
        /// </summary>
        public Vector3 CameraReference;

        /// <summary>
        /// The screen's aspect ratio.
        /// </summary>
        public float AspectRatio;

        /// <summary>
        /// The angle 
        /// </summary>
        public double ViewAngle;

        /// <summary>
        /// The near clipping plane.
        /// </summary>
        public float NearClip;

        /// <summary>
        /// The far clipping plane.
        /// </summary>
        public float FarClip;

        /// <summary>
        /// The view matrix.
        /// </summary>
        public Matrix View;

        /// <summary>
        /// The projection matrix.
        /// </summary>
        public Matrix Projection;

        public float ZoomFactor;
        public float ZoomAdjustment;
        public float ZoomMax;
        public float ZoomMin;

        public Camera(GraphicsDevice graphics)
        {
            // Projection
            Viewport viewport = graphics.Viewport;
            AspectRatio = (float)viewport.Width / (float)viewport.Height;
            ViewAngle = MathHelper.PiOver4;         

            ZoomFactor = 1f;
            ZoomAdjustment = 0.05f;
            ZoomMax = 2;
            ZoomMin = 0.40f;

            NearClip = 1.0f;
            FarClip = NORMAL_FAR_CLIP;

            Projection = Matrix.CreatePerspectiveFieldOfView((float)ViewAngle, AspectRatio,
                NearClip, FarClip);

            CameraReference = new Vector3(0, 0, 1);
            AvatarHeadOffset = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Updates the camera's position and viewing properties.
        /// </summary>
        /// <param name="position">The avatar's position.</param>
        /// <param name="avatarYaw">The avatar's yaw (rotation about the X axis).</param>
        /// <param name="avatarPitch">The avatar's pitch (rotation about the Y axis).</param>
        public void Update(Vector3 position, float avatarYaw, float avatarPitch)
        {           
            Matrix rotation;
            Matrix.CreateFromYawPitchRoll(MathHelper.ToRadians(avatarYaw), MathHelper.ToRadians(avatarPitch), 0f, out rotation);

            // Update the camera's position
            CameraPosition = position + AvatarHeadOffset;

            // Create LookAt vector by rotating the z-axis
            Vector3.Transform(ref CameraReference, ref rotation, out CameraLookAt);

            // Update the View vector
            View = Matrix.CreateLookAt(
                position,
                position + CameraLookAt,
                Vector3.Up
            );

            View *= Matrix.CreateScale(new Vector3(ZoomFactor, ZoomFactor, 1));
        }

        public void ZoomIn()
        {
            ZoomFactor += ZoomAdjustment;

            if (Math.Abs(ZoomFactor) > ZoomMax)
            {
                ZoomFactor = ZoomMax;
            }
        }

        public void ZoomOut()
        {
            ZoomFactor -= ZoomAdjustment;

            if (Math.Abs(ZoomFactor) < ZoomMin)
            {
                ZoomFactor = ZoomMin;
            }
        }

        public void ResetZoom()
        {
            ZoomFactor = 1;
        }
    }
}
