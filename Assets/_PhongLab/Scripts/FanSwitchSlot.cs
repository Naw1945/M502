using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables; // Hỗ trợ XRI 3.x

public class FanSwitchSlot : MonoBehaviour
{
    [Header("Quạt điều khiển")]
    [Tooltip("Kéo đối tượng quạt chứa CeilingFanController vào đây")]
    public CeilingFanController targetFan;

    [Header("Cấu hình xoay núm")]
    [Tooltip("Trục xoay của mặt núm công tắc (thường là Z_Axis hoặc Vector3.forward)")]
    public Vector3 knobAxis = Vector3.forward;
    [Tooltip("Góc xoay mỗi nấc (ví dụ 5 nấc chia 50 độ mỗi nấc)")]
    public float anglePerStep = 50f;

    [Header("Thời gian chuyển nấc khi giữ Grip")]
    [Tooltip("Cứ sau bao nhiêu giây thì nhảy sang nấc tiếp theo")]
    public float stepInterval = 0.3f;

    [Header("Âm thanh khấc xoay")]
    public AudioSource audioSource;
    public AudioClip clickSound;

    [Header("Cấp độ nấc hiện tại")]
    [Range(0, 4)]
    public int currentLevel = 0;

    private XRSimpleInteractable interactable;
    private Quaternion initialRotation;
    private Coroutine stepCoroutine;
    private bool isHolding = false;

    void Awake()
    {
        initialRotation = transform.localRotation;

        interactable = GetComponent<XRSimpleInteractable>();
        if (interactable == null)
            interactable = gameObject.AddComponent<XRSimpleInteractable>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGripStart);
        interactable.selectExited.AddListener(OnGripEnd);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGripStart);
        interactable.selectExited.RemoveListener(OnGripEnd);
        StopAllCoroutines();
    }

    private void OnGripStart(SelectEnterEventArgs args)
    {
        isHolding = true;
        if (stepCoroutine != null)
            StopCoroutine(stepCoroutine);

        stepCoroutine = StartCoroutine(CycleStepsRoutine());
    }

    private void OnGripEnd(SelectExitEventArgs args)
    {
        isHolding = false;
        if (stepCoroutine != null)
        {
            StopCoroutine(stepCoroutine);
            stepCoroutine = null;
        }

        // Cập nhật tốc độ quạt tương ứng với nấc khi nhả tay
        if (targetFan != null)
        {
            targetFan.SetLevel(currentLevel);
        }
    }

    private IEnumerator CycleStepsRoutine()
    {
        while (isHolding)
        {
            // Nhảy nấc xoay vòng: 0 -> 1 -> 2 -> 3 -> 4 -> 0
            currentLevel = (currentLevel + 1) % 5;

            // Xoay góc núm vặn theo cấp
            transform.localRotation = initialRotation * Quaternion.Euler(knobAxis * (currentLevel * anglePerStep));

            // Phát tiếng click nấc vặn
            PlayClickSound();

            yield return new WaitForSeconds(stepInterval);
        }
    }

    private void PlayClickSound()
    {
        if (audioSource != null && clickSound != null)
        {
            audioSource.pitch = Random.Range(0.95f, 1.05f);
            audioSource.PlayOneShot(clickSound);
        }
    }

    // Dùng cho script Manager cha kết nối
    public void BindFan(CeilingFanController fan)
    {
        targetFan = fan;
        if (targetFan != null)
        {
            targetFan.SetLevel(currentLevel);
        }
    }
}