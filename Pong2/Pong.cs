using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System;
using System.Collections.Generic;

namespace Pong2
{
    /// <summary>
    /// This is the main game class.
    /// </summary>
    public class Pong : Game
    {

        //singleton structure so gameobjects can make changes to game specific things like points
        private static Pong game;
        public static Pong Game { get => game; }
        private GameState gameState = GameState.MainMenu; // the current state of the game
        public static Vector2 windowSize => new Vector2(game.Window.ClientBounds.Width, game.Window.ClientBounds.Height); // window size to be used in the rest of the program

        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        private Color background; //the color of the background
        private Color text; //the color of the text (is always slightly offset from the background so its readable)
        private const float colorOffset = 2; // the amound with which the color is offset
        private const float colorChangeSpeed = 0.05f; // the speed with which the color changes

        private List<GameObject> gameObjects = new List<GameObject>(); // list off all the GameObjects in the game
        private Ball ball; // A round thing represented by a block in this game
        private const float initialPlayerSpeed = 400, initialBallSpeed = 200; // the initial speed of the player and ball

        private const float screenShakeFalloff = 0.4f; // the time left in seconds from which the screenshake starts to falloff
        private Vector3 screenOffset; // the amount with which the virtual camera is offset
        private Random rand = new Random(); // variable to create random values
        private float screenShakeDuration = 0; // the time the screenshake lasts
        private float screenShakeIntensity = 0; // the intensity of the screenshake

        private const float scoreOffset = 50; // the offset in pixels of the score display
        private int pointsLeft; //the amount of points the left side has scored
        private int pointsRight; // the amount of points the right side has scored
        private int pointsToWin = 2; // The amount of points needed to win
        private SpriteFont textFont; // the font with which all the text is displayed

        private SoundEffect paddleHitSFX, pointScoredSFX; // the sound effects that are played when a ball hits a paddle and when a point is scored



        public Pong()
        {
            game = this;
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
            base.Initialize();
        }

        /// <summary>
        /// Resets the points for both players
        /// </summary>
        public void ResetScore()
        {
            pointsLeft = 0;
            pointsRight = 0;
        }

