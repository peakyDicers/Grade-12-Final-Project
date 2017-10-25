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
    class Penguin:CharacterSprite //Created By: Nick  Geofroy
    {

        int drawTimer;
        int atkCooldown = 120;
        int shootTimer;
        bool firedBullet;
        bool right;
        bool bulletRight;
        bool moving;
        int counter;
        int spwnEnemiesTimer = 5000;
        int stunlocked;
        Random rndm = new Random();
        Player player;
        Rectangle rAtkBx;
        Rectangle lAtkBx;
        SoundEffect shootEffect;
        List<PenguinBot> bots = new List<PenguinBot>();

        Rectangle bullet;
        Texture2D hitboxPic;
        

        //btw his pictures are 100 pixels wide and 60 pixels tall

        public Penguin(Texture2D aPic, Rectangle aRec, Rectangle aHitbox, int aHealth, Player aPlayer):base(aPic,aRec,aHitbox,aHealth)
        {
            column = 0;
            health = 200;
            player = aPlayer;
        }
        public void Update(ContentManager content,List<AIEnemy> enemies, GraphicsDevice gd)
        {
            //creates the hitboxes to determine if penguin can attack
            rAtkBx = new Rectangle(hitbox.X + hitbox.Width, hitbox.Y, 30, 60);
            lAtkBx = new Rectangle(hitbox.X - 15, hitbox.Y, 30, 60);
            
            counter++;
            hitboxPic = content.Load<Texture2D>("hitbox");
            //==========================================Penguin Facing Logic===========================================
            if (player.getHitbox().X + (player.getHitbox().Width / 2) < hitbox.X) //if batman is to the right of penguin
            {
                picture = content.Load<Texture2D>("Penguin"); //changes penguin to face left
                right = false;
            }
            else if ((player.getHitbox().X > hitbox.X + (hitbox.Width / 2))) //if batman is to the left of penguin
            {
                picture = content.Load<Texture2D>("Penguin Right"); //changes penguin to face right
                right = true;
            }

            //============================================BULLET UPDATE=============================================
            if (bullet.X > gd.Viewport.Width || bullet.X < 0) //if the bullet leaves the screen, stops updating the bullet
            {
                firedBullet = false;
            }
            else if (bullet.Intersects(player.getHitbox()) && firedBullet == true) //if the player is hit by penguin's bullet
            {
                firedBullet = false; //despawns the bullet
                player.TakeDamage(10); //damages the player by 10
            }
            
            //==========================================Boss Movement Logic=========================================
            FollowBatman(gd); //runs the follow batman method to follow batman

            //==========================================Spawn Enemies Logic=========================================
            spwnEnemiesTimer -= rndm.Next(5); //decreases the spawn enemies timer by a random amount
            if (spwnEnemiesTimer <= 0) //once the spawn enemies timer goes below zero
            {
                SpawnEnemies(enemies, content); //spawns a random amount of enemies
                spwnEnemiesTimer = 5000; //resets the spawn enemies timer
            }
            if (stunlocked >= 5) //if the penguin has been attacked 5 times without attacking
            {
                SpawnEnemies(enemies, content); //spawn enemies
                SpawnPenguinBots(gd.Viewport.Height-120,content);//spawn penguin bots
                stunlocked = 0; //resets the stun locked variable
            }

            //Shooting
            if (Math.Abs(player.getHitbox().X - hitbox.X) > 400 && !firedBullet && atkCooldown > 120) //if the player is > 400 units away from the penguin
            {
                shootEffect = content.Load<SoundEffect>("gunshot"); //plays the gunshot sound effect
                Shoot(); //spawns and updates the bullet
            }
                
                
            atkCooldown++;
            //Updating the Penguin bots
            for (int i = 0; i < bots.Count; i++) //for the number of penguin bots
            {
                bots[i].Update(content, player, enemies); //updates the penguin bots
                if (bots[i].getHealth() <= 0) //if any of the penguin bots' health goes below zero, removes it
                {
                    bots.RemoveAt(i);
                }
            }
        }
        public void Draw(SpriteBatch spriteBatch, List<AIEnemy> enemies)
        {
            //==========================================ANIMATION=====================================================
            //This block of code is simply to play the animations for all of the different actions
            //idle
            if (column == 0)
            {
                row = 0; 
            }
            //walking
            if (column == 1)
            {
                if (row >= 10)
                {
                    row = 0;
                }
            }
            //attacking
            if(column == 2) //once in the attack animation
            {
                if (row >= 4) //at the end of the animation
                {
                    Attack(); //runs the attack code
                    row = 0; //goes back to the idle postion
                    column = 0;
                    moving = true; //starts moving again
                    
                }
            }
            //shooting
            if (column == 3) //in the shooting animation
            {
                
                if (row >= 3) //once at the end of the animation
                {
                    row = 0; //sets the picture back to idle
                    column = 0;
                    moving = true; //allows the penguin to move again
                }
            }
            //taking damage
            if (column == 4)
            {
                moving = false; //stops moving
                if (right) //if the penguin is facing right, recoils left
                    hitbox.X--;
                else //otherwise recoils right
                    hitbox.X++;
                if(row>=5) //at the end of the animation
                {
                    column = 0; //goes back to idle animation
                    row = 0;
                    moving = true; //allows the penguin to follow batman again
                }
            }
            if (column == 5) //Death animation
            {
                moving = false; //stops moving
                if (row >= 5) //at the end of the animation
                {
                    health = -1; //sets the health below zero to allow the penguin to be deleted
                }
            }
            
            drawTimer++;
            //due to the images being slightly out of sinc, this block of code repositions the hitbox in order for it to be more accurate of the image displayed
            if (right)
                rect.X = hitbox.X - hitbox.Width*2/3;
            else
                rect.X = hitbox.X - hitbox.Width * 2 / 3 - 15;
            //changes the drawing rectangle location and size based on the hitbox
            rect.Y = hitbox.Y - 10;
            rect.Width = hitbox.Width * 200 / 80;
            rect.Height = hitbox.Height * 12 / 10;
            spriteBatch.Draw(picture, rect, new Rectangle(column * 100, row * 60, 100, 60), Color.White); //Drawing penguin

            //Drawing the bullet
            if (firedBullet && bulletRight) //if the bullet is moving right, moves the bullet right
            {
                shootTimer+=10;
                bullet.X = hitbox.X + shootTimer + hitbox.Width;
                spriteBatch.Draw(hitboxPic, bullet, Color.Black);
            }
            else if (firedBullet && !bulletRight) //if the bullet us set to move left, moves it left and draws it
            {
                shootTimer -= 10;
                bullet.X = hitbox.X + shootTimer + hitbox.Width;
                spriteBatch.Draw(hitboxPic, bullet, Color.Black);
            }
            //Animating the penguin
            if (drawTimer % 7 == 0)
                row++;

            //Drawing The Penguin Bots
            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].Draw(spriteBatch);
            }
        }
        public void Shoot()
        {
            //resets the attack cooldown
            atkCooldown = 0;
            column = 3; //plays shooting animation
            row = 0;
            firedBullet = true; 
            shootTimer = 0;
            bullet = new Rectangle(hitbox.X, hitbox.Y + (13*hitbox.Height /24 ), 3, 2); //creates the bullet
            bulletRight = right;
            moving = false; //stops the penguin from moving while shooting
            shootEffect.Play(0.5f,0,0); //plays tje shoot effect sound

           
        }
        public void Attack()
        {
            if (right && rAtkBx.Intersects(player.getHitbox())) //if batman is in the attack range of penguin
            {
                //do attack stuff
                stunlocked = 0; //resets the stun locked variable
                player.TakeDamage(30); //does damage to the player
            }
            else if (!right && lAtkBx.Intersects(player.getHitbox()))
            {
                //do attack stuff
                stunlocked = 0;
                player.TakeDamage(30);
            }
                
                atkCooldown = 0; //resets the attack variable
            
            
        }
        public void FollowBatman(GraphicsDevice gd)
        {
            if (moving) //if penguin is allowed to move (not shooting or attacking)
            {
                if (player.getHitbox().X + (player.getHitbox().Width) <= hitbox.X) //moves the penguin towards batman
                {
                    right = false;
                    hitbox.X--;
                    column = 1; //plays the running animation
                }
                else if ((player.getHitbox().X >= hitbox.X + (hitbox.Width)))
                {
                    right = true;
                    hitbox.X++;
                    column = 1; //plays the running animation
                }
                else //if not moving either way
                {
                    column = 0; //plays the idle animation
                }
            }
            
            if ((lAtkBx.Intersects(player.getHitbox()) || rAtkBx.Intersects(player.getHitbox())) && atkCooldown > 180) //if the attack cooldown is high enough and penguin is in attack range of batman
            {
                column = 2; //plays attack animation
                row = 0;
                atkCooldown = 0; //resets the cooldown
                moving = false; //stops penguin from moving

            }
            
        }
        public void SpawnEnemies(List<AIEnemy> enemies, ContentManager content)
        {
            
            int temp; 
            int location;
            temp = rndm.Next(2,5);
            for(int i = 0; i < temp; i++) //for a random amount of times between 2 and five
            {
                location = 60 * rndm.Next(10); //generates a random location
               enemies.Add(new AIEnemy(content.Load<Texture2D>("Thug"), new Rectangle(location, 360, 100, 120), new Rectangle(location, 360, 100, 120), 30, rndm.Next(1,3))); //adds a new enemy to the list
            }
        }
        public void SpawnPenguinBots(int ground, ContentManager content)
        {
            Texture2D penBotPic = content.Load<Texture2D>("penguin bot");
            for (int i = 0; i < rndm.Next(7); i++) //creates a random amount of penguin bots
            {
                bots.Add(new PenguinBot(penBotPic, new Rectangle(150, ground - 40, 55, 35), new Rectangle(rndm.Next(5)*200, ground - 40, 20, 35), 1)); //generates a penguin bot at a random location
            }
        }
        public void TakeDamage(int damage)
        {
            moving = false; //prevents penguin from moving
            health -= damage; //decreases penguin's health
            column = 4; //plays the recoil animation
            row = 0;
            stunlocked++; //increases the stunlocked variable
            if (health <=0) //if health goes below zero sets it back to 1 to play the animation before he dies
            {
                health = 1;
                column = 5;
            }
            
        }
        public void revive()
        {
            setHealth(300); //set penguins health to 300
            column = 0;
        }
        
    }
}
