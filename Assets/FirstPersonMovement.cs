using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Настройки")]
    public float moveSpeed = 5f;
    public float mouseSensitivity = 2f;

    private Rigidbody rb;
    private Transform cameraTransform;
    private float verticalRotation = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        
        // Отключаем влияние вращения физики по силам, чтобы персонаж не падал сам по себе
        rb.freezeRotation = true;

        Camera cam = GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cameraTransform = cam.transform;
        }

        Cursor.lockState = CursorLockMode.Locked;

        MeshRenderer localMesh = GetComponent<MeshRenderer>();
        if (localMesh != null)
        {
            localMesh.enabled = false;
        }
    }

    void Update()
    {
        if (cameraTransform == null) return;

        // 1. Поворот камеры мышью
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        transform.Rotate(Vector3.up * mouseX);

        verticalRotation -= mouseY;
        verticalRotation = Mathf.Clamp(verticalRotation, -80f, 80f);
        cameraTransform.localRotation = Quaternion.Euler(verticalRotation, 0f, 0f);
    }

    // Для плавного и четкого движения физики в Unity лучше использовать FixedUpdate
    void FixedUpdate()
    {
        // Считываем WASD
        float moveX = Input.GetAxisRaw("Horizontal"); // Используем GetAxisRaw вместо обычного GetAxis — он дает мгновенный переход от 0 к 1 без плавного разгона
        float moveZ = Input.GetAxisRaw("Vertical");   

        Vector3 moveDirection = (transform.right * moveX + transform.forward * moveZ).normalized;

        // Мгновенно задаем скорость (без инерции и скольжения)
        Vector3 targetVelocity = moveDirection * moveSpeed;
        rb.velocity = new Vector3(targetVelocity.x, rb.velocity.y, targetVelocity.z);
    }
}