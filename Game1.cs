using Legend.Core;
using Legend.Core.Actions;
using Legend.Core.Data;
using Legend.Core.Display;
using Legend.Core.Management;
using Legend.Core.Resources;
using Legend.Core.Structures;
using Legend.Core.Tiles;
using Legend.Core.Units;
using Legend.Objects;

namespace Legend {
    public class Game1 : Game {

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private DisplayList displayList;
        private MouseInput mi;
        private MonoCamera2D camera;
        private Grid grid;

        private PathFinder pathFinder;

        private Player player;
        private Enemy enemy;

        private readonly string texturesXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/TextureList.xml";
        private readonly string unitXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Units.xml";
        private readonly string terrianXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Terrain.xml";
        private readonly string gridXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Grid.xml";
        private readonly string mapXMLPath = "C:/Users/arron/source/repos/arronfoxley/Legend/Content/Assets/xml/Map.xml";

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
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
            CreateGameButtons();
            CreatePlayers();
            CreateUnits();

            TurnManager.StartGlobalTurn();

            UnitManager.Grid = grid;
            UnitManager.Units = TurnManager.ActiveCharacter.Units;

        }
        private void InitCamera()
        {

            Renderer.InitSpriteBatcher();
            Renderer.InitCamera(0, 0, 528, 528);
            Renderer.InitViewports(0, 0, 528, 528);
            Renderer.Camera.LookAt(new Vector2(528 / 2, 528 / 2));

        }
        private void InitInputs()
        {

            MouseInputManager.Window = this.Window;

        }
        /*
         * Generation of game assets from XML parsing start 
         */
        private void ParseXML()
        {

            textureList = XMLParser.ParseTexturesXML(texturesXMLPath);

            gridData = XMLParser.ParseGridXML(gridXMLPath);

            CreateGrid(gridData);

            mapData = XMLParser.ParseMapXML(mapXMLPath);

            CreateMap(mapData);

            unitData = XMLParser.ParseUnitXML(unitXMLPath);

            List<TerrainCellData> terrainCellData = XMLParser.ParseTerrainCellXML(terrianXMLPath);

            CreateTerrain(terrainCellData);

        }
        private void CreateTerrain(List<TerrainCellData> terrainCellData)
        {

            foreach (TerrainCellData tcd in terrainCellData)
            {

                GameSprite gameSprite = new GameSprite(Content.Load<Texture2D>(textureList[tcd.textureID].textureName), cellSize, cellSize);

                Terrain resource = new Terrain(gameSprite, tcd.type);

                resource.Cell = grid.GetCellByXY(tcd.cellX, tcd.cellY);

                resource.GameSprite.WorldX = cellSize * tcd.cellX;
                resource.GameSprite.WorldY = cellSize * tcd.cellY;

                resource.GameSprite.DrawX = resource.GameSprite.WorldX;
                resource.GameSprite.DrawY = resource.GameSprite.WorldY;

                if (!tcd.isPassable)
                {

                    grid.GetCellByXY(tcd.cellX, tcd.cellY).Passable = false;

                }

                switch (resource.Type)
                {

                    case "Mountain": resource.GameSprite.DrawLayer = GameDrawDepthList.GAME_MOUNTAIN_DEPTH_LAYER; break;
                    case "Wood": resource.GameSprite.DrawLayer = GameDrawDepthList.GAME_RESOURCE_DEPTH_LAYER; break;
                    case "Ore":
                    resource.GameSprite.DrawLayer = GameDrawDepthList.GAME_RESOURCE_DEPTH_LAYER; break;


                    Renderer.AddGameSprite(resource.GameSprite);

                    resource.GameSprite.Active = true;

                    if (tcd.shadowTextureID != 0)
                    {

                        shadow.WorldX = cellSize * resource.Cell.GridX;
                        shadow.WorldY = cellSize * resource.Cell.GridY;

                    }

                }

            }

        }
        private void CreateGrid(GridData gridData)
        {

            grid = new Grid(gridData.cellsX * gridData.cellSize, gridData.cellsY * gridData.cellSize, gridData.cellsX, gridData.cellsY, gridData.cellSize);

            cellSize = gridData.cellSize;

            grid.Generate();

            PathFinder.Grid = grid;

        }
        private void CreateMap(List<MapData> mapData)
        {

            foreach (MapData md in mapData)
            {

                GameSprite gameSprite = new GameSprite(Content.Load<Texture2D>(textureList[md.textureId].textureName), cellSize, cellSize);
                Tile tile = new Tile(gameSprite);

                tile.GameSprite.Color = new Color(md.R, md.G, md.B);

                tile.GameSprite.Active = true;

                tile.GameSprite.WorldX = md.cellX * cellSize;
                tile.GameSprite.WorldY = md.cellY * cellSize;
                tile.GameSprite.DrawX = tile.GameSprite.WorldX;
                tile.GameSprite.DrawY = tile.GameSprite.WorldY;

                Renderer.AddGameSprite(tile.GameSprite);
                map.Add(grid.GetCellByXY(md.cellX, md.cellY), tile);

                tile.GameSprite.AddEventListener(MouseEvent.LEFT_CLICK, OnTileLeftClick);
                tile.GameSprite.AddEventListener(MouseEvent.RIGHT_CLICK, OnTileRightClick);

            }

        }
        /*
         * Generation of game assets from XML parsing end 
         */

