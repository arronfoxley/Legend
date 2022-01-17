using FGame.Camera;
using FGame.Core;
using FGame.Events.MouseEvents;
using FGame.Grid;
using FGame.Objects;
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

        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private DisplayList displayList;
        private MouseInput mi;
        private MonoCamera2D camera;
        private Grid grid;

        private PathFinder pathFinder;

        private Unit player;
        private Unit enemy;

        private TurnManager turnManager;

        Player player1;
        Player player2;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            InitCamera();
            InitInputs();
            LoadGrid();
            DrawMap();
            CreateTurnManager();
            CreatePlayers();
            CreateTestUnits();
            SetPlayerTurn(player1);
            
        }

        private void InitCamera()
        {

            Viewport gPort = new Viewport();
            gPort.X = 0;
            gPort.Y = 0;
            gPort.Width = 1280;
            gPort.Height = 800;
            gPort.MinDepth = 0;
            gPort.MaxDepth = 1;

            displayList = new DisplayList(gPort, _graphics, _spriteBatch);

            _graphics.PreferredBackBufferWidth = 1280;
            _graphics.PreferredBackBufferHeight = 800;

            _graphics.ApplyChanges();

            camera = new MonoCamera2D(displayList.gPort);

            displayList.Camera = camera;

            displayList.Camera.LookAt(new Vector2(640, 400));

        }

        private void InitInputs()
        {

            mi = new MouseInput(this.Window);

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

                Sprite sprite = new Sprite(Content.Load<Texture2D>("gravel"), kvp.Value.Width, kvp.Value.Height);
                sprite.X = kvp.Value.GridX * kvp.Value.Width;
                sprite.Y = kvp.Value.GridY * kvp.Value.Height;
                displayList.AddWorldSprite(sprite);

                sprite.AddEventListener(MouseEvent.LEFT_CLICK, OnClick);

            }

        }

        private void CreateTurnManager()
        {

            turnManager = new TurnManager();

        }

        private void CreatePlayers()
        {

             player1 = new Player();
             player2 = new Player();

            turnManager.AddPlayerToTurnList(player1);
            turnManager.AddPlayerToTurnList(player2);

        }

        private void CreateTestUnits()
        {

            Sprite sprite = new Sprite(Content.Load<Texture2D>("player"), 32, 32);

            player = new Unit(sprite);

            player.Sprite.X = 32 * 2;
            player.Sprite.Y = 32 * 2;
           
            player1.AddUnitToUnitList(player);

            displayList.AddWorldSprite(sprite);

            Sprite sprite2 = new Sprite(Content.Load<Texture2D>("enemy"), 32, 32);

            enemy = new Unit(sprite2);

            enemy.Sprite.X = 32 * 2;
            enemy.Sprite.Y = 32 * 2;

            player2.AddUnitToUnitList(enemy);

            displayList.AddWorldSprite(sprite2);

        }


        private void SetPlayerTurn(Player player)
        {

            turnManager.ActivePlayer = player;

            turnManager.ActiveUnit = player.units[turnManager.PlayerUnitTurnNumber];

        }

        private void OnClick(MouseEvent e)
        {

                if (turnManager.ActivePlayer == player1 && !turnManager.ActiveUnit.IsMoving)
                {
                 
                    //pick new path
                    List<Cell> path = pathFinder.BreadthFirstSearch(
                        grid.GetCellByXY(turnManager.ActiveUnit.Sprite.X / grid.CellWidth, turnManager.ActiveUnit.Sprite.Y / grid.CellHeight), 
                        grid.GetCellByXY((int)(e.MousePosition.X / grid.CellWidth), (int)(e.MousePosition.Y / grid.CellHeight))
                        );

                turnManager.ActiveUnit.PrepForMovememnt(path);

                }

        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            
            if (turnManager.ActivePlayer == player2 && !turnManager.ActiveUnit.IsMoving)
            {


                Random rX = new Random();
                Random rY = new Random();

                int x = rX.Next(0, 20);
                int y = rY.Next(0, 20);
                //pick new path
                List<Cell> path = pathFinder.BreadthFirstSearch(
                    grid.GetCellByXY(turnManager.ActiveUnit.Sprite.X / grid.CellWidth, turnManager.ActiveUnit.Sprite.Y / grid.CellHeight),
                    grid.GetCellByXY(  x, y )
                    );

                turnManager.ActiveUnit.PrepForMovememnt(path);

            }

            turnManager.ActiveUnit.Update(gameTime);
            turnManager.Update();

            //displayList.Camera.LookAt(new Vector2(player.Sprite.X, player.Sprite.Y));

            mi.Update(displayList);
            base.Update(gameTime);

        }

        protected override void Draw(GameTime gameTime)
        {
          
            displayList.Render();
            base.Draw(gameTime);

        }
    }
}
