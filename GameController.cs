using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; private set; }

    [Header("Component References")]
    public Controller komponenController;
    public KABELController kabelController;
    public LEDController ledController;
    
    [Header("UI References")]
    public TextMeshProUGUI completionText;
    public TextMeshProUGUI enterPromptText; // Text untuk menampilkan petunjuk tekan Enter
    
    private bool isGameComplete = false;
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SetupUI();
    }

    private void SetupUI()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        
        if (enterPromptText != null)
        {
            enterPromptText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        CheckCompletion();
        CheckEnterKeyPress();
    }

    private void CheckEnterKeyPress()
    {
        if (isGameComplete && Input.GetKeyDown(KeyCode.Return))
        {
            RestartGame();
        }
    }

    private void CheckCompletion()
    {
        if (komponenController == null || kabelController == null || ledController == null) return;

        bool isComplete = !komponenController.IsObjectOnTable() && 
                         !kabelController.IsObjectOnTable() && 
                         !ledController.IsObjectOnTable();

        if (isComplete && !isGameComplete)
        {
            isGameComplete = true;
            ShowCompletionUI();
        }
    }

    private void ShowCompletionUI()
    {
        if (completionText != null)
        {
            completionText.gameObject.SetActive(true);
            completionText.text = "Selamat! Anda telah menyelesaikan pemasangan!";
            completionText.color = Color.green;
        }
        
        if (enterPromptText != null)
        {
            enterPromptText.gameObject.SetActive(true);
            enterPromptText.text = "Tekan ENTER untuk mengulang";
            enterPromptText.color = Color.white;
        }
    }

    private void RestartGame()
    {
        Debug.Log("Restart Game triggered");
        
        // Reset score
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.ResetScore();
        }

        // Reset Komponen positions
        ResetComponents();

        // Hide UI elements
        if (completionText != null)
        {
            completionText.gameObject.SetActive(false);
        }
        
        if (enterPromptText != null)
        {
            enterPromptText.gameObject.SetActive(false);
        }

        // Reset game complete flag
        isGameComplete = false;
    }

    private void ResetComponents()
    {
        if (komponenController != null)
        {
            komponenController.transform.position = komponenController.GetInitialPosition();
            komponenController.ResetToTable();
        }

        if (kabelController != null)
        {
            kabelController.transform.position = kabelController.GetInitialPosition();
            kabelController.ResetToTable();
        }

        if (ledController != null)
        {
            ledController.transform.position = ledController.GetInitialPosition();
            ledController.ResetToTable();
        }
    }
}
