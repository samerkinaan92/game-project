using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;

namespace GameProject
{
    /// <summary>
    /// display the main menu
    /// </summary>
    public class MainMenu
    {
        #region Fileds

        SpriteFont font;
        SpriteFont selectedFont;

        string[] options = new string[GameConstants.OPTIONS_NUM];

        //support
        bool active = false;
        bool upPresed = false;
        bool downPresed = false;
        bool exit = false;
        bool resume = false;
        bool restart = false;
        bool enterPresed = false;
        bool option = false;
        int highlighted = 0;
        Vector2[] locations = new Vector2[GameConstants.OPTIONS_NUM];

        #endregion

        #region Constructors

        /// <summary>
        /// constructs main menu 
        /// </summary>
        /// <param name="contentManager"></param>
        public MainMenu(ContentManager contentManager)
        {
            LoadContent(contentManager);
        }

        #endregion

        #region Properties

        public bool Active
        {
            get { return active; }
            set { active = value; }
        }

        public bool Exit
        {
            get { return exit; }
            set { exit = value; }
        }

        public bool Resume
        {
            get { return resume; }
            set { resume = value; }
        }

        public bool Restart
        {
            get { return restart; }
            set { restart = value; }
        }

        public bool Option
        {
            get { return option; }
            set { option = value; }
        }

        public int Highlighted
        {
            get { return highlighted; }
        }

        #endregion

        #region Public methods

        public void Update(GameTime gameTime, MouseState mouse, KeyboardState keyboard)
        {
            if (keyboard.IsKeyDown(Keys.Enter))
            {
                enterPresed = true;
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                downPresed = true;
            }
            if (keyboard.IsKeyDown(Keys.Up))
            {
                upPresed = true;
            }

            if (!option)
            {
                if (highlighted == 3 && enterPresed && keyboard.IsKeyUp(Keys.Enter))
                {
                    exit = true;
                    enterPresed = false;
                }
                else if (highlighted == 0 && enterPresed && keyboard.IsKeyUp(Keys.Enter))
                {
                    resume = true;
                    enterPresed = false;
                }
                else if (highlighted == 1 && enterPresed && keyboard.IsKeyUp(Keys.Enter))
                {
                    restart = true;
                    enterPresed = false;
                }
                else if (highlighted == 2 && enterPresed && keyboard.IsKeyUp(Keys.Enter))
                {
                    option = true;
                    enterPresed = false;
                }
                if (downPresed && keyboard.IsKeyUp(Keys.Down))
                {
                    highlighted++;
                    highlighted = highlighted % GameConstants.OPTIONS_NUM;
                    downPresed = false;
                    enterPresed = false;
                }

                if (upPresed && keyboard.IsKeyUp(Keys.Up))
                {
                    highlighted--;
                    if (highlighted < 0)
                        highlighted = GameConstants.OPTIONS_NUM - 1;
                    upPresed = false;
                    enterPresed = false;
                }
            }
            else
            {
                //TODO
                // option menu update
            }

        }

        public void Draw(SpriteBatch spriteBatch)
        {
            if (!option)
            {
                for (int i = 0; i < GameConstants.OPTIONS_NUM; i++)
                {
                    if (i == highlighted)
                    {
                        spriteBatch.DrawString(selectedFont, options[i], locations[i], Color.DarkRed);
                    }
                    else
                    {
                        spriteBatch.DrawString(font, options[i], locations[i], Color.Black);
                    }
                }
            }
            else
            {
                //option menu
            }
        }

        #endregion

        #region Private methods

        private void LoadContent(ContentManager contentManager)
        {
            // load strings
            options[0] = GameConstants.RESUME;
            options[1] = GameConstants.RESTART;
            options[2] = GameConstants.OPTION;
            options[3] = GameConstants.EXIT;

            // set locations
            for (int i = 0; i < GameConstants.OPTIONS_NUM; i++)
            {
                locations[i].X = GameConstants.MENU_DISPLAY_OFFSET;
                locations[i].Y = (i + 1) * GameConstants.MENU_DISPLAY_OFFSET;
            }

            // load fonts
            font = contentManager.Load<SpriteFont>("MainMenu");
            selectedFont = contentManager.Load<SpriteFont>("Selected");
        }

        #endregion
    }
}
