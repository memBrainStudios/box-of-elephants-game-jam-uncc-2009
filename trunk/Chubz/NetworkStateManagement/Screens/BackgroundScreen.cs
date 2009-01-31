#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
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
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D backgroundTexture;

        AnimatingSprite chubzSprite;
        AnimatingSprite donutSprite;
        AnimatingSprite orangeSprite;

        Animation mediumRight;
        Animation mediumLeft;

        Animation donutRight;
        Animation donutLeft;
        Animation orangeRight;
        Animation orangeLeft;

        Vector2 pos;
        Vector2 posD;
        Vector2 posO;

        bool right;
        bool rightD;
        bool rightO;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen()
        {
            //content = new ContentManager(
            //content.RootDirectory = "Content";

            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            chubzSprite = new AnimatingSprite();
            donutSprite = new AnimatingSprite();
            orangeSprite = new AnimatingSprite();

            mediumRight = new Animation(512, 128, 4, 0, 128);
            mediumRight.FramesPerSecond = 5;
            mediumLeft = new Animation(512, 128, 4, 512, 128);
            mediumLeft.FramesPerSecond = 5;

            donutRight = new Animation(512, 128, 4, 0, 512);
            donutRight.FramesPerSecond = 5;
            donutLeft = new Animation(512, 128, 4, 512, 512);
            donutLeft.FramesPerSecond = 5;

            orangeRight = new Animation(512, 128, 4, 0, 1280);
            orangeRight.FramesPerSecond = 5;
            orangeLeft = new Animation(512, 128, 4, 512, 1280);
            orangeLeft.FramesPerSecond = 5;

            chubzSprite.Animations.Add("medium right", mediumRight);
            chubzSprite.Animations.Add("medium left", mediumLeft);

            donutSprite.Animations.Add("right", donutRight);
            donutSprite.Animations.Add("left", donutLeft);
            orangeSprite.Animations.Add("right", orangeRight);
            orangeSprite.Animations.Add("left", orangeLeft);

            chubzSprite.CurrentAnimation = "medium right";
            chubzSprite.StopAnimation();

            donutSprite.CurrentAnimation = "right";
            donutSprite.StopAnimation();

            orangeSprite.CurrentAnimation = "right";
            orangeSprite.StopAnimation();

            pos = new Vector2(100, 472);
            posD = new Vector2(0, 536);
            posO = new Vector2(278, 536);

            right = true;
            rightD = true;
            rightO = true;
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            backgroundTexture = content.Load<Texture2D>("background");
            chubzSprite.Texture = content.Load<Texture2D>("chubs spritesheet");
            donutSprite.Texture = content.Load<Texture2D>("FoodSpriteSheet");
            orangeSprite.Texture = content.Load<Texture2D>("FoodSpriteSheet");
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
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            if (pos.X == 672)
                right = false;
            if (pos.X == 0)
                right = true;

            if (right)
            {
                pos.X++;
                chubzSprite.CurrentAnimation = "medium right";
                chubzSprite.StartAnimation();
            }
            else if (!right)
            {
                pos.X--;
                chubzSprite.CurrentAnimation = "medium left";
                chubzSprite.StartAnimation();
            }

            if (posD.X == 672)
                rightD = false;
            if (posD.X == 0)
                rightD = true;

            if (rightD)
            {
                posD.X++;
                donutSprite.CurrentAnimation = "right";
                donutSprite.StartAnimation();
            }
            else if (!rightD)
            {
                posD.X--;
                donutSprite.CurrentAnimation = "left";
                donutSprite.StartAnimation();
            }

            if (posO.X == 672)
                rightO = false;
            if (posO.X == 0)
                rightO = true;

            if (rightO)
            {
                posO.X++;
                orangeSprite.CurrentAnimation = "right";
                orangeSprite.StartAnimation();
            }
            else if (!rightO)
            {
                posO.X--;
                orangeSprite.CurrentAnimation = "left";
                orangeSprite.StartAnimation();
            }

            chubzSprite.Update(gameTime);
            donutSprite.Update(gameTime);
            orangeSprite.Update(gameTime);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            spriteBatch.Begin(/*SpriteBlendMode.None*/);

            spriteBatch.Draw(backgroundTexture, fullscreen,
                             new Color(fade, fade, fade));
            orangeSprite.Position = posO;
            orangeSprite.Draw(spriteBatch, 0.5f);
            chubzSprite.Position = pos;
            chubzSprite.Draw(spriteBatch);
            donutSprite.Position = posD;
            donutSprite.Draw(spriteBatch, 0.5f);
            spriteBatch.End(); 
        }


        #endregion
    }
}
