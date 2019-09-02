using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace App1.Product
{
    /// <summary>
    /// Продукт
    /// </summary>
    struct SProduct : IProduct
    {
        /// <summary>
        /// ID - продукту
        /// </summary>
        public int ID { get; private set; }
        /// <summary>
        /// Назва продукту
        /// </summary>
        public string Name { get; private set; }
        /// <summary>
        /// Ціна продукту
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        /// Додавання продукту
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        public SProduct(int id, string name, double price)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
        }
    }
}
