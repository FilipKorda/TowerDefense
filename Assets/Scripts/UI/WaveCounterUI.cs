using TMPro;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    [DisallowMultipleComponent]
    public class WaveCounterUI : MonoBehaviour
    {
        [SerializeField] private EnemySpawner spawner;
        [SerializeField] private TextMeshProUGUI waveText;
        [SerializeField] private string waveFormat = "Wave: {0} / {1}";

        private void OnEnable()
        {
            if (spawner != null)
            {
                spawner.OnWaveChanged += HandleWaveChanged;
            }
        }

        private void OnDisable()
        {
            if (spawner != null)
            {
                spawner.OnWaveChanged -= HandleWaveChanged;
            }
        }

        private void HandleWaveChanged(int currentWave, int totalWaves)
        {
            if (waveText != null)
            {
                waveText.text = string.Format(waveFormat, currentWave, totalWaves);
            }
        }
    }
}