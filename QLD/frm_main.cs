using System;
using System.Collections.Generic;
using System.Windows.Forms;

/// <summary>
/// IDE : Visual Studio 2017
/// .NET : 4.5 
/// Office : 2013
/// Cấu hình kết nối máy chủ SQL  => SQLHelper.cs
/// </summary>

namespace QLD
{
    public partial class frm_main : Form
    {
        public frm_main()
        {
            InitializeComponent();
        }

        const string lst_col = "[ID],[Address],[Area],[Owner],[Type],[Aim],[Price]";
        const string lst_col_ins = "[Address],[Area],[Owner],[Type],[Aim],[Price]";


        SQLHelper sqlHelper;
        DataDB dB;
        ThuaDat col;        
        List<ThuaDat> dataFexcel;
        List<ThuaDat> dataAllBoth;

        ExcelProcess excelProcess = null;

        private void Create_connect()
        {
            sqlHelper = new SQLHelper(SQLHelper.host, SQLHelper.dbname,SQLHelper.user,SQLHelper.pass);
            while(sqlHelper.Connect()==null)
            {
                if(MessageBox.Show("Cố gắng tạo một kết nối tới SQLServer thất bại, kiểm tra Server !", "Lỗi!", MessageBoxButtons.RetryCancel, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1)
                    !=DialogResult.Retry)
                {
                    Environment.Exit(1);
                }
            }
            dB = new DataDB(sqlHelper);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Create_connect();

            saveFileDialog1.Filter = "Chuẩn củ(*.xls)|*.xls|Chuẩn mới (*.xlsx)|*.xlsx|All file (*.*)|*.*";
            saveFileDialog1.FilterIndex = 2;
            saveFileDialog1.RestoreDirectory = true;

            
            openFileDialog1.Filter = "Chuẩn củ(*.xls)|*.xls|Chuẩn mới (*.xlsx)|*.xlsx|All file (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;
            openFileDialog1.RestoreDirectory = true;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) return;
            textBox1.Text = openFileDialog1.FileName;

            string path = textBox1.Text;

            excelProcess = new ExcelProcess(path);

            dataFexcel = new List<ThuaDat>();

            dataFexcel = excelProcess.ImportAllData();

            dB.InsertDatas("tblLandInf", lst_col_ins, dataFexcel);            

            LoadGrid();            
        }

        private void LoadGrid()
        {
            dataAllBoth = dB.ToList($"select {lst_col} from tblLandInf");

            HeapSort hs = new HeapSort(dataAllBoth);
            dataAllBoth = hs.GetListSorted();

            label4.Text = dataAllBoth.Count + " kết quả !";

            dataGridView1.Rows.Clear();
            int i = 1;
            foreach (var item in dataAllBoth)
            {
                dataGridView1.Rows.Add(item.ToArray(i));
                i++;
            }
        }

        private void NonQLoadGrid()
        {            
            HeapSort hs = new HeapSort(dataAllBoth);
            dataAllBoth = hs.GetListSorted();
            label4.Text = dataAllBoth.Count + " kết quả !";
            dataGridView1.Rows.Clear();
            int i = 1;
            foreach (var item in dataAllBoth)
            {
                dataGridView1.Rows.Add(item.ToArray(i));
                i++;
            }
        }

        private void LoadGrid(string condition)
        {
            dataAllBoth = dB.ToList($"select {lst_col} from tblLandInf where {condition}");
            HeapSort hs = new HeapSort(dataAllBoth);
            dataAllBoth = hs.GetListSorted();
            label4.Text = dataAllBoth.Count + " kết quả !";
            dataGridView1.Rows.Clear();
            int i = 1;
            foreach (var item in dataAllBoth)
            {
                dataGridView1.Rows.Add(item.ToArray(i));
                i++;
            }
        }

        private void ExitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Button7_Click(object sender, EventArgs e)
        {
            
            if (saveFileDialog1.ShowDialog() != DialogResult.OK) return;
            label3.Text = saveFileDialog1.FileName;

            string pathexp = label3.Text;

            //Set custom column name in file export
            col = new ThuaDat("STT,Địa chỉ,Diện tích,Chủ sở hữu hiện tại,Loại nhà,Mục đích sử dụng,Giá tiền");
            
            ExcelProcess.ExportData(pathexp, dataAllBoth, col);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void Button3_Click(object sender, EventArgs e)
        {
            Form fr = new Frm_insert();
            fr.FormClosed += Fr_FormClosed;
            fr.Show();
        }

        private void Fr_FormClosed(object sender, FormClosedEventArgs e)
        {
            LoadGrid();
            this.Activate();
        }

        private void Button4_Click(object sender, EventArgs e)
        {
            string key = textBox2.Text;
            if(key.Equals(""))
            {
                if(MessageBox.Show("Bạn có muốn xoá toàn bộ danh sách hiện tại !","Xác nhận!",MessageBoxButtons.OKCancel)!=DialogResult.OK)
                {
                    return;
                }
            }
            int rf=dB.DeleteData("tblLandInf", $"[Address] like N'%{key}%'");
            MessageBox.Show($"Đã xoá {rf} thữa đất phù hợp với điều kiện !");
            LoadGrid();
        }

        private void Button5_Click(object sender, EventArgs e)
        {
            string key = textBox2.Text;
            LoadGrid($"[Address] like N'%{key}%'");
        }

        private void LoadFromDataBaseToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            LoadGrid();
        }

        // Dùng cho Delegate callback
        public void GetMerge(List<ThuaDat> items)
        {
            dataAllBoth = items;
            NonQLoadGrid();
        }

        private void Button6_Click(object sender, EventArgs e)
        {
            Frm_merge frm_merge = new Frm_merge();
            frm_merge.mGetMerge = new Frm_merge.GetMerge(GetMerge);
            frm_merge.Show();
        }

        private void ThôngTinThànhViênThựcHiệnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("1. Nguyễn Nhựt Tường \n\t15551150058 - KM15" +
                "\n2. Lưu Văn Hùng \n\t15551150031 - KM15", "Thông tin thành viên thực hiện !",  MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }
    }
}
