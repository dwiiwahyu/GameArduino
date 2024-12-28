using UnityEngine;
using TMPro;

public class LEDController : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform targetObject;
    public Transform playerTransform;
    public KABELController kabelController;
    public Controller komponenController;
    
    [Header("Referensi UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI ledStatusText;
    
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 5f;
    public float jarakMinimal = 2f;
    
    [Header("Pengaturan Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float offsetZ = 0f;
    
    [Header("Warna UI")]
    public Color successColor = Color.green;
    public Color failedColor = Color.red;

    private Vector3 posisiAwal;
    private bool adaDiMeja = true;
    private bool dapatDinyalakan = false;
    private bool pointsGivenForInstall = false;
    private bool bonusPointsGiven = false;

    void Start()
    {
        posisiAwal = transform.position;
        InisialisasiUI();
    }

    public Vector3 GetInitialPosition()
    {
        return posisiAwal;
    }

    public void ResetToTable()
    {
        adaDiMeja = true;
        pointsGivenForInstall = false;
        bonusPointsGiven = false;
        UpdateStatusUI();
    }

    void InisialisasiUI()
    {
        if (instructionText == null || statusText == null || ledStatusText == null)
        {
            Debug.LogError("TextMeshPro elements belum di-assign! Silakan assign di Inspector.");
            return;
        }
        
        instructionText.text = "3. Ambil LED di meja Led dan Tekan E";
        statusText.text = "Failed";
        statusText.color = failedColor;
        ledStatusText.text = "LED = Mati";
        ledStatusText.color = failedColor;
    }
    
    void Update()
    {
        PeriksaKetergantungan();
        
        if (!adaDiMeja && targetObject != null)
        {
            Vector3 posisiTarget = targetObject.position + new Vector3(offsetX, offsetY, offsetZ);
            transform.position = Vector3.Lerp(transform.position, posisiTarget, moveSpeed * Time.deltaTime);
        }
        
        HandleInput();
    }

    bool CekJarakPemain()
    {
        if (playerTransform == null) return false;
        float jarak = Vector3.Distance(transform.position, playerTransform.position);
        return jarak <= jarakMinimal;
    }
    
    void PeriksaKetergantungan()
    {
        if (kabelController == null || komponenController == null) return;
        
        bool kabelTerpasang = !kabelController.IsObjectOnTable();
        bool komponenTerpasang = !komponenController.IsObjectOnTable();
        dapatDinyalakan = kabelTerpasang && komponenTerpasang;
        
        if (dapatDinyalakan && !adaDiMeja && !bonusPointsGiven)
        {
            ScoreManager.Instance.AddPoints(200);
            bonusPointsGiven = true;
        }
        
        if (!dapatDinyalakan && !adaDiMeja)
        {
            adaDiMeja = true;
            StartCoroutine(GerakkanObjek(posisiAwal));
            UpdateStatusUI();
            bonusPointsGiven = false;
        }
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (targetObject != null)
            {
                if (adaDiMeja && !CekJarakPemain())
                {
                    Debug.Log("Terlalu jauh dari meja LED! Silakan mendekat.");
                    return;
                }
                
                if (adaDiMeja && !dapatDinyalakan)
                {
                    Debug.Log("Pasang Komponen dan Kabel terlebih dahulu!");
                    return;
                }
                
                adaDiMeja = !adaDiMeja;
                
                if (!adaDiMeja && !pointsGivenForInstall)
                {
                    ScoreManager.Instance.AddPoints(100);
                    pointsGivenForInstall = true;
                }
                else if (adaDiMeja)
                {
                    pointsGivenForInstall = false;
                }
                
                UpdateStatusUI();
                
                if (adaDiMeja)
                {
                    StartCoroutine(GerakkanObjek(posisiAwal));
                }
            }
        }
    }

    void UpdateStatusUI()
    {
        if (statusText != null && instructionText != null && ledStatusText != null)
        {
            if (!adaDiMeja && dapatDinyalakan)
            {
                statusText.text = "Success";
                statusText.color = successColor;
                ledStatusText.text = "LED = Nyala";
                ledStatusText.color = successColor;
            }
            else
            {
                statusText.text = "Failed";
                statusText.color = failedColor;
                instructionText.text = "3. Ambil LED di meja Led dan Tekan E";
                ledStatusText.text = "LED = Mati";
                ledStatusText.color = failedColor;
            }
        }
    }

    private System.Collections.IEnumerator GerakkanObjek(Vector3 posisiTarget)
    {
        float jarak = Vector3.Distance(transform.position, posisiTarget);
        
        while (jarak > 0.01f)
        {
            transform.position = Vector3.Lerp(transform.position, posisiTarget, moveSpeed * Time.deltaTime);
            jarak = Vector3.Distance(transform.position, posisiTarget);
            yield return null;
        }
        
        transform.position = posisiTarget;
    }

    public bool IsObjectOnTable()
    {
        return adaDiMeja;
    }
    
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jarakMinimal);
    }
}
