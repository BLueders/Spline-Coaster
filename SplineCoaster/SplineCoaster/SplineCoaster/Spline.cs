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
using System.Collections;


namespace SplineCoaster
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Spline : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public ArrayList splinePoints;
        private int selectedPoint;

        public Model sphere;
        BasicEffect basicSplineEffect;
        GraphicsDeviceManager graphics;
        Camera camera;

        private VertexPositionColor[] drawPointList;
        private short[] lineStripIndices;

        public Spline(Game game, GraphicsDeviceManager graphics, Camera camera)
            : base(game)
        {
            // TODO: Construct any child components here
            this.graphics = graphics;
            this.camera = camera;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            basicSplineEffect = new BasicEffect(this.graphics.GraphicsDevice);
            basicSplineEffect.VertexColorEnabled = true;
            basicSplineEffect.View = camera.ViewMatrix;
            basicSplineEffect.Projection = camera.ProjectionMatrix;                                        // near, far plane

            splinePoints = new ArrayList();

            // Add Test Data
            splinePoints.Add(new SplinePoint(new Vector3(0, 0, 0), new Vector3(0, 10, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(-15, 0, 0), new Vector3(10, 0, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(20, 0, 0), new Vector3(0, 10, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(-20, -10, 0), new Vector3(0, 10, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(0, 20, 0), new Vector3(10, 0, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(30, 0, 0), new Vector3(0, 10, 0)));
            createNewDrawPointList();
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        bool SelectPoint(Ray selectionRay)
        {
            return true;
        }

        public void SetPoint(Vector3 position)
        {

        }

        public void SetTangentRotation(Vector3 rotation)
        {

        }

        public void SetTangenScale(float scale)
        {

        }

        public Vector3 GetSelectedPosition()
        {
            return new Vector3();
        }

        public Vector3 GetSelectedRotation()
        {
            return new Vector3();
        }

        public float GetSelectedScale()
        {
            return 0;
        }

        public void AddNewPoint()
        {
            splinePoints.Add(new SplinePoint(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
        }

        public void AutoComplete()
        {

        }

        public override void Draw(GameTime gameTime)
        {
                basicSplineEffect.CurrentTechnique.Passes[0].Apply();
                
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                drawPointList,
                0,  // vertex buffer offset to add to each element of the index buffer
                drawPointList.Length,  // number of vertices in pointList
                lineStripIndices,  // the index buffer
                0,  // first index element to read
                lineStripIndices.Length-1   // number of primitives to draw
                );

                // Copy any parent transforms.
                Matrix[] transforms = new Matrix[sphere.Bones.Count];
                sphere.CopyAbsoluteBoneTransformsTo(transforms);
                
                // Draw all the Points
            foreach(SplinePoint point in splinePoints)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in sphere.Meshes)
                {
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.015f, 0.015f, 0.015f))
                            * Matrix.CreateTranslation(point.position);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f))
                            * Matrix.CreateTranslation(point.position+point.tangent);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f))
                            * Matrix.CreateTranslation(point.position - point.tangent);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                }

                basicSplineEffect.CurrentTechnique.Passes[0].Apply();
                VertexPositionColor[] line = new VertexPositionColor[3]{new VertexPositionColor(point.position-point.tangent,Color.Red),new VertexPositionColor(point.position,Color.Red),new VertexPositionColor(point.position+point.tangent,Color.Red)};
                short[] indices = new short[3]{0,1,2};
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                line,
                0,  // vertex buffer offset to add to each element of the index buffer
                3,  // number of vertices in pointList
                indices,  // the index buffer
                0,  // first index element to read
                2   // number of primitives to draw
                );
            }
        }

        private void DrawSelection()
        {

        }

        private Vector3[] getSplineIterations(int count)
        {
            return new Vector3[2];
        }

        private void createNewDrawPointList()
        {
            drawPointList = new VertexPositionColor[(splinePoints.Count-1) * 10];
            for (int i = 0; i < splinePoints.Count-1; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    float time = i + ((float)j) / 10;
                    drawPointList[i * 10 + j] = new VertexPositionColor(getPointOnSpline(time), Color.White);
                }
            }
            lineStripIndices = new short[drawPointList.Length];
            for (int i = 0; i < lineStripIndices.Length; i++)
            {
                lineStripIndices[i] = (short)(i);
            }
        }

        private void updateDrawPointList(int start, int end)
        {
            
        }

        private Vector3 getPointOnSpline(float time)
        {
            int p1Index = (int)Math.Floor(time);
            int p2Index = p1Index+1;
            float actTime = time - (float)p1Index;
            Vector3 p1 = ((SplinePoint)splinePoints[p1Index]).position;
            Vector3 t1 = p1 + ((SplinePoint)splinePoints[p1Index]).tangent;
            Vector3 p2 = ((SplinePoint)splinePoints[p2Index]).position;
            Vector3 t2 = p2 - ((SplinePoint)splinePoints[p2Index]).tangent;

            Vector3 E = p1 + actTime * (t1 - p1);
            Vector3 F = t1 + actTime * (t2 - t1);
            Vector3 G = t2 + actTime * (p2 - t2);
            Vector3 H = E + actTime * (F - E);
            Vector3 I = F + actTime * (G - F);
            Vector3 J = H + actTime * (I - H);
            return J;
        }
    }
}
