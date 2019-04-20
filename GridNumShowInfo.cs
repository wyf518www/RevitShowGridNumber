using System;
using System.Drawing;
using Autodesk.Revit.DB;

namespace ShowGridNumber
{

	public class GridNumShowInfo
	{

		public XYZ IntersectPoint { get; set; }

		public System.Drawing.Point TextLocation { get; set; }

		public System.Drawing.Point CirlLocation { get; set; }

		public string ElemText { get; set; }

        public OutViewSide OutlineSide { get; set; }

		public ElemType ElemClass { get; set; }
	}
}
