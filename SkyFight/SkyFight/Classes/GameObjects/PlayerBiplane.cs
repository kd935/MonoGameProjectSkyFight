using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Runtime.InteropServices;
using SkyFight;

namespace SkyFight
{
    class PlayerBiplane : Biplane
    {
        public PlayerBiplane(float X, float Y, string texturePath, ContentManager contentManager, float Scale) : base(X, Y, texturePath, contentManager, Scale)
        {
            this.OriginXY = new Vector2(0, this.OriginalHeight);
            HitBox.spriteType = SpriteType.LeftSidedOrigin;
            //HitBox.Origin = new Vector2(0, (int)this.ScaledHeight);
            //HitBox.Origin = new Vector2((int)ScaledWidth / 2, (int)ScaledHeight / 2);
        }
        public override void Shoot()
        {
            if (FrameCounter >= 50)
            {
                Vector2 centralFrontPointOfThePlane = new Vector2();
                centralFrontPointOfThePlane.X = X - (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) + this.HalfScaledHeight * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - MathHelper.PiOver2));
                centralFrontPointOfThePlane.Y = Y + (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.PiOver2 - MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) + this.HalfScaledHeight * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
                MainGame.CreatePlayerBulleT(centralFrontPointOfThePlane, Angle + StandardAngleOfSprite / 4.5f);
                FrameCounter = 0;
            }
        }
        protected override Vector2 GetPlaneAxisProjections()
        {
            float xProjection = (float)(this.ScaledWidth * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) + this.HalfScaledHeight * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - MathHelper.PiOver2));
            float yProjection = (float)(this.ScaledWidth * Math.Cos(MathHelper.PiOver2 - MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) + this.HalfScaledHeight * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            return new Vector2(xProjection, yProjection);
        }
        protected override Vector2 GetUpperLeftCorner()
        {
            float upperLeftX = (float)(X + Math.Cos(MathHelper.PiOver2 + MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight);
            float upperLeftY = (float)(Y - Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight);
            return new Vector2(upperLeftX, upperLeftY);
        }
        protected override Vector2 GetCenter()
        {
            Vector2 CenterCoord = new Vector2();
            CenterCoord.X = (int)(X + Math.Cos(MathHelper.ToRadians(Angle + StandardAngleOfSprite)) * Distance);
            CenterCoord.Y = (int)(Y - Math.Sin(MathHelper.ToRadians(Angle + StandardAngleOfSprite)) * Distance);
            return CenterCoord;
        }
        public override void DealDamage()
        {
            HealthPoints = HealthPoints - MainGame.ONE_HIT_DAMAGE;

            if (HealthPoints == 2 * MainGame.ONE_HIT_DAMAGE)
            {
                MainGame.CreateSm(this);
            }
            else if (HealthPoints == MainGame.ONE_HIT_DAMAGE)
            {
                MainGame.CreateFi(this);
                MainGame.DestroySm(this);
            }
            else if (HealthPoints == 0)
            {
                MainGame.DestroyFi(this);
                MainGame.CreateExp(Center.X, Center.Y);
                MainGame.SpawnPlayer();
                MainGame.IncreaseEnemySc();
            }
        }
    }
}
