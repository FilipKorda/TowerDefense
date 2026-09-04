using System;
using System.Collections;
using System.Collections.Generic;
using TowerDefense.GameSystem;
using TowerDefense.PlayerSystem;
using UnityEngine;

namespace TowerDefense.EnemySystem
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField] private EnemyPath path;
        [SerializeField] private List<WaveDefinition> waves = new List<WaveDefinition>();
        [SerializeField, Min(0)] private int waveCompletionReward = 20;

        private readonly HashSet<EnemyRuntime> aliveEnemiesInWave = new HashSet<EnemyRuntime>();

        private Coroutine spawnRoutine;
        private int currentWaveIndex;
        private bool waveSpawningComplete;

        public int WaveCount => waves.Count;
        public bool IsWaveInProgress => spawnRoutine != null || aliveEnemiesInWave.Count > 0;
        public bool HasMoreWaves => currentWaveIndex < waves.Count;

        public event Action<int, int> OnWaveChanged;
        public event Action<int, int> OnWaveFinished;
        public event Action OnAllWavesCompleted;

        private void Awake()
        {
            if (GameSession.SelectedDifficulty != null && GameSession.SelectedDifficulty.Waves.Count > 0)
            {
                waves = new List<WaveDefinition>(GameSession.SelectedDifficulty.Waves);
            }
        }

        private void OnEnable()
        {
            OnWaveChanged?.Invoke(0, waves.Count);

            EnemyRuntime.AnyEnemyDestroyed += HandleEnemyDestroyed;
        }

        private void OnDisable()
        {
            EnemyRuntime.AnyEnemyDestroyed -= HandleEnemyDestroyed;
        }

        public bool StartNextWave()
        {
            if (IsWaveInProgress || !HasMoreWaves)
            {
                return false;
            }

            spawnRoutine = StartCoroutine(SpawnWaveRoutine(waves[currentWaveIndex]));
            return true;
        }

        private IEnumerator SpawnWaveRoutine(WaveDefinition wave)
        {
            OnWaveChanged?.Invoke(currentWaveIndex + 1, waves.Count);
            waveSpawningComplete = false;

            if (wave != null)
            {
                foreach (BurstDefinition burst in wave.Bursts)
                {
                    SpawnBurst(burst);
                    yield return new WaitForSeconds(wave.TimeBetweenBursts);
                }
            }

            waveSpawningComplete = true;
            spawnRoutine = null;

            TryFinishWave();
        }

        private void HandleEnemyDestroyed(EnemyRuntime enemy)
        {
            if (aliveEnemiesInWave.Remove(enemy))
            {
                TryFinishWave();
            }
        }

        private void TryFinishWave()
        {
            if (!waveSpawningComplete || aliveEnemiesInWave.Count > 0)
            {
                return;
            }

            int finishedWaveNumber = currentWaveIndex + 1;
            currentWaveIndex++;

            if (PlayerStats.Instance != null)
            {
                PlayerStats.Instance.AddMoney(waveCompletionReward);
            }

            OnWaveFinished?.Invoke(finishedWaveNumber, waves.Count);

            if (!HasMoreWaves)
            {
                OnAllWavesCompleted?.Invoke();
            }
        }

        private void SpawnBurst(BurstDefinition burst)
        {
            if (path == null || burst == null || burst.EnemyDefinition == null || burst.EnemyDefinition.Prefab == null)
            {
                return;
            }

            Vector3 rightAxis = GetFormationRightAxis(out Vector3 forwardAxis);

            for (int i = 0; i < burst.EnemyCount; i++)
            {
                Vector3 offset = GetDiceFormationOffset(i, burst.EnemyCount, burst.FormationSpacing, rightAxis, forwardAxis);
                SpawnEnemyAt(burst.EnemyDefinition, offset);
            }
        }

        private Vector3 GetFormationRightAxis(out Vector3 forwardAxis)
        {
            if (path.PointCount < 2)
            {
                forwardAxis = Vector3.forward;
                return Vector3.right;
            }

            Vector3 forward = path.GetPoint(1) - path.GetPoint(0);
            forward.y = 0f;

            if (forward.sqrMagnitude <= 0.001f)
            {
                forwardAxis = Vector3.forward;
                return Vector3.right;
            }

            forwardAxis = forward.normalized;
            return Vector3.Cross(Vector3.up, forwardAxis);
        }

        private static Vector3 GetDiceFormationOffset(int index, int count, float spacing, Vector3 rightAxis, Vector3 forwardAxis)
        {
            Vector2Int[] gridPositions = GetDicePattern(count);
            Vector2Int gridPosition = gridPositions[Mathf.Min(index, gridPositions.Length - 1)];

            return rightAxis * (gridPosition.x * spacing) + forwardAxis * (gridPosition.y * spacing);
        }

        private static Vector2Int[] GetDicePattern(int count)
        {
            switch (count)
            {
                case 1:
                    return new[] { new Vector2Int(0, 0) };
                case 2:
                    return new[] { new Vector2Int(-1, -1), new Vector2Int(1, 1) };
                case 3:
                    return new[] { new Vector2Int(-1, -1), new Vector2Int(0, 0), new Vector2Int(1, 1) };
                case 4:
                    return new[] { new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(-1, 1), new Vector2Int(1, 1) };
                case 5:
                    return new[] { new Vector2Int(-1, -1), new Vector2Int(1, -1), new Vector2Int(0, 0), new Vector2Int(-1, 1), new Vector2Int(1, 1) };
                case 6:
                    return new[] { new Vector2Int(-1, -1), new Vector2Int(-1, 0), new Vector2Int(-1, 1), new Vector2Int(1, -1), new Vector2Int(1, 0), new Vector2Int(1, 1) };
                default:
                    return new[] { new Vector2Int(0, 0) };
            }
        }

        private void SpawnEnemyAt(EnemyDefinition enemyDefinition, Vector3 formationOffset)
        {
            Vector3 spawnPosition = path.GetPoint(0) + formationOffset;

            GameObject enemyObject = Instantiate(
                enemyDefinition.Prefab,
                spawnPosition,
                Quaternion.identity);

            EnemyRuntime runtime = enemyObject.GetComponent<EnemyRuntime>();
            if (runtime == null)
            {
                runtime = enemyObject.AddComponent<EnemyRuntime>();
            }

            runtime.Initialize(enemyDefinition);
            CodexProgressStore.UnlockEnemy(enemyDefinition);
            EnsureEnemyCanBeDetected(enemyObject);
            aliveEnemiesInWave.Add(runtime);

            EnsureEnemyCanBeDetected(enemyObject);

            EnemyPathFollower follower = enemyObject.GetComponent<EnemyPathFollower>();
            if (follower == null)
            {
                follower = enemyObject.AddComponent<EnemyPathFollower>();
            }

            follower.Initialize(path, formationOffset);
        }

        private static void EnsureEnemyCanBeDetected(GameObject enemyObject)
        {
            if (enemyObject.GetComponentInChildren<Collider>() != null)
            {
                return;
            }

            CapsuleCollider collider = enemyObject.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0f, 1f, 0f);
            collider.height = 2f;
            collider.radius = 0.35f;
        }
    }
}