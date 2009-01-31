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

        public AnimatingSprite sprite;

        public int Weight = 300;
        //1 = light, 2 = medium, 3 = heavy
        public int Size = 2;

        float eatTimer = 0f;
        float eatTime = 0.5f;

        public Player(Vector2 partial)
        {
            MapPosition = partial;
            ScreenPosition = new Vector2(336, 236);
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

            CollisionDetection();

            //change size
            if (Size != 1 && Weight < 250f)
            {
                Size = 1;
                int firstSpace = sprite.CurrentAnimation.IndexOf(' ');
                sprite.CurrentAnimation = "light" + sprite.CurrentAnimation.Substring(firstSpace);
            }
            else if (Size != 2 && Weight >= 250f && Weight <= 350f)
            {
                Size = 2;
                int firstSpace = sprite.CurrentAnimation.IndexOf(' ');
                sprite.CurrentAnimation = "medium" + sprite.CurrentAnimation.Substring(firstSpace);
            }
            else if (Size != 3 && Weight > 350f)
            {
                Size = 3;
                int firstSpace = sprite.CurrentAnimation.IndexOf(' ');
                sprite.CurrentAnimation = "heavy" + sprite.CurrentAnimation.Substring(firstSpace);
            }

            if (sprite.CurrentAnimation.Contains("eat"))
            {
                eatTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (eatTimer >= eatTime)
                {
                    sprite.CurrentAnimation = sprite.CurrentAnimation.Remove(sprite.CurrentAnimation.Length - 4);
                    sprite.Animations[sprite.CurrentAnimation].Reset();
                    eatTimer = 0f;
                }
            }
        }

        public void HandleInput(InputState input)
        {
            KeyboardState ks = input.CurrentKeyboardStates[0];
            PlayerIndex playerIndex;
            bool keyPressed = false;

            //on solid platform
            if ((Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] > 0 &&
                Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] < 4) ||
                (Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)((MapPosition.X + 127) / Levels.TileSize)] > 0 &&
                Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)((MapPosition.X + 127) / Levels.TileSize)] < 4))
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
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Position = ScreenPosition;
            sprite.Draw(spriteBatch);
        }

        public void CollisionDetection()
        {
            Rectangle boundingBox, tileBoundingBox;

            if (Velocity.X != 0)
            {
                MapPosition.X += Velocity.X;

                boundingBox = new Rectangle(
                    (int)MapPosition.X, (int)MapPosition.Y,
                    127, 127
                    );

                for (int y = (int)(MapPosition.Y / Levels.TileSize); y <= (int)((MapPosition.Y + 127) / Levels.TileSize); y++)
                {
                    for (int x = (int)(MapPosition.X / Levels.TileSize); x <= (int)((MapPosition.X + 127) / Levels.TileSize); x++)
                    {
                        if (y >= Levels.Height || x >= Levels.Width ||
                            (Levels.level_1[y, x] > 0 && Levels.level_1[y, x] < 4))
                        {
                            tileBoundingBox = new Rectangle(
                                Levels.TileSize * x, Levels.TileSize * y,
                                Levels.TileSize, Levels.TileSize
                                );

                            if (boundingBox.Intersects(tileBoundingBox))
                            {
                                if (Velocity.X < 0)
                                    MapPosition.X = (x + 1) * Levels.TileSize;
                                else
                                    MapPosition.X = x * Levels.TileSize - 128;
                                Velocity.X = 0f;
                            }
                        }
                    }
                }
            }

            //gravity
            if (Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] == 0 &&
                Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)((MapPosition.X + 127) / Levels.TileSize)] == 0)
            {
                Velocity.Y += 0.25f;
            }

            MapPosition.Y += Velocity.Y;

            boundingBox = new Rectangle(
                (int)MapPosition.X, (int)MapPosition.Y,
                127, 127
                );

            for (int y = (int)(MapPosition.Y / Levels.TileSize); y <= (int)((MapPosition.Y + 127) / Levels.TileSize); y++)
            {
                for (int x = (int)(MapPosition.X / Levels.TileSize); x <= (int)((MapPosition.X + 127) / Levels.TileSize); x++)
                {
                    if (y >= Levels.Height || x >= Levels.Width ||
                            (Levels.level_1[y, x] > 0 && Levels.level_1[y, x] < 4))
                    {
                        tileBoundingBox = new Rectangle(
                            Levels.TileSize * x, Levels.TileSize * y,
                            Levels.TileSize, Levels.TileSize
                            );

                        if (boundingBox.Intersects(tileBoundingBox))
                        {
                            if (Velocity.Y < 0f)
                            {
                                MapPosition.Y = (y + 1) * Levels.TileSize;
                                Velocity.Y = 0f;
                            }
                            else if (Velocity.Y > 0f)
                                MapPosition.Y = y * Levels.TileSize - 128;
                        }
                    }
                }
            }

            //just landed on a platform
            if (sprite.CurrentAnimation.Contains("fall") &&
                ((Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] > 0 &&
                Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] < 4) ||
                (Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)((MapPosition.X + 127) / Levels.TileSize)] > 0 &&
                Levels.level_1[(int)((MapPosition.Y + 128) / Levels.TileSize), (int)((MapPosition.X + 127) / Levels.TileSize)] < 4)))
            {
                sprite.CurrentAnimation = sprite.CurrentAnimation.Remove(sprite.CurrentAnimation.Length - 5);
                sprite.StopAnimation();
            }
        }
    }
    
}
