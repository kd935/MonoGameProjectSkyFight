using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkyFight
{
    class Background : Sprite
    {
        public Background(float X, float Y, string texturePath, ContentManager contentManager, float Scale) : base(X, Y, texturePath, contentManager, Scale)
        {
            this.OriginXY = Vector2.Zero;
            this.HitBox.Origin = Vector2.Zero;
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.X, this.Y);
            spriteBatch.Draw(Texture, new Rectangle((int)spritePosition.X, (int)spritePosition.Y, MainGame.SCREEN_WIDTH, MainGame.SCREEN_HEIGHT), null, Color.White, -1 * MathHelper.ToRadians(this.Angle), OriginXY, SpriteEffects.None, 0f);
        }
    }
}
