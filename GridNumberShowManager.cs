using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ShowGridNumber
{

	public class GridNumberShowManager
	{

        public List<GridNumShowInfo> m_lstGridNumShowInfo = new List<GridNumShowInfo>();
        private List<RevitLinkInstance> m_lstLinkInst = new List<RevitLinkInstance>();
        private List<ViewOutLineInfo> m_lstViewOutLineInfo = new List<ViewOutLineInfo>();
        private List<DrawElement> m_lstDrawElems = new List<DrawElement>();
        private UIDocument m_uiDoc;
        private Document m_doc;
        private UIView m_uiView;
        private int m_nLeft;
        private int m_nRight;
        private int m_nTop;
        private int m_nBottom;

		public GridNumberShowManager(UIDocument uiDoc)
		{
			this.m_uiDoc = uiDoc;
			this.m_doc = uiDoc.Document;
		}
        
		public void RefreshUIDocument(UIDocument uiDoc)
		{
			this.m_uiDoc = uiDoc;
			this.m_doc = uiDoc.Document;
			this.m_uiView = null;
			this.m_lstViewOutLineInfo.Clear();
			this.m_lstDrawElems.Clear();
			this.m_lstGridNumShowInfo.Clear();
		}
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref System.Drawing.Point pt);
        public System.Drawing.Point Revit2Screen(XYZ point)
        {
            try
            {
                int m_left = 0;
                int m_top = 0;
                int m_right = 0;
                int m_bottom = 0;
                GetRevitWndRectangle(this.m_uiView, ref m_left, ref m_top, ref m_right, ref m_bottom);
                Transform inverse = this.m_uiDoc.ActiveView.CropBox.Transform.Inverse;
                IList<XYZ> zoomCorners = this.m_uiView.GetZoomCorners();
                XYZ xyz = inverse.OfPoint(point);
                XYZ xyz2 = inverse.OfPoint(zoomCorners[0]);
                XYZ xyz3 = inverse.OfPoint(zoomCorners[1]);
                double num5 = (double)(m_right - m_left) / (xyz3.X - xyz2.X);
                int x = m_left + (int)((xyz.X - xyz2.X) * num5);
                int y = m_top + (int)((xyz3.Y - xyz.Y) * num5);
                return new System.Drawing.Point(x, y);
            }
            catch
            {
            }
            return System.Drawing.Point.Empty;
        }
        //获取当前文档中链接模型实例
		private void GetAllRevitLinkInstancesInDoc()
		{
			this.m_lstLinkInst.Clear();
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(this.m_doc).OfClass(typeof(RevitLinkInstance));
			ICollection<ElementId> collection = filteredElementCollector.ToElementIds();
			foreach (ElementId elementId in collection)
			{
				Element element = this.m_doc.GetElement(elementId);
				if (!(element.GetType() != typeof(RevitLinkInstance)))
				{
					RevitLinkInstance revitLinkInstance = element as RevitLinkInstance;
					if (revitLinkInstance != null)
					{
						this.m_lstLinkInst.Add(revitLinkInstance);
					}
				}
			}
		}
        //获取轴号信息
		public CalcGridInfoResult GetGridNumberShowInfo(bool bIdling, DllImportManeger.Rect rcMDIClientWin, ref System.Drawing.Point ptForm, ref Size sizeForm)
		{
			if (this.m_uiDoc == null)
			{
				return CalcGridInfoResult.eError;
			}
			if (this.m_uiView == null)
			{
				IList<UIView> openUIViews = this.m_uiDoc.GetOpenUIViews();
				foreach (UIView uiview in openUIViews)
				{
					if (uiview.ViewId == this.m_uiDoc.ActiveView.Id)
					{
						this.m_uiView = uiview;
						break;
					}
				}
			}
			if (this.m_uiView == null)
			{
				return CalcGridInfoResult.eError;
			}
			this.GetAllRevitLinkInstancesInDoc();
			this.m_lstViewOutLineInfo.Clear();
			this.m_lstDrawElems.Clear();
			this.m_lstGridNumShowInfo.Clear();
			CalcGridInfoResult currentViewOutLine = this.GetCurrentViewOutLine(bIdling, rcMDIClientWin, ref sizeForm, ref ptForm);
			if (currentViewOutLine != CalcGridInfoResult.eSucceded)
			{
				return currentViewOutLine;
			}
			this.GetAllGridInfo();
			this.GetAllMultiSegmentGridInfo();
			this.GetAllLevelInfo();
			this.GetAllLevelInfoInLinkDoc();
			if (this.m_lstDrawElems.Count <= 0)
			{
				return CalcGridInfoResult.eError;
			}
			this.GetIntersectPtAndDirection();
			if (this.m_lstGridNumShowInfo.Count <= 0)
			{
				return CalcGridInfoResult.eError;
			}
			return CalcGridInfoResult.eSucceded;
		}
        //判断double是否近似相等
        public static bool IsEqual(double val1, double val2)
        {
            return !LessThan(val1, val2) && !LessThan(val2, val1);
        }
        //判断double大小  误差值-1E-09
        public static bool LessThan(double val1, double val2)
        {
            return val1 - val2 < -1E-09;
        }
        //判断double大小  误差值-1E-09
        public static bool GreaterThan(double val1, double val2)
        {
            return val1 - val2 > 1E-09;
        }
        //获取revit窗口
        public static void GetRevitWndRectangle(UIView view, ref int left, ref int top, ref int right, ref int bottom)
        {
            Autodesk.Revit.UI.Rectangle windowRectangle = view.GetWindowRectangle();
            left = windowRectangle.Left;
            top = windowRectangle.Top;
            right = windowRectangle.Right;
            bottom = windowRectangle.Bottom;
        }
        //获取当前视图的外部线
        private CalcGridInfoResult GetCurrentViewOutLine(bool bIdling, DllImportManeger.Rect rcMDIClientWin, ref Size szForm, ref System.Drawing.Point ptForm)
        {
            int m_left = 0;
            int m_right = 0;
            int m_top = 0;
            int m_bottom = 0;
            GetRevitWndRectangle(this.m_uiView, ref m_left, ref m_top, ref m_right, ref m_bottom);
            if (IsEqual((double)m_left, (double)m_right) || IsEqual((double)m_top, (double)m_bottom))
            {
                return CalcGridInfoResult.eMin;
            }
            if (bIdling &&
                IsEqual((double)m_left, (double)this.m_nLeft) &&
                IsEqual((double)m_top, (double)this.m_nTop) &&
                IsEqual((double)m_right, (double)this.m_nRight) &&
                IsEqual((double)m_bottom, (double)this.m_nBottom))
            {
                return CalcGridInfoResult.eIdling;
            }
            this.m_nLeft = m_left;
            this.m_nRight = m_right;
            this.m_nTop = m_top;
            this.m_nBottom = m_bottom;
            if (GreaterThan((double)rcMDIClientWin.left, (double)m_left))
            {
                m_left = rcMDIClientWin.left;
            }
            if (LessThan((double)rcMDIClientWin.right, (double)m_right))
            {
                m_right = rcMDIClientWin.right;
            }
            if (GreaterThan((double)rcMDIClientWin.top, (double)m_top))
            {
                m_top = rcMDIClientWin.top;
            }
            if (LessThan((double)rcMDIClientWin.bottom, (double)m_bottom))
            {
                m_bottom = rcMDIClientWin.bottom;
            }
            XYZ xyz = new XYZ((double)m_left, (double)m_bottom, 0.0);
            XYZ xyz2 = new XYZ((double)m_left, (double)m_top, 0.0);
            XYZ xyz3 = new XYZ((double)m_right, (double)m_top, 0.0);
            XYZ xyz4 = new XYZ((double)m_right, (double)m_bottom, 0.0);
            szForm.Width = m_right - m_left;
            szForm.Height = m_bottom - m_top;
            ptForm.X = m_left;
            ptForm.Y = m_top;
            Line lnViewOutline = Line.CreateBound(xyz4, xyz);
            ViewOutLineInfo item = new ViewOutLineInfo(lnViewOutline, OutLineSide.eBottom);
            this.m_lstViewOutLineInfo.Add(item);
            Line lnViewOutline2 = Line.CreateBound(xyz, xyz2);
            ViewOutLineInfo item2 = new ViewOutLineInfo(lnViewOutline2, OutLineSide.eLeft);
            this.m_lstViewOutLineInfo.Add(item2);
            Line lnViewOutline3 = Line.CreateBound(xyz2, xyz3);
            ViewOutLineInfo item3 = new ViewOutLineInfo(lnViewOutline3, OutLineSide.eUp);
            this.m_lstViewOutLineInfo.Add(item3);
            Line lnViewOutline4 = Line.CreateBound(xyz3, xyz4);
            ViewOutLineInfo item4 = new ViewOutLineInfo(lnViewOutline4, OutLineSide.eRight);
            this.m_lstViewOutLineInfo.Add(item4);
            return CalcGridInfoResult.eSucceded;
        }

        //获取所有轴网信息
		private void GetAllGridInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(this.m_doc, this.m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(Grid));
			foreach (Element element in filteredElementCollector)
			{
				Grid grid = element as Grid;
				if (grid != null)
				{
					DrawElement item = new DrawElement(grid, ElemType.eGrid);
					this.m_lstDrawElems.Add(item);
				}
			}
		}

        //获取所有链接模型轴网信息
		private void GetAllGridInfoInLinkDoc()
		{
			foreach (RevitLinkInstance revitLinkInstance in this.m_lstLinkInst)
			{
				Document linkDocument = revitLinkInstance.GetLinkDocument();
				if (linkDocument != null && this.m_doc.ActiveView.ViewType != ViewType.Section)
				{
					try
					{
						FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, this.m_doc.ActiveView.Id);
						filteredElementCollector.OfClass(typeof(Grid));
						foreach (Element element in filteredElementCollector)
						{
							Grid grid = element as Grid;
							if (grid != null)
							{
								DrawElement item = new DrawElement(grid, ElemType.eGrid);
								this.m_lstDrawElems.Add(item);
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		//获取多段轴网信息
		private void GetAllMultiSegmentGridInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(this.m_doc, this.m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(MultiSegmentGrid));
			foreach (Element element in filteredElementCollector)
			{
				MultiSegmentGrid multiSegmentGrid = element as MultiSegmentGrid;
				if (multiSegmentGrid != null)
				{
					ICollection<ElementId> gridIds = multiSegmentGrid.GetGridIds();
					foreach (ElementId elementId in gridIds)
					{
						Grid grid = this.m_doc.GetElement(elementId) as Grid;
						if (grid != null)
						{
							DrawElement item = new DrawElement(grid, ElemType.eMultiSegGrid);
							this.m_lstDrawElems.Add(item);
						}
					}
				}
			}
		}

		// 获取链接模型中多段轴网信息
		private void GetAllMultiSegmentGridInfoInLinkDoc()
		{
			foreach (RevitLinkInstance revitLinkInstance in this.m_lstLinkInst)
			{
				Document linkDocument = revitLinkInstance.GetLinkDocument();
                if (linkDocument != null && this.m_doc.ActiveView.ViewType != ViewType.Section)
				{
					try
					{
						FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, this.m_uiDoc.ActiveView.Id);
						filteredElementCollector.OfClass(typeof(MultiSegmentGrid));
						foreach (Element element in filteredElementCollector)
						{
							MultiSegmentGrid multiSegmentGrid = element as MultiSegmentGrid;
							if (multiSegmentGrid != null)
							{
								ICollection<ElementId> gridIds = multiSegmentGrid.GetGridIds();
								foreach (ElementId elementId in gridIds)
								{
									Grid grid = this.m_doc.GetElement(elementId) as Grid;
									if (grid != null)
									{
										DrawElement item = new DrawElement(grid, ElemType.eMultiSegGrid);
										this.m_lstDrawElems.Add(item);
									}
								}
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

		//获取所有楼层信息
		private void GetAllLevelInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(this.m_doc, this.m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(Level));
			foreach (Element element in filteredElementCollector)
			{
				Level level = element as Level;
				if (level != null)
				{
					DrawElement item = new DrawElement(level, ElemType.eLevel);
					this.m_lstDrawElems.Add(item);
				}
			}
		}

		// 获取链接模型中所有楼层信息
		private void GetAllLevelInfoInLinkDoc()
		{
			foreach (RevitLinkInstance revitLinkInstance in this.m_lstLinkInst)
			{
				Document linkDocument = revitLinkInstance.GetLinkDocument();
				if (linkDocument != null)
				{
					try
					{
						FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, this.m_uiDoc.ActiveView.Id);
						filteredElementCollector.OfClass(typeof(Level));
						foreach (Element element in filteredElementCollector)
						{
							Level level = element as Level;
							if (level != null)
							{
								DrawElement item = new DrawElement(level, ElemType.eLevel);
								this.m_lstDrawElems.Add(item);
							}
						}
					}
					catch (Exception)
					{
					}
				}
			}
		}

        //获取轴网曲线
        public static Curve GetGridCurve(View view, Grid grid)
        {
            if (grid == null)
            {
                return null;
            }
            Curve result;
            try
            {
                IList<Curve> curvesInView = grid.GetCurvesInView(DatumExtentType.ViewSpecific, view);
                if (curvesInView.Count <= 0)
                {
                    result = null;
                }
                else
                {
                    result = curvesInView[0];
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        //获取楼层曲线
        public static Curve GetLevelCurve(View view, Level level)
        {
            if (level == null)
            {
                return null;
            }
            Curve result;
            try
            {
                IList<Curve> curvesInView = level.GetCurvesInView(DatumExtentType.ViewSpecific, view);
                if (curvesInView.Count <= 0)
                {
                    result = null;
                }
                else
                {
                    result = curvesInView[0];
                }
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }
        // 获取插入点和方向
		private void GetIntersectPtAndDirection()
		{
			foreach (ViewOutLineInfo viewOutLineInfo in this.m_lstViewOutLineInfo)
			{
				foreach (DrawElement drawElement in this.m_lstDrawElems)
				{
					Curve curve = null;
					if (drawElement.DrawElemType == ElemType.eGrid || drawElement.DrawElemType == ElemType.eMultiSegGrid)
					{
						Grid grid = drawElement.DrawElem as Grid;
                        if (grid == null || (drawElement.DrawElemType == ElemType.eMultiSegGrid && (this.m_doc.ActiveView.ViewType == ViewType.Section || this.m_doc.ActiveView.ViewType == ViewType.Elevation)))
						{
							continue;
						}
						curve = GetGridCurve(this.m_doc.ActiveView, grid);
					}
					else if (drawElement.DrawElemType == ElemType.eLevel)
					{
						Level level = drawElement.DrawElem as Level;
						if (level == null)
						{
							continue;
						}
						curve = GetLevelCurve(this.m_doc.ActiveView, level);
					}
					if (!(curve == null))
					{
						Curve curve2;
						if (curve is Arc)
						{
							System.Drawing.Point point = this.Revit2Screen(curve.GetEndPoint(0));
							System.Drawing.Point point2 = this.Revit2Screen(curve.GetEndPoint(1));
                            System.Drawing.Point point3 = this.Revit2Screen(curve.Evaluate(0.5, true));
							XYZ xyz = new XYZ((double)point.X, (double)point.Y, 0.0);
							XYZ xyz2 = new XYZ((double)point2.X, (double)point2.Y, 0.0);
							XYZ xyz3 = new XYZ((double)point3.X, (double)point3.Y, 0.0);
							curve2 = Arc.Create(xyz, xyz2, xyz3);
						}
						else
						{
                            System.Drawing.Point point4 = this.Revit2Screen(curve.GetEndPoint(0));
                            System.Drawing.Point point5 = this.Revit2Screen(curve.GetEndPoint(1));
							XYZ xyz4 = new XYZ((double)point4.X, (double)point4.Y, 0.0);
							XYZ xyz5 = new XYZ((double)point5.X, (double)point5.Y, 0.0);
							curve2 = Line.CreateBound(xyz4, xyz5);
						}
						IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
						SetComparisonResult setComparisonResult = viewOutLineInfo.ViewOutLine.Intersect(curve2, out intersectionResultArray);
                        if (setComparisonResult == SetComparisonResult.Overlap && intersectionResultArray != null && intersectionResultArray.Size > 0)
						{
							for (int i = 0; i < intersectionResultArray.Size; i++)
							{
								GridNumShowInfo gridNumShowInfo = new GridNumShowInfo();
								gridNumShowInfo.IntersectPoint = intersectionResultArray.get_Item(i).XYZPoint;
								gridNumShowInfo.OutlineSide = viewOutLineInfo.OutlineSide;
								gridNumShowInfo.ElemText = drawElement.DrawElem.Name;
								gridNumShowInfo.ElemClass = drawElement.DrawElemType;
								this.m_lstGridNumShowInfo.Add(gridNumShowInfo);
							}
						}
					}
				}
			}
		}

		// 转换插入点   文字圆圈点修正
		public void TranformIntersectPt(IntPtr hFormClient)
		{
			try
			{
				foreach (GridNumShowInfo gridNumShowInfo in this.m_lstGridNumShowInfo)
				{
					XYZ intersectPoint = gridNumShowInfo.IntersectPoint;
					if (intersectPoint != null)
					{
						XYZ cirPt = null;
						XYZ textPt = null;
// 						switch (gridNumShowInfo.OutlineSide)
// 						{
// 						case OutLineSide.eUp:
//                             textPt = intersectPoint + XYZ.BasisY * 20;
//                             cirPt = intersectPoint - XYZ.BasisX * 20;
// 							break;
// 						case OutLineSide.eLeft:
//                             textPt = intersectPoint + XYZ.BasisX * 5.0;
//                             cirPt = intersectPoint - XYZ.BasisY * 20;
// 							break;
// 						case OutLineSide.eRight:
//                             textPt = intersectPoint - XYZ.BasisX * 5.0;
//                             cirPt = intersectPoint - XYZ.BasisY * 20 - XYZ.BasisX * 42;
// 							break;
// 						case OutLineSide.eBottom:
//                             textPt = intersectPoint - XYZ.BasisY * 20;
//                             cirPt = intersectPoint - XYZ.BasisY * 42 - XYZ.BasisX * 20;
// 							break;
// 						}
                        switch (gridNumShowInfo.OutlineSide)
                        {
                            case OutLineSide.eUp:
                                textPt = intersectPoint + XYZ.BasisX * 1 + XYZ.BasisY * 20;
                                cirPt = textPt - XYZ.BasisX * 12 - XYZ.BasisY * 10;
                                break;
                            case OutLineSide.eLeft:
                                textPt = intersectPoint + XYZ.BasisX * 5.0;
                                cirPt = textPt - XYZ.BasisX * 2 - XYZ.BasisY * 11;
                                break;
                            case OutLineSide.eRight:
                                textPt = intersectPoint - XYZ.BasisX * 5.0;
                                cirPt = textPt -XYZ.BasisX * 18 - XYZ.BasisY * 11;
                                break;
                            case OutLineSide.eBottom:
                                textPt = intersectPoint + XYZ.BasisX * 1 - XYZ.BasisY * 20;
                                cirPt = textPt - XYZ.BasisX * 12 - XYZ.BasisY * 10;
                                break;
                        }
                        System.Drawing.Point cirlLocation = new System.Drawing.Point((int)cirPt.X , (int)cirPt.Y );
						GridNumberShowManager.ScreenToClient(hFormClient, ref cirlLocation);
						gridNumShowInfo.CirlLocation = cirlLocation;
                        System.Drawing.Point textLocation = new System.Drawing.Point((int)textPt.X, (int)textPt.Y);
						GridNumberShowManager.ScreenToClient(hFormClient, ref textLocation);
						gridNumShowInfo.TextLocation = textLocation;
					}
				}
			}
			catch
			{
			}
		}





	}
}
