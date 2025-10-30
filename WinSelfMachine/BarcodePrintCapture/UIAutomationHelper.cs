using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Automation;

namespace BarcodePrintCapture
{
    /// <summary>
    /// 使用 UI Automation 读取窗口控件信息
    /// </summary>
    public static class UIAutomationHelper
    {
        /// <summary>
        /// 使用UIA读取指定父窗口下的所有可见子元素信息
        /// 返回的数据结构与 WindowCaptureHelper.GetChildControls 保持一致，便于复用现有逻辑
        /// </summary>
        public static List<WindowCaptureHelper.WindowInfo> GetChildControls(IntPtr parentHandle)
        {
            var results = new List<WindowCaptureHelper.WindowInfo>();
            try
            {
                if (parentHandle == IntPtr.Zero)
                {
                    return results;
                }

                AutomationElement root = AutomationElement.FromHandle(parentHandle);
                if (root == null)
                {
                    return results;
                }

                // 根窗口矩形，用于换算相对坐标
                System.Windows.Rect rootRect = root.Current.BoundingRectangle;

                // 遍历所有后代元素
                var all = root.FindAll(TreeScope.Descendants, Condition.TrueCondition);
                for (int i = 0; i < all.Count; i++)
                {
                    var el = all[i];
                    string name = Safe(() => el.Current.Name) ?? string.Empty;
                    string className = Safe(() => el.Current.ClassName) ?? string.Empty;
                    var rect = Safe(() => el.Current.BoundingRectangle);

                    // 针对表格/数据行优先使用Grid/Table模式逐单元格读取
                    if (SupportsGridLike(el))
                    {
                        TryExtractGridItems(el, rootRect, results);
                        continue;
                    }

                    // 仅处理可见且有矩形的元素
                    if (rect.HasValue && !rect.Value.IsEmpty)
                    {
                        int x = (int)Math.Round(rect.Value.Left - rootRect.Left);
                        int y = (int)Math.Round(rect.Value.Top - rootRect.Top);

                        // 兼容性补充：若Name为空，保持空（部分环境缺少LegacyIAccessible支持）

                        var info = new WindowCaptureHelper.WindowInfo
                        {
                            Handle = IntPtr.Zero,
                            Text = name ?? string.Empty,
                            ClassName = className ?? string.Empty,
                            Location = new Point(x, y),
                            FullInfo = $"Txt=[{name}]clsName=[{className}]Rect={x},{y}"
                        };

                        results.Add(info);
                    }
                }
            }
            catch
            {
                // 忽略异常，返回已采集的数据
            }

            return results;
        }

        /// <summary>
        /// 通过屏幕坐标命中 DataGridView 行，读取该行前两列文本（常用于“编号/姓名”）
        /// x、y 为窗口内相对坐标
        /// </summary>
        public static bool TryReadDataGridRowByPoint(IntPtr parentHandle, int relativeX, int relativeY, out string col1, out string col2)
        {
            col1 = string.Empty;
            col2 = string.Empty;
            try
            {
                if (parentHandle == IntPtr.Zero) return false;
                var root = AutomationElement.FromHandle(parentHandle);
                if (root == null) return false;

                var rootRect = root.Current.BoundingRectangle;
                var screenPoint = new System.Windows.Point(rootRect.Left + relativeX, rootRect.Top + relativeY);

                var hit = AutomationElement.FromPoint(screenPoint);
                if (hit == null) return false;

                // 向上寻找 DataItem（行）或 DataGrid
                var walker = TreeWalker.ControlViewWalker;
                var node = hit;
                AutomationElement dataItem = null;
                AutomationElement dataGrid = null;
                while (node != null)
                {
                    if (node.Current.ControlType == ControlType.DataItem)
                    {
                        dataItem = node;
                    }
                    if (node.Current.ControlType == ControlType.DataGrid || node.Current.ControlType == ControlType.Table)
                    {
                        dataGrid = node;
                        break;
                    }
                    node = walker.GetParent(node);
                }

                if (dataGrid == null) return false;

                // 若没命中行，尝试通过 GridPattern 的 GetItem
                if (dataItem == null)
                {
                    if (dataGrid.TryGetCurrentPattern(GridPattern.Pattern, out object gp))
                    {
                        var grid = gp as GridPattern;
                        // 估算行：用相对Y在 DataGrid 矩形内的比例推测
                        var gRect = dataGrid.Current.BoundingRectangle;
                        int rows = grid.Current.RowCount;
                        if (rows <= 0) return false;
                        int approxRow = (int)Math.Max(0, Math.Min(rows - 1, Math.Round((screenPoint.Y - gRect.Top) / Math.Max(1, gRect.Height) * rows)));
                        // 取前两列
                        var c1 = SafeGetCell(grid, approxRow, 0);
                        var c2 = SafeGetCell(grid, approxRow, 1);
                        col1 = GetCellText(c1);
                        col2 = GetCellText(c2);
                        return !string.IsNullOrWhiteSpace(col1) || !string.IsNullOrWhiteSpace(col2);
                    }
                    return false;
                }

                // 遍历该行的子元素，收集前两列文本
                var cells = dataItem.FindAll(TreeScope.Children, Condition.TrueCondition);
                var texts = new List<string>();
                for (int i = 0; i < cells.Count; i++)
                {
                    var t = GetCellText(cells[i]);
                    if (!string.IsNullOrWhiteSpace(t)) texts.Add(t);
                    if (texts.Count >= 2) break;
                }
                if (texts.Count > 0) col1 = texts[0];
                if (texts.Count > 1) col2 = texts[1];
                return texts.Count > 0;
            }
            catch { return false; }
        }

