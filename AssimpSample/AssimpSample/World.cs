// -----------------------------------------------------------------------
// <file>World.cs</file>
// <copyright>Grupa za Grafiku, Interakciju i Multimediju 2013.</copyright>
// <author>Srđan Mihić</author>
// <author>Aleksandar Josić</author>
// <summary>Klasa koja enkapsulira OpenGL programski kod.</summary>
// -----------------------------------------------------------------------
using System;
using Assimp;
using System.IO;
using System.Reflection;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.SceneGraph.Quadrics;
using SharpGL.SceneGraph.Core;
using SharpGL;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Threading;

namespace AssimpSample
{


    /// <summary>
    ///  Klasa enkapsulira OpenGL kod i omogucava njegovo iscrtavanje i azuriranje.
    /// </summary>
    public class World : IDisposable
    {
        #region Atributi

        //promj za tanjir
        private float distance;

        public float Distance
        {
            get { return distance; }
            set { distance = value; }
        }

        /// <summary>
        ///	 Ugao rotacije Meseca
        /// </summary>
        private float m_moonRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije Zemlje
        /// </summary>
        private float m_earthRotation = 0.0f;

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        private AssimpScene m_scene;
        private AssimpScene m_scene_holder;

        public float reflectorR = 0.0f;
        public float reflectorG = 0.0f;
        public float reflectorB = 0.8f;
        private float[] reflector = new float[] { 0.0f, 0.0f, 0.8f, 1.0f };

        //TEKSTURE
        private uint[] m_textures = new uint[6];
        private string[] m_textureFiles = { "..//..//imgs//bricks.jpg", "..//..//imgs//brick.jpg", "..//..//imgs//asphalt.jpg", "..//..//imgs//whiteAsphalt.jpg", "..//..//imgs//walls.jpg", "..//..//imgs//grass.jpg" };
        private enum TextureObjects { Bricks = 0, Brick, Asphalt, WhiteAsphalt, Walls, Grass};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

        //TOOLBAR
        private float raiseRamp = 0.0f;
        private double scaleTruck = 1;

        //ANIMACIJA
        private bool animationInProgress = false;
        public int animationChecker = 0;
        private DispatcherTimer timer1;
        private DispatcherTimer timer2;
        private DispatcherTimer timer21;
        private DispatcherTimer timer3;
        private DispatcherTimer timer4;
        private DispatcherTimer timer5;
        private bool boxChecker = false;

        public double truckTranslateX = -80f;
        public double truckTranslateY = -149.8f;
        public double truckTranslateZ = -20f;

        public float truckRotationX = 2.0f;
        public float truckRotationY = 20.0f;   //20
        public float truckRotationZ = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        private float m_xRotation = 0.0f;

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        private float m_yRotation = 0.0f;

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        private float m_sceneDistance = 7000.0f;

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_width;

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        private int m_height;



        public int Interakcija { get; set; }

        #endregion Atributi

        #region Properties

        /// <summary>
        ///	 Scena koja se prikazuje.
        /// </summary>
        public AssimpScene Scene
        {
            get { return m_scene; }
            set { m_scene = value; }
        }

        public AssimpScene Scene1
        {
            get { return m_scene_holder; }
            set { m_scene_holder = value; }
        }

        public double ScaleTruck
        {
            get { return this.scaleTruck; }
            set { this.scaleTruck = value; }
        }

        public float RaiseRamp
        {
            get
            {
                return this.raiseRamp;
            }
            set
            {
                this.raiseRamp = value;
            }
        }


