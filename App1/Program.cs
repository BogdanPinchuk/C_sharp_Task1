using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

using App1.Product;

/// <summary>
/// Додаток для ручного створення БД
/// </summary>
namespace App1
{
    class Program
    {
        static void Main()
        {
            // Join Unicode
            Console.OutputEncoding = Encoding.Unicode;

            // Створення файла БД
            string provAndFile = CreateDB.CreateFile("Product");

            // Створення таблиць згідно умові
            CreateDB.CreateTable(provAndFile);

            #region Testing Data Base
#if true
            // занесення даних
            for (int i = 0; i < 10; i++)
            {
                CreateDB.AddDrink(i, "Product " + i, i * 5.5, provAndFile);
                CreateDB.AddAdditiv(i, "Product " + i + 10, i * 5.5, provAndFile);
            }

            // зміна даних
            CreateDB.ChangeDrink(3, "New Product " + 3, 3 * 5.5 * 1.2, provAndFile);
            CreateDB.ChangeAdditiv(3, "New Product " + 3, 3 * 5.5 * 1.2, provAndFile);

            // видалення даних

#endif 
            #endregion

            // фінальне сповіщення
            Console.WriteLine("\n\tНе забудьте спопіювати дану БД у відповідну папку, або вказати на неї правильний шлях.");

            // delay
            Console.ReadKey(true);
        }
    }
}
