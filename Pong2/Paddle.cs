using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong2
{
    public class Paddle : GameObject
    {
        Keys up, down;
        float speed;

        public Paddle(Texture2D texture, Vector2 position,float speed,Keys up, Keys down) : base(texture,position)
        {
            this.speed = speed;
            this.up = up;
            this.down = down;
        }

        public override void Update(float gameTime)
        {
            float heightOffset = texture.Height / 2f;
            KeyboardState state = Keyboard.GetState();
            if(state.IsKeyDown(up) && position.Y > heightOffset)
            {
                position -= new Vector2(0, speed * gameTime);
            }
            if(state.IsKeyDown(down) && position.Y < Game1.windowSize.Y - heightOffset)
            {
                position += new Vector2(0, speed * gameTime);
            }
        }

    }
}
