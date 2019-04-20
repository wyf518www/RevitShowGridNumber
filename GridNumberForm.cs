using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.UI;


///SetWindowLoog函数通过改变与特定窗口相关的窗口过程，
///使系统调用新的窗口过程来创建子类，
///新的窗口过程替换了以前的窗口过程。
///应用程序必须通过调用CallWindowsProc来将新窗日过程没有处理的任何消息传送到以前的窗口过程中，
///这样就允许应用程序创建一系列窗口过程。
///
/// GWL_EXSTYLE// -20	// 设定一个新的扩展风格。
/// GWL_HINSTANCE// -6	// 设置一个新的应用程序实例句柄。
/// GWL_ID// -12	// 设置一个新的窗口标识符。
/// GWL_STYLE// -16	// 设定一个新的窗口风格。
/// GWL_USERDATA// -21	// 设置与窗口有关的32位值。每个窗口均有一个由创建该窗口的应用程序使用的32位值。
/// GWL_WNDPROC// -4	// 为窗口设定一个新的处理函数。
/// GWL_HWNDPARENT	-8	改变子窗口的父窗口,应使用SetParent函数。
///
/// 
/// 
/// 
///使用函数CallWindowsProc可进行窗口子分类。
///通常来说，同一类的所有窗口共享一个窗口过程。
///子类是一个窗口或者相同类的一套窗口，
///在其消息被传送到该类的窗口过程之前，
///这些消息是由另一个窗口过程进行解释和处理的。
namespace ShowGridNumber
{
    public struct Rect
    {

        public int left;

        public int top;

        public int right;

        public int bottom;
    }
	public partial class GridNumberForm : Form
	{

        private UIDocument m_uiDoc;
        // 轴号字体
        private float m_FontSize = 15;
        public float fontSize
        {
            get { return m_FontSize; }
            set { m_FontSize = value; }
        }
        /*private Font m_font = new Font("微软雅黑", 15, GraphicsUnit.Pixel);*/
        //字体颜色
        private System.Drawing.Color m_FontColor = Color.Red;
        public System.Drawing.Color fontColor
        {
            get { return m_FontColor; }
            set { m_FontColor = value; }
        }
        private ShowGridNumberManager m_GridNumShowMag;
        private GridNumberForm.NewWndProc m_hWndChildClientProcNew;
        private GridNumberForm.NewWndProc m_hWndChildProcNew;
        private IntPtr m_hWndChildClientProcOld = IntPtr.Zero;
        private IntPtr m_hWndChildProcOld = IntPtr.Zero;
        private IntPtr m_hMDIChildHandle = IntPtr.Zero;
        private IntPtr m_hWndChildHandle = IntPtr.Zero;
        private IntPtr m_hWndChildClientHandle = IntPtr.Zero;
        private bool m_midButtonDown;
        private ShowGridApplication m_appShowGrid;
        public delegate IntPtr NewWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		public GridNumberForm(ShowGridApplication application)
		{
			m_appShowGrid = application;
			InitializeComponent();
		}

        #region Windows  User32 Api
        //在窗口列表中寻找与指定条件相符的第一个子窗口
        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

        //将消息信息传送给指定的窗口过程的函数
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, GridNumberForm.NewWndProc wndproc);

        //返回指定窗口的边框矩形的尺寸
        [DllImport("user32.dll")]
        public static extern int GetWindowRect(IntPtr hWnd, out Rect lpRect);


        public delegate bool CallBack(IntPtr hwnd, IntPtr lParam);

        //枚举一个父窗口的所有子窗口
        [DllImport("user32.dll")]
        public static extern IntPtr EnumChildWindows(IntPtr hWndParent, CallBack lpfn, IntPtr lParam);

