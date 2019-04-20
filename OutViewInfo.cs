using System;
using Autodesk.Revit.DB;

namespace ShowGridNumber
{
	
	public class OutViewInfo
	{
				
		public Line OutViewLine { get; private set; }
		public OutViewSide OutViewSide { get; private set; }
		public OutViewInfo(Line outViewline, OutViewSide outViewSide)
		{
			this.OutViewLine = outViewline;
			this.OutViewSide = outViewSide;
		}
	}
}
