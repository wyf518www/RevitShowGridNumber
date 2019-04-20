using System;
using Autodesk.Revit.DB;

namespace ShowGridNumber
{
	
	public class ViewOutLineInfo
	{
				
		public Line ViewOutLine { get; private set; }
		public OutLineSide OutlineSide { get; private set; }
		public ViewOutLineInfo(Line lnViewOutline, OutLineSide outLineSide)
		{
			this.ViewOutLine = lnViewOutline;
			this.OutlineSide = outLineSide;
		}
	}
}