        private void CreatePlayers()
        {

            CreateCharacter("Arron", 300, 100, 100);

        }
        private void CreateUnits()
        {

            CreateUnit("pioneer", 0, 1);
            CreateUnit("lumberjack", 1, 1);
            CreateUnit("miner", 2, 1);
            CreateUnit("engineer", 3, 1);
            CreateUnit("farmer", 4, 1);

        }
        private void CreateGameButtons()
        {

            Button passTurnButton = CreateButton("Button/buttonBg", 64, 16, "passTurnButton", "UI", true, Color.Red, Color.Black);
            passTurnButton.AddEventListener(MouseEvent.LEFT_CLICK, PassTurnButtonClick);

            Button buildCampButton = CreateButton("Button/buttonBg", 64, 16, "buildCampButton", "pioneer_action", false, Color.LightBlue, Color.Black);
            buildCampButton.AddEventListener(MouseEvent.LEFT_CLICK, BuildCampButtonClick);

            Button buildLumberMillButton = CreateButton("Button/buttonBg", 64, 16, "buildLumberMillButton", "engineer_action", false, Color.Orange, Color.Black);
            buildLumberMillButton.AddEventListener(MouseEvent.LEFT_CLICK, BuildLumberMillButtonClick);

            Button buildBlacksmithButton = CreateButton("Button/buttonBg", 64, 16, "buildBlacksmithsButton", "engineer_action", false, Color.OrangeRed, Color.Black);
            buildBlacksmithButton.AddEventListener(MouseEvent.LEFT_CLICK, BuildBlacksmithButtonClick);

            Button ploughButton = CreateButton("Button/buttonBg", 64, 16, "ploughButton", "pioneer_action", false, Color.DarkBlue, Color.Black);
            ploughButton.AddEventListener(MouseEvent.LEFT_CLICK, CultivationButtonClick);

            Button gatherActionButton = CreateButton("Button/buttonBg", 64, 16, "gatherActionButton", "gatherer_action", false, Color.CornflowerBlue, Color.Black);
            gatherActionButton.AddEventListener(MouseEvent.LEFT_CLICK, GatherActionButtonClick);

            Button setHomeButton = CreateButton("Button/buttonBg", 64, 16, "setHomeButton", "all_action", false, Color.LightGray, Color.Black);
            setHomeButton.AddEventListener(MouseEvent.LEFT_CLICK, SetHomeButtonClick);

            Button cancelMovementButton = CreateButton("Button/buttonBg", 64, 16, "cancelMovementButton", "all_action", false, Color.Red, Color.Black);
            cancelMovementButton.AddEventListener(MouseEvent.LEFT_CLICK, CancelMovementButtonClick);

            Button closeTurnButton = CreateButton("Button/buttonBg", 64, 16, "closeButton", "close", false, Color.DarkRed, Color.Black);
            closeTurnButton.AddEventListener(MouseEvent.LEFT_CLICK, CloseMenuButtonClick);

        }
        private void CreateUnit(string unitType, int cellX, int cellY)
        {

            Unit unit = null;

            switch (unitType)
            {

                case "pioneer": unit = new Pioneer(new GameSprite(Content.Load<Texture2D>(textureList[unitData[0].textureId].textureName), cellSize, cellSize)); break;
                case "lumberjack": unit = new LumberJack(new GameSprite(Content.Load<Texture2D>(textureList[unitData[1].textureId].textureName), cellSize, cellSize)); break;
                case "miner": unit = new Miner(new GameSprite(Content.Load<Texture2D>(textureList[unitData[2].textureId].textureName), cellSize, cellSize)); break;
                case "engineer": unit = new Engineer(new GameSprite(Content.Load<Texture2D>(textureList[unitData[3].textureId].textureName), cellSize, cellSize)); break;
                case "farmer": unit = new Farmer(new GameSprite(Content.Load<Texture2D>(textureList[unitData[4].textureId].textureName), cellSize, cellSize)); break;

            }

            unit.GameSprite.Active = true;

            unit.GameSprite.Name = unitType;

            unit.GameSprite.AddEventListener(MouseEvent.LEFT_CLICK, OnUnitLeftClick);
            unit.GameSprite.AddEventListener(MouseEvent.RIGHT_CLICK, OnUnitRightClick);

            unit.GameSprite.WorldX = cellSize * cellX;
            unit.GameSprite.WorldY = cellSize * cellY;

            unit.GameSprite.DrawX = unit.GameSprite.WorldX;
            unit.GameSprite.DrawY = unit.GameSprite.WorldY;

            TurnManager.Characters[0].AddUnitToUnitList(unit);

            Renderer.AddGameSprite(unit.GameSprite);


        }

