using UnityEngine;

public class MovementBall_2 : MonoBehaviour
{
    public Vector3 pointA = new Vector3(-6f, 0.55f, 0f);
    public Vector3 pointB = new Vector3(6f, 0.55f, 0f);

    [Header("动态参数")]
    [Min(0.01f)] public float speed = 4f;          // 端点最大速度
    [Range(0.05f, 1f)] public float middleFactor = 0.2f;
    [Min(0f)] public float waitTime = 0.4f;

    private float t;              // 0 = A, 1 = B
    private int direction = 1;
    private bool waiting;
    private float waitElapsed;

    private void Start()
    {
        transform.position = pointA;
        waiting = waitTime > 0f;
    }

    private void Update()
    {
        if (waiting)
        {
            waitElapsed += Time.deltaTime;
            if (waitElapsed < waitTime) return;

            waiting = false;
            waitElapsed = 0f;
        }

        float length = Vector3.Distance(pointA, pointB);
        if (length < 0.001f) return;

        // 两端 factor = 1；中间 factor = middleFactor
        float factor = Mathf.Lerp(middleFactor, 1f, Mathf.Abs(2f * t - 1f));
        t += direction * speed * factor / length * Time.deltaTime;

        if (t >= 1f || t <= 0f)
        {
            t = Mathf.Clamp01(t);
            direction = -direction;
            waiting = true;
            waitElapsed = 0f;
        }

        transform.position = Vector3.Lerp(pointA, pointB, t);
    }
}