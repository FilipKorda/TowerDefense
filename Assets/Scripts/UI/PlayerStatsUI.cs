using TowerDefense.PlayerSystem;
using TMPro;
using UnityEngine;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class PlayerStatsUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private TextMeshProUGUI hpText;
        [SerializeField] private string moneyFormat = "{0}";
        [SerializeField] private string hpFormat = "{0} / {1}";

        private PlayerStats playerStats;

        private void OnEnable()
        {
            StartCoroutine(SubscribeWhenReady());
        }

        private void OnDisable()
        {
            if (playerStats != null)
            {
                playerStats.OnMoneyChanged -= HandleMoneyChanged;
                playerStats.OnHpChanged -= HandleHpChanged;
            }
        }

        private System.Collections.IEnumerator SubscribeWhenReady()
        {
            while (PlayerStats.Instance == null)
            {
                yield return null;
            }

            playerStats = PlayerStats.Instance;
            playerStats.OnMoneyChanged += HandleMoneyChanged;
            playerStats.OnHpChanged += HandleHpChanged;

            HandleMoneyChanged(playerStats.Money);
            HandleHpChanged(playerStats.CurrentHp, playerStats.MaxHp);
        }

        private void HandleMoneyChanged(int currentMoney)
        {
            if (moneyText != null)
            {
                moneyText.text = string.Format(moneyFormat, currentMoney);
            }
        }

        private void HandleHpChanged(float currentHp, float maxHp)
        {
            if (hpText != null)
            {
                hpText.text = string.Format(hpFormat, currentHp, maxHp);
            }
        }
    }
}