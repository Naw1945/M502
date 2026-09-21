using UnityEngine;

public class LabFanSwitch : LabInteractableBase
{
    [Header("Danh sách quạt trần điều khiển")]
    public CeilingFanController[] fans;

    [Header("Trạng thái công tắc")]
    public bool isOn = false;

    [Header("Âm thanh bấm công tắc")]
    public AudioSource switchAudio;

    private InteractableInfo info;

    protected override void Awake()
    {
        base.Awake();
        info = GetComponent<InteractableInfo>();

        if (switchAudio == null)
            switchAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        UpdateSwitchState();
    }

    public override void ExecuteInteraction()
    {
        isOn = !isOn;
        UpdateSwitchState();

        if (switchAudio != null)
        {
            switchAudio.Play();
        }

        Debug.Log(">>> Trạng thái quạt trần: " + isOn);
    }

    private void UpdateSwitchState()
    {
        // 1. Gửi tín hiệu bật/tắt tới danh sách quạt
        if (fans != null)
        {
            foreach (var fan in fans)
            {
                if (fan != null)
                    fan.isRunning = isOn;
            }
        }

        // 2. Cập nhật dòng hướng dẫn trên Tooltip HUD
        if (info != null)
        {
            info.customDescription = isOn ? "Bấm Trigger để Tắt quạt" : "Bấm Trigger để Bật quạt";
        }
    }
}