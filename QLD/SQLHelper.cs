using System.Data.SqlClient;

namespace QLD
{
    class SQLHelper
    {
        #region privateVar
        /// <summary>
        /// Chứa chuổi kết nối
        /// </summary>
        private string cnnString;
        #endregion
        #region staticVar-Cấu hình kết nối máy chủ SQL
        /// <summary>
        /// Cấu hình kết nối máy chủ SQL
        /// </summary>
        static public string host = @"WILD-PC\SQLEXPRESS";
        static public string dbname = @"NHADAT";
        static public string user = @"";
        static public string pass = @"";
        #endregion
        #region Construct
        /// <summary>
        /// Khởi tạo đối tượng SQLHelper
        /// Tạo Connection string
        /// </summary>
        /// <param name="DataSource">SQLServer name</param>
        /// <param name="DataBase">Database name</param>
        /// <param name="username">Tài khoản đăng nhập( nếu có )</param>
        /// <param name="password">Mật khẩu</param>
        public SQLHelper(string DataSource,string DataBase,string username="",string password="")
        {
            if(username.Equals("")&&password.Equals(""))
            {
                cnnString = $"Data Source={DataSource};Initial Catalog={DataBase};integrated security=True";
            }
            else
            {
                cnnString = $"data source={DataSource};initial catalog={DataBase};user id={username};password={password}";
            }   
        }
        #endregion
        #region privateFunction

        #endregion
        #region publicFunction
        /// <summary>
        /// Tạo một kết nối tới DB
        /// Kiểm tra kết nối
        /// </summary>
        /// <returns>Trả về SqlConnection</returns>
        public SqlConnection Connect()
        {            
            SqlConnection sqlConnection= new SqlConnection(cnnString);
            try
            {
                sqlConnection.Open();
                sqlConnection.Close();
                return sqlConnection;
            }
            catch (SqlException)
            {
                return null;
            }
        }
        #endregion
    }
}