        private void CreateCharacter(string playerName, int startingGoldCount, int startingOreCount, int startingLumberCount)
        {

            Character character = new Character(playerName, startingGoldCount, startingOreCount, startingLumberCount);
            TurnManager.AddCharacter(character);

        }
        private Button CreateButton(string textureName, int width, int height, string buttonName, string set, Boolean active, Color color, Color overColor)
        {

            Button button = new Button(Content.Load<Texture2D>(textureName), width, height, set);

            grid = new Grid(cellsX * cellWidth, cellsY * cellHeight, cellsX, cellsY, cellWidth, cellHeight);

            grid.Generate();

            button.AddEventListener(MouseEvent.MOUSE_OVER, OnButtonOver);
            button.AddEventListener(MouseEvent.MOUSE_OUT, OnButtonOut);

            return button;

        }
        /*
         * Button click listeners start
         */
        private void BuildCampButtonClick(MouseEvent e)
        {

            HideUnitMenu();

            Pioneer pioneer = (Pioneer)UnitManager.ActiveUnit;
            BuildStructure("Camp", pioneer);

        }
        private void BuildLumberMillButtonClick(MouseEvent e)
        {

            HideUnitMenu();

            Engineer engineer = (Engineer)UnitManager.ActiveUnit;
            BuildStructure("Lumbermill", engineer);

        }
        private void BuildBlacksmithButtonClick(MouseEvent e)
        {

            HideUnitMenu();

            Engineer engineer = (Engineer)UnitManager.ActiveUnit;
            BuildStructure("Blacksmiths", engineer);

        }
        private void CultivationButtonClick(MouseEvent e)
        {

            HideUnitMenu();
            Pioneer pioneer = (Pioneer)UnitManager.ActiveUnit;

            Texture2D cultivationTexture = Content.Load<Texture2D>("Game/Field/CultivatedLand");
            GameSprite gameSprite = new GameSprite(cultivationTexture, 48, 48);

            Terrain cultivation = new Terrain(gameSprite, "CultivatedLand");

            CultivationAction cultivationAction = new CultivationAction(cultivation, pioneer.CurrentCell, 1, 6);

            resources.Add(pioneer.CurrentCell, cultivation);

            pioneer.PerformAction(cultivationAction);

            Debug.WriteLine("Cultivation button clicked");

        }
        private void GatherActionButtonClick(MouseEvent e)
        {

            HideUnitMenu();

            Gatherer gatherer = (Gatherer)UnitManager.ActiveUnit;
            Cell cell = grid.GetCellByXY(gatherer.GameSprite.WorldX / cellSize, gatherer.GameSprite.WorldY / cellSize);
            Terrain terrain;

            if (resources.ContainsKey(cell) && gatherer.Home != null)
            {
                terrain = resources[cell];
                GatherAction ga = null;

                if (UnitManager.ActiveUnit is Miner && terrain.Type == "Ore")
                {

                    ga = new GatherAction(1, 5, "Ore", 20, terrain);

                }
                else if (UnitManager.ActiveUnit is LumberJack && terrain.Type == "Wood")
                {
                    ga = new GatherAction(1, 5, "Lumber", 20, terrain);

                }

                ga.GatherPath = PathFinder.BreadthFirstSearch(terrain.Cell, gatherer.Home.cell);
                ga.ReturnPath = new List<Cell>();

                ga.ReturnPath.AddRange(ga.GatherPath);
                ga.ReturnPath.Reverse();
                gatherer.PerformAction(ga);

            }

        }
        private void CancelMovementButtonClick(MouseEvent e)
        {

            if (UnitManager.ActiveUnit.IsMoving)
            {

                HideUnitMenu();
                UnitManager.ActiveUnit.JourneyReset();
                Debug.WriteLine("Journey cancelled");

            }
            else
            {

                Debug.WriteLine("No Journey to cancel");

            }

        }
        private void SetHomeButtonClick(MouseEvent e)
        {

            Boolean test = false;

            for (int i = 0; i < structures.Count; i++)
            {

                if (structures[i] is Camp)
                {

                    test = true;
                    UnitManager.ActiveUnit.Home = (Camp)structures[i];
                    HideUnitMenu();
                    Debug.WriteLine("Camp exists, set as home");
                    break;

                }

            }

            if (!test)
            {

                Debug.WriteLine("No camps exist, cant set home");

            }

        }
        private void PassTurnButtonClick(MouseEvent e)
        {

            TurnManager.ActiveCharacter.IsTurnComplete = true;
            TurnManager.UpdateCharatcerTurnCount();

            if (!TurnManager.IsGlobalTurnComplete())
            {

                TurnManager.SetNextCharacterTurn();
                UnitManager.Units = TurnManager.ActiveCharacter.Units;

            }
            else
            {

                TurnManager.GlobalTurnComplete();
                UnitManager.ResetAllCharacterUnits(TurnManager.Characters);
                TurnManager.StartGlobalTurn();

            }

        }
        private void CloseMenuButtonClick(MouseEvent e)
        {

            HideUnitMenu();

        }
        private void OnButtonOver(MouseEvent e)
        {

            Button button = (Button)e.Target;
            button.ApplyEffect(Button.OVER);

        }
        private void OnButtonOut(MouseEvent e)
        {

            turnManager.ActiveUnit = player;

        }
        private void HideUnitMenu()
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

