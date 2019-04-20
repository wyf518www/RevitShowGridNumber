using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ShowGridNumber
{

	[Transaction(TransactionMode.Manual)]
	public class CmdShowGridNumber : IExternalCommand
	{
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
            SettingDlg settingDlg = new SettingDlg();
            settingDlg.ShowDialog();
            
			return Result.Succeeded;
		}
	}
}
