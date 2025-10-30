using System;
using System.Drawing;
using System.Runtime.InteropServices;
using Accessibility;

namespace BarcodePrintCapture
{
    /// <summary>
    /// MSAA（Active Accessibility）辅助：通过屏幕坐标命中元素并读取 Name
    /// 适用于 WinForms DataGridView 等控件，稳定性高于 UIA 枚举。
    /// </summary>
    public static class MSAAHelper
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool GetWindowRect(IntPtr hWnd, out RECT lpRect);

        [DllImport("oleacc.dll")]
        private static extern int AccessibleObjectFromPoint(POINT pt, out IAccessible ppacc, out object pvarChild);

        [StructLayout(LayoutKind.Sequential)]
        private struct RECT { public int Left, Top, Right, Bottom; }

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT { public int X; public int Y; }

        /// <summary>
        /// 读取窗口内相对坐标处元素的 Name 文本（失败返回空字符串）
        /// </summary>
        public static string GetNameAtWindowRelativePoint(IntPtr hwnd, int relX, int relY)
        {
            try
            {
                if (hwnd == IntPtr.Zero) return string.Empty;

                // 计算屏幕坐标
                RECT rect;
                if (!GetWindowRect(hwnd, out rect)) return string.Empty;
                POINT pt = new POINT { X = rect.Left + relX, Y = rect.Top + relY };

                // 命中元素
                if (AccessibleObjectFromPoint(pt, out IAccessible acc, out object child) >= 0 && acc != null)
                {
                    try
                    {
                        // 若 child 是子元素索引，优先读取子元素名
                        if (child is int childId && childId != 0)
                        {
                            return acc.get_accName(childId) ?? string.Empty;
                        }
                        // 否则读自身
                        return acc.get_accName(0) ?? string.Empty;
                    }
                    catch { }
                }
            }
            catch { }
            return string.Empty;
        }
    }
}


