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
            paths = getPaths();
            pathsListBindData(paths);
        }

        private void pathsListBindData(string[] p_paths)
        {
            listBox1.Items.Clear();
            foreach (string path in p_paths)
            {
                listBox1.Items.Add(path);
            }
        }

        private static string[] getPaths()
        {
            string[] paths = Environment.GetEnvironmentVariable("path").Split(new Char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (this.LastSelectedIndex == -1) return;
            listBox1.Items[this.LastSelectedIndex] = textBox1.Text.Trim();
        }
    }
}
