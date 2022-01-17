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

        private Player player;
        private Enemy enemy;

        private TurnManager turnManager;

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
            CreatePlayer();
            CreateEnemy();
            CreateTurnManager();
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

        private void CreatePlayer()
        {

            Sprite sprite = new Sprite(Content.Load<Texture2D>("player"),32 , 32);

            player = new Player(sprite);

            player.Sprite.X = 32 * 2;
            player.Sprite.Y = 32 * 2;

            displayList.AddWorldSprite(sprite);

        }

        private void CreateEnemy()
        {

            Sprite sprite = new Sprite(Content.Load<Texture2D>("enemy"), 32 , 32 );

            enemy = new Enemy(sprite);

            enemy.Sprite.X = 32 * 14;
            enemy.Sprite.Y = 32 * 14;

            displayList.AddWorldSprite(sprite);

        }

        private void CreateTurnManager()
        {

            turnManager = new TurnManager();
            turnManager.AddUnitToTurnList(player);
            turnManager.AddUnitToTurnList(enemy);

            turnManager.ActiveUnit = player;

        }

        private void OnClick(MouseEvent e)
        {

                if (!player.IsMoving && player == turnManager.ActiveUnit)
                {

                    //pick new path
                    List<Cell> path = pathFinder.BreadthFirstSearch(grid.GetCellByXY(player.Sprite.X / grid.CellWidth, player.Sprite.Y / grid.CellHeight), grid.GetCellByXY((int)(e.MousePosition.X / grid.CellWidth), (int)(e.MousePosition.Y / grid.CellHeight)));

                    player.PrepForMovememnt(path);
                    player.TurnStarted = true;

                }

        }

        protected override void Update(GameTime gameTime)
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
