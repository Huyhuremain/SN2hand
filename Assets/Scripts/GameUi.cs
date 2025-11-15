using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUi : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AuthManager authManager; 
    
    [Header("Game References")]
    [SerializeField] private Player player;
    [SerializeField] private Vector3 startPosition = new Vector3(0, 1, 0); // Vị trí khởi tạo mặc định

    public void StartGame()
    {
        if (authManager == null || authManager.auth == null || authManager.auth.CurrentUser == null)
        {
             Debug.LogError("Lỗi: Người dùng chưa đăng nhập hoặc AuthManager/Firebase chưa được khởi tạo.");
             return;
        }

        // Tải dữ liệu của người dùng hiện tại
        string currentUserId = authManager.auth.CurrentUser.UserId; 
        LoadGameData(currentUserId); 

        gameManager.StartGame(); 
    }

    public void SignOutAndReturnToLogin() 
    {
        if (authManager == null)
        {
            Debug.LogError("AuthManager chưa được gán trong GameUI Inspector!");
            return;
        }
        
        authManager.SignOutButton(); 
        
        // Đặt lại trạng thái game hiện tại
        ResetGameState(); 
    }
    
    public void QuitApplication() 
    {
        Application.Quit();
    }
    
    public void ContinueGame()
    {
        gameManager.ResumeGame();
    }

    public void MainMenu()
    {
        gameManager.MainMenu();
    }
    
    // **********************************************
    // --- LOGIC TẢI/RESET TRẠNG THÁI GAME ---
    // **********************************************

    /// <summary>
    /// TRIỂN KHAI THỰC TẾ: Tải dữ liệu (Vị trí và HP) từ PlayerPrefs và áp dụng vào Scene.
    /// </summary>
    private void LoadGameData(string userId)
    {
        if (player == null)
        {
            Debug.LogError("[GameUi] Player chưa được gán. Không thể tải dữ liệu.");
            return;
        }
        
        // TẢI VỊ TRÍ
        if (PlayerPrefs.HasKey(userId + "_posX"))
        {
            float x = PlayerPrefs.GetFloat(userId + "_posX");
            float y = PlayerPrefs.GetFloat(userId + "_posY");
            
            player.transform.position = new Vector3(x, y, player.transform.position.z);
            Debug.Log($"[GameUi] Đã tải vị trí cho {userId}: ({x}, {y})");

            // TẢI HP
            if (PlayerPrefs.HasKey(userId + "_hp"))
            {
                float loadedHp = PlayerPrefs.GetFloat(userId + "_hp");
                player.SetHealth(loadedHp); // Dùng hàm SetHealth mới
                Debug.Log($"[GameUi] Đã tải HP cho {userId}: {loadedHp}");
            }
        }
        else
        {
            // Nếu không tìm thấy dữ liệu (người chơi mới), đặt về vị trí khởi tạo và HP mặc định
            player.transform.position = startPosition;
            player.ResetHealth(); // Đặt HP về MaxHP
            Debug.Log($"[GameUi] Không tìm thấy dữ liệu cho {userId}. Đặt về vị trí và HP khởi tạo.");
        }
        
        // LƯU Ý: LoadGameData không cần reset thế giới vì khi StartGame được gọi, scene sẽ được thiết lập 
        // lại (hoặc bạn có một WorldManager riêng để spawn quái vật).
    }

    /// <summary>
    /// Đặt lại trạng thái người chơi và gọi GameManager để đặt lại trạng thái thế giới.
    /// </summary>
    private void ResetGameState()
    {
        if (player == null)
        {
            Debug.LogError("[GameUi] Player chưa được gán. Không thể reset trạng thái.");
            return;
        }
        
        // 1. RESET TRẠNG THÁI NGƯỜI CHƠI
        player.transform.position = startPosition; 
        player.ResetHealth(); 
        
        // 2. RESET TRẠNG THÁI THẾ GIỚI (MỚI)
        if (gameManager != null)
        {
            gameManager.ResetWorldState(); // GỌI HÀM MỚI TRONG GAMEMANAGER
        }
        
        Debug.Log("[GameUi] Scene game state reset sau khi đăng xuất.");
    }
}