using QLD;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace QLD
{
    /// <summary>
    /// .........
    /// </summary>
    class DataDB
    {
        #region privateVar
        /// <summary>
        /// Tạo kết nối tới SQL
        /// </summary>
        private SqlConnection SqlConnection = null;
        /// <summary>
        /// Tương tác Truy vấn với SQL
        /// </summary>
        private SqlCommand SqlCommand = null;
        /// <summary>
        /// Câu truy vấn
        /// </summary>
        private string strqr = "";
        /// <summary>
        /// Get set Câu truy vấn
        /// </summary>
        public string QueryCommand { get => strqr; set => strqr = value; }
        #endregion
        #region Construct
        /// <summary>
        /// Khởi tạo
        /// </summary>
        /// <param name="sQLHelper"> Khởi tạo với tham số từ SQLHelper</param>
        public DataDB(SQLHelper sQLHelper)
        {
            SqlConnection = sQLHelper.Connect();
            CreateSQLCommad();
        }
        #endregion
        #region privateFunction
        /// <summary>
        /// Kiểm tra và tái kết nối tới SQL
        /// </summary>
        private void CheckConnection()
        {

            if (SqlConnection.State == ConnectionState.Closed)
            {
                SqlConnection.Open();
            }
            else
            {
                SqlConnection.Close();
                SqlConnection.Open();
            }

        }
        //Private function
        /// <summary>
        /// Khởi tạo môi trường truy vấn CSDL
        /// </summary>
        private void CreateSQLCommad()
        {
            SqlCommand = new SqlCommand();
            SqlCommand.Connection = SqlConnection;
            SqlCommand.CommandType = CommandType.Text;
        }
        /// <summary>
        /// Truy vấn để lấy DataAdapter
        /// </summary>
        /// <param name="sqlqr"> Câu truy vấn </param>
        /// <returns>Trả về đối tượng SqlDataAdapter</returns>
        private SqlDataAdapter GetAdapterQuery(string sqlqr)
        {
            SqlDataAdapter adapter = new SqlDataAdapter(sqlqr, SqlConnection);
            return adapter;
        }
        /// <summary>
        /// Đổ dữ liệu từ Adapter vào Table
        /// </summary>
        /// <param name="adapter"> SqlDataAdapter </param>
        /// <returns>Trả về DataTable</returns>
        private DataTable GetDataTable(SqlDataAdapter adapter)
        {
            DataTable dataTable = new DataTable();
            adapter.Fill(dataTable);

            return dataTable;
        }
        /// <summary>
        /// Chuyển đổi Table thành danh sách liên kết chứa các dòng trong Table
        /// Do with that
        /// </summary>
        /// <param name="dataTable"> Data Table</param>
        /// <returns> List data rows </returns>
        private List<DataRow> GetDataRowsFromDataTable(DataTable dataTable)
        {
            return dataTable.Select().ToList();
        }
        /// <summary>
        /// Chuyển đổi Table thành danh sách liên kết với các dòng trong Table như một đối tượng Item
        /// Không kèm theo tên Cột trong CSDL
        /// </summary>
        /// <param name="ls"> List DataRow </param>
        /// <returns> List Items</returns>
        private List<ThuaDat> ToListFromDataRows(List<DataRow> ls)
        {

            List<ThuaDat> output = new List<ThuaDat>();

            foreach (var item in ls)
            {
                output.Add(item.ItemArray.ToList());
            }
            return output;
        }
        /// <summary>
        /// Chuyển đổi Table thành danh sách liên kết với các dòng trong Table như một đối tượng Item
        /// Kèm theo tên Cột trong CSDL
        /// </summary>
        /// <param name="dataTable"> DataTable </param>
        /// <returns> List Items</returns>
        private List<ThuaDat> ToListFromDataTable(DataTable dataTable)
        {
            List<string> lst_col_name = new List<string>();
            List<ThuaDat> it = new List<ThuaDat>();
            List<DataRow> dataRows = GetDataRowsFromDataTable(dataTable);

            for (int i = 0; i < dataTable.Columns.Count; i++)
            {
                lst_col_name.Add(dataTable.Columns[i].ColumnName);
            }
            foreach (var item in dataRows)
            {
                it.Add(new ThuaDat(item.ItemArray.ToList(), lst_col_name));
            }
            return it;
        }
        #endregion
        #region publicFunction
        #region getData
        //Public get data function <- SQL
        /// <summary>
        /// Lấy dữ liệu từ SQL
        /// </summary>
        /// <param name="qr">Câu truy vấn</param>
        /// <returns></returns>
        public List<ThuaDat> ToList(string qr)
        {
            if (qr.Equals(""))
            {
                return null;
            }
            else
            {
                List<ThuaDat> rt = ToListFromDataTable(GetDataTable(GetAdapterQuery(qr)));
                return rt;
            }
        }
        /// <summary>
        /// Lấy dữ liệu từ SQL, với cây truy vấn khi khởi tạo
        /// </summary>
        /// <returns></returns>
        public List<ThuaDat> ToList()
        {
            if (strqr.Equals(""))
            {
                return null;
            }
            else
            {
                ThuaDat lst_col_name = new ThuaDat();
                return ToListFromDataTable(GetDataTable(GetAdapterQuery(strqr)));
            }
        }
        #endregion
        #region setData
        //Public set data function -> SQL
        /// <summary>
        /// Insert to DB 
        /// </summary>
        /// <param name="table_name"> Name of Table in DB</param>
        /// <param name="item"> List data in one row </param>
        /// <returns>return rows affect</returns>
        public int InsertData(string table_name, ThuaDat item)
        {
            if(ToList($"SELECT * FROM {table_name} WHERE {item.Columns[1]} like N'{item.ToArray()[1].ToString()}'").Count>0)
            {
                return 0;
            }
            string generqr;
            if (table_name.Equals(""))
            {
                return 0;
            }
            generqr = $"INSERT INTO {table_name}({item.ToSQLStringColumns()}) VALUES({item.ToSQLStringDataInsertQuery("N")})";
            SqlCommand.CommandText = generqr;
            CheckConnection();
            return SqlCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Insert to DB
        /// </summary>
        /// <param name="table_name"> Name of Table in DB </param>
        /// <param name="columns_name"> List column in table to insert, it must correct with data arr in item </param>
        /// <param name="item"> List data in one row </param>
        /// <returns>return rows affect </returns>
        public int InsertData(string table_name, string columns_name, ThuaDat item)
        {
            if (ToList($"SELECT * FROM {table_name} WHERE {columns_name.Split(',')[0]} like N'{item.ToArray()[1].ToString()}'").Count > 0)
            {
                return 0;
            }
            string generqr;
            if (table_name.Equals(""))
            {
                return 0;
            }
            generqr = $"INSERT INTO {table_name}({columns_name}) VALUES({item.ToSQLStringDataInsertQuery("N")})";

            SqlCommand.CommandText = generqr;
            CheckConnection();
            return SqlCommand.ExecuteNonQuery();
        }
        /// <summary>
        /// Insert list Data to DB
        /// </summary>
        /// <param name="table_name">Name of Table in DB</param>
        /// <param name="items">List data in one row</param>
        /// <returns>return rows affect </returns>
        public int InsertDatas(string table_name, List<ThuaDat> items)
        {
            int rows = 0;
            if (table_name.Equals(""))
            {
                return 0;
            }
            foreach (var item in items)
            {
                rows += InsertData(table_name, item);
            }
            return rows;
        }
        /// <summary>
        /// Insert list Data to DB
        /// </summary>
        /// <param name="table_name">Name of Table in DB</param>
        /// <param name="columns_name">List column in table to insert, it must correct with data arr in item</param>
        /// <param name="items">List data in one row</param>
        /// <returns>return rows affect</returns>
        public int InsertDatas(string table_name, string columns_name, List<ThuaDat> items)
        {
            int rows = 0;
            if (table_name.Equals(""))
            {
                return 0;
            }
            foreach (var item in items)
            {
                rows += InsertData(table_name, columns_name, item);
            }
            return rows;
        }
        /// <summary>
        /// Xoá dữ liệu trong DB
        /// </summary>
        /// <param name="table_name">Tên của Table trong DB</param>
        /// <param name="condition">Điều kiện</param>
        /// <returns>Số dòng bị ảnh hưởng</returns>
        public int DeleteData(string table_name, string condition)
        {
            if (table_name.Equals(""))
            {
                return 0;
            }
            string gqr = $"DELETE FROM {table_name} WHERE {condition}; ";
            SqlCommand.CommandText = gqr;
            CheckConnection();
            return SqlCommand.ExecuteNonQuery();
        }
        #endregion
        #endregion
    }



    /// <summary>
    /// Định nghĩa kiểu dữ liệu thữa đất
    /// </summary>
    public class ThuaDat
    {
        #region privateVar
        /// <summary>
        /// Dữ liệu trong thữa đất
        /// </summary>
        private List<object> item;
        /// <summary>
        /// Tên cột trong CSDL
        /// </summary>
        private List<string> columns;
        /// <summary>
        /// Get set tên cột
        /// </summary>
        public List<string> Columns { get => columns; set => columns = value; }
        #endregion
        #region Construct
        /// <summary>
        /// Default contruct
        /// </summary>
        public ThuaDat()
        {

        }
        /// <summary>
        /// Chỉ lưu danh sách tên cột
        /// </summary>
        /// <param name="columns_name">list column name example: "a,b,c,d"</param>
        public ThuaDat(string columns_name)
        {
            string[] slipt = columns_name.Split(',');
            columns = slipt.ToList();
        }
        /// <summary>
        /// Chỉ lưu dữ liệu thữa đất
        /// </summary>
        /// <param name="row"> Dữ liệu một dòng trong CSDL</param>
        public ThuaDat(List<object> row)
        {
            item = row;
        }
        /// <summary>
        /// Lưu cả dữ liệu kèm tên cột
        /// </summary>
        /// <param name="row">List data in row</param>
        /// <param name="columns_name">list column name</param>
        public ThuaDat(List<object> row, List<string> columns_name)
        {
            item = row;
            columns = columns_name;
        }
        #endregion
        #region overloadOperator
        public static implicit operator ThuaDat(List<object> value)
        {
            return new ThuaDat(value);
        }

        public object this[int i]
        {
            get
            {
                return item[i];
            }
            set
            {
                item[i] = value;
            }
        }

        public object this[string column_name]
        {
            get
            {

                for (int i = 0; i < columns.Count; i++)
                {
                    if (columns[i].Equals(column_name))
                    {
                        return item[i];
                    }
                }
                return null;
            }
            set
            {
                int i = 0;
                for (i = 0; i < columns.Count; i++)
                {
                    if (columns[i].Equals(column_name))
                    {
                        item[i] = value;
                    }
                }
            }
        }

        #endregion
        #region publicFunction
        /// <summary>
        /// Tách dữ liệu ra thành một danh sách
        /// </summary>
        /// <returns>object array</returns>
        public object[] ToArray(int stt = -1)
        {
            if (stt == -1)
            {
                object[] output = new object[item.Count];

                for (int i = 0; i < item.Count; i++)
                {
                    output[i] = item[i];
                }
                return output;
            }
            else
            {
                object[] output = new object[item.Count];
                output[0] = stt;
                for (int i = 1; i < item.Count; i++)
                {
                    output[i] = item[i];
                }
                return output;
            }
        }
        /// <summary>
        /// To String 
        /// Dành cho kiểm thử
        /// </summary>
        /// <returns>Example: data,data,data</returns>
        public override string ToString()
        {
            StringBuilder output = new StringBuilder();
            int i = 0;
            for (i = 0; i < item.Count - 1; i++)
            {
                output.Append(item[i].ToString() + ",");
            }
            output.Append(item[i].ToString());
            return output.ToString();
        }
        /// <summary>
        /// Định dạng dữ liệu xuất dùng trong câu truy vấn SQL
        /// </summary>
        /// <returns>Example: 'data','data','data'</returns>
        public string ToSQLStringDataInsertQuery(string encode = "")
        {
            StringBuilder output = new StringBuilder();
            int i = 0;
            for (i = 1; i < item.Count - 1; i++)
            {
                output.Append(encode + "'" + item[i].ToString() + "',");
            }
            output.Append(encode + "'" + item[i].ToString() + "'");
            return output.ToString();
        }

        /// <summary>
        /// Định dạng danh sách cột dùng trong câu truy vấn SQL
        /// </summary>
        /// <returns>Example: col1,col2,col3</returns>
        public string ToSQLStringColumns()
        {
            StringBuilder output = new StringBuilder();
            int i = 0;
            for (i = 1; i < columns.Count - 1; i++)
            {
                output.Append(columns[i] + ",");
            }
            output.Append(columns[i]);
            return output.ToString();
        }
        /// <summary>
        /// Number of column in row
        /// </summary>
        /// <returns>Count</returns>
        public int Count()
        {
            return item.Count;
        }
        /// <summary>
        /// Get home number in column Address
        /// Custom
        /// </summary>
        /// <param name="index">Column index</param>
        /// <returns>Home number</returns>
        public string GetHomeNumber(int index)
        {
            return item[index].ToString().Split(' ')[0];
        }
        /// <summary>
        /// Compare to another object
        /// Compare Address
        /// </summary>
        /// <param name="other"> other Item</param>
        /// <returns></returns>
        public int CompareTo(ThuaDat other)
        {
            string[] s1 = this.GetHomeNumber(1).Split('/');
            string[] s2 = other.GetHomeNumber(1).Split('/');
            if (s1.Length > s2.Length)
            {
                return 1;
            }
            else if (s1.Length < s2.Length)
            {
                return -1;
            }
            else
            {
                for (int i = s1.Length - 1; i >= 0; i--)
                {
                    if (s1[i].CompareTo(s2[i]) > 0)
                    {
                        return 1;
                    }
                    else if (s1[i].CompareTo(s2[i]) < 0)
                    {
                        return -1;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return 0;
        }
        /// <summary>
        /// Set column name custom
        /// </summary>
        /// <param name="columns_name">EXP "col1,col2,col3,..."</param>
        public void SetColumnsname(string columns_name)
        {
            string[] slipt = columns_name.Split(',');
            columns = slipt.ToList();
        }
        #endregion
    }
}
