using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WinQueue
{
    public class TextScrollControl : System.Windows.Forms.UserControl
    {
        private System.Windows.Forms.Label label1;
        private string _labelText;
        private int _speed;
        private Timer timer = null;
        private int _speedParam = 1000;
        private int _maxSpeedParam = 100;
        private int _minSpeedParam = 1;
        private int _normalSpeed = 13;
        private DirectionEnum _direction;
        private int _gap = 5;

        #region 自定义属性
        [Browsable(true), Category("自定义属性"), Description("滚动文本设置")]
        public string LabelText { set { _labelText = value; Invalidate(); } get { return _labelText; } }
        [Browsable(true), Category("自定义属性"), Description("滚动速度设置,最大值为100,最少值为1")]
        public int Speed { set { _speed = value; Invalidate(); } get { return _speed; } }
        [Browsable(true), Category("自定义属性"), Description("文字滚动的方向")]
        public DirectionEnum Direction { set { _direction = value; Invalidate(); } get { return _direction; } }

        public enum DirectionEnum
        {
            RightToLeft,
            LeftToRight
        }
        #endregion
        public TextScrollControl()
        {
            InitializeComponent();
            this.BorderStyle = BorderStyle.FixedSingle;
            this.label1.Location = new Point(this.Width, this.label1.Location.Y);
            timer = new Timer();
            Speed = _normalSpeed;
            timer.Interval = _speedParam / Speed;
            timer.Tick += Timer_Tick;
            timer.Start();
            LabelText = this.label1.Text;
            Direction = DirectionEnum.RightToLeft;
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            SetControlProperty();
            SetTextPosition();
        }
        /// <summary>
        /// 设置文字的位置
        /// </summary>
        private void SetTextPosition()
        {
            if (Direction == DirectionEnum.RightToLeft)
            {
                if ((this.label1.Location.X - _gap) > 0)
                {
                    this.label1.Location = new Point(this.label1.Location.X - _gap, this.label1.Location.Y);
                }
                else
                {
                    this.label1.Location = new Point(this.Width, this.label1.Location.Y);
                }
            }
            else if (Direction == DirectionEnum.LeftToRight)
            {
                if ((this.label1.Location.X + _gap) < this.Width)
                {
                    this.label1.Location = new Point(this.label1.Location.X + _gap, this.label1.Location.Y);
                }
                else
                {
                    this.label1.Location = new Point(0, this.label1.Location.Y);
                }
            }
        }

        /// <summary>
        /// 设置控件的属性值
        /// </summary>
        private void SetControlProperty()
        {
            this.label1.Text = LabelText;
            if (Speed > _maxSpeedParam) Speed = _maxSpeedParam;
            if (Speed < _minSpeedParam) Speed = _minSpeedParam;
            timer.Interval = _speedParam / Speed;
        }


        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(239, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "label1";
            // 
            // TextRollUserControl
            // 
            this.Controls.Add(this.label1);
            this.Size = new System.Drawing.Size(283, 34);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

    }
}