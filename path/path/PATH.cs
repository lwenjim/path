using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.IO;
using System.Collections;
namespace path
{
    public partial class PATH : Form
    {
        public int LastSelectedIndex = -1;
        public string[] paths;
        string varItem = "用户环境变量";
        string varItem2 = "系统环境变量";

        public PATH()
        {
            InitializeComponent();
            pathsListBindData(getPaths());
            tabControl1.SelectedIndex = 2;

            comboBox1.Items.Add(varItem);
            comboBox1.Items.Add(varItem2);
            comboBox2.Items.Add(varItem);
            comboBox2.Items.Add(varItem2);
        }

        private void bindEnvironmentVariables(EnvironmentVariableTarget varType)
        {
            dataGridView1.Rows.Clear();
            foreach (DictionaryEntry obj in Environment.GetEnvironmentVariables(varType))
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells[0].Value = obj.Key.ToString();
                dataGridView1.Rows[index].Cells[1].Value = obj.Value.ToString();
            }
        }

        private void pathsListBindData(string[] p_paths)
        {
            listBox1.Items.Clear();
            foreach (string path in p_paths)
            {
                listBox1.Items.Add(path);
            }
            paths = p_paths;
        }

        private static string[] getPaths()
        {
            string[] paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine)
                .Replace("%SystemRoot%", "C:\\Windows").Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return paths;
        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            List<string> l_paths = new List<string>();
            string keyWord = textBox2.Text.Trim();
            for (int i = 0; i < paths.Length; i++)
            {
                if (paths[i].ToLower().IndexOf(keyWord.ToLower()) == -1) continue;
                l_paths.Add(paths[i]);
            }
            string[] s_paths = new string[l_paths.Count];
            for (int i = 0; i < l_paths.Count; i++)
            {
                s_paths[i] = l_paths[i];
            }
            pathsListBindData(s_paths);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim().Length == 0) return;
            for (int i = 0; i < listBox1.Items.Count; i++)
                if (listBox1.Items[i].ToString().Trim().Equals(textBox3.Text.Trim().ToString())) return;
            listBox1.Items.Add(textBox3.Text.Trim());
            this.listBox1.TopIndex = this.listBox1.Items.Count - (int)(this.listBox1.Height / this.listBox1.ItemHeight);
            List<string> newPaths = paths.ToList<string>();
            newPaths.Add(textBox3.Text.Trim());
            paths = newPaths.ToArray();
            updateVariablePath();
            savePaths();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.LastSelectedIndex == -1) return;
            listBox1.Items[this.LastSelectedIndex] = textBox1.Text.Trim();
            paths[LastSelectedIndex] = textBox1.Text.Trim();
            updateVariablePath();
            savePaths();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            moveUp();
        }

        private void moveUp()
        {
            if (isEmptySelected()) return;
            moreSelected2One();
            int lastIndex = listBox1.SelectedIndex;
            if (lastIndex == 0) return;
            string tmp = paths[lastIndex - 1];
            paths[lastIndex - 1] = paths[lastIndex];
            paths[lastIndex] = tmp;
            pathsListBindData(paths);
            listBox1.SetSelected(lastIndex - 1, true);
            listBox1.SetSelected(lastIndex, false);

            updateVariablePath();
            savePaths();

        }

        private void updateVariablePath()
        {
            string str = string.Join(";", paths);
            str = str.Replace("C:\\Windows", "%SystemRoot%");
            Environment.SetEnvironmentVariable("path", str, EnvironmentVariableTarget.Machine);
            Environment.SetEnvironmentVariable("path", str, EnvironmentVariableTarget.Process);
        }

        private void savePaths()
        {
            string str = string.Join(";", paths);
            File.AppendAllText("C:\\Program Files\\a.log", "【" + DateTime.Now.ToString() + "】" + str + "\r\n");
        }

        private void moreSelected2One()
        {
            if (listBox1.SelectedIndices.Count > 1)
            {
                int count = listBox1.SelectedIndices.Count; this.Text = count.ToString();
                int index = listBox1.SelectedIndices[count - 1];
                listBox1.ClearSelected();
                listBox1.SetSelected(index, true);
            }
        }

        private bool isEmptySelected()
        {
            return listBox1.SelectedIndices.Count <= 0;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            moveDown();
        }

        private void moveDown()
        {
            if (isEmptySelected()) return;
            moreSelected2One();
            int lastIndex = listBox1.SelectedIndex;
            if (lastIndex >= listBox1.Items.Count - 1) return;
            string tmp = paths[lastIndex + 1];
            paths[lastIndex + 1] = paths[lastIndex];
            paths[lastIndex] = tmp;
            pathsListBindData(paths);
            listBox1.SetSelected(lastIndex + 1, true);
            listBox1.SetSelected(lastIndex, false);
            updateVariablePath();
            savePaths();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string str = "";
            for (int i = 0; i < listBox1.SelectedItems.Count; i++)
            {
                str += listBox1.SelectedItems[i].ToString() + "\r\n";
                if (i == listBox1.SelectedItems.Count - 1)
                {
                    this.LastSelectedIndex = listBox1.SelectedIndices[i];
                    textBox1.Text = listBox1.SelectedItems[i].ToString();
                }
            }
            Clipboard.SetDataObject(str);
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2) editEnvironmentVariable(e);
            else if (e.ColumnIndex == 3) 
            {
                switch (comboBox2.SelectedIndex)
                {
                    case 0:
                        Environment.SetEnvironmentVariable(dataGridView1[0, e.RowIndex].Value.ToString(), "", EnvironmentVariableTarget.User);
                        bindEnvironmentVariables(EnvironmentVariableTarget.User);
                        break;
                    case 1:
                        Environment.SetEnvironmentVariable(dataGridView1[0, e.RowIndex].Value.ToString(), "", EnvironmentVariableTarget.Machine);
                        bindEnvironmentVariables(EnvironmentVariableTarget.Machine);
                        break;
                }
            }
        }

        private void editEnvironmentVariable(DataGridViewCellEventArgs e)
        {
            comboBox1.SelectedIndex = comboBox2.SelectedIndex;
            comboBox1.Enabled = false;
            tabControl1.SelectedIndex = 2;
            tabControl1.TabPages[2].Text = "编辑环境变量";
            button5.Text = "更新环境变量";
            textBox4.Text = dataGridView1[0, e.RowIndex].Value.ToString();
            textBox5.Text = dataGridView1[1, e.RowIndex].Value.ToString();
            label4.Text = textBox4.Text;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (textBox4.Text.Trim().Length == 0 || textBox5.Text.Trim().Length == 0) return;
            if (comboBox1.SelectedIndex == -1) return;
            if (label4.Text.Trim() != "empty") updateEnvironmentVariable();
            else insertEnvironmentVariable();
        }

        private void insertEnvironmentVariable()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    Environment.SetEnvironmentVariable(textBox4.Text.Trim(), textBox5.Text.Trim(), EnvironmentVariableTarget.User);
                    bindEnvironmentVariables(EnvironmentVariableTarget.User);
                    break;
                case 1:
                    Environment.SetEnvironmentVariable(textBox4.Text.Trim(), textBox5.Text.Trim(), EnvironmentVariableTarget.Machine);
                    bindEnvironmentVariables(EnvironmentVariableTarget.Machine);
                    break;
            }
            tabControl1.SelectedIndex = 1;
            textBox4.Text = "";
            textBox5.Text = "";
            comboBox2.SelectedIndex = comboBox1.SelectedIndex;
        }

        private void updateEnvironmentVariable()
        {
            switch (comboBox1.SelectedIndex)
            {
                case 0:
                    if (label4.Text.Trim() != textBox4.Text.Trim()) 
                    {
                        Environment.SetEnvironmentVariable(label4.Text.Trim(), "", EnvironmentVariableTarget.User);    
                    }
                    Environment.SetEnvironmentVariable(textBox4.Text.Trim(), textBox5.Text.Trim(), EnvironmentVariableTarget.User);
                    bindEnvironmentVariables(EnvironmentVariableTarget.User);
                    break;
                case 1:
                    if (label4.Text.Trim() != textBox4.Text.Trim())
                    {
                        Environment.SetEnvironmentVariable(label4.Text.Trim(), "", EnvironmentVariableTarget.Machine);
                    }
                    Environment.SetEnvironmentVariable(textBox4.Text.Trim(), textBox5.Text.Trim(), EnvironmentVariableTarget.Machine);
                    bindEnvironmentVariables(EnvironmentVariableTarget.Machine);
                    break;
            }

            comboBox1.Enabled = true;
            tabControl1.SelectedIndex = 1;
            tabControl1.TabPages[2].Text = "新增环境变量";
            button5.Text = "新增环境变量";
            textBox4.Text = "";
            textBox5.Text = "";
            label4.Text = "empty";
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == -1) return;
            switch (comboBox2.SelectedIndex)
            {
                case 0: bindEnvironmentVariables(EnvironmentVariableTarget.User); break;
                case 1: bindEnvironmentVariables(EnvironmentVariableTarget.Machine); break;
            }
        }
    }
}
