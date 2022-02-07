using FGame.Camera;
using FGame.Core;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Menus;
using FGame.Objects;
using GameMenu.Objects;
using Legend.Core.Management;
using Legend.Core.Structures;
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
        private MouseInputManager2 mouseInputManager2;
        private Grid grid;
        Player player1;
        private PathFinder pathFinder;

        private Pioneer pioneer;
        private Pioneer pioneer2;

        private TurnManager turnManager;
        private GameMenuManager gameMenuManager;
        private UIManager uiManager;
        private UnitManager unitManager;

        Button cancelMovementButton;

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
            CreateManagers();
            CreateUIButtons();
            CreatePlayers();
            CreateTestUnits();
            CreateGameMenus();

            turnManager.SetPlayerTurn();
            unitManager.AddUnits(turnManager.ActivePlayer.Units);

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
            mouseInputManager2 = new MouseInputManager2();

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

            Debug.WriteLine(cellWidth);

            XmlNodeList cellList = xmlDocument.SelectNodes("Grid/Cells/Cell");

            grid = new Grid(cellsX * cellWidth, cellsY * cellHeight, cellsX, cellsY, cellWidth, cellHeight);

            grid.Generate();

            pathFinder = new PathFinder(grid);

        }

        private void DrawMap()
        {

            foreach (KeyValuePair<int[],Cell> kvp in grid.Cells)
            {

                Tile tile = new Tile(Content.Load<Texture2D>("bg"), kvp.Value.Width, kvp.Value.Height);

                tile.WorldX = kvp.Value.GridX * kvp.Value.Width;
                tile.WorldY = kvp.Value.GridY * kvp.Value.Height;
                tile.DrawX = tile.WorldX;
                tile.DrawY = tile.WorldY;

                gameSpriteManager.AddSprite(tile);

                tile.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
                tile.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            }

        }

        private void CreateManagers()
        {
           
            gameMenuManager = new GameMenuManager();
            uiManager = new UIManager();
            unitManager = new UnitManager();
            turnManager = new TurnManager();

        }

        private void CreateUIButtons()
        {


            Button passTurnButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);
            passTurnButton.Color = Color.Red;

            passTurnButton.DrawLayer = 1;

            passTurnButton.OverColor = Color.DarkRed;
            passTurnButton.OutColor = Color.Red;

            passTurnButton.Name = "passTurnButton";

            passTurnButton.Active = true;

            uiManager.AddSprite(passTurnButton);
            Renderer.SpriteList.Add(passTurnButton);

            passTurnButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            passTurnButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            passTurnButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            

        }

        private void CreatePlayers()
        {

            player1 = new Player("Arron");
            turnManager.AddPlayerToTurnList(player1);

        }

        private void CreateTestUnits()
        {

            pioneer = new Pioneer(Content.Load<Texture2D>("player"), 32, 32);

            pioneer.Name = "pioneer";

            pioneer.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
            pioneer.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            pioneer.WorldX = 32 * 2;
            pioneer.WorldY = 32 * 2;

            pioneer.DrawX = pioneer.WorldX;
            pioneer.DrawY = pioneer.WorldY;

            player1.AddUnitToUnitList(pioneer);

            gameSpriteManager.AddSprite(pioneer);

            pioneer2 = new Pioneer(Content.Load<Texture2D>("player"), 32, 32);           

            pioneer2.Name = "pioneer2";

            pioneer2.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
            pioneer2.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            pioneer2.WorldX = 32 * 8;
            pioneer2.WorldY = 32 * 8;

            pioneer2.DrawX = pioneer.WorldX;
            pioneer2.DrawY = pioneer.WorldY;

            player1.AddUnitToUnitList(pioneer2);

            gameSpriteManager.AddSprite(pioneer2);


        }

        private void CreateGameMenus()
        {

            Button buildButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);
            Button actionButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);
            Button closeButton = new Button(Content.Load<Texture2D>("smallButtonBg"), 32, 32);

            Menu menu = new Menu(Content.Load<Texture2D>("menuBg"), 64, 32, closeButton);
            menu.Name = "pioneerMenu";

            buildButton.Color = Color.LightBlue;
            actionButton.Color = Color.CornflowerBlue;

            buildButton.OverColor = Color.DarkBlue;
            actionButton.OverColor = Color.DarkBlue;

            buildButton.OutColor = buildButton.Color;
            actionButton.OutColor = actionButton.Color;

            closeButton.Color = Color.Red;

            buildButton.Name = "buildButton";
            actionButton.Name = "pioneerActionButton";
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

            menu.Color = Color.Black;

            gameMenuManager.AddMenu(menu.Name, menu);

            //****************

            Button settlementButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);
            Button closeButton2 = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);

            Menu buildMenu = new Menu(Content.Load<Texture2D>("menuBg"), 64, 32, closeButton2);

            buildMenu.Name = "buildMenu";

            settlementButton.Color = Color.LightGreen;
            settlementButton.OverColor = Color.DarkGreen;
            settlementButton.OutColor = settlementButton.Color;

            buildMenu.AddButton(settlementButton);

            settlementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            settlementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            settlementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton2.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            settlementButton.Name = "buildSettlementButton";
            closeButton2.Name = "closeButton";

            closeButton2.Color = Color.Red;

            buildMenu.Hide();

            gameMenuManager.AddMenu(buildMenu.Name, buildMenu);

            //******************

             cancelMovementButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);
            Button closeButton3 = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16);

            Menu actionMenu = new Menu(Content.Load<Texture2D>("menuBg"), 64, 32, closeButton3);

            actionMenu.Name = "pioneerActionMenu";
            cancelMovementButton.Name = "cancelMovementButton";
            closeButton3.Name = "closeButton";

            closeButton3.Color = Color.Red;

            cancelMovementButton.Color = Color.Yellow;
            cancelMovementButton.OverColor = Color.YellowGreen;
            cancelMovementButton.OutColor = cancelMovementButton.Color;

            actionMenu.AddButton(cancelMovementButton);

            cancelMovementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            cancelMovementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            cancelMovementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton3.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            actionMenu.Hide();

            gameMenuManager.AddMenu(actionMenu.Name, actionMenu);

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

            Debug.WriteLine(button.Name);

            Debug.WriteLine(cancelMovementButton.Active);

            if (button.Name.Contains("passTurnButton"))
            {

                turnManager.ActivePlayer.IsTurnComplete = true;

                if (turnManager.CheckGlobalTurnComplete())
                {

                    turnManager.ResetPlayers();
                    unitManager.ResetPlayerUnits(turnManager.Players);

                    Debug.WriteLine("Global turn over");

                }

                Debug.WriteLine("Pass turn to next player");

                turnManager.SetPlayerTurn();             
                unitManager.CheckForAutomatedUnitTurns();
                unitManager.AddUnits(turnManager.ActivePlayer.Units);

            }

            if (button.Name.Contains("closeButton"))
            {

                gameMenuManager.HideMenu();

            }

            if (button.Name.Contains("buildButton"))
            {

                gameMenuManager.HideMenu();
                gameMenuManager.ShowMenu("buildMenu", (int)e.MouseScreenPosition.X, (int)e.MouseScreenPosition.Y, Menu.VERTICAL_STACK, 100);

            }

            if (button.Name.Contains("pioneerActionButton"))
            {

                Debug.WriteLine("Action button selected");
                gameMenuManager.HideMenu();
                gameMenuManager.ShowMenu("pioneerActionMenu", (int)e.MouseScreenPosition.X, (int)e.MouseScreenPosition.Y, Menu.VERTICAL_STACK, 100);


            }

            if (button.Name.Contains("cancelMovementButton"))
            {

                Debug.WriteLine("Jounrey has been cancelled");
                if (unitManager.ActiveUnit.AutomatedMovement)
                {

                    Debug.WriteLine("Jounrey has been cancelled");
                    unitManager.ActiveUnit.JourneyReset();
                    gameMenuManager.HideMenu();

                }

            }

            if (button.Name.Contains("buildSettlementButton"))
            {

                if (unitManager.ActiveUnit is Pioneer)
                {

                    gameMenuManager.HideMenu();
                    Pioneer pioneer = (Pioneer)unitManager.ActiveUnit;

                    pioneer.Build("settlement", Content.Load<Texture2D>("bg"),32,32);

                }            

            }

        }

        private void OnButtonOver(MouseEvent e)
        {

            Button button = (Button)e.Target;
            button.ApplyEffect(Button.OVER);

        }

        private void OnButtonOut(MouseEvent e)
        {

            Button button = (Button)e.Target;
            button.ApplyEffect(Button.OUT);

        }

        private void OnClick(MouseEvent e)
        {

            Sprite target = (Sprite)e.Target;

            if (!unitManager.AutomationMode)
            {

                if (e.Type == MouseEvent.LEFT_CLICK)
                {

                    if (target is Pioneer)
                    {

                        Debug.WriteLine("Pioneer");

                        Pioneer pioneer = (Pioneer)e.Target;

                        if (unitManager.UnitOfActivePlayer(pioneer))
                        {
                            Debug.WriteLine("Is player unit");
                            unitManager.ActiveUnit = pioneer;
                            Renderer.Camera.Target = pioneer;

                        }

                    }

                    if (target is Tile)
                    {

                        if (unitManager.ActiveUnit != null && !unitManager.ActiveUnit.IsTurnOver && gameMenuManager.ActiveMenu == null)
                        {

                            if (!unitManager.ActiveUnit.IsMoving)
                            {

                                Renderer.Camera.Target = unitManager.ActiveUnit;

                                //pick new path
                                List<Cell> path = pathFinder.BreadthFirstSearch(grid.GetCellByXY(unitManager.ActiveUnit.WorldX / grid.CellWidth, unitManager.ActiveUnit.WorldY / grid.CellHeight), grid.GetCellByXY((int)(e.MouseWorldPosition.X / grid.CellWidth), (int)(e.MouseWorldPosition.Y / grid.CellHeight)));

                                unitManager.ActiveUnit.PrepForMovememnt(path);
                                unitManager.ActiveUnit.TurnStarted = true;

                            }

                        }

                    }

                }


                if (e.Type == MouseEvent.RIGHT_CLICK)
                {

                    Debug.WriteLine(e.MouseScreenPosition);
                    Debug.WriteLine(e.MouseWorldPosition);

                    if (target is Tile)
                    {
                        
                        Renderer.Camera.Target = target;

                    }

                    //Open unit menu
                    if (target is Pioneer && gameMenuManager.ActiveMenu == null)
                    {

                        gameMenuManager.ShowMenu("pioneerMenu", (int)e.MouseScreenPosition.X , (int)e.MouseScreenPosition.Y, Menu.VERTICAL_STACK, 100);
                        Menu menu = gameMenuManager.GetMenu("pioneerMenu");                       

                    }

                }

            }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            mouseInputManager.Update();
            mouseInputManager2.Update(Mouse.GetState(this.Window).X, Mouse.GetState(this.Window).Y);
            turnManager.Update(gameTime);
            unitManager.Update(gameTime);
            base.Update(gameTime);
            

        }

        protected override void Draw(GameTime gameTime)
        {

            Renderer.GraphicsDeviceManager.GraphicsDevice.Clear(Color.Black);

            if (Renderer.Camera.Target != null)
            {

                Renderer.Camera.LookAt(new Vector2(Renderer.Camera.Target.WorldX, Renderer.Camera.Target.WorldY));

            }

            gameSpriteManager.Render();
            gameMenuManager.Render();
            uiManager.Render();

            base.Draw(gameTime);
            

        }
    }
}
