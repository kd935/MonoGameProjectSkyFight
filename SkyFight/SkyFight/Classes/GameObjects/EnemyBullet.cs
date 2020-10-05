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
    class EnemyBullet : Bullet
    {
        public EnemyBullet(float X, float Y, string texturePath, ContentManager contentManager, float Scale, float Angle, float bulletSpeed) : base(X, Y, texturePath, contentManager, Scale, Angle, bulletSpeed)
        {
            this.OriginXY = new Vector2(0, 0);
            this.HitBox.spriteType = SpriteType.RightSidedOrigin;
        }
        protected override Vector2 GetUpperLeftCorner()
        {
            float upperLeftX = (float)(X - Math.Cos(MathHelper.PiOver2 + MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight + ScaledWidth * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            float upperLeftY = (float)(Y + Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight - ScaledWidth * Math.Cos(MathHelper.PiOver2 - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            return new Vector2(upperLeftX, upperLeftY);
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.X, this.Y);
            spriteBatch.Draw(Texture, spritePosition, null, Color.White, -1 * MathHelper.ToRadians(this.Angle), OriginXY, Scale, SpriteEffects.FlipVertically, 0f);
        }
    }
}
