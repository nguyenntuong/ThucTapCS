using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

/// <summary>
/// 
/// </summary>
namespace QLD
{
    public partial class Frm_merge : Form
    {
        public Frm_merge()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Create_connect()
        {
            sqlHelper = new SQLHelper(SQLHelper.host, SQLHelper.dbname, SQLHelper.user, SQLHelper.pass);
            dB = new DataDB(sqlHelper);
        }


        private void Frm_merge_Load(object sender, EventArgs e)
        {
            Create_connect();

            openFileDialog1.RestoreDirectory = true;
            openFileDialog1.Filter = "Chuẩn củ(*.xls)|*.xls|Chuẩn mới (*.xlsx)|*.xlsx|All file (*.*)|*.*";
            openFileDialog1.FilterIndex = 2;

            button4.Enabled = flag;

            type = new List<ThuaDat>();
            aim = new List<ThuaDat>();

            type = dB.ToList("select [ID],[Name] from tblType");
            aim = dB.ToList("select [ID],[Name] from tblAim");

            foreach (var item in type)
            {
                checkedListBox1.Items.Add(item["Name"]);
            }

            foreach (var item in aim)
            {
                checkedListBox2.Items.Add(item["Name"]);
            }

        }


        const string lst_col = "[ID],[Address],[Area],[Owner],[Type],[Aim],[Price]";
        const string lst_col_ins = "[Address],[Area],[Owner],[Type],[Aim],[Price]";
        private string pathexcel = "";


        private SQLHelper sqlHelper;
        private DataDB dB;
        private ExcelProcess excelProcess;
        private List<ThuaDat> dataFexcel;
        List<ThuaDat> type, aim;


        private bool flag = false;

        private void Button3_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() != DialogResult.OK) { return; }
            pathexcel = openFileDialog1.FileName;
            textBox7.Text = pathexcel;
        }

        private string GenStringQuery()
        {
            StringBuilder output = new StringBuilder();
            if (!textBox1.Text.Equals(""))
            {
                flag = true;
                output.Append($"Address like N'%{textBox1.Text}%' ");
            }
            if (textBox2.Text.Equals(""))
            {
                textBox2.Text = "0";
            }
            if (textBox6.Text.Equals(""))
            {
                textBox6.Text = "0";
            }
            if (!textBox2.Text.Equals(""))
            {
                flag = true;
                if (!textBox5.Text.Equals(""))
                {
                    if (!output.ToString().Equals(""))
                    {
                        output.Append($" and Area >= '{float.Parse(textBox2.Text)}' and Area <= '{float.Parse(textBox5.Text)}' ");
                    }
                    else
                    {
                        output.Append($"Area >= '{float.Parse(textBox2.Text)}' and Area <= '{float.Parse(textBox5.Text)}' ");
                    }
                }
                else
                {
                    if (!output.ToString().Equals(""))
                    {
                        output.Append($" and Area >= '{float.Parse(textBox2.Text)}' ");
                    }
                    else
                    {
                        output.Append($"Area >= '{float.Parse(textBox2.Text)}' ");
                    }
                }
            }
            if (!textBox3.Text.Equals(""))
            {
                flag = true;
                if (!output.ToString().Equals(""))
                {
                    output.Append($" and Owner like N'%{textBox3.Text}%' ");
                }
                else
                {
                    output.Append($"Owner like N'%{textBox3.Text}%' ");
                }
            }
            if (!textBox6.Text.Equals(""))
            {
                flag = true;
                if (!textBox4.Text.Equals(""))
                {
                    if (!output.ToString().Equals(""))
                    {
                        output.Append($" and Price >= '{float.Parse(textBox6.Text)}' and Price <= '{float.Parse(textBox4.Text)}' ");
                    }
                    else
                    {
                        output.Append($"Price >= '{float.Parse(textBox6.Text)}' and Price <= '{float.Parse(textBox4.Text)}' ");
                    }
                }
                else
                {
                    if (!output.ToString().Equals(""))
                    {
                        output.Append($" and Price >= '{float.Parse(textBox6.Text)}' ");
                    }
                    else
                    {
                        output.Append($"Price >= '{float.Parse(textBox6.Text)}' ");
                    }
                }
            }
            int i = 0;
            StringBuilder builder = new StringBuilder();
            foreach (var item in checkedListBox1.Items)
            {
                if (checkedListBox1.GetItemChecked(i))
                {
                    flag = true;
                    if (!builder.ToString().Equals(""))
                    {
                        builder.Append($" or Type = '{type[i]["ID"]}' ");
                    }
                    else
                    {
                        builder.Append($" Type = '{type[i]["ID"]}' ");
                    }
                }
                i++;
            }
            if (!output.ToString().Equals("") && !builder.ToString().Equals(""))
            {
                output.Append($"and {builder.ToString()} ");
            }
            else
            {
                output.Append(builder.ToString());
            }
            builder = new StringBuilder();
            i = 0;
            foreach (var item in checkedListBox2.Items)
            {
                if (checkedListBox2.GetItemChecked(i))
                {
                    flag = true;
                    if (!builder.ToString().Equals(""))
                    {
                        builder.Append($" or Aim = '{aim[i]["ID"]}' ");
                    }
                    else
                    {
                        builder.Append($" Aim = '{aim[i]["ID"]}' ");
                    }
                }
                i++;
            }
            if (!output.ToString().Equals("") && !builder.ToString().Equals(""))
            {
                output.Append($"and {builder.ToString()} ");
            }
            else
            {
                output.Append(builder.ToString());
            }

            return output.ToString();
        }

        private void TextBox7_TextChanged(object sender, EventArgs e)
        {
            if (textBox7.Text.Equals(""))
            {
                button4.Enabled = false;
                button1.Text = "Lọc";
            }
            else
            {
                button4.Enabled = true;
                button1.Text = "Kết hợp";
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!textBox.Text.Equals(""))
            {
                try
                {
                    float.Parse(textBox.Text);
                }
                catch
                {
                    MessageBox.Show("Các thông số diện tích hay giá tiền phải là số!");
                    textBox.Text = "";
                }
            }
        }

        private void CheckedListBox1_MouseHover(object sender, EventArgs e)
        {
            CheckedListBox chk = (CheckedListBox)sender;
            chk.BorderStyle = BorderStyle.FixedSingle;
            chk.Focus();
        }

        private void CheckedListBox1_MouseLeave(object sender, EventArgs e)
        {
            CheckedListBox chk = (CheckedListBox)sender;
            chk.BorderStyle = BorderStyle.Fixed3D;
        }


        // Delegate
        public delegate void GetMerge(List<ThuaDat> items);

        public GetMerge mGetMerge;

        private void Button4_Click(object sender, EventArgs e)
        {
            textBox7.Text = "";
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            if (!pathexcel.Equals(""))
            {
                excelProcess = new ExcelProcess(pathexcel);
                dataFexcel = new List<ThuaDat>();
                dataFexcel = excelProcess.ImportAllData();
                dB.InsertDatas("tblLandInf", lst_col_ins, dataFexcel);
            }
            string condition = GenStringQuery();
            string tmp = $"select {lst_col} from tblLandInf where {condition}";
            if (flag)
            {
                mGetMerge?.Invoke(dB.ToList(tmp));
            }
            else
            {
                if (MessageBox.Show("Không chọn lọc, xuất tất cả các dữ liệu!", "Chú ý!", MessageBoxButtons.OKCancel) != DialogResult.OK) return;
                mGetMerge?.Invoke(dB.ToList($"select {lst_col} from tblLandInf"));
            }
            this.Close();
        }
    }
}
