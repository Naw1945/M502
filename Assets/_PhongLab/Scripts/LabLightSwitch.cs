using System.Collections;
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

    [Header("Cấu hình xoay nút công tắc")]
    [Tooltip("Góc trục X khi Bật đèn")]
    public float onAngleX = -80f;
    [Tooltip("Góc trục X khi Tắt đèn")]
    public float offAngleX = -100f;
    [Tooltip("Tốc độ bập bênh của nút")]
    public float toggleSpeed = 12f;

    [Header("Trạng thái")]
    public bool isOn = true;

    [Header("Âm thanh công tắc")]
    public AudioSource switchAudio;

    private InteractableInfo info;
    private Vector3 initialEuler;
    private Quaternion targetRotation;
    private Coroutine rotateCoroutine;

    protected override void Awake()
    {
        base.Awake();
        info = GetComponent<InteractableInfo>();

        // Tự động tìm AudioSource gắn cùng vật thể nếu chưa kéo vào Inspector
        if (switchAudio == null)
            switchAudio = GetComponent<AudioSource>();

        // Lưu góc ban đầu của trục Y và Z để chỉ can thiệp góc xoay trục X
        initialEuler = transform.localEulerAngles;
    }

    void Start()
    {
        // Đặt góc xoay và trạng thái tức thì lúc bắt đầu
        ApplyRotationInstant(isOn);
        UpdateLightState();
    }

    public override void ExecuteInteraction()
    {
        isOn = !isOn;
        UpdateLightState();
        AnimateSwitch(isOn);

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

    private void AnimateSwitch(bool state)
    {
        float targetX = state ? onAngleX : offAngleX;
        targetRotation = Quaternion.Euler(targetX, initialEuler.y, initialEuler.z);

        if (rotateCoroutine != null)
            StopCoroutine(rotateCoroutine);

        rotateCoroutine = StartCoroutine(RotateSmoothRoutine());
    }

    private IEnumerator RotateSmoothRoutine()
    {
        while (Quaternion.Angle(transform.localRotation, targetRotation) > 0.05f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, Time.deltaTime * toggleSpeed);
            yield return null;
        }

        transform.localRotation = targetRotation;
        rotateCoroutine = null;
    }

    private void ApplyRotationInstant(bool state)
    {
        float targetX = state ? onAngleX : offAngleX;
        transform.localRotation = Quaternion.Euler(targetX, initialEuler.y, initialEuler.z);
    }

    void OnDisable()
    {
        if (rotateCoroutine != null)
        {
            StopCoroutine(rotateCoroutine);
            rotateCoroutine = null;
        }
    }
}