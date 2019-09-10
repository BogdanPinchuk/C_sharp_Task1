using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.M
{
    /// <summary>
    /// Модель, дані
    /// </summary>
    class Model
    {
        /// <summary>
        /// Валюта
        /// </summary>
        public enum Currency
        {
            National,
            Dollar
        }

        /// <summary>
        /// Інформатор
        /// </summary>
        private readonly Checker checker;

        /// <summary>
        /// Культура
        /// </summary>
        public RegionInfo region { get; private set; }

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
        /// Загальне замовлення
        /// </summary>
        public SOrder Order { get; set; }

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
                    region = RegionInfo.CurrentRegion;
                    break;
                case Currency.Dollar:
                    region = new RegionInfo("en-US");
                    break;
            } 
            #endregion

            // завантаження БД
            LoadDataBase.UpLoadDataBase(path + file);

            // якщо БД завантажена успішно запускаємо обробку запитів користувача
            if (LoadDataBase.Successful)
            {
                // Запуск інформатора який оновлюватиме БД
                checker = new Checker(file);
            }
        }

    }
}
