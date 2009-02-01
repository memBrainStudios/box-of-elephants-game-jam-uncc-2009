#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
#endregion

namespace Chubz
{

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields
        Vector2[] Enemies = new Vector2[40];
        NetworkSession networkSession;
        ContentManager content;
        SpriteFont gameFont;

        Random random = new Random();

        Player player;
        GoodEnemies[] enemiesGood = new GoodEnemies[20];
        BadEnemies[] enemiesBad = new BadEnemies[20];

        float enemySpawnTimer = 0f;
        float totalTime = 0f;

        Platforms[] platforms = new Platforms[40];

        Texture2D grndTexture;
        Texture2D NormalPlatformA;
        Texture2D NormalPlatformB;
        Texture2D enemiesTexture;
        Texture2D goalTexture;
        Texture2D gameBackground;
        Texture2D staticBG;

        AnimatingSprite bg;
        Animation bgAnim;

        #endregion

        #region Properties


        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }


        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;

            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            player = new Player(new Vector2(Levels.TileSize, 18*Levels.TileSize - 128));
            Levels.Initialize();
            initilizeEnemies();


            bg = new AnimatingSprite();
            bgAnim = new Animation(800, 600, 2, 0, 0);
            bgAnim.FramesPerSecond = 4;

            bg.Animations.Add("animation", bgAnim);

            bg.CurrentAnimation = "animation";
            bg.StopAnimation();
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            gameFont = content.Load<SpriteFont>("gamefont");

            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();

            player.LoadContent(content.Load<Texture2D>("chubs spritesheet"));
            NormalPlatformA = content.Load<Texture2D>("platformA");
            NormalPlatformB = content.Load<Texture2D>("platformB");
            grndTexture = content.Load<Texture2D>("wall");
            goalTexture = content.Load<Texture2D>("Door");
            enemiesTexture = content.Load<Texture2D>("FoodSpriteSheet");
            staticBG = content.Load<Texture2D>("backgroundFinal");

            bg.Texture = content.Load<Texture2D>("backgroundFinal");

            initPlatforms();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        public void initPlatforms()
        {
            int count = 0;

            for (int y = 0; y < Levels.Height; y++)
            {
                for (int x = 0; x < Levels.Width; x++)
                {
                    if (Levels.level_1[y, x] == 3)
                    {
                        platforms[count] = new Platforms(new Vector2(x, y), NormalPlatformB, 350);
                        count++;
                    }
                }
            }
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (!IsActive)
                return;

            player.Update(gameTime, platforms);

            enemySpawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            totalTime += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (player.Win)
            {
                ScreenManager.AddScreen(new GameOverScreen(networkSession, true), (PlayerIndex)1);
                return;
            }
            else if (player.Lose)
            {
                ScreenManager.AddScreen(new GameOverScreen(networkSession, false), (PlayerIndex)1);
                return;
            }

            for (int i = 0; i < Levels.level_1.GetLength(0); i++)
            {
                for (int j = 0; j < Levels.level_1.GetLength(1); j++)
                {
                    if ((Levels.level_1[i, j] == 4))//detecting GoodEnemies
                    {
                        UpdateEnemy(new Vector2(j, i), true);
                    }
                    if ((Levels.level_1[i, j] == 5))//detecting BadEnemies
                    {
                        UpdateEnemy(new Vector2(j, i), false);
                    }

                }
            }

            for (int i = 0; i < enemiesBad.Length; i++)
            {
                if (enemiesBad[i] != null && enemiesBad[i].alive)
                    enemiesBad[i].Update(gameTime, player);
            }

            for (int i = 0; i < enemiesGood.Length; i++)
            {
                if (enemiesGood[i] != null && enemiesGood[i].alive)
                    enemiesGood[i].Update(gameTime, player);
            }

            for (int i = 0; i < platforms.Length; i++)
            {
                if (platforms[i] != null && platforms[i].Active)
                    platforms[i].Update(gameTime, player);
            }

            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, true, null,
                                       new BackgroundScreen(),
                                       new LobbyScreen(networkSession));
                }
            }

            bg.StartAnimation();
            bg.Update(gameTime);
        }


        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (ControllingPlayer.HasValue)
            {
                // In single player games, handle input for the controlling player.
                HandlePlayerInput(input, ControllingPlayer.Value);
            }
            else if (networkSession != null)
            {
                // In network game modes, handle input for all the
                // local players who are participating in the session.
                foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
                {
                    if (!HandlePlayerInput(input, gamer.SignedInGamer.PlayerIndex))
                        break;
                }
            }
        }


        /// <summary>
        /// Handles input for the specified player. In local game modes, this is called
        /// just once for the controlling player. In network modes, it can be called
        /// more than once if there are multiple profiles playing on the local machine.
        /// Returns true if we should continue to handle input for subsequent players,
        /// or false if this player has paused the game.
        /// </summary>
        bool HandlePlayerInput(InputState input, PlayerIndex playerIndex)
        {
            // Look up inputs for the specified player profile.
            KeyboardState keyboardState = input.CurrentKeyboardStates[(int)playerIndex];
            GamePadState gamePadState = input.CurrentGamePadStates[(int)playerIndex];

            // The game pauses either if the user presses the pause button, or if
            // they unplug the active gamepad. This requires us to keep track of
            // whether a gamepad was ever plugged in, because we don't want to pause
            // on PC if they are playing with a keyboard and have no gamepad at all!
            bool gamePadDisconnected = !gamePadState.IsConnected &&
                                       input.GamePadWasConnected[(int)playerIndex];

            if (input.IsPauseGame(playerIndex) || gamePadDisconnected)
            {
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession), playerIndex);
                return false;
            }

            player.HandleInput(input);
            

            return true;
        }
        /// <summary>
        /// Initializes the Enemies list to all blanks
        /// </summary>
        public void initilizeEnemies()
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                Enemies[i] = new Vector2(0, 0);
            }
        }
        /// <summary>
        /// Determines if enemies are alive
        /// </summary>
        public bool enemyIsAlive(Vector2 TestVector)// is alive?
        {
            for (int i = 0; i < enemiesBad.Length; i++)//check all enemies
            {
                if ((enemiesBad[i]!= null)&& ((enemiesBad[i].OriginalVector.Equals(TestVector)) && (enemiesBad[i].alive))) //has same origin and is alive
                {
                    return true;
                }

            }
            for (int i = 0; i < enemiesGood.Length; i++)//check all enemies
            {
                if ((enemiesGood[i]!= null)&& ((enemiesGood[i].OriginalVector.Equals(TestVector)) && (enemiesGood[i].alive))) //has same origin and is alive
                {
                    return true;
                }

            }
            return false;// no living enemy found with same vector
        }
        /// <summary>
        /// Updates Enemies aka creates or removes depending on situation.
        /// </summary>
        public void UpdateEnemy(Vector2 OriginalVector, bool type)
        {
            Vector2 TestVector = OriginalVector;// make test vector
            // if is already monster check to kill or let live? if not create
            if (!existingEnemy(TestVector))
            {
                if (enemySpawnTimer > 1f || totalTime < 1f)
                {
                    for (int i = 0; i < Enemies.Length; i++)
                    {
                        if (Enemies[i] == new Vector2(0, 0))//find first opening in list
                        {
                            Enemies[i] = TestVector; //create monster
                            if (type)//if good enemy
                            {
                                enemiesGood[i] = new GoodEnemies(OriginalVector, enemiesTexture, random.Next(6, 12)); //create good enemy
                            }
                            else // if bad enemy
                            {
                                enemiesBad[i] = new BadEnemies(OriginalVector, enemiesTexture, random.Next(6)); //create bad enemy
                            }

                            enemySpawnTimer = 0f;

                            return;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < Enemies.Length; i++) 
                {
                    if (Enemies[i].Equals(TestVector))//find the monster in list
                    {
                        if (!enemyIsAlive(TestVector))// check if alive?
                        {
                            Enemies[i] = new Vector2(0, 0);// remove from list if not alive
                        }
                    }
                }
            }
        }
        public bool existingEnemy(Vector2 TestVector)
        {
            for (int i = 0; i < Enemies.Length; i++)
            {
                if (Enemies[i].Equals(TestVector))
                {
                    return true;
                }
            }
            return false;
            
        }

        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            // This game has a blue background. Why? Because!
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target,
                                               Color.ForestGreen, 0, 0);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            Vector2 playerPos = player.ScreenPosition - player.MapPosition;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.BackToFront, SaveStateMode.None);
            //bg.Draw(spriteBatch);
            spriteBatch.Draw(staticBG, new Rectangle(0, 0, 800, 600), new Rectangle(0, 0, 800, 600), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1.0f);
            for (int i = 0; i < Levels.level_1.GetLength(0); i++)
            {
                for (int j = 0; j < Levels.level_1.GetLength(1); j++)
                {
                    Vector2 tilePos = new Vector2(j * Levels.TileSize, i * Levels.TileSize);

                    tilePos += playerPos;

                    if ((Levels.level_1[i, j] == 1))//detecting walls
                    {
                        //spriteBatch.Draw(grndTexture, tilePos, Color.White);
                        spriteBatch.Draw(grndTexture, new Rectangle((int)tilePos.X,(int)tilePos.Y, 32, 32), new Rectangle(0, 0, 32, 32), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, .9f);
                    }
                    if ((Levels.level_1[i, j] == 2))//detecting immobile plats
                    {
                        //spriteBatch.Draw(NormalPlatformA, tilePos, Color.White);
                        spriteBatch.Draw(NormalPlatformA, new Rectangle((int)tilePos.X, (int)tilePos.Y, 32, 32), new Rectangle(0, 0, 32, 32), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, .9f);
                    }
                    if ((Levels.level_1[i, j] == 3))//detecting moving plats
                    {
                                                //spriteBatch.Draw(NormalPlatformB, new Rectangle((int)tilePos.X, (int)tilePos.Y, 32, 32), new Rectangle(0, 0, 32, 32), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, 1.0f);
                    }

                    if (Levels.level_1[i, j] == 9)
                    {
                        /*spriteBatch.Draw(goalTexture,
                            new Rectangle((int)tilePos.X, (int)tilePos.Y, 128, 128), Color.White);*/
                        spriteBatch.Draw(goalTexture, new Rectangle((int)tilePos.X, (int)tilePos.Y, 128, 128), new Rectangle(0, 0, 128, 128), Color.White, 0f, new Vector2(0, 0), SpriteEffects.None, .99f);
                    }

                    if ((Levels.level_1[i, j] == 4))
                    {
                        Vector2 enemyPosition = new Vector2(j, i);

                        for (int a = 0; a < enemiesGood.Length; a++)
                        {
                            if (enemiesGood[a] != null && enemiesGood[a].alive && enemiesGood[a].OriginalVector.Equals(enemyPosition))
                                enemiesGood[a].Draw(spriteBatch, playerPos);
                        }
                    }
                    if ((Levels.level_1[i, j] == 5))
                    {
                        Vector2 enemyPosition = new Vector2(j, i);

                        for (int a = 0; a < enemiesBad.Length; a++)
                        {
                            if (enemiesBad[a] != null && enemiesBad[a].alive && enemiesBad[a].OriginalVector.Equals(enemyPosition))
                                enemiesBad[a].Draw(spriteBatch, playerPos);
                        }
                    }
                }
            }

            for (int a = 0; a < platforms.Length; a++)
            {
                if (platforms[a] != null && platforms[a].Active)
                    platforms[a].Draw(spriteBatch, playerPos);
            }

            player.Draw(spriteBatch);

            /*spriteBatch.DrawString(
                ScreenManager.Font,
                "Weight: " + player.Weight,
                new Vector2(10, 10),
                Color.White
                );*/

            spriteBatch.DrawString(
                ScreenManager.Font,
                "Weight: " + player.Weight + " lbs.",
                new Vector2(10, 10),
                Color.White,
                0f,
                new Vector2(0, 0),
                1f,
                SpriteEffects.None,
                0f);

            if (networkSession != null)
            {
                string message = "Players: " + networkSession.AllGamers.Count;
                Vector2 messagePosition = new Vector2(100, 480);
                spriteBatch.DrawString(gameFont, message, messagePosition, Color.White);
            }

            spriteBatch.End();

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        #endregion
    }
}
