using System;

namespace Win32
{

	public struct CWPRETSTRUCT
	{
		public IntPtr lResult;

		public IntPtr lParam;

		public IntPtr wParam;

		public uint message;

		public IntPtr hwnd;
	}
}
