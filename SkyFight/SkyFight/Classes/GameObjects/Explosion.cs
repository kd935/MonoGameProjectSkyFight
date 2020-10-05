using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace SkyFight
{
    public class Explosion : Sprite, IAnimatable
    {
        public Explosion(float X, float Y, string texturePath, ContentManager contentManager, float Scale) : base(X, Y, texturePath, contentManager, Scale)
        {
            Texture = TextureList[0];
        }
        public static List<Texture2D> TextureList { set; get; }
        public int CurrentTextureFrame { get; set; } = 0;
        public bool IsFinished { get; set; } = false;
        public override void Update()
        {
            CurrentTextureFrame += 1;
            if(CurrentTextureFrame < TextureList.Count)
            {
                Texture = TextureList[CurrentTextureFrame];
            }
            else
            {
                IsFinished = true;
            }
        }
    }
}
