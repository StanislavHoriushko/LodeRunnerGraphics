using System;

namespace LodeRunnerGraphics
{
    public abstract class Cell
    {
        public int X;
        public int Y;
        public string image;
        public bool isWalkable;
        public bool collision;
        public string currentUnit = "None";
        public int endTime = -1;
        public virtual string GetCellType()
        {
            return "";
        }
        public Cell(int _X, int _Y)
        {
            X = _X;
            Y = _Y;
        }
        public virtual void Activate(int frameCount)
        {
            Board.Draw("Recources\\Empty.png", X, Y);
            Board.Draw("Recources\\Player.png", X, Y);
        }
        public virtual void DurationEnd()
        {
            endTime = -1;
            Board.eventsList.Remove(new int[] { Y, X });
        }
        public virtual void DrawSelf()
        {
            Board.Draw(image, X, Y);
        }
    }
    class Field
    {
        public Cell[,] map;
        public int goldAmount;
        public Field(Cell[,] _map)
        {
            map = _map;
            goldAmount = CountGold(_map);
        }
        private int CountGold(Cell[,] _map)
        {
            int height = _map.GetUpperBound(0);
            int length = _map.GetUpperBound(1);
            int result = 0;
            for (int i = 0; i <= height; i++)
            {
                for (int j = 0; j <= length; j++)
                {
                    if (_map[i, j].GetCellType() == "Gold")
                    {
                        result++;
                    }
                }
            }
            return result;
        }
        public Cell this[int Y, int X]
        {
            get
            {
                return map[Y, X];
            }
            set
            {
                map[Y, X] = value;
            }
        }
    }
    abstract class Bonus : Cell
    {
        public Bonus(int _X, int _Y)
            : base(_X, _Y)
        {
            isWalkable = false;
            collision = false;
        }
        public override string GetCellType()
        {
            return "Bonus";
        }
        public override void DurationEnd()
        {
            base.DurationEnd();
        }
    }
    class Freeze : Bonus
    {
        public Freeze(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusFreeze.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            endTime = frameCount + 400;
            Board.eventsList.Add(new int[] { Y, X });
            Board.enemiesAreFrozen = true;
            image = "Recources\\Empty.png";
        }
        public override void DurationEnd()
        {
            base.DurationEnd();
            Board.enemiesAreFrozen = false;
            Board.currentMap[Y, X] = new Empty(X, Y);
        }
    }
    class Boost : Bonus
    {
        public Boost(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusBoost.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            if (Player.speed >= 4)
            {
                endTime = frameCount + 400;
                Board.eventsList.Add(new int[] { Y, X });
                Player.speed -= 3;
            }
            image = "Recources\\Empty.png";
        }
        public override void DurationEnd()
        {
            base.DurationEnd();
            Player.speed = Player.baseSpeed;
            Board.currentMap[Y, X] = new Empty(X, Y);
        }
    }
    class KillAllHumans : Bonus
    {
        public KillAllHumans(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusKillAllHumans.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            foreach (Enemy enemy in Board.enemyList)
            {
                Board.KillEnemy(enemy);
            }
            Board.currentMap[Y, X] = new Empty(X, Y);
        }
    }
    class MineSpawner : Bonus
    {
        public MineSpawner(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusMineSpawner.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            Board.currentMap[Y, X] = new Mine(X, Y);
        }
    }
    class Mine : Cell
    {
        public Mine(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusMine.png";
            isWalkable = false;
            collision = false;
        }
        public override string GetCellType()
        {
            return "Mine";
        }
    }
    class AddLife : Bonus
    {
        public AddLife(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\BonusAddLife.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            Board.lives += 1;
            Board.WritePoints($"Lives : {Board.lives}   ", 30);
            Board.currentMap[Y, X] = new Empty(X, Y);
        }
    }
    class RandomTeleport : Bonus
    {
        public RandomTeleport(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\BonusRandomTeleport.png";
        }
        public override void Activate(int frameCount)
        {
            int randomX;
            int randomY;
            base.Activate(frameCount);
            while (true)
            {
                randomX = new Random().Next(3, Board.currentMap.map.GetUpperBound(1) - 2);
                randomY = new Random().Next(3, Board.currentMap.map.GetUpperBound(0) - 2);
                if (Board.currentMap[randomY, randomX].GetCellType() == "Empty" && Board.currentMap[randomY, randomX].currentUnit != "Enemy" && Board.currentMap[randomY + 1, randomX].currentUnit != "Enemy" && Board.currentMap[randomY - 1, randomX].currentUnit != "Enemy" && Board.currentMap[randomY, randomX + 1].currentUnit != "Enemy" && Board.currentMap[randomY, randomX + 2].currentUnit != "Enemy" && Board.currentMap[randomY, randomX - 1].currentUnit != "Enemy" && Board.currentMap[randomY, randomX - 2].currentUnit != "Enemy")
                {
                    Board.currentMap[Y, X] = new Empty(X, Y);
                    Board.currentMap[Y, X].DrawSelf();
                    Player.SetPlayerPosition(randomX, randomY);
                    break;
                }
            }
        }
    }
    class BonusPoints : Bonus
    {
        public BonusPoints(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusBonusPoints.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            Board.points += 300;
            Board.WritePoints($"Score : {Board.points}pts   ", 0);
            Board.currentMap[Y, X] = new Empty(X, Y);
        }
    }
    class SlowEnemies : Bonus
    {
        public SlowEnemies(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\BonusSlow.png";
        }
        public override void Activate(int frameCount)
        {
            base.Activate(frameCount);
            endTime = frameCount + 700;
            Board.eventsList.Add(new int[] { Y, X });
            Board.gamespeed = 2;
            image = "Recources\\Empty.png";
        }
        public override void DurationEnd()
        {
            base.DurationEnd();
            Board.gamespeed = 1;
            Board.currentMap[Y, X] = new Empty(X, Y);
        }

    }
    class Wall : Cell
    {
        public bool isAHole = false;
        public Wall(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\Wall.png";
            isWalkable = true;
            collision = true;
        }
        public override string GetCellType()
        {
            if (!isAHole)
            {
                return "Wall";
            }
            else
            {
                return "Hole";
            }
        }
        public override void DrawSelf()
        {
            if (!isAHole)
            {
                Board.Draw("Recources\\Wall.png", X, Y);
            }
            else
            {
                Board.Draw("Recources\\Empty.png", X, Y);
            }
        }
        public override void Activate(int frameCount)
        {
            isAHole = true;
            isWalkable = false;
            collision = false;
            endTime = frameCount + 300;
            Board.eventsList.Add(new int[] { Y, X });
        }
        public override async void DurationEnd()
        {
            image = "Recources\\Wall.png";
            isAHole = false;
            collision = true;
            isWalkable = true;
            endTime = -1;
            if (currentUnit == "Enemy")
            {
                foreach (Enemy enemy in Board.enemyList)
                {
                    if (enemy.enemyX == X && enemy.enemyY == Y)
                    {
                        Board.KillEnemy(enemy);
                    }
                }
            }
            Board.Draw(image, X, Y);
            if (currentUnit == "Player")
            {
                await Board.KillPlayerViaDispatcher();
            }
        }
    }

