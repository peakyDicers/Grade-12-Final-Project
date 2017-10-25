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

namespace Project_Mayhem
{
    class PenguinBot:CharacterSprite //Created By: Nick Geofroy
    {
        //fields
        bool exploding = false;
        int counter;
        int delay;
        Color colour = Color.White;
        SoundEffect explosionSound; //sound effect for when the penguins explode

       public PenguinBot(Texture2D aPic, Rectangle aRec, Rectangle aHitbox, int aHealth):base(aPic,aRec,aHitbox,aHealth)
        {
            
        }

        //Methods
        public void Update(ContentManager content, Player player, List<AIEnemy> enemies)
       {
           
           counter++;
            //places the image rectange in relation to the hitbox and also resized it to fit the hitbox
           rect.X = hitbox.X - 10;
           rect.Y = hitbox.Y;
           rect.Width = hitbox.Width*2;
           if (counter == 7) //conter for animations that gets reset after 7 ticks
               counter = 0;
           if (((player.getHitbox().X + (player.getHitbox().Width/2)) < hitbox.X) && !exploding) //if the penguin isn't in its exploding state, and it's to the right og the player, moves it left
           {
               hitbox.X -= 1;
           }
           else if (((player.getHitbox().X) > hitbox.X + (hitbox.Width/2))&& !exploding) //if the penguin is to the left of the player, moves it right
           {
               hitbox.X += 1;
           }
            //Running (moving) animation
           if (!exploding && counter == 6) //if the penguin bot is not in the exploding state
           {
               //CODE FOR ANIMATION
               column++; 
               if (row == 0 && column == 5) //once the end of the first row of pictures ends, goes to the next one
               {
                   row = 1;
                   column = 0;
               }
               if (row == 1 && column == 3) //once the second row of animations finishes, resets back to the first one
               {
                   row = 0;
                   column = 0;
               }
               if (hitbox.Intersects(player.getHitbox())&& counter == 6) //if the penguin intersects with the player begins to explode and turns the image red
               {
                   exploding = true;
                   colour = Color.Red;
               }
           }
           if (exploding) //if in the exploding mode, adds one to the delay timer
           {
               delay ++;
           }
           if (delay == 60) //when the delay equals 60, (the explosion) changes the colour back to white for a regular explosion and resets the column
           {
               column = 0;
               colour = Color.White;
           }
            if (delay >= 60) //when the delay is greater than 60
            {
                row = 3; //sets it to explosion animation row
                column++; //increases the column to paly animation
                
                if (column == 4) //if exploding
                {
                    explosionSound = content.Load<SoundEffect>("explosionSound"); //loads plays the explosion sound effect
                    explosionSound.Play(0.5f, 0, 0);
                    if (hitbox.Intersects(player.getHitbox())) //if the player is in the explosion radius
                    {
                        player.TakeDamage(30); //deals 30 damage to the player
                    }
                    for (int i = 0; i < enemies.Count; i++) //checks through all of the enemies
                    {
                        if (hitbox.Intersects(enemies[i].getHitbox())) //if the enemy is in the explosion radius
                        {
                            enemies[i].takeDamage(60); //deals 60 damage to the enemy
                            player.GainPoints(1000); //player gains 1000 points
                        }
                    }
                }
            }
            
       }
        public void Draw(SpriteBatch sb)
        {
            if (health > 0) //if not dead
            {
                sb.Draw(picture, rect, new Rectangle(column * 110, row * 65, 110, 70), colour); //draws the penguin bot
            }
        }
    }
}
