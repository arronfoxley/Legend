using FGame.Camera;
using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Menus;
using Legend.Core.Display;
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

        private TurnManager turnManager;
        private UnitManager unitManager;
      
        List<Button> buttons = new List<Button>();
        List<Button> activeButtons = new List<Button>();
        Vector2 screenPosition;

        private Dictionary<Cell,Terrain> resources = new Dictionary<Cell, Terrain>();
        private List< Structure> structures = new List<Structure>();

        private int cellSize = 48;

        public Game1()
        {

            Renderer.InitGraphicsDeviceManager(this);
            Content.RootDirectory = "Content/Assets/Graphics";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            Renderer.GraphicsDeviceManager.PreferredBackBufferWidth = 528;
            Renderer.GraphicsDeviceManager.PreferredBackBufferHeight = 528;

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
            CreateUnits();

            turnManager.SetPlayerTurn();
            unitManager.AddUnits(turnManager.ActivePlayer.Units);

        }

        private void InitCamera()
        {

            Renderer.InitSpriteBatcher();
            Renderer.InitCamera(0, 0, 528, 528);
            Renderer.AddScreen(0, 0, 528, 528, "GameScreen");           
            Renderer.AddScreen(0, 0, 528, 528, "UIScreen");
            Renderer.Camera.LookAt(new Vector2(528 / 2, 528 / 2));

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
                string type = node.Attributes.GetNamedItem("Type").InnerText;
                string textureName = node.Attributes.GetNamedItem("TextureName").InnerText;

                Terrain resource = new Terrain(Content.Load<Texture2D>(textureName),cellWidth,cellHeight,type);

                resource.cellX = cellX;
                resource.cellY = cellY;

                resource.WorldX = cellSize * cellX;
                resource.WorldY = cellSize * cellY;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                Renderer.AddGameSprite(resource);

                resource.Active = true;

                resources.Add( grid.GetCellByXY(cellX,cellY), resource );


            }

            XmlNodeList mountainNodeList = xmlDocument.SelectNodes("Map/Terrain/Mountain/Resource");

            foreach (XmlNode node in mountainNodeList)
            {

                int cellX = int.Parse(node.Attributes.GetNamedItem("CellX").InnerText);
                int cellY = int.Parse(node.Attributes.GetNamedItem("CellY").InnerText);
                string type = node.Attributes.GetNamedItem("Type").InnerText;
                string textureName = node.Attributes.GetNamedItem("TextureName").InnerText;

                Terrain resource = new Terrain(Content.Load<Texture2D>(textureName), cellWidth, cellHeight, type);

                resource.cellX = cellX;
                resource.cellY = cellY;

                resource.WorldX = cellSize * cellX;
                resource.WorldY = cellSize * cellY;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                resource.DrawLayer = GameDrawDepthList.GAME_MOUNTAIN_DEPTH_LAYER;

                //Set mountain cell as not passable
                grid.GetCellByXY(cellX, cellY).Passable = false;

                Renderer.AddGameSprite(resource);

                resource.Active = true;

                resources.Add(grid.GetCellByXY(cellX, cellY), resource);

            }

            //Shadow list

            XmlNodeList shadowNodeList = xmlDocument.SelectNodes("Map/Terrain-Shadow/Shadow");

            foreach (XmlNode node in shadowNodeList)
            {

                Debug.WriteLine(node);

                int cellX = int.Parse(node.Attributes.GetNamedItem("CellX").InnerText);
                int cellY = int.Parse(node.Attributes.GetNamedItem("CellY").InnerText);
                string textureName = node.Attributes.GetNamedItem("TextureName").InnerText;

                GameSprite shadow = new GameSprite(Content.Load<Texture2D>(textureName), cellWidth, cellHeight);

                shadow.WorldX = cellSize * cellX;
                shadow.WorldY = cellSize * cellY;

                Debug.WriteLine(shadow.WorldX);

                shadow.DrawX = shadow.WorldX;
                shadow.DrawY = shadow.WorldY;

                shadow.DrawLayer = GameDrawDepthList.GAME_MOUNTAIN_SHADOW_DEPTH_LAYER;
                shadow.Alpha = 0.5f;

                Renderer.AddGameSprite(shadow);

                shadow.Active = true;


            }

        }

        private void DrawMap()
        {

            Random random = new Random();

            foreach (KeyValuePair<int[],Cell> kvp in grid.Cells)
            {

                int i = random.Next(1, 6);

                Tile tile = new Tile(Content.Load<Texture2D>("Game/Field/field" + i), kvp.Value.Width, kvp.Value.Height);

                tile.Color = new Color(105, 137, 105);              

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

        }

        private void CreateManagers()
        {
           
            unitManager = new UnitManager(pathFinder,grid);
            turnManager = new TurnManager();

        }

        private void CreateUIButtons()
        {

            Button passTurnButton = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "UI");
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

        private void CreateUnits()
        {

            CreateUnit("pioneer", 0, 1);
            CreateUnit("lumberjack", 1, 1);
            CreateUnit("miner", 2, 1);

        }

        private void CreateUnit(string unitType, int cellX, int cellY)
        {

            Unit unit = null;

            switch (unitType)
            {

                case "pioneer": unit = new Pioneer(Content.Load<Texture2D>("Game/Units/pioneer"), cellSize, cellSize) ; break;
                case "lumberjack": unit = new LumberJack(Content.Load<Texture2D>("Game/Units/lumberjack"), cellSize, cellSize); break;
                case "miner": unit = new Miner(Content.Load<Texture2D>("Game/Units/miner"), cellSize, cellSize); break;

            }

            unit.Active = true;

            unit.Name = unitType;

            unit.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
            unit.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            unit.WorldX = cellSize * cellX;
            unit.WorldY = cellSize * cellY;

            unit.DrawX = unit.WorldX;
            unit.DrawY = unit.WorldY;

            player1.AddUnitToUnitList(unit);

            Renderer.AddGameSprite(unit);


        }

        private void CreateGameButtons()
        {

            //Pioneer Actions
            //Build Settlement
            Button buildSettlementButton = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "pioneer_action");
            //Build Road
            //Cultivate Land
            //Cancel Movement

            //Lumberjack Actions
            //Gather Lumber
            Button gatherActionButton = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "lumberjack_action");
            //Cancel Movement

            Button setHomeButton = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "all_action");
            Button cancelMovement = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "all_action");           
            Button closeButton = new Button(Content.Load<Texture2D>("Button/buttonBg"), 64, 16, "close");

            buttons.Add(buildSettlementButton);
            buttons.Add(gatherActionButton);
            buttons.Add(cancelMovement);
            buttons.Add(setHomeButton);
            buttons.Add(closeButton);

            buildSettlementButton.Color = Color.LightBlue;
            gatherActionButton.Color = Color.CornflowerBlue;
            setHomeButton.Color = Color.LightGray;
            cancelMovement.Color = Color.Red;
            closeButton.Color = Color.DarkRed;

            buildSettlementButton.OutColor = buildSettlementButton.Color;
            gatherActionButton.OutColor = gatherActionButton.Color;
            setHomeButton.OutColor = setHomeButton.Color;
            cancelMovement.OutColor = cancelMovement.Color;
            closeButton.OutColor = closeButton.Color;

            buildSettlementButton.OverColor = Color.Black;
            gatherActionButton.OverColor = Color.Black;
            setHomeButton.OverColor = Color.Black;
            cancelMovement.OverColor = Color.Black;
            closeButton.OverColor = Color.Black;

            buildSettlementButton.Name = "buildSettlementButton";
            gatherActionButton.Name = "gatherActionButton";
            setHomeButton.Name = "setHomeButton";
            cancelMovement.Name = "cancelMovementButton";
            closeButton.Name = "closeButton";

            Renderer.AddUISprite(buildSettlementButton);
            Renderer.AddUISprite(gatherActionButton);
            Renderer.AddUISprite(cancelMovement);
            Renderer.AddUISprite(closeButton);
            Renderer.AddUISprite(setHomeButton);

            buildSettlementButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            buildSettlementButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            buildSettlementButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            gatherActionButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            gatherActionButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            gatherActionButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            cancelMovement.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            cancelMovement.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            cancelMovement.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            closeButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            closeButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            closeButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

            setHomeButton.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            setHomeButton.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            setHomeButton.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

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

            if (button.Name.Contains("closeButton"))
            {

                HideMenu();

            }

            if (button.Name.Contains("setHomeButton")){

                //TODO Set units home town
                Debug.WriteLine("Select Structure");

                Boolean test = false;

                for (int i = 0; i < structures.Count; i++)
                {

                    if (structures[i] is Settlement)
                    {

                        test = true;
                        unitManager.ActiveUnit.Home = structures[i];
                        HideMenu();
                        Debug.WriteLine("Settlement exists, set as home");
                        break;

                    }

                }

                if (!test)
                {

                    Debug.WriteLine("No settlements exist, cant set home");

                }
            
            }

            if (button.Name.Contains("buildSettlementButton"))
            {

                HideMenu();

                Pioneer pioneer = (Pioneer) unitManager.ActiveUnit;
                Cell cell = grid.GetCellByXY(pioneer.WorldX / cellSize, pioneer.WorldY / cellSize);

                if (resources.ContainsKey(cell))
                {

                   Debug.WriteLine("Resources in the way, cannot build");

                }
                else
                {

                    Structure structure = pioneer.StartBuild("settlement", Content.Load<Texture2D>("Game/Structures/settlement"), cellSize, cellSize, cell);
                    structures.Add( structure);

                    Debug.WriteLine("Structure build started");

                }

            }

            if (button.Name.Contains("gatherActionButton"))
            {

                HideMenu();

                //TODO Check if cell contains wood resource
                LumberJack lumberJack = (LumberJack )unitManager.ActiveUnit;               
                Cell cell = grid.GetCellByXY(lumberJack.WorldX / cellSize, lumberJack.WorldY / cellSize);
                Terrain terrain = null;

                if (resources.ContainsKey(cell))
                {
                    
                    terrain = resources[cell];

                    if (terrain.type == "Wood")
                    {

                        
                        lumberJack.PerfromAction(terrain);

                    }
                    else
                    {

                        Debug.WriteLine("No wood to gather");

                    }

                }
                else
                {

                    Debug.WriteLine("No resource to gather");

                }

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

            Debug.WriteLine(activeButtons.Count);

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

                    if (target is Unit)
                    {

                        Debug.WriteLine("Unit");

                        Unit unit = (Unit)e.Target;

                        if (unitManager.UnitOfActivePlayer(unit) && !unit.PerformingAction)
                        {
                            Debug.WriteLine("Is player unit");
                            unitManager.ActiveUnit = unit;
                            Renderer.Camera.Target = unit;

                        }

                    }

                    if (target is Tile)
                    {

                        if (unitManager.ActiveUnit != null && !unitManager.ActiveUnit.IsTurnOver)
                        {

                            if (!unitManager.ActiveUnit.IsMoving && !unitManager.ActiveUnit.PerformingAction)
                            {
                                
                                MoveUnit(grid.GetCellByXY(unitManager.ActiveUnit.WorldX / grid.CellWidth, unitManager.ActiveUnit.WorldY / grid.CellHeight), 
                                    grid.GetCellByXY((int)(e.Position.X / grid.CellWidth), (int)(e.Position.Y / grid.CellHeight)));
                                                                  
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

                        activeButtons = GetButtons("pioneer_action", "all_action");
                        screenPosition = Renderer.Camera.WorldToScreenPosition(new Vector2(target.WorldX, target.WorldY));

                        unitManager.ActiveUnit = pioneer;
                        ShowUnitMenu(screenPosition, pioneer);

                    }

                    if (target is LumberJack)
                    {

                        LumberJack lumberjack = (LumberJack)target;

                        activeButtons = GetButtons("lumberjack_action", "all_action");
                        screenPosition = Renderer.Camera.WorldToScreenPosition(new Vector2(target.WorldX, target.WorldY));

                        unitManager.ActiveUnit = lumberjack;
                        ShowUnitMenu(screenPosition, lumberjack);


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

        private List<Button> GetButtons(string unitSpecificSetName, string setName)
        {

            Button closeButton = null;

            for (int i = 0; i < buttons.Count; i++)
            {

                if (buttons[i].set == unitSpecificSetName || buttons[i].set == setName)
                {

                    //Debug.WriteLine(buttons[i].Name);
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

                //Renderer.Camera.LookAt(new Vector2(Renderer.Camera.Target.WorldX, Renderer.Camera.Target.WorldY));

            }

            Renderer.Draw();

            base.Draw(gameTime);
            

        }
    }
}
