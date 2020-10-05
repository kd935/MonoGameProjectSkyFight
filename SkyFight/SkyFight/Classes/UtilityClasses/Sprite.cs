using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SkyFight
{
    public class Sprite
    {
        public Sprite(float X, float Y, string texturePath, ContentManager contentManager, float Scale)
        {
            this.X = X;
            this.Y = Y;
            this.Scale = Scale;
            this.Texture = contentManager.Load<Texture2D>(texturePath);
            this.ScaledWidth = this.Scale * Texture.Width;
            this.ScaledHeight = this.Scale * Texture.Height;
            this.OriginalWidth = Texture.Width;
            this.OriginalHeight = Texture.Height;
            this.OriginXY = new Vector2(this.OriginalWidth / 2, this.OriginalHeight / 2);
            this.HalfScaledWidth = this.ScaledWidth / 2;
            this.HalfScaledHeight = this.ScaledHeight / 2;
            this.Angle = 0;
            this.HitBox = new RotatedRectangle(new Rectangle(0, 0, (int)ScaledWidth, (int)ScaledHeight), Vector2.Zero, Angle);
        }

        public RotatedRectangle HitBox;
        
        public Texture2D Texture
        {
            get;
            set;
        }
        public float X
        {
            get;
            set;
        }

        public float Y
        {
            get;
            set;
        }
        public float ScaledWidth
        {
            get;
            set;
        }
        public float ScaledHeight
        {
            get;
            set;
        }
        public float OriginalWidth
        {
            get;
            set;
        }
        public float OriginalHeight
        {
            get;
            set;
        }
        public float Angle
        {
            get;
            set;
        }

        public float Speed
        {
            get;
            set;
        }
        public float RotationPerTick
        {
            get;
            set;
        }

        public float Scale
        {
            get;
            set;
        }
        protected Vector2 OriginXY
        {
            get;
            set;
        }
        protected float HalfScaledWidth
        {
            get;
            set;
        }
        protected float HalfScaledHeight
        {
            get;
            set;
        }
        protected virtual void UpdateHitBox()
        {
            throw new NotImplementedException();
        }
        protected virtual void UpdatePosition()
        {
            throw new NotImplementedException();
        }
        public virtual void Update()
        {
            throw new NotImplementedException();
        }
        public virtual void Draw(SpriteBatch spriteBatch)
        {
            Vector2 spritePosition = new Vector2(this.X, this.Y);
            spriteBatch.Draw(Texture, spritePosition, null, Color.White, -1 * MathHelper.ToRadians(this.Angle), OriginXY, Scale, SpriteEffects.None, 0f);
        }
    }
}
