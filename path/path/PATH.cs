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
namespace path
{
    public partial class PATH : Form
    {
        public int LastSelectedIndex = -1;
        public string[] paths;
        public PATH()
        {
            InitializeComponent();
            pathsListBindData(getPaths());
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
            string[] paths = Environment.GetEnvironmentVariable("path", EnvironmentVariableTarget.Machine).Replace("%SystemRoot%", "C:\\Windows").Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            return paths;
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
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
    }
}
