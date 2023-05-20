using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace Random_Item_Giver_Updater
{

	public partial class MainWindow : Window
	{
		public ScrollViewer svWorkspace = new ScrollViewer();
		public static StackPanel stpWorkspace = new StackPanel();
		public List<itemEntry> itemEntryList = new List<itemEntry>();
		public static List<string> itemList = new List<string>();

		public MainWindow()
		{
			InitializeComponent();

			//Create workspace stack panel
			stpWorkspace.HorizontalAlignment = HorizontalAlignment.Stretch;
			stpWorkspace.VerticalAlignment = VerticalAlignment.Stretch;
			stpWorkspace.Children.Clear();

			//Create workspace scrollviewer
			svWorkspace.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
			svWorkspace.HorizontalAlignment = HorizontalAlignment.Stretch;
			svWorkspace.VerticalAlignment = VerticalAlignment.Stretch;
			svWorkspace.Content = stpWorkspace;
			svWorkspace.Background = new SolidColorBrush(Colors.DarkGray);

			//Add the ScrollViewer as the Content of the parent Window object
			Grid.SetColumn(svWorkspace, 1);
			Grid.SetRow(svWorkspace, 1);
			grdWorkspace.Children.Add(svWorkspace);
		}

		private void btnLoadDatapack_Click(object sender, RoutedEventArgs e)
		{
			var loadedItems = File.ReadLines("C:/Users/Louis/OneDrive/Dokumente/Programmieren/Minecraft Datapacks/Random Item Giver/GitHub Repository/Random-Item-Giver-Datapack/Random Item Giver 1.20/data/randomitemgiver/loot_tables/1item/main.json");
			foreach (var item in loadedItems)
			{
				if (!item.Contains("{") && !item.Contains("}") && !item.Contains("[") && !item.Contains("]") && !item.Contains("rolls") && !item.Contains("\"minecraft:item\"") && item.Contains("\""))
				{
					string itemFiltered;
					itemFiltered = item.Replace("\"", "");
					itemFiltered = itemFiltered.Replace("name:", "");
					itemFiltered = itemFiltered.Replace(" ", "");
					itemList.Add(itemFiltered);
				}

			}

			for (int i = 0; i < itemList.Count; i++)
			{
				itemEntryList.Add(new itemEntry(i));
			}

			foreach (itemEntry entry in itemEntryList)
			{
				stpWorkspace.Children.Add(entry.itemBorder);
			}
		}
	}

	public class itemEntry
	{
		//Attributes of an item slot
		public Border itemBorder = new Border();
		public Canvas itemCanvas = new Canvas();
		public TextBlock itemTextblock = new TextBlock();

		public itemEntry(int slotNumber)
		{
			//Set backcolor
			if (slotNumber % 2 == 0)
			{
				itemCanvas.Background = new SolidColorBrush(Colors.DarkGray);
			}
			else
			{
				itemCanvas.Background = new SolidColorBrush(Color.FromArgb(100, 191, 191, 191));
			}
			itemCanvas.Height = 50;

			//Create itemborder
			itemBorder.Margin = new Thickness(0, 0, 0, 0);
			itemBorder.Child = itemCanvas;
			itemBorder.HorizontalAlignment = HorizontalAlignment.Stretch;
			itemBorder.VerticalAlignment = VerticalAlignment.Top;

			//Create text
			itemTextblock.Margin = new Thickness(10, 10, 0, 0);
			itemTextblock.Text = MainWindow.itemList[slotNumber];
			itemTextblock.FontSize = 20;
			itemCanvas.Children.Add(itemTextblock);

		}

	}
}

