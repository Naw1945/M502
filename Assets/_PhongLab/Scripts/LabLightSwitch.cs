using UnityEngine;

public class LabLightSwitch : LabInteractableBase
{
    [Header("Dàn đèn điều khiển (Spotlight/Point Light)")]
    public GameObject[] lights;

    [Header("Danh sách bóng đèn cần đổi vật liệu (Mesh Renderer)")]
    public MeshRenderer[] bulbRenderers;

    [Header("Vật liệu trạng thái")]
    public Material matPhatSang;
    public Material matTat;

    [Header("Trạng thái")]
    public bool isOn = true;

    [Header("Âm thanh công tắc")]
    public AudioSource switchAudio;

    private InteractableInfo info;

    protected override void Awake()
    {
        base.Awake();
        info = GetComponent<InteractableInfo>();

        // Tự động tìm AudioSource gắn cùng vật thể nếu chưa kéo vào Inspector
        if (switchAudio == null)
            switchAudio = GetComponent<AudioSource>();
    }

    void Start()
    {
        UpdateLightState();
    }

    public override void ExecuteInteraction()
    {
        isOn = !isOn;
        UpdateLightState();

        // Phát âm thanh tiếng bấm công tắc
        if (switchAudio != null)
        {
            switchAudio.Play();
        }

        Debug.Log(">>> Đã đổi trạng thái đèn: " + isOn);
    }

    private void UpdateLightState()
    {
        if (lights != null)
        {
            foreach (var lightObj in lights)
            {
                if (lightObj != null)
                    lightObj.SetActive(isOn);
            }
        }

        if (bulbRenderers != null && matPhatSang != null && matTat != null)
        {
            Material targetMat = isOn ? matPhatSang : matTat;
            foreach (var renderer in bulbRenderers)
            {
                if (renderer != null)
                    renderer.material = targetMat;
            }
        }

        if (info != null)
        {
            info.customDescription = isOn ? "Bấm Trigger để Tắt đèn" : "Bấm Trigger để Bật đèn";
        }
    }
}