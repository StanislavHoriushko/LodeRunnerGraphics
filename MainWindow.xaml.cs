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
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static bool outputExit;
        public static TextBlock[] output;
        public MainWindow()
        {
            InitializeComponent();
            Board.ImportSettings();
            Application.Current.MainWindow.WindowState = WindowState.Maximized;
            Start();

            //Board.StartLevel("level1");
        }
        public static async Task EndGame()
        {
            await Output(new string[] { "Game over", "Seems like you`ve run out of lives", $"Score : {Board.points}" });
            await Board.CreateMenu(new string[] { "Restart", "Main Menu", "Quit" });
        }
        public static async Task Win()
        {
            Application.Current.MainWindow.RemoveHandler(KeyDownEvent, new KeyEventHandler(KeyDownHandler));
            int timeBonus = 8000 - Board.frameCount;
            await MainWindow.Output(new string[] { "Victory", "You`ve collected all the gold", $"Score : {Board.points}", $"Time Bonus : { timeBonus }", $"Total : {Board.points + timeBonus}" });
            await Board.CreateMenu(new string[] { "Restart", "Main Menu", "Quit" });
        }
        public async static Task Output(string[] data)
        {
            //await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(async() =>
            //{
            Application.Current.MainWindow.AddHandler(MainWindow.KeyDownEvent, new KeyEventHandler(OutputHandler));
            Grid outputGrid = new Grid();
            outputGrid.ColumnDefinitions.Add(new ColumnDefinition());
            output = new TextBlock[data.Length + 1];
            Board.menu = new Menu(new string[1]);
            for (int i = 0; i < data.Length; i++)
            {
                outputGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(600 / data.Length) });
                output[i] = new TextBlock();
                output[i].Text = data[i];
                output[i].FontStyle = FontStyles.Italic;
                output[i].TextWrapping = TextWrapping.Wrap;
                output[i].FontSize = (600 / data.Length) * 0.3;
                Grid.SetRow(output[i], i);
                Grid.SetColumn(output[i], 0);
                outputGrid.Children.Add(output[i]);
            }
            outputGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(600 / data.Length) });
            output[output.Length - 1] = new TextBlock();
            output[output.Length - 1].FontSize = (600 / data.Length) * 0.3;
            output[output.Length - 1].Text = "Press Enter to Continue";
            output[output.Length - 1].FontSize = (600 / data.Length) * 0.4;
            output[output.Length - 1].FontStyle = FontStyles.Italic;
            Grid.SetRow(output[output.Length - 1], output.Length - 1);
            Grid.SetColumn(output[output.Length - 1], 0);
            outputGrid.Children.Add(output[output.Length - 1]);
            Application.Current.MainWindow.Content = outputGrid;
            Application.Current.MainWindow.Show();
            while (true)
            {
                if (outputExit)
                {
                    outputExit = false;
                    break;
                }
                else
                {
                    await Task.Delay(50);
                }
            }
            Application.Current.MainWindow.RemoveHandler(MainWindow.KeyDownEvent, new KeyEventHandler(OutputHandler));
            //}));
        }
        public static void Start()
        {
            Board.Menu();
        }
        public static Cell[,] PrintMap(string mapName)
        {
            Rectangle rect;
            ImageBrush img;
            List<string> mapList = new List<string>();
            using (StreamReader sr = new StreamReader($"maps\\{mapName}.txt", Encoding.Default))
            {
                string line;
                while ((line = sr.ReadLine()) != null)
                {
                    mapList.Add(line);
                }
            }
            Board.gold = Board.CountGold(mapList);
            int width = mapList[0].Length;
            int height = mapList.Count;
            OpenField(width, height);
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    switch (mapList[i][j])
                    {
                        case '#':
                            rect = new Rectangle();
                            img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri("Recources\\Wall.png", UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            Board.FieldGrid.Children.Add(rect);
                            break;
                        case '=':
                            rect = new Rectangle();
                            img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri("Recources\\Ladder.png", UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            Board.FieldGrid.Children.Add(rect);
                            break;
                        case '@':
                            rect = new Rectangle();
                            img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri("Recources\\Rock.png", UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            Board.FieldGrid.Children.Add(rect);
                            break;
                        case '-':
                            rect = new Rectangle();
                            img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri("Recources\\Rope.png", UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            Board.FieldGrid.Children.Add(rect);
                            break;
                        case '$':
                            rect = new Rectangle();
                            img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri("Recources\\Gold.png", UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, i);
                            Grid.SetColumn(rect, j);
                            Board.FieldGrid.Children.Add(rect);
                            break;

                    }
                }
            }
            return Board.MapStringToCell(mapList);
        }
        public static void OpenField(int x, int y)
        {
            //Application.Current.MainWindow.Hide();
            Window FieldWindow = Application.Current.MainWindow;
            Board.FieldGrid = new Grid();
            Board.PointsBar = new TextBlock[x];
            //Board.FieldGrid.ShowGridLines = true;
            for (int j = 0; j < y; j++)
            {
                Board.FieldGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(29) });
            }

            for (int j = 0; j < x; j++)
            {
                Board.FieldGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(16)});
            }
            Board.FieldGrid.HorizontalAlignment = HorizontalAlignment.Left;
            Board.FieldGrid.VerticalAlignment = VerticalAlignment.Top;
            Board.FieldGrid.Height = 29 * y;
            Board.FieldGrid.Width = 16 * x;
            FieldWindow.Content = Board.FieldGrid;
            FieldWindow.Show();
            FieldWindow.AddHandler(KeyDownEvent, new KeyEventHandler(KeyDownHandler));
        }
        public static void KeyDownHandler(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Q:
                    Player.Dig(-1, Board.frameCount);
                    break;
                case Key.E:
                    Player.Dig(1, Board.frameCount);
                    break;
                case Key.W:
                    Player.moveDirection = "up";
                    break;
                case Key.A:
                    Player.moveDirection = "left";
                    break;
                case Key.S:
                    Player.moveDirection = "down";
                    break;
                case Key.D:
                    Player.moveDirection = "right";
                    break;
                case Key.P:
                    Board.KillPlayer();
                    break;
                default:
                    Player.moveDirection = null;
                    break;
            }
        }
        public static void OutputHandler(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                outputExit = true;
            }
        }
    }
    [Serializable]
    class Settings
    {
        public int enemyNumber;
        public int maxLives;
        public int[] enemySpeeds;
        public Settings(int _enemyNumber, int[] _enemySpeeds, int _maxLives)
        {
            enemyNumber = _enemyNumber;
            enemySpeeds = _enemySpeeds;
            maxLives = _maxLives;
        }
    }
    class Menu
    {
        public int option;
        public string[] options;
        public Menu(string[] _options)
        {
            options = _options;
        }
    }
    class Board
    {

        public static MediaPlayer music = new MediaPlayer();
        public static Menu menu;
        public static Grid FieldGrid;
        public static TextBlock[] PointsBar;
        public static TextBlock[] menuOptions;
        public static string receivedOption;
        public static int gamespeed = 1;
        public static int randomNumber;
        //public static int menuOption;
        public static List<Enemy> enemyList = new List<Enemy> { };
        public static Field currentMap;
        public static string[] enemyIcons = new string[] { "Recources\\Enemy.png", "Recources\\Enemy.png", "Recources\\Enemy.png", "Recources\\Enemy.png", "Recources\\Enemy.png" };
        public static int[] enemySpeeds = new int[] { 9, 10, 11, 12, 13 };
        public static int enemyNumber = 3;
        //public static string answer;
        public static int maxLives = 3;
        public static int lives = maxLives;
        public static int gold;
        public static bool enemiesAreFrozen = false;
        public static int EnemyNumber
        {
            get
            {
                return enemyNumber;
            }
            set
            {
                if (enemyNumber > 5 || enemyNumber < 1)
                {
                    enemyNumber = 3;
                }
                else
                {
                    enemyNumber = value;
                }
            }
        }
        static public Player player = new Player("Recources\\Player.png");
        private static System.Timers.Timer frameUpdate;
        public static int points = 0;
        public static int frameCount;
        public static List<int[]> eventsList = new List<int[]>();
        private static string currentMapName = "";
        public async static Task CreateMenu(string[] options)
        {
            Application.Current.MainWindow.RemoveHandler(MainWindow.KeyDownEvent, new KeyEventHandler(MenuKeyDownHandler));
            int option = 0;
            Application.Current.MainWindow.Show();
            menuOptions = new TextBlock[options.Length];
            Grid MenuGrid = new Grid();
            MenuGrid.ShowGridLines = false;
            menu = new Menu(options);
            MenuGrid.ColumnDefinitions.Add(new ColumnDefinition());
            for (int i = 0; i < options.Length; i++)
            {
                if (options.Length > 3)
                {
                    MenuGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(800 / options.Length) });
                    menuOptions[i] = new TextBlock();
                    menuOptions[i].FontSize = (800 / options.Length) * 0.75;
                }
                else
                {
                    MenuGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(200) });
                    menuOptions[i] = new TextBlock();
                    menuOptions[i].FontSize = 125;
                }
                menuOptions[i].FontStyle = FontStyles.Italic;
                Grid.SetColumn(menuOptions[i], 0);
                Grid.SetRow(menuOptions[i], i);
                menuOptions[i].Text = options[i];
                MenuGrid.Children.Add(menuOptions[i]);
            }
            Application.Current.MainWindow.Content = MenuGrid;
            menuOptions[0].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#555EE1"));
            Application.Current.MainWindow.AddHandler(MainWindow.KeyDownEvent, new KeyEventHandler(MenuKeyDownHandler));
            menu.option = 0;
            while (true)
            {
                if (menu.option == -1)
                {
                    Application.Current.MainWindow.RemoveHandler(MainWindow.KeyDownEvent, new KeyEventHandler(MenuKeyDownHandler));
                    receivedOption =  options[option];
                    break;
                }
                else if (menu.option != option)
                {
                    menuOptions[option].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFFFFF"));
                    option = menu.option;
                    menuOptions[option].Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#555EE1"));
                }
                await Task.Delay(50);
            }
        }
        public static void MenuKeyDownHandler(object sender, KeyEventArgs e)
        {

            switch (e.Key)
            {
                case Key.Up:
                    if (menu.option > 0)
                    {
                        menu.option -= 1;
                    }
                    else
                    {
                        menu.option = menu.options.Length - 1;
                    }
                    break;
                case Key.Down:
                    if (menu.option < menu.options.Length - 1)
                    {
                        menu.option += 1;
                    }
                    else
                    {
                        menu.option = 0;
                    }
                    break;
                case Key.W:
                    if (menu.option > 0)
                    {
                        menu.option -= 1;
                    }
                    else
                    {
                        menu.option = menu.options.Length - 1;
                    }
                    break;
                case Key.S:
                    if (menu.option < menu.options.Length - 1)
                    {
                        menu.option += 1;
                    }
                    else
                    {
                        menu.option = 0;
                    }
                    break;
                case Key.Enter:
                    menu.option = -1;
                    break;
                default:
                    break;
            }
        }

        public async static void StartLevel(string mapName)
        {
            gamespeed = 1;
            enemiesAreFrozen = false;
            music.Open(new Uri("Sounds\\Undertale - Megalovania.mp3", UriKind.Relative));
            music.Play();
            Player.speed = Player.baseSpeed;
            if (File.Exists($"maps\\{mapName}.txt"))
            {
                currentMap = new Field(MainWindow.PrintMap(mapName));
                enemyList.Clear();
                for (int i = 0; i < EnemyNumber; i++)
                {
                    enemyList.Add(new Enemy(enemyIcons[i], enemySpeeds[i]));
                }
                Player.currentMap = currentMap;
                Player.SetPlayerPosition(2, currentMap.map.GetUpperBound(0) - 2);
                Draw(Player.playerImage, Player.playerX, Player.playerY);
                foreach (Enemy enemy in enemyList)
                {
                    enemy.SetEnemyPosition((10 + enemyList.IndexOf(enemy) * (currentMap.map.GetUpperBound(1)-20)/enemyNumber), 2);
                }
                lives = maxLives;
                frameCount = 0;
                currentMapName = mapName;
                WritePoints($"Score : {points}pts   ", 0);
                WritePoints($"Lives : {lives}   ", 30);
                SetTimer();
            }
            else
            {
                await MainWindow.Output(new string[] { $"Unable to find maps\\{mapName}.dat" });
                Menu();
            }
        }
        public static void SetToDefault()
        {
            lives = maxLives;
            points = 0;
            enemyList.Clear();
            gamespeed = 1;
            frameCount = 0;
            eventsList.Clear();
            enemiesAreFrozen = false;
        }
        public static async void Settings()
        {
            await CreateMenu(new string[] { "Amount of enemies", "Enemy speed", "Lives", "Save changes", "Cancel" });
            switch (receivedOption)
            {
                case "Amount of enemies":
                    await CreateMenu(new string[] { $"Change", "Back" });
                    switch (receivedOption)
                    {
                        case "Change":
                            await CreateMenu(new string[] { "Set to 1", "Set to 2", "Set to 3", "Set to 4", "Set to 5", "Back" });
                            switch (receivedOption)
                            {
                                case "Back":
                                    Settings();
                                    break;
                                case "Set to 1":
                                    EnemyNumber = 1;
                                    Settings();
                                    break;
                                case "Set to 2":
                                    EnemyNumber = 2;
                                    Settings();
                                    break;
                                case "Set to 3":
                                    EnemyNumber = 3;
                                    Settings();
                                    break;
                                case "Set to 4":
                                    EnemyNumber = 4;
                                    Settings();
                                    break;
                                case "Set to 5":
                                    EnemyNumber = 5;
                                    Settings();
                                    break;
                                default:
                                    Settings();
                                    break;
                            }
                            break;
                        case "Back":
                            Settings();
                            break;
                        default:
                            Settings();
                            break;
                    }
                    break;
                case "Enemy speed":
                    await MainWindow.Output(new string[] { $"Current speed of 5 enemies - {enemySpeeds[0]} {enemySpeeds[1]} {enemySpeeds[2]} {enemySpeeds[3]} {enemySpeeds[4]} respectively.", "The higher the value the lower the speed.", "Choose which one`s speed you want to change" });
                    await CreateMenu(new string[] { "1", "2", "3", "4", "5", "Back" });
                    if (receivedOption == "Back")
                    {
                        Settings();
                        break;
                    }
                    else
                    {
                        int answer = Convert.ToInt32(receivedOption);
                        await CreateMenu(new string[] { "Set speed to 5", "Set speed to 6", "Set speed to 7", "Set speed to 8", "Set speed to 9", "Set speed to 10", "Set speed to 11", "Set speed to 12", "Set speed to 13", "Set speed to 14", "Set speed to 15", "Back" });
                        switch (receivedOption)
                        {
                            case "Set speed to 5":
                                enemySpeeds[answer - 1] = 5;
                                break;
                            case "Set speed to 6":
                                enemySpeeds[answer - 1] = 6;
                                break;
                            case "Set speed to 7":
                                enemySpeeds[answer - 1] = 7;
                                break;
                            case "Set speed to 8":
                                enemySpeeds[answer - 1] = 8;
                                break;
                            case "Set speed to 9":
                                enemySpeeds[answer - 1] = 9;
                                break;
                            case "Set speed to 10":
                                enemySpeeds[answer - 1] = 10;
                                break;
                            case "Set speed to 11":
                                enemySpeeds[answer - 1] = 11;
                                break;
                            case "Set speed to 12":
                                enemySpeeds[answer - 1] = 12;
                                break;
                            case "Set speed to 13":
                                enemySpeeds[answer - 1] = 13;
                                break;
                            case "Set speed to 14":
                                enemySpeeds[answer - 1] = 14;
                                break;
                            case "Set speed to 15":
                                enemySpeeds[answer - 1] = 15;
                                break;
                            case "Back":
                                break;
                        }
                        Settings();
                        break;
                    }
                case "Lives":
                    await MainWindow.Output(new string[] { $"Current amount of lives - {maxLives}.", "Choose the amount of lives or leave the current value." });
                    await CreateMenu(new string[] { "Set to 1", "Set to 2", "Set to 3", "Set to 4", "Set to 5", "Back" });
                    switch (receivedOption)
                    {
                        case "Set to 1":
                            maxLives = 1;
                            break;
                        case "Set to 2":
                            maxLives = 2;
                            break;
                        case "Set to 3":
                            maxLives = 3;
                            break;
                        case "Set to 4":
                            maxLives = 4;
                            break;
                        case "Set to 5":
                            maxLives = 5;
                            break;
                        case "Back":
                            break;
                    }
                    Settings();
                    break;
                case "Cancel":
                    Menu();
                    break;
                case "Save changes":
                    SaveChanges(enemyNumber, enemySpeeds, maxLives);
                    Menu();
                    break;
                default:
                    Settings();
                    break;

            }
        }
        public async static void Menu()
        {
            music.Stop();
            await CreateMenu(new string[] { "Start", "Create map", "Edit map", "Settings", "Quit" });
            switch (receivedOption)
            {
                case "Start":
                    await CreateMenu(GetMapNames());
                    switch (receivedOption)
                    {
                        case "Back":
                            Menu();
                            break;
                        default:
                            StartLevel(receivedOption);
                            break;
                    }
                    break;
                case "Create map":
                    MapEditor.GetParameters();
                    break;
                case "Edit map":
                    await CreateMenu(GetMapNames());
                    switch (receivedOption)
                    {
                        case "Back":
                            Menu();
                            break;
                        default:
                            await MapEditor.Edit(receivedOption);
                            break;
                    }
                    break;
                case "Settings":
                    Settings();
                    break;
                case "Quit":
                    Application.Current.MainWindow.Close();
                    break;
            }
        }
        public static void KillEnemy(Enemy enemy)
        {
            enemy.StartRessurectTimer(frameCount);
            points += 100;
            WritePoints($"Score : {points}pts   ", 0);
        }
        public async static void KillPlayer()
        { 
            lives--;
            points *= 2;
            points /= 3;
            if (lives == 0)
            {
                frameUpdate.Stop();

                await MainWindow.EndGame();
                SetToDefault();
                //await MainWindow.Output(new string[] { "Game over", "Seems like you`ve run out of lives", $"Score : {points}" });
                //await CreateMenu(new string[] { "Restart", "Main Menu", "Quit" }); 
                switch (receivedOption)
                {
                    case "Restart":
                        StartLevel(currentMapName);
                        break;
                    case "Quit":
                        Environment.Exit(-1);
                        break;
                    case "Main Menu":
                        Menu();
                        break;
                }
            }
            else
            {
                Player.SetPlayerPosition(2, currentMap.map.GetUpperBound(0) - 2);
                Draw(Player.playerImage, Player.playerX, Player.playerY);
                WritePoints($"Score : {points}pts   ", 0);
                WritePoints($"Lives : {lives}   ", 30);
            }
        }
        public async static void Win()
        {
            frameUpdate.Stop();
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(async() =>
            {
                await MainWindow.Win();
                SetToDefault();
                switch (receivedOption)
                {
                    case "Restart":
                        points = 0;
                        StartLevel(currentMapName);
                        break;
                    case "Quit":
                        Application.Current.MainWindow.Close();
                        break;
                    case "Main Menu":
                        Menu();
                        break;
                }
            }));
        }
        public static int CountGold(List<string>map)
        {
            int result = 0;
            foreach(string str in map)
            {
                for(int i = 0; i<str.Length; i++)
                {
                    if (str[i] == '$')
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public static void SpawnBonus()
        {
            int randomX;
            int randomY;
            int randomBonus;

            while (true)
            {
                randomX = new Random().Next(3, (currentMap.map.GetUpperBound(1) - 2));
                randomY = new Random().Next(3, currentMap.map.GetUpperBound(0) - 2);
                if (currentMap[randomY, randomX].GetCellType() == "Empty")
                {
                    while (true)
                    {
                        if (currentMap[randomY + 1, randomX].isWalkable && currentMap[randomY, randomX].GetCellType() != "Rope")
                        {
                            break;
                        }
                        randomY += 1;
                    }
                    if (currentMap[randomY + 1, randomX].GetCellType() != "Rock")
                    {
                        randomBonus = new Random().Next(10);
                        switch (randomBonus)
                        {
                            case 0:
                                currentMap[randomY, randomX] = new Freeze(randomX, randomY);
                                break;
                            case 1:
                                currentMap[randomY, randomX] = new Boost(randomX, randomY);
                                break;
                            case 2:
                                currentMap[randomY, randomX] = new KillAllHumans(randomX, randomY);
                                break;
                            case 3:
                                currentMap[randomY, randomX] = new MineSpawner(randomX, randomY);
                                break;
                            case 4:
                                currentMap[randomY, randomX] = new AddLife(randomX, randomY);
                                break;
                            case 5:
                                currentMap[randomY, randomX] = new RandomTeleport(randomX, randomY);
                                break;
                            case 6:
                                currentMap[randomY, randomX] = new BonusPoints(randomX, randomY);
                                break;
                            case 7:
                                currentMap[randomY, randomX] = new SlowEnemies(randomX, randomY);
                                break;
                            default:
                                break;
                        }
                        currentMap[randomY, randomX].DrawSelf();
                        break;
                    }
                }
            }
            return;
        }
        public static string[] BuildPath(Enemy enemy, Player player, int length)
        {
            string[] path = new string[length];
            if (Player.playerY == enemy.enemyY)
            {
                for (int i = 0; i < length; i++)
                {
                    if (Player.playerX > enemy.enemyX)
                    {
                        path[i] = "right";
                    }
                    else if (Player.playerX < enemy.enemyX)
                    {
                        path[i] = "left";
                    }
                    else if (Player.playerX == enemy.enemyX)
                    {
                        path[i] = "";
                    }
                }
            }
            if (Player.playerY < enemy.enemyY)
            {
                int ladderLocationHorizontal = FindLadderUp(enemy, 200);
                if (ladderLocationHorizontal != 0 || Board.currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Ladder")
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (i < ladderLocationHorizontal && ladderLocationHorizontal > 0 || i < -ladderLocationHorizontal && ladderLocationHorizontal < 0)
                        {
                            if (ladderLocationHorizontal > 0)
                            {
                                path[i] = "right";
                            }
                            else
                            {
                                path[i] = "left";
                            }
                        }
                        else
                        {
                            path[i] = "up";
                        }
                    }
                }
                else if (Board.currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Ladder")
                {
                    for (int j = 0; j < length; j++)
                    {
                        path[j] = "up";
                    }
                }
            }
            if (Player.playerY > enemy.enemyY)
            {
                int ladderLocationHorizontal = FindLadderDown(enemy, 200);
                if (currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Rope" && !currentMap[enemy.enemyY + 1, enemy.enemyX].collision)
                {
                    for (int k = 0; k < length; k++)
                    {
                        path[k] = "down";
                    }
                }
                else if (ladderLocationHorizontal != 0 || Board.currentMap[enemy.enemyY + 1, enemy.enemyX].GetCellType() == "Ladder" || Board.currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Handpole" && !Board.currentMap[enemy.enemyY + 1, enemy.enemyX].isWalkable)
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (i < ladderLocationHorizontal && ladderLocationHorizontal > 0 || i < -ladderLocationHorizontal && ladderLocationHorizontal < 0)
                        {
                            if (ladderLocationHorizontal > 0)
                            {
                                path[i] = "right";
                            }
                            else
                            {
                                path[i] = "left";
                            }
                        }
                        else
                        {
                            path[i] = "down";
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < length; i++)
                    {
                        if (Player.playerX > enemy.enemyX)
                        {
                            path[i] = "right";
                        }
                        else if (Player.playerX < enemy.enemyX)
                        {
                            path[i] = "left";
                        }
                        else if (Player.playerX == enemy.enemyX)
                        {
                            path[i] = "";
                        }
                    }
                }
            }
            return path;
        }
        public static int FindLadderUp(Enemy enemy, int searchRange)
        {
            bool rightSide = true;
            bool leftSide = true;
            for (int i = 1; i <= searchRange; i++)
            {
                if (rightSide && Board.currentMap[enemy.enemyY, enemy.enemyX + i].collision || rightSide && !Board.currentMap[enemy.enemyY + 1, enemy.enemyX + i].isWalkable)
                {
                    rightSide = false;
                }
                if (leftSide && Board.currentMap[enemy.enemyY, enemy.enemyX - i].collision || leftSide && !Board.currentMap[enemy.enemyY + 1, enemy.enemyX - i].isWalkable)
                {
                    leftSide = false;
                }
                if (rightSide && Board.currentMap[enemy.enemyY, enemy.enemyX + i].GetCellType() == "Ladder")
                {
                    return 1;
                }
                if (leftSide && Board.currentMap[enemy.enemyY, enemy.enemyX - i].GetCellType() == "Ladder")
                {
                    return -1;
                }
            }
            return 0;
        }
        public static int FindLadderDown(Enemy enemy, int searchRange)
        {
            bool rightSide = true;
            bool leftSide = true;
            for (int i = 1; i <= searchRange; i++)
            {
                if (rightSide && Board.currentMap[enemy.enemyY, enemy.enemyX + i].collision || rightSide && !Board.currentMap[enemy.enemyY + 1, enemy.enemyX + i].isWalkable)
                {
                    rightSide = false;
                }
                if (leftSide && Board.currentMap[enemy.enemyY, enemy.enemyX - i].collision || leftSide && !Board.currentMap[enemy.enemyY + 1, enemy.enemyX - i].isWalkable)
                {
                    leftSide = false;
                }
                if (rightSide && Board.currentMap[enemy.enemyY + 1, enemy.enemyX + i].GetCellType() == "Ladder")
                {
                    return 1;
                }
                if (leftSide && Board.currentMap[enemy.enemyY + 1, enemy.enemyX - i].GetCellType() == "Ladder")
                {
                    return -1;
                }
            }
            return 0;
        }
        public static Cell[,] MapStringToCell(List<string> mapString)
        {
            int mapStringLength = mapString.Count;
            Cell[,] map = new Cell[mapStringLength, mapString[0].Length];
            for (int i = 0; i < mapStringLength; i++)
            {
                for (int j = 0; j < mapString[i].Length; j++)
                {
                    switch (mapString[i][j])
                    {
                        case '#':
                            map[i, j] = new Wall(j, i);
                            break;
                        case '@':
                            map[i, j] = new Rock(j, i);
                            break;
                        case '=':
                            map[i, j] = new Ladder(j, i);
                            break;
                        case '-':
                            map[i, j] = new Rope(j, i);
                            break;
                        case '$':
                            map[i, j] = new Gold(j, i);
                            break;
                        case ' ':
                            map[i, j] = new Empty(j, i);
                            if (map[i - 1, j].GetCellType() == "Rope")
                            {
                                map[i, j].isWalkable = true;
                            }
                            break;
                        default:
                            map[i, j] = new Empty(j, i);
                            break;
                    }
                }
            }
            return map;
        }
        public static string[] GetMapNames()
        {
            IEnumerable<string> mapNames = Directory.EnumerateFiles("maps", "*.txt");
            string[] maps = mapNames.ToArray();
            for (int i = 0; i < maps.Length; i++)
            {
                maps[i] = maps[i].Substring(maps[i].LastIndexOf("\\") + 1, maps[i].LastIndexOf(".") - maps[i].LastIndexOf("\\") - 1);
            }
            string[] maps1 = new string[maps.Length + 1];
            for (int j = 0; j < maps.Length; j++)
            {
                maps1[j] = maps[j];
            }
            maps1[maps1.Length - 1] = "Back";
            return maps1;
        }
        public static void ImportSettings()
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Settings));
            Settings result;
            using (FileStream fs = new FileStream("settings\\settings.json", FileMode.OpenOrCreate))
            {
                result = (Settings)formatter.ReadObject(fs);
            }
            enemyNumber = result.enemyNumber;
            maxLives = result.maxLives;
            enemySpeeds = result.enemySpeeds;
        }
        public static void SaveChanges(int _enemyNumber, int[] _enemySpeeds, int _maxLives)
        {
            DataContractJsonSerializer formatter = new DataContractJsonSerializer(typeof(Settings));
            using (FileStream fs = new FileStream("settings\\settings.json", FileMode.OpenOrCreate))
            {
                Settings settings = new Settings(_enemyNumber, _enemySpeeds, _maxLives);
                formatter.WriteObject(fs, settings);
            }
        }
        public static int[] GetSpeedValues(string answer)
        {
            int count = 0;
            int start = -1;
            int[] result = new int[5];
            for (int i = 0; i < answer.Length; i++)
            {
                if (count == 5)
                {
                    break;
                }
                if (Int32.TryParse(answer.Substring(i, 1), out int t) && start == -1 && i != answer.Length - 1)
                {
                    start = i;
                }
                else if (Int32.TryParse(answer.Substring(i, 1), out int o) && i != answer.Length - 1)
                {
                    continue;
                }
                else if (answer[i] == ' ' && start != -1)
                {
                    result[count] = Int32.Parse(answer.Substring(start, i - start));
                    count++;
                    start = -1;
                }
                else if (answer[i] == ' ')
                {
                    continue;
                }
                else if (i == answer.Length - 1 && start == -1 && Int32.TryParse(answer.Substring(i, 1), out int q))
                {
                    result[count] = Int32.Parse(answer.Substring(i, 1));
                    count++;
                    break;
                }
                else if (i == answer.Length - 1 && start != -1)
                {
                    result[count] = Int32.Parse(answer.Substring(start));
                    count++;
                    break;
                }
                else
                {
                    return new int[] { -1, -1, -1, -1, -1 };
                }
            }
            if (count == 5)
            {
                return result;
            }
            else
            {
                return new int[] { -1, -1, -1, -1, -1 };
            }
        }
        public static void WritePoints(string line, int x)
        {
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                for (int i = 0; i < line.Length; i++)
                {
                    if (PointsBar[x+i]==null||PointsBar[x + i].Text == null)
                    {
                        TextBlock text = new TextBlock();
                        text.Text = line[i].ToString();
                        text.Foreground = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                        //text.Background = (SolidColorBrush)(new BrushConverter().ConvertFrom("#FF0000"));
                        Grid.SetRow(text, 0);
                        Grid.SetColumn(text, x + i);
                        PointsBar[x + i] = text;
                        FieldGrid.Children.Add(text);
                    }
                    else
                    {
                        PointsBar[x + i].Text = line[i].ToString();
                    }
                }
            }));
        }
        public static void Draw(string way, int x, int y)
        {
                        Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                        {
                            Rectangle rect = new Rectangle();
                            ImageBrush img = new ImageBrush();
                            img.ImageSource = new BitmapImage(new Uri(way, UriKind.Relative));
                            rect.Fill = img;
                            Grid.SetRow(rect, y);
                            Grid.SetColumn(rect, x);
                            FieldGrid.Children.Add(rect);
                        }));
        }

        public static async Task KillPlayerViaDispatcher()
        {
            await Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
            {
                KillPlayer();
            }));
        }
        private static void SetTimer()
        {
            frameCount = 0;
            frameUpdate = new System.Timers.Timer(15);
            frameUpdate.Elapsed += UpdateFrame;
            frameUpdate.AutoReset = true;
            frameUpdate.Enabled = true;
        }
        private static void UpdateFrame(Object source, ElapsedEventArgs e)
        {
            frameCount++;
            foreach (Enemy enemy in enemyList)
            {
                if (enemy.ressurectAt == frameCount)
                {
                    enemy.Ressurect();
                }
                if (enemy.enemyX == Player.playerX && enemy.enemyY == Player.playerY)
                {
                    Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(() =>
                    {
                        KillPlayer();
                    }));
                    KillEnemy(enemy);
                }
            }
            if (eventsList.Count != 0)
            {
                foreach (int[] _event in eventsList)
                {
                    if (currentMap[_event[0], _event[1]].endTime == frameCount)
                    {
                        currentMap[_event[0], _event[1]].DurationEnd();
                    }
                }
            }
            if (!currentMap[Player.playerY + 1, Player.playerX].isWalkable && !currentMap[Player.playerY + 1, Player.playerX].collision && frameCount % Player.speed == 0)
            {
                Player.Step("down");
                Player.isFalling = true;
            }
            else if (Player.isFalling == true)
            {
                Player.isFalling = false;
            }
            else if (!Player.isFalling)
            {
                if (frameCount % Player.speed == 0)
                {
                    Player.Step(Player.moveDirection);
                    Player.moveDirection = null;
                }
            }
            if (!enemiesAreFrozen)
            {
                foreach (Enemy enemy in enemyList)
                {
                    if (currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Mine")
                    {
                        currentMap[enemy.enemyY, enemy.enemyX] = new Empty(enemy.enemyX, enemy.enemyY);
                        KillEnemy(enemy);
                    }
                    if (frameCount % (enemy.enemySpeed * gamespeed) == 0)
                    {
                        if (currentMap[enemy.enemyY, enemy.enemyX].GetCellType() == "Hole" && enemy.releaseAt == -1)
                        {
                            enemy.StartReleaseTimer(frameCount);
                        }
                        else if (enemy.releaseAt < frameCount && enemy.releaseAt > 0)
                        {
                            enemy.Release();
                        }
                        enemy.EnemyStep(frameCount);
                    }
                }
            }
            randomNumber = new Random().Next(10001);
            {
                if (randomNumber < 50)
                {
                    SpawnBonus();
                }
            }
        }
    }
}
