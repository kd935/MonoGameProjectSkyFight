using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using SkyFight;

namespace SkyFight
{
    public class Biplane : Sprite
    {
        public Biplane(float X, float Y, string texturePath, ContentManager contentManager, float Scale) : base(X, Y, texturePath, contentManager, Scale)
        {
            Distance = GetDistance();
            StandardAngleOfSprite = MathHelper.ToDegrees(GetStandardAngleOfSprite());
        }
        protected float HealthPoints { get; set; } = MainGame.BIPLANE_HEALTH_POINTS;
        protected long FrameCounter { get; set; }
        protected float Distance { get; set; }
        protected float StandardAngleOfSprite { get; set; }
        
        public Vector2 Center = Vector2.Zero;
        protected float GetRadius()
        {
            return (float)Math.Pow(this.Speed, 2) / MainGame.RADIUS_DIVIDER;
        }
        protected float GetDistance()
        {
            double a = Math.Pow(Convert.ToDouble(this.HalfScaledWidth), 2);
            double b = Math.Pow(Convert.ToDouble(this.HalfScaledHeight), 2);
            return (float)Math.Sqrt(a + b);
        }
        protected float GetStandardAngleOfSprite()
        {
            return (float)Math.Asin(this.HalfScaledHeight / this.Distance);
        }
        protected bool IsInBounds(out Vector2 coordinatesToSet, out List<Bounds> ViolatedBounds)
        {
            ViolatedBounds = new List<Bounds>();
            bool isViolated = true;
            coordinatesToSet = new Vector2();
            // A projection of a plane relatively to X axis starting from its origin to its central front x point
            Vector2 XYProjections = GetPlaneAxisProjections();
            float xProjection = XYProjections.X;
            float yProjection = XYProjections.Y;

            if ((this.X - xProjection) < 0)
            {
                ViolatedBounds.Add(Bounds.Left);
                coordinatesToSet.X = xProjection;
                isViolated = false;
            }

            if ((this.X - xProjection) > MainGame.SCREEN_WIDTH) //Math.Abs(xProjection)
            {
                ViolatedBounds.Add(Bounds.Right);
                coordinatesToSet.X = Math.Abs(xProjection);
                isViolated = false;
            }

            if ((this.Y + yProjection) < 0)
            {
                ViolatedBounds.Add(Bounds.Top);
                coordinatesToSet.Y = Math.Abs(yProjection);
                isViolated = false;
            }
            return isViolated;
        }
        protected void PlaceInBounds(Vector2 coordinatesToSet, List<Bounds> ViolatedBounds)
        {
            foreach(Bounds violatedBound in ViolatedBounds)
            {
                switch (violatedBound)
                {
                    case Bounds.Left:
                        X = (float)coordinatesToSet.X;
                        break;
                    case Bounds.Top:
                        Y = (float)coordinatesToSet.Y;
                        break;
                    case Bounds.Right:
                        X = MainGame.SCREEN_WIDTH - (float)coordinatesToSet.X;
                        break;
                }
            }
        }
        protected void RotateThePlane()
        {
            this.Angle += this.RotationPerTick;
        }
        public override void Update()
        {
            UpdateFrameCounter();
            RotateThePlane();
            UpdatePosition();
            UpdateHitBox();

            if (!IsInBounds(out Vector2 coordinatesToSet, out List<Bounds> ViolatedBounds))
            {
                PlaceInBounds(coordinatesToSet, ViolatedBounds);
            }
        }
        protected override void UpdatePosition()
        {
            float radius = GetRadius();

            if (RotationPerTick == 0)
            {
                X += (float)(Speed * Math.Cos(MathHelper.ToRadians(Angle)));
                Y -= (float)(Speed * Math.Sin(MathHelper.ToRadians(Angle)));
            }
            else
            {
                X = (float)(X + Math.Cos(MathHelper.ToRadians(Angle)) * radius);
                Y = (float)(Y - Math.Sin(MathHelper.ToRadians(Angle)) * radius);
            }

            Center = GetCenter();
        }
        protected override void UpdateHitBox()
        {
            Vector2 upperLeft = GetUpperLeftCorner();
            HitBox.CollisionRectangle.X = (int)upperLeft.X;
            HitBox.CollisionRectangle.Y = (int)upperLeft.Y;
            HitBox.Rotation = MathHelper.WrapAngle(MathHelper.ToRadians(Angle));
        }
        protected virtual void UpdateFrameCounter()
        {
            FrameCounter += 1;
        }
        public virtual void Shoot()
        {
            throw new NotImplementedException();
        }
        protected virtual Vector2 GetPlaneAxisProjections()
        {
            throw new NotImplementedException();
        }
        protected virtual Vector2 GetUpperLeftCorner()
        {
            throw new NotImplementedException();
        }
        protected virtual Vector2 GetCenter()
        {
            throw new NotImplementedException();
        }
        public virtual void DealDamage()
        {
            throw new NotImplementedException();
        }
    }
}
