using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SplineCoaster
{
    class SplinePoint
    {
        private Vector3 position;
        private Vector3 tangent;
        private float scale;
        private Vector3 rotation;

        public SplinePoint(Vector3 position, Vector3 tangent)
        {
            Position = position;
            Tangent = tangent;
        }

        public SplinePoint(Vector3 position, Vector3 rotation, float scale)
        {
            Position = position;
            Rotation = rotation;
            Scale = scale;
        }

        public SplinePoint()
        {
            Position = new Vector3(0, 0, 0);
            Tangent = new Vector3(0, 0, 1); ;
        }

        public Vector3 Position
        { 
            get { return this.position; }
            set { this.position = value; }
        }

        public Vector3 Tangent
        {
            get { return this.tangent; }
            set 
            { 
                this.tangent = value;
                Vector3 rTangent = this.tangent;
                rTangent.Normalize();
                Vector2 XrotationVector = new Vector2(rTangent.Z, rTangent.Y);
                Vector2 YrotationVector = new Vector2(rTangent.X, rTangent.Z);
                float angleX = ((0 - XrotationVector.X) > 0 ? 1 : -1) * (float)Math.Acos((double)Vector2.Dot(Vector2.Normalize(XrotationVector), Vector2.Normalize(new Vector2(0, 1))));
                float angleY = ((0 - YrotationVector.X) > 0 ? 1 : -1) * (float)Math.Acos((double)Vector2.Dot(Vector2.Normalize(XrotationVector), Vector2.Normalize(new Vector2(0, 1))));
                this.rotation = new Vector3(angleX, angleY, 0);
                this.scale = value.Length();
            }
        }

        public Vector3 Rotation
        {
            get { return this.rotation; }
            set
            {
                this.rotation = value;
                float length = this.tangent.Length();
                Vector3 newTangent = Vector3.Transform(Vector3.Up * length, Quaternion.CreateFromYawPitchRoll(value.Y, value.X, value.Z));
                this.tangent = newTangent;
            }
        }

        public float Scale
        {
            get { return this.scale; }
            set
            {
                this.tangent.Normalize();
                this.tangent *= value;
                this.scale = value;
            }
        }
    }
}
