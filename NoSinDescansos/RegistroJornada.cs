namespace NoSinDescansos
{
    public class RegistroJornada
    {

        private string _tiempoJornada;
        private int _descansosRealizados;


        public string TiempoJornada { get => _tiempoJornada; set => _tiempoJornada = value; }
        public int DescansosRealizados { get => _descansosRealizados; set => _descansosRealizados = value; }

        public RegistroJornada(string pTiempoJornada, int pDescansosRealizados)
        {
            _tiempoJornada = pTiempoJornada;
            _descansosRealizados = pDescansosRealizados;
        }

    }
}
