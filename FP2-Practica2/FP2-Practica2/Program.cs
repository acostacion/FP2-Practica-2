// Andrea Aparicio López.
// José Tomás Gómez Becerra.

namespace FP2P2
{
    internal class Program
    {


        static void Main(string[] args)
        {
            string file = "levels/level00.dat";
            Tablero t = new Tablero(file);
            t.Render();

            int lap = 200; // retardo para bucle ppal
            char c = ' ';

            int i = 0;
            while (i < 200) {
                // input de usuario
                LeeInput(ref c);
                // procesamiento del input
                if (c !=' ' && t.CambiaDir(c)) c =' ';

                t.MuevePacman();

                // IA de los fantasmas: TODO

                // rederizado
                t.Render();

                // retardo
                System.Threading.Thread.Sleep(lap);
                i++;
            }
        }

        static void LeeInput(ref char dir)
        {
            if (Console.KeyAvailable)
            {
                string tecla = Console.ReadKey(true).Key.ToString();
                switch (tecla)
                {
                    case "LeftArrow": dir = 'l'; break;
                    case "UpArrow": dir = 'u'; break;
                    case "RightArrow": dir = 'r'; break;
                    case "DownArrow": dir = 'd'; break;
                }
            }
            while (Console.KeyAvailable) Console.ReadKey().Key.ToString();
        }

    }
}