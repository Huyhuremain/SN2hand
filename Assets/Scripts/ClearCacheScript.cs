using UnityEngine;public class ClearCacheScript : MonoBehaviour

{

// Gán hàm này vào một nút trong giao diện

public void ClearAllPlayerPrefs()

{

PlayerPrefs.DeleteAll();

PlayerPrefs.Save();

Debug.Log("Đã xóa tất cả dữ liệu PlayerPrefs. Phiên đăng nhập có thể đã bị xóa.");

}



// Bạn có thể dùng hàm này bằng cách đặt script này lên một GameObject bất kỳ

// và gọi trong Editor Menu.

[UnityEditor.MenuItem("Tools/Clear All PlayerPrefs")]

private static void ClearEditorPlayerPrefs()

{

PlayerPrefs.DeleteAll();

PlayerPrefs.Save();

Debug.Log("Đã xóa tất cả PlayerPrefs!");

}

}

