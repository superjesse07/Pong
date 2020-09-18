using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace Pong2
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Color background;
        List<GameObject> gameObjects = new List<GameObject>();
        public static Vector2 windowSize;
        public Ball ball;
        const float initialPlayerSpeed= 600, initialBallSpeed=200;

        Vector3 screenOffset;
        Random rand = new Random();
        float screenShakeDuration = 0;
        float screenShakeIntensity = 0;
        const float screenShakeFalloff = 0.4f;

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
            windowSize = new Vector2(Window.ClientBounds.Width, Window.ClientBounds.Height);
            base.Initialize();
        }


        public void Reset()
        {
            //ball.direction = new Vector2(1, 1);
            ball.speed = initialBallSpeed;
            ball.position = windowSize / 2f;
            foreach(GameObject go in gameObjects)
            {
                if(go is Paddle)
                {
                    go.position.Y = windowSize.Y / 2f;
                }
            }
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // Storing the textures in variables
            var ballTexture = Content.Load<Texture2D>("bal");
            var player1Texture = Content.Load<Texture2D>("rodeSpeler");
            var player2Texture = Content.Load<Texture2D>("blauweSpeler");

            // Creating the game objects
            
            ball = new Ball(this,ballTexture,windowSize/2f,new Vector2(1,1),initialBallSpeed);


            gameObjects.Add(ball);
            Paddle player1 = new Paddle(player1Texture, new Vector2(player1Texture.Width / 2f,windowSize.Y / 2f),initialPlayerSpeed, Keys.W,Keys.S);
            gameObjects.Add(player1);
            Paddle player2 = new Paddle(player2Texture, new Vector2(windowSize.X - player2Texture.Width / 2f, windowSize.Y / 2f),initialPlayerSpeed,Keys.Up,Keys.Down);
            gameObjects.Add(player2);
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            
        }

        /// <summary>
        /// Gets a color that shifts over time
        /// </summary>
        /// <param name="time">time value in seconds</param>
        /// <param name="speed"></param>
        /// <returns></returns>
        Color GetRainbowColor(double time,double speed)
        {
            double value = time *speed;
            double TAU = Math.PI * 2;

            float redComponent = (float)Math.Abs(Math.Sin(value* TAU));
            float greenComponent = (float)Math.Abs(Math.Sin(value * TAU + TAU / 3));
            float blueComponent = (float)Math.Abs(Math.Sin(value  * TAU + TAU * 2 / 3));
            return new Color(redComponent, greenComponent, blueComponent);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            /// 3 offset sin waves for RGB values
            background = GetRainbowColor(gameTime.TotalGameTime.TotalSeconds, 0.25);
            Rectangle ballBounds = ball.GetBounds();
            foreach(GameObject go in gameObjects)
            {
                go.Update(deltaTime);
                if(go is Paddle)
                {
                    var bounds = go.GetBounds();
                    if(ballBounds.Intersects(bounds))
                    {
                        if((go.position - ball.position).X * ball.direction.X > 0)
                        {
                            ball.OnCollide();
                            screenShakeDuration = 0.2f;
                            screenShakeIntensity = 10;
                        }
                    }
                }
            }

            if(screenShakeDuration > 0)
            {
                screenShakeDuration -= deltaTime;
                double angle = rand.NextDouble();
                double length = rand.NextDouble() * screenShakeIntensity * (Math.Min(screenShakeDuration, screenShakeFalloff)/screenShakeFalloff);
                screenOffset = new Vector3((float)(Math.Cos(angle * Math.PI * 2)*length), (float)(Math.Sin(angle * Math.PI * 2) * length), 0);
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(background);

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(screenOffset));
            foreach(GameObject go in gameObjects)
            {
                go.Draw(spriteBatch);
            }
            spriteBatch.End();

            base.Draw(gameTime);
        }
    
        public void PointScored(bool rightSide)
        {
            screenShakeDuration = 0.5f;
            screenShakeIntensity = 20;
            Reset();
        }
    
    }
}
