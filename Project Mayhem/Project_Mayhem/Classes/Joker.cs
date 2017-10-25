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
    class Joker : CharacterSprite //Created By: Ian Chui
    {
        Random randomNum = new Random();
        Texture2D hitboxPic;
        List<AIEnemy> thugs;
        Rectangle punchHitbox = new Rectangle(0, 0, 60, 20);
        ContentManager content;

        int runDir;
        int oldHealth;
        int state, oldState;
        int accel, ranDir, counter;
        int frameTimer, frameTimer2, frameTimer3, frameTimer4; //used in: draw method, action method (state 4) , to determermine batman's running direction, stun lock check.
        int oldBatmanLcn, batmanLcn;
        bool invincible;
        bool idle = true;

        #region AI variables
        bool atk1;
        bool runAway, beginRunAway;
        bool overWrite = true;
        bool punch;
        bool onCD;
        bool tossBomb;
        bool jump;
        bool dead;
        int punches;
        int counter2, cdCounter, cdCounter2;  //used in: running away, run away cooldown, stun lock cooldown.
        int punchDir;
        int ground;
        int time;
        int gravity, upVelocity;
        int rdmTime;
        int flyTime = 20000;
        const int speed = 4; 
        #endregion
          
        //constructor
        public Joker(Texture2D jokerPic, Rectangle aJokerRec, Rectangle jokerHitbox, int aJokerHP, int aGround, List<AIEnemy> newEnemies, ContentManager newContent):base(jokerPic, aJokerRec, jokerHitbox, aJokerHP)
        {
            thugs = newEnemies;
            content = newContent;

            //setting joker hitbox values.
            ground = aGround;
            hitbox.Width = 45;
            hitbox.Height = 100;
            hitbox.X = 1100;
            hitbox.Y = ground - hitbox.Height;


            //setting joker sprite rectangle values.
            rect.X = 1100;
            rect.Y = ground - 105;
            rect.Width = 160;
            rect.Height = 140;
        }

        #region Setters and Getters
        //setters
        public void setBombToss(bool aTossBomb)
        {
            tossBomb = aTossBomb;
        }

        //getters
        public bool getBombToss()
        {
            return tossBomb;
        }
        public int getRunDir()
        {
            return runDir;
        }        
        #endregion


        public void update(Player batman, Joker joker, GraphicsDevice gd)
        {
            //keep sprite rectangle ontop of hitbox.
            rect.X = hitbox.X - 60;
            rect.Y = hitbox.Y - 35;

            //gravity affects joker if he is above the ground (and not using jetpack).
            if (hitbox.Y + hitbox.Height < ground && state != 4)
            {
                gravity++;
                hitbox.Y += gravity;
            }     

            //When the joker is alive.
            if (!dead) 
            {
                batmanLcn = (batman.getHitbox().X + (batman.getHitbox().Width / 2));
                runDir = faceBatman();
                
                #region Jump over baterangs
                for (int i = 0; i < batman.getBatarang().Count; i++)
                {
                    //if batarang is 130 pixels away from joker. 
                    if ((Math.Abs((batman.getBatarang()[i].getRec().X + (batman.getBatarang()[i].getRec().Width / 2)) - (hitbox.X + (hitbox.Width / 2))) < 120) && idle)                   
                    {
                        if ((batman.getBatarang()[i].getRec().X < hitbox.X) && batman.getBatarang()[i].getDir())
                        // if batarang is left of joker and flying right.
                        {
                            state = 6;
                            idle = false;
                        }
                        if ((batman.getBatarang()[i].getRec().X > hitbox.X + hitbox.Width) && !batman.getBatarang()[i].getDir())
                        // if batarang is right of joker and flying left.
                        {
                            state = 6;
                            idle = false;
                        }
                    }
                }
                #endregion

                #region Use the jetpack
                rdmTime = randomNum.Next(20);
                flyTime -= rdmTime;

                if (flyTime <= 0 && !dead)
                {

                    flyTime = 20000;
                    idle = false;
                    state = 4;
                    accel = 5;
                    counter = 0;
                    ranDir = randomNum.Next(1, 3);
                    if (ranDir == 2)
                        ranDir = -1;
                } 
                #endregion

                #region Toss a bomb
                /// The joker will throw a bomb at batman if he is running towards him.
                if ((Math.Abs(oldBatmanLcn - hitbox.X) - Math.Abs(batmanLcn - hitbox.X) > 60) && (Math.Abs(hitbox.X - batmanLcn) > 150) && idle)
                //                (batman is running towards joker)                           (batman is >150 pixels away)
                {
                    idle = false;
                    state = 3;
                } 
                #endregion

                #region Joker AI #1
                //================================================================================================================
                /// The following code makes it so Joker runs towards Batman when he less than 200 pixels away. Once reached, 
                /// he will punch the batman three times, before turning around and running away.           

                if (Math.Abs(hitbox.X - batmanLcn) < 300 && (Math.Abs(batmanLcn - hitbox.X) > 50) && idle && !onCD) //if batman < 200 away.
                {
                    atk1 = true;
                    idle = false;
                }
                if (atk1 && state != 5)
                {
                    if (Math.Abs(batmanLcn - (hitbox.X + hitbox.Width / 2)) > 60 && !punch) //run towards batman, until he is in punching range.
                    {
                        state = 1;
                        punchDir = runDir;
                    }
                    else
                    {
                        punch = true;
                        state = 2;
                    }
                    if (punches == 3)  //once reached, punch batman three times.
                    {
                        atk1 = false;
                        punches = 0;
                        punch = false;
                        runAway = true;
                    }
                }
                if (runAway)    //after three punches, run away from batman.
                {
                    if (!beginRunAway)
                    {
                        beginRunAway = true;
                        counter2 = 0;
                        state = 1;
                    }
                    counter2++;
                    if (overWrite)
                        runDir = faceBatman() * -1; //change facing direction to run away from batman.
                    if ((hitbox.X + hitbox.Width >= gd.Viewport.Width || hitbox.X <= 0))   //turns around if joker hits the edge of screen.           
                        overWrite = false;

                    if (counter2 == 90) //resetting all values, and run cooldown.
                    {
                        runAway = false;
                        beginRunAway = false;
                        idle = true;
                        state = 0;
                        overWrite = true;
                        onCD = true;
                    }
                }
                if (onCD)
                    coolDown(150);
                //================================================================================================================
                #endregion   
            }

            //prevents joker from being stun locked.
            if (frameTimer4 == 0)
            {
                oldHealth = health;
                frameTimer4 = 30;
            }
            frameTimer4--;

            if ((oldHealth - health) > 15)
                invincible = true;

            if (invincible)
                invincibleCD(120);

            //find out which way the batman is running.
            if (frameTimer3 == 0)
            {
                oldBatmanLcn = batman.getHitbox().X + batman.getHitbox().Width / 2;
                frameTimer3 = 30;
            }
            frameTimer3--;

            //if joker changes actions, reset animation counters.
            if (state != oldState)
            {
                row = 0;
                counter = 0;
            }             

            action(batman, gd);

            //prevent joker from falling into ground, after using jetpack.
            if (hitbox.Y + hitbox.Height >= ground)
            {
                hitbox.Y = ground - hitbox.Height;
                gravity = 0;
            }

            //if joker's health goes below zero. stop the AI.
            if (health <= 0)
            {
                dead = true;
                state = 7;
            }
                
            oldState = state;                     
        }
        public int faceBatman()
        {
            //determines which direction to face batman.
            if (batmanLcn - (hitbox.X + (hitbox.Width / 2)) > 0)
                return 1;
            else
                return -1;
        }
        public void takeDamge(int damage)
        {
            //makes the joker take damage.
            if (!dead && !invincible && state != 4)
            {
                health -= damage;
                state = 5;
            }
        }
        public void SpawnEnemies()
        {
            int temp;
            int location;
            Random rdm = new Random();
            temp = rdm.Next(2, 5);

            for (int i = 0; i < temp; i++) //spawn a random number of bad guys (up to five).
            {
                location = 60 * rdm.Next(10);
                thugs.Add(new AIEnemy(content.Load<Texture2D>("Thug"), new Rectangle(location, 360, 100, 120), new Rectangle(location, 360, 100, 120), 30, rdm.Next(1, 3)));
            }
        }
        public void coolDown(int amount)
        {
            //cooldown timer, used for 'Joker AI #1'
            cdCounter++;
            if (cdCounter == amount)
            {
                onCD = false;
                cdCounter = 0;
            }
        }
        public void invincibleCD(int amount)
        {
            //cooldown timer, used for stunlock.
            cdCounter2++;
            if (cdCounter2 == amount)
            {
                invincible = false;
                cdCounter2 = 0;
            }
        }
        public void revive() // bring me to life
        {
            state = 0; // reset joker
            column = 0;
            row = 0;
            dead = false;
            health = 400;
            hitbox.Width = 45;
            hitbox.Height = 100;
            hitbox.X = 1100;
            hitbox.Y = ground - hitbox.Height;
        }

        public void action(Player batman, GraphicsDevice gd)
        {
            punchHitbox.Y = -50; //hides punch hitbox when not in use.

            //idle state.
            #region State 0
            if (state == 0)
            {
                counter++;
                hitbox.X = hitbox.X;
                if (counter == 50)
                    state = 1;

            }
            #endregion

            //running state.
            #region State 1
            if (state == 1)
            {
                hitbox.X += speed* runDir; 
                counter++;
                if (counter == 100)
                {
                    state = 0;
                    counter = 0;
                }
            }
            #endregion

            //punching state.
            #region State 2
            if (state == 2)
            {
                counter++;
                if (counter > 24 && counter < 30) 
                {                                          
                    if (punchDir == 1) //punch towards the right.
                    {
                        punchHitbox.X = hitbox.X + (hitbox.Width/2);
                        punchHitbox.Y = hitbox.Y + 5;
                    }

                    else //punch towards the left.
                    {
                        punchHitbox.X = hitbox.X - (punchHitbox.Width/2);
                        punchHitbox.Y = hitbox.Y + 5;
                    }
                    if (punchHitbox.Intersects(batman.getHitbox()))
                        batman.TakeDamage(10);
                }

                if (counter == 30) //reset punching variables.
                {                  
                    punches++;
                    punch = false;
                    counter = 0;
                    state = 0;
                }
            }
            #endregion

            //throwing bomb state.
            #region State 3
            if (state == 3)
            {
                counter++;     
                if (counter > 45)
                {
                    tossBomb = true;
                    idle = true;
                    state = 0;
                    counter = 0;
                }
            }
            #endregion

            //flying state.
            #region State 4
            if (state == 4)
            {
                if (frameTimer2 == 0)
                {
                    frameTimer2 = 2;
                    counter++;
                    //===================== Joker's launch into the air ===================
                    if (counter < 20)
                    {
                        if (counter % 2 == 0)   //vibrate up and down.
                            hitbox.Y += 2;
                        else
                            hitbox.Y -= 2;
                    }

                    else if (counter < 300)     //fly up. once up, hover up and down.
                    {
                        if (hitbox.Y > 120)
                        {
                            if (accel > -10)        
                                accel--;
                        }
                        if (hitbox.Y < 115)     
                        {
                            if (accel < 3)
                                accel++;
                        }
                        hitbox.Y += accel;
                    }
                    if (counter == 20)
                        SpawnEnemies();
                    //=================== Joker flies in random direction ==================

                    if (counter > 100 && counter < 300)
                    {
                        hitbox.X = hitbox.X += ranDir * 6; 
                        if (hitbox.X + hitbox.Width> gd.Viewport.Width || hitbox.X < 0)   //change dirn when hits border.
                        {
                            ranDir *= -1;
                        }
                    }
                    //====================== Return joker to the ground =====================
                    if (counter > 300)
                    {
                        if (hitbox.Y + hitbox.Width < ground)
                        {
                            accel++;
                            hitbox.Y += accel;
                        }
                        if (hitbox.Y + hitbox.Height >= ground) //reset variables after landing.
                        {
                            hitbox.Y = ground - hitbox.Height;
                            idle = true;
                            state = 0;
                            counter = 0;
                            accel = 0;
                        }
                    }       
                }
                frameTimer2--;
            }
            #endregion //flying

            //getting hit state.
            #region State 5
            if (state == 5)
            {
                counter++;
                if (counter == 30)
                {
                    counter = 0;
                    state = 0;
                    idle = true;
                }
            }
            #endregion                    

            //jumping state
            #region State 6
            if (state == 6)
            {
                counter++;
                if (!jump)  //set variables to jump up.
                {
                    jump = true;
                    upVelocity = -17;
                }
                
                hitbox.X += speed * runDir;
                hitbox.Y += upVelocity;
                
                if (hitbox.Y + hitbox.Height >= ground ) //after landing jump, reset variables.
                { 
                    upVelocity = 0;
                    hitbox.Y = ground - hitbox.Height;
                    state = 0;
                    counter = 0;
                    jump = false;
                    idle = true;
                }  
            } 
            #endregion

            //dying state
            #region State 7
            if (state == 7)
            {
                counter++;
                if (counter < 65)
                {
                    if (runDir == 1) //fly back in opposite direction.
                        hitbox.X -= 3;
                    else
                        hitbox.X += 3;
                }
            } 
            #endregion
        }            
        public void draw(SpriteBatch sb, ContentManager Content)
        {
            //load joker pictures.
            hitboxPic = Content.Load <Texture2D>("hitbox"); 
            if (runDir == 1)
                picture = Content.Load<Texture2D>("Joker");
            else
                picture = Content.Load<Texture2D>("Joker Left");

            //adjust framerate of different animations.
            if (state == 3 || state == 5 || state == 7)
                time = 5;
            else if (state == 2)
                time = 6;
            else
                time = 7;

            //roll through the frames of each animation.
            if (frameTimer == 0)
            {
                row += 1;
                frameTimer = time;
            }
            frameTimer--;

            //idle animation.
            #region State 0
            if (state == 0)
            {
                column = 0;
                if (row > 6)
                    row = 0;                  
            } 
            #endregion         

            //running animation.
            #region State 1
            if (state == 1)
            {
                column = 1;

                if (row > 8)
                    row = 0;        
            } 
            #endregion

            //punching animation.
            #region State 2
            if (state == 2)
            {
                //pictures are loaded again, to prevent joker from changing direction when punching.
                if (punchDir == 1)
                    picture = Content.Load<Texture2D>("Joker");
                else
                    picture = Content.Load<Texture2D>("Joker Left");
                column = 2;                                   
                if (row > 4)  
                    row = 0; 
            } 
            #endregion               

            //bomb animation.
            #region State 3
            if (state == 3)
            {
                column = state;
                if (row > 8)
                    row = 0;
            }

            #endregion

            //flying animation.
            #region State 4
            if (state == 4)
            {
                if (counter < 20)
                {
                    column = 4;
                    row = 0;
                }
                if (counter > 20 ) 
                {
                    column = 4;
                    row = 1;
                }
                if (counter > 300) 
                {
                    column = 2;
                    row = 2;
                }
            } 
            #endregion

            //getting hit animation.
            #region State 5
            if (state == 5)
            {
                column = state;
                if (row > 6)
                    row = 0;
            } 
            #endregion

            //jumping animation.
            #region State 6
            if (state == 6)
            {
                column = 1;
                if ((upVelocity + gravity) < 0) 
                {
                    if (row > 8)
                        row = 0;
                }
                else
                    row = 7; 
            } 
            #endregion

            //dying animation.
            #region State 7
            if (state == 7)
            {
                column = 6;
                if (row > 13)
                    row = 13;
            } 
            #endregion

            sb.Draw(picture, rect, new Rectangle(column * 80, row * 70, 80, 70), Color.White);

            //The following is used for drawing hitboxes.
            //sb.Draw(hitboxPic, punchHitbox, Color.Blue);
            //sb.Draw(hitboxPic, hitbox, Color.Green);  
        }
    }
}
