using System;
using System.Windows.Forms;

namespace ShowGridNumber
{
	
	public class WindowHandle : IWin32Window
	{
		
		public WindowHandle(IntPtr h)
		{
			this._hwnd = h;
		}	
		
		public IntPtr Handle
		{
			get
			{
				return this._hwnd;
			}
		}		
		private IntPtr _hwnd;
	}
}
