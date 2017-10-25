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
    class AIEnemy:CharacterSprite //Created By: Nick Geofroy Dimentions: 60x70
    {
        //=================================================================Fields=============================================================
        
        //Booleans for the two 'states'
        bool isAttacking = false;
        bool beingHit = false;
        //timers for the animations of various actions and misc
        int attackTimer, hitTimer, drawTimer, cooldownTimer;
        //Hitboxed to determine if the enemy should attack
        Rectangle attackingHitboxL;
        Rectangle attackingHitboxR;
        Texture2D hitboxPic; //for testing purposes
        int speed; //speed of the enemy
        SoundEffect punch; //punching sound effect
        
        //Constructor
        public AIEnemy(Texture2D aPic, Rectangle aRec, Rectangle aHitbox, int aHealth, int aspeed):base(aPic,aRec,aHitbox,aHealth)
        {
            setSpeed(aspeed);
        }
        #region Methods
        public void Draw(SpriteBatch sb)
        {
            
                //===============================================CYCLING THROUGH IMAGES=====================================
            //depending on the column of the sprite sheet, it will go back to image 1 after all of the images have been cycled through
                if (column == 0)
                {
                    if (row >= 12)
                        row = 0;
                }
                if (column == 1)
                {
                    if (row >= 8)
                        row = 0;
                }
                if (column == 2)
                {
                    if (row >= 7)
                        row = 0;
                }
                if (column == 3)
                {
                    if (row >= 7)
                        row = 0;
                }
            //Sets the hitbox to change its size based on the size of the image
                hitbox.Width = 4 * rect.Width / 10;
                hitbox.Height = rect.Height;
                drawTimer++;
               // sb.Begin(); (code for testing purposes)
                //sb.Draw(hitboxPic, hitbox, Color.Green); //ian
                //sb.Draw(hitboxPic, attackingHitboxL, Color.Purple);
                sb.Draw(picture, rect, new Rectangle(column * 60, row * 70, 60, 70), Color.White); //draes the penguin in the specified animation image
              //  sb.End();
                if (drawTimer % 7 == 0) //changes the picture every 7 ticks to make the animations slower
                {
                    row++; //cycles through images
                    drawTimer = 0;
                }
            

        }
        public void Update(Player player, ContentManager content)
        {
            
                hitboxPic = content.Load<Texture2D>("hitbox"); //ian
            //sets the hitbox location to always be in the same spot in relation to the visible image
                hitbox.X = rect.X + (3 * hitbox.Width / 4);
                hitbox.Y = rect.Y + 5;
            //creates the attacking area's on either side of the penguin
                attackingHitboxR = new Rectangle(hitbox.X + hitbox.Width, hitbox.Y, hitbox.Width / 4, hitbox.Height);
                attackingHitboxL = new Rectangle(hitbox.X - (hitbox.Width / 4), hitbox.Y, hitbox.Width / 4, hitbox.Height);

                //===============================================MOVING=====================================================
                if (((player.getHitbox().X + player.getHitbox().Width) < hitbox.X) && !isAttacking && !beingHit) //if the thug is neither attacking or being hit, and is to the right of the player, moves the thug left and makes him face left
                {
                    column = 1;
                    rect.X -= speed;
                    picture = content.Load<Texture2D>("Thug left");
                    cooldownTimer = 40; //increases the attack cooldown timer so that the thug will attack sooner after moving instead of having to wait a long time

                }
                else if (((player.getHitbox().X) > hitbox.X + hitbox.Width) && !isAttacking && !beingHit) //if the thug is to the left of batman and isn't in any action, moves it right and changes the image to face right
                {
                    column = 1;
                    hitbox.X += 1;
                    rect.X += speed;
                    picture = content.Load<Texture2D>("Thug");
                    cooldownTimer = 40;
                }
                else if (!isAttacking & !beingHit) //if the thug is not moving
                {
                    column = 0; //changes the animation to idle
                }

                //Initiate attack phase if not being hit by the enemy and the hitbox for attacking collides with batman and the cooldown timer is > 60
                if ((attackingHitboxL.Intersects(new Rectangle(player.getHitbox().X + player.getHitbox().Width - 10, player.getHitbox().Y, 15, 70)) || attackingHitboxR.Intersects(new Rectangle(player.getHitbox().X, player.getHitbox().Y, 15, 70))) && !beingHit && !isAttacking && cooldownTimer > 60)
                {
                    isAttacking = true;
                    column = 2;//starts playing the attacking animation
                    row = 0;
                    cooldownTimer = 0; //resets the attack cooldown
                    punch = content.Load<SoundEffect>("Weak Punch"); //loads the punching sound effefct
                }

                //=============================================ATTACKING===================================================
                if (isAttacking)
                {
                    column = 2;//insert column of the attacking animation here
                    attackTimer++;

                    if (attackTimer == 7 * 3) //at the end of the attacking animation
                    {
                        if (attackingHitboxR.Intersects(player.getHitbox()) || attackingHitboxL.Intersects(player.getHitbox())) //if the two hitboxes are still intersecting after the animation is complete
                        {
                            player.TakeDamage(10); //damages the player by 10hp and plays the punch sound for the first punch
                            punch.Play(0.3f, 0, 0);
                        }


                    }
                    else if (attackTimer == 7 * 8)
                    {
                        if (attackingHitboxR.Intersects(player.getHitbox()) || attackingHitboxL.Intersects(player.getHitbox()))
                        {
                            player.TakeDamage(10); //damages the player by 10hp and plays the punch sound for the second punch
                            punch.Play(0.3f, 0, 0);
                        }
                        attackTimer = 0; //resets the attack timer to 0 in order to stop the enemy from constantly attacking
                        isAttacking = false; //ends the attacking animation
                    }
                }
                else
                {
                    cooldownTimer++; //if not attacking, increases the cooldown timer
                }
                //=============================================BEING HIT===================================================
                if (beingHit) //when the AI has been hit by a player
                {
                    column = 3; //plays the being hit animation
                    hitTimer++; //increases timer that is used for visual effects
                    if (hitTimer > 21 && hitTimer < 35) //for the portion of the animation the thug is stepping back
                    {
                        if (hitbox.X > player.getHitbox().X + (player.getHitbox().Width / 2)) //if the thug is on the right of batman, recoils right otherwise, recoils left
                            rect.X+= speed;
                        else
                            rect.X-= speed;
                    }
                    if (hitTimer == 7 * 7) //at the end of the being hit animation
                    {
                        hitTimer = 0; //resets thehit timer
                        beingHit = false; //ends the being hit state
                    }
                }
            
        }
        public void takeDamage(int damage) //used by the player class to damage the thug
        {
            
            beingHit = true; //sets the being hit state to true
            health -= damage; //decreases the thug's health
            row = 0;
            isAttacking = false; //prevents the thug from attacking
            attackTimer = 0; //resets the attack timer so that the thug doesn't instantly attack again
            

        }
        
        #endregion
        #region Getters and Setters
        //Getters + Setters for speed, all other variables don't need to be accessed or are in the base class
        public int getSpeed()
        {
            return speed;
        }
        public void setSpeed(int aSpeed)
        {
            speed = aSpeed;
        } 
        #endregion
    }
}
