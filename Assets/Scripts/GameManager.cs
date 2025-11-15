using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Các biến UI cũ
    [SerializeField] private GameObject loginRegisterMenu; 
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameOverMenu;
    [SerializeField] private GameObject pauseMenu;
    
    void Start()
    {
        // Logic hiển thị menu ban đầu được quản lý hoàn toàn trong AuthManager.Start/AuthStateChanged
    }

    // --- CÁC HÀM QUẢN LÝ MENU (Không đổi) ---
    
    public void LoginRegisterMenu()
    {
        HideAllMenus(); 
        loginRegisterMenu.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }

    public void MainMenu()
    {
        HideAllMenus();
        mainMenu.SetActive(true);
        Time.timeScale = 0f; // Dừng game
    }
    
    public void GameOverMenu()
    {
        HideAllMenus();
        gameOverMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void PauseGameMenu()
    {
        HideAllMenus();
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
    }
    
    public void StartGame()
    {
        HideAllMenus();
        Time.timeScale = 1f;
    }
    
    public void ResumeGame()
    {
        StartGame();
    }
    
    private void HideAllMenus()
    {
        loginRegisterMenu.SetActive(false);
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        pauseMenu.SetActive(false);
    }
    
    // --- HÀM RESET TRẠNG THÁI THẾ GIỚI (Cập nhật để gọi EnemyManager) ---

    /// <summary>
    /// Đặt lại trạng thái toàn bộ thế giới game (Quái vật, Item, v.v.).
    /// </summary>
    public void ResetWorldState()
    {
        // 1. TÌM VÀ GỌI RESET ENEMY MANAGER
        EnemyManager enemyManager = FindAnyObjectByType<EnemyManager>();
        if (enemyManager != null)
        {
            // EnemyManager sẽ tự lo việc hủy Enemy hiện tại và reset timer
            enemyManager.ResetManager(); 
        }
        else
        {
            // Nếu không có EnemyManager (hoặc không tìm thấy), thực hiện xóa thủ công
            // (Giữ lại logic này để đảm bảo)
            BasicEnemy[] enemies = FindObjectsByType<BasicEnemy>(FindObjectsSortMode.None);
            foreach (BasicEnemy enemy in enemies)
            {
                if (enemy != null)
                {
                    Destroy(enemy.gameObject);
                }
            }
        }
        
        Debug.Log("[GameManager] Đã hoàn thành reset trạng thái thế giới.");

        // TODO: Thêm logic xóa các loại Item, hoặc reset điểm số khác tại đây.
    }
}