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
    static class LoadDataBase
    {
        // Примітка. Згідно умови ТЗ достатньо було б використати нестатичний клас,
        // але якщо думати на перед, про можливість розширення програми в майбутньому
        // то краще скористатися статичним класом. Це дасть перевагу в тому, до можна 
        // в одному потоці запустити виконання обробки запитів користувача,
        // а в іншому потоці обробку запитів сервера, для оновлення БД, де
        // для внесення змін не потрібно буде зупитяти роботу обробки запитів користувача
        // так як внесенні зміни сервером стануть зразу доступні користувачу

        /// <summary>
        /// Провайдер і шлях до файлу БД
        /// </summary>
        private static string connect;
        /// <summary>
        /// Назви таблиць
        /// </summary>
        private static readonly List<string> table =
            new List<string>() { "Drinks", "Additivs" };
        /// <summary>
        /// Автономна база даних продуктів
        /// </summary>
        private static DataSet products;
        /// <summary>
        /// Блокує доступ потоків
        /// </summary>
        private static readonly object block = new object();
        /// <summary>
        /// Перевірка успошності завантаження БД
        /// </summary>
        public static bool Succesfull { get; private set; } = false;

        /// <summary>
        /// Таблиці продуктів
        /// </summary>
        public static DataSet Products
        {
            get { lock (block) { return products; } }
            set { lock (block) { products = value; } }
        }
        // Примітка. Постійне підключення до файла БД не ефективне, хоча займатиме
        // менше оперативної пам'яті. Але запити і доступ елементів через автономну
        // БД буде суттєво швидше + Нам не потрібно буде переривати роботу програми
        // якщо в нас оновиться БД, просто необхідно виконати оновлення автономної БД
        // в той чай як при постійному доступі до файлу БД, не було б змоги знобити
        // оновлення даних підчас роботи додатку.

        /// <summary>
        /// Завантаження БД
        /// </summary>
        /// <param name="path">Шлях до БД</param>
        public static void UpLoadDataBase(string pathFile)
        {
            // провайдер і шлях до файлу БД
            connect = "Provider=Microsoft.Jet." +
                    $@"OLEDB.4.0; Data Source = {pathFile}.mdb";

            // завантаження БД
            UpLoadDataBase();
        }

        /// <summary>
        /// Оновлення БД
        /// </summary>
        public static void UpLoadDataBase()
        {
            Console.WriteLine(DateTime.Now.ToString());
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

                    // установка прапора успошного завантаження БД
                    Succesfull = true;
                }
            }
            catch (OleDbException ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("Скопіюйте файл БД в папку з *.exe файлом, " +
                    "або якщо файл БД відстуній створіть його за допомогою програми App1 " +
                    "і тоді спопіюйте.");
            }
        }

    }
}
