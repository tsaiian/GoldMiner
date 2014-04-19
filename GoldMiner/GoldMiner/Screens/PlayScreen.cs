using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using GoldMiner.Screens;

//Example on how to use the ScreenSystem.  If you wish to use it in your own
//project, grab the ScreenSystem.dll found in the ScreenSystemImplementation
//folder and paste it in your project's folder.  Once that is done, right click
//References on your code project (not your content project) and click Add
//Reference.  Click the browse tab and look for the DLL.
namespace GoldMiner
{
    public class PlayScreen : MenuScreen
    {
        string prevEntry, nextEntry, selectedEntry, cancelMenu;
        public override string PreviousEntryActionName
        {
            get { return prevEntry; }
        }

        public override string NextEntryActionName
        {
            get { return nextEntry; }
        }

        public override string SelectedEntryActionName
        {
            get { return selectedEntry; }
        }

        public override string MenuCancelActionName
        {
            get { return cancelMenu; }
        }

        MainMenuEntry single, fight,back;

        public PlayScreen()
        {
            //Set up the action names
            prevEntry = "MenuUp";
            nextEntry = "MenuDown";
            selectedEntry = "MenuAccept";
            cancelMenu = "MenuCancel";

            //Allow transitions
            TransitionOnTime = TimeSpan.FromSeconds(1);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //Customize the text colors.
            Selected = Color.Yellow;
            Highlighted = Color.Red;
            Normal = Color.White;
        }

        public override void Initialize()
        {
            //Get a reference to the input system
            InputSystem input = ScreenSystem.InputSystem;

            //Load the actions
            input.NewAction(PreviousEntryActionName, Keys.Up);
            input.NewAction(NextEntryActionName, Keys.Down);
            input.NewAction(SelectedEntryActionName, Keys.Enter);
            input.NewAction(MenuCancelActionName, Keys.Escape);

            //Initialize the entries
            single = new MainMenuEntry(this, "Single");
            fight = new MainMenuEntry(this, "Fight");
            back = new MainMenuEntry(this,"Back");

            //Set up the screen events
            Removing += new EventHandler(MainMenuRemoving);
            Entering += new TransitionEventHandler(MainMenuScreen_Entering);
            Exiting += new TransitionEventHandler(MainMenuScreen_Exiting);

            //Set up the entry events, and load a submenu.
            single.Selected += new EventHandler(SingleSelect);
            fight.Selected += new EventHandler(FightSelect);
            back.Selected += new EventHandler(BackSelect);


            //Finally, add all entries to the list
            MenuEntries.Add(single);
            MenuEntries.Add(fight);
            MenuEntries.Add(back);

            
            
        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            SpriteFont = content.Load<SpriteFont>(@"menu");

            //Initialize is called before LoadContent, so if you want to 
            //use relative position with the line spacing like below,
            //you need to do it after load content and spritefont
            single.SetPosition(new Vector2(100, 200), true);
            fight.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), single, true);
            back.SetRelativePosition(new Vector2(0, SpriteFont.LineSpacing + 5), fight, true);

            
        }

        public override void UnloadContent()
        {
            SpriteFont = null;
        }

        void MainMenuScreen_Entering(object sender, TransitionEventArgs tea)
        {
            //Slide effect from left to right
            float effect = (float)Math.Pow(tea.percent - 1, 2) * -100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.InitialPosition + entry.Acceleration;
            }
        }

        void MainMenuScreen_Exiting(object sender, TransitionEventArgs tea)
        {
            //Slide effect from right to left
            float effect = (float)Math.Pow(tea.percent - 1, 2) * 100;
            foreach (MenuEntry entry in MenuEntries)
            {
                entry.Acceleration = new Vector2(effect, 0);
                entry.Position = entry.InitialPosition - entry.Acceleration;
            }
        }

        void SingleSelect(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new SingleScreen());
        }

        void FightSelect(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new FightScreen());
        }
        void BackSelect(object sender, EventArgs e)
        {
            ExitScreen();
            ScreenSystem.AddScreen(new MainMenuScreen());
        }



        void MainMenuRemoving(object sender, EventArgs e)
        {
            MenuEntries.Clear();
        }
    }
}
