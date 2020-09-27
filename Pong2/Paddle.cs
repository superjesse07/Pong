using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong2
{
    /// <summary>
    /// Class for all the paddles in the game.
    /// This is a subclass of GameObjects.
    /// This paddles have a position, a texture, a bounds, a up and down key and a speed. 
    /// </summary>
    public class Paddle : GameObject
    {
        Keys up, down; // the keys which the player uses to move up and down

        public Paddle(Texture2D texture, Vector2 position,float speed,Keys up, Keys down) : base(texture,position,speed)
        {
            this.speed = speed;
            this.up = up;
            this.down = down;
        }

        /// <summary>
        /// Update function to do general changes to the paddle per frame
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            KeyboardState state = Keyboard.GetState();
            if(state.IsKeyDown(up)) // triggered if the up key is pressed
            {
                position -= new Vector2(0, speed * deltaTime); // update the position of the paddle with speed per seconds downwards
            }
            if(state.IsKeyDown(down)) // triggered if the down key is pressed
            {
                position += new Vector2(0, speed * deltaTime); // update the position of the paddle with speed per seconds upwards
            }
            position = Vector2.Clamp(position, new Vector2(bounds.Width/2f, bounds.Height / 2f), new Vector2(Pong.windowSize.X - bounds.Width/2f, Pong.windowSize.Y - bounds.Height / 2f)); //Clamps the position so that the object stays inside the screen
            
        }

    }
}
