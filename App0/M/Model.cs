using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using App0.M.Product;

namespace App0.M
{
    /// <summary>
    /// Модель, дані
    /// </summary>
    class Model
    {
        /// <summary>
        /// Делегат на відслідковування зміни БД
        /// </summary>
        public delegate void ChangeData();
        /// <summary>
        /// Подія яка відбувається при зміні БД
        /// </summary>
        public event ChangeData ChangeDB;

        /// <summary>
        /// Валюта
        /// </summary>
        public enum Currency
        {
            National,
            Dollar
        }

        /// <summary>
        /// Культура
        /// </summary>
        public RegionInfo Region { get; private set; }
        /// <summary>
        /// Статус оновлення БД, 
        /// true - оновлення було, false - оновлення не було
        /// </summary>
        public bool IsUpdateDB
            => LoadDataBase.IsUpdateDB;
        /// <summary>
        /// Таблиці продуктів
        /// </summary>
        public DataSet Products
        {
            get
            {
                lock (LoadDataBase.Block)
                {
                    return LoadDataBase.Products;
                }
            }
        }
        /// <summary>
        /// Набір стаканчиків
        /// </summary>
        public int[] Glasses { get; private set; }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="path">Адреса директорії в якій знаходиться БД</param>
        /// <param name="file">Назва файла БД без розширення</param>
        /// <param name="momey">Вибір валюти</param>
        public Model(string path, string file, Currency momey)
        {
            // В умові вказані $ але можливо 
            // треба використовувати регіональну валюту - гривні
            #region Установка культури
            switch (momey)
            {
                case Currency.National:
                    Region = RegionInfo.CurrentRegion;
                    break;
                case Currency.Dollar:
                    Region = new RegionInfo("en-US");
                    break;
            }
            #endregion

            // завантаження БД
            LoadDataBase.UpLoadDataBase(path + file);

            // якщо БД завантажена успішно запускаємо обробку запитів користувача
            if (LoadDataBase.Successful)
            {
                // Запуск інформатора який оновлюватиме БД
                new Checker(file);
            }

            // заповнення набору стаканчиків
            Glasses = Enum.GetValues(typeof(TypeOfGlass)).Cast<int>().ToArray();

            // запуск відслідковування змін БД
            new Thread(CheckerChangeDB).Start();
        }

        /// <summary>
        /// Перевірка зміни розмірів консолі і оновлення БД
        /// </summary>
        private void CheckerChangeDB()
        {
            while (true)
            {
                // якщо оновилася БД
                if (LoadDataBase.IsUpdateDB && ChangeDB != null)
                {
                    // оновлюємо БД
                    ChangeDB.Invoke();

                    // ставимо, що БД "застаріла"
                    LoadDataBase.IsUpdateDB = false;
                }
            }
        }

    }
}
