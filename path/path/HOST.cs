using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Data.SQLite;
using System.Text.RegularExpressions;
namespace path
{
    public partial class HOST : Form
    {
        private string configPath = "C:\\Windows\\System32\\drivers\\etc\\hosts";
        HostDomainDao hddao;
        HostTypeDao htdao;
        List<HostDomain> hdlist;
        List<HostType> htlist;
        HostType ht;
        public HOST()
        {
            InitializeComponent();
            hddao = new HostDomainDao();
            htdao = new HostTypeDao();
            bandComb();
            tabControl1.SelectedIndex = 0;
            reflushTableEnvirment();
            
        }

        private void reflushTableEnvirment()
        {
            dataGridView2.Rows.Clear();
            htlist = htdao.fetchAll("select id,name from tbl_host_type");
            foreach (HostType ht in htlist)
            {
                int index = dataGridView2.Rows.Add();
                dataGridView2.Rows[index].Cells["tid"].Value = ht.Id;
                dataGridView2.Rows[index].Cells["name"].Value = ht.Name;
                dataGridView2.Rows[index].Cells["caozuo"].Value = "编辑";
            }
        }

        private void bandComb()
        {
            htlist = htdao.fetchAll("select id,name from tbl_host_type");
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();
            foreach (HostType ht in htlist)
            {
                comboBox1.Items.Add(HostTypeDao.formatItemByItem(ht));
                comboBox2.Items.Add(HostTypeDao.formatItemByItem(ht));
            }
        }

        private void reflushTableHostDomain(int type)
        {
            dataGridView1.Rows.Clear();
            hdlist = hddao.fetchAll("select id,ip,domain,type from tbl_host_domain where type=" + type);
            foreach (HostDomain host in hdlist)
            {
                int index = dataGridView1.Rows.Add();
                dataGridView1.Rows[index].Cells["id"].Value = host.Id;
                dataGridView1.Rows[index].Cells["ip"].Value = host.Ip;
                dataGridView1.Rows[index].Cells["domain"].Value = host.Domain;

                ht = htdao.fetch("select id,name from tbl_host_type where id=" + host.Type);
                dataGridView1.Rows[index].Cells["type"].Value = HostTypeDao.formatItemByItem(ht);
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text.Trim().Length == 0 || textBox2.Text.Trim().Length == 0 || comboBox2.SelectedIndex == -1) return;
            if (!HostDomainDao.checkIp(textBox1.Text.Trim()))
            {
                MessageBox.Show("【ip地址】格式有误！");
                return;
            }
            if (!HostDomainDao.checkDomain(textBox2.Text.Trim()))
            {
                MessageBox.Show("【域名】格式有误！");
                return;
            }
            if (Regex.IsMatch(label6.Text.Trim(), @"^\d+$")) updateTableHostDomain();
            else if (label5.Text.Trim() == "empty") insertTableHostDomain();


        }

        private void updateTableHostDomain()
        {
            string sql = "update tbl_host_domain set ip='{0}',domain='{1}',type={2} where id={3}";
            int type = HostTypeDao.getIdByStr(comboBox2.SelectedItem.ToString());
            sql = String.Format(sql, textBox1.Text.Trim(), textBox2.Text.Trim(), type.ToString(), label6.Text.Trim());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("添加失败！");
                    return;
                }
                label6.Text = "empty";
                textBox1.Text = "";
                textBox2.Text = "";
                comboBox2.SelectedIndex = -1;
                tabControl1.SelectedIndex = 0;
                button1.Text = "新增";
                tabControl1.TabPages[1].Text = "新增绑定";
                reflushTableHostDomain(type);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void insertTableHostDomain()
        {
            string sql = "insert into tbl_host_domain(id,ip,domain,type) values(null,'{0}','{1}',{2})";
            int type = HostTypeDao.getIdByStr(comboBox2.SelectedItem.ToString());
            sql = String.Format(sql, textBox1.Text.Trim(), textBox2.Text.Trim(), type.ToString());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("添加失败！");
                    return;
                }
                if (comboBox2.SelectedIndex != -1 && comboBox1.SelectedIndex != -1 && comboBox1.SelectedItem.ToString().Trim() == comboBox2.SelectedItem.ToString().Trim())
                {
                    reflushTableHostDomain(type);
                }
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (textBox3.Text.Trim().Length == 0) return;
            if (Regex.IsMatch(label5.Text.Trim(), @"^\d+$")) updateTableEnvirment();
            else if (label5.Text.Trim() == "empty") insertTableEnvirment();
        }

