using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Diagnostics;
using System.IO;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using System.Windows.Forms;

namespace ShowGridNumber
{

	public class ShowGridApplication : IExternalApplication
	{

		public bool DrawGridNumber { get; set; }

        public static ShowGridApplication m_ThisApp;

        public static GridNumberForm m_gridNumberShowForm;

        public IntPtr m_hRevitHandle = IntPtr.Zero;

        private UIDocument m_uiDoc;

        private WindowHandle m_hWndRevit;
        //启动时加载事件
		public Result OnStartup(UIControlledApplication application)
		{
            DrawGridNumber = true;
            ShowGridApplication.m_ThisApp = this;
            ShowGridApplication.m_gridNumberShowForm = new GridNumberForm(ShowGridApplication.m_ThisApp);
            application.ViewActivated += this.OnRefreshGridsAndViewInfo;
            application.Idling += this.OnIdlingEvent;
            if (this.m_hWndRevit == null)
            {
                Process currentProcess = Process.GetCurrentProcess();
                this.m_hRevitHandle = currentProcess.MainWindowHandle;
                this.m_hWndRevit = new WindowHandle(this.m_hRevitHandle);
            }
			return Result.Succeeded;
		}
		public Result OnShutdown(UIControlledApplication application)
		{
            application.ViewActivated -= this.OnRefreshGridsAndViewInfo;
            application.Idling -= this.OnIdlingEvent;
            ShowGridApplication.m_gridNumberShowForm.Close();
            return Result.Succeeded;
		}

		// 刷新时事件
		private void OnRefreshGridsAndViewInfo(object sender, ViewActivatedEventArgs ViewActivated)
		{
			UIApplication uiapplication = sender as UIApplication;
			if (uiapplication == null)
			{
				return;
			}
			ShowGridApplication.m_gridNumberShowForm.UnLoad();
			this.m_uiDoc = uiapplication.ActiveUIDocument;
			if (!this.DrawGridNumber)
			{
				ShowGridApplication.m_gridNumberShowForm.Hide();
			}
			else if (!ShowGridApplication.m_gridNumberShowForm.Visible)
			{
				ShowGridApplication.m_gridNumberShowForm.Show(this.m_hWndRevit);
			}
			ShowGridApplication.m_gridNumberShowForm.RefreshDocAndView(this.m_uiDoc);
		}

		// Iding事件
		private void OnIdlingEvent(object sender, IdlingEventArgs idlingArgs)
		{
			if (this.m_uiDoc == null)
			{
				return;
			}
			ShowGridApplication.m_gridNumberShowForm.DrawGridNumText(true);
		}

		// 全局轴号开关
		public void Switcher()
		{
			if (this.DrawGridNumber)
			{
				MessageBox.Show("全局轴号已关闭");
				ShowGridApplication.m_gridNumberShowForm.Hide();
			}
			else
			{
                MessageBox.Show("全局轴号已开启");
				ShowGridApplication.m_gridNumberShowForm.Show(this.m_hWndRevit);
				ShowGridApplication.m_gridNumberShowForm.UnLoad();
				ShowGridApplication.m_gridNumberShowForm.RefreshDocAndView(this.m_uiDoc);
			}
			this.DrawGridNumber = !this.DrawGridNumber;
		}
 



	}
}
