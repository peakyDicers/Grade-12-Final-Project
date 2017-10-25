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
    class Instructions //Created By: Nick Geofroy
    {
        //Fields
        GraphicsDevice graphicsDevice;
        ContentManager Content;
        Texture2D backgroundImage;
        SpriteFont font;
        string text;
        Texture2D blankRec;
        Texture2D instructions1;
        int scroll;
        Button controlsBtn, bossMechanicsBtn, initialInstructionsBtn, mechanicsBtn, backBtn;
        int column;
        Rectangle lightRec;
        int row;
        GamePadState oldPad1;
        bool exit = false;
        

        //Constructor
        public Instructions(GraphicsDevice gd, ContentManager aContent)
        {
            //loads the graphics device and content manager 
            graphicsDevice = gd;
            Content = aContent;
            //loads the necessary backgrounds
            backgroundImage = Content.Load<Texture2D>("background");
            font = Content.Load<SpriteFont>("Font");
            blankRec = Content.Load<Texture2D>("hitbox");
            instructions1 = blankRec;

            //creates the different buttons 
            initialInstructionsBtn = new Button(blankRec, new Rectangle(10, 100, 150, 30), Color.Blue,"Story", false, font);
            controlsBtn = new Button(blankRec, new Rectangle(10, 140, 150, 30), Color.Blue, "Controls",false,font);
            mechanicsBtn = new Button(blankRec, new Rectangle(10, 180, 150, 30), Color.Blue, "Mechanics", false, font);
            bossMechanicsBtn = new Button(blankRec, new Rectangle(10, 220, 150, 30), Color.Blue, "Boss Mechanics", false, font);
            backBtn = new Button(blankRec, new Rectangle(graphicsDevice.Viewport.Width-160, graphicsDevice.Viewport.Height -40 , 150, 30), Color.Blue, "Back", false, font);
            
            
            lightRec = new Rectangle(graphicsDevice.Viewport.Width / 6, graphicsDevice.Viewport.Height / 12, 2 * graphicsDevice.Viewport.Width / 3, 10 * graphicsDevice.Viewport.Height / 12);
        }
        //Necessary getters and setters
        public bool getExit()
        {
            return exit;
        }
        public void setExit(bool aExit)
        {
            exit = aExit;
        }
        
        //Methods
        public void Draw(SpriteBatch sb)
        {
            
            //Draws the city background image
            sb.Draw(backgroundImage, new Rectangle(0, 0, graphicsDevice.Viewport.Width, graphicsDevice.Viewport.Height), new Rectangle(0, 0, 640, 360), Color.White);

            //Draws the buttons on the side of the screen
            bossMechanicsBtn.Draw(sb);
            controlsBtn.Draw(sb);
            initialInstructionsBtn.Draw(sb);
            mechanicsBtn.Draw(sb);
            backBtn.Draw(sb);

            text = "Instructions";
            //Draws the title (Instructions)
            sb.DrawString(font, text, new Vector2(((graphicsDevice.Viewport.Width/2)-(font.MeasureString(text).X/2)),0),Color.Red);
            //Draws the rectange that appears behind the instructions so that they can be read easier
            sb.Draw(blankRec, lightRec, new Rectangle(graphicsDevice.Viewport.Width / 4, graphicsDevice.Viewport.Height / 4, graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2), new Color(150,150,150,50));
            if (mechanicsBtn.getSelected() && oldPad1.Buttons.A == ButtonState.Pressed) //if the mechanics button has been selected, loads the mechanics image
            {
                instructions1 = Content.Load<Texture2D>("Instructions/mechanics");
                column = 1;
            }
            if (initialInstructionsBtn.getSelected() && oldPad1.Buttons.A == ButtonState.Pressed) //if the mechanics button has been selected, loads the initial instrutions image
            {
                instructions1 = Content.Load<Texture2D>("Instructions/Initial Instructions");
                column = 1;
            }
            if (controlsBtn.getSelected() && oldPad1.Buttons.A == ButtonState.Pressed) //if the mechanics button has been selected, loads the controls image
            {
                instructions1 = Content.Load<Texture2D>("Instructions/Controls");
                column = 1;
            }
            if (bossMechanicsBtn.getSelected() && oldPad1.Buttons.A == ButtonState.Pressed) //if the mechanics button has been selected, loads the boss mechanics image
            {
                instructions1 = Content.Load<Texture2D>("Instructions/bossMechanics");
                column = 1;
            }
            if (backBtn.getSelected() && oldPad1.Buttons.A == ButtonState.Pressed) //if the back button has been pressed
            {
                exit = true; //exirs the game
                backBtn.setSelected(false); //deselects the back button so it doesn't instantly exit once the instructions are selected again
                column = 0; //changes the column back to the original one for when someone restarts the instructions

            }

            sb.Draw(instructions1, new Rectangle(graphicsDevice.Viewport.Width / 6, graphicsDevice.Viewport.Height / 12, 2 * graphicsDevice.Viewport.Width / 3, 10 * graphicsDevice.Viewport.Height / 12),new Rectangle(0,scroll,instructions1.Bounds.Width,200) , Color.White);//draws the clear instructions image to be read
            
        }

        public void Update(GamePadState pad1)
        {


            if (pad1.ThumbSticks.Left.X == -1 && oldPad1.ThumbSticks.Left.X != -1) //changes the column if the thunbstick has been moved left (edge detection)
            {
                column --;
            }
            if (pad1.ThumbSticks.Left.X == 1 && oldPad1.ThumbSticks.Left.X != 1) //changes the column if the thunbstick has been moved right (edge detection)
            {
                column++;
            }
            if (column > 2) //loops the column back to zero once the end has been reached
            {
                column = 0;
            }
            if (column < 0) //if you try to reduce the column below zero, changes to the end one
            {
                column = 2;
            }
            //======================================Column of Buttons=======================================================
            if (column == 0)
            {
                if (pad1.ThumbSticks.Left.Y < -0.5 && oldPad1.ThumbSticks.Left.Y > -0.5) //allows the buttons to be scrolled through using the thumbstick
                {
                    row++;
                    scroll = 0; //moves the instructions displayed back to the beginning of the instructions
                }
                if (pad1.ThumbSticks.Left.Y > 0.5 && oldPad1.ThumbSticks.Left.Y < 0.5) //same as above but with moving up
                {
                    row--;
                    scroll = 0;
                }

                
                lightRec = new Rectangle(graphicsDevice.Viewport.Width / 6, graphicsDevice.Viewport.Height / 12, 2 * graphicsDevice.Viewport.Width / 3, 10 * graphicsDevice.Viewport.Height / 12); //shrinks the background rectangle slightly so that it is easy to tell the buttons have been selected
                if (row < 0) //goes back to the beginning if you try to go before the first option
                {
                    row = 3;
                }
                if (row == 0) //when the first button has been hovered
                {
                    initialInstructionsBtn.setSelected(true); //increases the size of the button to show it has been collected
                    
                }
                else //when the button isnt selected
                {
                    initialInstructionsBtn.setSelected(false);
                }
                if (row == 1) //row = 1 is the controls button
                {
                    controlsBtn.setSelected(true); //enlarges the button size to show its been se;ected
                    
                }
                else
                {
                    controlsBtn.setSelected(false); //schrinks the button size once it is deselected
                }
                if (row == 2) //ROW = 2 is for the mechanics button
                {
                    mechanicsBtn.setSelected(true); //increases the size to show it has been selected
                    
                }
                else
                {
                    mechanicsBtn.setSelected(false); //decreases the size to show that it has been deselected
                }
                if (row == 3) //ROW = 3 boss mechanics
                {
                    bossMechanicsBtn.setSelected(true); //increases the size of the button to show that it has been selected
                    
                }
                else
                {
                    bossMechanicsBtn.setSelected(false); //decreases the size so that it is obvious that the button has been deselected
                }
                if (row > 3) //once the player scrolls past the end of the row of buttons
                {
                    row = 0;
                }
            }
            //========================================THE CODE FOR THE SCROLLING AND CENTER RECTANGLE=========================
            if (column == 1)
            {
                //deselects all of the other buttons
                bossMechanicsBtn.setSelected(false);
                mechanicsBtn.setSelected(false);
                controlsBtn.setSelected(false);
                initialInstructionsBtn.setSelected(false);


                lightRec = new Rectangle(graphicsDevice.Viewport.Width / 6-3, graphicsDevice.Viewport.Height / 12-3, 2 * graphicsDevice.Viewport.Width / 3+6, 10 * graphicsDevice.Viewport.Height / 12+6); //increases the size of the white rectangle when it has been selected
                if (pad1.ThumbSticks.Left.Y > 0 && scroll > 0) //if the thumbstick moves down or up, increases/decreases scroll which changes the rectangle the viewer can see from the original brawing of the instructions
                {
                    scroll--;
                }
                if (pad1.ThumbSticks.Left.Y < 0 && scroll < 350)
                {
                    scroll++;
                }
            }
            if (column == 2) //EXIT BUTTON COLUMN
            {
                //selects the back button and deselects all of the other ones
                backBtn.setSelected(true);
                bossMechanicsBtn.setSelected(false);
                mechanicsBtn.setSelected(false);
                controlsBtn.setSelected(false);
                initialInstructionsBtn.setSelected(false);
            }
            else //deselects the back button when 
            {
                backBtn.setSelected(false);
            }
            oldPad1 = pad1; //used for edge selection
        }
    }
}
