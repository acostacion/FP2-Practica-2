// Andrea Aparicio López.
// José Tomás Gómez Becerra.

namespace FP2P2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Mostillo();
        }

        static void Mostillo()
        {
            // Para la comprobacion de la existencia del archivo File.Exist creo...

            // Abrimos flujo de archivo y se leen los niveles de levels/level0X.dat. ¡¡¡Ojo, meter luego ($"levels/{file}")!!!
            StreamReader archivo = new StreamReader($"levels/level00.dat");

            // Habrá dos lecturas:
            // 1. Para determinar el tamaño de la matriz.
            int fils = SacaFilas(archivo);
            int cols = SacaColumnas(archivo);
            int[,] tableroNumeros = new int[fils, cols];

            // 2. Para ir rellenando la matriz.
            for (int i = 0; i < tableroNumeros.GetLength(0); i++)
            {
                // Lee la fila actual.
                string s = archivo.ReadLine();

                // En el array v va almacenando el contenido de la fila actual pero sin los espacios. Se guardan los números.
                string[] v = s.Split(' ', StringSplitOptions.RemoveEmptyEntries);

                // String -> Int.
                int[] myInts = Array.ConvertAll(v, int.Parse);

                for (int j = 0; j < tableroNumeros.GetLength(1); j++)
                {
                    // Rellena.
                    tableroNumeros[i, j] = myInts[j];
                    Console.Write($"{tableroNumeros[i, j]} ");
                }
            }

            // Cerramos flujo.
            archivo.Close();
        }

        static int SacaColumnas(StreamReader archivo)
        {
            int numCols = 0; // Inicialmente el tamaño es 0.
            string leeCols = archivo.ReadLine().Replace(" ", ""); // Lee la linea y elimina los espacios en blanco.
            numCols = leeCols.Length; // Saca la cantidad de columnas.
            return numCols;
        }

        static int SacaFilas(StreamReader archivo)
        {
            int numFils = 1; // Empezamos a contar desde 1.

            while (archivo.ReadLine() != null)
            {
                numFils++;
            }

            return numFils;
        }
    }
}
