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
            StreamReader archivo = new StreamReader($"levels/level01.dat");

            // Habrá dos lecturas:
            // 1. Para determinar el tamaño de la matriz.
            int fils = SacaFilas(archivo);
            int cols = SacaColumnas(archivo);
            Console.Write($"{fils} {cols}");
            int[,] tableroNumeros = new int[fils, cols];

            // 2. Para ir rellenando la matriz. // Mirar a ver si la parte 2 está bien, ya que no la he comprobado.
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

            /* FORMA QUE NO DEBERÍA DAR ERROR PERO ME PARECE POCO EFICIENTE

            using (StreamReader archivo = new StreamReader($"levels/level01.dat"))
            {
                int fils = SacaFilas(archivo);
                Console.Write($"{fils} ");
            }

            // Reopen the file and determine the number of columns.
            using (StreamReader archivo = new StreamReader($"levels/level01.dat"))
            {
                int cols = SacaColumnas(archivo);
                Console.Write($"{cols}");
            }*/


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
