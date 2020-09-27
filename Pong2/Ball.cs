using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong2
{
    /// <summary>
    /// Class for all the balls in the game.
    /// This is a subclass of gameObjects
    /// The balls have a position, a texture, a bounds, a direction, a speed, a maxSpeed and a speedGrowth variable.
    /// </summary>
    public class Ball : GameObject
    {
        
        public Vector2 direction; // the direction of the ball
        const float maxSpeed = 650; // after years of experimentation this seemed to be a nice value
        const float speedGrowth = 30; // a modifier to set the growth of the ballspeed after each hit

        public Ball(Texture2D texture, Vector2 position, Vector2 direction, float speed) : base(texture, position,speed)
        {
            this.speed = speed;
            this.direction = direction;
            this.direction.Normalize(); //Normalizes the direction so it has a length of one, so that when it is timed by the speed it has a length of that speed
        }

        /// <summary>
        /// Update function to do general changes to the ball per frame
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public override void Update(float deltaTime)
        {
            position += direction * speed *  deltaTime; //moves the ball in a direction with a certain speed per second
            float heightOffset = texture.Height / 2f; // 
            if (position.Y > Pong.windowSize.Y-heightOffset || position.Y < heightOffset) //triggered when the ball touches the edge of the screen
            {
                direction *= new Vector2(1, -1); // inverts the Y direction of the ball
            }
            if (position.X > Pong.windowSize.X) // triggered when the ball touches the right side of the screen
            {
                Pong.Game.PointScored(true); // Triggers the PointScored method with the rightSide variable to true
            }
            if(position.X < 0) // triggered when the ball touches the left side of the screen
            {
                Pong.Game.PointScored(false); // Triggers the PointScored method with the rightSide variable to false
            }
        }

        /// <summary>
        /// Is called whenever the ball collides with another GameObject
        /// </summary>
        /// <param name="collider">the GameObject that the ball collides with</param>
        public void OnCollide(GameObject collider)
        {
            
            direction *= new Vector2(-1, 0); //reverses the direction the ball is headed
            direction += new Vector2(0, 3 * (float)Math.Sin(.5 * Math.PI * ((position.Y - collider.position.Y) / (texture.Height + collider.texture.Height) / 2.0))); //Changes the direction the ball is headed in the y axis based on the location it hits the paddle
            direction.Normalize(); //Normalizes the direction so that the length of the vector is 1
            speed += speedGrowth * (1 - speed /maxSpeed); //Changes the balls speed by a fraction of (1 - speed/maxSpeed) so that it never exceeds the maxspeed
        }
    }
}