        private void insertTableEnvirment()
        {
            string sql = "insert into tbl_host_type(id,name) values(null,'{0}')";
            sql = String.Format(sql, textBox3.Text.Trim());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("添加失败！");
                }
                bandComb();
                reflushTableEnvirment();
                tabControl1.SelectedIndex = 3;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void updateTableEnvirment()
        {
            string sql = "update tbl_host_type set name='{0}' where id={1}";
            sql = String.Format(sql, textBox3.Text.Trim(), label5.Text.Trim());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("更新失败！");
                    return;
                }
                bandComb();
                reflushTableEnvirment();
                tabControl1.SelectedIndex = 3;
                button2.Text = "新增";
                tabControl1.TabPages[2].Text = "添加环境";
                label5.Text = "empty";
                textBox3.Text = "";
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string str = comboBox1.SelectedItem.ToString();
            string[] list = comboBox1.SelectedItem.ToString().Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            int type = Convert.ToInt32(list[0]);
            reflushTableHostDomain(type);
        }

        private void dataGridView2_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 2) editTableEnvirment(e);
            else if (e.ColumnIndex == 3) deleteTableEnvirment(e);
        }

        private void editTableEnvirment(DataGridViewCellEventArgs e)
        {
            label5.Text = dataGridView2[0, e.RowIndex].Value.ToString();
            textBox3.Text = dataGridView2[1, e.RowIndex].Value.ToString();
            tabControl1.SelectedIndex = 2;
            button2.Text = "保存";
            tabControl1.TabPages[2].Text = "修改环境";
            button5.Visible = true;
        }

        private void deleteTableEnvirment(DataGridViewCellEventArgs e)
        {
            string sql = "delete from tbl_host_type where id={0}";
            sql = String.Format(sql, dataGridView2[0, e.RowIndex].Value.ToString());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("删除失败！");
                }
                bandComb();
                reflushTableEnvirment();
                dataGridView2.Rows[0].Selected = false;
                dataGridView2.Rows[e.RowIndex - 1].Selected = true;
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == 4) editTableHostDomain(e);
            else if (e.ColumnIndex == 5) deleteTableHostDomain(e);
        }

        private void editTableHostDomain(DataGridViewCellEventArgs e)
        {
            label6.Text = dataGridView1[0, e.RowIndex].Value.ToString();
            textBox1.Text = dataGridView1[1, e.RowIndex].Value.ToString();
            textBox2.Text = dataGridView1[2, e.RowIndex].Value.ToString();
            for (int i = 0; i < comboBox2.Items.Count; i++)
            {
                if (comboBox2.Items[i].ToString() == dataGridView1[3, e.RowIndex].Value.ToString())
                {
                    comboBox2.SelectedIndex = i;
                    break;
                }
            }
            tabControl1.SelectedIndex = 1;
            button1.Text = "保存";
            button4.Visible = true;
            tabControl1.TabPages[1].Text = "修改绑定";
        }

        private void deleteTableHostDomain(DataGridViewCellEventArgs e)
        {
            string sql = "delete from tbl_host_domain where id={0}";
            sql = String.Format(sql, dataGridView1[0, e.RowIndex].Value.ToString());
            try
            {
                if (hddao.ExecuteNonQuery(sql) == 0)
                {
                    MessageBox.Show("删除失败！");
                    return;
                }
                int type = HostTypeDao.getIdByStr(comboBox1.SelectedItem.ToString());
                reflushTableHostDomain(type);
            }
            catch (Exception ex) { MessageBox.Show(ex.Message); }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex > -1)
            {
                string[] list = new string[dataGridView1.Rows.Count];
                for (int i = 0; i < dataGridView1.Rows.Count; i++)
                {
                    string pathStr = "";
                    pathStr += dataGridView1.Rows[i].Cells[1].Value.ToString() + " ";
                    pathStr += dataGridView1.Rows[i].Cells[2].Value.ToString();
                    list[i] = pathStr;
                }
                File.WriteAllLines(this.configPath, list);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            label6.Text = "empty";
            textBox1.Text = "";
            textBox2.Text = "";
            comboBox2.SelectedIndex = -1;
            tabControl1.SelectedIndex = 0;
            button1.Text = "新增";
            tabControl1.TabPages[1].Text = "新增绑定";
            button4.Visible = false;
            tabControl1.SelectedIndex = 0;
        }

        private void button5_Click(object sender, EventArgs e)
        {
            tabControl1.SelectedIndex = 3;
            button2.Text = "新增";
            button5.Visible = false;
            tabControl1.TabPages[2].Text = "添加环境";
            label5.Text = "empty";
            textBox3.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedIndex == -1) return;
            DialogResult dr = MessageBox.Show("点击【是】加载本地host文件，【否】打开文件对话框", "请选择导入的文件路径！",
                MessageBoxButtons.YesNo, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1,
                MessageBoxOptions.DefaultDesktopOnly);

            if (DialogResult.Yes == dr) 
            {
                int j = importHostDomain(configPath);
                MessageBox.Show(String.Format("导入成功{0}个！", j.ToString()));
            }
            else if (DialogResult.No == dr)
            {
                OpenFileDialog ofd = new OpenFileDialog();
                DialogResult drr = ofd.ShowDialog();
                if (drr != DialogResult.OK) return;
                int j = importHostDomain(ofd.FileName);
                MessageBox.Show(String.Format("导入成功{0}个！", j.ToString()));
            }
        }

        private int importHostDomain(string filename)
        {
            int j = 0;
            int type = HostTypeDao.getIdByStr(comboBox1.SelectedItem.ToString());
            string[] lines = File.ReadAllLines(filename, Encoding.Default);
            foreach (string line in lines)
            {
                string[] fields = line.Trim().Split(new char[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
                if (!HostDomainDao.checkIp(fields[0]) || !HostDomainDao.checkDomain(fields[1])) continue;
                try
                {
                    int i = hddao.add(new HostDomain() { Ip = fields[0].Trim(), Domain = fields[1].Trim(), Type = type });
                    if (i > 0) j++;
                }
                catch (Exception ex) { string s = ex.Message; }
            }
            reflushTableHostDomain(type);
            return j;
        }

    }
    class HostDomain
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string ip;

        public string Ip
        {
            get { return ip; }
            set { ip = value; }
        }
        private string domain;

        public string Domain
        {
            get { return domain; }
            set { domain = value; }
        }
        private int type;

        public int Type
        {
            get { return type; }
            set { type = value; }
        }
    }
    class HostType
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
    class HostDomainDao : ModelDao
    {
        public int add(HostDomain hd)
        {
            string sql = "insert into tbl_host_domain(id,ip,domain,type) values(null,'{0}','{1}',{2})";
            sql = String.Format(sql, hd.Ip, hd.Domain, hd.Type.ToString());
            int i = this.ExecuteNonQuery(sql);
            return i;
        }
        public static bool checkIp(string ip)
        {
            return Regex.IsMatch(ip, @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])$");
        }
        public static bool checkDomain(string domain)
        {
            return Regex.IsMatch(domain, @"^[a-zA-Z0-9][-a-zA-Z0-9]{0,62}(\.[a-zA-Z0-9][-a-zA-Z0-9]{0,62})+$", RegexOptions.IgnoreCase);
        }
        public List<HostDomain> fetchAll(string sql)
        {
            List<HostDomain> list = new List<HostDomain>();
            SQLiteDataReader dr = getDataReader(sql);
            while (dr.Read())
            {
                list.Add(new HostDomain()
                {
                    Id = Convert.ToInt32(dr["id"]),
                    Ip = dr["ip"].ToString(),
                    Domain = dr["domain"].ToString(),
                    Type = Convert.ToInt32(dr["type"])
                });
            }
            return list;
        }
        public HostDomain fetch(string sql)
        {
            List<HostDomain> list = fetchAll(sql);
            return list[0];
        }
    }
    class HostTypeDao : ModelDao
    {

        public List<HostType> fetchAll(string sql)
        {
            List<HostType> list = new List<HostType>();
            SQLiteDataReader dr = getDataReader(sql);
            while (dr.Read())
            {
                list.Add(new HostType()
                {
                    Id = Convert.ToInt32(dr["id"]),
                    Name = dr["name"].ToString()
                });
            }
            return list;
        }

        public HostType fetch(string sql)
        {
            List<HostType> list = fetchAll(sql);
            return list[0];
        }
        public static int getIdByStr(string str)
        {
            string[] list = str.Split(new char[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return Convert.ToInt32(list[0]);
        }
        public static string getNameByStr(string str)
        {
            string[] list = str.Split(new char[] { '-' }, 2, StringSplitOptions.RemoveEmptyEntries);
            return list[1];
        }
        public static string formatItemByItem(HostType ht)
        {
            return ht.Id + "-" + ht.Name;
        }
    }
    class ModelDao
    {
        private SQLiteConnection conn;
        private string dbFileName = "Data Source=path.db";
        private SQLiteConnection getConn()
        {
            if (conn == null)
            {
                conn = new SQLiteConnection(dbFileName);
            }
            if (conn.State == ConnectionState.Closed)
            {
                conn.Open();
            }
            return conn;
        }
        protected SQLiteDataReader getDataReader(string sql)
        {
            SQLiteConnection conn = getConn();
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            return cmd.ExecuteReader(CommandBehavior.CloseConnection);
        }
        public int ExecuteNonQuery(string sql)
        {
            SQLiteConnection conn = getConn();
            SQLiteCommand cmd = new SQLiteCommand(sql, conn);
            return cmd.ExecuteNonQuery(CommandBehavior.CloseConnection);
        }
    }
}
