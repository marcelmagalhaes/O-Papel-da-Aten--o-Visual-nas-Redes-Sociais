/*
 * Copyright (c) 2013-present, The Eye Tribe. 
 * All rights reserved.
 *
 * This source code is licensed under the BSD-style license found in the LICENSE file in the root directory of this source tree. 
 *
 */
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Windows7.Multitouch;
using Windows7.Multitouch.WPF;
using TETCSharpClient;
using TETCSharpClient.Data;
using MessageBox = System.Windows.MessageBox;

namespace Scroll
{
    public partial class MainWindow : IGazeListener
    {
        #region Variables

        private const float DPI_DEFAULT = 96f; // default system DIP setting
        private const double SPEED_BOOST = 20.0;
        private const double ACTIVE_SCROLL_AREA = 0.25; // 25% top and bottom
        private const int MAX_IMAGE_WIDTH = 1600;
        private ImageButton latestSelection;
        private readonly double dpiScale;
        private Matrix transfrm;
        private readonly Timer scrollTimer;
        private double scrollLevel;
        private bool canScroll;
        enum Direction { Up = -1, Down = 1 }
        public int count = 0;
        public int cont = 0;
        

        #endregion

        #region Get/Set

        private bool IsTouchEnabled { get; set; }

        #endregion

        #region Enums

        public enum DeviceCap
        {
            /// <summary>
            /// Logical pixels inch in X
            /// </summary>
            LOGPIXELSX = 88,
            /// <summary>
            /// Logical pixels inch in Y
            /// </summary>
            LOGPIXELSY = 90
        }

        #endregion

        #region Constructor

        public MainWindow()
        {
            var connectedOk = true;
            GazeManager.Instance.Activate(GazeManager.ApiVersion.VERSION_1_0, GazeManager.ClientMode.Push);
            GazeManager.Instance.AddGazeListener(this);

            if (!GazeManager.Instance.IsActivated)
            {
                Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("EyeTribe Server has not been started")));
                connectedOk = false;
            }
            /*else if (!GazeManager.Instance.IsCalibrated)
            {
                Dispatcher.BeginInvoke(new Action(() => MessageBox.Show("User is not calibrated")));
                connectedOk = false;
            }*/
            if (!connectedOk)
            {
                Close();
                return;
            }

            InitializeComponent();

            // Check if multi-touch capability is available

        }


        #endregion

        #region Public methods

        public void OnGazeUpdate(GazeData gazeData)
        {
            var x = (int)Math.Round(gazeData.SmoothedCoordinates.X, 0);
            var y = (int)Math.Round(gazeData.SmoothedCoordinates.Y, 0);
            var bitmap = ScreenUtil.Capture(0, 0, 1920, 1080/*, x, y*/);
            string path = "C:\\Users\\Marcel\\Desktop\\teste\\fix" + cont + ".txt";
            if (count % 10 == 0){
                    using ( bitmap )
                    {
                        System.Drawing.Pen blackPen = new System.Drawing.Pen(System.Drawing.Color.Pink, 3);
                        int x2 = 40;
                        int y2 = 40;
                        using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
                    {
                        graphics.DrawRectangle(blackPen, x, y, x2, y2);
                        using (System.Drawing.Brush blackBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Red))
                        {
                            graphics.FillRectangle(blackBrush, x, y, x2, y2);
                        }
                    }
                    bitmap.Save("C:\\Users\\Marcel\\Desktop\\teste\\screen" + cont + ".png");
                    //if (!File.Exists(path))
                    //{
                    string[] lines = { x + " " + y};
                    System.IO.File.WriteAllLines("C:\\Users\\Marcel\\Desktop\\teste\\fix" + cont + ".txt", lines);
                        //File.Create(path);
                        //TextWriter tw = new StreamWriter(path);
                        //tw.WriteLine(x + " " + y);
                        //tw.Close();
                    //}
                    }
                }
            if (count % 10 != 0)
            {
                 if (File.Exists(path))
                        {
                            TextWriter tw = new StreamWriter(path, true);
                            tw.WriteLine(x + " " + y);
                            tw.Close(); 
                        }
                //salva no txt equivalente a cont
            }
            if (count % 10 == 9)
            {
                cont++;
            }

            
            count++;
            if (x == 0 & y == 0) return;
            // Invoke thread
        }

        #endregion



        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            GazeManager.Instance.Deactivate();
            base.OnClosing(e);
        }

        private static double CalcDpiScale()
        {
            return DPI_DEFAULT / GetSystemDpi().X;
        }


        #region Native methods

        public static Point GetSystemDpi()
        {
            Point result = new Point();
            IntPtr hDc = GetDC(IntPtr.Zero);
            result.X = GetDeviceCaps(hDc, (int)DeviceCap.LOGPIXELSX);
            result.Y = GetDeviceCaps(hDc, (int)DeviceCap.LOGPIXELSY);
            ReleaseDC(IntPtr.Zero, hDc);
            return result;
        }

        [DllImport("gdi32.dll")]
        private static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll")]
        private static extern IntPtr GetDC(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern int ReleaseDC(IntPtr hWnd, IntPtr hDc);

        #endregion
    }
}
