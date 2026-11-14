# Movement of Ball

一个使用 Unity 制作的 3D 小球直线往返运动演示项目。

小球沿一条固定的 3D 直线在 A、B 两点之间往返移动，到达端点后等待，再反向返回；运动速度呈现 **“两端快、中间慢”** 的变化。

## 演示视频

[点击观看 BallDemo.mp4](BallDemo.mp4)

## 项目信息

| 项目 | 内容 |
| --- | --- |
| 引擎版本 | Unity 6000.6.0f1 |
| 渲染管线 | Universal Render Pipeline (URP) |
| 主场景 | `Assets/Scenes/SampleScene.unity` |
| 运动脚本 | `Assets/MovementBall_2.cs` |

## 运行效果

- A 点：`(-6, 0.55, 0)`
- B 点：`(6, 0.55, 0)`
- 端点最大速度：`4`
- 中间速度比例：`0.2`
- 端点等待时间：`0.4 秒`

运行后小球会：

1. 在 A 点等待；
2. 加速出发；
3. 经过中间时明显减速；
4. 接近 B 点时再次加速；
5. 在 B 点等待后反向返回。

## 场景组成

`Assets/Scenes/SampleScene.unity` 包含：

- `Floor`：地面
- `Track`：直线轨道
- `Sphere`：运动小球
- `Main Camera`：主相机
- `Directional Light`：方向光
- `Global Volume`：URP 后处理

## 核心代码

当前场景启用的是 `Assets/MovementBall_2.cs`：

```csharp
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
```

## 速度曲线

代码通过归一化位置 `t` 计算速度系数：

```csharp
float factor = Mathf.Lerp(middleFactor, 1f, Mathf.Abs(2f * t - 1f));
```

其中：

- `t = 0`：A 点，`factor = 1`
- `t = 0.5`：中间，`factor = 0.2`
- `t = 1`：B 点，`factor = 1`

因此小球在每个单程中呈现：

```text
快 → 慢 → 快
```

## 如何运行

1. 使用 Unity 6000.6.0f1 打开项目。
2. 打开 `Assets/Scenes/SampleScene.unity`。
3. 点击 Play。

参数可以在运行前或运行时通过 Inspector 调整：

- `Speed`：端点最大速度
- `Middle Factor`：中间速度相对端点的比例
- `Wait Time`：端点等待时间

## 代码结构

- `Assets/MovementBall_2.cs`：当前场景使用的简单实现，基于 `Update` 和 `Mathf.Lerp`。
- `Assets/MovementBll.cs`：更完整的协程实现，支持暂停、重置和当前速度状态，可作为进阶参考。

## 仓库内容

```text
Movement of Ball/
├─ Assets/
│  ├─ Scenes/SampleScene.unity
│  ├─ MovementBall_2.cs
│  └─ MovementBll.cs
├─ Packages/
├─ ProjectSettings/
├─ BallDemo.mp4
└─ README.md
```

## License

本项目仅用于学习与课程作业演示。
