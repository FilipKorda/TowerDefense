using TowerDefense.EnemySystem;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
[RequireComponent(typeof(EnemyRuntime))]
public class EnemyHealthBar : MonoBehaviour
{
    [SerializeField] private GameObject canvasHp;
    [SerializeField] private Slider healthSlider;
    [SerializeField] private bool hideWhenFullHp = false;

    private EnemyRuntime enemyRuntime;
    private Camera mainCamera;

    private void Awake()
    {
        enemyRuntime = GetComponent<EnemyRuntime>();
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        enemyRuntime.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        enemyRuntime.OnHealthChanged -= HandleHealthChanged;
    }

    private void LateUpdate()
    {
        FaceCamera();
    }

    private void HandleHealthChanged(float currentHp, float maxHp)
    {
        healthSlider.maxValue = maxHp;
        healthSlider.value = currentHp;

        if (hideWhenFullHp)
        {
            healthSlider.gameObject.SetActive(currentHp < maxHp);
        }
    }

    private void FaceCamera()
    {
        Vector3 direction = canvasHp.transform.position - mainCamera.transform.position;

        if (direction.sqrMagnitude <= 0.001f)
        {
            return;
        }

        canvasHp.transform.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
    }
}