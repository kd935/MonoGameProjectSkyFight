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
    public class Fire : Sprite, IAnimatable
    {
        public Fire(float X, float Y, string texturePath, ContentManager contentManager, float Scale, Biplane biplane) : base(X, Y, texturePath, contentManager, Scale)
        {
            Texture = TextureList[0];
            this.biplane = biplane;
            OriginXY = new Vector2(OriginalWidth / 2, OriginalHeight / 2);
        }
        public Biplane biplane { get; set; }
        public static List<Texture2D> TextureList { set; get; }
        public int CurrentTextureFrame { get; set; } = 0;
        public bool IsFinished { get; set; } = false;
        public float FireSpriteAngleOffset { get; set; } = 90;
        public override void Update()
        {
            CurrentTextureFrame += 1;
            if (CurrentTextureFrame < TextureList.Count)
            {
                Texture = TextureList[CurrentTextureFrame];
            }
            else
            {
                CurrentTextureFrame = 0;
            }

            Angle = biplane.Angle + FireSpriteAngleOffset;
            UpdatePosition();
        }
        protected override void UpdatePosition()
        {
            X = biplane.Center.X;
            Y = biplane.Center.Y;
        }
    }
}
