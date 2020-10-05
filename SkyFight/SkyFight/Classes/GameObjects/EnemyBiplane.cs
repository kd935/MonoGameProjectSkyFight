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
    class EnemyBiplane : Biplane
    {
        private GameModes gameMode;
        private float? requiredAngleAI = null;
        private Rotations rotationTypeAI;
        protected long FrameCounterAI { get; set; }
        public EnemyBiplane(float X, float Y, string texturePath, ContentManager contentManager, float Scale, GameModes gameMode) : base(X, Y, texturePath, contentManager, Scale)
        {
            this.OriginXY = new Vector2(0, 0);
            this.Angle = 180;
            HitBox.spriteType = SpriteType.RightSidedOrigin;
            this.gameMode = gameMode;
            if (gameMode == GameModes.PlayerVersusAI) { Speed = MainGame.MIN_PLANE_SPEED; } 
        }
        public override void Shoot()
        {
            if (FrameCounter >= 50)
            {
                Vector2 centralFrontPointOfThePlane = new Vector2();
                centralFrontPointOfThePlane.X = X - (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - MathHelper.PiOver2));
                centralFrontPointOfThePlane.Y = Y + (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.PiOver2 - MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
                MainGame.CreateEnemyBulleT(centralFrontPointOfThePlane, Angle - StandardAngleOfSprite / 4.5f);
                FrameCounter = 0;
            }
        }
        public override void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.X, this.Y);
            spriteBatch.Draw(Texture, spritePosition, null, Color.White, -1 * MathHelper.ToRadians(this.Angle), OriginXY, Scale, SpriteEffects.FlipVertically, 0f);
        }
        protected override Vector2 GetPlaneAxisProjections()
        {
            float xProjection = (float)(this.ScaledWidth * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - MathHelper.PiOver2));
            float yProjection = (float)(this.ScaledWidth * Math.Cos(MathHelper.PiOver2 - MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            return new Vector2(xProjection, yProjection);
        }
        protected override Vector2 GetUpperLeftCorner()
        {
            float upperLeftX = (float)(X - Math.Cos(MathHelper.PiOver2 + MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight + ScaledWidth * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            float upperLeftY = (float)(Y + Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) * ScaledHeight - ScaledWidth * Math.Cos(MathHelper.PiOver2 - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
            return new Vector2(upperLeftX, upperLeftY);
        }
        protected override Vector2 GetCenter()
        {
            Vector2 CenterCoord = new Vector2();
            CenterCoord.X = (int)(X + Math.Cos(MathHelper.ToRadians(Angle - StandardAngleOfSprite)) * Distance);
            CenterCoord.Y = (int)(Y - Math.Sin(MathHelper.ToRadians(Angle - StandardAngleOfSprite)) * Distance);
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
                MainGame.SpawnEnemy();
                MainGame.IncreasePlayerSc();
            }
        }
        protected override void UpdatePosition()
        {
            switch(gameMode)
            {
                case GameModes.PlayerVersusPlayer:
                    base.UpdatePosition();
                    break;
                case GameModes.PlayerVersusAI:
                    UpdateAI();
                    base.UpdatePosition();
                    break;
            }
        }
        protected void UpdateAI()
        {
            UpdateAngleAI();
            ShootAI();
        }

        private void UpdateAngleAI()
        {
            if (FrameCounterAI % 100 == 0)
            {
                Vector2 playerCoords = MainGame.FGetPlayerCenterCoords();
                float clockwiseAngleRad = 0;
                float counterclockwiseAngleRad = 0;
                requiredAngleAI = -(float)Math.Atan2(playerCoords.Y - Center.Y, playerCoords.X - Center.X);
                float angleBetweenPlaneAndRequiredAnglesRad = MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - (float)requiredAngleAI;

                if (angleBetweenPlaneAndRequiredAnglesRad < 0)
                {
                    counterclockwiseAngleRad = angleBetweenPlaneAndRequiredAnglesRad;
                    clockwiseAngleRad = MathHelper.TwoPi - Math.Abs(counterclockwiseAngleRad);
                }
                else
                {
                    clockwiseAngleRad = angleBetweenPlaneAndRequiredAnglesRad;
                    counterclockwiseAngleRad = MathHelper.TwoPi - Math.Abs(clockwiseAngleRad);
                }

                if (Math.Abs(clockwiseAngleRad) > Math.Abs(counterclockwiseAngleRad))
                {
                    rotationTypeAI = Rotations.Counterclockwise;
                }
                else
                {
                    rotationTypeAI = Rotations.Clockwise;
                }
            }
            if (requiredAngleAI != null)
            {
                var normalizedAngle = MathHelper.ToDegrees(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)));
                var requiredAngleAIDeg = (int)MathHelper.ToDegrees((float)requiredAngleAI);
                if (!(normalizedAngle <= requiredAngleAIDeg + 1 && normalizedAngle >= requiredAngleAIDeg - 1))
                {
                    if (rotationTypeAI == Rotations.Clockwise) { Angle -= MainGame.PLANE_ROTATION_PER_TICK; }
                    if (rotationTypeAI == Rotations.Counterclockwise) { Angle += MainGame.PLANE_ROTATION_PER_TICK; }
                }
                else
                {
                    requiredAngleAI = null;
                }
            }
        }
        private void ShootAI()
        {
            if (FrameCounter >= 50)
            {
                Random rand = new Random();
                if (rand.Next(0, 10000) >= 9700) {
                    Vector2 centralFrontPointOfThePlane = new Vector2();
                    centralFrontPointOfThePlane.X = X - (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.WrapAngle(MathHelper.ToRadians(Angle)) - MathHelper.PiOver2));
                    centralFrontPointOfThePlane.Y = Y + (float)((this.ScaledWidth + MainGame.BULLET_STARTING_POSITION_X_OFFSET) * Math.Cos(MathHelper.PiOver2 - MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))) - this.HalfScaledHeight * Math.Cos(MathHelper.Pi - MathHelper.WrapAngle(MathHelper.ToRadians(Angle))));
                    MainGame.CreateEnemyBulleT(centralFrontPointOfThePlane, Angle - StandardAngleOfSprite / 4.5f);
                    FrameCounter = 0;
                }
            }
        }

        protected override void UpdateFrameCounter()
        {
            FrameCounter += 1;
            FrameCounterAI += 1;
            if (FrameCounterAI > 1000000) { FrameCounterAI = 0; }
        }
    }
}
