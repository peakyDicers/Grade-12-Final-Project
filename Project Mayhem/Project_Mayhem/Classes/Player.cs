//Alex green
//16/06/2016
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

namespace Project_Mayhem        //ian stop changing things just so you can paste your name all over my class 
                                //get xd'd
{
    class Player : CharacterSprite        //Created by Alexander Green,  02/06/2016
    {
        //constants and timers
        int ground; //constant for what y value batman has at the ground
        int speed; //value for how far you can move each tick
        int atktimer; //timer for time left for next attack
        int atktimer2; //timer for time left for third attack
        int downtime; //holds time left until you can attack
        int frametimer; //timer for holding when the sprite will change
        int framelength; //changes how long each frame lasts
        int iFrames; //can still take damage, can't be stunned

        //states of batman
        bool facingRight = true; //holds if facing right
        int yvelocity; //holds how fast it's going up/down
        bool canmoveforwards; //is false if an enemy is directly in front of you
        Rectangle atkRect; //rectangle holding hitbox for attacks

        //game progression info
        bool canContinue; //holds whether batman can move forwards or not (is he in battle)
        int distanceCovered; //how far batman has travelled, int used for triggering events

        //projectiles
        List<Batarang> batarang = new List<Batarang>(); //list of projectiles
        bool isExplosive; //determines whether batarang spawned is explosive or not
        int ammo; //holds how many explosive batarangs you have

        //sounds
        SoundEffect weakpunch, strongpunch, legsweep, kick, throwing; //sound effects for each attack
        bool isplayingsound; //if true don't play the sound again (so if an attack hits multiple enemies, doesn't blur the sound together)

        //points
        int score, combo, combotimer;

        //ian 
        Texture2D hitboxPic; //picture to draw hitbox for testing

        public Player(Texture2D aPic, Rectangle aRec, Rectangle aHitbox, int aHealth, int aground, ContentManager content):base(aPic,aRec,aHitbox,aHealth)
        {
            ground = aground - hitbox.Height + 10;
            speed = 6;
            frametimer = 3; 
            framelength = 3;
            ammo = 3;
            weakpunch = content.Load<SoundEffect>("Weak Punch");
            strongpunch = content.Load<SoundEffect>("Strong Punch");
            legsweep = content.Load<SoundEffect>("Sweep Kick");
            kick = content.Load<SoundEffect>("Kick");
            throwing = content.Load<SoundEffect>("woosh");
        }

        //getters and setters
        public int getDistance()
        {
            return distanceCovered;
        }
        public bool getContinue()
        {
            return canContinue;
        }
        public List<Batarang> getBatarang()
        {
            return batarang;
        }
        public void setContinue(bool acontinue)
        {
            canContinue = acontinue;
        }
        public int getAmmo()
        {
            return ammo;
        }
        public void setAmmo(int aAmmo)
        {
            ammo = aAmmo;
        }
        public int getScore()
        {
            return score;
        }
        public int getCombo()
        {
            return combo;
        }

        public void Draw(SpriteBatch sb, SpriteFont font) //draws batman and batarangs
        {
            
            for (int i = 0; i < batarang.Count; i++) //draw all batarangs
                batarang[i].Draw(sb);   
            if (facingRight)  //picture isn't quite centered, slight differences when centering around hitbox depending on direction
            {
                rect.X = hitbox.X - 65;
                rect.Y = hitbox.Y -60;
            }else
            {
                rect.X = hitbox.X - 55;
                rect.Y = hitbox.Y - 60;
            }
            //take picture from grid of pictures, each picture is 80x80, each column is a different action, row holds how progressed the action is
            //sb.Draw(hitboxPic, atkRect, Color.Red); //draw hitboxs to see them
            //sb.Draw(hitboxPic, hitbox, Color.Blue);
            sb.Draw(picture, rect, new Rectangle(column * 80, row * 80, 80, 80), Color.White); //each picture is 80 by 80, finds its location based on current action
            if (column == 10 && row == 10)
                sb.DrawString(font, "Press start to return to the main menu", new Vector2(500 - font.MeasureString("Press start to return to the main menu").X / 2, 200), Color.White);
        }
        public void Update(GamePadState pad1, GamePadState oldpad1, ContentManager Content, KeyboardState kb, List<AIEnemy> enemies,List<Joker> joker, Penguin penguin)
        {
            //ian
            hitboxPic = Content.Load<Texture2D>("hitbox");
            //not ian
            frametimer--; //decrease timers
            atktimer--;
            atktimer2--;
            if (frametimer == 0) //when time runs out move on to the next frame of the action
            {
                row += 1;
                frametimer = framelength;
            }
            if (downtime >0)
                downtime--;
            if (iFrames > 0)
                iFrames--;
            if (combotimer > 0) //if you run out of time to continue your combo, combo becomes 0
                combotimer--;
            else
                combo = 0;
            //*****************************column holds which action is currently being done ******************************

            //----------------------------------------idle------------------------------column 0
            #region idle
            if (column == 0)
            {
                //if x is pressed while you have at least a 10 hit combo, combo resets and you do your finisher
                if (pad1.Buttons.X == ButtonState.Pressed && oldpad1.Buttons.X == ButtonState.Pressed && combo >= 10) 
                {
                    combo = 0;
                    row = 0;
                    column = 12;
                }
                //if B is pressed, punch. Different punch depending on recent attacks
                else if (pad1.Buttons.B == ButtonState.Pressed && downtime == 0 && oldpad1.Buttons.B == ButtonState.Released)
                {
                    if (atktimer2 > 0) //start third punch if second punch happened recently
                    {
                        column = 5;
                        row = 0;
                        framelength = 4; //increase the length of each frame so the punch lasts longer (necessary cause less frames)
                    }
                    else if (atktimer > 0) //if first punch was recent then second punch
                    {
                        column = 4;
                        row = 0;
                    }
                    else 
                    {
                        column = 3;
                        row = 0;
                    }
                }
                //if Y is pressed, kick begins
                else if (pad1.Buttons.Y == ButtonState.Pressed && downtime == 0 && oldpad1.Buttons.Y == ButtonState.Released)
                {
                    if (atktimer > 0) //if first punch or kick occured recently then do second kick
                    {
                        column = 6;
                        row = 0;
                        framelength = 5; //increase time spent on each frame to increase how long action lasts
                    }
                    else //haven't attacked in a while so do first kick
                    {
                        column = 7;
                        row = 0;
                        framelength = 4;
                    }
                }
                //if LT is pressed and you have ammo, explosive batarang is thrown 
                else if (pad1.Triggers.Left > 0.1 && oldpad1.Triggers.Left < 0.1 && ammo > 0) 
                {
                    column = 8;
                    row = 0;
                    framelength = 2; //action has a lot of frames so decrease time spent on each to reduce time spent in action
                    isExplosive = true;
                    ammo--;
                }
                //if RT is pressed, batarang is thrown 
                else if (pad1.Triggers.Right > 0.1 && oldpad1.Triggers.Right < 0.1) 
                {
                    column = 8;
                    row = 0;
                    framelength = 2; //action has a lot of frames so decrease time spent on each to reduce time spent in action
                    isExplosive = false;
                }
                //if A is pressed, jump
                else if (pad1.Buttons.A == ButtonState.Pressed && oldpad1.Buttons.A == ButtonState.Released) 
                {
                    column = 2;
                    row = 0;
                    framelength = 6;
                    frametimer = framelength;
                }
                //if thumbstick is moved started moving
                else if (pad1.ThumbSticks.Left.X != 0 || kb.IsKeyDown(Keys.Left) || kb.IsKeyDown(Keys.Right)) 
                {
                    column = 1;
                    row = 0;
                }

                //if past last frame, return to first
                if (row > 15)
                    row = 0;
            }
            #endregion
            //----------------------------------------moving---------------------------column 1
            #region moving
            else if (column == 1)
            {
                #region move
                if (pad1.ThumbSticks.Left.X > 0 || kb.IsKeyDown(Keys.Right))  //moving right
                {
                    if (!facingRight) //if it was previously facing left change the picture
                    {
                        picture = Content.Load<Texture2D>("Batman");
                        facingRight = true;
                    }
                    canmoveforwards = true; //if moving forwards would put you in an enemy you can't move forwards
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (hitbox.X + hitbox.Width + 3 > enemies[i].getHitbox().X && hitbox.X + hitbox.Width + 3 < enemies[i].getHitbox().X + enemies[i].getHitbox().Width)
                            canmoveforwards = false;
                    }
                    if (canContinue)
                    {
                        if (hitbox.X + 4 + hitbox.Width <= 800) //if not fighting it stops you at a certain point and scrolls background instead
                        {
                            hitbox.X += speed;
                        }
                        else
                        {
                            distanceCovered += 4;
                        }
                    }
                    else if (canmoveforwards && hitbox.X + 14 + hitbox.Width <= 1000) //in fight can move across screen for more room to fight
                    {
                        hitbox.X += speed;
                    }
                }
                else if (pad1.ThumbSticks.Left.X < 0 || kb.IsKeyDown(Keys.Left))
                {
                    if (facingRight) //if it was previously facing right switch pictures to be left
                    {
                        picture = Content.Load<Texture2D>("Batman left");
                        facingRight = false;
                    }
                    canmoveforwards = true;
                    for (int i = 0; i < enemies.Count; i++) //if moving would put you in an enemy, you can't move forwards
                    {
                        if (hitbox.X - 3 < enemies[i].getHitbox().X + enemies[i].getHitbox().Width && hitbox.X - 3 > enemies[i].getHitbox().X)
                            canmoveforwards = false;
                    }
                    //if (hitbox.X - 2 < joker.getHitbox().X + joker.getHitbox().Width)
                    //  canmoveforwards = false;
                    if (canmoveforwards && hitbox.X - 14 >= 0)
                    {
                        hitbox.X -= speed;
                    }
                } 
                #endregion
                //if x is pressed while you have at least a 10 hit combo, combo resets and you do your finisher
                if (pad1.Buttons.X == ButtonState.Pressed && oldpad1.Buttons.X == ButtonState.Pressed && combo >= 10)
                {
                    combo = 0;
                    row = 0;
                    column = 12;
                }
                //if B is pressed, punch 1 begins
                else if (pad1.Buttons.B == ButtonState.Pressed && downtime == 0 && oldpad1.Buttons.B == ButtonState.Released) 
                {
                    if (atktimer2 > 0) //chooses punch based on recent actions, punch 3
                    {
                        column = 5;
                        row = 0;
                        framelength = 4;
                    }
                    else if (atktimer > 0)//punch 2
                    {
                        column = 4;
                        row = 0;
                    }
                    else //punch 1
                    {
                        column = 3;
                        row = 0;
                    }
                }
                //if Y is pressed, kick begins
                else if (pad1.Buttons.Y == ButtonState.Pressed && downtime == 0 && oldpad1.Buttons.Y == ButtonState.Released)
                {
                    if (atktimer > 0) //choose kick based on recent actions, kick 2
                    {
                        column = 6;
                        row = 0;
                        atktimer = 0;
                        framelength = 4;
                    }
                    else //kick 1
                    {
                        column = 7;
                        row = 0;
                        framelength = 4;
                    }
                }
                //if RT is pressed, batarang is thrown 
                else if (pad1.Triggers.Right > 0.1 && oldpad1.Triggers.Right < 0.1) 
                {
                    column = 8;
                    row = 0;
                    framelength = 2;
                    isExplosive = false;
                }
                //if LT is pressed and you have ammo, explosive batarang is thrown 
                else if (pad1.Triggers.Left > 0.1 && oldpad1.Triggers.Left < 0.1 && ammo > 0) 
                {
                    column = 8;
                    row = 0;
                    framelength = 2;
                    isExplosive = true;
                    ammo--;
                }
                //if A is pressed, jump
                else if (pad1.Buttons.A == ButtonState.Pressed && oldpad1.Buttons.A == ButtonState.Released) 
                {
                    column = 2;
                    row = 0;
                    framelength = 6;
                    frametimer = framelength;

                }
                else if (pad1.ThumbSticks.Left.X == 0) //if stop moving then idle again
                {
                    column = 0;
                    row = 0;
                }

                if (row > 21) //goes back to 4 for more seemless looping
                    row = 4;
            } 
            #endregion
            //----------------------------------------jumping---------------------------column 2
            #region jumping
            else if (column == 2)
            {
                if (row == 0) //when you first jump set initial velocity
                    yvelocity = 20;

                if (pad1.ThumbSticks.Left.X > 0)
                {
                    if (!facingRight) //if it was previously facing left change picture to right
                    {
                        picture = Content.Load<Texture2D>("Batman"); 
                        facingRight = true; 
                    }
                    if (canContinue)
                    {
                        if (hitbox.X + 4 + hitbox.Width <= 800) //if not fighting scroll screen at this point
                        {
                            hitbox.X += 4;
                        }
                        else
                        {
                            distanceCovered += 4;
                        }
                    }
                    else if (hitbox.X + 4 + hitbox.Width <= 1000) //if fighting allow moving for more room
                    {
                        hitbox.X += 4;
                    }
                }
                if (pad1.ThumbSticks.Left.X < 0)
                {
                    if (facingRight) //if it was previously facing right change picture to left facing batman
                    {
                        picture = Content.Load<Texture2D>("Batman left");
                        facingRight = false;
                    }
                    if (hitbox.X - 4 >= 0)
                    {
                        hitbox.X -= 4;
                    }
                }
                if (row < 3) //if in the first half of the jump move upwards
                {
                    hitbox.Y -= yvelocity; 
                    if (row < 4) //decreases the amount you go up by each tick, decreases more early on.
                        yvelocity -= 3;
                    else yvelocity -= 1;
                    if (yvelocity < 1) //can't decrease velocity below 0
                        yvelocity = 1;
                }
                else //if in second half of jump start going back down
                {
                    hitbox.Y += yvelocity;
                    if (row > 11) //increase velocity, acceleration is higher later in jump
                        yvelocity += 3;
                    else yvelocity+=1;
                }
                if (hitbox.Y > ground) //can't go through ground
                    hitbox.Y = ground;
                if (row > 5) //when it ends return to idle
                {
                    column = 0;
                    row = 0;
                    framelength = 3;
                }

            } 
            #endregion
            //----------------------------------------punch1----------------------------column 3
            #region punch1
            else if (column == 3)
            {
                if (row == 3 && frametimer == 3) //frame 3 is the most outstretched so do the intersection check there
                {
                    isplayingsound = false;
                    framelength = 2;
                    if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                        atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 50, 110);
                    else atkRect = new Rectangle(hitbox.X - 40, hitbox.Y, 50, 110);
                    for (int i = 0; i < enemies.Count; i++) 
                    {
                        if (Attack(atkRect, 10, enemies[i]) && !isplayingsound) //if enemy is hit play sound, stop checking for enemies
                        {
                            weakpunch.Play(0.5f,0,0);
                            isplayingsound = true;
                        }
                    }
                    for (int i = 0; i < joker.Count; i++)
                        if (joker[i].getHealth() > 0) //check if joker is alive so you can't hit his dead body for points
                            if (Attack(atkRect, 10, joker[i]) && !isplayingsound) {
                                weakpunch.Play(0.5f,0,0);
                                isplayingsound = true;
                            }
                    if (Attack(atkRect, 10, penguin) && !isplayingsound) {
                        weakpunch.Play(0.5f,0,0);
                        isplayingsound = true;
                    }
                }

                if (row > 10) //return to idle
                {
                    framelength = 3;
                    atktimer = 40; //you have 40 ticks (2/3rds of a second) in which the next punch or kick will be different
                    column = 0;
                    row = 0;
                }
            } 
            #endregion
            //----------------------------------------punch2--------------------------column 4
            #region punch2
            else if (column == 4)
            {
                if (row == 3 && frametimer == 3) //on frame 3 check for intersections with enemies
                {
                    isplayingsound = false;
                    framelength = 2;
                    if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                        atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 60, 110);
                    else atkRect = new Rectangle(hitbox.X - 50, hitbox.Y, 60, 110);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (Attack(atkRect, 15, enemies[i]) && !isplayingsound) //run attack method, if they intersect play sound
                        {
                            weakpunch.Play(0.5f,0,0);
                            isplayingsound = true;
                        }
                    }
                    for (int i = 0; i < joker.Count; i++)
                        if (joker[i].getHealth() > 0)
                            if (Attack(atkRect, 15, joker[i]) && !isplayingsound) {
                                weakpunch.Play(0.5f, 0, 0);
                                isplayingsound = true;
                            }
                    if (Attack(atkRect, 15, penguin) && !isplayingsound) {
                        weakpunch.Play(0.5f,0,0);
                        isplayingsound = true;
                    }
                }

                if (row > 9) //return to idle
                {
                    atktimer2 = 40; // two thirds of a second in which next punch will be punch3
                    column = 0;
                    row = 0;
                    framelength = 3;
                }
            } 
            #endregion
            //----------------------------------------punch3--------------------------column 5
            #region punch3
            else if (column == 5)
            {
                if (row == 4 && frametimer == 3) //check for intersections with enemies
                {
                    isplayingsound = false;
                    if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                        atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 70, 110);
                    else 
                        atkRect = new Rectangle(hitbox.X - 60, hitbox.Y, 70, 110);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (Attack(atkRect, 25, enemies[i]) && !isplayingsound) //if enemy is hit play sound
                        {
                            strongpunch.Play(0.5f,0,0);
                            isplayingsound = true;
                        }
                    }
                    for (int i = 0; i < joker.Count; i++)
                        if (joker[i].getHealth() > 0)
                            if (Attack(atkRect, 25, joker[i]) && !isplayingsound)
                                strongpunch.Play(0.5f, 0, 0);
                    if (Attack(atkRect, 25, penguin) && !isplayingsound)
                        strongpunch.Play(0.5f,0,0);
                }

                if (row > 13) //if at end of action, return to idle
                {
                    column = 0;
                    row = 0;
                    downtime = 5; //can't attack again for another 5 ticks
                    framelength = 3; //set frame length back to normal
                }
            } 
            #endregion
            //----------------------------------------kick----------------------------column 6
            #region kick
            else if (column == 6)
            {
                if (row == 2 && frametimer == 3) //check for intersections on frame 2
                {
                    isplayingsound = false;
                    if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                        atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 70, 110);
                    else atkRect = new Rectangle(hitbox.X - 60, hitbox.Y, 70, 110);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        if (Attack(atkRect, 20, enemies[i]) && !isplayingsound) //if enemy is hit play sound
                        {
                            kick.Play(0.2f, 0, 0);
                            isplayingsound = true;
                        }
                    }
                    for (int i = 0; i < joker.Count; i++)
                        if (joker[i].getHealth() > 0)
                            if (Attack(atkRect, 20, joker[i]) && !isplayingsound)
                                kick.Play(0.2f, 0, 0);
                    if (Attack(atkRect, 20, penguin) && !isplayingsound)
                        kick.Play(0.2f,0,0);
                    framelength = 4;
                }

                if (row > 9) //return to idle
                {
                    column = 0;
                    row = 0;
                    downtime = 5; //5 tick downtime where you can't attack
                    framelength = 3;
                }
            } 
            #endregion
            //----------------------------------------leg sweep------------------------column 7
            #region leg sweep
            else if (column == 7)
            {
                if (row == 1 && frametimer == 1) //play sound early on when sweeping motion starts
                    legsweep.Play(0.2f,0,0);
                if (row == 3 && frametimer == 3) //check for intersections
                {
                    if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                        atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 50, 110);
                    else atkRect = new Rectangle(hitbox.X - 40, hitbox.Y, 50, 110);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        Attack(atkRect, 15, enemies[i]);
                    }
                    for (int i = 0; i < joker.Count; i++)
                        if (joker[i].getHealth() > 0)
                            Attack(atkRect, 15, joker[i]);
                    Attack(atkRect, 15, penguin);
                }

                if (row > 8) //return to idle
                {
                    atktimer = 40; //if you attack again before this reaches 0 it will be the second variant
                    column = 0;
                    row = 0;
                    framelength = 3;
                }
            } 
            #endregion
            //----------------------------------------batarang-------------------------column 8
            #region batarang
            else if (column == 8)
            {
                if (row == 6) //spawn a new batarang into the list
                {
                    throwing.Play(0.4f,0,0); //throwing sound when batarang spawns
                    if (isExplosive) //if explosive, different imagem different bool
                        batarang.Add(new Batarang(Content.Load<Texture2D>("Explosive batarang"), new Rectangle(hitbox.X + 35, hitbox.Y + 30, 15, 10), 10, facingRight, isExplosive, Content));
                    else
                        batarang.Add(new Batarang(Content.Load<Texture2D>("Batarang"), new Rectangle(hitbox.X + 35, hitbox.Y + 30, 15, 10), 10, facingRight, isExplosive, Content));
                }
                if (row > 18) //return to idle
                {
                    column = 0;
                    row = 0;
                    framelength = 3;
                }
            } 
            #endregion
            //----------------------------------------dying---------------------------column 9/10
            #region dying
            else if (column == 9) //dying was split into two columns due to size constraints
            {
                GamePad.SetVibration(PlayerIndex.One, 1, 1); //super vibrating
                if (row > 19) //switch to DYING 2 ELECTRIC BOOGALOO
                {
                    column = 10;
                    row = 0;
                }
            }
            //--------------------------------------dying part 2--------------------column 10
            if (column == 10)
            {
                GamePad.SetVibration(PlayerIndex.One, 0, 0); //stop vibrating
                if (row == 10) //stop changing pictures
                {
                    frametimer = -1; //frame timer is negative so it can never go to 0, therefore picture stops changing
                    if (kb.IsKeyDown(Keys.R)) //CHEATING revive
                        Revive();
                }
            } 
            #endregion
            //----------------------------------------getting hit---------------------------column 11
            #region getting hit
            else if (column == 11)
            {
                GamePad.SetVibration(PlayerIndex.One, 0.7f, 0.7f); 
                if (hitbox.Y < ground) //if you're in midair move down slowly
                {
                    hitbox.Y += 2;
                }
                if (row == 6) //back to idle
                {
                    column = 0;
                    row = 0;
                    hitbox.Y = ground; //at the end if you're still in midair just drop to the ground
                    GamePad.SetVibration(PlayerIndex.One, 0, 0);
                }
            } 
            #endregion
            //----------------------------------------FINISHER------------------------------column 12
            #region FINISHER
            else if (column == 12)
            {
                if (row < 6)
                {
                    hitbox.Y--; //go up slightly
                    if (row == 4 && frametimer == 1)
                    {
                        isplayingsound = false;
                        if (facingRight) //creates a hitbox infront of batman, approx the size of attack
                            atkRect = new Rectangle(hitbox.X + 40, hitbox.Y, 30, 110);
                        else atkRect = new Rectangle(hitbox.X - 30, hitbox.Y, 40, 110);
                        //for each enemy, check if they're hit with method
                        for (int i = 0; i < enemies.Count; i++) 
                        {
                            if (Attack(atkRect, 100, enemies[i])) //if hit make sound
                                if (!isplayingsound)
                                {
                                    strongpunch.Play(0.5f,0,0);
                                    isplayingsound = true;
                                }
                        }
                        for (int i = 0; i < joker.Count; i++)
                            if (Attack(atkRect, 50, joker[i]) && joker[i].getHealth() > 0)
                                if (!isplayingsound)
                                    strongpunch.Play(0.5f,0,0);
                        if (Attack(atkRect, 50, penguin))
                            if (!isplayingsound)
                                strongpunch.Play(0.5f,0,0);
                        combo = 0; //to prevent people from spamming this, it resets combo to 0
                    }
                }
                else if (row > 9) //after peak of jump start going down again
                {
                    hitbox.Y++;
                    if (hitbox.Y > ground)
                        hitbox.Y = ground;
                }
                if (row > 15) //return to idle
                {
                    column = 0;
                    row = 0;
                }
            } 
            #endregion
            for (int i = 0; i < batarang.Count; i++)
                batarang[i].Update(enemies, joker, penguin, batarang, i);
        }

        public void GainPoints(int points) //method for adding points
        {
            score += points;
        }
        public void Heal(int heal) //method for gaining health
        {
            if (health + heal > 100) //max health is 100, if it would heal higher just max health
                health = 100;
            else
                health += heal;
        }
        public void GainAmmo() //gain an explosive batarang if not already full
        {
            if (ammo < 3)
                ammo++;
        }
        public void TakeDamage(int damage) //if and enemy attack hits you
        {
            if (column <9) //if you are in a vulnerable action (not already getting hit/dead/in finisher attack)
            {
                health -= damage;
                if (health <= 0) //if you have no health left, switch to dying action
                {
                    hitbox.Y = ground;
                    column = 9;
                }
                else if (iFrames == 0)
                {
                    frametimer = 4; //if still alive switch to getting hit animation
                    column = 11;
                    row = 0;
                    iFrames = 20;
                }
            }
        }
        public bool Attack(Rectangle hitbox, int damage, AIEnemy enemy) //check and return whether you hit an enemy
        {
            if (enemy.getHitbox().Intersects(hitbox))
            {
                combo++; //increase your combo
                score += 50 + combo * 20; //increase score by more if combo is high
                combotimer = 120; //you have 2 minutes to attack again before combo resets
                enemy.takeDamage(damage); //call enemy method to take damage
                return true;
            }
            else
                return false;
        }
        public bool Attack(Rectangle hitbox, int damage, Joker joker) //checks and returns whether you hit joker
        {


            if (joker.getHitbox().Intersects(hitbox))
            {
                //increases combo count, score more with more combo, 2 minutes to continue combo
                combo++; 
                score += 50 + combo * 20;
                combotimer = 120;
                joker.takeDamge(damage); //method for joker to take damage
                return true;
            }
            else
                return false;

        }
        public bool Attack(Rectangle hitbox, int damage, Penguin penguin) //same for penguin
        {
            if (penguin.getHitbox().Intersects(hitbox))
            {
                combo++;
                score += 50 + combo * 20;
                combotimer = 120;
                penguin.TakeDamage(damage);
                return true;
            }
            else
                return false;
        }
        public void Revive() //revive method for testing
        {
            health = 100; //resets health and returns to idle
            column = 0;
            row = 0;
            framelength = 3;
            frametimer = 3;
        }
        public void Deactivate(int count) //method for others to destroy batarangs
        {
            batarang[count].Deactivate(); 
        }


        public void testDistance(int dis) //method to set how far batman has travelled
        {
            distanceCovered = dis;
        }
    }
}
