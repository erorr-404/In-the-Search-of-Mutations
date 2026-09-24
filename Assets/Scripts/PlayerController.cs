using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] SpermStatsData playerStats;
    [SerializeField] float angleOffset = 90f;

    Camera mainCamera;
    SpermStats spermStats;

    Quaternion targetRotation;
    float currentSpeed;


    void Awake()
    {
        mainCamera = Camera.main;
        targetRotation = transform.rotation;

        // default stats without any micro-implants
        playerStats = new SpermStatsData
        {
            MaxSpeed = 5f,
            Acceleration = 8f,
            Deceleration = 10f,
            TurnSpeed = 1f
        };
    }

    void Start()
    {
        spermStats = GetComponent<SpermStats>();
        playerStats = spermStats.GetPlayerStats();
    }

    void Update()
    {
        bool leftMouseButtonPressed = Input.GetMouseButton(0);
        // bool rightMouseButtonPress = Input.GetMouseButtonDown(1); // TODO: add damageable punch

        if (leftMouseButtonPressed)
        {
            Vector3 mouseScreenPos = Input.mousePosition;
            mouseScreenPos.z = -mainCamera.transform.position.z;
            Vector3 mouseWorldPos = mainCamera.ScreenToWorldPoint(mouseScreenPos);
            Vector2 difference = (Vector2)(mouseWorldPos - transform.position);

            if (difference.sqrMagnitude < 0.01f) return;
            float targetAngle = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg + angleOffset;
            targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, playerStats.TurnSpeed * Time.deltaTime);

        float targetSpeed = leftMouseButtonPressed ? playerStats.MaxSpeed : 0f;
        float rate = leftMouseButtonPressed ? playerStats.Acceleration : playerStats.Deceleration;
        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, rate * Time.deltaTime);

        if (currentSpeed > 0.001f)
        {
            transform.position += transform.up * (currentSpeed * Time.deltaTime);
        }
    }

    public void UpdatePlayerStats(SpermStatsData newStats)
    {
        playerStats = newStats;
    }
}
