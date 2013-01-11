using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SplineCoaster
{
    public class Camera
    {
        private Vector3 _position;
        private Vector3 _lookAt;
        private Matrix _viewMatrix;
        private Matrix _projectionMatrix;
        private float _aspectRatio;

        float camXrotation;
        float camYrotation;
        float mouseX;
        float mouseY;
        float mouseWheel;

        public Camera(Viewport viewport)
        {
            this._aspectRatio = ((float)viewport.Width) / ((float)viewport.Height);
            this._projectionMatrix = Matrix.CreatePerspectiveFieldOfView(
                                        MathHelper.ToRadians(40.0f),
                                        this._aspectRatio,
                                        1.0f,
                                        10000.0f);
        }

        public Vector3 Position
        {
            get { return this._position; }
            set { this._position = value; }
        }
        public Vector3 LookAt
        {
            get { return this._lookAt; }
            set { this._lookAt = value; }
        }
        public Matrix ViewMatrix
        {
            get { return this._viewMatrix; }
        }
        public Matrix ProjectionMatrix
        {
            get { return this._projectionMatrix; }
        }
        public void Update()
        {
            float newMouseY = Mouse.GetState().Y;
            float newMouseX = Mouse.GetState().X;
            Matrix newPosition = Matrix.CreateScale(Position.Length()
                    * (((mouseWheel - Mouse.GetState().ScrollWheelValue) * 0.001f) + 1))
                    * Matrix.CreateRotationX(camXrotation)
                    * Matrix.CreateRotationY(camYrotation);
            mouseWheel = Mouse.GetState().ScrollWheelValue;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                camYrotation += (mouseX - newMouseX) * 0.05f;
                camXrotation += (mouseY - newMouseY) * 0.05f;
            }
            Position = Vector3.Transform(Vector3.Forward, newPosition);
            mouseX = newMouseX;
            mouseY = newMouseY;
            
            
            this._viewMatrix =
                Matrix.CreateLookAt(this._position, this._lookAt, Vector3.Up);
        }
    }
}