        private static AutomationElement SafeGetCell(GridPattern grid, int row, int col)
        {
            try { return grid.GetItem(row, col); } catch { return null; }
        }

        private static T? Safe<T>(Func<T> getter) where T : struct
        {
            try { return getter(); } catch { return null; }
        }

        private static string Safe(Func<string> getter)
        {
            try { return getter(); } catch { return string.Empty; }
        }

        private static bool SupportsGridLike(AutomationElement el)
        {
            try
            {
                // WinForms DataGridView、WPF DataGrid、第三方表格大多支持 GridPattern 或 TablePattern
                return el.TryGetCurrentPattern(GridPattern.Pattern, out _) ||
                       el.TryGetCurrentPattern(TablePattern.Pattern, out _) ||
                       el.Current.ControlType == ControlType.DataGrid ||
                       el.Current.ControlType == ControlType.Table;
            }
            catch { return false; }
        }

        private static void TryExtractGridItems(AutomationElement grid, System.Windows.Rect rootRect, List<WindowCaptureHelper.WindowInfo> results)
        {
            try
            {
                GridPattern gridPattern = null;
                if (grid.TryGetCurrentPattern(GridPattern.Pattern, out object gp))
                {
                    gridPattern = gp as GridPattern;
                }

                if (gridPattern != null)
                {
                    int rows = gridPattern.Current.RowCount;
                    int cols = gridPattern.Current.ColumnCount;
                    for (int r = 0; r < rows; r++)
                    {
                        for (int c = 0; c < cols; c++)
                        {
                            AutomationElement cell = null;
                            try { cell = gridPattern.GetItem(r, c); } catch { }
                            if (cell == null) continue;

                            string text = GetCellText(cell);
                            var rect = cell.Current.BoundingRectangle;
                            if (string.IsNullOrWhiteSpace(text) || rect.IsEmpty) continue;

                            int x = (int)Math.Round(rect.Left - rootRect.Left);
                            int y = (int)Math.Round(rect.Top - rootRect.Top);

                            results.Add(new WindowCaptureHelper.WindowInfo
                            {
                                Handle = IntPtr.Zero,
                                Text = text,
                                ClassName = cell.Current.ClassName,
                                Location = new Point(x, y),
                                FullInfo = $"Txt=[{text}]clsName=[{cell.Current.ClassName}]Rect={x},{y}"
                            });
                        }
                    }
                    return; // 已读取到单元格
                }

                // TablePattern 作为降级
                if (grid.TryGetCurrentPattern(TablePattern.Pattern, out object tp))
                {
                    var table = tp as TablePattern;
                    var items = table.Current.GetRowHeaders();
                    // 即便没有RowHeaders，也可以遍历所有DataItem
                    var descendants = grid.FindAll(TreeScope.Descendants, new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.DataItem));
                    for (int i = 0; i < descendants.Count; i++)
                    {
                        var item = descendants[i];
                        var rect = item.Current.BoundingRectangle;
                        string text = GetCellText(item);
                        if (rect.IsEmpty || string.IsNullOrWhiteSpace(text)) continue;

                        int x = (int)Math.Round(rect.Left - rootRect.Left);
                        int y = (int)Math.Round(rect.Top - rootRect.Top);

                        results.Add(new WindowCaptureHelper.WindowInfo
                        {
                            Handle = IntPtr.Zero,
                            Text = text,
                            ClassName = item.Current.ClassName,
                            Location = new Point(x, y),
                            FullInfo = $"Txt=[{text}]clsName=[{item.Current.ClassName}]Rect={x},{y}"
                        });
                    }
                }
            }
            catch { }
        }

        private static string GetCellText(AutomationElement element)
        {
            // 读取单元格文本的多重策略
            try
            {
                if (element.TryGetCurrentPattern(ValuePattern.Pattern, out object vp))
                {
                    var v = vp as ValuePattern;
                    if (!string.IsNullOrWhiteSpace(v.Current.Value)) return v.Current.Value;
                }
            }
            catch { }

            try
            {
                if (element.TryGetCurrentPattern(TextPattern.Pattern, out object tp))
                {
                    var t = tp as TextPattern;
                    var range = t.DocumentRange;
                    string s = range?.GetText(-1) ?? string.Empty;
                    if (!string.IsNullOrWhiteSpace(s)) return s.Trim();
                }
            }
            catch { }

            string name = Safe(() => element.Current.Name);
            if (!string.IsNullOrWhiteSpace(name)) return name;

            return string.Empty;
        }

        // LegacyIAccessiblePattern 在部分目标框架/系统上不可用，这里不强依赖
    }
}


