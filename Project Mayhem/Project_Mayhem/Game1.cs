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
    /*                                              Citations
     Adventures of Batman and Robin (SNES)
        - Batman sprites
        - Joker sprites
     
     Batman Forever(Play Station)
        - Riddler thug sprites
        - Penguin sprites
     
     Batman:Arkham Origins (PC)
        - menu theme
     
    -City Background: https://www.google.ca/url?sa=i&rct=j&q=&esrc=s&source=images&cd=&cad=rja&uact=8&ved=0ahUKEwjpg9C7p6jNAhUi6IMKHSk8ANgQjB0IBg&url=http%3A%2F%2Fgizmodo.com%2Fi-want-to-live-in-this-pixelated-gotham-1509881025&psig=AFQjCNF08yOIe9BRNAn27Bbd7p9HDmp5tw&ust=1466020870667875

    -Sound Effects courtesy of http://soundbible.com/

    -Coin picture: http://pressthebuttons.typepad.com/.a/6a00d83452033569e20148c778fd94970c-600wi









    */
    //Welcome to fight club. 
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        MainMenu menu;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        GamePadState pad1;
        GamePadState oldpad1;
        Random rng = new Random();
        Player player;
        List<Joker> joker = new List<Joker>();
        Penguin penguin;
        Texture2D penguinPic;
        Texture2D thugPic;
        Texture2D batmanpic;
        Rectangle testRec = new Rectangle(600,360, 100, 120);
        Rectangle logoRec = new Rectangle(200, 10, 300,220 );
        Rectangle pauseOverlay = new Rectangle(0, 0, 0, 500);
        Texture2D logoPic;
        Texture2D jokerPic;
        Rectangle groundRec;
        Texture2D groundPic;
        Texture2D bombPic;
        Texture2D penBotPic;
        Rectangle bgRec;
        Texture2D bgPic;
        int bgcolor;
        int currentbg;
        Texture2D trainPic;
        Rectangle trainRec;
        int trainTimer;
        SpriteFont font, comboFont;
        SoundEffect woosh, explosion;

        Rectangle healthBar1, healthBar2, healthBar3;
        Texture2D healthPic1, healthPic2, healthPic3;
        Rectangle comboBox;
        int oldCombo;
        Vector2 comboVector;
        string comboText;
        int comboRecTimer;
        Color comboBoxColr = Color.Red;
        Color originalComboBoxColr;

        Song mainSong;

        int pauseAccel = 0;
        int pauseOptionSelector = 0;
        bool pausing = false;
        bool paused = false;
        bool[] pauseSelection = new bool[2];
        Color[] pauseOptionColors = new Color[2];

        Instructions instructions;

        Texture2D batarangPic, comboPic;

        bool spawnPenguin; //testing purposes.
        bool spawnJoker; //also testing purposes
        
        int ground; 
        int state = 0; //what state the game is in. 0 = menu, 1 = instructions, 2 = game ?, 3 = infinite?
        int progression;

        int wave; //holds what wave in endless mode you're on
        
        
        List<AIEnemy> enemies = new List<AIEnemy>();
        List<Bomb> bombs = new List<Bomb>();
        List<PenguinBot> bots = new List<PenguinBot>();
        List<Items> drops = new List<Items>();
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Window.Title = "";

            graphics.PreferredBackBufferHeight = 600;
            graphics.PreferredBackBufferWidth = 1000;
            graphics.PreferMultiSampling = false;

            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            currentbg = 1;
            bgcolor = 255;
            trainRec = new Rectangle(-500,(GraphicsDevice.Viewport.Height - 116) / 2,GraphicsDevice.Viewport.Width / 640 * 225, (GraphicsDevice.Viewport.Height - 120) / 6);
            bgRec = new Rectangle(0, 0, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height - 120);
            ground = GraphicsDevice.Viewport.Height - 120;
                base.Initialize();

        }
        protected override void LoadContent()
        {
           
            spriteBatch = new SpriteBatch(GraphicsDevice);
            bgPic = Content.Load<Texture2D>("background");
            trainPic = Content.Load<Texture2D>("train");
            //logoPic = Content.Load<Texture2D>("logo2");
            batmanpic = Content.Load<Texture2D>("Batman");
            thugPic = Content.Load<Texture2D>("Thug");
            jokerPic = Content.Load<Texture2D>("Joker");
            penguinPic = Content.Load<Texture2D>("Penguin");
            groundPic = Content.Load<Texture2D>("hitbox");
            bombPic = Content.Load<Texture2D>("Bomb");
            penBotPic = Content.Load<Texture2D>("penguin bot");
            healthPic1 = Content.Load<Texture2D>("healthbar1");
            healthPic2 = Content.Load<Texture2D>("healthbar2");
            healthPic3 = Content.Load<Texture2D>("healthbar3");
            batarangPic = Content.Load<Texture2D>("baterang");
            comboPic = Content.Load<Texture2D>("comboOutline");
            woosh = Content.Load<SoundEffect>("woosh");
            explosion = Content.Load<SoundEffect>("explosionSound");
            mainSong = Content.Load<Song>("MAINSONG");


            comboFont = Content.Load<SpriteFont>("comboFont");
            font = Content.Load<SpriteFont>("Font");

            groundRec = new Rectangle(0, GraphicsDevice.Viewport.Height - 120, GraphicsDevice.Viewport.Width, 120);

            joker.Add(new Joker(jokerPic, new Rectangle(), new Rectangle(), 100, ground, enemies, Content));
            joker.Add(new Joker(jokerPic, new Rectangle(), new Rectangle(), 100, ground, enemies, Content));
            player = new Player(batmanpic, new Rectangle(0,0, 170, 170), new Rectangle(200, ground - 100, 50, 110), 100, ground, Content);
            menu = new MainMenu(GraphicsDevice, Content);
            penguin = new Penguin(penguinPic, new Rectangle(2000, ground - 200, 200, 120), new Rectangle(2000, ground - 100, 80, 100), 100, player);
            instructions = new Instructions(GraphicsDevice, Content);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            pad1 = GamePad.GetState(PlayerIndex.One);
            KeyboardState kb = Keyboard.GetState();

            
            if (kb.IsKeyDown(Keys.D))  //testing purposes.
            {

            }

            if (state == 0)
            #region menu state
            {
                menu.update(pad1, oldpad1);

                if (menu.getOptionChoice() == "Play")
                {
                    state = 2;
                    player.setContinue(true);
                    StartGame();
                    menu.stopMusic();
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.2f;
                    MediaPlayer.Play(mainSong);
                }

                if (menu.getOptionChoice() == "Instructions")
                    state = 4;
                   
                if (menu.getOptionChoice() == "Quit")
                    this.Exit();

                if (menu.getOptionChoice() == "Endless")
                {
                    state = 3;
                    StartGame();
                    menu.stopMusic();
                    MediaPlayer.IsRepeating = true;
                    MediaPlayer.Volume = 0.2f;
                    MediaPlayer.Play(mainSong);
                }
                if (menu.getOptionChoice() == "Testing")
                {
                    menu.stopMusic();
                    state = 1;
                }
                if (menu.getOptionChoice() == "Double Trouble")
                {
                    menu.stopMusic();
                    state = 6;
                }
            }
            #endregion
            
            else if (state == 4)
            #region instructions state
            {
                instructions.Update(pad1); //updates instructions
                if (instructions.getExit()) //if the instruction have been exited from in the class
                {
                    state = 0; //goes back to the main menu and resets variables
                    menu.setOptionChoice("");
                    instructions.setExit(false);
                }
            } 
            #endregion

            else if (state == 1)
            #region testing mode
            {

                if (pad1.Buttons.Start == ButtonState.Pressed)
                {
                    player.setContinue(true);
                    state = 2;
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].setHealth(0);
                    }
                }

                if (pad1.Buttons.RightShoulder == ButtonState.Pressed)
                {
                    state = 3;
                    player.setContinue(true);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].setHealth(0);
                    }

                }

                //skips to joker boss fight
                if (pad1.Buttons.LeftShoulder == ButtonState.Pressed)
                {
                    player.testDistance(4150);
                    progression = 13;
                    state = 2;
                    player.setContinue(true);
                    for (int i = 0; i < enemies.Count; i++)
                    {
                        enemies[i].setHealth(0);
                    }

                }
            } 
            #endregion

            else if (state == 2) 
            #region StoryMode
            {
                /* ***************************************Story mode info******************************************
                 * bunch of chunks of triggered events
                 * first checks if distance travelled is in the bounds to trigger attack event
                 * first time you are in the area it sets your ability to continue to false and start spawning enemies
                 * increase progression so it doesn't trigger a second time
                 * when enemies fall below a certain amount spawn a few more enemies
                 * if all spawns have finished and all enemies have died you can start progressing again
                 */

                #region Penguin Level
                if (player.getDistance() < 2100) //if in the first half of the game, adding this if decreases the max posible if checks
                {
                    if (player.getDistance() > 500 && player.getDistance() < 520) //if between 500 and 520 distance start this event
                    {
                        if (progression == 0) //first trigger
                        {
                            progression++; //increase progression so this doesn't trigger again
                            player.setContinue(false); //can't keep scrolling screen
                            SpawnEnemies(2, rng, true); //spawn enemies
                            SpawnEnemies(2, rng, false);
                        }
                        else if (progression == 1) //if first wave have already spawned
                        {
                            if (enemies.Count < 3) //after a certain amount of enemies have died
                            {
                                progression++; //increase progression so this can't happen again
                                SpawnEnemies(2, rng, false); //spawn more enemies
                            }
                        }
                        else if (progression == 2) //repeat
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(2, rng, true);
                            }
                        }
                        else if (progression == 3) //if all enemies have been spawned
                            if (enemies.Count == 0) //if all enemies have died you can continue again
                                player.setContinue(true);
                    }
                    else if (player.getDistance() > 1000 && player.getDistance() < 1020) //see story mode info
                    {
                        if (progression == 3)
                        {
                            progression++;
                            player.setContinue(false);
                            SpawnEnemies(4, rng, true);
                        }
                        else if (progression == 4)
                        {
                            if (enemies.Count < 3)
                            {
                                progression++;
                                SpawnEnemies(3, rng, false);
                            }
                        }
                        else if (progression == 5)
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(3, rng, false);
                            }
                        }
                        else if (progression == 6)
                            if (enemies.Count == 0)
                                player.setContinue(true);
                    }
                    else if (player.getDistance() > 1500 && player.getDistance() < 1520) //see story mode info
                    {
                        if (progression == 6)
                        {
                            progression++;
                            player.setContinue(false);
                            SpawnEnemies(4, rng, false);
                        }
                        else if (progression == 7)
                        {
                            if (enemies.Count < 3)
                            {
                                progression++;
                                SpawnEnemies(3, rng, false);
                            }
                        }
                        else if (progression == 8)
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(3, rng, true);
                            }
                        }
                        else if (progression == 9)
                            if (enemies.Count == 0)
                                player.setContinue(true);
                    }
                    if (player.getDistance() > 2000 && player.getDistance() < 2020) //final event in the first half
                    {
                        if (progression == 9)
                        {
                            progression++;
                            player.setContinue(false);
                            penguin.revive();
                            penguin.setHitbox(new Rectangle(800, ground - 100, 80, 100)); //move penguin on screen
                            spawnPenguin = true; //if spawnPenguin is true then penguin will update and draw
                        }
                        else if (progression == 10)
                        {
                            if (!spawnPenguin && enemies.Count == 0)
                            {
                                progression = 0; //reset progression for next area
                                player.GainPoints(10000); //bonus points for killing boss
                                player.setContinue(true);
                            }
                        }
                    }
                }
                #endregion
                #region Joker Level
                else //second half of distance checks, divided in two for slightly less checks required to find end choice
                {
                    if (player.getDistance() > 2500 && player.getDistance() < 2520) //if batman has walked far enough to trigger this event
                    {
                        if (progression == 0)
                        {
                            progression++; //increase progression so this doesn't trigger twice
                            player.setContinue(false); //player can't keep scrolling screen
                            SpawnEnemies(2, rng, true); //spawn enemies
                            SpawnEnemies(2, rng, false);
                        }
                        else if (progression == 1) //after first wave has been triggered
                        {
                            if (enemies.Count < 3) //if enough enemies have been defeated spawn more
                            {
                                progression++; //increase progression so this doesn't trigger twice
                                SpawnEnemies(2, rng, false);
                            }
                        }
                        else if (progression == 2) //same as above
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(2, rng, true);
                            }
                        }
                        else if (progression == 3) //if all enemies have spawned
                            if (enemies.Count == 0) //and all enemies have died
                                player.setContinue(true); //you can continue again
                    }
                    else if (player.getDistance() > 3000 && player.getDistance() < 3020) //see story mode info
                    {
                        if (progression == 3)
                        {
                            progression++;
                            player.setContinue(false);
                            SpawnEnemies(4, rng, true);
                        }
                        else if (progression == 4)
                        {
                            if (enemies.Count < 3)
                            {
                                progression++;
                                SpawnEnemies(3, rng, false);
                            }
                        }
                        else if (progression == 5)
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(3, rng, false);
                            }
                        }
                        else if (progression == 6)
                            if (enemies.Count == 0)
                                player.setContinue(true);
                    }
                    else if (player.getDistance() > 3500 && player.getDistance() < 3520) //see story mode info
                    {
                        if (progression == 6)
                        {
                            progression++;
                            player.setContinue(false);
                            SpawnEnemies(4, rng, false);
                        }
                        else if (progression == 7)
                        {
                            if (enemies.Count < 3)
                            {
                                progression++;
                                SpawnEnemies(4, rng, false);
                            }
                        }
                        else if (progression == 8)
                        {
                            if (enemies.Count < 5)
                            {
                                progression++;
                                SpawnEnemies(3, rng, true);
                            }
                        }
                        else if (progression == 9)
                            if (enemies.Count == 0)
                                player.setContinue(true);
                    }
                    else if (player.getDistance() > 4000 && player.getDistance() < 4020) //see story mode info
                    {
                        if (progression == 9)
                        {
                            progression++;
                            player.setContinue(false);
                            SpawnEnemies(3, rng, true);
                            SpawnEnemies(3, rng, false);
                        }
                        else if (progression == 10)
                        {
                            if (enemies.Count < 4)
                            {
                                progression++;
                                SpawnEnemies(2, rng, true);
                                SpawnEnemies(2, rng, false);
                            }
                        }
                        else if (progression == 11)
                        {
                            if (enemies.Count < 6)
                            {
                                progression++;
                                SpawnEnemies(4, rng, true);
                            }
                        }
                        else if (progression == 12)
                        {
                            if (enemies.Count < 7)
                            {
                                progression++;
                                SpawnEnemies(4, rng, false);
                            }
                        }
                        else if (progression == 13)
                            if (enemies.Count == 0)
                                player.setContinue(true);
                    }
                    else if (player.getDistance() > 4200) //Final battle trigger
                    {
                        if (progression == 13) //initial trigger to prevent player from continuing
                        {
                            progression++;
                            player.setContinue(false);
                        }
                        if (progression == 14 && currentbg == 2 && bgcolor == 255) //if background has finished transitioning to joker's bg
                        {
                            progression++;
                            spawnJoker = true; //bool that allows joker to update and draw
                            joker[0].revive();
                        }
                        else if (progression == 15) //after joker's been spawned
                        {
                            if (joker[0].getColumn() == 6) //if joker is out of health
                            {
                                progression = 0; //reset progression so you can't get bonus points multiple times
                                player.GainPoints(10000);//gain points for beating boss
                            }
                        }
                        else if (progression == 0 && pad1.Buttons.Start == ButtonState.Pressed) //if you press start after beating game, return to main menu
                        {
                            Reset();
                        }
                    }
                }
                #endregion

                if (trainTimer != 0) //if train isn't currently going
                {
                    trainTimer--; //decrease time until train comes
                }
                
            }
            #endregion   

            else if (state == 3)
            #region Endless
           
            {
                if (player.getContinue()) //if you aren't currently in a fight
                {
                    if (player.getDistance() >= 200 * wave && player.getDistance() <= 200 * wave + 10)
                    { //if you've travelled 200 pixels from previous event
                        wave++; //increase wave count
                        player.setContinue(false); //prevent player from continuing and triggering more events
                    }
                }
                else //if you're already in a fight
                {
                    if (progression < Math.Ceiling((double)wave / 2)) //if you aren't on the final spawn of the wave
                    {
                        if (enemies.Count < wave && !spawnPenguin) //if less enemies are alive than the wave number spawn more enemies
                        {
                            if (wave % 5 == 0 && progression == Math.Ceiling((double)wave / 2) - 1) 
                            {// if last spawn on a wave that is a multiple of 5, spawn penguin instead of thugs
                                penguin.revive();
                                penguin.setHitbox(new Rectangle(800, ground - 100, 80, 100)); //move penguin on screen
                                spawnPenguin = true; //set to true so he will update and draw
                                progression++; //increase progression so it doesn't trigger again
                            }
                            else //if not time to spawn penguin then spawn thugs
                            {
                                int temp = rng.Next(wave * 2); //random number to decide how enemies will be divided between left and right
                                SpawnEnemies(temp, rng, true); //spawn random amount to the right
                                SpawnEnemies(wave * 2 - temp, rng, false); //spawn rest to the left
                                progression++;
                            }
                            if (rng.Next(3) == 1) //if this happens spawn penguin bots as well
                            {
                                for (int i = 0; i < 4; i++)
                                {//spawn 4, 2 from the left, 2 from the right
                                    int temp; 
                                    if (i % 2 == 0)
                                        temp = -30 * i; //distance of screen is changed based on i so they aren't overlapping
                                    else
                                        temp = 1000 + 30 * i;
                                    bots.Add(new PenguinBot(penBotPic, new Rectangle(temp, ground - 40, 55, 35), new Rectangle(temp, ground - 40, 20, 35), 1));
                                }
                            }
                        }
                    }

                    if (enemies.Count == 0 && !spawnPenguin) //if no enemies are left then player can continue again
                    {
                        player.setContinue(true);
                        progression = 0; //progression is reset for next wave
                    }
                }
                if (trainTimer != 0) //if train isn't currently going
                {
                    trainTimer--; //decrease time left until train
                }
            }
            #endregion

            else if (state == 6)
            #region Double Joker Mode
            {
                if (progression == 0)
                {
                    progression += 1;
                    joker[0].setHitbox(new Rectangle(800, ground - 90, 45, 100));
                    joker[0].setHealth(300);
                    joker[1].setHitbox(new Rectangle(200, ground - 90, 45, 100));
                    joker[1].setHealth(300);
                    spawnJoker = true;
                }
            }
            #endregion


            #region Updates Entities
            if ((state == 2 || state == 3 || state == 6) && !paused) //if in a playing state update people
            {
                player.Update(pad1, oldpad1, Content, kb, enemies, joker, penguin); //update player

                if (player.getColumn() == 10 && pad1.Buttons.Start == ButtonState.Pressed) //if player is dead and you press start, return to main menu
                    Reset();

                if (spawnJoker) //if joker is spawned then update him
                {
                    if (state != 6) //if not in double joker mode only update one
                    {
                        joker[0].update(player, joker[0], GraphicsDevice);
                        if (joker[0].getBombToss()) //if joker is throwing a bomb, spawn a bomb
                        {
                            bombs.Add(new Bomb(new Rectangle(), bombPic, joker[0].getRunDir(), joker[0], ground, Content));
                            joker[0].setBombToss(false);
                        }
                    }
                    else //if in double joker mode, update both jokers
                    {
                        for (int i = 0; i < joker.Count; i++)
                        {
                            joker[i].update(player, joker[i], GraphicsDevice);
                            if (joker[i].getBombToss()) //if joker is throwing a bomb, spawn a bomb
                            {
                                bombs.Add(new Bomb(new Rectangle(), bombPic, joker[i].getRunDir(), joker[i], ground, Content));
                                joker[i].setBombToss(false);
                            }
                        }
                    }
                }
                if (spawnPenguin) //if penguin is currently spawned, update him
                {
                    penguin.Update(Content, enemies, GraphicsDevice);
                }
                for (int i = 0; i < enemies.Count; i++) //updates all of the enemies
                {
                    enemies[i].Update(player, Content);
                }
                for (int i = 0; i < bots.Count; i++) //updates all of the bots
                {
                    bots[i].Update(Content, player, enemies);
                    if (bots[i].getHealth() <= 0) //if any of the bots have 0 hp, replaces them
                        bots.RemoveAt(i);
                }
                for (int i = 0; i < bombs.Count; i++) //updates all of the bombs
                {
                    bombs[i].update(Content, player, bombs, i, joker);
                }
                for (int i = 0; i < drops.Count; i++) //updates all of the drops
                {
                    drops[i].Update(player, drops, i);
                }
                //remove excess enemies
                for (int i = 0; i < enemies.Count; i++)
                {
                    if (enemies[i].getHealth() <= 0)
                    {
                        if (rng.Next(3) == 1)
                        {
                            drops.Add(new Items(enemies[i].getRec().X + 50, ground, rng.Next(60), player.getDistance()));
                        }
                        enemies.RemoveAt(i);
                        player.GainPoints(1000);
                    }
                }
                if (trainTimer == 0) //if the timer is at 0 move the train across the screen
                {
                    trainRec.X += 10;
                    if (trainRec.X > 1000)
                    { //when train leaves screen again reset train position and timer is set for 30 sec
                        trainRec.X = -500;
                        trainTimer = 1800;
                    }
                }

            }
            #endregion
            if (state == 6)
            {
                if (progression == 0)
                {
                    progression += 1;
                    joker[0].revive();
                    joker[1].revive();
                    spawnJoker = true;
                }
            }

            #region Pause Menu
            if (state == 2 || state == 3 || state == 6) { //if in a playable state.
                if (pad1.Buttons.Start == ButtonState.Pressed && oldpad1.Buttons.Start != ButtonState.Pressed && !pausing) {
                    pauseOptionSelector = 0;
                    pauseSelection[0] = true;
                    pauseSelection[1] = false;
                    pauseOptionColors[0] = Color.Red;
                    pauseOptionColors[1] = Color.White;

                    paused = !paused;
                    pauseAccel = 0;
                    if (paused) {
                        pauseOverlay.Y = 0;
                        pauseOverlay.Width = 0;
                    }
                }

                /*------------- DOCUMENTATION --------------
                        PauseSelection[0] = "Resume" 
                        PauseSelection[1] = "Quit to Menu"
                ------------------------------------------ */

                if (paused) {
                    MediaPlayer.Pause();
                    if (pad1.DPad.Down == ButtonState.Pressed && oldpad1.DPad.Down != ButtonState.Pressed) { //going down options
                        if (pauseOptionSelector < pauseSelection.Length - 1) {
                            pauseOptionSelector++;
                        }
                    } else if (pad1.DPad.Up == ButtonState.Pressed && oldpad1.DPad.Up != ButtonState.Pressed) { //going up options
                        if (pauseOptionSelector > 0) {
                            pauseOptionSelector--;
                        }
                    }

                    //determine selected option.
                    for (int i = 0; i < pauseSelection.Length; i++) {
                        pauseSelection[i] = false;
                        pauseOptionColors[i] = Color.White;
                    }
                    pauseSelection[pauseOptionSelector] = true;
                    pauseOptionColors[pauseOptionSelector] = Color.Red;

                    for (int i = 0; i < pauseSelection.Length; i++) {
                        //RESUME GAME
                        if (pauseSelection[0] == true) {
                            if (pad1.Buttons.A == ButtonState.Pressed && oldpad1.Buttons.A != ButtonState.Pressed) {
                                paused = false;
                            }
                        }
                        //QUIT TO MENU
                        if (pauseSelection[1] == true) {
                            if (pad1.Buttons.A == ButtonState.Pressed && oldpad1.Buttons.A != ButtonState.Pressed) {
                                paused = false;
                                Reset();
                            }
                        }
                    }
                    //public bool aDown() { 
                    //    if (pad1.Buttons.A == ButtonState.Pressed && oldpad1.Buttons.A != ButtonState.Pressed){
                    //        return true;
                    //    } else {
                    //        return false;
                    //    }  
                    //}

                    //pause game animation
                    if (pauseOverlay.Width <= 1200) {
                        pauseAccel += 3;
                        pauseOverlay.Width += pauseAccel;
                        pausing = true;
                    } else {
                        pauseAccel = 0;
                        pausing = false;
                    }
                } else { //resume game animation
                    pauseAccel += 3;
                    if (pauseOverlay.Y <= 900) {
                        pauseOverlay.Y += pauseAccel;
                    } else {
                        pausing = false;
                        MediaPlayer.Resume();
                    }
                }
                #endregion
            }
           
            

            oldpad1 = pad1; //for edge detection

            //======================================Ability to Delete Penguin==============================================
            if (penguin.getHealth() < 0)
            {
                penguin.setHitbox(new Rectangle(-100, -100, 0, 0)); //moves the penguin's hitbox off of the screen
                spawnPenguin = false;
            }

            base.Update(gameTime);
        }

      
        protected override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null); //stole this from zuhane on dream.in.code

            //=================================Background Drawing -Alex==============================================
            if ((player.getDistance() > 4200 && state == 2) || state == 6) //if in double joker or at joker fight
            {
                if (currentbg == 1)  //if still on normal background
                {
                    spriteBatch.Draw(bgPic, bgRec, new Rectangle(player.getDistance() % 1194, 0, 640, 360), new Color(bgcolor, bgcolor, bgcolor));
                    bgcolor--; //slowly fade to black
                    if (bgcolor <= 10) //if dark enough
                    {
                        currentbg = 2; //switch to carnival picture
                        bgPic = Content.Load<Texture2D>("bg2");
                    }
                }
                else //if on carnival picture
                {
                    bgcolor++; //increase slowly to normal
                    if (bgcolor > 255) //prevents color from going over
                    {
                        bgcolor = 255;
                    }
                    spriteBatch.Draw(bgPic, bgRec, new Color(bgcolor, bgcolor, bgcolor));
                }
            }
            else //if not fighting joker draw normal background
            {
                spriteBatch.Draw(bgPic, bgRec, new Rectangle(player.getDistance() % 1194, 0, 640, 360), Color.White);
                spriteBatch.Draw(trainPic, trainRec, Color.White);
            }
            spriteBatch.Draw(groundPic, groundRec, Color.Black);
            if (player.getDistance() > 4200 && progression == 0)
            {
                spriteBatch.DrawString(font, "Press start to return to the main menu", new Vector2(500 - font.MeasureString("Press start to return to the main menu").X / 2, 200), Color.White);
                spriteBatch.DrawString(font, "You won with a score of " + player.getScore(), new Vector2(500 - font.MeasureString("You won with a score of " + player.getScore()).X / 2, 300), Color.White);
            }
            #region In Game UI
            //=================================UI Drawing -Nick==============================================
            if (state > 1) //if in the main game
            {
                #region Health Bar
                //creats the rectangles that make up the health bar
                healthBar1 = new Rectangle(10, 10, 75, 75);
                healthBar2 = new Rectangle(85, 10, Convert.ToInt32(player.getHealth() * 1.5), 75); //this rectangle increases/decreases size based on the health of the player
                healthBar3 = new Rectangle(85 + Convert.ToInt32(player.getHealth() * 1.5), 10, Convert.ToInt32(10 * Math.Ceiling((double)player.getHealth() / 100)), 75);

                //draws the health bar
                spriteBatch.Draw(healthPic1, healthBar1, Color.White);
                spriteBatch.Draw(healthPic2, healthBar2, Color.White);
                spriteBatch.Draw(healthPic3, healthBar3, Color.White); 
                #endregion

                #region Score + Ammo
                spriteBatch.DrawString(font, "Score: " + player.getScore().ToString(), new Vector2(75, -4), Color.Black); //draws the score text at the top of the screen

                for (int i = 0; i < player.getAmmo(); i++) //for the vlue of the players ammo
                {
                    spriteBatch.Draw(batarangPic, new Rectangle(95 + 40 * (i + 1), 80, 30, 20), Color.White); //draws an ammo pic att he top right of the screen
                }
                spriteBatch.DrawString(font, "Ammo:", new Vector2(75, 70), Color.Black);  //draws the ammo text
                #endregion
                
                #region Combo Code
                //this block of code changes the colour of the combo image based on the value of the combo
                if (player.getCombo() < 5)
                {
                    originalComboBoxColr = Color.Purple;
                }
                else if (player.getCombo() < 10)
                {
                    originalComboBoxColr = Color.DarkBlue;
                }
                else if (player.getCombo() < 15)
                {
                    originalComboBoxColr = Color.CornflowerBlue;
                }
                else if (player.getCombo() < 20)
                {
                    originalComboBoxColr = Color.LimeGreen;
                }
                else if (player.getCombo() < 25)
                {
                    originalComboBoxColr = Color.Yellow;
                }
                else if (player.getCombo() < 30)
                {
                    originalComboBoxColr = Color.Orange;
                }
                else if (player.getCombo() > 35)
                {
                    originalComboBoxColr = Color.Red;
                }

                if (comboRecTimer < 10) //for the first 10 ticks increases the size of the combo box and makes it white
                {
                    comboBox.Width++;
                    comboBox.Height++;
                    comboBoxColr = Color.White;
                }
                else if (comboRecTimer < 20) //for the next 10 ticks decreases the size of the combo image and changes the colour to the respective one
                {
                    comboBox.Width--;
                    comboBox.Height--;
                    comboBoxColr = originalComboBoxColr;
                }
                else if (comboRecTimer < 30) //for the next 10 ticks increases the size of the combo box and makes it white
                {
                    comboBox.Width++;
                    comboBox.Height++;
                    comboBoxColr = Color.White;
                }
                else if (comboRecTimer < 40) //for the next 10 ticks decreases the size of the combo image and changes the colour to the respective one
                {
                    comboBox.Width--;
                    comboBox.Height--;
                    comboBoxColr = originalComboBoxColr;
                }
                if (player.getColumn() == 12) //if the upercut is being played, displays the kapow text
                {
                    comboText = "KAPOW";

                }
                if (oldCombo != player.getCombo()) // if the combo value changes
                {
                    comboText = player.getCombo().ToString(); 
                    comboBox = new Rectangle(player.getRec().X + (player.getRec().Width / 2) - 40, player.getRec().Y - 50, 80, 80); //displays the combo box at batmans current location

                    comboRecTimer = 0;

                }


                if (player.getCombo() > 0 || player.getColumn() == 12) //if the combo is greater than zero
                {
                    comboVector = new Vector2(comboBox.X + (comboBox.Width / 2) - (comboFont.MeasureString(comboText).X / 2), comboBox.Y + (comboBox.Height / 2) - (comboFont.MeasureString(comboText).Y / 2) + 5); // draws the combo value in the center of the box
                    spriteBatch.Draw(comboPic, comboBox, comboBoxColr); //draws the combo box
                    spriteBatch.DrawString(comboFont, comboText, comboVector, Color.White); //draws the combo text
                    comboRecTimer++;
                } 
                #endregion

                if (state == 3) 
                {
                    spriteBatch.DrawString(font, "Wave: " + wave.ToString(), new Vector2(GraphicsDevice.Viewport.Width / 2, 0), Color.Black); //displays the wave at the top middle of the screen
                }
            } 
            #endregion

            oldCombo = player.getCombo();
            //==============================================================================================
            if (spawnJoker)
            {
                joker[0].draw(spriteBatch, Content);   //draw joker  
                for (int i = 0; i < bombs.Count; i++)   //draw bombs
                    bombs[i].draw(spriteBatch);
                if (state == 6)
                    joker[1].draw(spriteBatch, Content);
            }

            for (int i = 0; i < bots.Count; i++)
            {
                bots[i].Draw(spriteBatch);
            }

                for (int i = 0; i < enemies.Count; i++) //draw thugs
                {
                    enemies[i].Draw(spriteBatch);
                }
            for (int i = 0; i < drops.Count; i++) //draw drops
            {
                drops[i].Draw(spriteBatch, Content);
            }
            if (spawnPenguin)
                penguin.Draw(spriteBatch, enemies); //draw penguin  

            if (paused) {
                spriteBatch.Draw(bgPic, pauseOverlay, Color.Black);
                spriteBatch.DrawString(font, "Paused", new Vector2(100 , 100), Color.White);

                spriteBatch.DrawString(font, "Resume", new Vector2(100, 200), pauseOptionColors[0]);
                spriteBatch.DrawString(font, "Quit", new Vector2(100, 250), pauseOptionColors[1]);
            }
            player.Draw(spriteBatch, font); //draw batman

            //main menu stuff ==================================================
            if (state == 0)
            {
                menu.draw(spriteBatch, Content);
                

            }
            if (state == 4)
            {
                instructions.Draw(spriteBatch);
            }

            if (state == 1)
            {
                spriteBatch.DrawString(font, "Press Left Bumper to skip to Joker boss fight.", new Vector2(200, 550), Color.White);
            }
          

            spriteBatch.End();
            base.Draw(gameTime); 
            
        }
        //method that spawns enemies, will spawn amount of enemies wanted in direction wanted - Alex
        public void SpawnEnemies(int numberofenemies, Random rng, Boolean right) 
        {
            int temp; //temp int that holds where enemy will spawn
            for (int i = 0; i < numberofenemies; i++) 
            {
                if (right) //if right, spawns to the right of screen
                    temp = 1000 + rng.Next(50) + i * 80; //rng is so it doesn't look as uniform, i * 80 ensures they don't overlap each other
                else //if not right, spawns to the left of screen
                    temp = -100 - rng.Next(50) - i * 80; 
                enemies.Add(new AIEnemy(thugPic, new Rectangle(temp, 360, 100, 120), new Rectangle(temp, 360, 100, 120), 100, rng.Next(3,5)));
                enemies[enemies.Count - 1].setRow(rng.Next(4)); //set them to a random idle frame so they aren't all synced up
            }
        }
        public void Reset() //method to return to main menu  - Alex
        {
            player.testDistance(0); //reset player's progression and stats
            player.setColumn(0);
            player.setHealth(100);
            player.GainPoints(-player.getScore());
            progression = 0;
            currentbg = 1;
            wave = 0;
            bgPic = Content.Load<Texture2D>("background");
            joker[0].setHitbox(new Rectangle(1000, 0, 0, 0));
            spawnJoker = false;
            penguin.setHitbox(new Rectangle(1000, 0, 0, 0));
            spawnPenguin = false;
            menu.reset();
            state = 0; //set state back to main menu
        }

        public void StartGame() //method run when a playing state begins - Alex
        {
            player.setHealth(100); //set player stuff to max
            player.setAmmo(3); 
            player.setHitbox(new Rectangle(200, ground - 100, 50, 110)); //move player on screen
            player.setColumn(0); //set player into idle
            player.setRow(0);
            for (int i = enemies.Count - 1; i >= 0; i--) //delete any enemies currently alive
                enemies.RemoveAt(i);
            for (int i = drops.Count - 1; i >= 0; i--) //delete any items currently on ground
                drops.RemoveAt(i);
        }
    }
}


