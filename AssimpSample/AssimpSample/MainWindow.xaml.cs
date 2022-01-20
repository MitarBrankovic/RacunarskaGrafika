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
            }
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

        }

        private void OkBtn_Click(object sender, RoutedEventArgs e)
        {
            //Reflektor ambijentalno
            /*try
            {
                float red = float.Parse(this.txtRed.Text);
                if (red < 0.0f || red > 1.0f)
                {
                    MessageBox.Show("Value RED must be a number between 0.0 and 1.0!");
                }
                else
                {
                    m_world.ReflectorAmbientRed = red;
                }

                float green = float.Parse(this.txtGreen.Text);
                if (green < 0.0f || green > 1.0f)
                {
                    MessageBox.Show("Value GREEN must be a number between 0.0 and 1.0!");
                }
                else
                {
                    m_world.ReflectorAmbientGreen = green;
                }

                float blue = float.Parse(this.txtBlue.Text);
                if (blue < 0.0f || blue > 1.0f)
                {
                    MessageBox.Show("Value BLUE must be a number between 0.0 and 1.0!");
                }
                else
                {
                    m_world.ReflectorAmbientBlue = blue;
                }

            }
            catch
            {
                MessageBox.Show("Value of RED, GREEN and BLUE must be a number between 0.0 and 1.0!");
            }*/

            //Transliranje po Y
            try
            {
                m_world.RaiseRamp = float.Parse(this.raiseRamp.Text);
            }
            catch
            {
                MessageBox.Show("Translation must be a number!");
            }

            //Skaliranje strelica
            try
            {
                m_world.ScaleTruck = double.Parse(this.scaleTruck_txt.Text);
            }
            catch
            {
                MessageBox.Show("Scaling must be a number!");
            }
        }

    }
}
