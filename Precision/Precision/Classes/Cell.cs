using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    enum CellState
    {
        Healthy,
        Attacked,
        Saved,
        Dead
    } 

    class Cell : Actor
    {
        #region props

        const float CELL_SAVE_TIME = 0.5f;
        const int CELL_REPLACING_RETRYCOUNT = 100;

        private static int cellCount;
        private CellState state;

        private Color healthyColor;
        private Color deadColor;

        private float lifePercent;
        private float deathTime;

        //lifebars
        const int LIFEBAR_HEIGHT = 8;
        private Color lifebarColor = Color.Red;
        private Bar lifeBar;

        public CellState State
        {
            get { return this.state; }
        }

        public static int CellCount
        {
            get { return cellCount; }
        }
        #endregion

        public Cell(Texture2D texture, Color healthyColor, Color deadColor)
            : base(texture)  
        {
            this.healthyColor = healthyColor;
            this.deadColor = deadColor;
            this.lifePercent = 1f;
            this.state = CellState.Healthy;
            this.tint = healthyColor;
            cellCount++;
        }

        public override void update(GameTime gameTime)
        {
            if (this.state == CellState.Attacked)
            {
                this.lifePercent -= (float)gameTime.ElapsedGameTime.TotalSeconds / this.deathTime * TimeScale;
                this.lifeBar.Percent = this.lifePercent;

                if (lifePercent <= 0)
                {
                    this.state = CellState.Dead;
                    cellCount--;
                    Game1.addEnemyAt(this.Position);
                    this.lifeBar = null;

                    Game1.currentScoreValue -= Game1.cellDeathPenalty;
                }

                this.tint = Lerb(deadColor, healthyColor, lifePercent);
            }

            if (this.state == CellState.Saved && !this.IsScaling)
            {
                actors.Remove(this);
                
                cellCount--;
            }


            base.update(gameTime);
        }

        public void SetAttacked(float DeathTime)
        {
            if (this.state == CellState.Healthy)
            {
                this.deathTime = DeathTime;
                this.state = CellState.Attacked;

                this.lifeBar = new Bar(this.texture.Width, LIFEBAR_HEIGHT, lifebarColor);
                this.lifeBar.Position = new Vector2(this.Position.X - this.Origin.X, this.Position.Y + this.Origin.Y + 2f);

            }
        }

        public void save()
        {
            if (this.state != CellState.Saved)
            {
                this.StartScaling(0f, CELL_SAVE_TIME);
                this.state = CellState.Saved;

                Bar.bars.Remove(this.lifeBar);
                this.lifeBar = null;
            }
        }

        public Color Lerb(Color Color1, Color Color2, float amount)
        {
            byte r = (byte)MathHelper.Lerp(Color1.R,Color2.R,amount);
            byte g = (byte)MathHelper.Lerp(Color1.G, Color2.G, amount);
            byte b = (byte)MathHelper.Lerp(Color1.B, Color2.B, amount);
            byte a = (byte)MathHelper.Lerp(Color1.A, Color2.A, amount);

            return new Color(r, g, b, a);
        }

        public static void addCells(int num,Texture2D texture, Color healthyColor, Color deadColor ) 
        {
            for (int i = 0; i < num; i++)
            {
                Cell cell = new Cell(texture, healthyColor, deadColor);

                int counter = 0;
                do
                {
                    cell.Position = Game1.GetRandomScreenPosition(cell.Radius);
                    counter++;
                    
                } while (cell.checkCollisionWithAny() && counter < CELL_REPLACING_RETRYCOUNT);
            }
        }
        public static void resetCellCount()
        {
            cellCount = 0;
        }
    }
}
