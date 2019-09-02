using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App0.Product
{
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
    }
}
