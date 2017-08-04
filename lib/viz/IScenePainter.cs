using System.Drawing;

namespace CinemaLib
{
	public interface IScenePainter
	{
		SizeF Size { get; }
		void Paint(Graphics g);
	}
}