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

namespace GameProject
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        // game objects. Using inheritance would make this
        // easier, but inheritance isn't a GDD 1200 topic
        Burger burger;
        List<TeddyBear> bears = new List<TeddyBear>();
        static List<Projectile> projectiles = new List<Projectile>();
        List<Explosion> explosions = new List<Explosion>();
        Health help;

        // projectile and explosion sprites. Saved so they don't have to
        // be loaded every time projectiles or explosions are created
        static Texture2D frenchFriesSprite;
        static Texture2D teddyBearProjectileSprite;
        static Texture2D explosionSpriteStrip;

        // scoring support
        int score = 0;
        string scoreString = GameConstants.SCORE_PREFIX + 0;

        // health support
        string healthString = GameConstants.HEALTH_PREFIX + 
            GameConstants.BURGER_INITIAL_HEALTH;
        bool burgerDead = false;
        bool EscPresed = false;
        bool enterPresed = false;
        MainMenu menu;

        // text display support
        SpriteFont font;
        SpriteFont lose;

        // sound effects
        SoundEffect burgerShot;
        SoundEffect explosion;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";

            // set resolution
            graphics.PreferredBackBufferHeight = GameConstants.WINDOW_HEIGHT;
            graphics.PreferredBackBufferWidth = GameConstants.WINDOW_WIDTH;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            RandomNumberGenerator.Initialize();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // load audio content
            burgerShot = Content.Load<SoundEffect>("sounds//BurgerShot");
            explosion = Content.Load<SoundEffect>("sounds//Explosion");

            // load sprite font
            font = Content.Load<SpriteFont>("Arial20");
            lose = Content.Load<SpriteFont>("LoseGame");

            // load projectile and explosion sprites
            teddyBearProjectileSprite = Content.Load<Texture2D>("teddybearprojectile");
            frenchFriesSprite = Content.Load<Texture2D>("frenchfries");
            explosionSpriteStrip = Content.Load<Texture2D>("explosion");

            // add initial game objects
            burger = new Burger(Content, "burger", graphics.PreferredBackBufferWidth / 2,graphics.PreferredBackBufferHeight - graphics.PreferredBackBufferHeight / 8, burgerShot);
            for (int i = 0; i < GameConstants.MAX_BEARS; i++)
            {
                SpawnBear();
            }
            // set initial health and score strings
            help = new Health(Content, "health-icon" , graphics);

            // load menu
            menu = new MainMenu(Content);
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
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            // get current mouse state and update burger
            MouseState mouse = Mouse.GetState();
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                EscPresed = true;
            }
            if (keyboard.IsKeyUp(Keys.Escape) && EscPresed && !menu.Option)
            {
                menu.Active = !menu.Active;
                EscPresed = false;
            }
            else if (keyboard.IsKeyUp(Keys.Escape) && EscPresed && menu.Option)
            {
                menu.Option = false;
                EscPresed = false;
            }

            if (!burgerDead && !menu.Active)
            {
                burger.Update(gameTime, mouse, keyboard);
                help.Update(gameTime);

                // update other game objects
                foreach (TeddyBear bear in bears)
                {
                    bear.Update(gameTime);
                }
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Update(gameTime);
                }
                foreach (Explosion explosion in explosions)
                {
                    explosion.Update(gameTime);
                }

                //checks for resolve collissions between buger and help
                if (help.Active && help.CollisionRectangle.Intersects(burger.CollisionRectangle))
                {
                    help.Active = false;
                    burger.Health += GameConstants.HEALTH_BUNOS;
                    healthString = GameConstants.HEALTH_PREFIX + burger.Health;
                }

                // check and resolve collisions between teddy bears
                for (int i = 0; i < bears.Count; i++)
                {
                    for (int j = i + 1; j < bears.Count; j++)
                    {
                        if (bears[i].Active && bears[j].Active)
                        {
                            CollisionResolutionInfo cri = CollisionUtils.CheckCollision(gameTime.ElapsedGameTime.Milliseconds, GameConstants.WINDOW_WIDTH, GameConstants.WINDOW_HEIGHT, bears[i].Velocity, bears[i].DrawRectangle, bears[j].Velocity, bears[j].DrawRectangle);
                            if (cri != null)
                            {
                                // resolve collision
                                if (cri.FirstOutOfBounds)
                                {
                                    bears[i].Active = false;
                                }
                                else
                                {
                                    bears[i].Velocity = cri.FirstVelocity;
                                    bears[i].DrawRectangle = cri.FirstDrawRectangle;
                                }
                                if (cri.SecondOutOfBounds)
                                {
                                    bears[j].Active = false;
                                }
                                else
                                {
                                    bears[j].Velocity = cri.SecondVelocity;
                                    bears[j].DrawRectangle = cri.SecondDrawRectangle;
                                }
                            }
                        }
                    }
                }


                // check and resolve collisions between burger and teddy bears
                foreach (TeddyBear bear in bears)
                {
                    if (bear.Active && bear.CollisionRectangle.Intersects(burger.CollisionRectangle))
                    {
                        burger.Health -= GameConstants.BEAR_DAMAGE;
                        bear.Active = false;
                        Explosion explosion = new Explosion(explosionSpriteStrip, bear.Location.X, bear.Location.Y, this.explosion);
                        explosions.Add(explosion);
                        healthString = GameConstants.HEALTH_PREFIX + burger.Health;
                        CheckBurgerKill();
                    }
                }

                // check and resolve collisions between burger and projectiles
                foreach (Projectile projectile in projectiles)
                {
                    if (projectile.Type == ProjectileType.TeddyBear && projectile.Active && projectile.CollisionRectangle.Intersects(burger.CollisionRectangle))
                    {
                        burger.Health -= GameConstants.TEDDY_BEAR_PROJECTILE_DAMAGE;
                        healthString = GameConstants.HEALTH_PREFIX + burger.Health;
                        projectile.Active = false;
                        CheckBurgerKill();
                    }
                }

                // check and resolve collisions between teddy bears and projectiles
                foreach (TeddyBear bear in bears)
                {
                    foreach (Projectile projectile in projectiles)
                    {
                        if (projectile.Type == ProjectileType.FrenchFries && bear.Active && projectile.Active && bear.CollisionRectangle.Intersects(projectile.CollisionRectangle))
                        {
                            bear.Active = false;
                            projectile.Active = false;
                            Explosion explosion = new Explosion(explosionSpriteStrip, bear.Location.X, bear.Location.Y, this.explosion);
                            explosions.Add(explosion);
                            score += GameConstants.BEAR_POINTS;
                            scoreString = GameConstants.SCORE_PREFIX + score;
                        }
                    }
                }
                // clean out inactive teddy bears and add new ones as necessary
                for (int i = bears.Count - 1; i >= 0; i--)
                {
                    if (!bears[i].Active)
                    {
                        bears.RemoveAt(i);
                    }
                }
                while (bears.Count < GameConstants.MAX_BEARS)
                {
                    SpawnBear();
                }

                // clean out inactive projectiles
                for (int i = projectiles.Count - 1; i >= 0; i--)
                {
                    if (!projectiles[i].Active)
                    {
                        projectiles.RemoveAt(i);
                    }
                }

                // clean out finished explosions
                for (int i = explosions.Count - 1; i >= 0; i--)
                {
                    if (explosions[i].Finished)
                    {
                        explosions.RemoveAt(i);
                    }
                }
            }
            else if (menu.Active)
            {
                menu.Update(gameTime, mouse, keyboard);

                if (menu.Exit)
                {
                    this.Exit();
                }
                else if (menu.Resume)
                {
                    menu.Resume = false;
                    menu.Active = false;
                }
                else if (menu.Restart)
                {
                    menu.Restart = false;
                    menu.Active = false;
                    ResetGame();
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            spriteBatch.Begin();

            // draw game objects
            if (!menu.Active)
            {
                burger.Draw(spriteBatch);
                foreach (TeddyBear bear in bears)
                {
                    bear.Draw(spriteBatch);
                }
                foreach (Projectile projectile in projectiles)
                {
                    projectile.Draw(spriteBatch);
                }
                foreach (Explosion explosion in explosions)
                {
                    explosion.Draw(spriteBatch);
                }

                //draw help
                if (help.Active)
                {
                    help.Draw(spriteBatch);
                }

                // draw you lose
                if (burgerDead)
                {
                    spriteBatch.DrawString(lose, GameConstants.GAME_LOSE, GameConstants.LOSE_STRING_LOCATION, Color.DarkRed);
                }

                // draw score and health
                spriteBatch.DrawString(font, scoreString, GameConstants.SCORE_LOCATION, Color.Black);
                spriteBatch.DrawString(font, healthString, GameConstants.HEALTH_LOCATION, Color.Black);
            }

            if (menu.Active)
            {
                GraphicsDevice.Clear(Color.DarkGray);
                menu.Draw(spriteBatch);
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        #region Public methods

        /// <summary>
        /// Gets the projectile sprite for the given projectile type
        /// </summary>
        /// <param name="type">the projectile type</param>
        /// <returns>the projectile sprite for the type</returns>
        public static Texture2D GetProjectileSprite(ProjectileType type)
        {
            // replace with code to return correct projectile sprite based on projectile type
            if (type == ProjectileType.FrenchFries)
                return frenchFriesSprite;
            else if (type == ProjectileType.TeddyBear)
                return teddyBearProjectileSprite;
            else
                return frenchFriesSprite;
        }

        /// <summary>
        /// Adds the given projectile to the game
        /// </summary>
        /// <param name="projectile">the projectile to add</param>
        public static void AddProjectile(Projectile projectile)
        {
            projectiles.Add(projectile);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Spawns a new teddy bear at a random location
        /// </summary>
        private void SpawnBear()
        {
            // generate random location
            int x = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth - 2 * GameConstants.SPAWN_BORDER_SIZE);
            int y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight - 2 * GameConstants.SPAWN_BORDER_SIZE);

            // generate random velocity
            float speed = GameConstants.MIN_BEAR_SPEED + RandomNumberGenerator.NextFloat(GameConstants.BEAR_SPEED_RANGE);
            float angle = RandomNumberGenerator.NextFloat((float)Math.PI * 2);
            Vector2 velocity = new Vector2((float)(speed * Math.Cos(angle)), (float)(speed * Math.Sin(angle)));

            // create new bear
            TeddyBear newBear = new TeddyBear(Content, "teddybear", x, y, velocity, null, null);
            
            // make sure we don't spawn into a collision
            List<Rectangle> collisionRectangles = GetCollisionRectangles();
            while (!CollisionUtils.IsCollisionFree(newBear.DrawRectangle, collisionRectangles))
            {
                x = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth - 2 * GameConstants.SPAWN_BORDER_SIZE);
                y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight - 2 * GameConstants.SPAWN_BORDER_SIZE);
                newBear.X = x;
                newBear.Y = y;
            }

            // add new bear to list
            bears.Add(newBear);

        }

        /// <summary>
        /// Gets a random location using the given min and range
        /// </summary>
        /// <param name="min">the minimum</param>
        /// <param name="range">the range</param>
        /// <returns>the random location</returns>
        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        /// <summary>
        /// Gets a list of collision rectangles for all the objects in the game world
        /// </summary>
        /// <returns>the list of collision rectangles</returns>
        private List<Rectangle> GetCollisionRectangles()
        {
            List<Rectangle> collisionRectangles = new List<Rectangle>();
            collisionRectangles.Add(burger.CollisionRectangle);
            foreach (TeddyBear bear in bears)
            {
                collisionRectangles.Add(bear.CollisionRectangle);
            }
            foreach (Projectile projectile in projectiles)
            {
                collisionRectangles.Add(projectile.CollisionRectangle);
            }
            foreach (Explosion explosion in explosions)
            {
                collisionRectangles.Add(explosion.CollisionRectangle);
            }
            return collisionRectangles;
        }

        /// <summary>
        /// Checks to see if the burger has just been killed
        /// </summary>
        private void CheckBurgerKill()
        {
            if (burger.Health == 0 && !burgerDead)
            {
                burgerDead = true;
            }
        }

        private void ResetGame()
        {
            score = 0;
            burger.Health = 100;
            burgerDead = false;
            for (int i = explosions.Count - 1; i >= 0; i--)
            {
                    explosions.RemoveAt(i);
            }
            for (int i = bears.Count - 1; i >= 0; i--)
            {
                    bears.RemoveAt(i);
            }
            for (int i = projectiles.Count - 1; i >= 0; i--)
            {
                projectiles.RemoveAt(i);
            }
            scoreString = GameConstants.SCORE_PREFIX + score;
            healthString = GameConstants.HEALTH_PREFIX + burger.Health;
            help.Active = false;
        }

        #endregion
    }
}