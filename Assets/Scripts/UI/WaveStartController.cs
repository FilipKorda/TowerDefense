using TMPro;
using TowerDefense.EnemySystem;
using UnityEngine;
using UnityEngine.UI;

namespace TowerDefense.UI
{
    [DisallowMultipleComponent]
    public class WaveStartController : MonoBehaviour
    {
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private Button startWaveButton;

        private void Awake()
        {
            if (startWaveButton != null)
            {
                startWaveButton.onClick.AddListener(HandleButtonClicked);
            }
        }

        private void OnEnable()
        {
            if (spawner != null)
            {
                spawner.OnWaveChanged += HandleWaveChanged;
                spawner.OnWaveFinished += HandleWaveFinished;
                spawner.OnAllWavesCompleted += HandleAllWavesCompleted;
            }

            RefreshButtonState();
        }

        private void OnDisable()
        {
            if (spawner != null)
            {
                spawner.OnWaveChanged -= HandleWaveChanged;
                spawner.OnWaveFinished -= HandleWaveFinished;
                spawner.OnAllWavesCompleted -= HandleAllWavesCompleted;
            }
        }

        private void HandleWaveFinished(int finishedWave, int totalWaves)
        {
            RefreshButtonState();
        }

        private void OnDestroy()
        {
            if (startWaveButton != null)
            {
                startWaveButton.onClick.RemoveListener(HandleButtonClicked);
            }
        }

        private void HandleButtonClicked()
        {
            if (spawner == null)
            {
                return;
            }

            if (spawner.StartNextWave())
            {
                RefreshButtonState();
            }
        }

        private void HandleWaveChanged(int currentWave, int totalWaves)
        {
            RefreshButtonState();
        }

        private void HandleAllWavesCompleted()
        {
            RefreshButtonState();
        }

        private void RefreshButtonState()
        {
            if (spawner == null || startWaveButton == null)
            {
                return;
            }

            bool canStart = !spawner.IsWaveInProgress && spawner.HasMoreWaves;
            startWaveButton.interactable = canStart;
        }
    }
}