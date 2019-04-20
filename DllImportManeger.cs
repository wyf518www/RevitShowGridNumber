using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace ShowGridNumber
{

	public class DllImportManeger
	{

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

		[DllImport("user32.dll")]
		public static extern IntPtr FindWindow(string strclassName, string strWindowName);

		[DllImport("user32.dll")]
		public static extern IntPtr GetLastActivePopup(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr AnyPopup();

		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

		[DllImport("user32.dll", CharSet = CharSet.Ansi)]
		public static extern int SetWindowText(IntPtr hwnd, string lpString);

		[DllImport("user32.dll")]
		public static extern IntPtr ReleaseCapture();

		[DllImport("user32.dll")]
		public static extern IntPtr SetCapture(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern IntPtr SetFocus(IntPtr hWnd);

		[DllImport("user32.dll")]
		public static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);

		[DllImport("user32.dll")]
		public static extern IntPtr EnumThreadWindows(IntPtr dwThreadId, DllImportManeger.CallBack lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr EnumWindows(DllImportManeger.CallBack lpfn, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr EnumChildWindows(IntPtr hWndParent, DllImportManeger.CallBack lpfn, IntPtr lParam);

		[DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
		public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

		[DllImport("user32.dll")]
		public static extern IntPtr WindowFromPoint(Point Point);

		[DllImport("user32 ")]
		public static extern int GetKeyboardState(byte[] pbKeyState);

		[DllImport("user32 ")]
		public static extern int ToAscii(int uVirtKey, int uScanCode, byte[] lpbKeyState, byte[] lpwTransKey, int fuState);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool ClientToScreen(IntPtr hWnd, ref Point pt);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool ScreenToClient(IntPtr hWnd, ref Point pt);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, DllImportManeger.NewWndProc wndproc);

		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr CallWindowProc(IntPtr wndProc, IntPtr hWnd, IntPtr msg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll")]
		public static extern IntPtr DefWindowProc(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

		[DllImport("user32.dll", EntryPoint = "SendMessageA")]
		public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, StringBuilder lParam);

		[DllImport("user32.dll")]
		public static extern int GetWindowRect(IntPtr hWnd, out DllImportManeger.Rect lpRect);

		[DllImport("user32.dll")]
		public static extern int GetClientRect(IntPtr hWnd, out DllImportManeger.Rect lpRect);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr BeginPaint(IntPtr hWnd, ref DllImportManeger.PAINTSTRUCT ps);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern bool EndPaint(IntPtr hWnd, ref DllImportManeger.PAINTSTRUCT ps);

		[DllImport("User32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr UpdateWindow(IntPtr hWnd);


		public delegate bool CallBack(IntPtr hwnd, IntPtr lParam);

		public delegate IntPtr NewWndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

		public struct PAINTSTRUCT
		{

			public IntPtr hdc;

			public int fErase;

			public Rectangle rcPaint;

			public int fRestore;

			public int fIncUpdate;

			public int Reserved1;

			public int Reserved2;

			public int Reserved3;

			public int Reserved4;

			public int Reserved5;

			public int Reserved6;

			public int Reserved7;

			public int Reserved8;
		}


		public struct Rect
		{
			
			public int left;

			public int top;

			public int right;

			public int bottom;
		}
	}
}
