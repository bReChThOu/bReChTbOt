using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace bReChTbOt.Map
{
	public class ArmyTransfer
	{
		/// <summary>
		/// Gets or sets the armies.
		/// </summary>
		/// <value>
		/// The armies.
		/// </value>
		public int Armies { get; set; }


		/// <summary>
		/// Gets or sets the source region.
		/// </summary>
		/// <value>
		/// The source region.
		/// </value>
		public Region SourceRegion { get; set; }


		/// <summary>
		/// Gets or sets the target region.
		/// </summary>
		/// <value>
		/// The target region.
		/// </value>
		public Region TargetRegion { get; set; }
	}
}
