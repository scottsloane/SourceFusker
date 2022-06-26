using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ST_Source_Fusker
{
    public partial class Form1 : Form
    {
        Worker worker1;
        Worker worker2;
        Worker worker3;
        Worker worker4;

        BlockingCollection<string> LinkList = new BlockingCollection<string>();

        public Form1()
        {
            InitializeComponent();
        }

        char[] charlist = new char[] { 'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'r', 'x', 'y', 'z', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9'  };

        public string[] recurse(string pre, int item, int max)
        {
            if(item < max)
            {
                List<string> items = new List<string>();
                for(int i = 0; i < charlist.Length; i++)
                {
                    items.AddRange(recurse(pre + charlist[i], item + 1, max));
                }
                return items.ToArray();
            }else
            {
                return new string[] { pre };
            }
        }

        private void startBtn_Click(object sender, EventArgs e)
        {

            //for (int d = 3; d < charlist.Length; d++)
            //{
            //for (int a = 0; a < charlist.Length; a++)
            //{
            //    for (int b = 0; b < charlist.Length; b++)
            //    {
            //        for (int c = 0; c < charlist.Length; c++)
            //        {
            //            LinkList.Add(prefixTbx.Text + charlist[a] + charlist[b] + charlist[c] + ".jpg");
            //        }
            //    }
            //}
            //}

            string[] results = recurse("", 0, (int) variablesNum.Value);
            for(int i = 0; i < results.Length; i++)
            {
                LinkList.Add(results[i]);
            }

            string outputdir = outputTbx.Text ?? Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\demo\";
            string url = rootTbx.Text;

            worker1 = new Worker(1, url, outputdir);
            worker1.ScreenAvailable += Worker_ScreenAvailable;
            worker1.StatusChage += Worker1_StatusChage;
            worker1.Ready += Worker_Ready;

            worker2 = new Worker(2, url, outputdir);
            worker2.ScreenAvailable += Worker_ScreenAvailable;
            worker2.StatusChage += Worker1_StatusChage;
            worker2.Ready += Worker_Ready;

            worker3 = new Worker(3, url, outputdir);
            worker3.ScreenAvailable += Worker_ScreenAvailable;
            worker3.StatusChage += Worker1_StatusChage;
            worker3.Ready += Worker_Ready;

            worker4 = new Worker(4, url, outputdir);
            worker4.ScreenAvailable += Worker_ScreenAvailable;
            worker4.StatusChage += Worker1_StatusChage;
            worker4.Ready += Worker_Ready;

            worker1.resume();
            worker2.resume();
            worker3.resume();
            worker4.resume();
        }



        bool starting = false;
        private void Worker_Ready(object sender, ReadyEventArgs e)
        {
            string currentLink;
            //The worker is ready
            if (starting)
            {
                starting = false;
                if (LinkList.Count > 1)
                {
                    if (LinkList.TryTake(out currentLink)) worker2.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                    else worker2.doPause();
                }
                if (LinkList.Count > 1)
                {
                    if (LinkList.TryTake(out currentLink)) worker3.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                    else worker3.doPause();
                }
                if (LinkList.Count > 1)
                {
                    if (LinkList.TryTake(out currentLink)) worker4.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                    else worker4.doPause();
                }
            }
            if (LinkList.Count > 1)
            {
                switch (e.ID)
                {
                    case 1:
                        if (LinkList.TryTake(out currentLink)) worker1.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                        else worker1.doPause();
                        break;
                    case 2:
                        if (LinkList.TryTake(out currentLink)) worker2.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                        else worker2.doPause();
                        break;
                    case 3:
                        if (LinkList.TryTake(out currentLink)) worker3.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                        else worker3.doPause();
                        break;
                    case 4:
                        if (LinkList.TryTake(out currentLink)) worker4.doWork(prefixTbx.Text + currentLink + postfixTbx.Text);
                        else worker4.doPause();
                        break;
                }
            }

        }

        private void Worker1_StatusChage(object sender, StatusChangeEventArgs e)
        {
            switch (e.ID)
            {
                case 1: setStatus(label1, e.Status); updateStauts(th1OutTbx, e.Status); break;
                case 2: setStatus(label2, e.Status); updateStauts(th2OutTbx, e.Status); break;
                case 3: setStatus(label3, e.Status); updateStauts(th3OutTbx, e.Status); break;
                case 4: setStatus(label4, e.Status); updateStauts(th4OutTbx, e.Status); break;

            }

            //Debug.WriteLine(e.ID + " - " + e.Status);
        }

        private void setStatus(Label lbl, string status)
        {
            if (lbl.InvokeRequired)
                lbl.Invoke(new MethodInvoker(() => setStatus(lbl, status)));
            else
                lbl.Text = status;
        }

        private void updateStauts(TextBox tbx, string status)
        {
            if (tbx.InvokeRequired)
                tbx.Invoke(new MethodInvoker(() => updateStauts(tbx, status)));
            else
            {
                tbx.Text += Environment.NewLine + status;
                tbx.SelectionStart = tbx.Text.Length - 1;
                tbx.ScrollToCaret();
            }
        }

        private void Worker_ScreenAvailable(object sender, ScreenAvaialableEventArgs e)
        {
            switch (e.ID)
            {
                case 1:
                    var oldImg1 = pictureBox1.Image;
                    pictureBox1.Image = e.Screen;
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    if (oldImg1 != null) 
                        oldImg1.Dispose();
                    break;
                case 2:
                    var oldImg2 = pictureBox2.Image;
                    pictureBox2.Image = e.Screen;
                    pictureBox2.SizeMode = PictureBoxSizeMode.StretchImage;
                    if(oldImg2 != null) 
                        oldImg2.Dispose();
                    break;
                case 3:
                    var oldImg3 = pictureBox3.Image;
                    pictureBox3.Image = e.Screen;
                    pictureBox3.SizeMode = PictureBoxSizeMode.StretchImage;
                    if(oldImg3 != null) 
                        oldImg3.Dispose();
                    break;
                case 4:
                    var oldImg4 = pictureBox4.Image;
                    pictureBox4.Image = e.Screen;
                    pictureBox4.SizeMode = PictureBoxSizeMode.StretchImage;
                    if(oldImg4 != null) 
                        oldImg4.Dispose();
                    break;
            }
            //e.Screen.Dispose();
            //throw new NotImplementedException();
            //GC.Collect();
        }
    }
}
