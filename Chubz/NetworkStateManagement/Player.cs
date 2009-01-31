﻿using System;
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
        private Texture2D texture;

        public Player(Vector2 partial)
        {
            MapPosition = partial;
            ScreenPosition = new Vector2(400, 300);
        }

        public void LoadContent(Texture2D t)
        {
            texture = t;
        }

        public void Update()
        {

            // Player Movement Code here!
            //movement code modifies the MapPosition  allowing scrolling action

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, ScreenPosition, Color.White);
        }
    }
    
}
