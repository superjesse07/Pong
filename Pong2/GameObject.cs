using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Pong2
{
    public abstract class GameObject
    {
        protected Texture2D texture;
        public Vector2 position;

        public GameObject(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public abstract void Update(float deltaTime);

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 textureCenter = new Vector2(texture.Width, texture.Height) / 2f;
            spriteBatch.Draw(texture, position, null, Color.White, 0, textureCenter, 1, SpriteEffects.None, 0) ;
        }

        public Rectangle GetBounds()
        {
            Rectangle bounds = texture.Bounds;
            bounds.Offset(position - new Vector2(texture.Width,texture.Height)/2f);
            return bounds;
        }
    }
}
