using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using System.Diagnostics;

namespace ST_Source_Fusker
{
    public class ScreenAvaialableEventArgs : EventArgs
    {
        private readonly Image screen;
        private readonly int _id;

        public ScreenAvaialableEventArgs(Image Screen, int id)
        {
            this._id = id;
            this.screen = Screen;
        }

        public Image Screen
        {
            get { return screen; }
        }
        public int ID
        {
            get { return _id; }
        }
    }
    public delegate void ScreenAvaiableEventHandler(object sender, ScreenAvaialableEventArgs e);

    public class StatusChangeEventArgs : EventArgs
    {
        private readonly string status;
        private readonly int _id;

        public StatusChangeEventArgs(string Status, int id)
        {
            status = Status;
            _id = id;
        }

        public string Status
        {
            get { return status; }
        }

        public int ID
        {
            get { return _id; }
        }
    }
    public delegate void StatusChangeEventHandler(object sender, StatusChangeEventArgs e);

    public class DataReadyEventArgs : EventArgs
    {
        private readonly string[] links;
        private readonly int _id;

        public DataReadyEventArgs(string[] Links, int id)
        {
            links = Links;
            _id = id;
        }

        public string[] Links
        {
            get { return links; }
        }
        public int ID
        {
            get { return _id; }
        }
    }
    public delegate void DataReadyEventHandler(object sender, DataReadyEventArgs e);


    public class ReadyEventArgs : EventArgs
    {
        private readonly int _id;
        public ReadyEventArgs(int id)
        {
            _id = id;
        }
        public int ID
        {
            get { return _id; }
        }
    }
    public delegate void ReadyEventHandler(object sender, ReadyEventArgs e);

    class Worker
    {
        public bool pause = false;
        public int _id;
        public string _root;

        public event ScreenAvaiableEventHandler ScreenAvailable;
        public event StatusChangeEventHandler StatusChage;
        public event DataReadyEventHandler DataReady;
        public event ReadyEventHandler Ready;


        protected virtual void OnScreenAvailable(ScreenAvaialableEventArgs e)
        {
            if (ScreenAvailable != null)
                ScreenAvailable(this, e);
        }

        protected virtual void OnStatusChage(StatusChangeEventArgs e)
        {
            if (StatusChage != null)
                StatusChage(this, e);
        }

        protected virtual void OnDataReady(DataReadyEventArgs e)
        {
            if (DataReady != null)
                DataReady(this, e);
        }

        protected virtual void OnReady(ReadyEventArgs e)
        {
            if (Ready != null)
                Ready(this, e);
        }

        string outputdir = "";


        public Worker(int id, string root, string outdir)
        {
            _id = id;
            _root = root;
            outputdir = outdir;
            doPause();
        }

        public void changeSettings(bool savescreens, bool savesource, bool saveImg, string outdir)
        {
            outputdir = outdir;
        }

        private bool working = false;
        public void doWork(string filename)
        {
            if (!working)
            {
                working = true;
                OnStatusChage(new StatusChangeEventArgs("Loading " + filename, _id));
                using (WebClient wc = new WebClient())
                {
                    try
                    {
                        byte[] data = wc.DownloadData(_root + filename);
                        if (data.Length > 0)
                        {
                            OnScreenAvailable(new ScreenAvaialableEventArgs((Bitmap)(new ImageConverter()).ConvertFrom(data), _id));
                            
                            OnStatusChage(new StatusChangeEventArgs("Saving" + filename, _id));
                            File.WriteAllBytes(outputdir + filename.Replace('/', '_'), data);
                            working = false;
                            OnReady(new ReadyEventArgs(_id));
                        }
                    }
                    catch (ArgumentException x)
                    {
                        Debug.WriteLine(filename + " Not Found or Not an Image");
                        working = false;
                        OnStatusChage(new StatusChangeEventArgs("Not Found", _id));
                        OnReady(new ReadyEventArgs(_id));
                    }
                    catch (Exception x)
                    {
                        Console.WriteLine(x.Message);
                        working = false;
                        OnStatusChage(new StatusChangeEventArgs("Not Found", _id));
                        OnReady(new ReadyEventArgs(_id));
                    }
                }
            }
            return;
        }

        public async void doPause()
        {
            await Task.Delay(50).ContinueWith(e =>
            {
                OnReady(new ReadyEventArgs(_id));
            });

        }

        public void resume()
        {
            pause = false;
            doPause();
        }

        public void hold()
        {
            pause = true;
        }

        public void Dispose()
        {
            //Cef.Shutdown();
        }
        
    }
}
