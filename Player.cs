namespace LodeRunnerGraphics
{
    class Player
    {
        public static bool isFalling = false;
        public static string moveDirection;
        public static string playerImage = "Recources\\Player.png";
        public static int baseSpeed = 7;
        public static int speed = baseSpeed;
        public static Field currentMap;
        public static int playerX;
        public static int playerY;
        public Player(string image)
        {
            playerImage = image;
            currentMap = Board.currentMap;
        }
        public static void Dig(int digDirection, int frameCount)
        {
            if (currentMap[playerY + 1, playerX + digDirection].GetCellType() == "Wall" && currentMap[playerY, playerX + digDirection].collision == false)
            {
                currentMap[playerY + 1, playerX + digDirection].Activate(frameCount);
                currentMap[playerY + 1, playerX + digDirection].DrawSelf();
                currentMap[playerY + 1, playerX + digDirection].isWalkable = false;
                currentMap[playerY + 1, playerX + digDirection].collision = false;
            }
        }

        public static void Step(string stepDirection)
        {
            switch (stepDirection)
            {
                case "up":
                    if (currentMap[playerY, playerX].GetCellType() == "Ladder" && !currentMap[playerY - 1, playerX].collision && !isFalling)
                    {
                        currentMap[playerY, playerX].DrawSelf();
                        Board.Draw(playerImage, playerX, playerY - 1);
                        if (currentMap[playerY - 1, playerX].GetCellType() == "Bonus" || currentMap[playerY - 1, playerX].GetCellType() == "Gold")
                        {
                            currentMap[playerY - 1, playerX].Activate(Board.frameCount);
                        }
                        currentMap[playerY, playerX].currentUnit = "None";
                        currentMap[playerY - 1, playerX].currentUnit = "Player";
                        playerY -= 1;
                        break;
                    }
                    break;
                case "down":
                    if (!currentMap[playerY + 1, playerX].collision)
                    {
                        currentMap[playerY, playerX].DrawSelf();
                        if (currentMap[playerY + 1, playerX].GetCellType() == "Bonus" || currentMap[playerY + 1, playerX].GetCellType() == "Gold")
                        {
                            currentMap[playerY + 1, playerX].Activate(Board.frameCount);
                        }
                        currentMap[playerY, playerX].currentUnit = "None";
                        currentMap[playerY + 1, playerX].currentUnit = "Player";
                        Board.Draw(playerImage, playerX, playerY + 1);
                        playerY += 1;
                        break;
                    }
                    break;
                case "right":
                    if (!currentMap[playerY, playerX + 1].collision && !isFalling)
                    {
                        currentMap[playerY, playerX].DrawSelf();
                        if (currentMap[playerY, playerX + 1].GetCellType() == "Bonus" || currentMap[playerY, playerX + 1].GetCellType() == "Gold")
                        {
                            currentMap[playerY, playerX + 1].Activate(Board.frameCount);
                        }
                        currentMap[playerY, playerX].currentUnit = "None";
                        currentMap[playerY, playerX + 1].currentUnit = "Player";
                        Board.Draw(playerImage, playerX + 1, playerY);
                        playerX += 1;
                        break;
                    }
                    break;
                case "left":
                    if (!currentMap[playerY, playerX - 1].collision && !isFalling)
                    {
                        currentMap[playerY, playerX].DrawSelf();
                        if (currentMap[playerY, playerX - 1].GetCellType() == "Bonus" || currentMap[playerY, playerX - 1].GetCellType() == "Gold")
                        {
                            currentMap[playerY, playerX - 1].Activate(Board.frameCount);
                        }
                        currentMap[playerY, playerX].currentUnit = "None";
                        currentMap[playerY, playerX - 1].currentUnit = "Player";
                        Board.Draw(playerImage, playerX - 1, playerY);
                        playerX -= 1;
                        break;
                    }
                    break;
            }
        }


        public static void SetPlayerPosition(int positionX, int positionY)
        {
            if (playerX != 0 && playerY != 0)
            {
                currentMap[playerY, playerX].DrawSelf();
                currentMap[playerY, playerX].currentUnit = "None";
            }
            playerX = positionX;
            playerY = positionY;
            currentMap[playerY, playerX].currentUnit = "Player";
        }
    }
}
