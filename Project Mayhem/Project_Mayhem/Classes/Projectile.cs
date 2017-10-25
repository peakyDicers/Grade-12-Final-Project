using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Project_Mayhem
{
    class Projectile:Sprite //Created by Ian Chui
    {
        protected bool facingRight;
        protected int speed;
        protected int row;

        public Projectile(Texture2D aPic, Rectangle aRec, int spd, bool right):base(aPic,aRec)
        {
            facingRight = right;
            speed = spd;
        }

        public void Draw(SpriteBatch sb)
        {
            //draw the projectile.
            sb.Draw(picture, rect, new Rectangle(0, row * rect.Height, rect.Width, rect.Height), Color.White);
        }

        public void move()
        {
            if (facingRight)
                rect.X += speed; //move right, if facing right.
            else
                rect.X -= speed; //move left, if facing left.
        }

        public bool getDir()
        {
            return facingRight; //get direction of projectile.
        }
    }
}
    