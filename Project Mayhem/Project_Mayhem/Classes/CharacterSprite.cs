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
    class CharacterSprite:Sprite //Created By: Nick Geofroy
    {
        //fields
        protected int row;
        protected int column;
        protected int health;
        protected Rectangle hitbox;


        //getters
        public int getRow()
        {
            return row;
        }
        public int getColumn()
        {
            return column;
        }
        public int getHealth()
        {
            return health;
        }
        public Rectangle getHitbox()
        {
            return hitbox;
        }

        //setters
        public void setRow(int aRow)
        {
            row = aRow;
        }
        public void setColumn(int aColumn)
        {
            column = aColumn;
        }
        public void setHealth(int aHealth)
        {
            health = aHealth;
        }
        public void setHitbox(Rectangle aHitbox)
        {
            hitbox = aHitbox;
        }

        //constructors
        public CharacterSprite(Texture2D aPic, Rectangle aRec, Rectangle aHitbox, int aHealth):base(aPic, aRec)
        {
            setHealth(aHealth);
            setHitbox(aHitbox);
        }
    }
}