        public float[] Reflector
        {
            get
            {
                return this.reflector;
            }
            set
            {
                this.reflector = value;
            }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko X ose.
        /// </summary>
        public float RotationX
        {
            get { return m_xRotation; }
            set { m_xRotation = value; }
        }

        /// <summary>
        ///	 Ugao rotacije sveta oko Y ose.
        /// </summary>
        public float RotationY
        {
            get { return m_yRotation; }
            set { m_yRotation = value; }
        }

        /// <summary>
        ///	 Udaljenost scene od kamere.
        /// </summary>
        public float SceneDistance
        {
            get { return m_sceneDistance; }
            set { m_sceneDistance = value; }
        }

        /// <summary>
        ///	 Sirina OpenGL kontrole u pikselima.
        /// </summary>
        public int Width
        {
            get { return m_width; }
            set { m_width = value; }
        }

        /// <summary>
        ///	 Visina OpenGL kontrole u pikselima.
        /// </summary>
        public int Height
        {
            get { return m_height; }
            set { m_height = value; }
        }

        #endregion Properties

        #region Konstruktori

        /// <summary>
        ///  Konstruktor klase World.
        /// </summary>
        public World(String scenePath, String sceneFileName, int width, int height, OpenGL gl)
        {
            this.m_scene = new AssimpScene(scenePath, sceneFileName, gl);
            this.m_width = width;
            this.m_height = height;
            this.m_scene_holder = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Truck2"), "faun_stw.obj", gl);
        }

        /// <summary>
        ///  Destruktor klase World.
        /// </summary>
        ~World()
        {
            this.Dispose(false);
        }

        #endregion Konstruktori

        #region Metode

        /// <summary>
        ///  Korisnicka inicijalizacija i podesavanje OpenGL parametara.
        /// </summary>
        public void Initialize(OpenGL gl)
        {
            gl.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.Color(1f, 0f, 0f);
            gl.ShadeModel(OpenGL.GL_FLAT);
            gl.Enable(OpenGL.GL_CULL_FACE);
            gl.Enable(OpenGL.GL_DEPTH_TEST);
            gl.CullFace(OpenGL.GL_FRONT);

            //TACKASTA SVETLOST
            gl.Enable(OpenGL.GL_COLOR_MATERIAL);
            gl.ColorMaterial(OpenGL.GL_FRONT, OpenGL.GL_AMBIENT_AND_DIFFUSE);

            float[] light0pos = new float[] { 400.0f, 2000.0f, 120.0f, 1.0f };
            float[] light0ambient = new float[] { 1.0f, 1.0f, 0.8f, 1f };
            float[] light0diffuse = new float[] { 1.0f, 1.0f, 0.8f, 1.0f };
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_POSITION, light0pos);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_SPOT_CUTOFF, 180.0f);      //Tackasto
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_AMBIENT, light0ambient);
            gl.Light(OpenGL.GL_LIGHT0, OpenGL.GL_DIFFUSE, light0diffuse);

            gl.Enable(OpenGL.GL_LIGHTING);
            gl.Enable(OpenGL.GL_LIGHT0);
            gl.Enable(OpenGL.GL_NORMALIZE);


            //TEKSTURE
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.TexEnv(OpenGL.GL_TEXTURE_ENV, OpenGL.GL_TEXTURE_ENV_MODE, OpenGL.GL_MODULATE);

