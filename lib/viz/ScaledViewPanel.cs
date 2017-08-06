using System;
using System.Drawing;
using System.Windows.Forms;

namespace lib.viz
{
	public class ScaledViewPanel : Panel
	{
		private readonly IScenePainter painter;
		private PointF centerLogicalPos;
		private bool dragInProgress;
		private Point dragStart;
		private PointF dragStartCenter;
		private PointF mouseLogicalPos;
		private float zoomScale;

		public ScaledViewPanel(IScenePainter painter)
			: this()
		{
			this.painter = painter;
		}

		public ScaledViewPanel()
		{
			FitToWindow = true;
			zoomScale = 1f;
		}

		public PointF MouseLogicalPos => mouseLogicalPos;

		public PointF CenterLogicalPos
		{
			get => centerLogicalPos;
			set
			{
				centerLogicalPos = value;
				FitToWindow = false;
			}
		}

		public float ZoomScale
		{
			get => zoomScale;
			set
			{
				zoomScale = Math.Min(1000f, Math.Max(0.001f, value));
				FitToWindow = false;
			}
		}

		public bool FitToWindow { get; set; }

		protected override void InitLayout()
		{
			base.InitLayout();
			ResizeRedraw = true;
			DoubleBuffered = true;
		}

		protected override void OnMouseClick(MouseEventArgs e)
		{
			base.OnMouseClick(e);
			if (e.Button == MouseButtons.Middle)
				FitToWindow = true;
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			base.OnMouseDown(e);
			if (e.Button == MouseButtons.Right)
			{
				dragInProgress = true;
				dragStart = e.Location;
				dragStartCenter = CenterLogicalPos;
			}
		}

		protected override void OnMouseUp(MouseEventArgs e)
		{
			base.OnMouseUp(e);
			dragInProgress = false;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			base.OnMouseMove(e);
			mouseLogicalPos = ToLogical(e.Location);
			if (dragInProgress)
			{
				var loc = e.Location;
				var dx = (loc.X - dragStart.X) / ZoomScale;
				var dy = (loc.Y - dragStart.Y) / ZoomScale;
				CenterLogicalPos = new PointF(dragStartCenter.X - dx, dragStartCenter.Y - dy);
				Invalidate();
			}
			else
			{
				//Invalidate(new Rectangle(0, 0, 100, 50));
                //var loc = e.Location;
				Invalidate();
				//Invalidate(new Rectangle(loc.X - 100, loc.Y - 100, loc.X + 100, loc.Y + 100));
			}
		}

		protected override void OnMouseWheel(MouseEventArgs e)
		{
			base.OnMouseWheel(e);
			const float zoomChangeStep = 1.1f;
			if (e.Delta > 0)
				ZoomScale *= zoomChangeStep;
			if (e.Delta < 0)
				ZoomScale /= zoomChangeStep;
			Invalidate();
		}

		private PointF ToLogical(Point p)
		{
			var shift = GetShift();
			return new PointF(
				(p.X - shift.X) / zoomScale,
				(p.Y - shift.Y) / zoomScale);
		}

		private PointF GetShift()
		{
			return new PointF(
				ClientSize.Width / 2f - CenterLogicalPos.X * ZoomScale,
				ClientSize.Height / 2f - CenterLogicalPos.Y * ZoomScale);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
			e.Graphics.Clear(Color.White);
			if (painter == null) return;
			var sceneSize = painter.Size;
			if (FitToWindow)
			{
				var vMargin = sceneSize.Height * ClientSize.Width < ClientSize.Height * sceneSize.Width;
				zoomScale = vMargin
					? ClientSize.Width / sceneSize.Width
					: ClientSize.Height / sceneSize.Height;
				centerLogicalPos = new PointF(sceneSize.Width / 2, sceneSize.Height / 2);
			}

			var text = $"{mouseLogicalPos.X:#} {mouseLogicalPos.Y:#}";
			e.Graphics.DrawString(text, new Font("Arial", 8), new SolidBrush(Color.FromArgb(100, 255, 255, 255)), 5, 5);
			var shift = GetShift();
			e.Graphics.ResetTransform();
			e.Graphics.TranslateTransform(shift.X, shift.Y);
			e.Graphics.ScaleTransform(ZoomScale, ZoomScale);
            painter.Paint(e.Graphics, MouseLogicalPos, e.Graphics.VisibleClipBounds, ZoomScale);
		}
	}
}