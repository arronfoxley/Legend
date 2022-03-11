using FGame.Camera;
using FGame.Core;
using FGame.Core.Objects.Game;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Menus;
using Legend.Core.Data;
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

        private List<UnitData> unitData;

        private List<MapData> mapData;

        private GridData gridData;

        private Dictionary<int, TextureData> textureList;

        private int cellSize = 48;

        private List<Structure> structures = new List<Structure>();

        private string texturesXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/TextureList.xml";
        private string unitXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Units.xml";
        private string terrianXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Terrain.xml";
        private string gridXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Grid.xml";
        private string mapXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Map.xml";

        public Game1()
        {

            Renderer.InitGraphicsDeviceManager(this);
            Content.RootDirectory = "Content/Assets/Graphics";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
          
            Renderer.GraphicsDeviceManager.PreferredBackBufferWidth = 528;
            Renderer.GraphicsDeviceManager.PreferredBackBufferHeight = 528;

            Renderer.GraphicsDeviceManager.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent()
        {
      
            InitCamera();
            InitInputs();
            ParseXML();
            CreateManagers();
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

        private void ParseXML()
        {

            textureList = XMLParser.ParseTexturesXML(texturesXMLPath);

            gridData = XMLParser.ParseGridXML(gridXMLPath);

            CreateGrid(gridData);

            mapData = XMLParser.ParseMapXML(mapXMLPath);

            CreateMap(mapData);

            unitData = XMLParser.ParseUnitXML(unitXMLPath);           

            List<TerrainCellData> terrainCellData =  XMLParser.ParseTerrainCellXML(terrianXMLPath);

            CreateTerrain(terrainCellData);           

        }

        private void CreateTerrain(List<TerrainCellData> terrainCellData)
        {

            foreach (TerrainCellData tcd in terrainCellData)
            {

                Terrain resource = new Terrain(Content.Load<Texture2D>(textureList[tcd.textureID].textureName), cellSize, cellSize, tcd.type);

                resource.cellX = tcd.cellX;
                resource.cellY = tcd.cellY;

                resource.WorldX = cellSize * tcd.cellX;
                resource.WorldY = cellSize * tcd.cellY;

                resource.DrawX = resource.WorldX;
                resource.DrawY = resource.WorldY;

                if (!tcd.isPassable)
                {

                    grid.GetCellByXY(tcd.cellX, tcd.cellY).Passable = false;

                }

                switch (resource.type)
                {

                    case "Ore": resource.DrawLayer = GameDrawDepthList.GAME_MOUNTAIN_DEPTH_LAYER; break;
                    case "Wood": resource.DrawLayer = GameDrawDepthList.GAME_RESOURCE_DEPTH_LAYER; break;

                }

                Renderer.AddGameSprite(resource);

                resource.Active = true;

                if (tcd.shadowTextureID != 0)
                {

                    GameSprite shadow = new GameSprite(Content.Load<Texture2D>(textureList[tcd.shadowTextureID].textureName), cellSize, cellSize);

                    shadow.WorldX = cellSize * resource.cellX;
                    shadow.WorldY = cellSize * resource.cellY;

                    shadow.DrawX = shadow.WorldX;
                    shadow.DrawY = shadow.WorldY;

                    shadow.DrawLayer = GameDrawDepthList.GAME_MOUNTAIN_SHADOW_DEPTH_LAYER;
                    shadow.Alpha = 0.5f;

                    Renderer.AddGameSprite(shadow);

                    shadow.Active = true;

                }

                resources.Add(grid.GetCellByXY(tcd.cellX, tcd.cellY), resource);

            }

        }

        private void CreateGrid(GridData gridData)
        {

            grid = new Grid(gridData.cellsX * gridData.cellSize , gridData.cellsY * gridData.cellSize, gridData.cellsX, gridData.cellsY, gridData.cellSize, gridData.cellSize);

            cellSize = gridData.cellSize;

            grid.Generate();

            pathFinder = new PathFinder(grid);

        }

        private void CreateMap(List<MapData> mapData)
        {

            foreach(MapData md in mapData){

                Tile tile = new Tile(Content.Load<Texture2D>(textureList[md.textureId].textureName), cellSize, cellSize);

                tile.Color = new Color(105, 137, 105);

                tile.Active = true;

                tile.WorldX = md.cellX * cellSize;
                tile.WorldY = md.cellY * cellSize;
                tile.DrawX = tile.WorldX;
                tile.DrawY = tile.WorldY;

                Renderer.AddGameSprite(tile);

                //tile.Color = Color.Red;

                tile.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);
                tile.AddEventListener(MouseEvent.RIGHT_CLICK, OnClick);

            }

        }
        //TODO Dynamically generate map data from XML

        private void CreateManagers()
        {
           
            unitManager = new UnitManager(pathFinder,grid);
            turnManager = new TurnManager();

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

        private void CreateGameButtons()
        {

            CreateButton("Button/buttonBg", 64, 16, "passTurnButton" ,"UI", true, Color.Red, Color.Black);
            CreateButton("Button/buttonBg", 64, 16, "buildSettlementButton", "pioneer_action", false, Color.LightBlue, Color.Black);
            CreateButton("Button/buttonBg", 64, 16, "gatherActionButton", "lumberjack_action", false, Color.CornflowerBlue, Color.Black);
            CreateButton("Button/buttonBg", 64, 16, "setHomeButton", "all_action", false, Color.LightGray, Color.Black);
            CreateButton("Button/buttonBg", 64, 16, "cancelMovementButton", "all_action", false, Color.Red, Color.Black);
            CreateButton("Button/buttonBg", 64, 16, "closeButton", "close", false, Color.DarkRed, Color.Black);

        }

        private void CreateUnit(string unitType, int cellX, int cellY)
        {

            Unit unit = null;

            switch (unitType)
            {

                case "pioneer": unit = new Pioneer(Content.Load<Texture2D>(textureList[unitData[0].textureId].textureName), cellSize, cellSize) ; break;
                case "lumberjack": unit = new LumberJack(Content.Load<Texture2D>(textureList[unitData[1].textureId].textureName), cellSize, cellSize); break;
                case "miner": unit = new Miner(Content.Load<Texture2D>(textureList[unitData[2].textureId].textureName), cellSize, cellSize); break;

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

        private void CreateButton(string textureName, int width, int height, string buttonName, string set, Boolean active, Color color, Color overColor)
        {

            Button button = new Button(Content.Load<Texture2D>(textureName), 64, 16, set);

            buttons.Add(button);

            button.Color = color;
            button.OutColor = color;
            button.OverColor = overColor;

            button.Name = buttonName;

            button.Active = active;

            Renderer.AddUISprite(button);

            button.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            button.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);
            button.AddEventListener(MouseEvent.LEFT_CLICK, OnButtonClick);

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

                                Cell startCell = grid.GetCellByXY(unitManager.ActiveUnit.WorldX / grid.CellWidth, unitManager.ActiveUnit.WorldY / grid.CellHeight);
                                Cell destinationCell = grid.GetCellByXY((int)(e.Position.X / grid.CellWidth), (int)(e.Position.Y / grid.CellHeight));

                                //pick new path
                                List<Cell> path = pathFinder.BreadthFirstSearch(startCell, destinationCell);

                                unitManager.ActiveUnit.PrepForMovememnt(path);
                                unitManager.ActiveUnit.TurnStarted = true;

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
            Renderer.Draw();
            base.Draw(gameTime);           

        }
    }

}
