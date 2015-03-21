using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Precision
{
    class Enemy : Actor
    {
        const float enemyScaleTime = 0.5f;

        private float baseSpeed;
        private float speedVariation;

        private Vector2 pointA, pointB;
        private float ammount;
        private float moveTime;

        public bool isHarmful { get { return !this.IsScaling; } }

        public Enemy(Texture2D texture, float baseSpeed,float speedVariation) : base (texture) 
        {
            this.baseSpeed = baseSpeed;
            this.speedVariation = speedVariation;
            this.Scale = 0f;
            StartScaling(1f,enemyScaleTime);
        }

        public override void update(GameTime gameTime)
        {
            if (moveTime <= 0)
                this.setRandomMove();

            if (this.isHarmful)
            {
                ammount += (float)gameTime.ElapsedGameTime.TotalSeconds / moveTime * TimeScale;
                this.Position = Vector2.SmoothStep(this.pointA, this.pointB, this.ammount);
            }

            if (this.ammount >= 1f)
                this.setRandomMove();

            base.update(gameTime);
        }

        private void setRandomMove()
        {
            this.pointA = this.Position;
            this.pointB = Game1.GetRandomScreenPosition(this.Radius);

            this.moveTime = baseSpeed + Game1.Range(-speedVariation,speedVariation);
            this.ammount = 0f;

        }

        public static void AddEnemies (int numEnemies,Texture2D texture, float baseSpeed,float speedVariation )
        {
            for (int i = 0; i < numEnemies; i++)
            {
                Enemy enemy = new Enemy(texture, baseSpeed, speedVariation);
                enemy.Position = Game1.GetRandomScreenPosition(enemy.Radius);
            }
        }
    }
}
