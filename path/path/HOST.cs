using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace path
{
    public partial class HOST : Form
    {
        private string[] hosts;
        private int LastAccessIndex = -1;
        private bool isSelecting = false;
        private string configPath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        public HOST()
        {
            InitializeComponent();
            hosts = File.ReadAllLines(this.configPath);
            BindData(hosts);
        }

        private void BindData(string[] hosts)
        {
            dataGridView1.Rows.Clear();
            dataGridView1.AutoGenerateColumns = true;

            for (int i = 0; i < hosts.Length; i++)
            {
                string line = hosts[i];
                if (line.Length == 0 || line.Substring(0, 1).Equals("#")) continue;
                string[] field = line.Split(new string[] { " " }, 2, StringSplitOptions.None);
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells["name"].Value = field[0];
                dataGridView1.Rows[index].Cells["domain"].Value = field[1];
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            List<string> l_l = new List<string>();
            for (int i = 0; i < hosts.Length; i++)
            {
                if (hosts[i].ToLower().IndexOf(textBox1.Text.ToLower()) == -1) continue;
                l_l.Add(hosts[i]);
            }
            string[] s_l = new string[l_l.Count];
            for (int i = 0; i < l_l.Count; i++)
            {
                s_l[i] = l_l[i];
            }
            BindData(s_l);
        }

        private void dataGridView1_MouseUp(object sender, MouseEventArgs e)
        {
            this.isSelecting = true;
            string str = "【" + DateTime.Now.ToString() + "】 ";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                DataGridViewRow dgvr = dataGridView1.Rows[i];
                if (!dgvr.Selected) continue;
                textBox2.Text = dgvr.Cells[0].Value.ToString();
                textBox3.Text = dgvr.Cells[1].Value.ToString();
                str += dgvr.Cells[0].Value.ToString() + "\t" + dgvr.Cells[1].Value.ToString() + "\r\n";
                this.LastAccessIndex = i;
            }
            File.AppendAllText("c:\\path.log", str);
            try
            {
                Clipboard.SetDataObject(str);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            this.isSelecting = false;
        }

        private void SaveRow()
        {
            if (this.LastAccessIndex == -1 || this.isSelecting) return;
            DataGridViewRow dgvr = dataGridView1.Rows[this.LastAccessIndex];
            dgvr.Cells[0].Value = textBox2.Text;
            dgvr.Cells[1].Value = textBox3.Text;
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            SaveRow();
        }

        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            SaveRow();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Trim().Length == 0 || textBox4.Text.Trim().Length == 0) return;
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
                if (dataGridView1.Rows[i].Cells[0].Value.ToString().Trim().Equals(textBox4.Text.Trim().ToString())) return;
            int index = dataGridView1.Rows.Add();
            dataGridView1.Rows[index].Cells[0].Value = textBox4.Text.Trim();
            dataGridView1.Rows[index].Cells[1].Value = textBox5.Text.Trim();
            string[] n_host = new string[this.hosts.Length + 1];
            int j;
            for (j = 0; j < this.hosts.Length; j++)
                n_host[j] = hosts[j];
            n_host[j] = textBox4.Text.Trim() + " " + textBox5.Text.Trim() + "\r\n";
            this.dataGridView1.FirstDisplayedScrollingRowIndex = this.dataGridView1.Rows.Count - 1;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string s_host = "";
            for (int i = 0; i < dataGridView1.Rows.Count; i++)
            {
                s_host += dataGridView1.Rows[i].Cells[0].Value.ToString() + " ";
                s_host += dataGridView1.Rows[i].Cells[1].Value.ToString() + "\r\n";
            }
            File.WriteAllText(this.configPath, s_host);
            this.hosts = s_host.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
