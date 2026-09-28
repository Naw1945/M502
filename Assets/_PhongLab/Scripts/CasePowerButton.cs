using UnityEngine;

public class LabComputerCase : LabInteractableBase
{
    [Header("Màn hình máy tính điều khiển")]
    [SerializeField] private ScreenDisplay targetScreen;

    [Header("Âm thanh nút nguồn (Tùy chọn)")]
    [SerializeField] private AudioSource powerAudio;

    private InteractableInfo info;

    protected override void Awake()
    {
        base.Awake();
        info = GetComponent<InteractableInfo>();

        if (powerAudio == null)
            powerAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        UpdateInteractionInfo();
    }

    // Hàm này được LabInteractableBase tự động gọi khi người chơi bấm tương tác
    public override void ExecuteInteraction()
    {
        if (targetScreen != null)
        {
            targetScreen.TogglePower();

            if (powerAudio != null)
            {
                powerAudio.Play();
            }

            UpdateInteractionInfo();
            Debug.Log($">>> [LabComputerCase] Đã tương tác nút nguồn: {gameObject.name}");
        }
        else
        {
            Debug.LogError($">>> [LabComputerCase] Chưa kéo Target Screen trên: {gameObject.name}!");
        }
    }

    private void UpdateInteractionInfo()
    {
        if (info != null && targetScreen != null)
        {
            info.customDescription = targetScreen.IsOn ? "Bấm Trigger để Tắt máy" : "Bấm Trigger để Bật máy";
        }
    }
}