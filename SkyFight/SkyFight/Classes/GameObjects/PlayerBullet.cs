using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SkyFight
{
    class PlayerBullet : Bullet
    {
        public PlayerBullet(float X, float Y, string texturePath, ContentManager contentManager, float Scale, float Angle, float bulletSpeed) : base(X, Y, texturePath, contentManager, Scale, Angle, bulletSpeed)
        {
            this.OriginXY = new Vector2(0, this.OriginalHeight);
            this.HitBox.spriteType = SpriteType.LeftSidedOrigin;
        }
        protected override Vector2 GetUpperLeftCorner()
        {
            float upperLeftX = (float)(X + Math.Cos(MathHelper.PiOver2 + MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight);
            float upperLeftY = (float)(Y - Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight);
            return new Vector2(upperLeftX, upperLeftY);
        }
    }
}
