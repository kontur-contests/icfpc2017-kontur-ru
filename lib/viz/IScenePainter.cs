using System.Drawing;

namespace lib.viz
{
	public interface IScenePainter
	{
		SizeF Size { get; }
		void Paint(Graphics g);
	}
}