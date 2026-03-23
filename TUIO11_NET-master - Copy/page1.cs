using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using TUIO;

public class page1 : Form, TuioListener
{
    private TuioClient client;
    private Dictionary<long, TuioObject> objectList;

    Image apple;
    Image banana;
    Image dog;
    Image watermelon;
    Image back2;

    bool showApple = false;
    bool showBanana = false;
    bool showWatermelon = false;


    Font font = new Font("Arial", 30, FontStyle.Bold);

    int width = 640;
    int height = 480;

    public page1(TuioClient tclient)
    {
        client = tclient;

        this.ClientSize = new Size(width, height);
        this.Text = "Page 1";

        objectList = new Dictionary<long, TuioObject>();

        apple = Image.FromFile("apple.png");
        banana = Image.FromFile("banana.png");
        dog = Image.FromFile("dog.png");
        watermelon= Image.FromFile("watermelon.png");
       back2 = Image.FromFile("page1.png");
        // Use SafeLoad to avoid throwing if images are missing
        apple = SafeLoad("apple.png", 200, 200);
        banana = SafeLoad("banana.png", 200, 200);
        dog = SafeLoad("dog.png", 120, 120);
        watermelon = SafeLoad("watermelon.png", 120, 120);


        client.addTuioListener(this);

        this.SetStyle(
            ControlStyles.AllPaintingInWmPaint |
            ControlStyles.UserPaint |
            ControlStyles.DoubleBuffer, true
        );
    }

    // Helper: try to load an image file, return a simple placeholder bitmap when missing
    private Image SafeLoad(string path, int w, int h)
    {
        try
        {
            if (System.IO.File.Exists(path))
            {
                return Image.FromFile(path);
            }
        }
        catch { }

        Bitmap bmp = new Bitmap(Math.Max(1, w), Math.Max(1, h));
        using (Graphics g = Graphics.FromImage(bmp))
        {
            g.Clear(Color.LightGray);
            using (Pen p = new Pen(Color.DarkGray, 3))
            {
                g.DrawRectangle(p, 0, 0, bmp.Width - 1, bmp.Height - 1);
                g.DrawLine(p, 0, 0, bmp.Width - 1, bmp.Height - 1);
                g.DrawLine(p, bmp.Width - 1, 0, 0, bmp.Height - 1);
            }
        }
        return bmp;
    }

    public void addTuioObject(TuioObject o)
    {
        lock (objectList)
        {
            objectList[o.SessionID] = o;

            if (o.SymbolID == 5)
            {
                showApple = true;

                showBanana = false;
            }

            if (o.SymbolID == 4)
            {
                showBanana = true;
                showApple = false;
            }
            if (o.SymbolID == 6)
            {
                showBanana = false;
                showApple = false;
                showWatermelon = true;
            }
        }
    }

    public void updateTuioObject(TuioObject o) { }

    public void removeTuioObject(TuioObject o)
    {
        lock (objectList)
        {
            objectList.Remove(o.SessionID);
            showApple = false;
            showBanana = false;
            showWatermelon= false;
        }
    }

    public void addTuioCursor(TuioCursor c) { }
    public void updateTuioCursor(TuioCursor c) { }
    public void removeTuioCursor(TuioCursor c) { }

    public void addTuioBlob(TuioBlob b) { }
    public void updateTuioBlob(TuioBlob b) { }
    public void removeTuioBlob(TuioBlob b) { }

    public void refresh(TuioTime frameTime)
    {
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        Graphics g = e.Graphics;

        if (back2 != null)
        {
            g.DrawImage(back2, 0, 0, width, height);
        }
        else
        {
            g.Clear(Color.Beige);
        }

       // g.DrawImage(dog, 20, height - 150, 120, 120);

       
        //if (showApple)
        //{
        //    g.DrawImage(apple, width / 2 - 100, 100, 200, 200);
        //  //  g.DrawString("APPLE", font, Brushes.Red, width / 2 - 70, 320);
        //}
        //
        //if (showBanana)
        //{
        //   g.DrawImage(banana, width / 2 - 100, 100, 200, 200);
        //    //g.DrawString("BANANA", font, Brushes.Goldenrod, width / 2 - 90, 320);
        //}

        foreach (TuioObject tobj in objectList.Values)
        {
            int ox = tobj.getScreenX(width);
            int oy = tobj.getScreenY(height);
            int size = height / 10;
                
            if (showBanana)
            {
            g.TranslateTransform(ox, oy);
            g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
            g.TranslateTransform(-ox, -oy);
                g.DrawImage(banana, width / 2 - 100, 100, 200, 200);
                g.ResetTransform();
                g.DrawString(tobj.Angle < Math.PI ? "Bannana" : "Monkey eat bannana",
                             font, Brushes.Red, width / 2 - 100, 320);
            }


            if (showApple)
            {
                g.TranslateTransform(ox, oy);
                g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
                g.TranslateTransform(-ox, -oy);

                g.DrawImage(apple, width / 2 - 100, 100, 200, 200);
                g.ResetTransform();//by5aly elly ba3daha sabet la2eny 3amalt reset ll position
                g.DrawString(tobj.Angle < Math.PI ? "APPLE" : "Ahmed drink apple",
                             font, Brushes.Red, width / 2 - 100, 320);
            }


            if (showWatermelon)
            {
                g.TranslateTransform(ox, oy);
                g.RotateTransform((float)(tobj.Angle / Math.PI * 180.0f));
                g.TranslateTransform(-ox, -oy);

                g.DrawImage(watermelon, width / 2 - 100, 100, 200, 200);
                g.ResetTransform();//by5aly elly ba3daha sabet la2eny 3amalt rest ll position
                g.DrawString(tobj.Angle < Math.PI ? "watermelon" : "Watermelon is a big",
                             font, Brushes.Red, width / 2-100 , 320);
            }




        }
        }
    }