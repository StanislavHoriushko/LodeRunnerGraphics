using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
using System.Timers;
using System.Runtime.Serialization.Json;
using System.Windows.Threading;

namespace LodeRunnerGraphics
{
    class MapEditor
    {
        static int x = 2;
        static int y = 2;
        static int width;
        static int height;
        static int input = -1;
        public static TextBox widthBox = new TextBox();
        public static TextBox heightBox = new TextBox();
        public static TextBox nameBox = new TextBox();
        static TextBlock Wrong = new TextBlock();
        static int NameState = 0;
        public async static Task Edit(string mapName)
        {
            int _x = 2;
            int _y = 2;
            string[,] map;
            List<string> mapList = new List<string>();
            using (StreamReader sr = new StreamReader($"maps\\{mapName}.txt", Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    mapList.Add(line);
                }
            }
            map = new string[mapList.Count, mapList[0].Length];
            int i = 0;
            foreach (string str in mapList)
            {
                for (int j = 0; j < mapList[i].Length; j++)
                {
                    map[i, j] = str[j].ToString();
                }
                i++;
            }
            height = map.GetUpperBound(0) + 1;
            width = map.GetUpperBound(1) + 1;
            Board.FieldGrid = new Grid();
            //Board.FieldGrid.ShowGridLines = true;
            for (int j = 0; j < height; j++)
            {
                Board.FieldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(29) });
            }
            for (int j = 0; j < width; j++)
            {
                Board.FieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });
            }
            Board.FieldGrid.HorizontalAlignment = HorizontalAlignment.Left;
            Board.FieldGrid.VerticalAlignment = VerticalAlignment.Top;
            Board.FieldGrid.Height = 29 * height;
            Board.FieldGrid.Width = 16 * width;
            Application.Current.MainWindow.Content = Board.FieldGrid;
            for (int k = 0; k < height; k++)
            {
                for (int j = 0; j < width; j++)
                {
                    switch(map[k, j])
                    {
                        case " ":
                            Board.Draw("Recources\\Empty.png", j, k);
                            break;
                        case "@":
                            if (k != 0|| j != 0 && j!= 1)
                            {
                                Board.Draw("Recources\\Rock.png", j, k);
                            }
                            break;
                        case "#":
                            Board.Draw("Recources\\Wall.png", j, k);
                            break;
                        case "-":
                            Board.Draw("Recources\\Rope.png", j, k);
                            break;
                        case "=":
                            Board.Draw("Recources\\Ladder.png", j, k);
                            break;
                        case "$":
                            Board.Draw("Recources\\Gold.png", j, k);
                            break;
                    }
                }
            }
            TextBlock help = new TextBlock();
            help.Text = "He";
            help.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            Grid.SetColumn(help, 0);
            Grid.SetRow(help, 0);
            help.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
            Board.FieldGrid.Children.Add(help);
            TextBlock help2 = new TextBlock();
            help2.Text = "lp";
            help2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            Grid.SetColumn(help2, 1);
            Grid.SetRow(help2, 0);
            help2.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
            Board.FieldGrid.Children.Add(help2);
            ToolTip tip = new ToolTip();
            TextBlock tiptext = new TextBlock();
            tip.Content = tiptext;
            tiptext.Text = "Press:\nArrow keys to navigate\n1 - empty\n2 - Wall\n3 - Rock\n4 - Rope\n5 - Ladder\n6 - Gold\n0 - Save map";
            help.ToolTip = tip;
            help2.ToolTip = tip;
            Application.Current.MainWindow.AddHandler(MainWindow.KeyDownEvent, new KeyEventHandler(EditorKeyDownHandler));
            Board.Draw("Recources\\Choice.png", x, y);
            while (true)
                {
                    if (_x != x || _y != y)
                    {
                        switch (map[_y, _x])
                        {
                            case " ":
                                Board.Draw("Recources\\Empty.png", _x, _y);
                                break;
                            case "#":
                                Board.Draw("Recources\\Wall.png", _x, _y);
                                break;
                            case "@":
                                Board.Draw("Recources\\Rock.png", _x, _y);
                                break;
                            case "-":
                                Board.Draw("Recources\\Rope.png", _x, _y);
                                break;
                            case "=":
                                Board.Draw("Recources\\Ladder.png", _x, _y);
                                break;
                            case "$":
                                Board.Draw("Recources\\Gold.png", _x, _y);
                                break;
                    }
                        _x = x;
                        _y = y;
                        Board.Draw("Recources\\Choice.png", x, y);
                    }
                    switch (input)
                    {
                        case 1:
                            map[y, x] = " ";
                            Board.Draw("Recources\\Empty.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 2:
                            map[y, x] = "#";
                            Board.Draw("Recources\\Wall.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 3:
                            map[y, x] = "@";
                            Board.Draw("Recources\\Rock.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 4:
                            map[y, x] = "-";
                            Board.Draw("Recources\\Rope.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 5:
                            map[y, x] = "=";
                            Board.Draw("Recources\\Ladder.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 6:
                            map[y, x] = "$";
                            Board.Draw("Recources\\Gold.png", x, y);
                            Board.Draw("Recources\\Choice.png", x, y);
                            break;
                        case 0:
                            input = -1;
                            x = 2;
                            y = 2;
                            NameState = 0;
                            Application.Current.MainWindow.RemoveHandler(MainWindow.KeyDownEvent, new KeyEventHandler(EditorKeyDownHandler));
                            await SaveMap(map, mapName);
                            Board.Menu();
                            return;
                        default:
                            break;
                    }
                    input = -1;
                    await Task.Delay(15);
            }
        }
        public static void GetParameters()
        {
            Grid prompt = new Grid();
            prompt.ShowGridLines = false;
            prompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(250) });
            prompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(800) });
            prompt.RowDefinitions.Add(new RowDefinition() {Height = new GridLength(80) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(80) });
            TextBlock initial = new TextBlock();
            initial.Text = "Type width and height of a new map";
            Grid.SetRow(initial, 0);
            Grid.SetColumn(initial, 1);
            initial.FontSize = 45;
            prompt.Children.Add(initial);
            widthBox = new TextBox();
            Grid.SetRow(widthBox, 1);
            Grid.SetColumn(widthBox, 1);
            widthBox.FontSize = 45;
            prompt.Children.Add(widthBox);
            heightBox = new TextBox();
            Grid.SetRow(heightBox, 2);
            Grid.SetColumn(heightBox, 1);
            heightBox.FontSize = 45;
            prompt.Children.Add(heightBox);
            TextBlock widthdescr = new TextBlock();
            widthdescr.Text = "Width";
            Grid.SetRow(widthdescr, 1);
            Grid.SetColumn(widthdescr, 0);
            widthdescr.FontSize = 45;
            prompt.Children.Add(widthdescr);
            TextBlock heightdescr = new TextBlock();
            heightdescr.Text = "Height";
            Grid.SetRow(heightdescr, 2);
            Grid.SetColumn(heightdescr, 0);
            heightdescr.FontSize = 45;
            prompt.Children.Add(heightdescr);
            Grid.SetRow(Wrong, 4);
            Grid.SetColumn(Wrong, 1);
            Wrong.FontSize = 23;
            prompt.Children.Add(Wrong);
            Button confirm = new Button();
            confirm.Click += ConfirmClick;
            Grid.SetRow(confirm, 3);
            Grid.SetColumn(confirm, 1);
            confirm.Content = "Confirm";
            prompt.Children.Add(confirm);
            Application.Current.MainWindow.Content = prompt;
        }
        public static void ConfirmClick(object sender, EventArgs e)
        {
            if(!Int32.TryParse(widthBox.Text, out int o1)||!Int32.TryParse(heightBox.Text, out int o2))
            {
                Wrong.Text = "Invalid arguments, use digits only";
            }
            else if(Convert.ToInt32(widthBox.Text) <15 || Convert.ToInt32(widthBox.Text) > 120 || Convert.ToInt32(heightBox.Text) < 10 || Convert.ToInt32(heightBox.Text) > 35)
            {
                Wrong.Text = "Invalid arguments, width must be 15-120, height must be 10-35";
            }
            else
            {
                width = Convert.ToInt32(widthBox.Text);
                height = Convert.ToInt32(heightBox.Text);
                _ = CreateMap(width, height);
            }
        }
        public static async Task GetMapName()
        {
            Grid prompt = new Grid();
            Application.Current.MainWindow.Content = prompt;
            prompt.ShowGridLines = false;
            prompt.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(800) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            prompt.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(100) });
            TextBlock initial = new TextBlock();
            initial.Text = "Type the name of a new map";
            Grid.SetRow(initial, 0);
            Grid.SetColumn(initial, 1);
            initial.FontSize = 45;
            prompt.Children.Add(initial);
            Grid.SetRow(nameBox, 1);
            Grid.SetColumn(nameBox, 0);
            nameBox.FontSize = 45;
            prompt.Children.Add(nameBox);
            Wrong.Text = "Text";
            TextBlock WrongText = new TextBlock();
            Grid.SetRow(WrongText, 3);
            Grid.SetColumn(WrongText, 0);
            WrongText.FontSize = 45;
            Button confirm = new Button();
            confirm.Click += ConfirmNameClick;
            Grid.SetRow(confirm, 2);
            Grid.SetColumn(confirm, 0);
            confirm.Content = "Confirm";
            confirm.FontSize = 45;
            prompt.Children.Add(confirm);
            prompt.Children.Add(WrongText);
            while(true)
            {
                switch(NameState)
                {
                    case 1:
                        Board.Menu();
                        return;
                    case 2:
                        WrongText.Text = "Name cannot be null";
                        break;
                    case 3:
                        WrongText.Text = "File already exists, try another name";
                        break;
                    default:
                        break;
                }
                await Task.Delay(50);
            }
        }
        public static void ConfirmNameClick(object sender, EventArgs e)
        {
            if(nameBox.Text == "")
            {
                NameState = 2;
            }
            else if (File.Exists($"maps\\{nameBox.Text}.txt"))
            {
                NameState = 3;
            }
            else
            {
                NameState = 1;
            }
        }
        public static async Task Start()
        {
            int width;
            int height;
            int input;
            while (true)
            {
                Console.WriteLine("Type map width(without borders, more than 15)");
                if (Int32.TryParse(Console.ReadLine(), out input))
                {
                    if (input >= 15)
                    {
                        width = input;
                        break;
                    }
                }
                Console.WriteLine("Invalid argument");
            }
            while (true)
            {
                Console.WriteLine("Type map height(without borders, more than 10)");
                if (Int32.TryParse(Console.ReadLine(), out input))
                {
                    if (input >= 10)
                    {
                        height = input;
                        break;
                    }
                }
                Console.WriteLine("Invalid argument");
            }
            await CreateMap(width, height);
        }
        public static async Task CreateMap(int width, int height)
        {
            int _x = x;
            int _y = y;
            Board.FieldGrid = new Grid();
            //Board.FieldGrid.ShowGridLines = true;
            for (int j = 0; j < height; j++)
            {
                Board.FieldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(29) });
            }

            for (int j = 0; j < width; j++)
            {
                Board.FieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16) });
            }
            Board.FieldGrid.HorizontalAlignment = HorizontalAlignment.Left;
            Board.FieldGrid.VerticalAlignment = VerticalAlignment.Top;
            Board.FieldGrid.Height = 29 * height;
            Board.FieldGrid.Width = 16 * width;
            Application.Current.MainWindow.Content = Board.FieldGrid;
            TextBlock help = new TextBlock();
            help.Text = "He";
            help.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            Grid.SetColumn(help, 0);
            Grid.SetRow(help, 0);
            help.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
            Board.FieldGrid.Children.Add(help);
            TextBlock help2 = new TextBlock();
            help2.Text = "lp";
            help2.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#000000"));
            Grid.SetColumn(help2, 1);
            Grid.SetRow(help2, 0);
            help2.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
            Board.FieldGrid.Children.Add(help2);
            ToolTip tip = new ToolTip();
            TextBlock tiptext = new TextBlock();
            tip.Content = tiptext;
            tiptext.Text = "Press:\nArrow keys to navigate\n1 - empty\n2 - Wall\n3 - Rock\n4 - Rope\n5 - Ladder\n6 - Gold\n0 - Save map";
            help.ToolTip = tip;
            help2.ToolTip = tip;
            string[,] map = new string[height, width];
            for (int i = 0; i < height ; i++)
            {
                for (int j = 0; j < width ; j++)
                {
                    if (i == 0 || i == height -1 || j == 0|| j == width-1 )
                    {
                        map[i, j] = "@";
                        if (i != 0 || j != 0 &&  j != 1)
                        {
                            Board.Draw("Recources\\Rock.png", j, i);
                        }
                    }
                    else if (i == 1 || i == height -2 || j == 1 || j == width - 2)
                    {
                        map[i, j] = "#";
                        Board.Draw("Recources\\Wall.png", j, i);
                    }
                    else
                    {
                        map[i, j] = " ";
                        Board.Draw("Recources\\Empty.png", j, i);
                    }
                }
            }
            Application.Current.MainWindow.AddHandler(MainWindow.KeyDownEvent, new KeyEventHandler(EditorKeyDownHandler));
            Board.Draw("Recources\\Choice.png", x, y);
            while (true)
            {
                if (_x != x || _y != y)
                {
                    switch (map[_y, _x])
                    {
                        case " ":
                            Board.Draw("Recources\\Empty.png", _x, _y);
                            break;
                        case "#":
                            Board.Draw("Recources\\Wall.png", _x, _y);
                            break;
                        case "@":
                            Board.Draw("Recources\\Rock.png", _x, _y);
                            break;
                        case "-":
                            Board.Draw("Recources\\Rope.png", _x, _y);
                            break;
                        case "=":
                            Board.Draw("Recources\\Ladder.png", _x, _y);
                            break;
                        case "$":
                            Board.Draw("Recources\\Gold.png", _x, _y);
                            break;
                    }
                    _x = x;
                    _y = y;
                    Board.Draw("Recources\\Choice.png", x, y);
                }
                switch (input)
                {
                    case 1:
                        map[y, x] = " ";
                        Board.Draw("Recources\\Empty.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 2:
                        map[y, x] = "#";
                        Board.Draw("Recources\\Wall.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 3:
                        map[y, x] = "@";
                        Board.Draw("Recources\\Rock.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 4:
                        map[y, x] = "-";
                        Board.Draw("Recources\\Rope.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 5:
                        map[y, x] = "=";
                        Board.Draw("Recources\\Ladder.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 6:
                        map[y, x] = "$";
                        Board.Draw("Recources\\Gold.png", x, y);
                        Board.Draw("Recources\\Choice.png", x, y);
                        break;
                    case 0:
                        input = -1;
                        x = 2;
                        y = 2;
                        NameState = 0;
                        Application.Current.MainWindow.RemoveHandler(MainWindow.KeyDownEvent, new KeyEventHandler(EditorKeyDownHandler));
                        await SaveMap(map);
                        return;
                    default:
                        break;
                }
                input = -1;
                await Task.Delay(15);
            }
            //    while (true)
            //    {
            //        Console.SetCursorPosition(x, y);
            //        input = Console.ReadKey(true);
            //        switch (input.Key)
            //        {
            //            case ConsoleKey.UpArrow:
            //                if (y > 2)
            //                    y -= 1;
            //                break;
            //            case ConsoleKey.DownArrow:
            //                if (y < height + 1)
            //                    y += 1;
            //                break;
            //            case ConsoleKey.LeftArrow:
            //                if (x > 2)
            //                    x -= 1;
            //                break;
            //            case ConsoleKey.RightArrow:
            //                if (x < width + 1)
            //                    x += 1;
            //                break;
            //            case ConsoleKey.Enter:
            //                Console.SetCursorPosition(0, height + 5);
            //                Console.WriteLine("You sure you want to save map? Type yes or no");
            //                switch (Console.ReadLine())
            //                {
            //                    case "yes":
            //                        SaveMap(map);
            //                        Board.Menu();
            //                        return;
            //                    case "no":
            //                        Console.SetCursorPosition(0, height + 5);
            //                        Console.WriteLine("                                                    ");
            //                        Console.WriteLine("                                                    ");
            //                        break;
            //                }
            //                break;
            //            default:
            //                map[y, x] = input.KeyChar.ToString();
            //                Board.Draw(input.KeyChar.ToString(), x, y);
            //                break;
            //        }
            //    }
        }
        public static void EditorKeyDownHandler(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (y > 2)
                        y -= 1;
                    break;
                case Key.Down:
                    if (y < height - 3)
                        y += 1;
                    break;
                case Key.Left:
                    if (x > 2)
                        x -= 1;
                    break;
                case Key.Right:
                    if (x < width - 3)
                        x += 1;
                    break;
                case Key.D1:
                    input = 1;
                    break;
                case Key.D2:
                    input = 2;
                    break;
                case Key.D3:
                    input = 3;
                    break;
                case Key.D4:
                    input = 4;
                    break;
                case Key.D5:
                    input = 5;
                    break;
                case Key.D6:
                    input = 6;
                    break;
                case Key.D0:
                    input = 0;
                    break;
                default:
                    break;
            }
        }
        public static async Task SaveMap(string[,] _map, string mapName = null)
        {
            string[] map = new string[_map.GetUpperBound(0) + 1];
            int height = _map.GetUpperBound(0);
            int width = _map.GetUpperBound(1);
            for (int i = 0; i <= height; i++)
            {
                for (int j = 0; j <= width; j++)
                {
                    map[i] += _map[i, j];
                }
            }
            if (mapName == null)
            {
                await GetMapName();
            }
            else
            {
                nameBox.Text = mapName;
            }
            using (StreamWriter stream = new StreamWriter($"maps\\{nameBox.Text}.txt"))
            {
                int i = 0;
                while (i < map.Length)
                {
                    stream.WriteLine(map[i]);
                    i++;
                }
            }
        }
    }
}
