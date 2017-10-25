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
    class Items //Created By: Alex Green
    {
        string type; //holds what type of item it is 
        Rectangle rect;
        Texture2D pic;
        int oldDistance; //distance it was at previous tick

        public Items(int x, int ground, int atype, int distance)
        {
            oldDistance = distance;
            rect = new Rectangle(x-15, ground - 30, 30, 30);
            //rng decides what type of item it is
            if (atype < 20) 
                type = "points";
            else if (atype < 50)
                type = "health";
            else if (atype < 60)
                type = "baterang";
        }

        public void Update(Player player, List<Items> item, int count)
        {
            if (player.getDistance() > oldDistance) //if player has progressed since last tick, move item along ground to simulate player moving forwards
            {
                rect.X -= player.getDistance() - oldDistance;
                oldDistance = player.getDistance();
            }
            if (rect.Intersects(player.getHitbox())) //if player touches item
            {
                if (type == "points") //if it was points gain points
                {
                    player.GainPoints(500); 
                }
                else if (type == "health") //if it was health heal player
                {
                    player.Heal(25);
                }
                else if (type == "baterang") //if it was batarang, gain a batarang
                {
                    player.GainAmmo();
                }
                item.RemoveAt(count); //afterwards delete item
            }
            if (rect.X < -30) //if item scrolls of screen, delete it
                item.RemoveAt(count);
        }

        public void Draw(SpriteBatch sb, ContentManager content)
        {
            pic = content.Load<Texture2D>(type);
            sb.Draw(pic, rect, Color.White);
        }

    }
}
