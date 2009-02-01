#region File Description
//-----------------------------------------------------------------------------
// GDDLogoScreen.cs
//
// Display GDD logo as a popup screen on top of the logo screen.--Robby/Chris
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace Chubz
{
    /// <summary>
    /// The GDD logo screen enlarges the GDD logo image, then 
    /// decreases its size back to 0, then exits.
    /// </summary>
    //Robby Florence
    class GDDLogoScreen : GameScreen
    {
        #region Fields

        TimeSpan elapsedTime = TimeSpan.Zero;
        TimeSpan stopGDDTime = TimeSpan.FromSeconds(1.0);

        //Game Audio --Chris Wykel
        ContentManager content;
        Texture2D logoTexture;

        #endregion

        #region Initialization

        /// <summary>
        /// Constructor.
        /// </summary>
        public GDDLogoScreen()
        {
            IsPopup = true;

            TransitionOnTime = TimeSpan.FromSeconds(0.0);
            TransitionOffTime = TimeSpan.FromSeconds(0.0);

        }

        /// <summary>
        /// Loads graphics content for this screen. The logo texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            logoTexture = content.Load<Texture2D>("Intro/GDD2007_Transparent");
            
        }

        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        #region Update and Draw

        /// <summary>
        /// Updates the GDD logo screen. This only increments the elapsed time counter
        /// and exits the screen after a set amount of display time.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
            
            elapsedTime += gameTime.ElapsedGameTime;

            //stop drawing GDD logo after set amount of time
            if (elapsedTime > stopGDDTime)
            {
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            //skip intro
            if (input.IsPauseGame(ControllingPlayer))
            {
                // Activate the first screens.
                LoadingScreen.Load(ScreenManager, false, ControllingPlayer, new BackgroundScreen(), new MainMenuScreen());
            }
        }

        /// <summary>
        /// Draws the GDD logo and/or explosion.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;

            spriteBatch.Begin();
            spriteBatch.End();
        }

        #endregion
    }
}
