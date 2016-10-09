using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Austeroids
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //AudioGenerator audioGen = new AudioGenerator();
        public MainWindow()
        {
            InitializeComponent();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //audioGen.MakeSomeNoise();
            World world = new World();
            world.Create<Entities.Ship>();
            //world.Create<Entities.Ship>(new Vector(50, 50));

            for (int i = 0; i < 10; i++)
            {
                world.Create<Entities.Asteroid>();
            }

            world.StartGame();

            while (true)
            {
                world.Tick();

                //forgive me father for I have sinned
                System.Windows.Forms.Application.DoEvents();
            }
        }
    }
}
