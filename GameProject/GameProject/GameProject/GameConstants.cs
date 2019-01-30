using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace GameProject
{
    /// <summary>
    /// All the constants used in the game
    /// </summary>
    public static class GameConstants
    {
        // resolution
        public const int WINDOW_WIDTH = 800;
        public const int WINDOW_HEIGHT = 600;

        // projectile characteristics
        public const float TEDDY_BEAR_PROJECTILE_SPEED = 0.3f;
        public const int TEDDY_BEAR_PROJECTILE_DAMAGE = 5;
        public const int TEDDY_BEAR_PROJECTILE_OFFSET = 20;
        public const float FRENCH_FRIES_PROJECTILE_SPEED = 0.4f;
        public const int FRENCH_FRIES_PROJECTILE_DAMAGE = 5;
        public const int FRENCH_FRIES_PROJECTILE_OFFSET = 10;

        // bear characteristics
        public const int MAX_BEARS = 5;
        public const int BEAR_POINTS = 10;
        public const int BEAR_DAMAGE = 10;
        public const float MIN_BEAR_SPEED = 0.1f;
        public const float BEAR_SPEED_RANGE = 0.2f;
        public const int BEAR_MIN_FIRING_DELAY = 500;
        public const int BEAR_FIRING_RATE_RANGE = 1000;

        //health characteristics
        public const int HEALTH_SPAWN_TIME_MILLISECONDS = 32000;
        public const int HEALTH_COOLDOWN_TIME_MILLISECONDS = 5000;
        public const int HEALTH_BUNOS = 20;

        // burger characteristics
        public const int BURGER_INITIAL_HEALTH = 100;
        public const int BURGER_MOVEMENT_AMOUNT = 5;
        public const int BURGER_COOLDOWN_MILLISECONDS = 500;

        // explosion hard-coded animation info. There are better
        // ways to do this, we just don't know enough to use them yet
        public const int EXPLOSION_FRAMES_PER_ROW = 3;
        public const int EXPLOSION_NUM_ROWS = 3;
        public const int EXPLOSION_NUM_FRAMES = 9;
        public const int EXPLOSION_FRAME_TIME = 10;

        // display support
        const int DISPLAY_OFFSET = 35;
        public const string SCORE_PREFIX = "Score: ";
        public static readonly Vector2 SCORE_LOCATION =
            new Vector2(DISPLAY_OFFSET, DISPLAY_OFFSET);
        public const string HEALTH_PREFIX = "Health: ";
        public static readonly Vector2 HEALTH_LOCATION =
            new Vector2(DISPLAY_OFFSET, 2 * DISPLAY_OFFSET);
        public const string GAME_LOSE = "GAME OVER!";
        public static readonly Vector2 LOSE_STRING_LOCATION = 
            new Vector2(DISPLAY_OFFSET, WINDOW_HEIGHT / 3);

        // spawn location support
        public const int SPAWN_BORDER_SIZE = 100;

        // main menu suppoort
        public const string START = "Start";
        public const string RESTART = "Restart";
        public const string RESUME = "Resume";
        public const string OPTION = "Option";
        public const string EXIT = "Exit";
        public const int OPTIONS_NUM = 4;
        public const int MENU_DISPLAY_OFFSET = 50;
    }
}