        /// <summary>
        /// Resets all the objects back to their original positions
        /// </summary>
        public void Reset()
        {
            ball.direction *= new Vector2(1, 0); // reset the y direction of the ball
            ball.speed = initialBallSpeed; // reset the speed of the ball
            ball.position = windowSize / 2f; // reset the position of the ball
            foreach (GameObject go in gameObjects) // a loop over all gameObject
            {
                if (go is Paddle) // check if the object is a paddle
                {
                    go.position.Y = windowSize.Y / 2f; // set the paddle Y position to the center of the screen 
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

            //loading all the textures, sounds and fonts
            Texture2D ballTexture = Content.Load<Texture2D>("bal");
            Texture2D player1Texture = Content.Load<Texture2D>("rodeSpeler");
            Texture2D player2Texture = Content.Load<Texture2D>("blauweSpeler");
            textFont = Content.Load<SpriteFont>("Arial");
            paddleHitSFX = Content.Load<SoundEffect>("hitsfx");
            pointScoredSFX = Content.Load<SoundEffect>("dingsfx");
            // Creates a new ball at the center of the screen
            ball = new Ball(ballTexture, windowSize / 2f, new Vector2(1, 0), initialBallSpeed);
            // create the paddles for the players
            Paddle player1 = new Paddle(player1Texture, new Vector2(player1Texture.Width / 2f, windowSize.Y / 2f), initialPlayerSpeed, Keys.W, Keys.S);
            Paddle player2 = new Paddle(player2Texture, new Vector2(windowSize.X - player2Texture.Width / 2f, windowSize.Y / 2f), initialPlayerSpeed, Keys.Up, Keys.Down);

            // adds all the GameObject to the list
            gameObjects.Add(ball);
            gameObjects.Add(player1);
            gameObjects.Add(player2);
        }

        /// <summary>
        /// Gets a color that shifts over time
        /// </summary>
        /// <param name="time">time value in seconds</param>
        /// <param name="speed">the speed with which the color changes</param>
        /// <returns></returns>
        private Color GetRainbowColor(double time, double speed)
        {

            double value = time * speed; // the value that is used for the sine wave
            double TAU = Math.PI * 2; // helper variable because radians go from 0 to 2PI
            // the 3 color components of the color are all calculated by a sine wave that is offset by 1/3 PI for every next value
            float redComponent = (float)Math.Abs(Math.Sin(value * TAU));
            float greenComponent = (float)Math.Abs(Math.Sin(value * TAU + TAU / 3));
            float blueComponent = (float)Math.Abs(Math.Sin(value * TAU + TAU * 2 / 3));
            return new Color(redComponent, greenComponent, blueComponent);
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape)) // a way to accidentally close the game
                Exit();

            background = GetRainbowColor(gameTime.TotalGameTime.TotalSeconds, colorChangeSpeed); // setting the background color (or colour for educated people)
            text = GetRainbowColor(gameTime.TotalGameTime.TotalSeconds + colorOffset, colorChangeSpeed); // setting the text color with an offset to the background color (or colour for educated people)

            if (gameState == GameState.MainMenu || gameState == GameState.Finish) // check the gamestate
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Space)) // check if the spacebar is pressed
                {
                    gameState = GameState.Playing; // set gamestate to playing
                    Reset(); // reset all gameObjects
                    ResetScore(); // reset score
                }
            }
            else if (gameState == GameState.Playing) // check the gamestate
            {

                float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                foreach (GameObject go in gameObjects) // going over all gameobjects
                {
                    go.Update(deltaTime); // call the update method of the GameObject go
                    if (go is Paddle) // check if gameObject is of type paddle
                    {
                        if (ball.bounds.Intersects(go.bounds)) // check if the ball bounds interects the paddle bounds
                        {
                            if ((go.position - ball.position).X * ball.direction.X > 0) // check if the ball is heading towards the paddle
                            {
                                ball.OnCollide(go); // call the OnCollide method of the ball (go being the object it collided with)
                                screenShakeDuration = 0.2f; // initiate the nausea machine for 0.2 seconds
                                screenShakeIntensity = ball.speed / 20f; // set the intensity of the nausea machine
                                paddleHitSFX.CreateInstance().Play(); // play sound of the ball hitting the paddle
                            }
                        }
                    }
                }


                // This code controls the screenShake effect
                if (screenShakeDuration > 0) // if the screen shake is ongoing
                {
                    screenShakeDuration -= deltaTime; // decrease the duration by the time that has passedx
                    double angle = rand.NextDouble(); // pick a new random direction for the camera to move
                    double length = rand.NextDouble() * screenShakeIntensity * (Math.Min(screenShakeDuration, screenShakeFalloff) / screenShakeFalloff); // pick a random length for the vector the camera will move towards, with the Intensity being the max length which will decrease once the duration reaches the falloff point
                    screenOffset = new Vector3((float)(Math.Cos(angle * Math.PI * 2) * length), (float)(Math.Sin(angle * Math.PI * 2) * length), 0); //move the screen offset towards the randomly created vector with the direction and the length
                }
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

            spriteBatch.Begin(SpriteSortMode.Deferred, null, null, null, null, null, Matrix.CreateTranslation(screenOffset)); // start of the spriteBatch. the last variable is the transform matrix which moves the screen by the screenOffset (for screenshake)

            if (gameState == GameState.MainMenu) // check the gamestate
            {
                string startText = "Press Space to begin";
                spriteBatch.DrawString(textFont, startText, windowSize / 2f, text, 0, textFont.MeasureString(startText) / 2f, 1, SpriteEffects.None, 0); // display the text centered on the screen
            }
            else if (gameState == GameState.Playing)
            {
                foreach (GameObject go in gameObjects) // go over all the gameObjects
                {
                    go.Draw(spriteBatch); // draw the gameObject
                }

                spriteBatch.DrawString(textFont, pointsLeft.ToString(), new Vector2(scoreOffset, scoreOffset), text, 0, textFont.MeasureString(pointsLeft.ToString()) / 2f, 1, SpriteEffects.None, 0); // draw the score of the left player with a rainbow effect that is ofset
                spriteBatch.DrawString(textFont, pointsRight.ToString(), new Vector2(windowSize.X - scoreOffset, scoreOffset), text, 0, textFont.MeasureString(pointsRight.ToString()) / 2f, 1, SpriteEffects.None, 0); // draw the score of the right player with a rainbow effect that is ofset
            }
            else
            {
                string winText = $"{((pointsLeft > pointsRight) ? "Red" : "Blue")} has won, press Space to restart"; // create the win text, if left has more points than blue display that red has won, and the same for the other side.
                spriteBatch.DrawString(textFont, winText, windowSize / 2f, text, 0, textFont.MeasureString(winText) / 2f, 0.5f, SpriteEffects.None, 0); // display the text centered on the screen
            }
            spriteBatch.End(); // end of the sprite batch

            base.Draw(gameTime);
        }

        /// <summary>
        /// Handles the scoring of points and all the visual and sound effects that go with it
        /// </summary>
        /// <param name="rightSide">is true if a point was scored on the right and false if a point was scored on the left</param>
        public void PointScored(bool rightSide)
        {
            pointScoredSFX.CreateInstance().Play();

            screenShakeDuration = 0.5f; // initiate the nausea machine with a duration of 0.5 seconds
            screenShakeIntensity = 20; // set the intensity (20 felt right)
            Reset(); // reset all the GameObjects
            if (rightSide) // if the ball hits the right side grant left a point
            {
                pointsLeft++;
                if (pointsLeft == pointsToWin) gameState = GameState.Finish; // if the left side has reached the points to win set the game state to Finish
            }
            else // otherwise the ball has hit the left side so grant right a point
            {
                pointsRight++;
                if (pointsRight == pointsToWin) gameState = GameState.Finish; // if the right side has reached the points to win set the game state to Finish

            }
        }

    }
}

public enum GameState
{
    MainMenu,
    Playing,
    Finish
}