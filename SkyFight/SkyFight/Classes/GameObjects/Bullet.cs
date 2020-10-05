using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace SkyFight
{
    public class Bullet : Sprite
    {
        public bool isAlive = true;
        public Bullet(float X, float Y, string texturePath, ContentManager contentManager, float Scale, float Angle, float bulletSpeed) : base(X, Y, texturePath, contentManager, Scale)
        {
            this.Angle = Angle;
            this.Speed = bulletSpeed;
        }
        public override void Update()
        {
            UpdatePosition();
            UpdateHitBox();

            if (!IsInBounds())
            {
                isAlive = false;
            }
        }
        protected override void UpdatePosition()
        {
            X += (float)(Speed * Math.Cos(MathHelper.ToRadians(Angle)));
            Y -= (float)(Speed * Math.Sin(MathHelper.ToRadians(Angle)));
        }
        protected override void UpdateHitBox()
        {
            Vector2 upperLeft = GetUpperLeftCorner();
            HitBox.CollisionRectangle.X = (int)upperLeft.X;
            HitBox.CollisionRectangle.Y = (int)upperLeft.Y;
            HitBox.Rotation = MathHelper.WrapAngle(MathHelper.ToRadians(Angle));
        }
        protected virtual bool IsInBounds()
        {
            if (this.X < 0)
            {
                return false;
            }
            if (this.X > MainGame.SCREEN_WIDTH)
            {
                return false;
            }
            if (this.Y < 0)
            {
                return false;
            }
            if (this.Y > MainGame.SCREEN_HEIGHT)
            {
                return false;
            }
            return true;
        }
        protected virtual Vector2 GetUpperLeftCorner()
        {
            throw new NotImplementedException();
        }
    }
}
