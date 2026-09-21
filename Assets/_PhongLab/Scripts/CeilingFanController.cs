using UnityEngine;

public class CeilingFanController : MonoBehaviour
{
    public enum RotationAxis { Y_Axis, X_Axis, Z_Axis }

    [Header("Bộ phận quay")]
    [Tooltip("Kéo đối tượng cánh/quạt con vào đây")]
    public Transform fanBlades;

    [Header("Trục xoay")]
    [Tooltip("Mặc định trục thẳng đứng là Y_Axis. Nếu bị lộn nhào thì chọn sang X hoặc Z")]
    public RotationAxis rotationAxis = RotationAxis.Y_Axis;

    [Header("Thông số tốc độ")]
    public float maxSpeed = 360f;      // Tốc độ tối đa (độ/giây)
    public float acceleration = 120f;  // Gia tốc tăng tốc khi bật
    public float deceleration = 80f;   // Gia tốc giảm dần khi tắt

    [Header("Trạng thái")]
    public bool isRunning = false;

    private float currentSpeed = 0f;

    void Update()
    {
        // Tăng hoặc giảm tốc mượt mà theo thời gian thực
        if (isRunning)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // Thực hiện xoay nếu quạt đang có vận tốc
        if (fanBlades != null && currentSpeed > 0f)
        {
            Vector3 axisVector = Vector3.up;

            switch (rotationAxis)
            {
                case RotationAxis.Y_Axis:
                    axisVector = Vector3.up;
                    break;
                case RotationAxis.X_Axis:
                    axisVector = Vector3.right;
                    break;
                case RotationAxis.Z_Axis:
                    axisVector = Vector3.forward;
                    break;
            }

            fanBlades.Rotate(axisVector, currentSpeed * Time.deltaTime, Space.Self);
        }
    }

    // Hàm gọi khi công tắc kích hoạt
    public void ToggleFan()
    {
        isRunning = !isRunning;
    }
}