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
    public class TextDrawer
    {
        public TextDrawer(SpriteFont spriteFont, SpriteBatch spriteBatch)
        {
            SpriteFont = spriteFont;
            SpriteBatch = spriteBatch;
        }

        public SpriteFont SpriteFont { get; set; }
        public Color DrawingColor { get; set; }
        public SpriteBatch SpriteBatch { get; set; }
        public void DrawText(Vector2 position, Color color, string textToDraw)
        {
            SpriteBatch.DrawString(SpriteFont, textToDraw, position, color);
        }
    }
}
