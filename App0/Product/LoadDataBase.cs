using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.Product
{
    /// <summary>
    /// Завантаження БД (бази даних)
    /// </summary>
    class LoadDataBase
    {
        /// <summary>
        /// Провайдер і шлях до файлу БД
        /// </summary>
        private readonly string connect;
        /// <summary>
        /// Назви таблиць
        /// </summary>
        private readonly List<string> table = new List<string>() { "Drinks", "Additivs" };

        /// <summary>
        /// Автономна база даних продуктів
        /// </summary>
        public DataSet Products { get; private set; }

        /// <summary>
        /// Завантаження БД
        /// </summary>
        /// <param name="path">Шлях до БД</param>
        public LoadDataBase(string pathFile)
        {
            // провайдер і шлях до файлу БД
            connect = "Provider=Microsoft.Jet." +
                    $@"OLEDB.4.0; Data Source = {pathFile}.mdb";

            // відовлення виключень, наприклад наявності файла
            try
            {
                // створення з'єднання
                using (OleDbConnection conn = new OleDbConnection(connect))
                {
                    // відкриття
                    conn.Open();

                    // створення об'єкта керування
                    OleDbCommand comm = conn.CreateCommand();

                    // задання команди
                    comm.CommandText = $"Select * From {table[0]}";

                    // створення адаптера з передачею команди
                    OleDbDataAdapter adapter = new OleDbDataAdapter(comm);

                    // Занесення даних в автономку БД
                    Products = new DataSet(table[0]);
                    adapter.Fill(Products, table[0]);

                    // задання команди
                    comm.CommandText = $"Select * From {table[1]}";

                    // передачею команди в адаптер
                    adapter.SelectCommand = comm;

                    // Занесення даних в автономку БД
                    adapter.Fill(Products, table[1]);
                }
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


    }
}
