using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.M.Product
{
    /// <summary>
    /// Тип оцінки продукту
    /// </summary>
    public enum TypeValue
    {
        /// <summary>
        /// Вага, г (грам) // true - при конвертації
        /// </summary>
        Weight,
        /// <summary>
        /// Об'єм, мл (мілілітр) // false - при конвертації
        /// </summary>
        Volume
    }

    /// <summary>
    /// Стакани для наливання
    /// </summary>
    public enum TypeOfGlass
    {
        /// <summary>
        /// стакан на 110 мл
        /// </summary>
        ml110 = 110,
        /// <summary>
        /// стакан на 175 мл
        /// </summary>
        ml175 = 175,
        /// <summary>
        /// стакан на 250 мл
        /// </summary>
        ml250 = 250,
        /// <summary>
        /// стакан на 400 мл
        /// </summary>
        ml400 = 400,
        /// <summary>
        /// стакан на 500 мл
        /// </summary>
        ml500 = 500,
    }
    // Примітка. Дані об'єму взяті з інтернету. Також вважатимемо,
    // що ціна стаканчика уже врахована в сам напій. В подальшому 
    // при необхідності програму можна вдосконалити і дані по стаканчиках
    // винести в файл БД.

    /// <summary>
    /// Продукт (напій, добавки, ...)
    /// </summary>
    interface IProduct
    {
        /// <summary>
        /// ID - продукту
        /// </summary>
        int ID { get; }
        /// <summary>
        /// Назва продукту
        /// </summary>
        string Name { get; }
        /// <summary>
        /// Ціна продукту
        /// </summary>
        double Price { get; }
        /// <summary>
        /// Тип в якому вимірюється продукт
        /// </summary>
        TypeValue TypeOfValue { get; }
        /// <summary>
        /// Величина виміру об'єму чи маси
        /// </summary>
        double Size { get; }
    }
}
