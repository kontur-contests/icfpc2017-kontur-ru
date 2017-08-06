using System.Drawing;

namespace lib.viz
{
	public interface IScenePainter
	{
		SizeF Size { get; }
		void Paint(Graphics g, PointF mouseLogicalPos, RectangleF paintRect, float zoomScale);
	}
}