using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

public class CameraRaycastHUD : MonoBehaviour
{
    [Header("Cấu hình Raycast")]
    [SerializeField] private float khoangCachTuongTac = 3.0f;
    [SerializeField] private LayerMask layerTuongTac = ~0;

    [Header("UI Crosshair")]
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color mauBinhThuong = Color.white;
    [SerializeField] private Color mauKhiCham = Color.green;
    [SerializeField] private Vector3 kichThuocBinhThuong = Vector3.one;
    [SerializeField] private Vector3 kichThuocKhiCham = new Vector3(1.4f, 1.4f, 1.4f);

    [Header("UI Panel Thông Tin")]
    [SerializeField] private GameObject panelThongTin;
    [SerializeField] private TextMeshProUGUI txtTen;
    [SerializeField] private TextMeshProUGUI txtHuongDan;

    private VatTheTuongTac vatTheHienTai;

    void Update()
    {
        KiemTraRaycast();
        XuLyBamNut();
    }

    void KiemTraRaycast()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, khoangCachTuongTac, layerTuongTac))
        {
            VatTheTuongTac vatThe = hit.collider.GetComponentInParent<VatTheTuongTac>();

            if (vatThe != null)
            {
                vatTheHienTai = vatThe;
                CapNhatUI(true, vatThe.tenVatThe, vatThe.huongDan);
                return;
            }
        }

        vatTheHienTai = null;
        CapNhatUI(false, "", "");
    }

    void CapNhatUI(bool chamDung, string ten, string huongDan)
    {
        if (crosshairImage != null)
        {
            crosshairImage.color = chamDung ? mauKhiCham : mauBinhThuong;
            crosshairImage.rectTransform.localScale = chamDung ? kichThuocKhiCham : kichThuocBinhThuong;
        }

        if (panelThongTin != null)
        {
            if (panelThongTin.activeSelf != chamDung)
            {
                panelThongTin.SetActive(chamDung);
            }

            if (chamDung)
            {
                if (txtTen != null) txtTen.text = ten;
                if (txtHuongDan != null) txtHuongDan.text = huongDan;
            }
        }
    }

    void XuLyBamNut()
    {
        bool coBamNut = false;

        // Bắt phím E hoặc Chuột trái qua New Input System
        if (Keyboard.current != null && Keyboard.current.eKey.wasPressedThisFrame) coBamNut = true;
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame) coBamNut = true;

        if (vatTheHienTai != null && coBamNut)
        {
            vatTheHienTai.TuongTac();
        }
    }
}