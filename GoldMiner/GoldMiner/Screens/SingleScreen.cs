using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScreenSystemLibrary;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.IO;
using System.Diagnostics;

namespace GoldMiner
{
    /// <summary>
    /// Sample play screen.  No new features are presented here,
    /// so there are no comments currently.
    /// </summary>
    public class SingleScreen : GameScreen
    {

        public override bool AcceptsInput
        {
            get { return true; }
        }

        GraphicsDeviceManager graphics;
        Texture2D backgroundTexture;
        Rectangle viewportRect;

        float seconds;
        SpriteFont font;
        InputSystem input;
        float clipDegree;
        bool clipLeft = true;
        bool grabbing = false, backGrabbing = false;
        int backGrabbingNo;
        int goalOfTotalMoney;
        int grabPosition = 0;
        double clipX, clipY, clipSlope;
        int startLevelTime;


        //@@@@@@@@@@@@@@@@@@@@@modified by 991558
        const int peopleX = 375, peopleY = 65;
        //@@@@@@@@@@@@@@@@@@@@@
        const int grabSpeed = 10;
        const int backGrabSpeed = 2;     // need >=1, 數字越大，拉得越慢
        const int totalLevel = 4;
        const int timeForEachLevel = 60;    //每關要在60秒內夾完

        //@@@@@@@@@@@@@@@@@@@@modified by 991558
        int nowLevel = 0;
        int grabbedMoney = 0;
        //@@@@@@@@@@@@@@@@@@@
        List<Gold> levelGold;
        Texture2D goldTexture;
        Texture2D giftTexture;
        Texture2D stoneTexture;
        Texture2D clipTexture;
        Texture2D blackLine;
        Texture2D [] manTexture;

        public override void Initialize()
        {
            TransitionOnTime = TimeSpan.FromSeconds(1);
            input = ScreenSystem.InputSystem;
            input.NewAction("Pause", Keys.Escape);
            input.NewAction("Grab", Keys.Space);

            Entering += new TransitionEventHandler(PlayScreen_Entering);
        }

        void NextLevel()
        {
            if (nowLevel >= totalLevel)
            {
                Debug.WriteLine("There is no more level.");
                return;
            }

            startLevelTime = (int)seconds;
            nowLevel++;
            levelGold.Clear();
            string levelPath = string.Format("Content/Levels/{0}.txt", nowLevel);

            using (StreamReader reader = new StreamReader(levelPath))
            {
                string line = reader.ReadLine();
                goalOfTotalMoney = Convert.ToInt16(line);
                line = reader.ReadLine();
                while (line != null)
                {
                    string[] token = line.Split(' ');
                    Gold gold = new Gold((float)Convert.ToInt16(token[0]), (float)Convert.ToInt16(token[1]), Convert.ToInt16(token[2]), Convert.ToInt16(token[3]));//---------------
                    levelGold.Add(gold);
                    line = reader.ReadLine();
                }
            }
        }

        void PlayScreen_Entering(object sender, TransitionEventArgs tea)
        {

        }

        public override void LoadContent()
        {
            ContentManager content = ScreenSystem.Content;

            backgroundTexture = content.Load<Texture2D>("gaming");
            font = content.Load<SpriteFont>("gamefont");
            clipTexture = content.Load<Texture2D>("clip");
            blackLine = content.Load<Texture2D>("blackLine");
            manTexture = new Texture2D[25];
            for (int i = 0; i < 25; i++)
                manTexture[i] = content.Load<Texture2D>("pull" + (i + 1));

            blackLine.SetData(new[] { Color.White });
            
            goldTexture = content.Load<Texture2D>("gold");
            stoneTexture = content.Load<Texture2D>("stone");
            giftTexture = content.Load<Texture2D>("gift");

            levelGold = new List<Gold>();
            NextLevel();
        }

        public override void UnloadContent()
        {
            font = null;
        }


