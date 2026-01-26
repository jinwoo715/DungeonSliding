using UnityEngine;
namespace JW.Utility
{
    public class MathUtil : MonoBehaviour
    {
        public static int GetFib(int offset, int scale = 1)
        {
            if (offset <= 0) return 0;

            if (offset < 3) return 1 * scale;

            int a = 1;
            int b = 1;

            for (int i = 3; i <= offset; i++)
            {
                int c = a + b;
                a = b;
                b = c;
            }

            return b * scale;
        }

        // --- Easing functions ---
        //https://easings.net/ko
        public static float EaseOutCubic(float t)
        {
            float p = 1f - t;
            return 1f - (p * p * p);
        }

        public static float EaseOutQuart(float t)
        {
            float p = 1f - t;
            return 1f - (p * p * p * p);
        }

        public static float EaseOutExpo(float t)
        {
            return (t >= 1f) ? 1f : 1f - Mathf.Pow(2f, -10f * t);
        }
    }
}
