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

    class BadEnemies
    {
        public bool alive = false;
        public Vector2 MapPosition = new Vector2(0,0);
        public Vector2 OriginalVector = new Vector2(0,0);
        public Vector2 Velocity = new Vector2(0, 0);

        AnimatingSprite sprite;

        public BadEnemies(Vector2 locationVector, Texture2D temp,  int type)
        {
            OriginalVector = locationVector;
            MapPosition = OriginalVector;
            alive = true;
            MapPosition.X = MapPosition.X * 32;
            MapPosition.Y = MapPosition.Y * 32;

            sprite = new AnimatingSprite();
            sprite.Texture = temp;

            Animation right = new Animation(512, 128, 4, 0, 128 * type);
            right.FramesPerSecond = 4;
            Animation left = new Animation(512, 128, 4, 512, 128 * type);
            left.FramesPerSecond = 4;
            sprite.Animations.Add("right", right);
            sprite.Animations.Add("left", left);
            sprite.CurrentAnimation = "left";
        }

        public void LoadContent(Texture2D t)
        {
        }

        public void Update(GameTime gameTime, Vector2 playerPosition)
        {
            //run towards player
            float distance = playerPosition.X - MapPosition.X;
            if (distance > 0f && distance < 100f)
                Velocity.X = 1f;
            else if (distance > -100f && distance < 0f)
                Velocity.X = -1f;
            else
                Velocity.X = 0f;

            CollisionDetection();

            if (Velocity.X < 0f)
                sprite.CurrentAnimation = "left";
            else if (Velocity.X > 0f)
                sprite.CurrentAnimation = "right";

            sprite.Update(gameTime);
        }

        public void HandleInput(InputState input)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenOffset)
        {
            sprite.Position = MapPosition + screenOffset;
            sprite.Draw(spriteBatch, 0.5f);
        }

        public void CollisionDetection()
        {
            Rectangle boundingBox, tileBoundingBox;

            if (Velocity.X != 0)
            {
                MapPosition.X += Velocity.X;

                boundingBox = new Rectangle(
                    (int)MapPosition.X, (int)MapPosition.Y,
                    63, 63
                    );

                for (int y = (int)(MapPosition.Y / Levels.TileSize); y <= (int)((MapPosition.Y + 63) / Levels.TileSize); y++)
                {
                    for (int x = (int)(MapPosition.X / Levels.TileSize); x <= (int)((MapPosition.X + 63) / Levels.TileSize); x++)
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
                                    MapPosition.X = x * Levels.TileSize - 64;
                                Velocity.X = 0f;
                            }
                        }
                    }
                }
            }

            //gravity
            if (Levels.level_1[(int)((MapPosition.Y + 64) / Levels.TileSize), (int)(MapPosition.X / Levels.TileSize)] == 0 &&
                Levels.level_1[(int)((MapPosition.Y + 64) / Levels.TileSize), (int)((MapPosition.X + 63) / Levels.TileSize)] == 0)
            {
                Velocity.Y += 0.25f;
            }

            MapPosition.Y += Velocity.Y;

            boundingBox = new Rectangle(
                (int)MapPosition.X, (int)MapPosition.Y,
                63, 63
                );

            for (int y = (int)(MapPosition.Y / Levels.TileSize); y <= (int)((MapPosition.Y + 63) / Levels.TileSize); y++)
            {
                for (int x = (int)(MapPosition.X / Levels.TileSize); x <= (int)((MapPosition.X + 63) / Levels.TileSize); x++)
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
                                MapPosition.Y = y * Levels.TileSize - 64;
                        }
                    }
                }
            }
        }
    }

}
