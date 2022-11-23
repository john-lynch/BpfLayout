using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BpfLayout
{
	public class Thickness
	{
		public Thickness()
		{

		}

		public Thickness(double margin)
		{
			Top = Left = Bottom = Right = margin;
		}

		public Thickness(double horizontal, double vertical)
		{
			Left = Right = horizontal;
			Top = Bottom = vertical;
		}

		public Thickness(double left, double top, double right, double bottom)
		{
			Left = left;
			Top = top;
			Right = right;
			Bottom = bottom;
		}

		public double Top { get; set; } = 0.0f;
		public double Left { get; set; } = 0.0f;
		public double Bottom { get; set; } = 0.0f;
		public double Right { get; set; } = 0.0f;
	}
}
