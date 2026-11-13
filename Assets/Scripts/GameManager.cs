using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public int totalKoin;
    private int koinTerkumpul = 0;

    [Header("UI References")]
    public TextMeshProUGUI teksSkor;
    public GameObject teksMenang;
    public GameObject gameOverPanel; // Panel Game Over (buat di Canvas)

    [Header("Item Data")]
    public ItemDetails coinData; 

    void Start()
    {
        totalKoin = GameObject.FindGameObjectsWithTag("Coin").Length;

        if (teksMenang != null) 
        {
            teksMenang.SetActive(false);
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
        
        UpdateSkorUI();

        // Pastikan game berjalan normal
        Time.timeScale = 1f;
    }

    public void AmbilKoin(Sprite icon = null)
    {
        koinTerkumpul++;
        UpdateSkorUI();

        // Kirim data ScriptableObject ItemCoin ke InventoryManager


        
        if (InventoryManager.Instance != null && coinData != null)
        {
            InventoryManager.Instance.AddItem(coinData);
        }

        if (koinTerkumpul == totalKoin)
        {
            Menang();
        }
    }

    void UpdateSkorUI()
    {
        if (teksSkor != null)
        {
            teksSkor.text = "Skor: " + koinTerkumpul;
        }
    }

    void Menang()
    {
        Debug.Log("KAMU MENANG!");

        if (teksMenang != null)
        {
            teksMenang.SetActive(true);
        }
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        // Hentikan game
        Time.timeScale = 0f;
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}