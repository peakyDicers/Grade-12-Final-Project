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
    class Batarang:Projectile //Alex
    {
        int frametimer; //timer for when to switch pictures
        bool active; //set to false when it should delete itself
        int damage; //how much damage it should do on collision
        SoundEffect hitsound; //sound effect for when it hits

        public Batarang(Texture2D aPic, Rectangle aRec, int spd, bool right, bool isExplosive, ContentManager content):base(aPic,aRec,spd,right)
        {
            frametimer = 3; //3 ticks between picture changes
            active = true; //active so it doesn't delete itself
            if (isExplosive) //if it is an explosive one it does more damage and has an explosive sound on hit
            {
                damage = 50;
                hitsound = content.Load<SoundEffect>("explosionSound");
            }
            else //if not explosive it does less damage and makes a normal sound on hit
            {
                damage = 10;
                hitsound = content.Load<SoundEffect>("Weak Punch");
            }
        }

        public void Update(List<AIEnemy> enemies, List<Joker> joker, Penguin penguin, List<Batarang> batarang, int count)
        {

            if (frametimer == 0) 
            {//when it hits 0 switch to the next picture and reset counter
                frametimer = 3;
                if (row == 3)
                    row = 0;
                else
                    row++;
            }
            frametimer--;
            move(); //moves batarang speed amount in direction it's travelling in
            for (int i = 0; i < joker.Count; i++) //checks collisions with joker
            {
                if (rect.Intersects(joker[i].getHitbox()))
                {
                    joker[i].takeDamge(damage / 2); //bosses take half damage
                    hitsound.Play(0.2f, 0, 0); //play hitsound
                    active = false;//no longer active, will be deleted at end of update

                }
            }
            if (rect.Intersects(penguin.getHitbox())) //check collision with penguin
            {
                penguin.TakeDamage(damage / 2); //bosses take half damage
                hitsound.Play(0.2f, 0, 0);
                active = false; 
            }
            for (int i = 0; i < enemies.Count; i++) //check collisions with enemies
            {
                if (rect.Intersects(enemies[i].getHitbox()) && active) //if batarang hasn't already hit someone
                {
                    enemies[i].takeDamage(damage);
                    hitsound.Play(0.2f, 0, 0);
                    active = false;
                    break; //break out of for loop so it doesn't keep checking for no reason
                }
            }
            if (rect.X < -15 || rect.X > 1000) //if it leaves screen it is inactive
                active = false;

            if (!active) //if it's inactive remove itself
                if (count < batarang.Count()) //double check that it hasn't already been removed (for some reason it would sometimes try to remove itself a second time and crash the game)
                    batarang.RemoveAt(count);
        }

        public void Deactivate() //method for other classes to use to destroy batarangs
        {
            active = false;
        }
    }
}
