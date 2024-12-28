using UnityEngine;
using TMPro;

public class Controller : MonoBehaviour
{
    [Header("Referensi Objek")]
    public Transform targetObject;
    public Transform playerTransform;
    
    [Header("Referensi UI")]
    public TextMeshProUGUI instructionText;
    public TextMeshProUGUI statusText;
    public TextMeshProUGUI pesanPeringatan;
    
    [Header("Pengaturan Pergerakan")]
    public float moveSpeed = 5f;
    
    [Header("Pengaturan Jarak")]
    public float jarakMinimum = 2f;
    
    [Header("Pengaturan Offset")]
    [SerializeField] private float offsetX = 0f;
    [SerializeField] private float offsetY = 1f;
    [SerializeField] private float offsetZ = 0f;
    
    [Header("Warna UI")]
    public Color warnaSuccess = Color.green;
    public Color warnaFailed = Color.red;
    public Color warnaPeringatan = Color.yellow;

    private Vector3 posisiAwal;
    private bool adaDiMeja = true;
    private bool pointsGivenForInstall = false;

    // Method yang dibutuhkan oleh LEDController
    public bool IsObjectOnTable()
    {
        return adaDiMeja;
    }

    void Start()
    {
        posisiAwal = transform.position;
        InisialisasiUI();
    }
    
    void InisialisasiUI()
    {
        if (instructionText == null || statusText == null || pesanPeringatan == null)
        {
            Debug.LogError("Elemen TextMeshPro belum di-assign! Silakan assign di Inspector.");
            return;
        }
        
        instructionText.text = "1. Ambil KOMPONEN di meja Komponen dan Tekan R";
        statusText.text = "Failed";
        statusText.color = warnaFailed;
        pesanPeringatan.text = "";
    }

    bool CekJarakPemain()
    {
        if (playerTransform == null) return false;
        
        float jarak = Vector3.Distance(transform.position, playerTransform.position);
        return jarak <= jarakMinimum;
    }
    
    void Update()
    {
        if (!adaDiMeja && targetObject != null)
        {
            Vector3 posisiTarget = targetObject.position + new Vector3(offsetX, offsetY, offsetZ);
            transform.position = Vector3.Lerp(transform.position, posisiTarget, moveSpeed * Time.deltaTime);
        }
        
        HandleInput();
    }
    
    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            if (targetObject != null)
            {
                if (adaDiMeja && !CekJarakPemain())
                {
                    TampilkanPeringatanJarak();
                    return;
                }

                adaDiMeja = !adaDiMeja;

                // Tambah poin saat berhasil memasang komponen
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
            else
            {
                Debug.LogWarning("Target object belum di-assign! Silakan assign di Inspector.");
            }
        }
    }
    
    void TampilkanPeringatanJarak()
    {
        if (pesanPeringatan != null)
        {
            pesanPeringatan.text = "Anda terlalu jauh dari meja! Silakan mendekat.";
            pesanPeringatan.color = warnaPeringatan;
            StartCoroutine(HapusPeringatanSetelahWaktu(3f));
        }
    }
    
    private System.Collections.IEnumerator HapusPeringatanSetelahWaktu(float waktuTunggu)
    {
        yield return new WaitForSeconds(waktuTunggu);
        if (pesanPeringatan != null)
        {
            pesanPeringatan.text = "";
        }
    }
    
    void UpdateStatusUI()
    {
        if (statusText != null && instructionText != null)
        {
            if (!adaDiMeja)
            {
                statusText.text = "Success";
                statusText.color = warnaSuccess;
                pesanPeringatan.text = "";
            }
            else
            {
                statusText.text = "Failed";
                statusText.color = warnaFailed;
                instructionText.text = "1. Ambil KOMPONEN di meja Komponen dan Tekan R";
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

    // Tambahkan method baru ini
    public Vector3 GetInitialPosition()
    {
        return posisiAwal;
    }

    public void ResetToTable()
    {
        adaDiMeja = true;
        pointsGivenForInstall = false;
        UpdateStatusUI();
    }

    // Untuk memvisualisasikan jarak di editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, jarakMinimum);
    }
}