            //Ucitaj slike i kreiraj teksture
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MIN_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_MAG_FILTER, OpenGL.GL_NEAREST);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_S, OpenGL.GL_REPEAT);
            gl.TexParameter(OpenGL.GL_TEXTURE_2D, OpenGL.GL_TEXTURE_WRAP_T, OpenGL.GL_REPEAT);


            gl.GenTextures(m_textureCount, m_textures);
            for (int i = 0; i < m_textureCount; ++i)
            {
                // Pridruzi teksturu odgovarajucem identifikatoru
                gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[i]);

                // Ucitaj sliku i podesi parametre teksture
                Bitmap image = new Bitmap(m_textureFiles[i]);
                // rotiramo sliku zbog koordinantog sistema opengl-a
                image.RotateFlip(RotateFlipType.RotateNoneFlipX);
                Rectangle rect = new Rectangle(0, 0, image.Width, image.Height);
                // RGBA format (dozvoljena providnost slike tj. alfa kanal)
                BitmapData imageData = image.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadOnly,
                                                      System.Drawing.Imaging.PixelFormat.Format32bppArgb);

                gl.Build2DMipmaps(OpenGL.GL_TEXTURE_2D, (int)OpenGL.GL_RGBA8, image.Width, image.Height, OpenGL.GL_BGRA, OpenGL.GL_UNSIGNED_BYTE, imageData.Scan0);

                image.UnlockBits(imageData);
                image.Dispose();
            }

            gl.Disable(OpenGL.GL_TEXTURE_2D);


            m_scene.LoadScene();
            m_scene.Initialize();
            m_scene_holder.LoadScene();
            m_scene_holder.Initialize();


            distance = 0.0f;

        }



        /// <summary>
        ///  Iscrtavanje OpenGL kontrole.
        /// </summary>
        public void Draw(OpenGL gl)
        {
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);

            gl.PushMatrix();

            //Definisanje kamere
            gl.LookAt(0f, 0f, -600f, 200f, -2500f, -6500f, 0f, 1f, 0f);
            gl.Translate(0f, 0.0f, -m_sceneDistance);
            gl.Rotate(m_xRotation, 1.0f, 0.0f, 0.0f);
            gl.Rotate(m_yRotation, 0.0f, 1.0f, 0.0f);


            gl.PushMatrix();
            gl.Scale(0.5f, 0.5f, 0.5f);
            gl.Translate(0.0f, -270f, 130f);
            //m_scene.Draw();
            gl.PopMatrix();
            DrawBase(gl);
            DrawReflectorLight(gl);
            DrawHolder(gl);
            DrawConstruction(gl);
            DrawWalls(gl);
            DrawRamp(gl);
            DrawText(gl);
            if(boxChecker)
                DrawBox(gl);

            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }

        public void DrawReflectorLight(OpenGL gl) 
        {
            float[] light1pos = new float[] { 80.0f, 2.0f, 95.0f, 1.0f };
            float[] smer = new float[] { 0.0f, -1.0f, 0.0f };
            float[] boja = new float[] { reflectorR, reflectorG, reflectorB, 1.0f };

            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_POSITION, light1pos);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_AMBIENT, boja);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_DIFFUSE, boja);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_CUTOFF, 40.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_EXPONENT, 5.0f);
            gl.Light(OpenGL.GL_LIGHT1, OpenGL.GL_SPOT_DIRECTION, smer);

            gl.Enable(OpenGL.GL_LIGHT1);
        }

        public void DrawHolder(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(truckTranslateX, truckTranslateY, truckTranslateZ);
            gl.Rotate(truckRotationX, truckRotationY, truckRotationZ);
            gl.Scale(0.06f * scaleTruck, 0.06f * scaleTruck, 0.06f * scaleTruck);
            //gl.Rotate(m_xRotation, 0.0f, 50.0f, 0.0f);
            gl.FrontFace(OpenGL.GL_CW);
            m_scene_holder.Draw();
            gl.PopMatrix();

        }

        public void DrawText(OpenGL gl)
        {
            gl.Viewport(m_width - 200, 0, m_width, m_height);
            gl.PushMatrix();
            gl.LoadIdentity();
            gl.DrawText3D("", 0, 0, 0, "");
            gl.DrawText(0, 130, 1.0f, 0.0f, 0.0f, "Verdana Bold", 14, "Predmet: Racunarska grafika");
            gl.DrawText(0, 100, 1.0f, 0.0f, 0.0f, "Verdana Bold", 12, "Sk.god: 2021 / 22.");
            gl.DrawText(0, 70, 1.0f, 0.0f, 0.0f, "Verdana Bold", 14, "Ime: Mitar");
            gl.DrawText(0, 40, 1.0f, 0.0f, 0.0f, "Verdana Bold", 14, "Prezime: Brankovic");
            gl.DrawText(0, 10, 1.0f, 0.0f, 0.0f, "Verdana Bold", 14, "Sifra zad: 5.2");
            gl.PopMatrix();
            gl.Viewport(0, 0, m_width, m_height); //podesavanje viewPorta preko cijelog ekrana
            gl.LoadIdentity();
        }

        public void DrawBase(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.FrontFace(OpenGL.GL_CCW);

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 120f);
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            //podloga
            gl.PushMatrix();
            gl.Scale(5.0f, 1.0f, 5.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Grass]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, -1.0f, 0.0f);
            gl.Color(6f, 6f, 5f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(250f, -150f, 220f);
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-220f, -150f, 220f);
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-220f, -150f, -220f);
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(250f, -150f, -220f);
            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D);


            //PLATFORMA
            Cube cube = new Cube();

            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(0.0f, 1.0f, 1.0f, 0.0f);
            gl.Translate(88.0f, -149.5f, 55.0f);
            gl.Rotate(0.0f, 0.0f, 0.0f);
            gl.Scale(25.0f, 0.0f, 40.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();


            gl.Enable(OpenGL.GL_TEXTURE_2D);

            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 120f);
            //prva ulica
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(12.0f, 1.0f, 0.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(-55f, -149.8f, 30f);   //dole desno
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-105f, -149.8f, 30f);    //dole levo
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-105f, -149.8f, -220f); //gore levo
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(-55f, -149.8f, -220f);  //gore desno
            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();


            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 120f);
            //druga ulica
            gl.MatrixMode(OpenGL.GL_TEXTURE);
            gl.PushMatrix();
            gl.Scale(1.0f, 6.0f, 0.0f);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.Translate(0.0f, 0.0f, 120f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(113f, -149.8f, 30f);   //dole desno
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-55f, -149.8f, 30f);    //dole levo
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-55f, -149.8f, -25f); //gore levo
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(113f, -149.8f, -25f);  //gore desno
            gl.End();
            gl.PopMatrix();
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D); 
        }


        public void DrawWalls(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Bricks]);

            // Cube klasa koja nam sluzi za iscvanje zidova
            Cube cube = new Cube();

            //levi gornji zid
            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(1.0f, 1.0f, 1.0f);  //gl.Color(0.43f, 0.55f, 0.63f);
            gl.Translate(60.0f, -134.9f, 38.0f);
            gl.Rotate(0.0f, 90.0f, 0.0f);
            gl.Scale(58.0f, 15.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();



            //levi donji zid
            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(60.0f, -134.9f, 213.0f);
            gl.Rotate(0.0f, 90.0f, 0.0f);
            gl.Scale(48.0f, 15.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            //desni zid
            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(220.0f, -134.9f, 121f);
            gl.Rotate(0.0f, 90.0f, 0.0f);
            gl.Scale(142.0f, 15.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            //gornji zid
            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(140.0f, -134.9f, -20.0f);
            gl.Rotate(0.0f, 0.0f, 0.0f);
            gl.Scale(80.5f, 15.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            //donji zid
            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(140.0f, -134.9f, 262.0f);
            gl.Rotate(0.0f, 0.0f, 0.0f);
            gl.Scale(80.5f, 15.0f, 1.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D);

        }

        public void DrawConstruction(OpenGL gl)
        {
            gl.Enable(OpenGL.GL_TEXTURE_2D);
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.WhiteAsphalt]);

            // Cube klasa koja nam sluzi za iscvanje zidova
            Cube cube = new Cube();
            cube.Material = new SharpGL.SceneGraph.Assets.Material();
            cube.Material.Ambient = Color.Red;
            cube.Material.Diffuse = Color.Red;

            //Sredisnji zid
            gl.PushMatrix();
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(150.0f, -78.5f, 130.0f);
            gl.Scale(40.0f, 60.0f, 100.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
        }


        public void DrawBox(OpenGL gl)
        {
            // Cube klasa koja nam sluzi za iscvanje zidova
            Cube cube = new Cube();

            //Sredisnji zid
            gl.PushMatrix();
            gl.Color(1.0f, 1.0f, 1.0f);
            gl.Translate(250.0f, -78.5f, 130.0f);
            gl.Scale(100.0f, 100.0f, 200.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        public void DrawRamp(OpenGL gl)
        {
            Cube cube = new Cube();

            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(0.5f, 0f, 0f);
            gl.Translate(0.0f, -139.9f, 91.0f);
            gl.Scale(3.0f, 10.0f, 3.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Normal(1f, 0f, 0f);
            gl.Color(0.5f, 0f, 0f);
            gl.Translate(0.0f, -139.9f, 155.0f);
            gl.Scale(3.0f, 10.0f, 3.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            Cylinder precka = new Cylinder();

            gl.PushMatrix();
            gl.Normal(0f, 1f, 0f);
            gl.Translate(0.0f, -133.0f, 89.0f);

            if (raiseRamp == 1)
            {
                gl.Rotate(-0.0f, -0.0f, 0.0f);
            }
            else if (raiseRamp == 2) 
            {
                gl.Rotate(-40.0f, -0.0f, 0.0f);
            }
            else if(raiseRamp == 3)
            {
                gl.Rotate(-80.0f, -0.0f, 0.0f);
            }
            gl.Scale(30.0f, 30.0f, 30.0f);
            precka.CreateInContext(gl);

            precka.Slices = 1500;
            precka.BaseRadius = 0.1f;
            precka.TopRadius = 0.1f;
            precka.Height = 2.1;
            gl.LineWidth(100.0f);

            precka.Render(gl, RenderMode.Render);
            gl.PopMatrix();
        }

        public void Animation()
        {
            this.boxChecker = false;

            this.animationInProgress = true;
            timer1 = new DispatcherTimer();
            timer1.Interval = TimeSpan.FromMilliseconds(50);
            timer1.Tick += new EventHandler(FirstStreet);
            timer1.Start();
        }

        public void FirstStreet(object sender, EventArgs e)
        {
            if (truckTranslateZ < 120)
            {
                truckTranslateZ += 20f;
            }
            else
            {
                truckRotationY = 105.0f;
                if (animationChecker == 0)
                {
                    Animation2();
                }
                else if (animationChecker == 1)
                {
                    Animation3();
                }
                timer1.Stop();

            }
        }

        public void SecondStreet(object sender, EventArgs e)
        {
            if (truckTranslateX < -20 && truckTranslateX > -35)
            {
                raiseRamp = 3;
                truckTranslateX += 10f;
            }
            else if (truckTranslateX < 85)
            {
                truckTranslateX += 10f;
            }
            else
            {
                Animation2_1();
                timer2.Stop();
            }
        }

        public void Animation2()
        {
            timer2 = new DispatcherTimer();
            timer2.Interval = TimeSpan.FromMilliseconds(1);
            timer2.Tick += new EventHandler(SecondStreet);
            timer2.Start();
        }

        public void Parking(object sender, EventArgs e)
        {
            if (truckTranslateZ > 80)
            {
                raiseRamp = 1;
                truckRotationY = 20f;
                truckTranslateZ -= 10f;
            }
            else
            {
                timer21.Stop();
            }
        }

        public void Animation2_1()
        {
            timer21 = new DispatcherTimer();
            timer21.Interval = TimeSpan.FromMilliseconds(1);
            timer21.Tick += new EventHandler(Parking);
            timer21.Start();
        }


        public void GoingBack(object sender, EventArgs e) //druga ulica pre rampe
        {
            if (truckTranslateX < -20 && truckTranslateX > -35)
            {
                raiseRamp = 1;

                //ove dve linije su za anim3
                timer3.Stop();
                Animation4();
            }
            else if (truckTranslateX < -20)
            {
                truckTranslateX += 10f;
            }
            else
            {
                Animation4();
                timer3.Stop();
            }
        }

        public void Animation3()
        {
            timer3 = new DispatcherTimer();
            timer3.Interval = TimeSpan.FromMilliseconds(1);
            timer3.Tick += new EventHandler(GoingBack);
            timer3.Start();
        }

        public void GoingBack2(object sender, EventArgs e)  //druga ulica posle rampe
        {
            if (truckTranslateX < -20 && truckTranslateX > -35)
            {
                raiseRamp = 1;
                truckRotationY = 285.0f;
                truckTranslateX -= 10f;
            }
            else if (truckTranslateX > -80)
            {
                truckTranslateX -= 10f;
            }
            else
            {

                Animation5();
                timer4.Stop();

            }
        }

        public void Animation4()
        {
            timer4 = new DispatcherTimer();
            timer4.Interval = TimeSpan.FromMilliseconds(250);
            timer4.Tick += new EventHandler(GoingBack2);
            timer4.Start();
        }

        public void GoingBack3(object sender, EventArgs e)  //prva ulica
        {
            if (truckTranslateZ > -35)
            {
                truckRotationY = 200;
                truckTranslateZ -= 20f;
            }
            else if (truckTranslateX > -80)
            {
                truckTranslateX -= 10f;
            }
            else
            {
                timer5.Stop();

            }
        }

        public void Animation5()
        {
            timer5 = new DispatcherTimer();
            timer5.Interval = TimeSpan.FromMilliseconds(1);
            timer5.Tick += new EventHandler(GoingBack3);
            timer5.Start();
        }


        /// <summary>
        /// Podesava viewport i projekciju za OpenGL kontrolu.
        /// </summary>
        public void Resize(OpenGL gl, int width, int height)
        {
            m_width = width;
            m_height = height;
            gl.Viewport(0, 0, m_width, m_height);
            gl.MatrixMode(OpenGL.GL_PROJECTION);      // selektuj Projection Matrix
            gl.LoadIdentity();
            gl.Perspective(50f, (double)width / height, 1f, 20000f);
            gl.MatrixMode(OpenGL.GL_MODELVIEW);
            gl.LoadIdentity();                // resetuj ModelView Matrix
        }

        /// <summary>
        ///  Implementacija IDisposable interfejsa.
        /// </summary>
        /// 
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                m_scene.Dispose();
                m_scene_holder.Dispose();
            }
        }

      
        #endregion Metode

        #region IDisposable metode

        /// <summary>
        ///  Dispose metoda.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable metode
    }
}
