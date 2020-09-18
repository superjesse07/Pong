using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace Pong2
{
    public class Ball : GameObject
    {
        // aanpassing
        public Vector2 direction; // the direction of the ball
        public float speed; // the speed of the ball
        const float maxSpeed = 650; // after years of experimentation this seemed to be a nice value
        const float speedGrowth = 30;
        Game1 game;

        public Ball(Game1 game,Texture2D texture, Vector2 position, Vector2 direction, float speed) : base(texture, position)
        {
            this.game = game; // this is the game
            this.speed = speed;
            this.direction = direction;
            this.direction.Normalize();
        }
        public override void Update(float deltaTime)
        {
            position += direction * speed *  deltaTime;
            float heightOffset = texture.Height / 2f;
            if (position.Y > Game1.windowSize.Y-heightOffset || position.Y < heightOffset)
            {
                direction *= new Vector2(1, -1);
            }
            if (position.X > Game1.windowSize.X)
            {
                game.PointScored(true);
            }
            if(position.X < 0)
            {
                game.PointScored(false);
            }
        }

        public void OnCollide(GameObject collider)
        {
            
            direction *= new Vector2(-1, 0);
            direction += new Vector2(0, 3 * (float)Math.Sin(.5 * Math.PI * ((position.Y - collider.position.Y) / (texture.Height + collider.texture.Height) / 2.0)));
            direction.Normalize();
            speed += speedGrowth * (1 - speed /maxSpeed);
        }
    }
}
