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

        AnimatingSprite sprite;

        public int Weight = 300;
        //1 = light, 2 = medium, 3 = heavy
        public int Size = 2;

        public Player(Vector2 partial)
        {
            MapPosition = partial;
            ScreenPosition = new Vector2(400, 300);
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
            sprite.Animations.Add("light eat", new Animation(128, 128, 1, 0, 384));
            sprite.Animations.Add("medium eat", new Animation(128, 128, 1, 128, 384));
            sprite.Animations.Add("heavy eat", new Animation(128, 128, 1, 256, 384));
            sprite.Animations.Add("light fall", new Animation(128, 128, 1, 384, 384));
            sprite.Animations.Add("medium fall", new Animation(128, 128, 1, 512, 384));
            sprite.Animations.Add("heavy fall", new Animation(128, 128, 1, 640, 384));

            sprite.CurrentAnimation = "medium right";
            sprite.StopAnimation();
        }

        public void Update(GameTime gameTime)
        {
            sprite.Update(gameTime);

            // Player Movement Code here!
            //movement code modifies the MapPosition  allowing scrolling action
            KeyboardState ks = Keyboard.GetState();

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
            }
            else
            {
                sprite.Animations[sprite.CurrentAnimation].Reset();
                sprite.StopAnimation();
            }


        }
        public void Draw(SpriteBatch spriteBatch)
        {
            sprite.Position = ScreenPosition;
            sprite.Draw(spriteBatch);
        }
    }
    
}
