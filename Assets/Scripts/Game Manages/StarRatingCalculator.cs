using UnityEngine;

namespace TowerDefense.GameSystem
{
    public static class StarRatingCalculator
    {
        public static int CalculateStars(float currentHp, float maxHp)
        {
            if (maxHp <= 0f)
            {
                return 0;
            }

            float lostPercent = Mathf.Clamp01(1f - currentHp / maxHp);

            if (lostPercent <= 0f)
            {
                return 3;
            }

            if (lostPercent <= 0.6f)
            {
                return 2;
            }

            return 1;
        }
    }
}