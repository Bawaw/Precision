using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    class PowerUps:Actor
    {
        const int TIME_BAR__WIDTH = 400;
        const int TIME_BAR__HEIGHT= 10;
        private Bar timeBar;

        private float activeTime;
        private float activeTimeRemaining;
        private float respondTime;
        private float respondTimeRemaining;

        public float ActiveTime
        {
            get { return activeTime; }
            set { activeTime = value; }
        }

        public float RespondTime
        {
            get { return respondTime; }

            set 
            {
                respondTime = value;
                respondTimeRemaining = value;
            }
        }

        public PowerUps( Texture2D texture, Color timeBarColor) :base(texture)
        {
            this.timeBar = new Bar(TIME_BAR__WIDTH, TIME_BAR__HEIGHT, timeBarColor);
            this.timeBar.Position = new Vector2((Game1.SCREEN_WIDTH / 2) - (timeBar.Width/2), TIME_BAR__HEIGHT);
            this.timeBar.Percent = 0;
            this.Position = -Origin;
            
        }

        public override void update(GameTime gameTime)
        {
            if (this.respondTimeRemaining > 0)
            {
                respondTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (this.respondTimeRemaining <= 0)
                {
                    this.Position = Game1.GetRandomScreenPosition(this.Radius);
                }
            }
            if (this.activeTimeRemaining > 0)
            {
                activeTimeRemaining -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                this.timeBar.Percent = activeTimeRemaining / activeTime;
                if (this.activeTimeRemaining <= 0f)
                {
                    this.respondTimeRemaining = this.respondTime;
                    this.deActivate();
                    timeBar.Percent = 0;
                }
            }            
                
            base.update(gameTime);
        }

        public void pickUp()
        {
            this.Position = -this.Origin;
            this.activeTimeRemaining = this.activeTime;
            this.activate();
        }

        protected virtual void activate()
        { 
        
        }

        protected virtual void deActivate()
        { 
        
        }
        
    }
}
