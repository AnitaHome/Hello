using System;
using System.Data.SqlClient;

public class SecurityExample
{
    // ⚠️ 警告：這個方法包含一個嚴重的 SQL 注入漏洞！
    public static bool UnsafeLogin(string username, string password)
    {
        // 假設這是您的資料庫連接字串
        string connectionString = "YourConnectionStringHere"; 
        
        // ❌ 漏洞點：直接將使用者輸入拼接到 SQL 字串中。
        // 這允許惡意用戶通過輸入特殊的 SQL 語法來改變查詢的邏輯。
        string sqlQuery = "SELECT COUNT(*) FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "'";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
                    // 執行查詢並獲取結果
                    int userCount = (int)command.ExecuteScalar(); 
                    
                    // 如果結果大於 0，則登入成功
                    return userCount > 0;
                }
            }
            catch (Exception ex)
            {
                // 實務上應該記錄錯誤，但為了範例簡潔，我們只輸出訊息
                Console.WriteLine("發生錯誤: " + ex.Message);
                return false;
            }
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("--- 測試有漏洞的登入函式 ---");
        
        // 1. 正常登入嘗試
        Console.WriteLine("\n[正常登入]");
        bool success1 = UnsafeLogin("Alice", "securepassword123");
        Console.WriteLine("登入結果: " + (success1 ? "成功" : "失敗")); // 預期失敗（因為沒有實際資料庫連線）

        // 2. 惡意 SQL 注入嘗試
        // 惡意使用者輸入： ' OR '1'='1' --
        // 當這個輸入被拼接到查詢中時，SQL 查詢會變成：
        // SELECT COUNT(*) FROM Users WHERE Username = '' OR '1'='1' --' AND Password = '...'
        // 由於 '1'='1' 永遠為 True，且 '--' 會將後面的內容註釋掉，
        // 查詢將檢查所有用戶並返回計數，實現無需密碼登入。
        
        Console.WriteLine("\n[惡意注入嘗試]");
        string maliciousUsername = "' OR 1=1 --"; 
        string dummyPassword = "anypass";
        
        // 由於我們沒有實際的資料庫連線，這個示範在執行時會失敗在連線步驟。
        // 但如果連線成功，且資料庫中 Users 表格存在，這個輸入將會導致登入成功。
        Console.WriteLine($"注入的 Username: {maliciousUsername}");
        bool success2 = UnsafeLogin(maliciousUsername, dummyPassword);
        
        // 再次強調：在實際運行中，這段程式碼在有資料庫連接時會導致所有人都登入成功 (繞過身份驗證)。
    }
}
