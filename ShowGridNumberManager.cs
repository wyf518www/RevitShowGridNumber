using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace ShowGridNumber
{

	public class ShowGridNumberManager
	{
        public List<GridNumShowInfo> m_GridNumShowInfos = new List<GridNumShowInfo>();
        private List<RevitLinkInstance> m_lstLinkInst = new List<RevitLinkInstance>();
        private List<OutViewInfo> m_OutViewInfos = new List<OutViewInfo>();
        private List<DrawElement> m_DrawElems = new List<DrawElement>();
        private UIDocument m_uiDoc;
        private Document m_doc;
        private UIView m_uiView;
        private int m_nLeft;
        private int m_nRight;
        private int m_nTop;
        private int m_nBottom;

		public ShowGridNumberManager(UIDocument uiDoc)
		{
			m_uiDoc = uiDoc;
			m_doc = uiDoc.Document;
		}
        
		public void RefreshUIDocument(UIDocument uiDoc)
		{
			m_uiDoc = uiDoc;
			m_doc = uiDoc.Document;
			m_uiView = null;
			m_OutViewInfos.Clear();
			m_DrawElems.Clear();
			m_GridNumShowInfos.Clear();
		}
        //把在屏幕上鼠标的位置转换为打开的程序的客户区的坐标（位置）。
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern bool ScreenToClient(IntPtr hWnd, ref System.Drawing.Point pt);
        //revit点坐标转换为屏幕坐标
        public System.Drawing.Point Revit2Screen(XYZ point)
        {
            try
            {
                int m_left = 0;
                int m_top = 0;
                int m_right = 0;
                int m_bottom = 0;
                GetRevitWndRectangle(m_uiView, ref m_left, ref m_top, ref m_right, ref m_bottom);
                //裁切框
                Transform inverseTrans = m_uiDoc.ActiveView.CropBox.Transform.Inverse;
                //两个角点
                IList<XYZ> zoomCorners = m_uiView.GetZoomCorners();
                XYZ tansPt = inverseTrans.OfPoint(point);
                XYZ transCornerPt0 = inverseTrans.OfPoint(zoomCorners[0]);
                XYZ transCornerPt1 = inverseTrans.OfPoint(zoomCorners[1]);
                double scale = (double)(m_right - m_left) / (transCornerPt1.X - transCornerPt0.X);
                int x = m_left + (int)((tansPt.X - transCornerPt0.X) * scale);
                int y = m_top + (int)((transCornerPt1.Y - tansPt.Y) * scale);
                return new System.Drawing.Point(x, y);
            }
            catch
            {
            }
            return System.Drawing.Point.Empty;
        }

        //获取轴号信息
		public CalcGridInfoResult GetGridNumberShowInfo(bool bIdling, Rect formRec, ref System.Drawing.Point formLeftTopPt, ref Size formSize)
		{
			if (m_uiDoc == null)
			{
				return CalcGridInfoResult._Error;
			}
			if (m_uiView == null)
			{
				IList<UIView> openUIViews = m_uiDoc.GetOpenUIViews();
				foreach (UIView uiview in openUIViews)
				{
					if (uiview.ViewId == m_uiDoc.ActiveView.Id)
					{
						m_uiView = uiview;
						break;
					}
				}
			}
			if (m_uiView == null)
			{
				return CalcGridInfoResult._Error;
			}
			GetLinkInstances();
			m_OutViewInfos.Clear();
			m_DrawElems.Clear();
			m_GridNumShowInfos.Clear();
			CalcGridInfoResult result = GetCurrentViewOutLine(bIdling, formRec, ref formSize, ref formLeftTopPt);
			if (result != CalcGridInfoResult._Succeded)
			{
				return result;
			}
			GetAllGridInfo();
			GetAllMultiSegmentGridInfo();
			GetAllLevelInfo();
			GetAllLevelInfoInLinkDoc();
			if (m_DrawElems.Count <= 0)
			{
				return CalcGridInfoResult._Error;
			}
			GetIntersectPtAndDirection();
			if (m_GridNumShowInfos.Count <= 0)
			{
				return CalcGridInfoResult._Error;
			}
			return CalcGridInfoResult._Succeded;
		}
        #region 带误差判断大小方法
        //判断double是否近似相等
        public static bool IsEqual(double d1, double d2)
        {
            return !IsLess(d1, d2) && !IsLess(d2, d1);
        }
        //判断double大小  误差值-1E-09
        public static bool IsLess(double d1, double d2)
        {
            return d1 - d2 < -1E-09;
        }
        //判断double大小  误差值-1E-09
        public static bool IsGreater(double d1, double d2)
        {
            return d1 - d2 > 1E-09;
        }
        #endregion
        //获取revit窗口
        public static void GetRevitWndRectangle(UIView view, ref int left, ref int top, ref int right, ref int bottom)
        {
            Autodesk.Revit.UI.Rectangle windowRectangle = view.GetWindowRectangle();
            left = windowRectangle.Left;
            top = windowRectangle.Top;
            right = windowRectangle.Right;
            bottom = windowRectangle.Bottom;
        }
        //获取当前视图的外框线
        private CalcGridInfoResult GetCurrentViewOutLine(bool bIdling, Rect formRec, ref Size formSize, ref System.Drawing.Point formLeftTopPt)
        {
            int m_left = 0;
            int m_right = 0;
            int m_top = 0;
            int m_bottom = 0;
            GetRevitWndRectangle(m_uiView, ref m_left, ref m_top, ref m_right, ref m_bottom);
            if (IsEqual((double)m_left, (double)m_right) || IsEqual((double)m_top, (double)m_bottom))
            {
                return CalcGridInfoResult._Min;
            }
            if (bIdling &&
                IsEqual((double)m_left, (double)m_nLeft) &&
                IsEqual((double)m_top, (double)m_nTop) &&
                IsEqual((double)m_right, (double)m_nRight) &&
                IsEqual((double)m_bottom, (double)m_nBottom))
            {
                return CalcGridInfoResult._Idling;
            }
            m_nLeft = m_left;
            m_nRight = m_right;
            m_nTop = m_top;
            m_nBottom = m_bottom;
            if (IsGreater((double)formRec.left, (double)m_left))
            {
                m_left = formRec.left;
            }
            if (IsLess((double)formRec.right, (double)m_right))
            {
                m_right = formRec.right;
            }
            if (IsGreater((double)formRec.top, (double)m_top))
            {
                m_top = formRec.top;
            }
            if (IsLess((double)formRec.bottom, (double)m_bottom))
            {
                m_bottom = formRec.bottom;
            }
            XYZ ptLeftBottom = new XYZ((double)m_left, (double)m_bottom, 0.0);
            XYZ ptLeftTop = new XYZ((double)m_left, (double)m_top, 0.0);
            XYZ ptRightTop = new XYZ((double)m_right, (double)m_top, 0.0);
            XYZ ptRightBottom = new XYZ((double)m_right, (double)m_bottom, 0.0);
            formSize.Width = m_right - m_left;
            formSize.Height = m_bottom - m_top;
            formLeftTopPt.X = m_left;
            formLeftTopPt.Y = m_top;
            Line bottomLine = Line.CreateBound(ptRightBottom, ptLeftBottom);
            OutViewInfo infoBottom = new OutViewInfo(bottomLine, OutViewSide._Bottom);
            m_OutViewInfos.Add(infoBottom);
            Line leftLine = Line.CreateBound(ptLeftBottom, ptLeftTop);
            OutViewInfo infoLeft = new OutViewInfo(leftLine, OutViewSide._Left);
            m_OutViewInfos.Add(infoLeft);
            Line topLine = Line.CreateBound(ptLeftTop, ptRightTop);
            OutViewInfo infoTop = new OutViewInfo(topLine, OutViewSide._Up);
            m_OutViewInfos.Add(infoTop);
            Line rightLine = Line.CreateBound(ptRightTop, ptRightBottom);
            OutViewInfo infoRight = new OutViewInfo(rightLine, OutViewSide._Right);
            m_OutViewInfos.Add(infoRight);
            return CalcGridInfoResult._Succeded;
        }

        //获取所有轴网信息
		private void GetAllGridInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc, m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(Grid));
			foreach (Element element in filteredElementCollector)
			{
				Grid grid = element as Grid;
				if (grid != null)
				{
					DrawElement item = new DrawElement(grid, ElemType._Grid);
					m_DrawElems.Add(item);
				}
			}
		}
        //获取多段轴网信息
		private void GetAllMultiSegmentGridInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc, m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(MultiSegmentGrid));
			foreach (Element element in filteredElementCollector)
			{
				MultiSegmentGrid multiSegmentGrid = element as MultiSegmentGrid;
				if (multiSegmentGrid != null)
				{
					ICollection<ElementId> gridIds = multiSegmentGrid.GetGridIds();
					foreach (ElementId elementId in gridIds)
					{
						Grid grid = m_doc.GetElement(elementId) as Grid;
						if (grid != null)
						{
							DrawElement item = new DrawElement(grid, ElemType._MultiSegGrid);
							m_DrawElems.Add(item);
						}
					}
				}
			}
		}
		//获取所有标高
		private void GetAllLevelInfo()
		{
			FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc, m_uiDoc.ActiveView.Id);
			filteredElementCollector.OfClass(typeof(Level));
			foreach (Element element in filteredElementCollector)
			{
				Level level = element as Level;
				if (level != null)
				{
					DrawElement item = new DrawElement(level, ElemType._Level);
					m_DrawElems.Add(item);
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
        #region 链接模型信息
        //获取当前文档中链接模型实例
        private void GetLinkInstances()
        {
            m_lstLinkInst.Clear();
            FilteredElementCollector filteredElementCollector = new FilteredElementCollector(m_doc).OfClass(typeof(RevitLinkInstance));
            ICollection<ElementId> collection = filteredElementCollector.ToElementIds();
            foreach (ElementId elementId in collection)
            {
                Element element = m_doc.GetElement(elementId);
                if (!(element.GetType() != typeof(RevitLinkInstance)))
                {
                    RevitLinkInstance revitLinkInstance = element as RevitLinkInstance;
                    if (revitLinkInstance != null)
                    {
                        m_lstLinkInst.Add(revitLinkInstance);
                    }
                }
            }
        }
        //获取所有链接模型轴网信息
        private void GetAllGridInfoInLinkDoc()
        {
            foreach (RevitLinkInstance revitLinkInstance in m_lstLinkInst)
            {
                Document linkDocument = revitLinkInstance.GetLinkDocument();
                if (linkDocument != null && m_doc.ActiveView.ViewType != ViewType.Section)
                {
                    try
                    {
                        FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, m_doc.ActiveView.Id);
                        filteredElementCollector.OfClass(typeof(Grid));
                        foreach (Element element in filteredElementCollector)
                        {
                            Grid grid = element as Grid;
                            if (grid != null)
                            {
                                DrawElement item = new DrawElement(grid, ElemType._Grid);
                                m_DrawElems.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        // 获取链接模型中多段轴网信息
        private void GetAllMultiSegmentGridInfoInLinkDoc()
        {
            foreach (RevitLinkInstance revitLinkInstance in m_lstLinkInst)
            {
                Document linkDocument = revitLinkInstance.GetLinkDocument();
                if (linkDocument != null && m_doc.ActiveView.ViewType != ViewType.Section)
                {
                    try
                    {
                        FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, m_uiDoc.ActiveView.Id);
                        filteredElementCollector.OfClass(typeof(MultiSegmentGrid));
                        foreach (Element element in filteredElementCollector)
                        {
                            MultiSegmentGrid multiSegmentGrid = element as MultiSegmentGrid;
                            if (multiSegmentGrid != null)
                            {
                                ICollection<ElementId> gridIds = multiSegmentGrid.GetGridIds();
                                foreach (ElementId elementId in gridIds)
                                {
                                    Grid grid = m_doc.GetElement(elementId) as Grid;
                                    if (grid != null)
                                    {
                                        DrawElement item = new DrawElement(grid, ElemType._MultiSegGrid);
                                        m_DrawElems.Add(item);
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
        // 获取链接模型中所有标高
        private void GetAllLevelInfoInLinkDoc()
        {
            foreach (RevitLinkInstance revitLinkInstance in m_lstLinkInst)
            {
                Document linkDocument = revitLinkInstance.GetLinkDocument();
                if (linkDocument != null)
                {
                    try
                    {
                        FilteredElementCollector filteredElementCollector = new FilteredElementCollector(linkDocument, m_uiDoc.ActiveView.Id);
                        filteredElementCollector.OfClass(typeof(Level));
                        foreach (Element element in filteredElementCollector)
                        {
                            Level level = element as Level;
                            if (level != null)
                            {
                                DrawElement item = new DrawElement(level, ElemType._Level);
                                m_DrawElems.Add(item);
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }
        #endregion
        #region 计算轴号显示位置
        // 获取插入点和方向
		private void GetIntersectPtAndDirection()
		{
			foreach (OutViewInfo viewOutLineInfo in m_OutViewInfos)
			{
				foreach (DrawElement drawElement in m_DrawElems)
				{
					Curve curve = null;
					if (drawElement.DrawElemType == ElemType._Grid || 
                        drawElement.DrawElemType == ElemType._MultiSegGrid)
					{
						Grid grid = drawElement.DrawElem as Grid;
                        if (grid == null || 
                            (drawElement.DrawElemType == ElemType._MultiSegGrid && 
                                 (m_doc.ActiveView.ViewType == ViewType.Section || 
                                    m_doc.ActiveView.ViewType == ViewType.Elevation)))
						{
							continue;
						}
						curve = GetGridCurve(m_doc.ActiveView, grid);
					}
					else if (drawElement.DrawElemType == ElemType._Level)
					{
						Level level = drawElement.DrawElem as Level;
						if (level == null)
						{
							continue;
						}
						curve = GetLevelCurve(m_doc.ActiveView, level);
					}
					if (!(curve == null))
					{
                        Curve curve2D = CurveToZero(curve);
                        //求边界线和轴网/标高的交点
						IntersectionResultArray intersectionResultArray = new IntersectionResultArray();
						SetComparisonResult setComparisonResult = viewOutLineInfo.OutViewLine.Intersect(curve2D, out intersectionResultArray);
                        if (setComparisonResult == SetComparisonResult.Overlap && 
                            intersectionResultArray != null && 
                            intersectionResultArray.Size > 0)
						{
							for (int i = 0; i < intersectionResultArray.Size; i++)
							{
								GridNumShowInfo gridNumShowInfo = new GridNumShowInfo();
								gridNumShowInfo.IntersectPoint = intersectionResultArray.get_Item(i).XYZPoint;
								gridNumShowInfo.OutlineSide = viewOutLineInfo.OutViewSide;
								gridNumShowInfo.ElemText = drawElement.DrawElem.Name;
								gridNumShowInfo.ElemClass = drawElement.DrawElemType;
								m_GridNumShowInfos.Add(gridNumShowInfo);
							}
						}
					}
				}
			}
		}
        //曲线标高归0
        private Curve CurveToZero(Curve curve)
        {
            Curve curve2D;
            if (curve is Arc)
            {
                System.Drawing.Point startPt = Revit2Screen(curve.GetEndPoint(0));
                System.Drawing.Point endPt = Revit2Screen(curve.GetEndPoint(1));
                System.Drawing.Point midPt = Revit2Screen(curve.Evaluate(0.5, true));
                XYZ startPt2D = new XYZ((double)startPt.X, (double)startPt.Y, 0.0);
                XYZ endPt2D = new XYZ((double)endPt.X, (double)endPt.Y, 0.0);
                XYZ midPt2D = new XYZ((double)midPt.X, (double)midPt.Y, 0.0);
                curve2D = Arc.Create(startPt2D, endPt2D, midPt2D);
            }
            else
            {
                System.Drawing.Point screenStartPt = Revit2Screen(curve.GetEndPoint(0));
                System.Drawing.Point screenEndPt = Revit2Screen(curve.GetEndPoint(1));
                XYZ screenStartPt2D = new XYZ((double)screenStartPt.X, (double)screenStartPt.Y, 0.0);
                XYZ screenEndPt2D = new XYZ((double)screenEndPt.X, (double)screenEndPt.Y, 0.0);
                curve2D = Line.CreateBound(screenStartPt2D, screenEndPt2D);
            }
            return curve2D;
        }

		// 转换插入点   文字圆圈位置修正
		public void TranformIntersectPt(IntPtr hFormClient)
		{
			try
			{
				foreach (GridNumShowInfo gridNumShowInfo in m_GridNumShowInfos)
				{
					XYZ intersectPoint = gridNumShowInfo.IntersectPoint;
					if (intersectPoint != null)
					{
						XYZ cirPt = null;
						XYZ textPt = null;
// 						switch (gridNumShowInfo.OutViewSide)
// 						{
// 						case OutViewSide._Up:
//                             textPt = intersectPoint + XYZ.BasisY * 20;
//                             cirPt = intersectPoint - XYZ.BasisX * 20;
// 							break;
// 						case OutViewSide._Left:
//                             textPt = intersectPoint + XYZ.BasisX * 5.0;
//                             cirPt = intersectPoint - XYZ.BasisY * 20;
// 							break;
// 						case OutViewSide._Right:
//                             textPt = intersectPoint - XYZ.BasisX * 5.0;
//                             cirPt = intersectPoint - XYZ.BasisY * 20 - XYZ.BasisX * 42;
// 							break;
// 						case OutViewSide._Bottom:
//                             textPt = intersectPoint - XYZ.BasisY * 20;
//                             cirPt = intersectPoint - XYZ.BasisY * 42 - XYZ.BasisX * 20;
// 							break;
// 						}
                        float fontSize = ShowGridApplication.m_gridNumberShowForm.fontSize;
                        float circleRadius = (fontSize + 5)/2;
                        int margin = 3;
                        double marginX = fontSize / 5 * 2.5;
                        switch (gridNumShowInfo.OutlineSide)
                        {
                            case OutViewSide._Up:
                                textPt = intersectPoint + XYZ.BasisY * (fontSize / 2 + margin);
                                /*cirPt = intersectPoint - XYZ.BasisX * (circleRadius) - XYZ.BasisY * (circleRadius - (fontSize / 2 + margin));*/
                                cirPt = textPt - XYZ.BasisX * circleRadius - XYZ.BasisY * circleRadius;
                                break;
                            case OutViewSide._Left:
                                textPt = intersectPoint + XYZ.BasisX * margin;
                                /*cirPt = intersectPoint / *- XYZ.BasisX * 2 - XYZ.BasisY * 11* /;*/
                                cirPt = textPt - XYZ.BasisX * circleRadius+XYZ.BasisX*marginX - XYZ.BasisY * circleRadius;
                                break;
                            case OutViewSide._Right:
                                textPt = intersectPoint - XYZ.BasisX * margin;
                                /*cirPt = intersectPoint - XYZ.BasisX * (circleRadius * 2) + XYZ.BasisY * ((fontSize / 2 + margin));*/
                                cirPt = textPt - XYZ.BasisX * circleRadius-XYZ.BasisX*marginX - XYZ.BasisY * circleRadius;
                                break;
                            case OutViewSide._Bottom:
                                textPt = intersectPoint - XYZ.BasisY * (fontSize / 2 + margin);
                                cirPt = textPt-XYZ.BasisX*circleRadius-XYZ.BasisY*circleRadius;
                                break;
                        }
                        System.Drawing.Point cirlLocation = new System.Drawing.Point((int)cirPt.X , (int)cirPt.Y );
						ShowGridNumberManager.ScreenToClient(hFormClient, ref cirlLocation);
						gridNumShowInfo.CirlLocation = cirlLocation;
                        System.Drawing.Point textLocation = new System.Drawing.Point((int)textPt.X, (int)textPt.Y);
						ShowGridNumberManager.ScreenToClient(hFormClient, ref textLocation);
						gridNumShowInfo.TextLocation = textLocation;
					}
				}
			}
			catch
			{
			}
        }
        #endregion




    }
}
