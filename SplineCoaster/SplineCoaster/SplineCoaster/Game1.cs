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

namespace SplineCoaster
{
    
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SpriteFont standartFont;
        Camera camera;
        Spline spline;
        Model sphere;
       

        //TEST
        float rotation;
        float mouseY;
        float mouseX;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new Camera(graphics.GraphicsDevice.Viewport);
            camera.Position = new Vector3(0, 0, 100);
            camera.LookAt = new Vector3(0, 0, 0);
            camera.Update();
            
            spline = new Spline(this, graphics,camera);
            
            Components.Add(spline);

            //TEST
            rotation = 0;
            

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            sphere = Content.Load<Model>("Models\\sphere"); // Kann man das auch irgendwie in Spline tun??????
            spline.sphere = sphere; // Kann man das auch irgendwie in Spline tun??????

            //Test
            standartFont = Content.Load<SpriteFont>("SpriteFont1");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back ==
                ButtonState.Pressed)
                this.Exit();

            float newMouseY = Mouse.GetState().Y;
            float newMouseX = Mouse.GetState().X;
            if (Keyboard.GetState().IsKeyDown(Keys.LeftControl))
            {
                camera.Position = Vector3.Transform(camera.Position, Matrix.CreateRotationY((mouseX - newMouseX)*0.05f));
                camera.Position = Vector3.Transform(camera.Position, Matrix.CreateRotationX((mouseY - newMouseY)*0.05f));
                camera.Update();
            }
            mouseX = newMouseX;
            mouseY = newMouseY;
            

            for (int i = 0; i < spline.splinePoints.Count; i++)
            {
                spline.SelectPoint(i);
                spline.SetTangentRotation(new Vector3(rotation,rotation,rotation));
            }

            // noch radians!!!!
            rotation+=0.01f;
            rotation %= 360;
            


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            spriteBatch.Begin();
                spriteBatch.DrawString(standartFont, rotation.ToString(), new Vector2(10, 10), Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
