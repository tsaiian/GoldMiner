using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace GoldMiner.Screens
{
    class IntroductionScreen : GameScreen
    {
        public override bool AcceptsInput
        {
            get { return true; }
        }

        SpriteFont font;
        string title;
        Color titleColor;
        Vector2 position;
        InputSystem input;
        public override void Initialize()
        {
            title = "Introduction";
            titleColor = Color.White;

            position = new Vector2(100, 200);

            input = ScreenSystem.InputSystem;
            input.NewAction("Pause", Keys.Escape);

        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;
            font = content.Load<SpriteFont>("gamefont");
        }

        protected override void UpdateScreen(GameTime gameTime)
        {

        }

        public override void UnloadContent()
        {
            font = null;
        }

        public override void HandleInput()
        {
            if (input.NewActionPress("Pause"))
            {
                ExitScreen(); ;
                ScreenSystem.AddScreen(new MainMenuScreen());
            }
        }

        protected override void DrawScreen(GameTime gameTime)
        {
            position = new Vector2(100, 200);
            SpriteBatch spriteBatch = ScreenSystem.SpriteBatch;
            spriteBatch.DrawString(font, title, position, titleColor);
            position = Vector2.Add(position, new Vector2(0, font.LineSpacing + 10));
        }
    }
}
