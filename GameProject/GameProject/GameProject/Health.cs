using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace GameProject
{
    /// <summary>
    /// A class for health
    /// </summary>
    public class Health
    {
        #region Filed
        GraphicsDeviceManager graphics;

        // graphic and drawing info
        Texture2D sprite;
        Rectangle drawRectangle;

        //health status
        bool active;
        int elapsedTime = 0;
        int elapsedCoolDown = 0;
        int elapsedBlinkTime = 0;
        bool show = false;

        #endregion

        #region constractors

        public Health(ContentManager contentManager, string spriteName, GraphicsDeviceManager graphics)
        {
            this.graphics = graphics;
            int x = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth);
            int y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight);
            LoadContent(contentManager, spriteName, x, y);
            active = false;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the collision rectangle for the health
        /// </summary>
        public Rectangle CollisionRectangle
        {
            get { return drawRectangle; }
        }

        /// <summary>
        /// Gets and sets whether or not the Health is active
        /// </summary>
        public bool Active
        {
            get { return active; }
            set 
            { 
                active = value;
                elapsedTime = 0;
            }
        }

        public Rectangle DrawRectangle
        {
            get { return drawRectangle; }
            set { drawRectangle = value; }
        }
        #endregion

        #region methods

        /// <summary>
        /// update the health status
        /// </summary>
        /// <param name="gameTime">game time</param>
        public void Update(GameTime gameTime)
        {
            if (active == false)
            {
                elapsedTime += gameTime.ElapsedGameTime.Milliseconds;
                if (elapsedTime >= GameConstants.HEALTH_SPAWN_TIME_MILLISECONDS)
                {
                    show = true;
                    active = true;
                    elapsedCoolDown = 0;
                }
            }

            if (active == true)
            {
                elapsedCoolDown += gameTime.ElapsedGameTime.Milliseconds;
                if (GameConstants.HEALTH_COOLDOWN_TIME_MILLISECONDS - elapsedCoolDown <= 2000)
                {
                     elapsedBlinkTime += gameTime.ElapsedGameTime.Milliseconds;
                    if (elapsedBlinkTime> 0 && elapsedBlinkTime < 200)
                    {
                        show = false;
                    }
                    else if (elapsedBlinkTime >= 200 && elapsedBlinkTime < 600)
                    {
                        show = true;
                    }
                    else
                    {
                        elapsedBlinkTime = 0;
                    }
                }
                if (elapsedCoolDown >= GameConstants.HEALTH_COOLDOWN_TIME_MILLISECONDS)
                {
                    drawRectangle.X = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferWidth);
                    drawRectangle.Y = GetRandomLocation(GameConstants.SPAWN_BORDER_SIZE, graphics.PreferredBackBufferHeight);
                    active = false;
                    show = false;
                    elapsedTime = 0;
                }
            }
        }

        /// <summary>
        /// Draws the health
        /// </summary>
        /// <param name="spriteBatch">the sprite batch to use</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            if (show)
            {
                spriteBatch.Draw(sprite, drawRectangle, Color.White);
            }
        }

        /// <summary>
        /// Loads the content for the health
        /// </summary>
        /// <param name="contentManager">the content manager to use</param>
        /// <param name="spriteName">the name of the sprite for the health</param>
        /// <param name="x">the x location of the center of the health</param>
        /// <param name="y">the y location of the center of the health</param>
        private void LoadContent(ContentManager contentManager, string spriteName,
            int x, int y)
        {
            // load content and set remainder of draw rectangle
            sprite = contentManager.Load<Texture2D>(spriteName);
            drawRectangle = new Rectangle(x - sprite.Width / 2,
                y - sprite.Height / 2, sprite.Width,
                sprite.Height);
        }

        private int GetRandomLocation(int min, int range)
        {
            return min + RandomNumberGenerator.Next(range);
        }

        #endregion
    }
}
