using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    enum barAlignment
    {
        left,
        center,
        right
    }

    class Bar
    {
        private Vector2 position;
        private barAlignment alignment;
        private int width;
        private int height;
        private Color color;
        private float percent;

        public static Texture2D texture;
        public static List<Bar> bars;

        #region props
        public Vector2 Position 
        {
            get { return this.position; }
            set {this.position = value;}
        }
        public barAlignment Alignment
        {
            get { return this.alignment; }
            set { this.alignment = value; }
        }
        public int Width
        {
            get { return this.width; }
            set { this.width = value; }
        }
        public int Height
        {
            get { return this.height; }
            set { this.height = value; }
        }
        public float Percent
        {
            get { return this.percent; }
            set { this.percent = value; }
        }

        #endregion

        static Bar()
        { 
            bars = new List<Bar>();
        }

        public Bar(int width, int height, Color color)
        {
            this.width = width;
            this.height = height;
            this.color = color;
            this.percent = 1f;
            this.alignment = barAlignment.left;

            bars.Add(this);
        }

        public static void DrawBars (SpriteBatch spriteBatch)
        {
            for (int i = 0; i < bars.Count; i++)
            {
                bars[i].Draw(spriteBatch);
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2((float)this.alignment, 0f);

            Rectangle barRect = new Rectangle((int)this.position.X, (int)this.position.Y, (int)(this.Width * this.percent), this.height);
            spriteBatch.Draw(Bar.texture, barRect, null, this.color, 0f, origin, SpriteEffects.None, 0f);
        }
    }
}
