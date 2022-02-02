using FGame.Camera;
using FGame.Core;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Menus;
using FGame.Objects;
using GameMenu.Objects;
using Legend.Core.Tiles;
using Legend.Core.Units;
using Legend.Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PathFinding.PathFinding;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml;

namespace Legend {
    public class Game1 : Microsoft.Xna.Framework.Game {

        private GameSpriteManager gameSpriteManager;
        private MouseInputManager mouseInputManager;
        private Grid grid;

        private PathFinder pathFinder;

        private Pioneer pioneer;

        private TurnManager turnManager;

        private GameMenuManager gameMenuManager;

        Player player1;

        private List<Sprite> test = new List<Sprite>();

        public Game1()
        {

            Renderer.InitGraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Renderer.GraphicsDeviceManager.PreferredBackBufferWidth = 1280;
            Renderer.GraphicsDeviceManager.PreferredBackBufferHeight = 800;

            Renderer.GraphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
      
            InitCamera();
            InitInputs();
            LoadGrid();
            DrawMap();
            CreateTurnManager();
            CreateGameMenuManager();
            //CreateUIManager();
            CreatePlayers();
            CreateTestUnits();
            CreateGameMenus();
            SetPlayerTurn(player1);
            
        }

        private void InitCamera()
        {

            Renderer.InitSpriteBatcher();
            Renderer.InitCamera(0, 0, 1280, 800);
            Renderer.InitViewPorts(0, 0, 1280, 800);
            gameSpriteManager = new GameSpriteManager();
            Renderer.Camera.LookAt(new Vector2(0, 0));

        }

        private void InitInputs()
        {

            mouseInputManager = new MouseInputManager(Renderer.Camera, this.Window);

        }

        private void LoadGrid()
        {

            string xmlURL = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/gridXML/testGrid.xml";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            XmlNode dataNode = xmlDocument.SelectSingleNode("Grid/Data");

            int cellsX = int.Parse(dataNode.Attributes.GetNamedItem("CellsX").InnerText);
            int cellsY = int.Parse(dataNode.Attributes.GetNamedItem("CellsY").InnerText);
            int cellWidth = int.Parse(dataNode.Attributes.GetNamedItem("CellWidth").InnerText);
            int cellHeight = int.Parse(dataNode.Attributes.GetNamedItem("CellHeight").InnerText);

            XmlNodeList cellList = xmlDocument.SelectNodes("Grid/Cells/Cell");

            grid = new Grid(cellsX * cellWidth, cellsY * cellHeight, cellsX, cellsY, cellWidth, cellHeight);

            grid.Generate();

            pathFinder = new PathFinder(grid);

        }

        private void DrawMap()
        {

            foreach (KeyValuePair<int[],Cell> kvp in grid.Cells)
            {

                Tile tile = new Tile(Content.Load<Texture2D>("gravel"), kvp.Value.Width, kvp.Value.Height);
                tile.X = kvp.Value.GridX * kvp.Value.Width;
                tile.Y = kvp.Value.GridY * kvp.Value.Height;
                gameSpriteManager.AddSprite(tile);

                tile.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);                

            }

        }

        private void CreateTurnManager()
        {

            turnManager = new TurnManager();

        }

        private void CreateGameMenuManager()
        {

            gameMenuManager = new GameMenuManager();

        }

        private void CreateUIManager()
        {

        }

        private void CreatePlayers()
        {

            player1 = new Player("Arron");
            turnManager.AddPlayerToTurnList(player1);

        }

        private void CreateTestUnits()
        {

            pioneer = new Pioneer(Content.Load<Texture2D>("player"), 32, 32);

            pioneer.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            pioneer.X = 32 * 2;
            pioneer.Y = 32 * 2;
           
            player1.AddUnitToUnitList(pioneer);

            gameSpriteManager.AddSprite(pioneer);

        }

