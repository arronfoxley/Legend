using FGame.Camera;
using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Menus;
using Legend.Core.Management;
using Legend.Core.Resources;
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
    public class Game1 : Game {

        private MouseInputManager mouseInputManager;
        private Grid grid;
        Player player1;
        private PathFinder pathFinder;

        private Pioneer pioneer;
        private Pioneer pioneer2;

        private TurnManager turnManager;
        private UnitManager unitManager;
        
        List<Button> buttons = new List<Button>();
        List<Button> activeButtons = new List<Button>();
        Vector2 screenPosition;

        private List<Resource> resources = new List<Resource>();

        private int cellSize = 48;

        public Game1()
        {

            Renderer.InitGraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Renderer.GraphicsDeviceManager.PreferredBackBufferWidth = 480;
            Renderer.GraphicsDeviceManager.PreferredBackBufferHeight = 480;

            Renderer.GraphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
      
            InitCamera();
            InitInputs();
            LoadXML();
            DrawMap();
            CreateManagers();
            CreateUIButtons();
            CreateGameButtons();
            CreatePlayers();
            CreateTestUnits();

            turnManager.SetPlayerTurn();
            unitManager.AddUnits(turnManager.ActivePlayer.Units);

        }

        private void InitCamera()
        {

            Renderer.InitSpriteBatcher();
            Renderer.InitCamera(0, 0, 480, 480);
            Renderer.AddScreen(0, 0, 480, 480, "GameScreen");           
            Renderer.AddScreen(0, 0, 480, 480, "UIScreen");
            Renderer.Camera.LookAt(new Vector2(0, 0));

        }

        private void InitInputs()
        {

            mouseInputManager = new MouseInputManager(this.Window);

        }

        private void LoadXML()
        {

            string xmlURL = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/gridXML/testGrid.xml";
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.Load(xmlURL);

            //GRID

            XmlNode gridNode = xmlDocument.SelectSingleNode("Map/Grid/Data");

            int cellsX = int.Parse(gridNode.Attributes.GetNamedItem("CellsX").InnerText);
            int cellsY = int.Parse(gridNode.Attributes.GetNamedItem("CellsY").InnerText);
            int cellWidth = int.Parse(gridNode.Attributes.GetNamedItem("CellWidth").InnerText);
            int cellHeight = int.Parse(gridNode.Attributes.GetNamedItem("CellHeight").InnerText);

            //XmlNodeList cellList = xmlDocument.SelectNodes("Grid/Cells/Cell");

            grid = new Grid(cellsX * cellWidth, cellsY * cellHeight, cellsX, cellsY, cellWidth, cellHeight);

            grid.Generate();

            pathFinder = new PathFinder(grid);

            //RESOURCES

            XmlNodeList forestNodeList = xmlDocument.SelectNodes("Map/Terrain/Forest/Resource");

            foreach (XmlNode node in forestNodeList)
            {
                
                int cellX = int.Parse(node.Attributes.GetNamedItem("CellX").InnerText);
                int cellY = int.Parse(node.Attributes.GetNamedItem("CellY").InnerText);
                string textureName = node.Attributes.GetNamedItem("TextureName").InnerText;

                Debug.WriteLine(textureName);

                Resource resource = new Resource(Content.Load<Texture2D>(textureName),cellWidth,cellHeight);

                resource.cellX = cellX;
                resource.cellY = cellY;

                resource.WorldX = cellSize * cellX;
                resource.WorldY = cellSize * cellY;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                Renderer.AddGameSprite(resource);

                resource.Active = true;


            }

            XmlNodeList mountainNodeList = xmlDocument.SelectNodes("Map/Terrain/Mountain/Resource");

            foreach (XmlNode node in mountainNodeList)
            {

                int cellX = int.Parse(node.Attributes.GetNamedItem("CellX").InnerText);
                int cellY = int.Parse(node.Attributes.GetNamedItem("CellY").InnerText);
                string textureName = node.Attributes.GetNamedItem("TextureName").InnerText;

                Resource resource = new Resource(Content.Load<Texture2D>(textureName), cellWidth, cellHeight);

                resource.cellX = cellX;
                resource.cellY = cellY;

                resource.WorldX = cellSize * cellX;
                resource.WorldY = cellSize * cellY;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                //Set mountain cell as not passable
                grid.GetCellByXY(cellX, cellY).Passable = false;

                Renderer.AddGameSprite(resource);

                resource.Active = true;

            }

        }

        private void DrawMap()
        {

            Random random = new Random();

            foreach (KeyValuePair<int[],Cell> kvp in grid.Cells)
            {

                int i = random.Next(1, 6);

                Tile tile = new Tile(Content.Load<Texture2D>("field"+i), kvp.Value.Width, kvp.Value.Height);

                if (kvp.Value.GridY < 4)
                {

                    tile.Color = Color.YellowGreen;

                }
                else
                {

                    tile.Color = Color.LightGray;

                }

                tile.Active = true;

                tile.WorldX = kvp.Value.GridX * kvp.Value.Width;
                tile.WorldY = kvp.Value.GridY * kvp.Value.Height;
                tile.DrawX = tile.WorldX;
                tile.DrawY = tile.WorldY;

                Renderer.AddGameSprite(tile);

                //tile.Color = Color.Red;

                tile.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
                tile.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            }

            foreach (Resource resource in resources)
            {

                Renderer.AddGameSprite(resource);

                resource.WorldX = cellSize * 5;
                resource.WorldY = cellSize * 5;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                resource.Active = true;

                resource.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
                resource.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            }          

        }

        private void CreateManagers()
        {
           
            unitManager = new UnitManager();
            turnManager = new TurnManager();

        }

        private void CreateUIButtons()
        {

            Button passTurnButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "UI");
            passTurnButton.Color = Color.Red;

            passTurnButton.OverColor = Color.DarkRed;
            passTurnButton.OutColor = Color.Red;

            passTurnButton.Name = "passTurnButton";

            passTurnButton.Active = true;

            Renderer.AddUISprite(passTurnButton);      

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

            pioneer = new Pioneer(Content.Load<Texture2D>("pioneer"), cellSize, cellSize);

            pioneer.Active = true;

            pioneer.Name = "pioneer";

            pioneer.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
            pioneer.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            pioneer.WorldX = cellSize * 2;
            pioneer.WorldY = cellSize * 2;

            pioneer.DrawX = pioneer.WorldX;
            pioneer.DrawY = pioneer.WorldY;

            player1.AddUnitToUnitList(pioneer);

            Renderer.AddGameSprite(pioneer);

            pioneer2 = new Pioneer(Content.Load<Texture2D>("bg"), cellSize, cellSize);

            pioneer2.Active = true;

            pioneer2.Name = "pioneer2";

            pioneer2.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
            pioneer2.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            pioneer2.WorldX = cellSize * 8;
            pioneer2.WorldY = cellSize * 8;

            pioneer2.DrawX = pioneer.WorldX;
            pioneer2.DrawY = pioneer.WorldY;

            player1.AddUnitToUnitList(pioneer2);

            Renderer.AddGameSprite(pioneer2);

        }

        private void CreateGameButtons()
        {

            Button buildButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "build");
            Button actionButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "build");
            Button closeButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "close");

            buttons.Add(buildButton);
            buttons.Add(actionButton);
            buttons.Add(closeButton);        

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

            Renderer.AddUISprite(buildButton);
            Renderer.AddUISprite(actionButton);
            Renderer.AddUISprite(closeButton);

            buildButton.DrawLayer = Renderer.UI_DEPTH_LAYER;
            actionButton.DrawLayer = Renderer.UI_DEPTH_LAYER;
            closeButton.DrawLayer = Renderer.UI_DEPTH_LAYER;

            buildButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            buildButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            buildButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            actionButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            actionButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            actionButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            //******************

            Button cancelMovementButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "action");

            buttons.Add(cancelMovementButton);   

            cancelMovementButton.Name = "cancelMovementButton";

            cancelMovementButton.Color = Color.Yellow;
            cancelMovementButton.OverColor = Color.YellowGreen;
            cancelMovementButton.OutColor = cancelMovementButton.Color;

            Renderer.AddUISprite(cancelMovementButton);

            cancelMovementButton.DrawLayer = Renderer.UI_DEPTH_LAYER;

            cancelMovementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            cancelMovementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            cancelMovementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            //*******************

            Button settlementButton = new Button(Content.Load<Texture2D>("buttonBg"), 64, 16, "structure");

            buttons.Add(settlementButton);

            settlementButton.Color = Color.Gray;
            settlementButton.OutColor = Color.Gray;
            settlementButton.OverColor = Color.DarkSlateGray;

            settlementButton.Name = "settlementButton";

            Renderer.AddUISprite(settlementButton);

            settlementButton.DrawLayer = Renderer.UI_DEPTH_LAYER;

            settlementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            settlementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            settlementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

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

            if (button.Name.Contains("passTurnButton"))
            {

                turnManager.ActivePlayer.IsTurnComplete = true;

                if (turnManager.CheckGlobalTurnComplete())
                {

                    turnManager.ResetPlayers();
                    unitManager.UpdatePlayerUnitsForNextTurn(turnManager.Players);

                    Debug.WriteLine("Global turn over");

                }

                Debug.WriteLine("Pass turn to next player");

                turnManager.SetPlayerTurn();             
                unitManager.CheckForAutomatedUnitTurns();
                unitManager.AddUnits(turnManager.ActivePlayer.Units);

            }

            if (button.Name.Contains("buildButton"))
            {

                HideMenu();

                activeButtons = GetButtons("structure");

                ShowUnitMenu(screenPosition, unitManager.ActiveUnit);

            }

            if (button.Name.Contains("closeButton"))
            {

                HideMenu();

            }

            if (button.Name.Contains("pioneerActionButton"))
            {

                HideMenu();

                activeButtons = GetButtons("action");

                ShowUnitMenu(screenPosition, unitManager.ActiveUnit);

            }

            if (button.Name.Contains("settlementButton"))
            {

                HideMenu();

                Pioneer pioneer = (Pioneer) unitManager.ActiveUnit;
                pioneer.StartBuild("settlement", Content.Load<Texture2D>("bg"), cellSize, cellSize);

            }

            if (button.Name.Contains("cancelMovementButton"))
            {

                if (unitManager.ActiveUnit.AutomatedMovement) {

                    HideMenu();
                    unitManager.ActiveUnit.JourneyReset();
                    Debug.WriteLine("Journey cancelled");

                }
                else
                {

                    Debug.WriteLine("No Journey to cancel");

                }

            }

        }

        private void HideMenu()
        {

            for (int i = 0; i < activeButtons.Count; i++)
            {

                activeButtons[i].Active = false;

            }

            activeButtons.Clear();

        }

        private void ShowUnitMenu(Vector2 screenPosition, Unit unit)
        {

            for (int i = 0; i < activeButtons.Count; i++)
            {

                activeButtons[i].DrawX = (int)screenPosition.X + unit.Width + 1;
                activeButtons[i].DrawY = (int)screenPosition.Y + (i * activeButtons[i].Height);

                activeButtons[i].ScreenX = (int)screenPosition.X + unit.Width + 1;
                activeButtons[i].ScreenY = (int)screenPosition.Y + (i * activeButtons[i].Height);

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

            GameSprite target = (GameSprite)e.Target;

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

                        if (unitManager.ActiveUnit != null && !unitManager.ActiveUnit.IsTurnOver)
                        {

                            if (!unitManager.ActiveUnit.IsMoving)
                            {

                                if (unitManager.ActiveUnit is Pioneer)
                                {

                                    Pioneer pioneer = (Pioneer)unitManager.ActiveUnit;
                                    Renderer.Camera.Target = pioneer;

                                    if (!pioneer.IsBuilding) {

                                        MoveUnit(grid.GetCellByXY(unitManager.ActiveUnit.WorldX / grid.CellWidth, unitManager.ActiveUnit.WorldY / grid.CellHeight), 
                                            grid.GetCellByXY((int)(e.Position.X / grid.CellWidth), (int)(e.Position.Y / grid.CellHeight))
                                            );
                                    
                                    }

                                }

                            }

                        }

                    }

                }

                if (e.Type == MouseEvent.RIGHT_CLICK)
                {

                    if (target is Tile)
                    {

                        Renderer.Camera.Target = target;

                    }

                    //Open unit menu
                    if (target is Pioneer)
                    {

                        Pioneer pioneer = (Pioneer)target;

                        activeButtons = GetButtons("build");
                        screenPosition = Renderer.Camera.WorldToScreenPosition(new Vector2(target.WorldX, target.WorldY));

                        unitManager.ActiveUnit = pioneer;

                        ShowUnitMenu(screenPosition, pioneer);

                    }

                }

            }

        }

        private void MoveUnit(Cell startCell, Cell destinationCell)
        {

            //pick new path
            List<Cell> path = pathFinder.BreadthFirstSearch(startCell, destinationCell);

            unitManager.ActiveUnit.PrepForMovememnt(path);
            unitManager.ActiveUnit.TurnStarted = true;

        }

        private List<Button> GetButtons(string setName)
        {

            Button closeButton = null;

            for (int i = 0; i < buttons.Count; i++)
            {

                if (buttons[i].set == setName)
                {

                    activeButtons.Add(buttons[i]);
                    buttons[i].Active = true;                   

                }

                if (buttons[i].Name == "closeButton")
                {
                    closeButton = buttons[i];
                    closeButton.Active = true;

                }

            }

            activeButtons.Add(closeButton);

            return activeButtons;

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            Vector2 screenPosition = new Vector2(Mouse.GetState(this.Window).X, Mouse.GetState(this.Window).Y);
            
            mouseInputManager.Update(screenPosition);

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

            Renderer.Draw();

            base.Draw(gameTime);
            

        }
    }
}
