using System;
using System.Runtime.InteropServices;
using System.Text;

namespace Win32
{
	
	public static class Functions
	{
		
		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern int GetLastError();

		
		[DllImport("kernel32.dll")]
		public static extern uint GetCurrentThreadId();

		
		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowsHookEx(HookType hook, Functions.HookProc callback, IntPtr hMod, uint dwThreadId);

		
		[DllImport("user32.dll", CharSet = CharSet.Auto)]
		public static extern IntPtr GetDlgItem(IntPtr hDlg, int nControlID);

		
		[DllImport("user32.dll")]
		public static extern IntPtr UnhookWindowsHookEx(IntPtr hhk);

		
		[DllImport("user32.dll")]
		public static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

		
		[DllImport("user32.dll")]
		public static extern IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

		
		[DllImport("user32.dll")]
		public static extern IntPtr DefDlgProc(IntPtr hDlg, uint Msg, IntPtr wParam, IntPtr lParam);

		
		[DllImport("user32.dll")]
		public static extern IntPtr GetWindowTextLength(IntPtr hWnd);

		
		[DllImport("user32.dll")]
		public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

		
		[DllImport("user32.Dll")]
		public static extern int GetLastActivePopup(IntPtr hwnd);

		
		[DllImport("user32.dll")]
		public static extern int SetWindowText(IntPtr hwnd, string str);

		
		[DllImport("user32.dll")]
		public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

		
		[DllImport("user32.dll")]
		public static extern int InternalGetWindowText(IntPtr hwnd, StringBuilder s, int nMaxCount);

		
		[DllImport("User32.Dll")]
		public static extern void GetClassName(IntPtr hwnd, StringBuilder s, int nMaxCount);

		
		[DllImport("user32.dll")]
		public static extern int FindWindow(string lpClassName, string lpWindowName);

		
		[DllImport("user32.dll")]
		public static extern int FindWindowEx(IntPtr parent_h, IntPtr child_h, string lpClassName, string lpWindowName);

		
		[DllImport("user32.dll")]
		public static extern int GetWindow(IntPtr hwnd, int flag);

		
		[DllImport("User32.dll")]
		public static extern int SendMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

		
		[DllImport("User32.dll")]
		public static extern int PostMessage(IntPtr hwnd, uint Msg, int wParam, int lParam);

		
		[DllImport("User32.dll")]
		public static extern int SetActiveWindow(IntPtr hwnd);

		
		[DllImport("user32")]
		public static extern int EnumWindows(Functions.EnumProc cbf, int lParam);

		
		[DllImport("user32")]
		public static extern int EnumChildWindows(IntPtr hwnd, Functions.EnumProc cbf, int lParam);

		
		[DllImport("user32.dll")]
		public static extern uint GetWindowThreadProcessId(IntPtr hwnd, out uint lpdwProcessId);

		
		[DllImport("User32.dll")]
		public static extern bool EnumThreadWindows(uint dwThreadId, Functions.EnumProc cbf, int lParam);

		
		public const int GW_HWNDFIRST = 0;

		
		public const int GW_HWNDLAST = 1;

		
		public const int GW_HWNDNEXT = 2;

		
		public const int GW_HWNDPREV = 3;

		
		public const int GW_OWNER = 4;

		
		public const int GW_CHILD = 5;

		
		public const int GW_MAX = 5;

		
		
		public delegate bool EnumProc(IntPtr hwnd, int lParam);

		
		
		public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

		
		public struct CWPSTRUCT
		{
			
			public IntPtr lparam;

			
			public IntPtr wparam;

			
			public int message;

			
			public IntPtr hwnd;
		}
	}
}
