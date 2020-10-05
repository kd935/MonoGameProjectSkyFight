using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using SkyFight;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace SkyFight
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class MainGame : Game
    {
        //[DllImport("kernel32.dll", SetLastError = true)]
        //static extern int AllocConsole();
        public const int SCREEN_WIDTH = 1170;
        public const int SCREEN_HEIGHT = 768;
        public const float ACCELERATION = 0.5F;
        public const float MAX_PLANE_SPEED = 6.5f;
        public const float MIN_PLANE_SPEED = 3f;
        public const float BULLET_SPEED = 7;
        public const float PLANE_ROTATION_PER_TICK = 2f;
        public const float PLANE_SCALING = 0.1F;
        public const float GROUND_SCALING = 1;
        public const float BACKGROUND_SCALING = 1;
        public const float BULLET_SCALING = 1;
        public const float SMOKE_SCALING = 0.6f;
        public const float FIRE_SCALING = 0.6f;
        public const float EXPLOSION_SCALING = 1;
        public const float SCORE_TO_END_THE_GAME = 3;
        public const float RADIUS_DIVIDER = 3f;
        public const int NUMBER_OF_FRAMES_IN_EXPLOSION_ANIMATION = 48;
        public const int NUMBER_OF_FRAMES_IN_SMOKE_ANIMATION = 30;
        public const int NUMBER_OF_FRAMES_IN_FIRE_ANIMATION = 25;
        public const int ONE_HIT_DAMAGE = 33;
        public const int BIPLANE_HEALTH_POINTS = 3 * ONE_HIT_DAMAGE;
        public const int BULLET_STARTING_POSITION_X_OFFSET = 10;

        public Queue<Explosion> explosionQue = new Queue<Explosion>();
        public List<Bullet> bulletList = new List<Bullet>();
        public List<Smoke> smokeList = new List<Smoke>();
        public List<Fire> fireList = new List<Fire>();

        public static Action<Vector2, float> CreatePlayerBulleT;
        public static Action<Vector2, float> CreateEnemyBulleT;
        public static Action SpawnPlayer;
        public static Action SpawnEnemy;
        public static Action<float, float> CreateExp;
        public static Action<Biplane> CreateSm;
        public static Action<Biplane> DestroySm;
        public static Action<Biplane> CreateFi;
        public static Action<Biplane> DestroyFi;
        public static Action IncreasePlayerSc;
        public static Action IncreaseEnemySc;
        public static Func<Vector2> FGetPlayerCenterCoords;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
  
        Ground ground;
        Background background;
        PlayerBiplane player;
        EnemyBiplane enemy;

        KeyboardState oldState;
        KeyboardState currentState;

        public string backgroundImagePath = "images/background1";
        public string groundImagePath = "images/ground";
        public string playerPlaneImagePath = "images/player_biplane";
        public string enemyPlaneImagePath = "images/enemy_biplane";
        public string explosionAnimationPath = "animations/explosion";
        public string smokeAnimationPath = "animations/smoke";
        public string fireAnimationPath = "animations/fire";
        public string playerBulletImagePath = "images/bullet_player";
        public string enemyBulletImagePath = "images/bullet_enemy";

        public SpriteFont gameFont; 
        public TextDrawer TextDrawer { get; set; }
        public ScoreManager scoreManager;

        public string playerScoreText = "Player score: ";
        public string enemyScoreText = "Enemy player score: ";
        public string endGameText = $" has won.{System.Environment.NewLine}Press Space button to restart the game.";
        public Vector2 playerScoreTextPosition;
        public Vector2 enemyScoreTextPosition;
        public Vector2 endGameTextPosition;

        public GameStates gameState = GameStates.Started;
        public Winners? winner = null;
        public GameModes gameMode;
        public MainGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            graphics.PreferredBackBufferWidth = SCREEN_WIDTH;
            graphics.PreferredBackBufferHeight = SCREEN_HEIGHT;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();
            //AllocConsole();
            CreatePlayerBulleT = CreatePlayerBullet;
            CreateEnemyBulleT = CreateEnemyBullet;
            SpawnPlayer = SpawnPlayerBiplane;
            SpawnEnemy = SpawnEnemyBiplane;
            CreateExp = CreateExplosion;
            CreateSm = CreateSmoke;
            DestroySm = DestroySmoke;
            CreateFi = CreateFire;
            DestroyFi = DestroyFire;
            IncreasePlayerSc = IncreasePlayerScore;
            IncreaseEnemySc = IncreaseEnemyScore;
            FGetPlayerCenterCoords = GetPlayerCenterCoords;
            gameMode = GameModes.PlayerVersusAI;
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            background = new Background(0, 0, backgroundImagePath, Content, 1);
            ground = new Ground(SCREEN_WIDTH / 2, 0, groundImagePath, Content, 1);
            player = new PlayerBiplane(0, SCREEN_HEIGHT - ground.ScaledHeight, playerPlaneImagePath, Content, PLANE_SCALING);
            enemy = new EnemyBiplane(SCREEN_WIDTH, SCREEN_HEIGHT - ground.ScaledHeight, enemyPlaneImagePath, Content, PLANE_SCALING, gameMode);

            gameFont = Content.Load<SpriteFont>("fonts/GameFont");
            TextDrawer = new TextDrawer(gameFont, spriteBatch);

            scoreManager = new ScoreManager(gameState);

            playerScoreTextPosition = new Vector2(0, 0);
            enemyScoreTextPosition = new Vector2((float)(SCREEN_WIDTH - SCREEN_WIDTH / 5.1), 0);
            endGameTextPosition = new Vector2(SCREEN_WIDTH / 2.6f, SCREEN_HEIGHT / 2 - SCREEN_HEIGHT / 4);

            // TODO: use this.Content to load your game content here

            Explosion.TextureList = new List<Texture2D>(new Texture2D[MainGame.NUMBER_OF_FRAMES_IN_EXPLOSION_ANIMATION]);
            for (int numberOfFrame = 0; numberOfFrame < NUMBER_OF_FRAMES_IN_EXPLOSION_ANIMATION; numberOfFrame++)
            {
                Explosion.TextureList[numberOfFrame] = Content.Load<Texture2D>($"{explosionAnimationPath}/explosion ({numberOfFrame + 1})");
            }

            Smoke.TextureList = new List<Texture2D>(new Texture2D[MainGame.NUMBER_OF_FRAMES_IN_SMOKE_ANIMATION]);
            for (int numberOfFrame = 0; numberOfFrame < NUMBER_OF_FRAMES_IN_SMOKE_ANIMATION; numberOfFrame++)
            {
                Smoke.TextureList[numberOfFrame] = Content.Load<Texture2D>($"{smokeAnimationPath}/smoke ({numberOfFrame + 1})");
            }

            Fire.TextureList = new List<Texture2D>(new Texture2D[MainGame.NUMBER_OF_FRAMES_IN_FIRE_ANIMATION]);
            for (int numberOfFrame = 0; numberOfFrame < NUMBER_OF_FRAMES_IN_FIRE_ANIMATION; numberOfFrame++)
            {
                Fire.TextureList[numberOfFrame] = Content.Load<Texture2D>($"{fireAnimationPath}/fire ({numberOfFrame + 1})");
            }
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (gameState == GameStates.Ended) { KeyboardHandler(); return; }

            CheckObjectsForCollision();
            // TODO: Add your update logic here
            KeyboardHandler(); // Handle keyboard input

            player.Update();
            enemy.Update();

            UpdateExplosionQue();
            UpdateBulletList();
            UpdateSmokeList();
            UpdateFireList();

            (gameState, winner) = scoreManager.CheckPlayersScore();

            base.Update(gameTime);
        }

        private void UpdateBulletList()
        {
            foreach (Bullet bullet in bulletList)
            {
                bullet.Update();
            }

            List<int> bulletsToDestroy = new List<int>();

            for(int i = 0; i < bulletList.Count; i++)
            {
                if (bulletList[i].isAlive == false)
                {
                    bulletsToDestroy.Add(i);
                }
            }
            bulletsToDestroy.Sort((a, b) => -1 * a.CompareTo(b));
            foreach (int bulletIndex in bulletsToDestroy)
            {
                bulletList.RemoveAt(bulletIndex);
            }
        }
        private void UpdateSmokeList()
        {
            foreach (Smoke smoke in smokeList)
            {
                smoke.Update();
            }

            List<int> smokeToDestroy = new List<int>();

            for (int i = 0; i < smokeList.Count; i++)
            {
                if (smokeList[i].IsFinished == true)
                {
                    smokeToDestroy.Add(i);
                }
            }
            smokeToDestroy.Sort((a, b) => -1 * a.CompareTo(b));
            foreach (int smokeIndex in smokeToDestroy)
            {
                smokeList.RemoveAt(smokeIndex);
            }
        }
        private void UpdateFireList()
        {
            foreach (Fire fire in fireList)
            {
                fire.Update();
            }

            List<int> fireToDestroy = new List<int>();

            for (int i = 0; i < fireList.Count; i++)
            {
                if (fireList[i].IsFinished == true)
                {
                    fireToDestroy.Add(i);
                }
            }
            fireToDestroy.Sort((a, b) => -1 * a.CompareTo(b));
            foreach (int fireIndex in fireToDestroy)
            {
                fireList.RemoveAt(fireIndex);
            }
        }

        private void UpdateExplosionQue()
        {
            foreach (Explosion explosion in explosionQue)
            {
                explosion.Update();
            }

            while (explosionQue.Count > 0)
            {
                if (explosionQue.Peek().IsFinished == true)
                {
                    explosionQue.Dequeue();
                }
                else
                {
                    return;
                }
            }
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.AliceBlue);

            // TODO: Add your drawing code here
            spriteBatch.Begin();

            background.Draw(spriteBatch);
            ground.Draw(spriteBatch);

            if (gameState == GameStates.Ended) { TextDrawer.DrawText(endGameTextPosition, Color.DarkGoldenrod, $"{winner}{endGameText}"); }


            foreach (Smoke smoke in smokeList)
            {
                smoke.Draw(spriteBatch);
            }

            foreach (Fire fire in fireList)
            {
                fire.Draw(spriteBatch);
            }

            player.Draw(spriteBatch);
            enemy.Draw(spriteBatch);

            foreach (Explosion explosion in explosionQue)
            {
                explosion.Draw(spriteBatch);
            }

            foreach(Bullet bullet in bulletList)
            {
                bullet.Draw(spriteBatch);
            }

            TextDrawer.DrawText(playerScoreTextPosition, Color.DarkGoldenrod, $"{playerScoreText}{scoreManager.PlayerScore}");
            TextDrawer.DrawText(enemyScoreTextPosition, Color.DarkGoldenrod, $"{enemyScoreText}{scoreManager.EnemyScore}");
            spriteBatch.End();

            base.Draw(gameTime);
        }
        private void KeyboardHandler()
        {
            currentState = Keyboard.GetState();

            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || currentState.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (currentState.IsKeyDown(Keys.Add))
            {
                player.Shoot();
            }
            if (currentState.IsKeyDown(Keys.Up) && oldState.IsKeyUp(Keys.Up))
            {
                player.Speed = MathHelper.Clamp(player.Speed + ACCELERATION, MIN_PLANE_SPEED, MAX_PLANE_SPEED);
            }
            if (currentState.IsKeyDown(Keys.Down) && oldState.IsKeyUp(Keys.Down))
            {
                player.Speed = MathHelper.Clamp(player.Speed - ACCELERATION, MIN_PLANE_SPEED, MAX_PLANE_SPEED);
            }
            if (currentState.IsKeyDown(Keys.Left) && oldState.IsKeyUp(Keys.Left))
            {
                player.RotationPerTick = PLANE_ROTATION_PER_TICK;
            }
            if (currentState.IsKeyDown(Keys.Right) && oldState.IsKeyUp(Keys.Right))
            {
                player.RotationPerTick = -PLANE_ROTATION_PER_TICK;
            }
            if (currentState.IsKeyUp(Keys.Left) && oldState.IsKeyDown(Keys.Left))
            {
                player.RotationPerTick = 0;
            }
            if (currentState.IsKeyUp(Keys.Right) && oldState.IsKeyDown(Keys.Right))
            {
                player.RotationPerTick = 0;
            }

            if (currentState.IsKeyDown(Keys.V))
            {
                enemy.Shoot();
            }
            if (currentState.IsKeyDown(Keys.W) && oldState.IsKeyUp(Keys.W))
            {
                enemy.Speed = MathHelper.Clamp(enemy.Speed + ACCELERATION, MIN_PLANE_SPEED, MAX_PLANE_SPEED);
            }
            if (currentState.IsKeyDown(Keys.S) && oldState.IsKeyUp(Keys.S))
            {
                enemy.Speed = MathHelper.Clamp(enemy.Speed - ACCELERATION, MIN_PLANE_SPEED, MAX_PLANE_SPEED);
            }
            if (currentState.IsKeyDown(Keys.A) && oldState.IsKeyUp(Keys.A))
            {
                enemy.RotationPerTick = PLANE_ROTATION_PER_TICK;
            }
            if (currentState.IsKeyDown(Keys.D) && oldState.IsKeyUp(Keys.D))
            {
                enemy.RotationPerTick = -PLANE_ROTATION_PER_TICK;
            }
            if (currentState.IsKeyUp(Keys.A) && oldState.IsKeyDown(Keys.A))
            {
                enemy.RotationPerTick = 0;
            }
            if (currentState.IsKeyUp(Keys.D) && oldState.IsKeyDown(Keys.D))
            {
                enemy.RotationPerTick = 0;
            }

            if (currentState.IsKeyDown(Keys.Space) && oldState.IsKeyUp(Keys.Space))
            {
                SetUpTheGame();
            }

            oldState = currentState;
        }
        public bool IsColliding(Sprite lsp, Sprite rsp)
        {
            if (lsp != null && rsp != null)
            {
                if (lsp.HitBox.Intersects(rsp.HitBox))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        void CheckObjectsForCollision()
        {
            if(IsColliding(player, ground))
            {
                CreateExplosion(player.Center.X, player.Center.Y);
                DestroySmoke(player);
                DestroyFire(player);
                SpawnPlayerBiplane();
                IncreaseEnemyScore();
            }
            if (IsColliding(enemy, ground))
            {
                CreateExplosion(enemy.Center.X, enemy.Center.Y);
                DestroySmoke(enemy);
                DestroyFire(enemy);
                SpawnEnemyBiplane();
                IncreasePlayerScore();
            }

            foreach(Bullet bullet in bulletList)
            {
                if (IsColliding(player, bullet))
                {
                    player.DealDamage();
                    bullet.isAlive = false;
                    continue;
                }
                if (IsColliding(enemy, bullet))
                {
                    enemy.DealDamage();
                    bullet.isAlive = false;
                    continue;
                }
            }
        }
        private void CreateExplosion(float x, float y)
        {
            Explosion explosion = new Explosion(x, y, $"{explosionAnimationPath}/explosion ({1})", Content, EXPLOSION_SCALING);
            explosionQue.Enqueue(explosion);
        }
        private void CreatePlayerBullet(Vector2 initialPosition, float initialAngle)
        {
            PlayerBullet bullet = new PlayerBullet(initialPosition.X, initialPosition.Y, playerBulletImagePath, Content, BULLET_SCALING, initialAngle, BULLET_SPEED + player.Speed);
            bulletList.Add(bullet);
        }
        private void CreateEnemyBullet(Vector2 initialPosition, float initialAngle)
        {
            EnemyBullet bullet = new EnemyBullet(initialPosition.X, initialPosition.Y, enemyBulletImagePath, Content, BULLET_SCALING, initialAngle, BULLET_SPEED + enemy.Speed);
            bulletList.Add(bullet);
        }
        public void SpawnPlayerBiplane()
        {
            player = new PlayerBiplane(0, SCREEN_HEIGHT - ground.ScaledHeight, playerPlaneImagePath, Content, PLANE_SCALING);
        }
        public void SpawnEnemyBiplane()
        {
            enemy = new EnemyBiplane(SCREEN_WIDTH, SCREEN_HEIGHT - ground.ScaledHeight, enemyPlaneImagePath, Content, PLANE_SCALING, gameMode);
        }
        private void CreateSmoke(Biplane biplane)
        {
            Smoke smoke = new Smoke(biplane.Center.X, biplane.Center.Y, $"{smokeAnimationPath}/smoke ({1})", Content, SMOKE_SCALING, biplane);
            smokeList.Add(smoke);
        }
        private void CreateFire(Biplane biplane)
        {
            Fire fire = new Fire(biplane.Center.X, biplane.Center.Y, $"{fireAnimationPath}/fire ({1})", Content, FIRE_SCALING, biplane);
            fireList.Add(fire);
        }
        private void DestroyFire(Biplane biplane)
        {
            foreach (Fire fire in fireList)
            {
                if (fire.biplane == biplane)
                {
                    fire.IsFinished = true;
                }
            }
        }
        private void DestroySmoke(Biplane biplane)
        {
            foreach(Smoke smoke in smokeList)
            {
                if(smoke.biplane == biplane)
                {
                    smoke.IsFinished = true;
                }
            }
        }
        private void IncreasePlayerScore()
        {
            scoreManager.IncreasePlayerScore();
        }
        private void IncreaseEnemyScore()
        {
            scoreManager.IncreaseEnemyScore();
        }
        public void SetUpTheGame()
        {
            gameState = GameStates.Started;
            winner = null;
            SpawnPlayerBiplane();
            SpawnEnemyBiplane();
            scoreManager = new ScoreManager(gameState);
            bulletList = new List<Bullet>();
            smokeList = new List<Smoke>();
            fireList = new List<Fire>();
        }
        public Vector2 GetPlayerCenterCoords()
        {
            return player.Center;
        }
    }
}
