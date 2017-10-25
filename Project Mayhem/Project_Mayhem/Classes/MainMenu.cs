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
    class MainMenu //Created By: Ian Chui
    {
        Texture2D pic, btnPic;
        Rectangle screenRec, cameraRec;
        SpriteFont font, betaFont, titlefont, nameFont;
        Button[] buttons = new Button[7];
        Song theme;
        int numOfOptions = 5;
        int counter;
        int frameTimer;
        bool musicPlaying;
        bool loadOptions;
        bool developerMode;
        int creditCounter;
        string option;
        bool[] developerLock = new bool[10];
        bool[] lockSequence = new bool[10];
       
        public MainMenu(GraphicsDevice graphics, ContentManager content)
        {
            screenRec = new Rectangle(0, 0, graphics.Viewport.Width, graphics.Viewport.Height);
            cameraRec = new Rectangle(520, 240, 1000 / 4, 700 / 4); //used to pan through pictures.

            //load fonts, textures, etc.
            titlefont = content.Load<SpriteFont>("TitleFont");
            font = content.Load<SpriteFont>("Font");
            nameFont = content.Load <SpriteFont>("nameFont");
            betaFont = content.Load<SpriteFont>("beta");         
            pic = content.Load<Texture2D>("MainMenu/smaller1");
            btnPic = content.Load<Texture2D>("hitbox");
            theme = content.Load<Song>("MainMenu/Menu Theme");       

            for (int i = 0; i < 7; i++) //create all main menu buttons.
            {
                buttons[0] = new Button(btnPic, new Rectangle(100, 300, 150, 40), Color.White, "Story Mode", true, font);
                buttons[1] = new Button(btnPic, new Rectangle(100, 345, 150, 40), Color.White, "Instructions", false, font);
                buttons[2] = new Button(btnPic, new Rectangle(100, 390, 150, 40), Color.White, "Endless Mode", false, font);
                buttons[3] = new Button(btnPic, new Rectangle(100, 435, 150, 40), Color.White, "Credits", false, font);
                buttons[4] = new Button(btnPic, new Rectangle(100, 480, 150, 40), Color.White, "Quit", false, font);
                buttons[5] = new Button(btnPic, new Rectangle(100, 525, 150, 40), Color.White, "Testing Mode", false, font);    
                buttons[6] = new Button(btnPic, new Rectangle(430, 480, 150, 40), Color.White, "Press Start", false, font);
            }
            lockSequence[0] = true;
        }

        public void startMusic()
        {
            MediaPlayer.Play(theme);
        }
        public void stopMusic()
        {
            MediaPlayer.Stop();
        }
        

        public void update(GamePadState pad, GamePadState oldPad)
        {
            if (developerMode) {
                numOfOptions = 6;
            }
            //allows to back to loading screen from menu.
            if (option != "Credits")    
                if (pad.Buttons.B == ButtonState.Pressed && oldPad.Buttons.B == ButtonState.Released)
                    loadOptions = false;

            //start music.
            if (!musicPlaying)
            {
                musicPlaying = true;
                MediaPlayer.Play(theme);
            }
            #region Loading Screen
            counter++;
            if (frameTimer == 0)
            {
                if (counter < 60 * 8) //pic1 slow zoom out.
                {
                    cameraRec.Width += 2;
                    cameraRec.X -= 1;
                    cameraRec.Height += 2;
                    cameraRec.Y -= 1;
                    frameTimer = 4;
                }

                if (counter > 470 && counter < 520) //pic1 fast exit.
                {
                    frameTimer = 1;
                    cameraRec.Width += 20;
                    cameraRec.X -= 3;
                    cameraRec.Height += 14;
                    cameraRec.Y += 0;
                }

                if (counter == 540) // load pic 2.
                {
                    frameTimer = 1;
                    cameraRec.X = 1200;
                    cameraRec.Y = 100;
                    cameraRec.Width = 990;
                    cameraRec.Height = 693;
                }
                if (counter > 540 && counter < 560) //pic 2. fast intro.
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                    cameraRec.Y += 0;
                    cameraRec.Width += 30;
                    cameraRec.Height += 21;
                }
                if (counter > 560 && counter < 1050) //pic 2. slow slide
                {
                    frameTimer = 2;
                    cameraRec.X -= 3;
                    cameraRec.Y++;
                    cameraRec.Width += 0;
                    cameraRec.Height += 0;
                }
                if (counter >= 1050 && counter < 1060) //pic 2. fast exit
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                    cameraRec.Y += 3;
                }
                if (counter == 1060) //pic 3 load.                                                          
                {
                    cameraRec.X = 900;
                    cameraRec.Y = 300;
                    cameraRec.Width = 1400;
                    cameraRec.Height = 980;
                }
                if (counter >= 1060 && counter < 1080) // pic 3 fast intro.
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter > 1080 && counter < 1160) //pic 3 slide
                {
                    frameTimer = 2;
                    cameraRec.X -= 3;
                }
                if (counter >= 1160 && counter < 1175) // pic 3 fast exit.
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter == 1175) //pic 4 load
                {
                    cameraRec.X = 1250;
                    cameraRec.Y = 250;
                    cameraRec.Width = 1400;
                    cameraRec.Height = 980;
                }
                if (counter >= 1175 && counter <= 1190) //pic 4 fast
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter > 1190 && counter < 1500) //pic4 slide
                {
                    frameTimer = 2;
                    cameraRec.X -= 3;
                }
                if (counter >= 1500 && counter < 1515) //pic4 fast exit.
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter == 1515) //pic5 load
                {
                    cameraRec.X = 800;
                    cameraRec.Y = 250;
                    cameraRec.Width = 1400;
                    cameraRec.Height = 980;
                }
                if (counter >= 1510 && counter < 1525) //pic5 fast intro
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter >= 1525 && counter < 1885) //pic 5 slide
                {
                    frameTimer = 2;
                    cameraRec.X--;
                }
                if (counter >= 1885 && counter < 1900) //pic 5 fast exit
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter == 1900) //load pic6
                {
                    cameraRec.X = 900;
                    cameraRec.Y = 0;
                    cameraRec.Width = 1400;
                    cameraRec.Height = 980;
                }
                if (counter >= 1900 && counter < 1915) //pic6 fast intro
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter >= 1915 && counter < 2040) //pic 6slide
                {
                    frameTimer = 2;
                    cameraRec.X--;
                }
                if (counter >= 2040 && counter < 2055) //pic 6 fast exit
                {
                    frameTimer = 1;
                    cameraRec.X -= 20;
                }
                if (counter == 2055) //load pic7
                {
                    cameraRec.X = -200;
                    cameraRec.Y = 0;
                    cameraRec.Width = 1400;
                    cameraRec.Height = 980;
                }
                if (counter >= 2055 && counter < 2070)// pic 7 fast intro
                {
                    frameTimer = 1;
                    cameraRec.X += 20;
                }
                if (counter >= 2070 && counter < 2340) //pic 7 slide.
                {
                    frameTimer = 2;
                    cameraRec.X++;
                }
                if (counter == 2340) //close up of pic 7.
                {
                    cameraRec.X = 950;
                    cameraRec.Y = 200;
                    cameraRec.Width = 500;
                    cameraRec.Height = 350;
                }
                if (counter > 2340) //slide.
                {
                    frameTimer = 2;
                    cameraRec.X++;
                }
                if (counter >= 2580)
                {
                    cameraRec = new Rectangle(520, 240, 1000 / 4, 700 / 4);
                    counter = 0;
                }
            }
            if (frameTimer > 0)
                frameTimer--; 
            #endregion

            if (pad.Buttons.Start == ButtonState.Pressed || pad.Buttons.A == ButtonState.Pressed && oldPad.Buttons.A != ButtonState.Released) 
                loadOptions = true;
         
            //load all the options.
            if (loadOptions)
            {
                if ((pad.ThumbSticks.Left.Y < -0.5 && oldPad.ThumbSticks.Left.Y > -0.5) || (pad.DPad.Down == ButtonState.Pressed && oldPad.DPad.Down != ButtonState.Pressed)) //scrolling down through options.
                {
                    for (int i = 0; i < numOfOptions; i++)
                    {
                        if (buttons[i].getSelected())
                        {
                            buttons[i].setSelected(false);
                            if (i != numOfOptions-1)
                            {
                                buttons[i + 1].setSelected(true);
                                break;
                            }
                            else //if bottom option reached, go to top option.              
                            {
                                buttons[0].setSelected(true);
                                break;
                            }
                        }
                    }
                }


                if ((pad.ThumbSticks.Left.Y > 0.5 && oldPad.ThumbSticks.Left.Y < 0.5) || (pad.DPad.Up== ButtonState.Pressed && oldPad.DPad.Up != ButtonState.Pressed)) //scrolling up through options.
                {
                    for (int i = 0; i < numOfOptions; i++)
                    {
                        if (buttons[i].getSelected())
                        {
                            buttons[i].setSelected(false);
                            if (i != 0)
                            {
                                buttons[i - 1].setSelected(true);
                                break;
                            }
                            else //if top option reached, go to bottom option.
                            {
                                buttons[numOfOptions-1].setSelected(true);
                                break;
                            }
                        }
                    }
                }


                //Check which option has been selected. Option is then sent to 'game1'
                if (pad.Buttons.A == ButtonState.Pressed && oldPad.Buttons.A == ButtonState.Released)     
                {
        
                    if (buttons[0].getSelected())
                        option = "Play";
                    if (buttons[1].getSelected())
                        option = "Instructions";
                    if (buttons[2].getSelected())
                        option = "Endless";                            
                    if (buttons[3].getSelected())
                        option = "Credits";
                    if (buttons[4].getSelected())
                        option = "Quit";
                    if (buttons[5].getSelected())
                        option = "Testing";      
                }

                //If option chosen is credits, allow player to back to main menu.
                if (option == "Credits")
                    if (pad.Buttons.B == ButtonState.Pressed)
                    {
                        creditCounter = 0;
                        option = "";
                    }

                //Secret gamemode for two jokers.
                if (pad.Buttons.LeftShoulder == ButtonState.Pressed && pad.Buttons.RightShoulder == ButtonState.Pressed && pad.Buttons.Y == ButtonState.Pressed) {
                    option = "Double Trouble";
                }

                
            }
            #region Cheat code for Testing Mode

            //This still needs work. Need to figure out how to reset locksequence when the wrong button is pressed.

            //up up 
            if (pad.DPad.Up == ButtonState.Released && oldPad.DPad.Up == ButtonState.Pressed && lockSequence[0] && loadOptions) {
                lockSequence[0] = false;
                lockSequence[1] = true;
                developerLock[0] = true;
                Console.WriteLine('1');
            } else {
                // resetLock();
            }
            if (developerLock[0] && pad.DPad.Up == ButtonState.Pressed && oldPad.DPad.Up == ButtonState.Released && lockSequence[1]) {
                developerLock[1] = true;
                lockSequence[1] = false;
                lockSequence[2] = true;
                Console.WriteLine('2');
            } else {
                //  resetLock();
            }

            //down down
            if (developerLock[1] && pad.DPad.Down == ButtonState.Released && oldPad.DPad.Down == ButtonState.Pressed && lockSequence[2]) {
                developerLock[2] = true;
                lockSequence[2] = false;
                lockSequence[3] = true;
                Console.WriteLine('3');
            } else {
                // resetLock();
            }
            if (developerLock[2] && pad.DPad.Down == ButtonState.Pressed && oldPad.DPad.Down == ButtonState.Released && lockSequence[3]) {
                developerLock[3] = true;
                lockSequence[3] = false;
                lockSequence[4] = true;
                Console.WriteLine('4');
            } else {
                //   resetLock();
            }

            //left right
            if (developerLock[3] && pad.DPad.Left == ButtonState.Pressed && oldPad.DPad.Left == ButtonState.Released && lockSequence[4]) {
                developerLock[4] = true;
                lockSequence[4] = false;
                lockSequence[5] = true;
                Console.WriteLine('5');
            } else {
                // resetLock();
            }
            if (developerLock[4] && pad.DPad.Right == ButtonState.Pressed && oldPad.DPad.Right == ButtonState.Released && lockSequence[5]) {
                developerLock[5] = true;
                lockSequence[5] = false;
                lockSequence[6] = true;
                Console.WriteLine('6');
            } else {
                //   resetLock();
            }

            //left right 
            if (developerLock[5] && pad.DPad.Left == ButtonState.Pressed && oldPad.DPad.Left == ButtonState.Released && lockSequence[6]) {
                developerLock[6] = true;
                lockSequence[6] = false;
                lockSequence[7] = true;
                Console.WriteLine('7');
            } else {
                //  resetLock();
            }
            if (developerLock[6] && pad.DPad.Right == ButtonState.Pressed && oldPad.DPad.Right == ButtonState.Released && lockSequence[7]) {
                developerLock[7] = true;
                lockSequence[7] = false;
                lockSequence[8] = true;
                Console.WriteLine('8');
            } else {
                //   resetLock();
            }

            //ba
            if (developerLock[7] && pad.Buttons.B == ButtonState.Pressed && oldPad.Buttons.B == ButtonState.Released && lockSequence[8]) {
                developerLock[8] = true;
                lockSequence[8] = false;
                lockSequence[9] = true;
                Console.WriteLine('9');
            } else {
                //    resetLock();
            }
            if (developerLock[8] && pad.Buttons.A == ButtonState.Pressed && oldPad.Buttons.A == ButtonState.Released && lockSequence[9]) {
                developerLock[9] = true;
                lockSequence[9] = false;
                developerMode = true;
                Console.WriteLine("unlocked");
            } else {
                //   resetLock();
            } 
            #endregion
        }
        public void resetLock() {
            for (int i = 0; i < developerLock.Length; i++) {
                developerLock[i] = false;
            }
            for (int i = 0; i < lockSequence.Length; i++) {
                lockSequence[i] = false;
            }
        }
        public string getOptionChoice()
        {
            return option;
        }
        public void setOptionChoice(string aOption)
        {
            option = aOption;
        }
        public void draw(SpriteBatch sb, ContentManager content)
        {
            #region Loading Screen images
            //Load loading screen images.
            if (counter < 540)
                pic = content.Load<Texture2D>("MainMenu/smaller1");
            if (counter >= 540 && counter < 1060)
                pic = content.Load<Texture2D>("MainMenu/pic2");
            if (counter >= 1060 && counter < 1175)
                pic = content.Load<Texture2D>("MainMenu/pic3");
            if (counter >= 1175 && counter < 1510)
                pic = content.Load<Texture2D>("MainMenu/pic4");
            if (counter >= 1515)
                pic = content.Load<Texture2D>("MainMenu/pic5");
            if (counter >= 1900)
                pic = content.Load<Texture2D>("MainMenu/pic6");
            if (counter >= 2055)
                pic = content.Load<Texture2D>("MainMenu/pic7"); 
            #endregion

            sb.Draw(pic, screenRec, cameraRec, Color.White); //drawing loading screen images.

            if (loadOptions) //if options loaded, draw options.
            {
                for (int i = 0; i < numOfOptions; i++){
                    buttons[i].Draw(sb);                
                }
            }
            else //otherwise display loading screen.
            {
                sb.DrawString(titlefont, "BATMAN:PROJECT MAYHEM", new Vector2(160, 300), Color.White);
                sb.DrawString(betaFont, "Release 1.1.2   Developers Kit", new Vector2(690, 575), Color.White);
                buttons[6].Draw(sb);
            }

            #region Credits
            if (option == "Credits") //if credits selected, display credits.
            {
                sb.Draw(pic, screenRec, cameraRec, Color.Black);
                creditCounter++;
                if (creditCounter >= 60 && creditCounter <= 180)
                    sb.DrawString(betaFont, "BATMAN : PROJECT MAYHEM", new Vector2(700, 550), Color.White);

                if (creditCounter > 180 && creditCounter <= 360) {
                    sb.DrawString(betaFont, "Alex Green\n - Batman\n - Batarang\n - Background \n - Level design", new Vector2(50, 100), Color.White);
                    sb.DrawString(betaFont, "Nick Geofroy\n - Penguin AI\n - Thug AI\n - Instruction Manual\n - Written Reports", new Vector2(300, 100), Color.White);
                    sb.DrawString(betaFont, "Ian Chui\n - Joker AI\n - Bomb physics\n - Main menu\n - Loading Screen drawings", new Vector2(650, 100), Color.White);
                }
                if (creditCounter > 360 && creditCounter < 700)
                    sb.DrawString(nameFont, "DIRECTORS & PROJECT LEADS", new Vector2(100, 100), Color.White);
                if (creditCounter > 365 && creditCounter < 705)
                    sb.DrawString(betaFont, "CREATIVE DIRECTOR", new Vector2(100, 200), Color.White);
                if (creditCounter > 370 && creditCounter < 710)
                    sb.DrawString(nameFont, "IAN CHUI", new Vector2(100, 230), Color.White);
                if (creditCounter > 375 && creditCounter < 715)
                    sb.DrawString(betaFont, "SENIOR PRODUCER", new Vector2(100, 300), Color.White);
                if (creditCounter > 380 && creditCounter < 720)
                    sb.DrawString(nameFont, "NICK GEOFROY", new Vector2(100, 330), Color.White);
                if (creditCounter > 385 && creditCounter < 725)
                    sb.DrawString(betaFont, "GAME DIRECTOR", new Vector2(100, 400), Color.White);
                if (creditCounter > 390 && creditCounter < 730)
                    sb.DrawString(nameFont, "ALEX GREEN", new Vector2(100, 430), Color.White);
                if (creditCounter > 395 && creditCounter < 735)
                    sb.DrawString(betaFont, "GAMEPLAY DIRECTOR", new Vector2(100, 500), Color.White);
                if (creditCounter > 400 && creditCounter < 740)
                    sb.DrawString(nameFont, "ALEX GREEN", new Vector2(100, 530), Color.White);

                //---------------------------------------------------------------------------------

                if (creditCounter > 900 && creditCounter < 1240)
                    sb.DrawString(betaFont, "NARRATIVE DIRECTOR", new Vector2(100, 200), Color.White);
                if (creditCounter > 905 && creditCounter < 1245)
                    sb.DrawString(nameFont, "ALEX GREEN", new Vector2(100, 230), Color.White);
                if (creditCounter > 910 && creditCounter < 1250)
                    sb.DrawString(betaFont, "LEVEL DESIGN DIRECTOR", new Vector2(100, 300), Color.White);
                if (creditCounter > 915 && creditCounter < 1255)
                    sb.DrawString(nameFont, "NICK GEOFROY", new Vector2(100, 330), Color.White);
                if (creditCounter > 920 && creditCounter < 1260)
                    sb.DrawString(betaFont, "ART DIRECTOR", new Vector2(100, 400), Color.White);
                if (creditCounter > 925 && creditCounter < 1265)
                    sb.DrawString(nameFont, "IAN CHUI", new Vector2(100, 430), Color.White);
                if (creditCounter > 930 && creditCounter < 1270)
                    sb.DrawString(betaFont, "LEAD PROGRAMMER", new Vector2(100, 500), Color.White);
                if (creditCounter > 935 && creditCounter < 1275)
                    sb.DrawString(nameFont, "NICK GEOFROY", new Vector2(100, 530), Color.White);

                //---------------------------------------------------------------------------------

                if (creditCounter > 1435 && creditCounter < 1775)
                    sb.DrawString(betaFont, "SPECIAL PROJECTS PRODUCERS", new Vector2(100, 100), Color.White);
                if (creditCounter > 1440 && creditCounter < 1780)
                    sb.DrawString(nameFont, "IAN CHUI", new Vector2(100, 130), Color.White);
                if (creditCounter > 1445 && creditCounter < 1785)
                    sb.DrawString(nameFont, "ALEX GREEN", new Vector2(100, 160), Color.White);

                if (creditCounter > 1450 && creditCounter < 1790)
                    sb.DrawString(betaFont, "WORLD TEAM PRODUCERS", new Vector2(100, 260), Color.White);
                if (creditCounter > 1455 && creditCounter < 1795)
                    sb.DrawString(nameFont, "IAN CHUI", new Vector2(100, 290), Color.White);
                if (creditCounter > 1460 && creditCounter < 1800)
                    sb.DrawString(nameFont, "NICK GEOFROY", new Vector2(100, 320), Color.White);

                if (creditCounter > 1465 && creditCounter < 1805)
                    sb.DrawString(betaFont, "CHARACTER & CINEMATICS PRODUCER", new Vector2(100, 420), Color.White);
                if (creditCounter > 1470 && creditCounter < 1810)
                    sb.DrawString(nameFont, "IAN CHUI", new Vector2(100, 450), Color.White);


                //---------------------------------------------------------------------------------

                if (creditCounter > 2000 && creditCounter < 2200)
                    sb.DrawString(nameFont, "BATMAN : PROJECT MAYHEM", new Vector2(340, 280), Color.White);
                if (creditCounter > 2000 && creditCounter < 2200)
                    sb.DrawString(nameFont, "2016", new Vector2(470, 500), Color.White);

                if (creditCounter > 2400) {
                    option = "";
                    creditCounter = 0;
                }

                // sb.DrawString(betaFont, "(B)", new Vector2(750, 550), Color.Red);
            } 
            #endregion
        }

        public void reset() {
            counter = 0;
            startMusic();
            MediaPlayer.Volume = 1.0f;
            option = "";
            cameraRec.X = 520;
            cameraRec.Y = 240;
            cameraRec.Width = 1000/4;
            cameraRec.Height = 700/4;
            for (int i = 0; i < buttons.Length; i++) {
                buttons[i].setSelected(false);
            }
            buttons[0].setSelected(true);
            
        }
    }
}
