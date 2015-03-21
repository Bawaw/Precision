using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    class Player : Actor
    {
        private float invincibilityTime;

        public bool IsInvincible
        { 
            get { return invincibilityTime > 0f; } 
        }

        public Player(Texture2D texture) : base(texture) { }

        public override void update(GameTime gameTime)
        {
            if (this.IsInvincible)
            {
                this.invincibilityTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (!this.IsInvincible)
                    this.Flicker = false;
            }

            base.update(gameTime);
        }

        public void reset(float invincibilityTime)
        {
            this.Position = new Vector2(Game1.SCREEN_WIDTH/2, Game1.SCREEN_HEIGHT/2);
            this.invincibilityTime = invincibilityTime;
            this.Flicker = true;
        }
    }
}
