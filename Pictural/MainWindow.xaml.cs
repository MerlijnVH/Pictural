#region Using Statements

using System.Windows;
using System.Windows.Forms;

#endregion

namespace Pictural
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region Constructor

		public MainWindow()
		{
			InitializeComponent();
		}

		#endregion

		#region Event Handlers

		private void buttonFolder_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				dialog.ShowDialog();
				textBoxPathSource.Text = dialog.SelectedPath;
			}
		}

		private void buttonOutput_Click(object sender, RoutedEventArgs e)
		{
			using (FolderBrowserDialog dialog = new FolderBrowserDialog())
			{
				dialog.ShowDialog();
				textBoxPathTarget.Text = dialog.SelectedPath;
			}
		}

		private void buttonOrganize_Click(object sender, RoutedEventArgs e)
		{
			string pathSource = textBoxPathSource.Text;
			string pathTarget = textBoxPathTarget.Text;

			int valueTimezone = 0;

			if (textBoxTimeValue.Text != string.Empty)
			{
				valueTimezone = int.Parse(textBoxTimeValue.Text);
			}

			if (pathSource != string.Empty && pathTarget != string.Empty)
			{
				Renamer renameProcess = new Renamer(pathSource, pathTarget, valueTimezone);
			}
			else
			{
				// There are no folders specified, we should throw a notification here.
			}
		}

		#endregion
	}
}
