// This code is distributed under MIT license. 
// Copyright (c) 2015 George Mamaladze
// See license.txt or http://opensource.org/licenses/mit-license.php

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Drawing.Imaging;
using Gma.System.MouseKeyHook;
using Gma.System.MouseKeyHook.Implementation;

namespace Demo
{
    public partial class Mian : Form
    {
        private IKeyboardMouseEvents m_Events;
        public int i = 0;
        public Mian()
        {
            InitializeComponent();
            radioGlobal.Checked = true;
            SubscribeGlobal();
            FormClosing += Mian_Closing;
        }

        private void Mian_Closing(object sender, CancelEventArgs e)
        {
            Unsubscribe();
        }

        private void SubscribeApplication()
        {
            Unsubscribe();
            Subscribe(Hook.AppEvents());
        }

        private void SubscribeGlobal()
        {
            Unsubscribe();
            Subscribe(Hook.GlobalEvents());
        }

        private void Subscribe(IKeyboardMouseEvents events)
        {
            m_Events = events;
            m_Events.KeyDown += OnKeyDown;
            m_Events.KeyUp += OnKeyUp;
            m_Events.KeyPress += HookManager_KeyPress;

            m_Events.MouseDown += OnMouseDown;
            m_Events.MouseUp += OnMouseUp;
            m_Events.MouseClick += OnMouseClick;
            m_Events.MouseDoubleClick += OnMouseDoubleClick;

            m_Events.MouseMove += HookManager_MouseMove;
            m_Events.MouseWheel += HookManager_MouseWheel;

            m_Events.MouseDownExt += HookManager_Supress;
        }

        private void Unsubscribe()
        {
            if (m_Events == null) return;
            m_Events.KeyDown -= OnKeyDown;
            m_Events.KeyUp -= OnKeyUp;
            m_Events.KeyPress -= HookManager_KeyPress;

            m_Events.MouseDown -= OnMouseDown;
            m_Events.MouseUp -= OnMouseUp;
            m_Events.MouseClick -= OnMouseClick;
            m_Events.MouseDoubleClick -= OnMouseDoubleClick;

            m_Events.MouseMove -= HookManager_MouseMove;
            m_Events.MouseWheel -= HookManager_MouseWheel;

            m_Events.MouseDownExt -= HookManager_Supress;
            m_Events.Dispose();
            m_Events = null;
        }

        private void HookManager_Supress(object sender, MouseEventExtArgs e)
        {
            if (e.Button != MouseButtons.Right)
            {
                return;
            }
            Log("Suppressed.\n");
            e.Handled = true;
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyDown  \t\t {0}\n", e.KeyCode));
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            Log(string.Format("KeyUp  \t\t {0}\n", e.KeyCode));
        }

        private void HookManager_KeyPress(object sender, KeyPressEventArgs e)
        {
            Log(string.Format("KeyPress \t\t {0}\n", e.KeyChar));
        }

        private void HookManager_MouseMove(object sender, MouseEventArgs e)
        {
            labelMousePosition.Text = string.Format("x={0:0000}; y={1:0000}", e.X, e.Y);
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {

            using (var bitmap = ScreenUtil.Capture(0, 0, 1920, 1080, e.X, e.Y))
            {
                Pen blackPen = new Pen(Color.Pink , 3);

            int x2 = 40;
            int y2 = 40;
            // Draw line to screen.
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.DrawRectangle(blackPen, e.X, e.Y, x2, y2);
                using (Brush blackBrush = new SolidBrush(Color.Red))
                {
                    graphics.FillRectangle(blackBrush, e.X, e.Y, x2, y2);
                }
            }
            string nome = DateTime.Now.ToString(" mm ss h");
            bitmap.Save("C:\\Users\\Marcel\\Desktop\\teste\\screen" + nome + ".png");
            }
            i++;
            Log(string.Format("MouseDown \t\t {0}\n", e.Button));
            }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseUp \t\t {0}\n", e.Button));
        }

        private void OnMouseClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseClick \t\t {0}\n", e.Button));
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            Log(string.Format("MouseDoubleClick \t\t {0}\n", e.Button));
        }

        private void HookManager_MouseWheel(object sender, MouseEventArgs e)
        {
            labelWheel.Text = string.Format("Wheel={0:000}", e.Delta);
        }

        private void Log(string text)
        {
            if (IsDisposed) return;
            textBoxLog.AppendText(text);
            textBoxLog.ScrollToCaret();
        }

        private void radioApplication_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Checked) SubscribeApplication();
        }

        private void radioGlobal_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Checked) SubscribeGlobal();
        }

        private void radioNone_CheckedChanged(object sender, EventArgs e)
        {
            if (((RadioButton) sender).Checked) Unsubscribe();
        }
    }
}