using System;
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
        public Vector2 MapPosition;
        private Texture2D texture;

        public Platforms(Vector2 partial)
        {
            MapPosition = partial;
        }

        public void LoadContent(Texture2D t)
        {
            texture = t;
        }

        public void Update()
        {
 
            // Platform Movement Code here!

        }
        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, MapPosition, Color.White);

        }
    }
}
