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
        public int quality;
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
            quality = 20;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            selectedPoint = -1;
            float aspectRatio = graphics.GraphicsDevice.Viewport.AspectRatio;
            basicSplineEffect = new BasicEffect(this.graphics.GraphicsDevice);
            basicSplineEffect.VertexColorEnabled = true;
            basicSplineEffect.View = camera.ViewMatrix;
            basicSplineEffect.Projection = camera.ProjectionMatrix;                                        // near, far plane

            splinePoints = new ArrayList();

            // Add Test Data
            splinePoints.Add(new SplinePoint(new Vector3(0, 0, 0), new Vector3(0, -10, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(-15, 0, 0), new Vector3(10, 8, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(20, 0, 0), new Vector3(0, -15, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(-20, -10, 0), new Vector3(-20, 20, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(0, 20, 0), new Vector3(10, 0, 0)));
            splinePoints.Add(new SplinePoint(new Vector3(30, 0, 0), new Vector3(10, -5, 0)));
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

        public bool SelectPoint(Ray selectionRay)
        {
            return true;
        }

        public void SelectPoint(int index)
        {
            selectedPoint = index;
        }

        public void SetPoint(Vector3 position)
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
            {
                ((SplinePoint)splinePoints[selectedPoint]).Position = position;
                createNewDrawPointList();
            }
        }

        public void SetTangentRotation(Vector3 rotation)
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
            {
                ((SplinePoint)splinePoints[selectedPoint]).Rotation = rotation;
                createNewDrawPointList();
            }
            
        }

        public void SetTangenScale(float scale)
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
            {
                ((SplinePoint)splinePoints[selectedPoint]).Scale = scale;
                createNewDrawPointList();
            }
           
        }

        public Vector3 GetSelectedPosition()
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
                return ((SplinePoint)splinePoints[selectedPoint]).Position;
            else
                return new Vector3(0, 0, 0);
        }

        public Vector3 GetSelectedRotation()
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
            {
                return ((SplinePoint)splinePoints[selectedPoint]).Rotation;
            }
                
            else
                return new Vector3(0, 0, 0);
        }

        public float GetSelectedScale()
        {
            if (selectedPoint != -1 && selectedPoint < splinePoints.Count)
            {
                return ((SplinePoint)splinePoints[selectedPoint]).Scale;
            }

            else
                return 0;
            
        }

        public void AddNewPoint()
        {
            splinePoints.Add(new SplinePoint(new Vector3(0, 0, 0), new Vector3(0, 0, 1)));
            createNewDrawPointList();
        }

        public void AutoComplete()
        {
            splinePoints.Add(new SplinePoint(((SplinePoint)splinePoints[0]).Position,
                                                ((SplinePoint)splinePoints[0]).Tangent));
            createNewDrawPointList();
        }

        public override void Draw(GameTime gameTime)
        {
                
            basicSplineEffect.CurrentTechnique.Passes[0].Apply();
            basicSplineEffect.View = camera.ViewMatrix;
            basicSplineEffect.Projection = camera.ProjectionMatrix;
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
                
            // Draw all the Points / Tangents / The Spline
            foreach(SplinePoint point in splinePoints)
            {
                // Draw the model. A model can have multiple meshes, so loop.
                foreach (ModelMesh mesh in sphere.Meshes)
                {
                    // Draw the Points
                    // This is where the mesh orientation is set, as well 
                    // as our camera and projection.
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.015f, 0.015f, 0.015f))
                            * Matrix.CreateTranslation(point.Position);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();

                    // Draw the Tangentpoints
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f))
                            * Matrix.CreateTranslation(point.Position+point.Tangent);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    mesh.Draw();

                    // Draw the other side Tangetpoints
                    foreach (BasicEffect effect in mesh.Effects)
                    {
                        effect.EnableDefaultLighting();
                        effect.World = transforms[mesh.ParentBone.Index]
                            * Matrix.CreateScale(new Vector3(0.01f, 0.01f, 0.01f))
                            * Matrix.CreateTranslation(point.Position - point.Tangent);
                        effect.View = camera.ViewMatrix;
                        effect.Projection = camera.ProjectionMatrix;
                    }
                    mesh.Draw();
                }

                // Draw the Connection between
                basicSplineEffect.CurrentTechnique.Passes[0].Apply();
                basicSplineEffect.View = camera.ViewMatrix;
                basicSplineEffect.Projection = camera.ProjectionMatrix;
                VertexPositionColor[] line = new VertexPositionColor[2]
                        {new VertexPositionColor(point.Position-point.Tangent,Color.Red), 
                         new VertexPositionColor(point.Position+point.Tangent,Color.Red)};
                short[] indices = new short[2]{0,1};
                graphics.GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColor>(
                PrimitiveType.LineStrip,
                line,
                0,  // vertex buffer offset to add to each element of the index buffer
                2,  // number of vertices in pointList
                indices,  // the index buffer
                0,  // first index element to read
                1   // number of primitives to draw
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
            drawPointList = new VertexPositionColor[(splinePoints.Count - 1) * quality];
            for (int i = 0; i < splinePoints.Count-1; i++)
            {
                for (int j = 0; j < quality; j++)
                {
                    float time = i + ((float)j) / quality;
                    drawPointList[i * quality + j] = new VertexPositionColor(getPointOnSpline(time), Color.White);
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
            Vector3 p1 = ((SplinePoint)splinePoints[p1Index]).Position;
            Vector3 t1 = p1 + ((SplinePoint)splinePoints[p1Index]).Tangent;
            Vector3 p2 = ((SplinePoint)splinePoints[p2Index]).Position;
            Vector3 t2 = p2 - ((SplinePoint)splinePoints[p2Index]).Tangent;

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
