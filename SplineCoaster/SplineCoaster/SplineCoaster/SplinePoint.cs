using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace SplineCoaster
{
    class SplinePoint
    {
        public Vector3 position;
        public Vector3 tangent;

        public SplinePoint(Vector3 position, Vector3 tangent)
        {
            this.position = position;
            this.tangent = tangent;

        }

        public SplinePoint()
        {
            this.position = new Vector3(0, 0, 0);
            this.tangent = new Vector3(0, 0, 1); ;
        }
    }
}
