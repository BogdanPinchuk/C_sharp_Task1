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
        /// Тип в якому вимірюється продукт
        /// </summary>
        public TypeValue TypeOfValue { get; private set; }
        /// <summary>
        /// Величина виміру
        /// </summary>
        public double Size { get; private set; }

        /// <summary>
        /// Додавання продукту
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="name">назва</param>
        /// <param name="price">ціна</param>
        /// <param name="size">розмів виміру мл/г</param>
        /// <param name="type">тип виміру вага/об'єм</param>
        public SProduct(int id, string name, double price,
            double size, TypeValue type)
        {
            this.ID = id;
            this.Name = name;
            this.Price = price;
            this.Size = size;
            this.TypeOfValue = type;
        }
    }
}
