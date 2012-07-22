#region Using Statements

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;

#endregion

namespace Pictural
{
	public class Renamer
	{
		#region Fields

		private List<Picture> fileList;

		private Stopwatch stopWatch;

		private int countFolders;
		private int countPictures;

		private int valueTimezone;

		#endregion

		#region Constructor

		public Renamer(string folderPath, string outputPath, int valueTimezone)
		{
			fileList = new List<Picture>();

			this.valueTimezone = valueTimezone;

			// Start a stopwatch to time the entire operation.

			stopWatch = Stopwatch.StartNew();

			ProcessFolder(folderPath, outputPath);
			ProcessOutputLog();

			stopWatch.Stop();

			// Show the result of the operation and clear the pictures from memory.

			string message = string.Format("{0} pictures processed from {1} folders in {2} ms.", countPictures, countFolders, stopWatch.ElapsedMilliseconds);
			MessageBox.Show(message, "Ready", MessageBoxButton.OK, MessageBoxImage.Information);

			fileList.Clear();
		}

		#endregion

		#region Helper Methods

		/// <summary>
		/// Processes the pictures within a folder and recursively processes sub-folders.
		/// </summary>
		/// <param name="pathSource">The path of the folder to process.</param>
		/// <param name="pathTarget">The output path for the processed pictures.</param>
		private void ProcessFolder(string folderPath, string outputPath)
		{
			countFolders++;

			try
			{
				foreach (string folder in Directory.EnumerateDirectories(folderPath))
				{
					ProcessFolder(folder, outputPath);
				}

				if (Directory.GetFiles(folderPath, "*.jpg").Length > 0)
				{
					ProcessPictures(folderPath, outputPath);
				}
			}
			catch (DirectoryNotFoundException ex)
			{
				stopWatch.Stop();
				MessageBox.Show(ex.Message, "There was a problem organizing your photos.", MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}

		/// <summary>
		/// Renames every picture from a folder using its metadata.
		/// </summary>
		/// <param name="pathSource">The path of the folder to process.</param>
		/// <param name="pathTarget">The output path for the processed pictures.</param>
		private void ProcessPictures(string folderPath, string outputPath)
		{
			foreach (string filePath in Directory.EnumerateFiles(folderPath, "*.jpg"))
			{
				countPictures++;

				// If we can't get the required data from the picture, default to another value.

				string subFolderName = "Unknown";
				string pictureName = "Unknown";

				string subFolderPath = string.Empty;
				DateTime dateTaken = DateTime.MinValue;

				// Get the metadata from the picture.

				using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
				{
					BitmapMetadata metaData = (BitmapMetadata)BitmapFrame.Create(fileStream).Metadata;
					DateTime.TryParse(metaData.DateTaken, out dateTaken);
				}

				// When we have a valid date from the metadata, set the folder and picture name.

				if (dateTaken != DateTime.MinValue)
				{
					// If we have specified a timezone value, change the dateTaken accordingly.

					dateTaken = dateTaken.AddHours(valueTimezone);

					// Set the folder name to the date.

					subFolderName = dateTaken.ToString("yyyy-MM-dd");

					// Set the picture name to the date and time.

					pictureName = dateTaken.ToString("yyyy-MM-dd HH-mm-ss tt");
				}

				subFolderPath = String.Format("{0}/{1}", outputPath, subFolderName);

				if (!Directory.Exists(subFolderPath))
				{
					Directory.CreateDirectory(subFolderPath);
				}

				string photoFilename = String.Format("{0}{1}", pictureName, ".jpg");
				string photoRelocation = String.Format("{0}/{1}", subFolderPath, photoFilename);

				fileList.Add(new Picture(filePath, photoRelocation));

				try
				{
					File.Copy(filePath, photoRelocation);
				}
				catch (IOException ex)
				{
					stopWatch.Stop();
					MessageBox.Show(ex.Message, "There was an error copying a photo.", MessageBoxButton.OK, MessageBoxImage.Error);
				}
			}
		}

		/// <summary>
		/// Store every processed filename in a log file.
		/// </summary>
		private void ProcessOutputLog()
		{
			StreamWriter log;

			string logFilename = "logfile.txt";

			if (!File.Exists(logFilename))
			{
				log = new StreamWriter(logFilename);
			}
			else
			{
				log = File.AppendText(logFilename);
			}

			foreach (Picture rename in fileList)
			{
				log.WriteLine();
				log.WriteLine(rename.FilenameOriginal);
				log.WriteLine(rename.FilenameCopy);
			}

			log.Close();
		}

		#endregion
	}
}
