using Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WinSelfMachine;
using WinSelfMachine.Common;
using Newtonsoft.Json;
using ModelClassLibrary.Model.HolModel;
using MyNamespace;
using static Common.Response;

namespace WinQueue
{
    public partial class Form1 : Form
    {
        private IniFileHelper _iniConfig;
        private string _configFilePath;
        private readonly ApiCommon _apiCommon;
        public Form1()
        {
            _configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.ini");
            _iniConfig = new IniFileHelper(_configFilePath);
            InitializeComponent();
            _apiCommon = new ApiCommon();
            richTextBox1.SelectionAlignment = HorizontalAlignment.Center;
            
            // 清除表格初始选中状态
            dataGridView1.ClearSelection();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // 获取所有屏幕
            Screen[] screens = Screen.AllScreens;
            // 假设要显示在第二块屏幕（索引为 1，若有多块屏幕可根据实际情况调整）
            if (screens.Length > 1)
            {
                Screen targetScreen = screens[1];
                // 设置窗体的位置为目标屏幕的工作区域（避免被任务栏遮挡等情况）
                this.StartPosition = FormStartPosition.Manual;
                this.Location = targetScreen.WorkingArea.Location;
                // 也可以设置窗体大小为目标屏幕工作区域的大小
                this.Size = targetScreen.WorkingArea.Size;
            }
            // 如果只有一块屏幕，正常显示

            var IsStart = _iniConfig.ReadInt("EquipmentIsStart", "IsStart", 0);
            if (IsStart == 0)
            {
                var Setting = new FormProInitialize();
                Setting.ShowDialog();
            }

            // 加载表格数据
            LoadTableData();
            
            // 启动定时器，每分钟刷新一次
            timer1.Start();
        }

        /// <summary>
        /// 加载表格数据
        /// </summary>
        private async void LoadTableData()
        {
            try
            {
                var response = await _apiCommon.GetExaminationAllUser();
                if (!string.IsNullOrEmpty(response))
                {
                    var responseData = JsonConvert.DeserializeObject<ApiResponse<List<HolExamination>>>(response);
                    if (responseData != null && responseData.success && responseData.Response != null)
                    {
                    this.Invoke(new Action(() =>
                    {
                        dataGridView1.Rows.Clear();
                        foreach (var exam in responseData.Response)
                        {
                            if (exam.patient != null)
                            {
                                var maskedName = MaskName(exam.patient.name);
                                dataGridView1.Rows.Add(
                                    exam.exam_no+maskedName 
                                    
                                );
                            }
                        }
                        // 清除选中状态
                        dataGridView1.ClearSelection();
                    }));
                    }
                }
            }
            catch (Exception ex)
            {
                // 记录错误但不中断程序
                System.Diagnostics.Debug.WriteLine($"加载数据失败: {ex.Message}");
            }
        }

        /// <summary>
        /// 定时器触发事件，每分钟刷新数据
        /// </summary>
        private void timer1_Tick(object sender, EventArgs e)
        {
            LoadTableData();
        }

        /// <summary>
        /// 姓名加密显示
        /// 两个字的隐藏最后一个字，三个字或多个字的隐藏中间字
        /// </summary>
        /// <param name="name">姓名</param>
        /// <returns>加密后的姓名</returns>
        private string MaskName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return name;

            if (name.Length == 1)
                return name;

            if (name.Length == 2)
            {
                // 两个字：隐藏最后一个字
                return name.Substring(0, 1) + "*";
            }
            else
            {
                // 三个字或多个字：隐藏中间字
                char firstChar = name[0];
                char lastChar = name[name.Length - 1];
                string middleStars = new string('*', name.Length - 2);
                return firstChar + middleStars + lastChar;
            }
        }
    }
}
