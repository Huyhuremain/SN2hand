using UnityEngine;
using Firebase.Auth;
using System.Threading.Tasks;
using TMPro;

public class AuthManager : MonoBehaviour
{
    private GameManager gameManager;

    [Header("UI References")]
    [SerializeField] private TMP_InputField emailInput;
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private TextMeshProUGUI statusText;

    // ĐÃ SỬA: Đổi kiểu dữ liệu và tên biến từ PlayerController playerController thành Player player
    [Header("Game References")]
    [SerializeField] private Player player; 
    
    public FirebaseAuth auth;

    void Awake()
    {
        gameManager = FindAnyObjectByType<GameManager>();
    }

    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        auth.StateChanged += AuthStateChanged;
        AuthStateChanged(this, null); 
    }
    
    void AuthStateChanged(object sender, System.EventArgs eventArgs)
    {
        if (auth.CurrentUser != null)
        {
            gameManager.MainMenu();
        }
        else
        {
            gameManager.LoginRegisterMenu();
            if (statusText != null)
            {
                statusText.text = "Vui lòng đăng nhập hoặc đăng ký.";
            }
        }
    }
    
    // ... (Logic Đăng ký/Đăng nhập và Xử lý lỗi không đổi) ...
    // ... (Hàm SignOutButton không đổi) ...

    public void RegisterButton() 
    {
        RegisterProcess(emailInput.text, passwordInput.text);
    }

    private async void RegisterProcess(string email, string password)
    {
        if (statusText != null) statusText.text = "Đang đăng ký...";

        try
        {
            AuthResult result = await auth.CreateUserWithEmailAndPasswordAsync(email, password);
            
            if (statusText != null)
            {
                statusText.text = $"Đăng ký thành công! Chào mừng {result.User.Email}.";
            }
        }
        catch (System.Exception e)
        {
            AuthError error = AuthError.None;
            Firebase.FirebaseException firebaseEx = e as Firebase.FirebaseException;
            if (firebaseEx != null)
            {
                error = (AuthError)firebaseEx.ErrorCode;
            }
            
            string errorMessage = GetErrorMessage(error);
            Debug.LogError($"Register Failed: {errorMessage}");
            if (statusText != null)
            {
                statusText.text = $"Đăng ký thất bại: {errorMessage}";
            }
        }
    }

    public void LoginButton() 
    {
        LoginProcess(emailInput.text, passwordInput.text);
    }

    private async void LoginProcess(string email, string password)
    {
        if (statusText != null) statusText.text = "Đang đăng nhập...";

        try
        {
            AuthResult result = await auth.SignInWithEmailAndPasswordAsync(email, password);

            if (statusText != null)
            {
                statusText.text = $"Đăng nhập thành công! Chào mừng {result.User.Email}.";
            }
        }
        catch (System.Exception e)
        {
            AuthError error = AuthError.None;
            Firebase.FirebaseException firebaseEx = e as Firebase.FirebaseException;
            if (firebaseEx != null)
            {
                error = (AuthError)firebaseEx.ErrorCode;
            }
            
            string errorMessage = GetErrorMessage(error);
            Debug.LogError($"Login Failed: {errorMessage}");
            if (statusText != null)
            {
                statusText.text = $"Đăng nhập thất bại: {errorMessage}";
            }
        }
    }

    private string GetErrorMessage(AuthError error)
    {
        switch (error)
        {
            case AuthError.MissingEmail:
            case AuthError.InvalidEmail:
                return "Email không hợp lệ.";
            case AuthError.MissingPassword:
                return "Vui lòng nhập mật khẩu.";
            case AuthError.WeakPassword:
                return "Mật khẩu quá yếu (tối thiểu 6 ký tự).";
            case AuthError.EmailAlreadyInUse:
                return "Email đã được sử dụng.";
            case AuthError.UserNotFound:
                return "Tài khoản không tồn tại.";
            case AuthError.WrongPassword:
                return "Mật khẩu không đúng.";
            default:
                return "Lỗi không xác định. Vui lòng kiểm tra kết nối mạng.";
        }
    }
    
    public void SignOutButton()
    {
        if (auth.CurrentUser == null)
        {
            if (statusText != null) statusText.text = "Bạn chưa đăng nhập.";
            return;
        }

        auth.SignOut();
        
        if (emailInput != null) emailInput.text = "";
        if (passwordInput != null) passwordInput.text = ""; 

        if (statusText != null)
            statusText.text = "Đã đăng xuất. Vui lòng đăng nhập lại.";
    }

    // --- LOGIC LƯU VÀ THOÁT ---

    /// <summary>
    /// TRIỂN KHAI THỰC TẾ: Lưu vị trí và máu người chơi bằng PlayerPrefs
    /// </summary>
    private void SaveGame()
    {
        // Đã sửa: Kiểm tra player thay vì playerController
        if (auth.CurrentUser == null || player == null)
        {
            Debug.LogError("Lỗi lưu game: Người dùng chưa đăng nhập hoặc Player chưa được gán.");
            return;
        }
        
        string userId = auth.CurrentUser.UserId;
        Vector3 position = player.transform.position; 
        float health = player.currentHp; // Lấy máu hiện tại (từ script Player)
        
        // Lưu vị trí
        PlayerPrefs.SetFloat(userId + "_posX", position.x);
        PlayerPrefs.SetFloat(userId + "_posY", position.y);
        
        // LƯU HP
        PlayerPrefs.SetFloat(userId + "_hp", health); 
        
        PlayerPrefs.Save(); 
        
        Debug.Log($"Lưu game cho User ID: {userId} tại ({position.x}, {position.y}) với HP: {health}");
        if (statusText != null) statusText.text = "Đang lưu tiến trình...";
    }

    public void SaveAndReturnToMainMenu()
    {
        SaveGame(); 
        gameManager.StartGame(); 
        gameManager.MainMenu(); 
        
        if (statusText != null)
            statusText.text = "Đã lưu và trở về Main Menu.";
    }
}