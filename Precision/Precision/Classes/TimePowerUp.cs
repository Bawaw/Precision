using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    class TimePowerUp : PowerUps
    {
        private float slowDownPercent;

        public float SlowDownPercent
        {
            get { return this.slowDownPercent; }
            set { slowDownPercent = value; }
        }

        public TimePowerUp(Texture2D texture, Color timeBarColor) : base(texture, timeBarColor){}

        protected override void activate()
        {
            Actor.TimeScale = slowDownPercent;
        }
        protected override void deActivate()
        {
            Actor.TimeScale = 1f;
        }
    }
}
