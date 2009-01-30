using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Chubz
{
    class Player
    {
        public Vector2 MapPosition;
        public Vector2 ScreenPosition;
        private Texture2D texture;
        private Vector2 speed;

        public Player(Vector2 partial)
        {
            MapPosition = partial;
            ScreenPosition = new Vector2(400, 300);
        }

        public void LoadContent(Texture2D t)
        {
            texture = t;
        }

        public void Update(GameTime gameTime)
        {

        }
        public void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if(input.IsNewKeyPress(Keys.Left, PlayerIndex Player))
            {

            }
        }
    }
    
}
