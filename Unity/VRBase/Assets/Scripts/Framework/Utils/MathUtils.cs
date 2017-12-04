using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HDJ.Framework.Utils
{
    public static class MathUtils
    {

        /// <summary>
        /// 返回二维向量的垂直向量
        /// </summary>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static Vector2[] VerticalVector2(Vector2 v2)
        {
            v2 = v2.normalized;
            Vector2[] res = new Vector2[2];
            float y = 1f / (v2.y * v2.y / (v2.x * v2.x) + 1);
            y = Mathf.Sqrt(y);
            float x = -v2.y * y / v2.x;
            res[0] = new Vector2(x, y);
            res[1] = new Vector2(-x, -y);

            return res;
        }
        /// <summary>
        /// 使用特定的概率，随机结果
        /// </summary>
        /// <param name="probability"></param>
        /// <returns></returns>
        public static bool RandomProbability(float probability)
        {
            if (probability <= 0)
                return false;
            if (probability >= 1f)
                return true;

            int tempProbab = (int)(probability * 100);

            int getNum = UnityEngine.Random.Range(0, 100);

            if (getNum <= tempProbab)
                return true;
            else
                return false;

        }
    }


}
