using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using System.Collections.Generic;
using TUIO;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
public class TuioDemo : Form, TuioListener
{
    private TuioClient client;

    private Dictionary<long, TuioObject> objectList;
    private Dictionary<long, TuioCursor> cursorList;
    private Dictionary<long, TuioBlob> blobList;

    public static int width, height;

    private int window_width = 640;
    private int window_height = 480;


    bool pageOpened = false;


    Image apple;
    Image banana;
    Image background;
    Image back2;

    Rectangle loginButton;
    Rectangle signupButton;

    Font titleFont = new Font("Comic Sans MS", 36, FontStyle.Bold);
    Font buttonFont = new Font("Arial", 14, FontStyle.Bold);
    TcpListener server;
    Thread socketThread;

    public TuioDemo(int port)
    {
        width = window_width;
        height = window_height;

        this.ClientSize = new Size(width, height);
        this.Text = "Learning Kids";

        this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                      ControlStyles.UserPaint |
                      ControlStyles.DoubleBuffer, true);

        objectList = new Dictionary<long, TuioObject>();
        cursorList = new Dictionary<long, TuioCursor>();
        blobList = new Dictionary<long, TuioBlob>();

        client = new TuioClient(port);
        client.addTuioListener(this);
        client.connect();

        loginButton = new Rectangle(width / 2 - 150, height / 2, 120, 50);
        signupButton = new Rectangle(width / 2 + 30, height / 2, 120, 50);

        apple = Image.FromFile("apple.png");
        banana = Image.FromFile("banana.png");
        background = Image.FromFile("Learning.png");
        back2 = Image.FromFile("page1.png");
        this.KeyDown += new KeyEventHandler(Form_KeyDown);
        this.Closing += new CancelEventHandler(Form_Closing);
        //socket server 
        server = new TcpListener(IPAddress.Any, 3333);//hena 3l4an n check el sotckets b listen lazem el sotcket yb2a nfs elport bta3 el client 3l4an y3ml connection 
        server.Start();
        socketThread = new Thread(ListenForClient);
        socketThread.IsBackground = true;
        socketThread.Start();
    }


    void ListenForClient()
    {
        while (true)
        {
            TcpClient clientSocket = server.AcceptTcpClient();
            NetworkStream stream = clientSocket.GetStream();

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);

            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);

            if (message.Contains("OPEN_PAGE1") && !pageOpened)
            {
                pageOpened = true;

                this.Invoke((MethodInvoker)delegate
                {
                    Openpage1();
                });
            }

            clientSocket.Close();
        }
    }


    private void Form_KeyDown(object sender, KeyEventArgs e)
    {
        if (e.KeyData == Keys.Escape)
            this.Close();
    }

    private void Form_Closing(object sender, CancelEventArgs e)
    {
        client.removeTuioListener(this);
        client.disconnect();
        Environment.Exit(0);
    }

    public void addTuioObject(TuioObject o)
    {
        lock (objectList)
        {
            objectList[o.SessionID] = o;
        }
    }

    public void updateTuioObject(TuioObject o) { }

    public void removeTuioObject(TuioObject o)
    {
        lock (objectList)
        {
            objectList.Remove(o.SessionID);
        }
    }

    public void addTuioCursor(TuioCursor c)
    {
        lock (cursorList)
        {
            cursorList[c.SessionID] = c;
        }
    }

    public void updateTuioCursor(TuioCursor c) { }

    public void removeTuioCursor(TuioCursor c)
    {
        lock (cursorList)
        {
            cursorList.Remove(c.SessionID);
        }
    }

    public void addTuioBlob(TuioBlob b)
    {
        lock (blobList)
        {
            blobList[b.SessionID] = b;
        }
    }

    public void updateTuioBlob(TuioBlob b) { }

    public void removeTuioBlob(TuioBlob b)
    {
        lock (blobList)
        {
            blobList.Remove(b.SessionID);
        }
    }

    public void refresh(TuioTime frameTime)
    {
        lock (objectList)
        {
            foreach (TuioObject obj in objectList.Values)
            {
                int x = obj.getScreenX(width);
                int y = obj.getScreenY(height);

                if (obj.SymbolID == 3 && loginButton.Contains(x, y) && !pageOpened)
                {
                    pageOpened = true;

                    this.Invoke((MethodInvoker)delegate
                    {
                        Openpage1();
                    });
                }
            }
        }

        Invalidate();
    }
    protected override void OnPaintBackground(PaintEventArgs pevent)
    {
        Graphics g = pevent.Graphics;

        g.Clear(Color.FromArgb(230, 195, 156));

        g.DrawImage(background, 0, 0, this.ClientSize.Width, this.ClientSize.Height);
        DrawButtons(g);
    }
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);

        int w = this.ClientSize.Width;
        int h = this.ClientSize.Height;

        loginButton = new Rectangle(w / 2 - 150, h / 2, 120, 50);
        signupButton = new Rectangle(w / 2 + 30, h / 2, 120, 50);

        Invalidate(); // redraw UI
    }


    void DrawButtons(Graphics g)
    {
        Brush loginBrush = new SolidBrush(Color.FromArgb(80, 170, 120));
        Brush signupBrush = new SolidBrush(Color.FromArgb(245, 190, 60));

        g.FillRectangle(loginBrush, loginButton);
        g.FillRectangle(signupBrush, signupButton);

        g.DrawString("Student", buttonFont, Brushes.White,
            loginButton.X + 20, loginButton.Y + 12);


        g.DrawString("Teacher", buttonFont, Brushes.White,
            signupButton.X + 20, signupButton.Y + 12);
    }



    void Openpage1()
    {
        page1 page = new page1(client);
        page.Show();
        this.Hide();
    }

    public static void Main(string[] argv)
    {
        int port = 3333;

        if (argv.Length == 1)
            port = int.Parse(argv[0]);

        TuioDemo app = new TuioDemo(port);
        Application.Run(app);
    }

    //  bool BluetoothLogin()
    //{
    //  ProcessStartInfo start = new ProcessStartInfo();
    //start.FileName = "python";
    //start.Arguments = "bluetooth_login.py";
    //start.UseShellExecute = false;
    //start.RedirectStandardOutput = true;

    //Process process = Process.Start(start);
    //string result = process.StandardOutput.ReadToEnd();
    //process.WaitForExit();

    //if (result.Contains("Student"))
    //  return true;

    //return false;
    //}
    //}
}