using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Tương thích XRI 3.x

public class SlidingBoardPanel : MonoBehaviour
{
    [Header("Mốc tọa độ trượt trên trục X cục bộ")]
    [Tooltip("Tọa độ X khi MỞ (màn hình lộ ra)")]
    public float openX = 0f;

    [Tooltip("Tọa độ X khi ĐÓNG (hai mép khép vào giữa)")]
    public float closedX = 0.5f;

    [Header("Tốc độ trượt")]
    [Tooltip("Tốc độ trượt mỗi giây")]
    public float slideSpeed = 0.5f;

    [Header("Âm thanh ray trượt")]
    public AudioSource audioSource;
    public AudioClip slideSoundLoop;

    [Header("Trạng thái hiện tại")]
    [Tooltip("Mục tiêu trượt: true = đang hướng về Mở, false = đang hướng về Đóng")]
    public bool targetIsOpen = false; // Mặc định lần đầu bóp sẽ kéo về đóng

    private XRSimpleInteractable interactable;
    private bool isHolding = false;
    private float fixedY;
    private float fixedZ;
    private float minX;
    private float maxX;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.loop = true;
            audioSource.playOnAwake = false;
            if (slideSoundLoop != null)
                audioSource.clip = slideSoundLoop;
        }

        // Cố định trục Y và Z để bảng không bị xô lệch khỏi ray
        fixedY = transform.localPosition.y;
        fixedZ = transform.localPosition.z;

        // Tính giới hạn biên nhỏ nhất và lớn nhất
        minX = Mathf.Min(openX, closedX);
        maxX = Mathf.Max(openX, closedX);
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGripPress);
        interactable.selectExited.AddListener(OnGripRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGripPress);
        interactable.selectExited.RemoveListener(OnGripRelease);
        isHolding = false;
        StopSlideSound();
    }

    private void OnGripPress(SelectEnterEventArgs args)
    {
        isHolding = true;
    }

    private void OnGripRelease(SelectExitEventArgs args)
    {
        isHolding = false;
        StopSlideSound();

        // Tự động đảo chiều cho lần bóp tiếp theo
        targetIsOpen = !targetIsOpen;
    }

    void Update()
    {
        if (!isHolding) return;

        // Xác định vị trí đích cần tới theo chiều hiện tại
        float targetX = targetIsOpen ? openX : closedX;
        float currentX = transform.localPosition.x;

        // Kiểm tra xem đã chạm kịch biên chưa
        if (Mathf.Abs(currentX - targetX) > 0.001f)
        {
            // Trượt từ từ về đích
            float newX = Mathf.MoveTowards(currentX, targetX, slideSpeed * Time.deltaTime);

            // Khống chế tuyệt đối không cho vượt ra ngoài biên
            newX = Mathf.Clamp(newX, minX, maxX);

            transform.localPosition = new Vector3(newX, fixedY, fixedZ);

            PlaySlideSound();
        }
        else
        {
            // Đã chạm kịch biên thì dừng âm thanh
            StopSlideSound();
        }
    }

    private void PlaySlideSound()
    {
        if (audioSource != null && audioSource.clip != null && !audioSource.isPlaying)
        {
            audioSource.Play();
        }
    }

    private void StopSlideSound()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }
}