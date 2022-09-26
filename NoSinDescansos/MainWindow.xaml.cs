using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Media;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace NoSinDescansos
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timer = new DispatcherTimer();
        Stopwatch stopwatch = new Stopwatch();
        SoundPlayer player = new SoundPlayer();
        Registro registro = new Registro();
        TimeSpan ts;
        private string _tiempoActual = string.Empty;
        private int _tiempoJornadaEnMinutos;
        private int _tiempoParaDescansarEnMinutos = 30;
        private int _tiempoInicioJornada;
        private int _maximoJornadaEnMinutos = 480;
        private int _minimoJornadaEnMinutos = 15;
        private int _descansos;


        public int Descansos { get => _descansos; set => _descansos = value; }
        public int TiempoJornadaEnMinutos { get => _tiempoJornadaEnMinutos; set => _tiempoJornadaEnMinutos = value; }

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan();
            txtMinutos.Text = string.Format($"{_minimoJornadaEnMinutos} minutos");
            TiempoJornadaEnMinutos = _minimoJornadaEnMinutos;

        }

        /// <summary>
        /// Funcionamiento del evento tick del contador cuando transcurre el intervalo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object? sender, EventArgs e)
        {
            // Si el contador está en marcha...
            if (stopwatch.IsRunning)
            {
                // Activamos el botón parar
                btnParar.IsEnabled = true;
                //Desactivamos botón iniciar
                btnComenzar.IsEnabled = false;

                // Guardamos el tiempo transcurrido
                ts = stopwatch.Elapsed;

                // Formateamos el tiempo en horas:minutos:segundos
                _tiempoActual = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

                // Asignamos 'tiempoActual' al texto del contador
                txtContador.Text = _tiempoActual;

                // Si hay descansos por hacer significa que todavía no hemos acabado nuestra jornada, así que continuamos.
                while (ts.TotalMinutes < TiempoJornadaEnMinutos)
                {
                    // Si han pasado 30 minutos desde que comenzamos la cuenta...
                    if (ts.TotalMinutes - _tiempoInicioJornada >= _tiempoParaDescansarEnMinutos && Descansos > 0)
                    {
                        // Hacemos un descanso
                        Descansos--;

                        // Activamos botón comenzar
                        btnComenzar.IsEnabled = true;

                        // Desactivamos botón parar
                        btnParar.IsEnabled = false;

                        // Asignamos un sonido de alarma al reproductor
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\alarma.wav";

                        // Reproducimos la alarma en bucle
                        player.PlayLooping();

                        // Paramos el contador
                        stopwatch.Stop();
                    }
                    return;
                }
                registro.listaRegistro.Add(new RegistroJornada(TiempoJornadaEnMinutos.ToString(), Descansos));
                btnParar.IsEnabled = false;
                btnComenzar.IsEnabled = false;
                Trace.WriteLine(registro.listaRegistro.ToString());
                stopwatch.Stop();
            }

        }

        /// <summary>
        /// Inicia el contador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComenzar_Click(object sender, RoutedEventArgs e)
        {
            _tiempoInicioJornada = (int)ts.TotalMinutes;

            player.Stop(); // Paramos la alarma al continuar la cuenta atrás después de un descanso

            // Calculamos los descansos sólo la primera vez que ejecutamos el contador
            if (Descansos == 0 && _tiempoInicioJornada == 0)
            {
                Descansos = CalcularDescansos(TiempoJornadaEnMinutos);
            }

            stopwatch.Start();
            timer.Start();
        }

        /// <summary>
        /// Calcula los descansos en base al tiempo total de la jornada. Un descanso cada 30 minutos.
        /// </summary>
        /// <param name="minutos"></param>
        /// <returns></returns>
        private int CalcularDescansos(int minutos) => Descansos = (minutos > 30) ? minutos / 30 : 0;

        /// <summary>
        /// Resetea el contador
        /// </summary>
        private void ResetearContador()
        {
            stopwatch.Reset();
            txtContador.Text = "00:00:00";
            txtMinutos.Text = string.Format($"{_minimoJornadaEnMinutos} minutos");
            btnComenzar.IsEnabled = true;
            btnParar.IsEnabled = false;
        }

        /// <summary>
        /// Para el contador
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnParar_Click(object sender, RoutedEventArgs e)
        {
            btnComenzar.IsEnabled = true;
            btnParar.IsEnabled = false;
            stopwatch.Stop();
        }

        /// <summary>
        /// Llama la función Resetearcontador(); al hacer clic en el botón RESETEAR
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetear_Click(object sender, RoutedEventArgs e)
        {
            ResetearContador();
        }

        /// <summary>
        /// Aumenta los minutos de la jornada al pulsar el botón INCREMENTAR y actualiza el texto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Incrementar_Click(object sender, RoutedEventArgs e)
        {

            if (TiempoJornadaEnMinutos < _maximoJornadaEnMinutos)
            {
                TiempoJornadaEnMinutos = TiempoJornadaEnMinutos + 15;
                txtMinutos.Text = string.Format($"{TiempoJornadaEnMinutos} minutos");
            }
        }

        /// <summary>
        /// Disminuye los minutos de la jornada al pulsar el botón DECREMENTAR y actualiza el texto
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decrementar_Click(object sender, RoutedEventArgs e)
        {

            if (TiempoJornadaEnMinutos > _minimoJornadaEnMinutos)
            {
                TiempoJornadaEnMinutos = TiempoJornadaEnMinutos - 15;
                txtMinutos.Text = string.Format($"{TiempoJornadaEnMinutos} minutos");
            }
        }

        /// <summary>
        /// Arrastra la ventana al mantener pulsado el clic del ratón
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        /// <summary>
        /// Deshabilita la escritura por teclado en el textbox donde se indican los minutos de la jornada
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }


        /// <summary>
        /// Muestra una nueva ventana con un registro de todas las jornadas realizadas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRegistro_Click(object sender, RoutedEventArgs e)
        {
            registro.Show();
        }
    }
}
