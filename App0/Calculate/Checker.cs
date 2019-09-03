using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using App0.Product;

// Примітка. Checker дозволяє слідкувати за файлом БД,
// і в разі внесених в нього змін чи перезапису оновлювати
// внутрішню автономну БД, без перезапуску програми

namespace App0.Calculate
{
    /// <summary>
    /// Інформатор
    /// </summary>
    class Checker
    {
        /// <summary>
        /// Наглядач
        /// </summary>
        private FileSystemWatcher watcher = new FileSystemWatcher();

        /// <summary>
        /// Шлях відслідковування каталога
        /// </summary>
        public string Path
        {
            get { return watcher.Path; }
            set { watcher.Path = value; }
        }

        /// <summary>
        /// Фільтр файлів для відстідковування
        /// </summary>
        public string Filter
        {
            get { return watcher.Filter; }
            set { watcher.Filter = value; }
        }

        /// <summary>
        /// Запуск інформатора з відслідковуванням в папці з файлом програми
        /// </summary>
        /// <param name="name">Назва файла</param>
        public Checker(string name)
            : this(name, @".") { }

        /// <summary>
        /// Запуск інформатора
        /// </summary>
        /// <param name="name">назва файлу</param>
        /// <param name="path">директорія яка відслідковується</param>
        public Checker(string name, string path)
        {
            this.Filter = name + ".mdb";
            this.Path = path;
            // установка події на зміну файла + реакція (метод) на подію
            watcher.Changed += UpdateDB;
            // додаємо відслідковування заміни файла іншим (наприклад при копіюванні)
            watcher.NotifyFilter = NotifyFilters.LastWrite;

            try
            {
                // запуск моніторингу
                watcher.EnableRaisingEvents = true;
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// При внесенні змін в файл оновлюється БД
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateDB(object sender, FileSystemEventArgs e)
            => new Thread(LoadDataBase.UpLoadDataBase).Start();
        // запускаємо в окремому потоці по слабкій ссилці для оновлення БД
    }
}
