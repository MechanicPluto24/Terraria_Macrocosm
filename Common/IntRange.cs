namespace Macrocosm.Common
{
	public struct IntRange
	{
		public int Start;
		public int End;

		public int Lenght => End - Start;

		public IntRange(int start, int end)
		{
			Start = start;
			End = end;
		}

		public IntRange(int end)
		{
			Start = 0;
			End = end;
		}

		public bool Contains(int value)
			=> Start <= value && value <= End;
	}
}
