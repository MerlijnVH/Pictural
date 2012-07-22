namespace Pictural
{
	public struct Picture
	{
		public string FilenameOriginal;
		public string FilenameCopy;

		public Picture(string filePath, string outputPath)
		{
			FilenameOriginal = filePath;
			FilenameCopy = outputPath;
		}
	}
}
