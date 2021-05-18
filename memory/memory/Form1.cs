using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace memory
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        int SystemMemory; //初始系统内存大小
        int MemorySize;    //页面大小
        List<area> l = new List<area>();    //内存链表
        area dele = null;   //删除区域
        public class area   //进程空闲区
        {
            public string ID;
            public int beginAdress;
            public int length;
            public Color tag ;  //color 为白色就是空闲区
            public int endAdress;

            public area(string id,int begin, int len, Color t)
            {
                ID = id;
                beginAdress = begin;
                length = len;
                tag = t;
                endAdress = beginAdress + length;
            }
            public void set(int begin ,int len , Color t)
            {
                beginAdress = begin;
                length = len;
                tag = t;
                endAdress = beginAdress + length;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        private void union()     //合并
        {          
            for (int i = 0; i < l.Count -1; i++)
            {
                if(l[i].tag==Color.White)
                {
                    if(l[i+1].tag==Color.White)
                    {
                        l[i].endAdress = l[i + 1].endAdress;
                        l[i].length = l[i].length + l[i + 1].length;
                        l.RemoveAt(i + 1);
                    }
                }
            }
            
        }
        private void insert()//插入
        {
            int len = Convert.ToInt32(textBox1.Text);
            len = len % MemorySize == 0 ? len / MemorySize : len / MemorySize + 1;
            foreach (var item in l)
            {
                if (item.tag == Color.White)
                {
                    if (item.length >= len)
                    {
                        l.Remove(item);

                        area tmp = new area("", item.beginAdress, len, colorDialog1.Color);
                        tmp.ID = textBox2.Text;
                        item.set(tmp.endAdress, item.length - len, Color.White);
                        item.ID = "空闲区";
                        l.Add(tmp);
                        if (item.length != 0)
                        {
                            l.Add(item);
                        }
                        return;
                    }
                }
            }
            MessageBox.Show("内存不足，请压缩内存或清理内存！");
        }

        private void delete()   //删除
        {
            foreach (var item in l)
            {
                if(item==dele)
                {
                    item.tag = Color.White;
                    item.ID ="空闲区";
                }
            }
        }

        private void compress()     //内存压缩
        {
            List<area> tmp = new List<area>();

            int shang = 0; //上一个非空尾地址
            int len = 0;
            foreach (var item in l)
            {
                if (item.tag == Color.White)
                {
                    len += item.length;
                }
                else
                {
                    item.beginAdress = shang;
                    item.endAdress = shang + item.length;
                    shang = item.endAdress;
                    tmp.Add(item);
                }
            }
            l = tmp;
            if (len != 0)
            {
                area kong = new area("空闲区", shang, len, Color.White);
                l.Add(kong);
            }
            paint();
        }
        private void paint()    //绘图
        {
            
            Graphics g = panel1.CreateGraphics();
            panel1.Controls.Clear();
            g.Clear(BackColor);
            panel1.Font = new Font("黑体", 9, panel1.Font.Style | FontStyle.Italic);
            foreach (var item in l)
            {

                Brush brush = new SolidBrush(item.tag);             
                g.FillRectangle(brush, 0, item.beginAdress * 2, 300, item.length * 2);
                g.DrawLine(Pens.Black, 0, item.beginAdress * 2, 300, item.beginAdress * 2);
                g.DrawString(item.beginAdress+"-"+(item.endAdress-1)+item.ID, Font, Brushes.Black, 100, item.beginAdress + item.endAdress);
            }
        }


        private void button2_Click(object sender, EventArgs e)  //插入按钮
        {
            

            insert();
            paint();
        }

        private void button3_Click(object sender, EventArgs e)  //选择颜色按钮
        {

            colorDialog1.AllowFullOpen = false;
            //提供自己给定的颜色
            colorDialog1.CustomColors = new int[] { 6916092, 15195440, 16107657, 1836924, 3758726, 12566463, 7526079, 7405793, 6945974, 241502, 2296476, 5130294, 3102017, 7324121, 14993507, 11730944 };
            colorDialog1.ShowHelp = true;
            colorDialog1.ShowDialog();
            button3.BackColor = colorDialog1.Color;
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)  //选择区域
        {
            foreach (var item in l)
            {
                if(e.X<=300&&e.X>=0&&e.Y<=item.endAdress*2&&e.Y>=item.beginAdress*2)
                {
                    dele = item;
                }
            }
        }

        private void 删除ToolStripMenuItem_Click(object sender, EventArgs e) //右键删除
        {
            if(dele!=null)
            {
                delete();
                union();
                union();
                paint();
            }
        }

        private void button4_Click(object sender, EventArgs e)  //内存压缩按钮
        {
            compress();
        }

        private void button1_Click(object sender, EventArgs e)  //显示内存按钮
        {
            
            l.Clear();
            Graphics g = panel1.CreateGraphics();
            SystemMemory = int.Parse(textBox3.Text.ToString());
            MemorySize = int.Parse(textBox4.Text.ToString());
            SystemMemory /= MemorySize;
            area tmp = new area("空闲区", 0, SystemMemory, Color.White);
            l.Add(tmp);
            paint();
        }
    }
}
