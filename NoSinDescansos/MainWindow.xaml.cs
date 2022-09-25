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
        TimeSpan ts;
        private string _tiempoActual = string.Empty;
        private int _tiempoJornadaEnMinutos;
        private int _tiempoParaDescansarEnMinutos = 30;
        private int _tiempoInicioJornada;
        private int _maximoJornadaEnMinutos = 480;
        private int _minimoJornadaEnMinutos = 15;
        private int _descansos;
        private bool _jornadaTerminada = false;

        public MainWindow()
        {
            InitializeComponent();

            timer.Tick += Timer_Tick;
            timer.Interval = new TimeSpan();
            btnParar.IsEnabled = false;
            txtMinutos.Text = string.Format($"{_minimoJornadaEnMinutos} minutos");
            _tiempoJornadaEnMinutos = _minimoJornadaEnMinutos;

            if (_jornadaTerminada)
            {
                btnComenzar.IsEnabled = false;
            }
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

                // Guardamos el tiempo transcurrido
                ts = stopwatch.Elapsed;

                // Formateamos el tiempo en horas:minutos:segundos
                _tiempoActual = string.Format("{0:00}:{1:00}:{2:00}", ts.Hours, ts.Minutes, ts.Seconds);

                // Asignamos 'tiempoActual' al texto del contador
                txtContador.Text = _tiempoActual;

                // Si hay descansos por hacer significa que todavía no hemos acabado nuestra jornada, así que continuamos.
                while (ts.TotalMinutes < _tiempoJornadaEnMinutos)
                {
                    // Si han pasado 30 minutos desde que comenzamos la cuenta...
                    if (ts.TotalMinutes - _tiempoInicioJornada >= _tiempoParaDescansarEnMinutos && _descansos > 0)
                    {
                        // Quitamos un descanso
                        _descansos--;

                        // Asignamos un sonido de alarma al reproductor
                        player.SoundLocation = AppDomain.CurrentDomain.BaseDirectory + "\\alarma.wav";

                        // Reproducimos la alarma en bucle
                        player.PlayLooping();

                        // Paramos el contador
                        stopwatch.Stop();
                    }
                    return;
                }
                btnComenzar.IsEnabled = false; // Si no hay descansos pendientes (jornada terminada), apagamos el botón comenzar. (ESTÁ BUGEADO!!!!)
                _jornadaTerminada = true;
                stopwatch.Stop();
            }
            else { btnParar.IsEnabled = false; } // Si el contador no está en marcha, botón parar desactivado.
        }

        /// <summary>
        /// Funcionamiento del evento clic en el botón comenzar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnComenzar_Click(object sender, RoutedEventArgs e)
        {
            _tiempoInicioJornada = (int)ts.TotalMinutes;

            player.Stop(); // Paramos la alarma al continuar la cuenta atrás después de un descanso

            // Calculamos los descansos sólo la primera vez que ejecutamos el contador
            if (_descansos == 0 && _tiempoInicioJornada == 0)
            {
                _descansos = CalcularDescansos(_tiempoJornadaEnMinutos);
            }

            stopwatch.Start();
            timer.Start();
            Incrementar.IsEnabled = false;
            Decrementar.IsEnabled = false;
        }

        /// <summary>
        /// Calcula los descansos en base al tiempo total de la jornada. Un descanso cada 30 minutos.
        /// </summary>
        /// <param name="minutos"></param>
        /// <returns></returns>
        private int CalcularDescansos(int minutos) => _descansos = (minutos > 30) ? minutos / 30 : 0;

        /// <summary>
        /// Resetea el contador
        /// </summary>
        private void ResetearContador()
        {
            stopwatch.Reset();
            txtContador.Text = "00:00:00";
            btnComenzar.IsEnabled = true;
        }

        /// <summary>
        /// Funcionamiento del evento click en el botón parar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnParar_Click(object sender, RoutedEventArgs e)
        {
            if (!stopwatch.IsRunning) return;
            stopwatch.Stop();
        }

        /// <summary>
        /// Funcionamiento del evento clic en el botón resetear
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResetear_Click(object sender, RoutedEventArgs e)
        {
            ResetearContador();
        }

        /// <summary>
        /// Funcionamiento del evento clic en el botón incrementar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Incrementar_Click(object sender, RoutedEventArgs e)
        {

            if (_tiempoJornadaEnMinutos < _maximoJornadaEnMinutos)
            {
                _tiempoJornadaEnMinutos += 15;
                txtMinutos.Text = string.Format($"{_tiempoJornadaEnMinutos} minutos");
            }
        }

        /// <summary>
        /// Funcionamiento del evento clic en el botón decrementar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Decrementar_Click(object sender, RoutedEventArgs e)
        {

            if (_tiempoJornadaEnMinutos > _minimoJornadaEnMinutos)
            {
                _tiempoJornadaEnMinutos -= 15;
                txtMinutos.Text = string.Format($"{_tiempoJornadaEnMinutos} minutos");
            }
        }

        /// <summary>
        /// Funcionamiento del evento mousedown (botón pulsado del ratón) de la ventana principal
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
        /// Funcionamiento del evento keydown (tecla pulsada) en el textbox: deshabilita la escritura
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }
    }
}
