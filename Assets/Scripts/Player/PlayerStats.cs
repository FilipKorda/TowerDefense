using System;
using TowerDefense.GameSystem;
using UnityEngine;

namespace TowerDefense.PlayerSystem
{
    [DisallowMultipleComponent]
    public class PlayerStats : MonoBehaviour
    {
        [SerializeField] private int startingMoney = 100;

        public static PlayerStats Instance { get; private set; }

        public int Money { get; private set; }
        public float CurrentHp { get; private set; }
        public float MaxHp { get; private set; }
        public bool IsAlive => CurrentHp > 0f;

        public event Action<int> OnMoneyChanged;
        public event Action<float, float> OnHpChanged;
        public event Action OnPlayerDied;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            float resolvedHp = GameSession.SelectedDifficulty.PlayerHp;

            Money = startingMoney;
            MaxHp = resolvedHp;
            CurrentHp = resolvedHp;
        }

        private void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void AddMoney(int amount)
        {
            if (amount <= 0)
            {
                return;
            }

            Money += amount;
            OnMoneyChanged?.Invoke(Money);
        }

        public bool TrySpendMoney(int amount)
        {
            if (amount <= 0)
            {
                return true;
            }

            if (Money < amount)
            {
                return false;
            }

            Money -= amount;
            OnMoneyChanged?.Invoke(Money);
            return true;
        }

        public void TakeDamage(float amount)
        {
            if (!IsAlive || amount <= 0f)
            {
                return;
            }

            CurrentHp = Mathf.Max(0f, CurrentHp - amount);
            OnHpChanged?.Invoke(CurrentHp, MaxHp);

            if (CurrentHp <= 0f)
            {
                OnPlayerDied?.Invoke();
            }
        }
    }
}