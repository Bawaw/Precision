using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace Precision
{
    class Actor
    {
        #region fields and props
        public static List<Actor> actors;

        private Vector2 position;
        protected Texture2D texture;
        protected Color tint;

        const float FLICKER_FREQUENCY = 15f;
        private bool visible;
        private bool flicker;
        double nextFlickerUpdate;

        private float scale;
        private float scaleTime;
        private float scalePerSecond;

        private static float timeScale = 1f;

        public static float TimeScale
        {
            get { return timeScale; }
            set { timeScale = value; }
        }

        public Vector2 Position 
        {
            get { return this.position; }
            set { position = value; } 
        }

        public Vector2 Origin 
        {
            get {return new Vector2(this.texture.Width/2 , this.texture.Height/2);}
        }

        public int Radius
        {
            get { return this.texture.Width / 2; }
        }

        public bool Flicker 
        {
            set 
            {
                this.flicker = value;
                this.visible = !value;
            }
        }
        public float Scale
        {
            get
            {
                return this.scale;
            }
             set
            {
                this.scale = value;
            }
        }

        public bool IsScaling
        {
            get
            {
                { return (this.scaleTime > 0f); }
            }
        }

        #endregion

        static Actor()
        {
            actors = new List<Actor>();
        }

        public Actor(Texture2D texture)
        {
            actors.Add(this);
            this.texture = texture;
            this.tint = Color.White;
            this.visible = true;
            this.scale = 1f;
        }

        public virtual void update(GameTime gameTime)
        {
            if (this.flicker)
            {
                if (gameTime.TotalGameTime.TotalSeconds > nextFlickerUpdate)
                {
                    this.visible = !this.visible;
                    this.nextFlickerUpdate = gameTime.TotalGameTime.TotalSeconds + 1 / FLICKER_FREQUENCY;
                }
            }

            if (this.IsScaling)
            {
                this.scale += this.scalePerSecond * (float)gameTime.ElapsedGameTime.TotalSeconds *timeScale;
                this.scaleTime -= (float)gameTime.ElapsedGameTime.TotalSeconds * timeScale;
            }
            
        }

        public static void drawActors(SpriteBatch spriteBatch)
        {
            foreach (Actor actor in actors)
            {
                actor.draw(spriteBatch);
            }
        }

        public void draw(SpriteBatch spriteBatch)
        { 
            if (this.visible)
                spriteBatch.Draw(this.texture,this.position,null, this.tint,0f,this.Origin,this.scale,SpriteEffects.None,0f);
        }

        public static bool collision(Actor actor1, Actor actor2)
        {
            float distance = Vector2.Distance(actor1.position, actor2.position);
            return (distance < (actor1.Radius + actor2.Radius));
        }

        public bool checkCollisionWithAny()
        {
            foreach (Actor actor in actors)
            {
                if (actor != this)
                { 
                    if(collision(this,actor))
                        return true;
                }
            }
            return false;
        }

        public void StartScaling(float target, float time)
        {
            if (this.IsScaling)
                return;

            this.scaleTime = time;

            this.scalePerSecond = (target - this.scale) / time;
        }
    }
}
