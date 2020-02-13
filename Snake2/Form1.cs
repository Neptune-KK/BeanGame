using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Snake2
{
    public partial class Form1 : Form
    {
        int row = 20;
        int col = 60;
        //int width = 100;
        //form本身就有一个Width属性，改一个名字
        int cell_width = 100;
        Random random = new Random();
        //Point bean = new Point(500, 500);
        Point basket = new Point(3000, 1900);
        //方块集合
        List<Point> points = new List<Point>();
        //最大方块数
        int maxcellcount = 5;
        //分数
        int score = 0;

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.DoubleBuffer, true);
        }

        public void PaintView()
        {
            if (this.BackgroundImage != null)
                this.BackgroundImage.Dispose();
            this.BackgroundImageLayout = ImageLayout.Zoom;
            Bitmap bitmap = new Bitmap(col * cell_width, row * cell_width);//注意是：指定宽高
            Pen pen = new Pen(Color.Black);
            Pen pen2 = new Pen(Color.Red, 100);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#f2f4f6")), 0, 0, cell_width * col, cell_width * row);
            for (int i = 1; i < row; i++)
            {
                graphics.DrawLine(pen, 0, i * cell_width, cell_width * col, i * cell_width);
            }
            for (int i = 1; i < col; i++)
            {
                graphics.DrawLine(pen, i * cell_width, 0, i * cell_width, row * cell_width);
            }
            graphics.DrawRectangle(pen, 0, 0, cell_width * col, cell_width * row);
            //graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#1ee3cf")), bean.X, bean.Y, cell_width, cell_width);
            //graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#66ccff")), basket.X, basket.Y, width * 3, width);

            //把basket设置成中间的方格
            graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#66ccff")), basket.X - cell_width, basket.Y, cell_width * 3, cell_width);

            //graphics.FillEllipse(new SolidBrush(Color.Black), bean.X + cell_width / 8, bean.Y + cell_width / 4, cell_width / 4, cell_width / 4);
            //graphics.FillEllipse(new SolidBrush(Color.Black), bean.X + cell_width / 2, bean.Y + cell_width / 4, cell_width / 4, cell_width / 4);

            //绘制points集合里的所有小方块
            foreach (Point bean in points)
            {
                graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#1ee3cf")), bean.X, bean.Y, cell_width, cell_width);
                //graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#66ccff")), basket.X, basket.Y, width * 3, width);
                //把basket设置成中间的方格
                graphics.FillRectangle(new SolidBrush(ColorTranslator.FromHtml("#66ccff")), basket.X - cell_width, basket.Y, cell_width * 2, cell_width);
                graphics.FillEllipse(new SolidBrush(Color.Black), bean.X + cell_width / 8, bean.Y + cell_width / 4, cell_width / 4, cell_width / 4);
                graphics.FillEllipse(new SolidBrush(Color.Black), bean.X + cell_width / 2, bean.Y + cell_width / 4, cell_width / 4, cell_width / 4);
            }

            this.BackgroundImage = bitmap;//注意：将图片设为背景才行
            graphics.Dispose();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            UpdatePoints();
            timer1.Start();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //basket = new Point(basket.X, basket.Y);
            //switch (e.KeyCode)
            //{
            //    case Keys.Left:
            //        if (basket.X != 0)
            //            basket = new Point(basket.X - cell_width, basket.Y);
            //        break;
            //    case Keys.Up:
            //        if (basket.Y != 0)
            //            basket = new Point(basket.X, basket.Y - cell_width);
            //        break;
            //    case Keys.Right:
            //        if (basket.X != col * cell_width - cell_width)
            //            basket = new Point(basket.X + cell_width, basket.Y);
            //        break;
            //    case Keys.Down:
            //        if (basket.Y != row * cell_width - cell_width)
            //            basket = new Point(basket.X, basket.Y + cell_width);
            //        break;
            //    default:
            //        break;
            //}
            //PaintView();

            //这样更高效
            switch (e.KeyCode)
            {
                case Keys.Left:
                    if(basket.X >= 0)
                        basket.X -= cell_width;
                    break;
                case Keys.Up:
                    if(basket.Y >= 0)
                        basket.Y -= cell_width;
                    break;
                case Keys.Right:
                    if (basket.X < col * cell_width)
                        basket.X += cell_width;
                    break;
                case Keys.Down:
                    if (basket.Y < row * cell_width)
                        basket.Y += cell_width;
                    break;
                default:
                    break;
            }
            CheckScore();
            PaintView();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //if (bean.Y < (row - 1) * cell_width)
            //{
            //    bean.Y += cell_width;
            //    //PaintView();
            //}
            //if (bean.Equals((object)basket))
            //{
            //    lbl1.Text += 1;

            //}
            CheckScore();
            UpdatePoints();
            MovePoints();
            PaintView();
        }


        #region 添加的方法

        /// <summary>
        /// 更新points集合，添加新的方块
        /// </summary>
        private void UpdatePoints()
        {
            //如果方块数量小于maxcellcount就有10%的概率生成新的方块
            if(points.Count< maxcellcount && random.Next(0, 10) == 0)
            {
                points.Add(new Point(random.Next(0, col) * cell_width, 0));
            }
        }

        /// <summary>
        /// 方块移动及删除
        /// </summary>
        private void MovePoints()
        {
            //超出边界要删除的方块的集合
            List<Point> delpoint = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                //所有方块向下移动一个格子
                points[i] = new Point(points[i].X, points[i].Y + cell_width);
                //如果方块超出区域，则加入删除集合
                if (points[i].Y > row * cell_width)
                {
                    delpoint.Add(points[i]);
                }
            }
            //删除所有要删除的方块
            for (int i = 0; i < delpoint.Count; i++)
            {
                points.Remove(delpoint[i]);
            }
        }

        /// <summary>
        /// 检查分数
        /// </summary>
        private void CheckScore()
        {
            //接到的即将要删除的方块集合
            List<Point> delpoint = new List<Point>();
            for (int i = 0; i < points.Count; i++)
            {
                //如果接到方块了
                if(points[i].Y == basket.Y && points[i].X >= basket.X - cell_width && points[i].X <= basket.X + cell_width)
                {
                    delpoint.Add(points[i]);
                    score++;
                    lbl1.Text = score.ToString();
                }
            }
            //删除所有接到的方块
            for (int i = 0; i < delpoint.Count; i++)
            {
                points.Remove(delpoint[i]);
            }
        }

        #endregion
    }
}
