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
        public Vector2 MapPosition;
        public Vector2 OriginalVector;
        Texture2D texture;


        public BadEnemies(Vector2 locationVector, Texture2D temp)
        {
            OriginalVector = locationVector;
            MapPosition = OriginalVector;
            alive = true;
            MapPosition.X = MapPosition.X * 32;
            MapPosition.Y = MapPosition.X * 32;

            texture = temp;
        }

        public void LoadContent(Texture2D t)
        {
        }

        public void Update(GameTime gameTime)
        {
        }

        public void HandleInput(InputState input)
        {
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, MapPosition, Color.White);
        }
    }

}
