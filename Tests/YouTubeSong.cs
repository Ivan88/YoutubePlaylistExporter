﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tests
{
	class YouTubeSong
	{
		private string p1;
		private string p2;
		private string p3;
		private string p4;
		private string p5;
		private ulong? duration;

		public YouTubeSong(string p1, string p2, string p3, string p4, string p5, ulong? duration)
		{
			// TODO: Complete member initialization
			this.p1 = p1;
			this.p2 = p2;
			this.p3 = p3;
			this.p4 = p4;
			this.p5 = p5;
			this.duration = duration;
		}

		public string Title { get; set; }
	}
}
