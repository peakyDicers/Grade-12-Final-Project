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
    class Bomb : Sprite //Created By: Ian Chui
    {
        Rectangle explodeRec;
       // List<Bomb> bombs;
        int tossDir;
        int row;
        int frameTimer, frameTimer2;
        int accel = -4;
        int bounceDir = 1;
        int ground;
        int counter;
        int column;
        bool explode;              

        public Bomb(Rectangle rec, Texture2D pic, int dir, Joker joker, int aGround, ContentManager content):base(pic,rec)
        {
            //set up basic bomb variables.
            rect.Width = 20;
            rect.Height = 20;
            setTossDir(dir);
            ground = aGround;

            //set initial location based on the direction joker is facing.
            if (joker.getRunDir() == 1)
                rect.X = joker.getHitbox().X + joker.getHitbox().Width + 60;
            else
                rect.X = joker.getHitbox().X - 60;

            rect.Y = joker.getHitbox().Y + 20;

        }
        public void setTossDir(int aTossDir)
        {
            //used so bomb toss direction is independent of joker's facing direction.
            tossDir = aTossDir;
        }
        public void update(ContentManager content, Player batman, List<Bomb> aBombs, int count, List<Joker> joker)
        {
            counter++;
            if (counter < 80) //bomb is bouncing.
            {
                #region Animating bomb
                if (frameTimer == 0)
                {
                    frameTimer = 7;
                    if (tossDir == 1) //make bomb animation clockwise.
                        row++;
                    else
                        row--;        //make bomb animation counter-clockwise.
                }
                if (tossDir == 1)
                {
                    if (row > 3)      //loop animation clockwise.
                        row = 0;
                }
                else
                {
                    if (row < 0)      //loop animation counter-clockwise.
                        row = 3;
                }
                frameTimer--; 
                #endregion

                #region Bomb movement
                rect.X += 8 * tossDir; //move the bomb horizontal.

                //Bouncing bomb.
                if (frameTimer2 == 0)
                {
                    frameTimer2 = 5;

                    if (bounceDir == 1) //if falling, accel downwards.
                        accel++;
                    if (bounceDir == -1) //if bouncing, accel downwards still.
                        accel--;

                    if (rect.Y + rect.Height > ground - 3) //reduce acceleration each time the bomb hits the ground. (bounce less)
                    {
                        rect.Y = ground - rect.Height - 3;
                        accel -= 2;
                        bounceDir = -1;
                    }
                    if (accel == 0) //when bomb reaches peak height, change direction of bomb down.
                        bounceDir = 1;
                }
                frameTimer2--;

                rect.Y += accel * bounceDir; 
                #endregion
            }

            #region Explode on impact
            if (!explode)
            {
                if (rect.Intersects(batman.getHitbox())) //explode of the bomb hits batman.
                    counter = 81;

                for (int i = 0; i < batman.getBatarang().Count; i++) //explode of bomb is hit by baterang.
                {
                    if (rect.Intersects(batman.getBatarang()[i].getRec()))
                    {
                        counter = 81;
                        batman.Deactivate(i);
                    }
                }
            } 
            #endregion

            if (counter > 80) //bomb is exploding.
            {
                #region Dealing damage 
                if (!explode)
                {
                    explodeRec = new Rectangle(rect.X - 50, rect.Y - 50, 100, 100);
                    frameTimer = 0;
                    row = 0;
                    column = -1;
                    explode = true;
                    if (explodeRec.Intersects(batman.getHitbox()))  //deal damage to batman if hit.
                        batman.TakeDamage(10);
                    for (int i = 0; i < joker.Count; i++)
                    {
                        if (explodeRec.Intersects(joker[i].getHitbox())) //deal damage to joker if hit.
                            joker[i].takeDamge(10);
                    }
                } 
                #endregion

                #region Exploding Animation
                picture = content.Load<Texture2D>("explode");

                if (frameTimer == 0)
                {
                    column += 5;
                    frameTimer = 1;
                }
                if (column > 8 && row < 8)
                {
                    column = 0;
                    row++; ;
                }

                if (column == 1 && row == 8) //remove bomb at the end of exploding animation.
                    aBombs.RemoveAt(count);

                frameTimer--; 
                #endregion
            }  
        }
        public void draw(SpriteBatch sb)
        {
            if (!explode) //draw bomb.
                sb.Draw(picture, rect, new Rectangle(0, row*8, 8, 8), Color.White);

            if (explode) //draw explosion
                sb.Draw(picture, explodeRec, new Rectangle(column * 100, row * 100, 100, 100), Color.White);
        }


    }
}
