using System;
using System.Data.SqlClient;

public class SecurityExample
{
   
    public static bool UnsafeLogin(string username, string password)
    {
    
        string connectionString = "YourConnectionStringHere"; 
        
      
        string sqlQuery = "SELECT COUNT(*) FROM Users WHERE Username = '" + username + "' AND Password = '" + password + "'";

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                
                using (SqlCommand command = new SqlCommand(sqlQuery, connection))
                {
           
                    int userCount = (int)command.ExecuteScalar(); 
                    
                  
                    return userCount > 0;
                }
            }
            catch (Exception ex)
            {
              
                Console.WriteLine("發生錯誤: " + ex.Message);
                return false;
            }
        }
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("--- 測試有漏洞的登入函式 ---");
        
 
        Console.WriteLine("\n[正常登入]");
        bool success1 = UnsafeLogin("Alice", "securepassword123");
        Console.WriteLine("登入結果: " + (success1 ? "成功" : "失敗")); // 預期失敗（因為沒有實際資料庫連線）

     
        
        Console.WriteLine("\n[惡意注入嘗試]");
        string maliciousUsername = "' OR 1=1 --"; 
        string dummyPassword = "anypass";
        
   
        Console.WriteLine($"注入的 Username: {maliciousUsername}");
        bool success2 = UnsafeLogin(maliciousUsername, dummyPassword);
        
        // 再次強調：在實際運行中，這段程式碼在有資料庫連接時會導致所有人都登入成功 (繞過身份驗證)。
    }
}
