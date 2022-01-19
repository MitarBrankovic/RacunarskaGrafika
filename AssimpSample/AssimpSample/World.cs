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


        private uint[] m_textures = new uint[5];
        private string[] m_textureFiles = { "..//..//imgs//bricks.jpg", "..//..//imgs//brick.jpg", "..//..//imgs//asphalt.jpg", "..//..//imgs//whiteAsphalt.jpg", "..//..//imgs//walls.jpg" };
        private enum TextureObjects { Bricks = 0, Brick, Asphalt, WhiteAsphalt, Walls};
        private readonly int m_textureCount = Enum.GetNames(typeof(TextureObjects)).Length;

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
            this.m_scene_holder = new AssimpScene(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Truck"), "camion jugete.obj", gl);
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
            //I DALJE SVETLOST

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
            DrawHolder(gl);
            DrawConstruction(gl);
            DrawWalls(gl);
            DrawRamp(gl);
            DrawText(gl);
            gl.PopMatrix();
            // Oznaci kraj iscrtavanja
            gl.Flush();
        }


        public void DrawHolder(OpenGL gl)
        {
            gl.PushMatrix();
            gl.Translate(-80f, -149.8f, -20f);
            gl.Scale(100f, 100f, 100f);
            gl.FrontFace(OpenGL.GL_CW);
            m_scene_holder.Draw();
            gl.PopMatrix();
        }

        public void DrawText(OpenGL gl)
        {
            gl.Viewport(m_width - 200, 0, m_width, m_height);
            gl.PushMatrix();
            gl.LoadIdentity();
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

            //podloga
            gl.PushMatrix();
            gl.Translate(0.0f, 0.0f, 120f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(6f, 6f, 5f);
            gl.Vertex(250f, -150f, 220f);
            gl.Vertex(-220f, -150f, 220f);
            gl.Vertex(-220f, -150f, -220f);
            gl.Vertex(250f, -150f, -220f);
            gl.End();
            gl.PopMatrix();

            gl.Enable(OpenGL.GL_TEXTURE_2D);

            //prva ulica
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.Translate(0.0f, 0.0f, 120f);
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


            //druga ulica
            gl.PushMatrix();
            gl.BindTexture(OpenGL.GL_TEXTURE_2D, m_textures[(int)TextureObjects.Asphalt]);
            gl.Translate(0.0f, 0.0f, 120f);
            gl.Begin(OpenGL.GL_QUADS);
            gl.Normal(0.0f, 1.0f, 0.0f);
            gl.Color(0.5f, 0.5f, 0.5f);
            gl.TexCoord(1.0f, 1.0f);
            gl.Vertex(93f, -149.8f, 30f);   //dole desno
            gl.TexCoord(1.0f, 0.0f);
            gl.Vertex(-55f, -149.8f, 30f);    //dole levo
            gl.TexCoord(0.0f, 0.0f);
            gl.Vertex(-55f, -149.8f, -25f); //gore levo
            gl.TexCoord(0.0f, 1.0f);
            gl.Vertex(93f, -149.8f, -25f);  //gore desno
            gl.End();
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

            //Sredisnji zid
            gl.PushMatrix();
            gl.Color(1.0f, 1.0f, 1.0f);     //0 0 0.4 plava
            gl.Translate(140.0f, -78.5f, 120.0f);
            gl.Scale(50.0f, 60.0f, 110.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.Disable(OpenGL.GL_TEXTURE_2D);
        }

        public void DrawRamp(OpenGL gl)
        {
            Cube cube = new Cube();

            gl.PushMatrix();
            gl.Color(0.5f, 0f, 0f);
            gl.Translate(0.0f, -139.9f, 91.0f);
            gl.Scale(3.0f, 10.0f, 3.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            gl.PushMatrix();
            gl.Color(0.5f, 0f, 0f);
            gl.Translate(0.0f, -139.9f, 155.0f);
            gl.Scale(3.0f, 10.0f, 3.0f);
            cube.Render(gl, RenderMode.Render);
            gl.PopMatrix();

            Cylinder precka = new Cylinder();

            gl.PushMatrix();
            gl.Translate(0.0f, -133.0f, 94.5f);
            gl.Rotate(-0.0f, -0.0f, -90.0f);
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
