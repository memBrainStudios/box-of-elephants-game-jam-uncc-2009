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
        Texture2D texture;


        public BadEnemies(Vector2 locationVector, Texture2D temp)
        {
            OriginalVector = locationVector;
            MapPosition = OriginalVector;
            alive = true;
            MapPosition.X = MapPosition.X * 32;
            MapPosition.Y = MapPosition.Y * 32;

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

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            spriteBatch.Draw(texture, position, Color.White);
        }
    }

}
