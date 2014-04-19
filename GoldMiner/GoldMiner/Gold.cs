using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace GoldMiner
{
    public class Gold
    {
        public Gold(float x1, float y1, int w,int t)
        {
            x = x1;
            y = y1;
            weight = w;
            scale = new Vector2((float)w);
            type = t;
            //showInfo();
        }
        public float x { get; set; }
        public float y { get; set; }
        public int weight { get; set; }
        public int type { get; set; }// 1-----gold    2-----stone   3-----gift
        public Vector2 scale { get; set; }

        public int money()
        {
            if (type == 1)
            {//gold
                if (weight == 1)
                    return 100;
                else if (weight == 2)
                    return 200;
                else if (weight == 3)
                    return 300;
            }
            else if (type == 2)
            {//stone
                if (weight == 1)
                    return 30;
                else if (weight == 2)
                    return 40;
                else if (weight == 3)
                    return 50;
            }

            //gift
            return 200;

        }

        public void showInfo()
        {
            Debug.WriteLine("X :{0}", x);
            Debug.WriteLine("Y :{0}", y);
            Debug.WriteLine("weight :{0}", weight);
            Debug.WriteLine("type :{0}", type);
        }
    }
}
