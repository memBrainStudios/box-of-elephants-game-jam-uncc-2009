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

    class GoodEnemies
    {
        public bool alive = false;
        public Vector2 MapPosition = new Vector2(0, 0);
        public Vector2 OriginalVector = new Vector2(0, 0);

        AnimatingSprite sprite;


        public GoodEnemies(Vector2 locationVector, Texture2D temp, int type)
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

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);
        }

        public void HandleInput(InputState input)
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position)
        {
            sprite.Position = position;
            sprite.Draw(spriteBatch, 0.5f);
        }
    }

}
