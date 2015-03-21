using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Precision
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>

   
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            Title,
            LevelChange,
            Playing,
            Died,
            Gameover
        }

        internal const int SCREEN_HEIGHT = 600;
        internal const int SCREEN_WIDTH = 800;

        private int level;
        const int BASE_NUM_CELLS = 2;
        const int BASE_NUM_ENEMIES = 1;

        private int lives;
        const int STARTING_LIVES = 3;

        private int score;
        const int INITIAL_SCORE_VAL = 10;
        internal const int cellDeathPenalty = 2;
        internal static int currentScoreValue;

        const float TIME_POWERUP_DURATION = 3f;
        const float TIME_POWERUP_RESPAWN_TIME = 1;
        private Color timeBarColor = new Color(0, 128, 255);
        const float TIME_POWER_UP_SLOWDOWN = 0.1f;

        const float PLAYERSPEED_SLOW = 5f;
        const float PLAYERSPEED_FAST = 10f;
        const float PLAYER_INVINCIBILITY_TIME = 1f;

        const float CELL_DEATH_TIME = 5f;
        private static Color cellHealthyColor = new Color(124, 207, 86);
        private static Color cellDeadColor = new Color(115,59,141);
        private double timeUntilAttack;
        private double cellAttackInterval = 20d; 

        const float ENEMY_BASESPEED = 3f;
        const float ENEMY_SPEED_VARIATION = 1f;

        SpriteFont mainFont;
        Color overlayColor = new Color(200, 0, 0, 128);
        
        
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        static Random random;
        GameState gameState;

        private Texture2D backGround;
        private Texture2D playerTexture;
        private Texture2D cellTexture;
        static private Texture2D enemyTexture;
        Texture2D overlayTexture;
        Texture2D titleTexture;
        Texture2D levelChangeTexture;
        static private KeyboardState keyboardstate;
        private Texture2D timeTexture;

        private Player player;

        private int levelCellCount 
        {
            get
            {
                return BASE_NUM_CELLS * level; 
            }
        }

        private int levelEnemyCount
        {
            get
            {
                return BASE_NUM_ENEMIES * level;
            }
        }

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            spriteBatch = new SpriteBatch(graphics.GraphicsDevice);

            random = new Random();
            this.gameState = GameState.Title;

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            backGround = Content.Load<Texture2D>(@"Textures\BackGround");
            playerTexture = Content.Load<Texture2D>(@"Textures\probe");
            enemyTexture = Content.Load<Texture2D>(@"Textures\AntiBody");
            cellTexture = Content.Load<Texture2D>(@"Textures\Cell");
            Bar.texture = Content.Load<Texture2D>(@"Textures\22bar");
            titleTexture = Content.Load<Texture2D>(@"Textures\titleScreen");
            levelChangeTexture = Content.Load<Texture2D>(@"Textures\LevelUpScreen");
            overlayTexture = Content.Load<Texture2D>(@"Textures\22bar");
            timeTexture = Content.Load<Texture2D>(@"Textures\StopWatch");
            mainFont = Content.Load<SpriteFont>(@"Textures\Arial");
                        
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            keyboardstate = Keyboard.GetState();

            // Allows the game to exit
            if (keyboardstate.IsKeyDown(Keys.Escape))
                this.Exit();

            switch (this.gameState)
            {
                case GameState.Title:
                       
                       if (keyboardstate.IsKeyDown(Keys.Enter))
                       {
                           beginGame();
                           gameState = GameState.LevelChange;
                           for (int i = 0; i < 18000000; i++) ;
                       }
                    break;

                case GameState.LevelChange:

                        keyboardstate = Keyboard.GetState();
                        if (keyboardstate.IsKeyDown(Keys.Enter))
                        {
                            beginLevel();
                            gameState = GameState.Playing;
                        }
                    break;
                case GameState.Playing:
                    for (int i = Actor.actors.Count - 1; i >= 0; i--)
                    {
                        timeUntilAttack -= gameTime.ElapsedGameTime.TotalSeconds;

                        if (Cell.CellCount <= 0)
                        {
                            this.level++;
                            gameState = GameState.LevelChange;
                            break;
                        }

                        Actor actor = Actor.actors[i]; 
                        actor.update(gameTime);

                        if (actor is Player)
                        {
                            Vector2 direction = Vector2.Zero;
                            if (keyboardstate.IsKeyDown(Keys.Up))
                            {
                                direction.Y--;
                            }
                            if (keyboardstate.IsKeyDown(Keys.Down))
                            {
                                direction.Y++;
                            }
                            if (keyboardstate.IsKeyDown(Keys.Left))
                            {
                                direction.X--;
                            }
                            if (keyboardstate.IsKeyDown(Keys.Right))
                            {
                                direction.X++;
                            }

                            if (direction.Length() > 0.0f)
                                direction.Normalize();
                            else
                                direction = Vector2.Zero;

                            if(keyboardstate.IsKeyDown(Keys.Space))
                                actor.Position += direction* PLAYERSPEED_FAST;
       
                            else
                                actor.Position += direction * PLAYERSPEED_SLOW;

                            Vector2 playerPos = actor.Position;

                            if (playerPos.X < actor.Origin.X/2)
                                playerPos.X = actor.Origin.X/2;

                            else if (playerPos.X > SCREEN_WIDTH - actor.Origin.X/2)
                                playerPos.X = SCREEN_WIDTH - actor.Origin.X/2;
                            
                            if (playerPos.Y < actor.Origin.Y / 2)
                                playerPos.Y = actor.Origin.Y / 2;

                            else if (playerPos.Y > SCREEN_HEIGHT - actor.Origin.Y / 2)
                                playerPos.Y = SCREEN_HEIGHT - actor.Origin.Y / 2;

                            actor.Position = playerPos;
                            continue; 
                        }

                        Enemy enemy = actor as Enemy;
                        if (enemy != null)
                        { 
                            if(!player.IsInvincible && enemy.isHarmful && Actor.collision(enemy,this.player))
                            {
                                this.lives--;
                                if (this.lives > 0)
                                    this.gameState = GameState.Died;
                                else
                                    this.gameState = GameState.Gameover;
                            }
                            continue;
                        }

                        Cell cell = actor as Cell;
                        if (cell != null)
                        {
                            if (timeUntilAttack <= 0 && cell.State == CellState.Healthy)
                            {
                                cell.SetAttacked(CELL_DEATH_TIME);
                                timeUntilAttack = cellAttackInterval;
                            }

                            if (cell.State == CellState.Attacked && Actor.collision(cell,player))
                            {
                                cell.save();
                                this.score += currentScoreValue;
                            }

                            continue;
                        }
                        PowerUps powerUps = actor as PowerUps;
                        if (powerUps != null)
                        {
                            if (Actor.collision(powerUps, player))
                            {
                                powerUps.pickUp();
                            }
                            continue;
                        }
                    }
                    break;
                case GameState.Died:
                    {
                        if (keyboardstate.IsKeyDown(Keys.Enter))
                        {
                            this.gameState = GameState.Playing;
                            player.reset(PLAYER_INVINCIBILITY_TIME);
                        }
                    }
                    break;
                case GameState.Gameover:
                    if (keyboardstate.IsKeyDown(Keys.Enter))
                    {
                        this.gameState = GameState.Title;
                        for (int i = 0; i < 18000000; i++) ;
                    }
                    break;
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin();
            switch (this.gameState)
            {
                case GameState.Title:
                     drawTitleScreen();
                     break;
       
                case GameState.LevelChange:
                    drawLevelChangeScreen();
                    break;

                case GameState.Playing:
                  
                case GameState.Died:
                    
                case GameState.Gameover:
                  

                    spriteBatch.Draw(backGround, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);

                    Bar.DrawBars(spriteBatch);
                    Actor.drawActors(spriteBatch);
                    spriteBatch.DrawString(mainFont, "Score: " + this.score , new Vector2(4, 0), Color.White);
                    spriteBatch.DrawString(mainFont, "Value: " + currentScoreValue, new Vector2(4, 20), Color.White);
                    spriteBatch.DrawString(mainFont, "Lives Remaining: " + this.lives, new Vector2(4, 40), Color.White);

                    if (gameState == GameState.Died)
                        drawDeathScreen();
                    else if (gameState == GameState.Gameover)
                        drawGameOverScreen();
                    break;
            }
      
            base.Draw(gameTime);
            spriteBatch.End();

        }
        #region GamePlay

        private void beginGame()
        { 
            this.level = 1;
            this.lives = STARTING_LIVES;
            this.score = 0;
        }

        private void beginLevel()
        {
            Actor.actors.Clear();
            Bar.bars.Clear();
            Cell.resetCellCount();

            Cell.addCells(levelCellCount, cellTexture, cellHealthyColor, cellDeadColor);
            Enemy.AddEnemies(levelEnemyCount, enemyTexture, ENEMY_BASESPEED, ENEMY_SPEED_VARIATION);

            TimePowerUp timePowerUp = new TimePowerUp(timeTexture,timeBarColor);
            timePowerUp.RespondTime = TIME_POWERUP_RESPAWN_TIME;
            timePowerUp.ActiveTime = TIME_POWERUP_DURATION;
            timePowerUp.SlowDownPercent = TIME_POWER_UP_SLOWDOWN;

            this.player = new Player(playerTexture);
            this.player.reset(PLAYER_INVINCIBILITY_TIME);

            this.timeUntilAttack = this.cellAttackInterval;

            currentScoreValue = INITIAL_SCORE_VAL * this.level;
            Actor.TimeScale = 1;
        }

        #endregion
        #region utilityMethods


        public static Vector2 GetRandomScreenPosition(int padding)
        {
            return new Vector2(random.Next(padding, SCREEN_WIDTH - padding), random.Next(padding, SCREEN_HEIGHT - padding));
        }

        public static float Range (float min, float max)
        {
            return (float)random.NextDouble() * max - min;
        }


        #endregion

        #region EnemyHelper Method

        public static void addEnemyAt(Vector2 position)
        {
            Enemy enemy = new Enemy(enemyTexture, ENEMY_BASESPEED, ENEMY_SPEED_VARIATION);
            enemy.Position = position;
        }

        #endregion 
        #region screens

        private void drawTitleScreen()
        { 
            spriteBatch.Draw(titleTexture,new Rectangle(0,0,SCREEN_WIDTH,SCREEN_HEIGHT),Color.White);
        }

        private void drawDeathScreen()
        {
            spriteBatch.Draw(overlayTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT),overlayColor);
            spriteBatch.DrawString(mainFont, "YOU Died!", new Vector2(300f, 300f), Color.White);
            spriteBatch.DrawString(mainFont, "Press 'Enter' to continue", new Vector2(470f, 390f), Color.White);
        }


        private void drawGameOverScreen()
        {
            spriteBatch.Draw(overlayTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), overlayColor);
            spriteBatch.DrawString(mainFont, "YOU FAILED!", new Vector2(300f, 300f), Color.White);
            spriteBatch.DrawString(mainFont, "Press 'Enter' to try again", new Vector2(470f, 390f), Color.White);
        }
        private void drawLevelChangeScreen()
        {
            Vector2 position = new Vector2(270, 230);

            spriteBatch.Draw(levelChangeTexture, new Rectangle(0, 0, SCREEN_WIDTH, SCREEN_HEIGHT), Color.White);
            spriteBatch.DrawString(mainFont, "level " + this.level, position,Color.White);

            spriteBatch.Draw(cellTexture, position + new Vector2(0f, 80), cellHealthyColor);
            spriteBatch.DrawString(mainFont, "x" + this.levelCellCount, position + new Vector2(70f, 90f), Color.White);

            spriteBatch.Draw(enemyTexture, position + new Vector2(0f, 160), Color.White);
            spriteBatch.DrawString(mainFont, "x" + this.levelEnemyCount, position + new Vector2(70f, 170f), Color.White);

            spriteBatch.DrawString(mainFont, "Press 'Enter' to continue!", new Vector2(470f, 400f), Color.White);
        }


        #endregion

    }
}