                activeButtons[i].DrawX = (int)screenPosition.X + unit.GameSprite.Width + 1;
                activeButtons[i].DrawY = (int)screenPosition.Y + (i * activeButtons[i].Height);

                activeButtons[i].ScreenX = (int)screenPosition.X + unit.GameSprite.Width + 1;
                activeButtons[i].ScreenY = (int)screenPosition.Y + (i * activeButtons[i].Height);

            }

        }
        /*
         * Button click listeners end
         */
        private void OnUnitLeftClick(MouseEvent e)
        {

            UnitManager.ActiveUnit = UnitManager.GetUnitFromGameSprite((GameSprite)e.Target);
            Renderer.Camera.Target = (GameSprite)e.Target;

        }
        private void OnUnitRightClick(MouseEvent e)
        {

            HideUnitMenu();
            UnitManager.ActiveUnit = UnitManager.GetUnitFromGameSprite((GameSprite)e.Target);

            switch (UnitManager.ActiveUnit)
            {

                case Pioneer pioneer: activeButtons = GetButtons("pioneer_action", "all_action"); break;
                case LumberJack lumberjack: activeButtons = GetButtons("gatherer_action", "all_action"); break;
                case Miner miner: activeButtons = GetButtons("gatherer_action", "all_action"); break;
                case Engineer engineer: activeButtons = GetButtons("engineer_action", "all_action"); break;
                case Farmer farmer: activeButtons = GetButtons("gatherer_action", "all_action"); break;

            }

            screenPosition = Renderer.Camera.WorldToScreenPosition(new Vector2(UnitManager.ActiveUnit.GameSprite.WorldX, UnitManager.ActiveUnit.GameSprite.WorldY));
            ShowUnitMenu(screenPosition, UnitManager.ActiveUnit);

            Debug.WriteLine("Test");

        }
        private void OnTileLeftClick(MouseEvent e)
        {

            if (UnitManager.ActiveUnit != null && !UnitManager.ActiveUnit.IsTurnOver)
            {

                if (!UnitManager.ActiveUnit.IsMoving && !UnitManager.ActiveUnit.PerformingAction)
                {

                    Cell startCell = grid.GetCellByXY(UnitManager.ActiveUnit.GameSprite.WorldX / grid.CellSize, UnitManager.ActiveUnit.GameSprite.WorldY / grid.CellSize);
                    Cell destinationCell = grid.GetCellByXY((int)(e.Position.X / grid.CellSize), (int)(e.Position.Y / grid.CellSize));

                    //pick new path
                    List<Cell> path = PathFinder.BreadthFirstSearch(startCell, destinationCell);

                    UnitManager.ActiveUnit.PrepForMovememnt(path);
                    UnitManager.ActiveUnit.TurnStarted = true;

                }

            }

        }
        private void OnTileRightClick(MouseEvent e)
        {

            Renderer.Camera.Target = (GameSprite)e.Target;

        }
        private List<Button> GetButtons(string unitSpecificSetName, string setName)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            player.Update(gameTime);
            enemy.Update(gameTime);

            turnManager.Update();

            if (!player.IsMoving && !enemy.IsMoving && enemy == turnManager.ActiveUnit)
            {

                //pick new path
                List<Cell> path = pathFinder.BreadthFirstSearch(grid.GetCellByXY(enemy.Sprite.X / grid.CellWidth, enemy.Sprite.Y / grid.CellHeight), grid.GetCellByXY(player.Sprite.X / grid.CellWidth, player.Sprite.Y / grid.CellHeight));

                enemy.PrepForMovememnt(path);
                enemy.TurnStarted = true;


            }

            activeButtons.Add(closeButton);

            return activeButtons;

        }
        private Boolean CheckPlanningPermission(Cell cell)
        {


            if (resources.ContainsKey(cell))
            {

                Debug.WriteLine("Resources in the way, cannot build");
                return false;

            }
            else
            {

                Debug.WriteLine("Planning permission");
                return true;

            }

        }
        private void BuildStructure(string structureType, Builder builder)
        {

            Cell cell = grid.GetCellByXY(builder.GameSprite.WorldX / cellSize, builder.GameSprite.WorldY / cellSize);

            if (CheckPlanningPermission(cell))
            {

                Structure structure = PrepareStructure(structureType, 20, 20);
                BuildAction buildAction = new BuildAction(structure, cell, 1, 5);
                structures.Add(structure);
                builder.PerformAction(buildAction);

            }

        }
        private Structure PrepareStructure(string structureType, int lumberCost, int oreCost)
        {

            Structure structure = null;
            GameSprite gameSprite = null;

            switch (structureType)
            {

                case "Camp":
                gameSprite = new GameSprite(Content.Load<Texture2D>("Game/Structures/Camp"), cellSize, cellSize);
                structure = new Camp(gameSprite);
                break;
                case "LumberMill":
                gameSprite = new GameSprite(Content.Load<Texture2D>("Game/Structures/LumberMill"), cellSize, cellSize);
                structure = new LumberMill(gameSprite);
                break;
                case "Blacksmiths":
                gameSprite = new GameSprite(Content.Load<Texture2D>("Game/Structures/Blacksmiths"), cellSize, cellSize);
                structure = new Blacksmiths(gameSprite);
                break;

            }

            return structure;

        }
        protected override void Update(GameTime gameTime)
        {
            Vector2 screenPosition = new Vector2(Mouse.GetState(this.Window).X, Mouse.GetState(this.Window).Y);

            MouseInputManager.Update(screenPosition);
            UnitManager.Update(gameTime);
            base.Update(gameTime);

        }
        protected override void Draw(GameTime gameTime)
        {
           
            Renderer.ClearScreen();
            Renderer.DrawGame();
            Renderer.DrawUi();
            

            base.Draw(gameTime);

        }

    }
}