        public override void HandleInput()
        {
            if (input.NewActionPress("Pause"))
            {
                FreezeScreen();
                ScreenSystem.AddScreen(new PauseScreen(this));
            }
            if (input.NewActionPress("Grab"))
            {
                if (!grabbing && !backGrabbing)
                {
                    grabbing = true;
                    grabPosition = 0;
                }

            }
        }
        private int count = 0;
        protected override void UpdateScreen(GameTime gameTime)
        {
            count++;
            seconds += (float)gameTime.ElapsedGameTime.TotalSeconds;
            clipSlope = Math.Tan(clipDegree) * (-1);
            double tempClipY = Math.Sqrt(grabPosition * grabPosition / (1 + clipSlope * clipSlope));
            double tempClipX = tempClipY * clipSlope;
            clipY = tempClipY + peopleY;
            clipX = tempClipX + peopleX;

            if (clipDegree > 1.57)
                clipLeft = false;
            if (clipDegree < -1.57)
                clipLeft = true;

            if (clipLeft && !grabbing && !backGrabbing)
                clipDegree += (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (!clipLeft && !grabbing && !backGrabbing)
                clipDegree -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            else if (grabbing)
                grabPosition += grabSpeed;
            else if (backGrabbing)
            {
                if (count % backGrabSpeed == 0)
                    grabPosition -= (int)((4 - levelGold[backGrabbingNo].weight));
                levelGold[backGrabbingNo].x = (float)clipX - levelGold[backGrabbingNo].scale.X * 19;
                levelGold[backGrabbingNo].y = (float)clipY - levelGold[backGrabbingNo].scale.X * 17;
            }

            //grab out of windows
            if (clipX > 800 || clipX < 0 || clipY > 480)
            {
                grabbing = false;
                grabPosition = 0;

            }

            
            if (grabbing)
            {
                for (int i = 0; i < levelGold.Count; i++)
                {
                    if (clipX > levelGold[i].x && clipX < levelGold[i].x + 38 * levelGold[i].scale.X && clipY > levelGold[i].y && clipY < levelGold[i].y + 34 * levelGold[i].scale.X)
                    {
                        grabbing = false;
                        backGrabbing = true;
                        backGrabbingNo = i;
                    }
                }
            }
            //////判斷抓到的物品的類型type以及weight的大小，來給grabbedMoney賦值     modify by 991557
            if (backGrabbing && grabPosition <= 0)
            {
                grabbedMoney += levelGold[backGrabbingNo].money();
                
                backGrabbing = false;
                levelGold.RemoveAt(backGrabbingNo);


                //全部抓完，直接下一關
                if (levelGold.Count == 0)
                    NextLevel();
            }
            //////modify by 991557
            //判斷是否抓到足夠的金礦 且時間到了進入如下一關 modified by 991558
            if ((grabbedMoney >= goalOfTotalMoney) && (timeForEachLevel - (int)seconds + startLevelTime == 0))
            {
                NextLevel();
            }

            //時間到 => game over且所抓的金額數沒達到本關的標準
            if ((grabbedMoney < goalOfTotalMoney) && (timeForEachLevel - (int)seconds + startLevelTime <= 0))
            {
                FreezeScreen();

                /*以後可能在這邊設計Try again的選項*/
            }
            
        }
        void  DrawLine(SpriteBatch batch, Texture2D blank, Vector2 startPoint, float angle, float length)
        {
            batch.Draw(blank, startPoint, null, Color.Black, angle, Vector2.Zero, new Vector2(length, 1), SpriteEffects.None, 0);
        }

        int temp = 0;
        protected override void DrawScreen(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenSystem.SpriteBatch;
            spriteBatch.Draw(backgroundTexture,new Rectangle(0,0,800,480), Color.White);
            spriteBatch.DrawString(font, "" + grabbedMoney, new Vector2(120, 6), Color.White);//現有金錢
            spriteBatch.DrawString(font, (timeForEachLevel - (int)seconds + startLevelTime).ToString(), new Vector2(700, 6), Color.White);//剩餘時間
            
           
            //目標得分
            spriteBatch.DrawString(font, goalOfTotalMoney.ToString(), new Vector2(120, 30), Color.White);

            //級數
            spriteBatch.DrawString(font, "" + nowLevel, new Vector2(700, 26), Color.White);

            //到達哪一級目標金額的限制
            spriteBatch.DrawString(font, "" + goalOfTotalMoney, new Vector2(120, 30), Color.White);

            //draw clip
            Vector2 clipScale = new Vector2((float)0.1);
            if (!grabbing && !backGrabbing)
            {
                spriteBatch.Draw(manTexture[0], new Vector2(350, 0), null, Color.White, 0.0f, new Vector2(0, 0), 0.59f, SpriteEffects.None, 0);
                spriteBatch.Draw(clipTexture, new Vector2(peopleX, peopleY), null, Color.White, clipDegree, new Vector2(285, 0), clipScale, SpriteEffects.None, 0);
            }
            else
            {
                temp++;
                if (temp >= 25) temp = 0;
                spriteBatch.Draw(manTexture[temp], new Vector2(350, 0), null, Color.White, 0.0f, new Vector2(0, 0), 0.59f, SpriteEffects.None, 0);
                spriteBatch.Draw(clipTexture, new Vector2(((float)clipX), (float)clipY), null, Color.White, clipDegree, new Vector2(285, 0), clipScale, SpriteEffects.None, 0);
            }
            //draw line
            DrawLine(spriteBatch, blackLine, new Vector2(peopleX, peopleY), (float)1.57 + clipDegree, grabPosition);




            for (int i = 0; i < levelGold.Count; i++)
            {
                if(levelGold[i].type == 1)
                {
                    spriteBatch.Draw(goldTexture, new Vector2(levelGold[i].x, levelGold[i].y), null, Color.White, (float)0, new Vector2(0, 0), levelGold[i].scale, SpriteEffects.None, 0);
                }
                else if(levelGold[i].type == 2)
                {
                    spriteBatch.Draw(stoneTexture, new Vector2(levelGold[i].x, levelGold[i].y), null, Color.White, (float)0, new Vector2(0, 0), levelGold[i].scale, SpriteEffects.None, 0);
                }
                else if (levelGold[i].type == 3)
                {
                    spriteBatch.Draw(giftTexture, new Vector2(levelGold[i].x, levelGold[i].y), null, Color.White, (float)0, new Vector2(0, 0), levelGold[i].scale, SpriteEffects.None, 0);
                }
            }
                

            
        }
    }
}
