using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace BarcodePrintCapture
{
    /// <summary>
    /// 窗口捕获辅助类
    /// </summary>
    public class WindowCaptureHelper
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        [DllImport("user32.dll")]
        private static extern bool EnumChildWindows(IntPtr hWnd, EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int X;
            public int Y;
        }

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        /// <summary>
        /// 窗口信息类
        /// </summary>
        public class WindowInfo
        {
            public IntPtr Handle { get; set; }
            public string ProcessName { get; set; }
            public string WindowTitle { get; set; }
            public string ClassName { get; set; }
            public Point Location { get; set; }
            public string Text { get; set; }
            public string FullInfo { get; set; }
        }

        /// <summary>
        /// 根据条件搜索窗口
        /// </summary>
        public static List<WindowInfo> SearchWindows(string processName, string windowTitle, string className)
        {
            var windows = new List<WindowInfo>();

            EnumWindows((IntPtr hWnd, IntPtr lParam) =>
            {
                uint processId;
                GetWindowThreadProcessId(hWnd, out processId);

                string processNameStr = "";
                try
                {
                    Process proc = Process.GetProcessById((int)processId);
                    processNameStr = proc.ProcessName;
                }
                catch { }

                // 如果指定了进程名，进行过滤
                if (!string.IsNullOrEmpty(processName) && !processNameStr.Equals(processName, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                StringBuilder sb = new StringBuilder(256);
                GetWindowText(hWnd, sb, 256);
                string windowTitleStr = sb.ToString();

                // 如果指定了窗口标题，进行过滤
                if (!string.IsNullOrEmpty(windowTitle) && !windowTitleStr.Contains(windowTitle))
                {
                    return true;
                }

                sb.Clear();
                GetClassName(hWnd, sb, 256);
                string classNameStr = sb.ToString();

                // 如果指定了类名，进行过滤
                if (!string.IsNullOrEmpty(className) && !classNameStr.Contains(className))
                {
                    return true;
                }

                RECT rect;
                GetWindowRect(hWnd, out rect);
                Point location = new Point(rect.Left, rect.Top);

                windows.Add(new WindowInfo
                {
                    Handle = hWnd,
                    ProcessName = processNameStr,
                    WindowTitle = windowTitleStr,
                    ClassName = classNameStr,
                    Location = location,
                    FullInfo = $"进程：{processNameStr}，标题：{windowTitleStr}，类名：{classNameStr}"
                });

                return true;
            }, IntPtr.Zero);

            return windows;
        }

        /// <summary>
        /// 获取指定窗口的所有子控件
        /// </summary>
        public static List<WindowInfo> GetChildControls(IntPtr parentHandle)
        {
            var controls = new List<WindowInfo>();

            EnumChildWindows(parentHandle, (IntPtr hWnd, IntPtr lParam) =>
            {
                try
                {
                    StringBuilder sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, 256);
                    string text = sb.ToString();

                    sb.Clear();
                    GetClassName(hWnd, sb, 256);
                    string className = sb.ToString();

                    RECT rect;
                    GetWindowRect(hWnd, out rect);

                    // 转换到窗口内坐标
                    POINT point = new POINT { X = rect.Left, Y = rect.Top };
                    GetWindowRect(parentHandle, out RECT parentRect);

                    int x = rect.Left - parentRect.Left;
                    int y = rect.Top - parentRect.Top;

                    controls.Add(new WindowInfo
                    {
                        Handle = hWnd,
                        Text = text,
                        ClassName = className,
                        Location = new Point(x, y),
                        FullInfo = $"Txt=[{text}]clsName=[{className}]Rect={x},{y}"
                    });
                }
                catch { }

                return true;
            }, IntPtr.Zero);

            return controls;
        }

        /// <summary>
        /// 获取控件的文本内容
        /// </summary>
        public static string GetControlText(IntPtr hWnd)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);
            return sb.ToString();
        }

        /// <summary>
        /// 获取控件的矩形区域
        /// </summary>
        public static Rectangle GetControlRect(IntPtr hWnd, IntPtr parentHandle)
        {
            RECT rect;
            GetWindowRect(hWnd, out rect);

            RECT parentRect;
            GetWindowRect(parentHandle, out parentRect);

            return new Rectangle(
                rect.Left - parentRect.Left,
                rect.Top - parentRect.Top,
                rect.Right - rect.Left,
                rect.Bottom - rect.Top);
        }
    }
}

