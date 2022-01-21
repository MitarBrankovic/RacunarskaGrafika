using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using SharpGL.SceneGraph;
using SharpGL;
using Microsoft.Win32;
using System.Collections.ObjectModel;

namespace AssimpSample
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Atributi

        /// <summary>
        ///	 Instanca OpenGL "sveta" - klase koja je zaduzena za iscrtavanje koriscenjem OpenGL-a.
        /// </summary>
        World m_world = null;

       
        #endregion Atributi

        #region Konstruktori

        public MainWindow()
        {
            
            // Inicijalizacija komponenti
            InitializeComponent();

            // Kreiranje OpenGL sveta
            try
            {
                m_world = new World(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "3D Models\\Truck"), "camion jugete.obj", (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight, openGLControl.OpenGL);
            }
            catch (Exception e)
            {
                MessageBox.Show("Neuspesno kreirana instanca OpenGL sveta. Poruka greške: " + e.Message, "Poruka", MessageBoxButton.OK);
                this.Close();
            }
            m_world.SceneDistance = 1000f;
        }

        #endregion Konstruktori

        /// <summary>
        /// Handles the OpenGLDraw event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLDraw(object sender, OpenGLEventArgs args)
        {
            m_world.Draw(args.OpenGL);
        }

        /// <summary>
        /// Handles the OpenGLInitialized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_OpenGLInitialized(object sender, OpenGLEventArgs args)
        {
            m_world.Initialize(args.OpenGL);
        }

        /// <summary>
        /// Handles the Resized event of the openGLControl1 control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="args">The <see cref="SharpGL.SceneGraph.OpenGLEventArgs"/> instance containing the event data.</param>
        private void openGLControl_Resized(object sender, OpenGLEventArgs args)
        {
            m_world.Resize(args.OpenGL, (int)openGLControl.ActualWidth, (int)openGLControl.ActualHeight);
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            this.okBtn.IsEnabled = true;


            switch (e.Key)
            {
                case Key.Q: this.Close(); break;
                case Key.I:
                    if (m_world.RotationX < -15)
                    {
                        break;
                    }
                    else
                    {
                        m_world.RotationX -= 5.0f;
                    } 
                    break;
                case Key.K:
                    if (m_world.RotationX > 65)
                    {
                        break;
                    }
                    else
                    {
                        m_world.RotationX += 5.0f;
                    }
                    break;
                case Key.J: m_world.RotationY -= 5.0f; break;
                case Key.L: m_world.RotationY += 5.0f; break;
                case Key.Add: m_world.SceneDistance -= 80.0f; break;
                case Key.Subtract: m_world.SceneDistance += 80.0f; break;
                case Key.C:
                    m_world.animationChecker = 0;
                    m_world.truckTranslateX = -80f;
                    m_world.truckTranslateY = -149.8f;
                    m_world.truckTranslateZ = -20f;
                    m_world.truckRotationX = 2.0f;
                    m_world.truckRotationY = 20.0f;
                    m_world.truckRotationZ = 0.0f;
                    m_world.RaiseRamp = 1;

                    m_world.Animation();
                    this.okBtn.IsEnabled = false;
                    break;
                case Key.V:
                    m_world.animationChecker = 1;
                    m_world.truckTranslateX = -80f;
                    m_world.truckTranslateY = -149.8f;
                    m_world.truckTranslateZ = -20f;
                    m_world.truckRotationX = 2.0f;
                    m_world.truckRotationY = 20.0f;
                    m_world.truckRotationZ = 0.0f;
                    m_world.RaiseRamp = 1;

                    m_world.Animation();
                    this.okBtn.IsEnabled = false;
                    break;
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            //Reflektor ambijentalno
            try
            {
                float red = float.Parse(this.txtRed.Text) / 10;
                if (red < 0.0f || red > 1.0f)
                {
                    MessageBox.Show("Vrednost crvenog polja mora biti izmedju 0.0 i 1.0!");
                }
                else
                {
                    float value = (float)red;
                    m_world.reflectorR = red;
                }

                float green = float.Parse(this.txtGreen.Text) / 10;
                if (green < 0.0f || green > 1.0f)
                {
                    MessageBox.Show("Vrednost zelenog polja mora biti izmedju 0.0 i 1.0!");
                }
                else
                {
                    float value = (float)green;
                    m_world.reflectorG = green;
                }

                float blue = float.Parse(this.txtBlue.Text) / 10;
                if (blue < 0.0f || blue > 1.0f)
                {
                    MessageBox.Show("Vrednost plavog polja mora biti izmedju 0.0 i 1.0!");
                }
                else
                {
                    float value = (float)blue;
                    m_world.reflectorB = blue;
                }

            }
            catch
            {
                MessageBox.Show("Value of RED, GREEN and BLUE must be a number between 0.0 and 1.0!");
            }

            //Transliranje po Y
            try
            {
                if (this.raiseRamp.SelectedIndex == 0)
                {
                    //MessageBox.Show("Ovo je 1x");
                    m_world.RaiseRamp = 1;
                }
                else if (this.raiseRamp.SelectedIndex == 1) 
                {
                    //MessageBox.Show("Ovo je 2x");
                    m_world.RaiseRamp = 2;
                }
                else if (this.raiseRamp.SelectedIndex == 2)
                {
                    //MessageBox.Show("Ovo je 3x");
                    m_world.RaiseRamp = 3;
                }


            }
            catch
            {
                MessageBox.Show("Greska kod rampe!");
            }

            //Skaliranje strelica
            try
            {
                if (this.scaleTruck_txt.SelectedIndex == 0)
                {
                    //MessageBox.Show("Ovo je 1x");
                    m_world.ScaleTruck = 1;
                }
                else if (this.scaleTruck_txt.SelectedIndex == 1)
                {
                    //MessageBox.Show("Ovo je 2x");
                    m_world.ScaleTruck = 2;
                }
                else if (this.scaleTruck_txt.SelectedIndex == 2)
                {
                    //MessageBox.Show("Ovo je 0.5x");
                    m_world.ScaleTruck = 0.5;
                }
            }
            catch
            {
                MessageBox.Show("Scaling must be a number!");
            }
        }

    }
}
