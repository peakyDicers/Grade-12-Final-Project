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
    class Sprite
    {
        protected Texture2D picture;
        protected Rectangle rect;

        //Getters
        public Texture2D getPic()
        {
            return picture;
        }
        public Rectangle getRec()
        {
            return rect;
        }

        //setters
        public void setPic(Texture2D aPic)
        {
            picture = aPic;
        }
        public void setRec(Rectangle aRec)
        {
            rect = aRec;
        }

        //constructors
        public Sprite(Texture2D aPic, Rectangle aRec)
        {
            setPic(aPic);
            setRec(aRec);
        }
    }
}
