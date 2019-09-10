using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.M
{
    /// <summary>
    /// Збереження даних в лог-файл
    /// </summary>
    public static class SaveLog
    {
        /// <summary>
        /// Спільний Log-File
        /// </summary>
        private static string pathFile = "Log.txt";

        /// <summary>
        /// Збереження інформації про покупку
        /// </summary>
        public static void SaveOrder(string info)
        {
            // створення *.txt файла якщо немає
            using (FileStream stream = new FileStream(pathFile,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // читаємо до кінця
                reader.ReadToEnd();

                // створення детальної інформації
                string save = "Order: "
                    + DateTime.Now.ToString()
                    + "; Result: " + info;

                // запис в файл з нового рядка
                writer.WriteLine(save);
            }
        }

        /// <summary>
        /// Збереження інформації про оновлення БД
        /// </summary>
        public static void SaveUpdate(string info)
        {
            // створення *.txt файла якщо немає
            using (FileStream stream = new FileStream(pathFile,
                FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            using (StreamReader reader = new StreamReader(stream))
            using (StreamWriter writer = new StreamWriter(stream))
            {
                // читаємо до кінця
                reader.ReadToEnd();

                // створення детальної інформації
                string save = "Update DataBase: "
                    + DateTime.Now.ToString()
                    + "; Result: " + info;

                // запис в файл з нового рядка
                writer.WriteLine(save);
            }
        }
    }
}
