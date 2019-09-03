using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.Product
{
    /// <summary>
    /// Тип оцінки продукту
    /// </summary>
    public enum TypeValue
    {
        /// <summary>
        /// Вага, мг (міліграм) // true - при конвертації
        /// </summary>
        Weight,
        /// <summary>
        /// Об'єм, мл (мілілітр) // false - при конвертації
        /// </summary>
        Volume
    }

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
