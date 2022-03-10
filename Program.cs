using System;
using System.IO;
using System.Text;

namespace _1._2laba
{
    public class Program
    {
        private struct Coefficients
        {
            public double a, b;
        };

        static void Main()
        {
            const double step = 0.2;
            const double a = 1;
            const double b = 2;

            int size = 0;
            double[,] matrix = generate(out size, a, b, step);

            Console.WriteLine("Исходная табличная функция");
            print(matrix);

            Coefficients coefficients = calculateCoefficients(matrix, size);

            Console.WriteLine($"Полученные коэффиценты\na = {coefficients.a}\nb = {coefficients.b}");

            double[] approxValues = new double[size];

            for (int i = 0; i < size; i++)
            {
                approxValues[i] = fApprox(coefficients, matrix[0, i]);
            }

            Console.WriteLine($"\nМера отклонения: {getPrecision(matrix, approxValues, size)}");

            var accurateStringValuesY = "";
            var approxStringValuesY = "";
            var stringValuesX = "";

            for (var i = 0; i < size; i++)
            {
                stringValuesX += matrix[0, i] + " ";
                accurateStringValuesY += matrix[1, i] + " ";
                approxStringValuesY += approxValues[i] + " ";
            }

            //using var fstream = new FileStream("data.txt", FileMode.OpenOrCreate);
            //fstream.Write(Encoding.Default.GetBytes(stringValuesX + "\n"));
            //fstream.Write(Encoding.Default.GetBytes(accurateStringValuesY + "\n"));
            //fstream.Write(Encoding.Default.GetBytes(approxStringValuesY + "\n"));

            Console.Read();
        }

        static double getPrecision(double[,] accurateValues, double[] approxValues, int size)
        {
            double precision = 0;
            for (var i = 0; i < size; i++)
            {
                Console.WriteLine(Math.Round(accurateValues[1, i], 3) + " : " + Math.Round(approxValues[i], 3));
                precision += (approxValues[i] - accurateValues[1, i]) * (approxValues[i] - accurateValues[1, i]);
            }

            return precision;
        }

        static Coefficients calculateCoefficients(double[,] data, int size)
        {
            Coefficients result;

            double ySumm = data[1, 0];
            for (int i = 1; i < size; ++i)
            {
                ySumm += data[1, i];
            }

            double xSumm = Math.Log(data[0, 0]);
            for (int i = 1; i < size; ++i)
            {
                xSumm += Math.Log(data[0, i]);
            }

            double xSquareSumm = Math.Pow(Math.Log(data[0, 0]), 2);
            for(int i = 1; i < size; ++i)
            {
                xSquareSumm += Math.Pow(Math.Log(data[0, i]), 2);
            }

            double xSummY = 0;
            for (int i = 0; i < size; ++i)
            {
                xSummY += Math.Log(data[0, i]) * data[1, i];
            }

            result.b = (size * xSummY - ySumm * xSumm) / (size * xSquareSumm - xSumm * xSumm);
            result.a = (ySumm - result.b * xSumm) / size;

            return result;
        }

        //static Coefficients calculateCoefficients(double[,] data, int size)
        //{
        //    Coefficients result;

        //    double ySumm = data[1, 0];
        //    for(int i = 1; i < size; ++i)
        //    {
        //        ySumm += data[1, i];
        //    }

        //    double xSumm = Math.Log(data[0, 0]);
        //    for(int i = 1; i < size; ++i)
        //    {
        //        xSumm += Math.Log(data[0, i]);
        //    }

        //    double xSummY = 0;
        //    for (int i = 0; i < size; ++i)
        //    {
        //        xSummY += Math.Log(data[0, i]) * data[1, i];
        //    }

        //    result.a = (ySumm - xSummY / xSumm) / size;
        //    result.b = xSummY / (xSumm * xSumm);

        //    return result;
        //}

        private static double fApprox(Coefficients coefficients, double x)
        {
            return coefficients.a + coefficients.b * Math.Log(x);
        }

        private static void print(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            int m = matrix.GetLength(1);
            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < m; ++j)
                {
                    Console.Write(Math.Round(matrix[i, j], 3) + " ");
                }
                Console.WriteLine();
            }
        }

        private static void divideRow(ref double[,] matrix, double amount, int row, int size)
        {
            for (var i = 0; i < size; i++)
            {
                matrix[row, i] /= amount;
            }
        }

        private static void substractRow(ref double[,] matrix, int rowToSubstract, int rowFromSubstract, double mult, int size)
        {
            for (var i = 0; i < size; i++)
            {
                matrix[rowFromSubstract, i] -= matrix[rowToSubstract, i] * mult;
            }
        }

        private static double[,] generate(out int size, double a, double b, double step/*, bool nodelta = false*/)
        {
            size = (int)Math.Round((b - a) / step) + 1;

            double[,] result = new double[2, size];

            fillingMatrix(result, a, b, step, size);
            double fmax = findMax(result, size);
            addingRandomNumbers(result, size, fmax);

            return result;
        }

        private static void fillingMatrix(double[,] matrix, double a, double b, double step, int size)
        {
            for (int i = 0; i < size; ++i)
            {
                matrix[0, i] = a + i * step;
                matrix[1, i] = f(matrix[0, i]);
            }
        }

        private static double findMax(double[,] matrix, double size)
        {
            double max = matrix[1, 0];
            for (int i = 1; i < size; ++i)
            {
                max = Math.Max(max, matrix[1, i]);
            }
            return max;
        }

        private static void addingRandomNumbers(double[,] matrix, int size, double max)
        {
            Random rnd = new Random();

            double d = 0.2 * max;
            for (int i = 0; i < size; ++i)
            {
                double delta = rnd.NextDouble() * d - d / 2;
                matrix[1, i] += delta;
            }
        }

        private static double f(double x)
        {
            return 0.6 + 0.5 * Math.Log(x);
        }
    }
}
