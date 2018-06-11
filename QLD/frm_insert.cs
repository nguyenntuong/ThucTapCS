using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QLD
{
    public partial class Frm_insert : Form
    {
        const string lst_col = "[ID],[Address],[Area],[Owner],[Type],[Aim],[Price]";
        const string lst_col_ins = "[Address],[Area],[Owner],[Type],[Aim],[Price]";

        ThuaDat it;
        List<ThuaDat> type, aim;
        SQLHelper sqlHelper;
        DataDB dB;

        public Frm_insert()
        {
            InitializeComponent();            
        }

        private void Create_connect()
        {
            sqlHelper = new SQLHelper(SQLHelper.host, SQLHelper.dbname, SQLHelper.user, SQLHelper.pass);
            dB = new DataDB(sqlHelper);
        }

        private void Frm_insert_Load(object sender, EventArgs e)
        {
            Create_connect();


            type = new List<ThuaDat>();
            aim = new List<ThuaDat>();
            type = dB.ToList("select [ID],[Name] from tblType");
            aim = dB.ToList("select [ID],[Name] from tblAim");

            foreach (var item in type)
            {
                comboBox1.Items.Add(item["Name"]);
            }

            foreach (var item in aim)
            {
                comboBox2.Items.Add(item["Name"]);
            }
            comboBox1.SelectedIndex = comboBox2.SelectedIndex = 0;            
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            if (!textBox.Text.Equals(""))
            {
                try
                {
                    if(float.Parse(textBox.Text)<0)
                    {
                        MessageBox.Show("Các thông số diện tích hay giá tiền phải là số và không âm!");
                        textBox.Text = "";
                    }
                }
                catch
                {
                    MessageBox.Show("Các thông số diện tích hay giá tiền phải là số!");
                    textBox.Text = "";
                }
            }
        }

        private void Button1_Click(object sender, EventArgs e)
        {            
            
            if(textBox1.Text.Equals("") || textBox2.Text.Equals("") || textBox3.Text.Equals("") || textBox6.Text.Equals(""))
            {
                MessageBox.Show("Bạn phải điền đầy đủ thông tin!");
                return;
            }

            List<object> temp = new List<object>();
            temp.Add(null);
            temp.Add(textBox1.Text);
            temp.Add(textBox2.Text);
            temp.Add(textBox3.Text);
            temp.Add(type[comboBox1.SelectedIndex]["ID"]);
            temp.Add(aim[comboBox2.SelectedIndex]["ID"]);
            temp.Add(textBox6.Text);
            it = new ThuaDat(temp);

            if (dB.InsertData("tblLandInf", lst_col_ins, it) <= 0) return;

            MessageBox.Show("Chèn vào thành công! Đóng cửa sổ này để xem kết quả, hoặc ẩn xuống và load lại kết quả trong cửa sổ chính.");
            button2.Text = "Close";

        }
    }
}
