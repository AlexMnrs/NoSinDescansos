using System;
using System.Collections.Generic;
using System.Windows;

namespace NoSinDescansos
{
    /// <summary>
    /// Lógica de interacción para Registro.xaml
    /// </summary>
    public partial class Registro : Window
    {
        public List<RegistroJornada> listaRegistro = new List<RegistroJornada>();
        
        public Registro()
        {
            InitializeComponent();

            RellenarRegistro();

        }


        /// <summary>
        /// Rellena el registro de jornadas
        /// </summary>
        internal void RellenarRegistro()
        {
            foreach (RegistroJornada item in listaRegistro)
            {
                Console.WriteLine(item.TiempoJornada);
            }

            dgRegistro.ItemsSource = listaRegistro;

        }


    }
}

