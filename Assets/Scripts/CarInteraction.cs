using UnityEngine;

public class CarInteraction : MonoBehaviour
{
    [Header("Ссылки на Персонажа")]
    [SerializeField] private GameObject playerObject;
    [SerializeField] private CharacterController playerCharacterController;
    [SerializeField] private GameObject playerCamera; // Виртуальная камера Cinemachine или Camera

    [Header("Ссылки на Автомобиль")]
    [SerializeField] private PrometeoCarController carController;
    [SerializeField] private GameObject carCamera;
    [SerializeField] private Transform exitPoint;

    [Header("Параметры")]
    [SerializeField] private KeyCode interactionKey = KeyCode.F;
    [SerializeField] private float enterDistance = 3.5f;

    [Header("Навигация")]
    [SerializeField] private SplineCheckpointTracker routeTracker;

    private Rigidbody carRigidbody;
    private bool isInsideCar = false;

    private void Awake()
    {
        carRigidbody = GetComponent<Rigidbody>();
        if (carController == null)
        {
            carController = GetComponent<PrometeoCarController>();
        }
    }

    private void Start()
    {
        // На старте машина заглушена, ее камера выключена
        if (carController != null) carController.enabled = false;
        if (carCamera != null) carCamera.SetActive(false);
    }

    private void Update()
    {
        if (isInsideCar)
        {
            if (Input.GetKeyDown(interactionKey))
            {
                ExitCar();
            }
        }
        else
        {
            if (playerObject != null)
            {
                float dist = Vector3.Distance(playerObject.transform.position, transform.position);
                if (dist <= enterDistance && Input.GetKeyDown(interactionKey))
                {
                    EnterCar();
                }
            }
        }
    }

    private void EnterCar()
    {
        isInsideCar = true;

        // Отключаем персонажа
        if (playerCharacterController != null) playerCharacterController.enabled = false;
        if (playerCamera != null) playerCamera.SetActive(false);
        playerObject.SetActive(false);

        // Включаем автомобиль
        if (carController != null) carController.enabled = true;
        if (carCamera != null) carCamera.SetActive(true);

        if (routeTracker != null)
        {
            routeTracker.SetTrackerTarget(transform);
        }
    }

    private void ExitCar()
    {
        isInsideCar = false;

        // Останавливаем машину и выключаем контроллер
        if (carController != null) carController.enabled = false;
        if (carCamera != null) carCamera.SetActive(false);

        if (carRigidbody != null)
        {
            carRigidbody.linearVelocity = Vector3.zero;
            carRigidbody.angularVelocity = Vector3.zero;
        }

        // Перемещаем персонажа к точке выхода
        if (exitPoint != null)
        {
            playerObject.transform.position = exitPoint.position;
            playerObject.transform.rotation = exitPoint.rotation;
        }

        playerObject.SetActive(true);
        if (playerCharacterController != null) playerCharacterController.enabled = true;
        if (playerCamera != null) playerCamera.SetActive(true);

        if (routeTracker != null && playerObject != null)
        {
            routeTracker.SetTrackerTarget(playerObject.transform);
        }
    }
}