using System;
using Autodesk.Revit.DB;

namespace ShowGridNumber
{

	public class DrawElement
	{

		public Element DrawElem { get; private set; }

		public ElemType DrawElemType { get; private set; }

		public DrawElement(Element elem, ElemType type)
		{
			this.DrawElem = elem;
			this.DrawElemType = type;
		}
	}
}