        //获得指定窗口所属的类的类名
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);
        #endregion
        private void GridNumberForm_Load(object sender, EventArgs e)
		{
			GetMDIChildWindowHandle();
		}

        //获取MDI子窗口句柄
		private void GetMDIChildWindowHandle()
		{
			IntPtr zero = IntPtr.Zero;
			try
			{
				IntPtr mainWindowHandle = Process.GetCurrentProcess().MainWindowHandle;
				m_hMDIChildHandle = FindWindowEx(mainWindowHandle, IntPtr.Zero, "MDIClient", "");
			}
			catch
			{
			}
			finally
			{
				Marshal.FreeHGlobal(zero);
			}
		}

		// 窗口关闭事件
		private void GridNumberForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (IntPtr.Zero != m_hWndChildClientProcOld)
			{
                // GWL_WNDPROC// -4	// 为窗口设定一个新的处理函数。
				GridNumberForm.SetWindowLong(m_hWndChildClientHandle, -4, m_hWndChildClientProcOld);
			}
			if (IntPtr.Zero != m_hWndChildProcOld)
			{
				GridNumberForm.SetWindowLong(m_hWndChildHandle, -4, m_hWndChildProcOld);
			}
			/*m_font.Dispose();*/
		}

		// 子窗口用户句柄
		public IntPtr WndMsgProcChildClient(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			IntPtr newPtr;
			try
			{
				IntPtr intPtr = GridNumberForm.CallWindowProc(m_hWndChildClientProcOld, hWnd, msg, wParam, lParam);
				if (msg <= 32)
				{
                    //2---窗口被销毁
					if (msg == 2)
					{
                        // GWL_WNDPROC// -4	// 为窗口设定一个新的处理函数。
                        GridNumberForm.SetWindowLong(m_hWndChildClientHandle, -4, m_hWndChildClientProcOld);
                        m_hWndChildClientProcNew = null;
                        base.Hide();
					}

				}
                    //512移动鼠标
				else if (msg != 512)
				{
					switch (msg)
					{
                            //519按下鼠标中键
					case 519:
						m_midButtonDown = true;
						break;
                            //520释放鼠标中键
					case 520:
						m_midButtonDown = false;
                        //MessageBox.Show("释放中键！");
						break;
                            //521双击鼠标中键
					case 521:
                            //522中键转动(滚轮）
					case 522:
						DrawGridNumText(false);
						break;
					}
				}
				else if (m_midButtonDown)
				{
					DrawGridNumText(false);
				}
				newPtr = intPtr;
			}
			catch (Exception)
			{
				newPtr = IntPtr.Zero;
			}
			return newPtr;
		}

        //子窗口句柄
		public IntPtr WndMsgProcChild(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam)
		{
			IntPtr result;
			try
			{
                //CallWindowProc是将消息信息传送给指定的窗口过程的函数。
				IntPtr intPtr = GridNumberForm.CallWindowProc(m_hWndChildProcOld, hWnd, msg, wParam, lParam);
                //2---窗口被销毁
				if (msg != 2)
				{
                    //5---窗口改变大小
					if (msg != 5)
					{
						switch (msg)
						{
                            //532调整窗口大小
                            //534移动窗口，通过此消息应用程序可以监视窗口大小和位置也可以修改他们
						case 532:
						case 534:
							DrawGridNumText(false);
							break;
						}
					}
					else
					{
                        /*MessageBox.Show("窗口大小改变");*/
						DrawGridNumText(false);
					}
				}
				else
				{
					GridNumberForm.SetWindowLong(m_hWndChildHandle, -4, m_hWndChildProcOld);
					m_hWndChildProcNew = null;
					base.Hide();
				}
				result = intPtr;
			}
			catch (Exception)
			{
				result = IntPtr.Zero;
			}
			return result;
		}

		// 刷新文档和视图
		public void RefreshDocAndView(UIDocument uiDoc)
		{
			if (uiDoc == null)
			{
				return;
			}
			m_uiDoc = uiDoc;
			if (m_GridNumShowMag == null)
			{
				m_GridNumShowMag = new ShowGridNumberManager(uiDoc);
			}
			else
			{
				m_GridNumShowMag.RefreshUIDocument(m_uiDoc);
			}
			IntPtr intPtr = IntPtr.Zero;
			try
			{
                //SendMessageWindows API宏，在WinUser.h中根据是否已定义Unicode被定义为SendMessageW或SendMessageA
                //，这两个函数将指定的消息发送到一个或多个窗口。此函数为指定的窗口调用窗口程序，直到窗口程序处理完消息再返回。
                //而和函数PostMessage不同，PostMessage是将一个消息寄送到一个线程的消息队列后就立即返回。
               //553u 应用程序将WM_MDIGETACTIVE消息发送到多文档界面（MDI）客户端窗口，以检索活动MDI子窗口的句柄。
				m_hWndChildHandle = (IntPtr)SendMessage(m_hMDIChildHandle,553u, 0, 0);
				if (m_hWndChildProcNew == null)
				{
					m_hWndChildProcNew = new GridNumberForm.NewWndProc(WndMsgProcChild);
				}
				m_hWndChildProcOld = GridNumberForm.SetWindowLong(m_hWndChildHandle, -4, m_hWndChildProcNew);
                //转换 String 类写入非托管内存
				intPtr = Marshal.StringToHGlobalAnsi("AfxFrameOrView");
				EnumChildWindows(m_hWndChildHandle, new CallBack(EnumMDIChildClient), intPtr);
				if (m_hWndChildClientProcNew == null)
				{
					m_hWndChildClientProcNew = new GridNumberForm.NewWndProc(WndMsgProcChildClient);
				}
                //SetWindowLong该函数用来改变指定窗口的属性

				m_hWndChildClientProcOld = GridNumberForm.SetWindowLong(m_hWndChildClientHandle, -4, m_hWndChildClientProcNew);
			}
			catch (Exception)
			{
				m_appShowGrid.DrawGridNumber = false;
			}
			finally
			{
                //释放非托管的字符串
				Marshal.FreeHGlobal(intPtr);
			}
		}

		// 是否加载
		public bool UnLoad()
		{
			if (m_hWndChildClientProcNew != null)
			{
				GridNumberForm.SetWindowLong(m_hWndChildHandle, -4, m_hWndChildProcOld);
				GridNumberForm.SetWindowLong(m_hWndChildClientHandle, -4, m_hWndChildClientProcOld);
				return false;
			}
			return true;
		}
        //枚举MDI子窗口
		private bool EnumMDIChildClient(IntPtr hwnd, IntPtr ptrKey)
		{
			try
			{
                //指针到字符串
				string value = Marshal.PtrToStringAnsi(ptrKey);
				StringBuilder classname = new StringBuilder(512);
				GetClassName(hwnd, classname, classname.Capacity);
				if (classname.ToString().Contains(value))
				{
					m_hWndChildClientHandle = hwnd;
					return false;
				}
			}
			catch
			{
			}
			return true;
		}

		// 绘制轴圈/文字
		public void DrawGridNumText(bool bIdling = false)
		{
			Graphics graphics = base.CreateGraphics();
			if (m_uiDoc == null || m_uiDoc.ActiveView == null)
			{
				graphics.Clear(System.Drawing.Color.White);
				graphics.Dispose();
				return;
			}
            if (!m_appShowGrid.DrawGridNumber || m_uiDoc.ActiveView.ViewType == Autodesk.Revit.DB.ViewType.ThreeD)
			{
				graphics.Clear(System.Drawing.Color.White);
				graphics.Dispose();
				return;
			}
			Size dlgSzie = default(Size);
			Point locationPt = Point.Empty;
			Rect rcMDIClientWin;
			GetWindowRect(m_hMDIChildHandle, out rcMDIClientWin);
			CalcGridInfoResult gridNumberShowInfo = m_GridNumShowMag.GetGridNumberShowInfo(bIdling, rcMDIClientWin, ref locationPt, ref dlgSzie);
			if (gridNumberShowInfo == CalcGridInfoResult._Idling)
			{
				return;
			}
			base.Location = locationPt;
			base.Size = dlgSzie;
			m_GridNumShowMag.TranformIntersectPt(base.Handle);
			graphics.SmoothingMode = SmoothingMode.AntiAlias;
			graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
			graphics.CompositingQuality = CompositingQuality.HighQuality;
			graphics.Clear(System.Drawing.Color.White);
            
			Brush brush = new SolidBrush(fontColor);
            Pen pen = new Pen(fontColor, 2f);
            Font m_font = new Font("微软雅黑", m_FontSize, GraphicsUnit.Pixel);
            int circleSize = Convert.ToInt32(m_FontSize) + 5;
			StringFormat bottomStr = new StringFormat();
			bottomStr.Alignment = StringAlignment.Center;
			bottomStr.LineAlignment = StringAlignment.Center;
			StringFormat leftStr = new StringFormat();
			leftStr.Alignment = StringAlignment.Near;
			leftStr.LineAlignment = StringAlignment.Center;
			StringFormat rightStr = new StringFormat();
			rightStr.Alignment = StringAlignment.Far;
			rightStr.LineAlignment = StringAlignment.Center;
			for (int i = 0; i < m_GridNumShowMag.m_GridNumShowInfos.Count; i++)
			{
				Point cirlLocation = m_GridNumShowMag.m_GridNumShowInfos[i].CirlLocation;
				Point textLocation = m_GridNumShowMag.m_GridNumShowInfos[i].TextLocation;
				string elemText = m_GridNumShowMag.m_GridNumShowInfos[i].ElemText;
				if (m_GridNumShowMag.m_GridNumShowInfos[i].ElemClass == ElemType._Grid)
				{
                    //轴号圈大小位置颜色
                    graphics.DrawArc(pen, cirlLocation.X, cirlLocation.Y, circleSize, circleSize, 0, 360);
				}
				switch (m_GridNumShowMag.m_GridNumShowInfos[i].OutlineSide)
				{
				case OutViewSide._Up:
				case OutViewSide._Bottom:
					graphics.DrawString(elemText, m_font, brush, textLocation, bottomStr);
					break;
				case OutViewSide._Left:
					graphics.DrawString(elemText, m_font, brush, textLocation, leftStr);
					break;
				case OutViewSide._Right:
					graphics.DrawString(elemText, m_font, brush, textLocation, rightStr);
					break;
				}
			}
			bottomStr.Dispose();
			leftStr.Dispose();
			rightStr.Dispose();
			graphics.Dispose();
		}



	}
}
