﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;


//This is not used ATM soon to allow moving Platforms.
namespace Chubz
{
    class Platforms
    {
        public Vector2 OriginalVector = new Vector2(0, 0);
        public Vector2 MapPosition = new Vector2(0, 0);
        public Vector2 Velocity = new Vector2(0, 0);
        Texture2D texture;
        int tolerance;
        public bool Active = true;

        public Platforms(Vector2 partial, Texture2D tex, int weight)
        {
            OriginalVector = partial;
            MapPosition.X = OriginalVector.X * 32;
            MapPosition.Y = OriginalVector.Y * 32;
            texture = tex;
            tolerance = weight;
        }

        public void Update(Player player)
        {
            Rectangle boundingBox, playerBoundingBox;

            //bounding box only determines if the player is on top of the platform
            boundingBox = new Rectangle((int)MapPosition.X, (int)MapPosition.Y - 2, 32, 1);
            playerBoundingBox = new Rectangle((int)player.MapPosition.X, (int)player.MapPosition.Y, 127, 127);

            if (Velocity.Y == 0f && player.Weight > tolerance && boundingBox.Intersects(playerBoundingBox))
            {
                Velocity.Y = 0.25f;

                Levels.level_1[(int)MapPosition.Y / Levels.TileSize,
                    (int)MapPosition.X / Levels.TileSize] = 0;
            }

            if (Velocity.Y != 0f)
            {
                Velocity.Y += 0.25f;
                MapPosition += Velocity;

                if (MapPosition.Y >= Levels.Height * Levels.TileSize)
                    Active = false;
            }

        }
        public void Draw(SpriteBatch spriteBatch, Vector2 screenOffset)
        {
            spriteBatch.Draw(texture, MapPosition + screenOffset, Color.White);
        }
    }
}
