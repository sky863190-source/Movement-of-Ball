using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class MovementBll : MonoBehaviour
{
    [Header("直线端点")]
    public Vector3 pointA = new Vector3(-6f, 0.55f, 0f);
    public Vector3 pointB = new Vector3(6f, 0.55f, 0f);

    [Header("动态参数")]
    [Tooltip("端点处的最大移动速度，单位/秒")]
    [Min(0.01f)] public float moveSpeed = 4f;

    [Tooltip("中间速度 / 端点速度")]
    [Range(0.05f, 0.95f)] public float middleSpeedRatio = 0.2f;

    [Tooltip("到达端点后的等待时间，秒")]
    [Min(0f)] public float waitTime = 0.4f;

    [Tooltip("开始运行时是否先在 A 点等待")]
    public bool waitAtStart = true;

    [Header("运行时状态（只读）")]
    [SerializeField] private bool isPaused;

    public float CurrentSpeed { get; private set; }
    public bool IsWaiting { get; private set; }
    public bool IsPaused => isPaused;

    private int direction = 1; // 1: A -> B, -1: B -> A

    private void Start()
    {
        transform.position = pointA;
        StartCoroutine(RunLoop());
    }

    private IEnumerator RunLoop()
    {
        if (waitAtStart)
        {
            yield return WaitAtEndpoint();
        }

        while (true)
        {
            Vector3 target = direction > 0 ? pointB : pointA;
            yield return MoveTo(target);
            yield return WaitAtEndpoint();
            direction = -direction;
        }
    }

    private IEnumerator MoveTo(Vector3 target)
    {
        Vector3 start = transform.position;
        float totalDistance = Vector3.Distance(start, target);

        if (totalDistance < 0.001f)
        {
            yield break;
        }

        float travelled = 0f;

        while (travelled < totalDistance)
        {
            if (!isPaused)
            {
                // t = 0：端点，t = 0.5：中间，t = 1：另一端点
                float t = Mathf.Clamp01(travelled / totalDistance);

                // 两端为 1，中间为 0
                float sideFactor = Mathf.Abs(2f * t - 1f);

                // 端点 factor = 1，中间 factor = middleSpeedRatio
                float factor = Mathf.Lerp(middleSpeedRatio, 1f, sideFactor);

                CurrentSpeed = moveSpeed * factor;
                travelled += CurrentSpeed * Time.deltaTime;

                float lerp = Mathf.Clamp01(travelled / totalDistance);
                transform.position = Vector3.Lerp(start, target, lerp);
            }
            else
            {
                CurrentSpeed = 0f;
            }

            yield return null;
        }

        transform.position = target;
    }

    private IEnumerator WaitAtEndpoint()
    {
        IsWaiting = true;
        CurrentSpeed = 0f;

        float elapsed = 0f;

        // 每帧重新读取 waitTime，运行时拖动滑条会立即影响等待时间
        while (elapsed < waitTime)
        {
            if (!isPaused)
            {
                elapsed += Time.deltaTime;
            }

            yield return null;
        }

        IsWaiting = false;
    }

    public void TogglePaused()
    {
        isPaused = !isPaused;
    }

    public void Restart()
    {
        StopAllCoroutines();

        transform.position = pointA;
        direction = 1;
        isPaused = false;
        IsWaiting = false;
        CurrentSpeed = 0f;

        StartCoroutine(RunLoop());
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pointA, pointB);
        Gizmos.DrawWireSphere(pointA, 0.2f);
        Gizmos.DrawWireSphere(pointB, 0.2f);
    }
}