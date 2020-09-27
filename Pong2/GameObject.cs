using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong2
{
    /// <summary>
    /// This is a class for all the objects in the game. For now there are only 2 types: the ball and the paddles.
    /// This class has 3 variables: a texture, a position and a bounds variable.
    /// </summary>
    public abstract class GameObject
    {
        public Texture2D texture; // the texture of the object
        public Vector2 position; // the position of the object
        public float speed; // the speed the object moves in pixels per second
        public Rectangle bounds => new Rectangle((int)position.X - texture.Width/2, (int)position.Y - texture.Height/2, texture.Width, texture.Height); // the hitbox of the object

        public GameObject(Texture2D texture, Vector2 position,float speed)
        {
            this.texture = texture;
            this.position = position;
            this.speed = speed;
        }

        /// <summary>
        /// Update function to do general changes to the gameobject per frame
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds</param>
        public abstract void Update(float deltaTime);

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 textureCenter = new Vector2(texture.Width, texture.Height) / 2f; // the center of the texture
            spriteBatch.Draw(texture, position, null, Color.White, 0, textureCenter, 1, SpriteEffects.None, 0) ; // draws the object centered to it's origin
        }
    }
}