        private void CreateGameMenus()
        {

            Sprite sprite = new Sprite(Content.Load<Texture2D>("bg"), 50, 40);

            Button buildButton = new Button(Content.Load<Texture2D>("bg"), 50, 20);
            Button actionButton = new Button(Content.Load<Texture2D>("bg"), 50, 20);
            Button closeButton = new Button(Content.Load<Texture2D>("bg"), 20, 20);

            Menu menu = new Menu(sprite.Width, sprite.Height, sprite, closeButton);

            buildButton.Color = Color.LightBlue;
            actionButton.Color = Color.CornflowerBlue;

            buildButton.OverColor = Color.DarkBlue;
            actionButton.OverColor = Color.DarkBlue;

            buildButton.OutColor = buildButton.Color;
            actionButton.OutColor = actionButton.Color;

            buildButton.Name = "buildButton";
            actionButton.Name = "actionButton";
            closeButton.Name = "closeButton";

            menu.AddButton(buildButton);
            menu.AddButton(actionButton);

            buildButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            buildButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            buildButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            actionButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            actionButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            actionButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            menu.Hide();

            menu.BackgroundSprite.Color = Color.Black;

            gameMenuManager.AddMenu("pioneerMenu", menu);

            Button settlementButton = new Button(Content.Load<Texture2D>("bg"), 50, 20);
            Button closeButton2 = new Button(Content.Load<Texture2D>("bg"), 20, 20);

            Menu buildMenu = new Menu(sprite.Width, sprite.Height, sprite, closeButton2);

            settlementButton.Color = Color.LightGreen;
            settlementButton.OverColor = Color.DarkGreen;
            settlementButton.OutColor = settlementButton.Color;

            buildMenu.AddButton(settlementButton);

            settlementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            settlementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            settlementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton2.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            settlementButton.Name = "settlementButton";
            closeButton2.Name = "closeButton";

            buildMenu.Hide();

            gameMenuManager.AddMenu("buildMenu", buildMenu);

        }

        private void SetPlayerTurn(Player player)
        {

            turnManager.ActivePlayer = player;
            turnManager.ActiveUnit = player.Units[0];

        }

        private void OnOver(MouseEvent e)
        {


        }

        private void OnOut(MouseEvent e)
        {
 
        }

        private void OnButtonClick(MouseEvent e)
        {

            Button button = (Button)e.Target;

            if (button.Name.Contains("closeButton"))
            {

                button.ParentMenu.Hide();

            }

            if (button.Name.Contains("buildButton"))
            {

                Debug.WriteLine("Build button selected");
                button.ParentMenu.Hide();

                gameMenuManager.ShowMenu("buildMenu", (int)e.MouseScreenPosition.X, (int)e.MouseScreenPosition.Y);

                Menu menu = gameMenuManager.GetMenu("buildMenu");
                menu.StackButtonsVertically(100);

            }

            if (button.Name.Contains("actionButton"))
            {

                Debug.WriteLine("Action button selected");

            }

            if (button.Name.Contains("settlementButton"))
            {

                Debug.WriteLine("Settlement button clicked");

            }

        }

        private void OnButtonOver(MouseEvent e)
        {

            Button button = (Button)e.Target;
            button.ApplyEffect(Button.OVER);
            Debug.WriteLine(button.Name+" over");

        }

        private void OnButtonOut(MouseEvent e)
        {

            Button button = (Button)e.Target;
            button.ApplyEffect(Button.OUT);

        }

        private void OnClick(MouseEvent e)
        {

            Sprite target = (Sprite)e.Target;

            if (e.Type == MouseEvent.LEFT_CLICK)
            {

                if (target is Pioneer)
                {

                    Debug.WriteLine("Pioneer");
                    

                }

                if(target is Tile)
                {

                    if (!turnManager.ActiveUnit.IsMoving)
                    {

                        //pick new path
                        List<Cell> path = pathFinder.BreadthFirstSearch(grid.GetCellByXY(turnManager.ActiveUnit.X / grid.CellWidth, turnManager.ActiveUnit.Y / grid.CellHeight), grid.GetCellByXY((int)(e.MouseWorldPosition.X / grid.CellWidth), (int)(e.MouseWorldPosition.Y / grid.CellHeight)));

                        turnManager.ActiveUnit.PrepForMovememnt(path);
                        turnManager.ActiveUnit.TurnStarted = true;

                    }

                }

            }


            if (e.Type == MouseEvent.RIGHT_CLICK)
            {

                //Open unit menu
                if (target is Pioneer)
                {

                   gameMenuManager.ShowMenu("pioneerMenu", (int)e.MouseScreenPosition.X, (int)e.MouseScreenPosition.Y);

                    Menu menu = gameMenuManager.GetMenu("pioneerMenu");
                    menu.StackButtonsVertically(100);

                }            

            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            turnManager.ActiveUnit.Update(gameTime);
            turnManager.Update();

            mouseInputManager.Update();           
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {

            Renderer.GraphicsDeviceManager.GraphicsDevice.Clear(Color.White);
            Renderer.Camera.LookAt(new Vector2(turnManager.ActiveUnit.X, turnManager.ActiveUnit.Y));

            gameSpriteManager.Render();
            gameMenuManager.Render();
            Renderer.DrawUI(test);
            
            base.Draw(gameTime);

        }
    }
}
