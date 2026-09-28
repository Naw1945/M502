using UnityEngine;

public class CeilingFanController : MonoBehaviour
{
    public enum RotationAxis { Y_Axis, X_Axis, Z_Axis }

    [Header("Bộ phận quay")]
    [Tooltip("Kéo đối tượng cánh/quạt con vào đây")]
    public Transform fanBlades;

    [Header("Trục xoay")]
    public RotationAxis rotationAxis = RotationAxis.Y_Axis;

    [Header("5 Cấp độ tốc độ (Mặc định đã tăng 50%)")]
    [Tooltip("Bạn có thể tùy ý sửa số của từng nấc trực tiếp ngay trên Inspector")]
    public float[] speedLevels = new float[] { 0f, 225f, 450f, 720f, 1050f };

    [Header("Hệ số nhân tốc độ tổng thể")]
    [Tooltip("Hệ số nhân giúp tăng/giảm nhanh: 1 = mặc định, 1.5 = tăng thêm 50%, 2 = gấp đôi")]
    [Range(0.1f, 3f)]
    public float speedMultiplier = 1f;

    [Header("Âm thanh theo cấp độ (Loop)")]
    public AudioSource audioSource;
    [Tooltip("Độ to âm thanh theo 5 cấp (0 = 0 volume)")]
    public float[] volumeLevels = new float[] { 0f, 0.25f, 0.45f, 0.7f, 1.0f };
    [Tooltip("Độ rít (Pitch) theo 5 cấp: càng cao tiếng rít càng nhanh")]
    public float[] pitchLevels = new float[] { 0.5f, 0.8f, 1.0f, 1.25f, 1.5f };

    [Header("Gia tốc")]
    public float acceleration = 180f;  // Tăng gia tốc để bắt tốc độ nhanh hơn
    public float deceleration = 100f;  // Gia tốc quán tính khi giảm số/tắt

    [Header("Trạng thái hiện tại")]
    [Range(0, 4)]
    public int currentLevel = 0;

    private float currentSpeed = 0f;

    void Awake()
    {
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            audioSource.volume = 0f;
        }
    }

    void Update()
    {
        int levelIndex = Mathf.Clamp(currentLevel, 0, speedLevels.Length - 1);
        
        // Tốc độ đích tính theo cấp và nhân với hệ số Speed Multiplier
        float targetSpeed = speedLevels[levelIndex] * speedMultiplier;

        // 1. Tăng hoặc giảm tốc độ mượt mà
        if (currentSpeed < targetSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else if (currentSpeed > targetSpeed)
        {
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, deceleration * Time.deltaTime);
        }

        // 2. Xoay cánh quạt
        if (fanBlades != null && currentSpeed > 0f)
        {
            Vector3 axisVector = Vector3.up;
            switch (rotationAxis)
            {
                case RotationAxis.Y_Axis: axisVector = Vector3.up; break;
                case RotationAxis.X_Axis: axisVector = Vector3.right; break;
                case RotationAxis.Z_Axis: axisVector = Vector3.forward; break;
            }
            fanBlades.Rotate(axisVector, currentSpeed * Time.deltaTime, Space.Self);
        }

        // 3. Xử lý âm thanh mượt mà theo vận tốc thực tế
        UpdateFanAudio(levelIndex);
    }

    private void UpdateFanAudio(int targetLevel)
    {
        if (audioSource == null || audioSource.clip == null) return;

        // Nếu quạt đang có vận tốc quay
        if (currentSpeed > 1f)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();

            // Lerp volume và pitch từ từ theo nấc để âm thanh tăng giảm êm tai
            float targetVol = volumeLevels[targetLevel];
            float targetPitch = pitchLevels[targetLevel];

            audioSource.volume = Mathf.MoveTowards(audioSource.volume, targetVol, Time.deltaTime * 0.8f);
            audioSource.pitch = Mathf.MoveTowards(audioSource.pitch, targetPitch, Time.deltaTime * 0.8f);
        }
        else
        {
            // Quạt đã dừng hẳn
            audioSource.volume = Mathf.MoveTowards(audioSource.volume, 0f, Time.deltaTime * 1.5f);
            if (audioSource.volume <= 0.01f && audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }

    public void SetLevel(int level)
    {
        currentLevel = Mathf.Clamp(level, 0, speedLevels.Length - 1);
    }
}