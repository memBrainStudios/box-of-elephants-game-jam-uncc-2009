using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;

namespace Chubz
{

    class Player
    {
        public Vector2 MapPosition;
        public Vector2 ScreenPosition;
        public Vector2 Velocity = Vector2.Zero;
        public float Ground;

        AnimatingSprite sprite;

        public int Weight = 300;
        //1 = light, 2 = medium, 3 = heavy
        public int Size = 2;

        public Player(Vector2 partial)
        {
            MapPosition = partial;
            ScreenPosition = new Vector2(400, 300);
            Ground = MapPosition.Y;
        }

        public void LoadContent(Texture2D t)
        {
            sprite = new AnimatingSprite();

            sprite.Texture = t;

            //add animations
            Animation lightRight = new Animation(512, 128, 4, 0, 0);
            lightRight.FramesPerSecond = 6;
            Animation lightLeft = new Animation(512, 128, 4, 512, 0);
            lightLeft.FramesPerSecond = 6;
            Animation mediumRight = new Animation(512, 128, 4, 0, 128);
            mediumRight.FramesPerSecond = 5;
            Animation mediumLeft = new Animation(512, 128, 4, 512, 128);
            mediumLeft.FramesPerSecond = 5;
            Animation heavyRight = new Animation(512, 128, 4, 0, 256);
            heavyRight.FramesPerSecond = 4;
            Animation heavyLeft = new Animation(512, 128, 4, 512, 256);
            heavyLeft.FramesPerSecond = 4;
            sprite.Animations.Add("light right", lightRight);
            sprite.Animations.Add("light left", lightLeft);
            sprite.Animations.Add("medium right", mediumRight);
            sprite.Animations.Add("medium left", mediumLeft);
            sprite.Animations.Add("heavy right", heavyRight);
            sprite.Animations.Add("heavy left", heavyLeft);
            sprite.Animations.Add("light right eat", new Animation(128, 128, 1, 0, 384));
            sprite.Animations.Add("medium right eat", new Animation(128, 128, 1, 128, 384));
            sprite.Animations.Add("heavy right eat", new Animation(128, 128, 1, 256, 384));
            sprite.Animations.Add("light right fall", new Animation(128, 128, 1, 384, 384));
            sprite.Animations.Add("medium right fall", new Animation(128, 128, 1, 512, 384));
            sprite.Animations.Add("heavy right fall", new Animation(128, 128, 1, 640, 384));
            sprite.Animations.Add("light left eat", new Animation(128, 128, 1, 0, 512));
            sprite.Animations.Add("medium left eat", new Animation(128, 128, 1, 128, 512));
            sprite.Animations.Add("heavy left eat", new Animation(128, 128, 1, 256, 512));
            sprite.Animations.Add("light left fall", new Animation(128, 128, 1, 384, 512));
            sprite.Animations.Add("medium left fall", new Animation(128, 128, 1, 512, 512));
            sprite.Animations.Add("heavy left fall", new Animation(128, 128, 1, 640, 512));
            

            sprite.CurrentAnimation = "medium right";
            sprite.StopAnimation();
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void HandleInput(InputState input)
        {
            KeyboardState ks = input.CurrentKeyboardStates[0];
            PlayerIndex playerIndex;
            bool keyPressed = false;

            //on ground
            if (MapPosition.Y == Ground)
            {
                if (ks.IsKeyDown(Keys.Left))
                {
                    Velocity.X = -(8 - 2 * Size);

                    if (Size == 1 && !sprite.CurrentAnimation.Equals("light left"))
                        sprite.CurrentAnimation = "light left";
                    else if (Size == 2 && !sprite.CurrentAnimation.Equals("medium left"))
                        sprite.CurrentAnimation = "medium left";
                    else if (Size == 3 && !sprite.CurrentAnimation.Equals("heavy left"))
                        sprite.CurrentAnimation = "heavy left";

                    MapPosition += Velocity;
                    sprite.StartAnimation();
                    keyPressed = true;
                }
                else if (ks.IsKeyDown(Keys.Right))
                {
                    Velocity.X = 8 - 2 * Size;

                    if (Size == 1 && !sprite.CurrentAnimation.Equals("light right"))
                        sprite.CurrentAnimation = "light right";
                    else if (Size == 2 && !sprite.CurrentAnimation.Equals("medium right"))
                        sprite.CurrentAnimation = "medium right";
                    else if (Size == 3 && !sprite.CurrentAnimation.Equals("heavy right"))
                        sprite.CurrentAnimation = "heavy right";

                    MapPosition += Velocity;
                    sprite.StartAnimation();
                    keyPressed = true;
                }

                if (input.IsNewKeyPress(Keys.Space, null, out playerIndex))
                {
                    Velocity.Y = -(12 - 1.5f * Size);

                    if (Size == 1)
                    {
                        if (sprite.CurrentAnimation.Equals("light right"))
                            sprite.CurrentAnimation = "light right fall";
                        else
                            sprite.CurrentAnimation = "light left fall";
                    }
                    else if (Size == 2)
                    {
                        if (sprite.CurrentAnimation.Equals("medium right"))
                            sprite.CurrentAnimation = "medium right fall";
                        else
                            sprite.CurrentAnimation = "medium left fall";
                    }
                    else if (Size == 3)
                    {
                        if (sprite.CurrentAnimation.Equals("heavy right"))
                            sprite.CurrentAnimation = "heavy right fall";
                        else
                            sprite.CurrentAnimation = "heavy left fall";
                    }

                    MapPosition += Velocity;
                    sprite.StartAnimation();
                    keyPressed = true;
                }

                if (!keyPressed)
                {
                    Velocity = Vector2.Zero;
                    sprite.Animations[sprite.CurrentAnimation].Reset();
                    sprite.StopAnimation();
                }
            }

            //gravity
            if (MapPosition.Y < Ground)
            {
                Velocity.Y += 0.25f;
                MapPosition += Velocity;

                if (MapPosition.Y >= Ground)
                {
                    MapPosition.Y = Ground;
                    Velocity = Vector2.Zero;
                    sprite.CurrentAnimation = sprite.CurrentAnimation.Remove(sprite.CurrentAnimation.Length - 5);
                    sprite.Animations[sprite.CurrentAnimation].Reset(); 
                    sprite.StopAnimation();
                }
            }

            //change chub's size (just for testing)
            if (input.IsNewKeyPress(Keys.Enter, null, out playerIndex))
            {
                Size++;
                if (Size > 3)
                    Size = 1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Position = ScreenPosition;
            sprite.Draw(spriteBatch);
        }
    }
    
}
