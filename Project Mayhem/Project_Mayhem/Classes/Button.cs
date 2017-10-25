using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Text;

namespace Project_Mayhem
{
    class Button:Sprite //Created By: Nick Geofroy
    {
        //fields
        Color color;
        string text;
        bool selected;
        SpriteFont font;

        public Button(Texture2D aPic, Rectangle aRec, Color aColor, string aText, bool aSelected, SpriteFont afont):base(aPic,aRec)
        {
            setSelected(aSelected);
            setText(aText);
            color = aColor;
            font = afont;
        }
        //getters and setters
        public bool getSelected()
        {
            return selected;
        }
        public string getText()
        {
            return text;
        }
        public void setSelected(bool aSelected)
        {
            selected = aSelected;
        }
        public void setText(string aText)
        {
            text = aText;
        }
        public void Draw(SpriteBatch sb)
        {
            if (!selected) //if the button isn't selected
            {
                sb.Draw(picture, rect, Color.Transparent);//draws the button with a transparent background
                sb.DrawString(font, text, new Vector2(rect.X + rect.Width / 2 - font.MeasureString(text).X / 2, rect.Y), Color.Turquoise); //draws the button label in the centre of the button
            }
            else if (selected) //if the button has been selected
            {
                sb.Draw(picture, new Rectangle(rect.X - 3, rect.Y - 3, rect.Width + 6, rect.Height + 6), Color.Turquoise); //draws the button with a turquise background and larger than the not selected one
                sb.DrawString(font, text, new Vector2(rect.X + rect.Width / 2 - font.MeasureString(text).X / 2, rect.Y), Color.Black); //changes the font colour to black
            }
            
        }
    }
}