    class Rock : Cell
    {
        public Rock(int _X, int _Y)
            : base(_X, _Y)
        {
            image = "Recources\\Rock.png";
            isWalkable = true;
            collision = true;
        }
        public override string GetCellType()
        {
            return "Rock";
        }
    }
    class Ladder : Cell
    {
        public Ladder(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\Ladder.png";
            isWalkable = true;
            collision = false;
        }
        public override string GetCellType()
        {
            return "Ladder";
        }
    }
    class Rope : Cell
    {
        public Rope(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\Rope.png";
            isWalkable = false;
            collision = false;
        }
        public override string GetCellType()
        {
            return "Rope";
        }
    }
    class Empty : Cell
    {
        public Empty(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\Empty.png";
            isWalkable = false;
            collision = false;
        }
        public override string GetCellType()
        {
            return "Empty";
        }
    }
    class Gold : Bonus
    {
        public Gold(int _X, int _Y)
           : base(_X, _Y)
        {
            image = "Recources\\Gold.png";
        }
        public override string GetCellType()
        {
            return "Gold";
        }
        public override void Activate(int frameCount)
        {
            Board.gold--;
            Board.points += 200;
            Board.WritePoints($"Score : {Board.points}pts   ", 0);
            Board.currentMap[Y, X] = new Empty(X, Y);
            if (Board.gold == 0)
            {
                Board.Win();
            }
        }
    }
}
