namespace LodeRunnerGraphics
{
    class Enemy
    {
        public bool isFalling = false;
        public string enemyImage = "T";
        static Cell[,] currentMap;
        public int enemyX;
        public int enemyY;
        private int stepNumber = 0;
        public int enemySpeed = 10;
        public int releaseAt = -1;
        public int ressurectAt = -1;
        public void StartRessurectTimer(int frameCount)
        {
            releaseAt = -1;
            SetEnemyPosition(1, 1);
            currentMap[1, 1].DrawSelf();
            if (ressurectAt == -1)
            {
                ressurectAt = frameCount + 500;
            }
        }
        public void Ressurect()
        {
            ressurectAt = -1;
            SetEnemyPosition((10 + Board.enemyList.IndexOf(this) * (currentMap.GetUpperBound(1) - 20) / Board.enemyNumber), 2);
        }
        public void StartReleaseTimer(int frameCount)
        {
            if (releaseAt == -1)
            {
                currentMap[enemyY, enemyX].isWalkable = true;
                releaseAt = frameCount + 200;
            }
        }
        public void Release()
        {
            if (!currentMap[enemyY - 1, enemyX].collision && currentMap[enemyY, enemyX].GetCellType() != "Wall")
            {
                releaseAt = -1;
                currentMap[enemyY, enemyX].isWalkable = false;
                if (Player.playerX > enemyX && !currentMap[enemyY - 1, enemyX + 1].collision)
                {
                    Board.Draw(enemyImage, enemyX + 1, enemyY - 1);
                    Board.Draw("Recources\\Empty.png", enemyX, enemyY);
                    enemyX++;
                    enemyY--;
                }
                else if (!currentMap[enemyY - 1, enemyX - 1].collision)
                {
                    Board.Draw(enemyImage, enemyX - 1, enemyY - 1);
                    Board.Draw("Recources\\Empty.png", enemyX, enemyY);
                    enemyX--;
                    enemyY--;
                }
            }
        }
        public int EnemySpeed
        {
            get
            {
                return enemySpeed;
            }
            set
            {
                if (value > 7)
                {
                    enemySpeed = value;
                }
                else
                {
                    enemySpeed = 10;
                }
            }
        }
        public int enemyPathResetDelay = 2;
        private string[] path = new string[3];
        public Enemy(string image, int speed)
        {
            EnemySpeed = speed;
            enemyImage = image;
            currentMap = Board.currentMap.map;
        }
        public void SetEnemyPosition(int positionX, int positionY)
        {
            currentMap = Board.currentMap.map;
            if (enemyX != 0 && enemyY != 0)
            {
                currentMap[enemyY, enemyX].DrawSelf();
            }
            enemyX = positionX;
            enemyY = positionY;
            Board.Draw(enemyImage, enemyX, enemyY);
        }
        public void Step(string stepDirection)
        {
            switch (stepDirection)
            {
                case "up":
                    if (currentMap[enemyY, enemyX].GetCellType() == "Ladder" && !currentMap[enemyY - 1, enemyX].collision && !isFalling)
                    {
                        currentMap[enemyY, enemyX].DrawSelf();
                        currentMap[enemyY, enemyX].currentUnit = "None";
                        currentMap[enemyY - 1, enemyX].currentUnit = "Enemy";
                        Board.Draw(enemyImage, enemyX, enemyY - 1);
                        enemyY -= 1;
                        break;
                    }
                    break;
                case "down":
                    if (currentMap[enemyY, enemyX].GetCellType() == "Rope" && !currentMap[enemyY + 1, enemyX].collision || currentMap[enemyY + 1, enemyX].GetCellType() == "Ladder" && !currentMap[enemyY + 1, enemyX].collision || !currentMap[enemyY + 1, enemyX].isWalkable && currentMap[enemyY, enemyX].GetCellType() != "Hole" && !currentMap[enemyY, enemyX].collision)
                    {
                        currentMap[enemyY, enemyX].DrawSelf();
                        currentMap[enemyY, enemyX].currentUnit = "None";
                        currentMap[enemyY + 1, enemyX].currentUnit = "Enemy";
                        Board.Draw(enemyImage, enemyX, enemyY + 1);
                        enemyY += 1;
                        break;
                    }
                    break;
                case "right":
                    if (!currentMap[enemyY, enemyX + 1].collision && !isFalling && currentMap[enemyY + 1, enemyX + 1].isWalkable && currentMap[enemyY, enemyX].GetCellType() != "Hole" || !isFalling && currentMap[enemyY + 1, enemyX + 1].GetCellType() == "Hole")
                    {
                        currentMap[enemyY, enemyX].DrawSelf();
                        currentMap[enemyY, enemyX].currentUnit = "None";
                        currentMap[enemyY, enemyX + 1].currentUnit = "Enemy";
                        Board.Draw(enemyImage, enemyX + 1, enemyY);
                        enemyX += 1;
                        break;
                    }
                    break;
                case "left":
                    if (!currentMap[enemyY, enemyX - 1].collision && !isFalling && currentMap[enemyY + 1, enemyX - 1].isWalkable && currentMap[enemyY, enemyX].GetCellType() != "Hole" || !isFalling && currentMap[enemyY + 1, enemyX - 1].GetCellType() == "Hole")
                    {
                        currentMap[enemyY, enemyX].DrawSelf();
                        currentMap[enemyY, enemyX].currentUnit = "None";
                        currentMap[enemyY, enemyX - 1].currentUnit = "Enemy";
                        Board.Draw(enemyImage, enemyX - 1, enemyY);
                        enemyX -= 1;
                        break;
                    }
                    break;
            }
        }
        public void EnemyStep(int frameCount)
        {
            if (!currentMap[enemyY + 1, enemyX].isWalkable)
            {
                isFalling = true;
            }
            else
            {
                isFalling = false;
            }
            if (isFalling)
            {
                Step("down");
            }
            if (frameCount % (enemyPathResetDelay * enemySpeed) == 0)
            {
                path = Board.BuildPath(this, Board.player, enemyPathResetDelay + 1);
                stepNumber = 0;
            }
            if (frameCount % enemySpeed == 0 && !isFalling)
            {
                Step(path[stepNumber]);
                stepNumber++;
            }
        }

    }
}
