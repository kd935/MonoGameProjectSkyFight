using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkyFight
{
    public class Ground : Sprite
    {
        public Ground(float X, float Y, string texturePath, ContentManager contentManager, float Scale) : base(X, Y, texturePath, contentManager, Scale)
        {
            this.Y = MainGame.SCREEN_HEIGHT - this.ScaledHeight / 2;
            this.HitBox.CollisionRectangle.X = 0;
            this.HitBox.CollisionRectangle.Y = (int)(MainGame.SCREEN_HEIGHT - this.ScaledHeight);
            HitBox.spriteType = SpriteType.LeftSidedOrigin;
            //this.HitBox.CollisionRectangle.X = (int)this.X;
            //this.HitBox.CollisionRectangle.Y = (int)this.Y;
            //this.HitBox.Origin = new Vector2((int)ScaledWidth / 2, (int)ScaledHeight / 2);
        }
    }
}
