using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using UnityEngine.UI;

[RequireComponent(typeof(SplineContainer))]
public class SplineCheckpointTracker : MonoBehaviour
{
    [Header("Отслеживаемый объект")]
    [Tooltip("Текущий активный объект игрока или машины")]
    [SerializeField] private Transform trackerTarget;

    [Header("Стрелка навигации")]
    [Tooltip("Объект полупрозрачной стрелки")]
    [SerializeField] private Transform arrowPointer;
    [Tooltip("Смещение стрелки относительно игрока/машины, высота над крышей/головой")]
    [SerializeField] private Vector3 arrowOffset = new Vector3(0f, 2.5f, 0f);
    [Tooltip("Скорость плавного доворота стрелки к цели")]
    [SerializeField] private float arrowRotationSpeed = 12f;

    [Header("Визуализация контрольной точки")]
    [Tooltip("3D-маркер над узлом")]
    [SerializeField] private GameObject targetWorldMarker;

    [Header("Интерфейс")]
    [SerializeField] private Text counterText;

    [Header("Параметры маршрута")]
    [Tooltip("Радиус регистрации точки в метрах")]
    [SerializeField] private float reachRadius = 6.0f;
    [SerializeField] private bool loopRoute = false;

    private SplineContainer splineContainer;
    private int currentKnotIndex = 0;
    private int totalKnots = 0;
    private bool isRouteCompleted = false;

    private void Awake()
    {
        splineContainer = GetComponent<SplineContainer>();
    }

    private void Start()
    {
        if (splineContainer != null && splineContainer.Spline != null)
        {
            totalKnots = splineContainer.Spline.Count;
        }

        if (totalKnots == 0)
        {
            Debug.LogWarning("Сплайн пуст! Добавьте узлы (Knots) в SplineContainer.");
            return;
        }

        currentKnotIndex = 0;
        UpdateActiveTargetVisuals();
        UpdateUI();
    }

    private void Update()
    {
        if (isRouteCompleted || trackerTarget == null || totalKnots == 0)
            return;

        Vector3 currentTargetPos = GetKnotWorldPosition(currentKnotIndex);

        // Проверка дистанции до контрольной точки
        float distanceToTarget = Vector3.Distance(trackerTarget.position, currentTargetPos);
        if (distanceToTarget <= reachRadius)
        {
            AdvanceToNextCheckpoint();
        }

        // Позиционирование и поворот навигационной стрелки
        UpdateArrow(currentTargetPos);
    }

    private void UpdateArrow(Vector3 targetWorldPos)
    {
        if (arrowPointer == null) return;

        // Позиционируем стрелку над персонажем/машиной
        arrowPointer.position = trackerTarget.position + arrowOffset;

        // Направление стрелки на чекпоинт по "y"
        Vector3 targetDirection = targetWorldPos - arrowPointer.position;
        targetDirection.y = 0f;

        if (targetDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
            arrowPointer.rotation = Quaternion.Slerp(arrowPointer.rotation, targetRotation, Time.deltaTime * arrowRotationSpeed);
        }
    }

    private void AdvanceToNextCheckpoint()
    {
        currentKnotIndex++;

        if (currentKnotIndex >= totalKnots)
        {
            if (loopRoute)
            {
                currentKnotIndex = 0;
                UpdateActiveTargetVisuals();
                UpdateUI();
            }
            else
            {
                CompleteRoute();
            }
        }
        else
        {
            UpdateActiveTargetVisuals();
            UpdateUI();
        }
    }

    private void UpdateActiveTargetVisuals()
    {
        if (targetWorldMarker != null && currentKnotIndex < totalKnots)
        {
            targetWorldMarker.SetActive(true);
            targetWorldMarker.transform.position = GetKnotWorldPosition(currentKnotIndex);
        }

        if (arrowPointer != null)
            arrowPointer.gameObject.SetActive(true);
    }

    private void UpdateUI()
    {
        if (counterText != null)
        {
            counterText.text = $"Чекпоинт: {currentKnotIndex} / {totalKnots}";
        }
    }

    private Vector3 GetKnotWorldPosition(int index)
    {
        BezierKnot knot = splineContainer.Spline[index];
        return splineContainer.transform.TransformPoint((Vector3)knot.Position);
    }

    private void CompleteRoute()
    {
        isRouteCompleted = true;

        if (counterText != null)
            counterText.text = $"Пройдено: {totalKnots} / {totalKnots}";

        if (targetWorldMarker != null)
            targetWorldMarker.SetActive(false);

        // Выключаем стрелку после завершения маршрута
        if (arrowPointer != null)
            arrowPointer.gameObject.SetActive(false);
    }

    private void OnDrawGizmos()
    {
        if (splineContainer == null || splineContainer.Spline == null) return;

        for (int i = 0; i < splineContainer.Spline.Count; i++)
        {
            Vector3 knotPos = splineContainer.transform.TransformPoint((Vector3)splineContainer.Spline[i].Position);
            Gizmos.color = (i == currentKnotIndex && Application.isPlaying) ? Color.green : Color.yellow;
            Gizmos.DrawWireSphere(knotPos, reachRadius);
        }
    }

    public void SetTrackerTarget(Transform newTarget)
    {
    trackerTarget = newTarget;
    }
}