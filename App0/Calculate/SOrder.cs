using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using App0.Product;

namespace App0.Calculate
{
    /// <summary>
    /// Заказ
    /// </summary>
    struct SOrder
    {
        /// <summary>
        /// Чи вибраний стаканчик
        /// </summary>
        private bool isGlass;
        /// <summary>
        /// Список нопоїв, null - не вибрано
        /// </summary>
        private List<IDrink> drinks;
        /// <summary>
        /// Список добавок, null - не вибрано
        /// </summary>
        private List<IAdditiv> additivs;
        /// <summary>
        /// Тип стаканчика
        /// </summary>
        private TypeOfGlass glass;

        /// <summary>
        /// Тип стаканчика
        /// </summary>
        public TypeOfGlass Glass
        {
            get { return glass; }
            set
            {
                // так як при зміні стаканчика можливе переповнення,
                // то краще все очистити
                Clear();
                glass = value;
                isGlass = true;
            }
        }
        /// <summary>
        /// Чи вибраний стаканчик
        /// </summary>
        public bool IsGlass
        {
            get { return isGlass; }
            set
            {
                // зовні стуруктури стаканчик можна лише забрати, але не поставити
                // поставити стаканчик можна лише присвоївши зразу його тип,
                // тобто, змінити на true можна лише зсередини структури
                if (!value)
                {
                    // очищення всього, а там є реалізація присвоєння false
                    Clear();
                }
            }
        }
        /// <summary>
        /// Ємність вибраного стаканчика
        /// </summary>
        public double Capacity
        {
            get
            {
                if (!IsGlass)
                {
                    return 0;
                }
                else
                {
                    return (double)Glass;
                }
            }
        }
        /// <summary>
        /// Вільне місце в стаканчику
        /// </summary>
        public double Free
        {
            get
            {
                if (!IsGlass)
                {
                    return 0;
                }

                // TODO: змінити на наявність колекцій
                // сума всії продуктів
                double sum = default(double);
                // Примітка. Окрім рідини в нас ще є тверді продукти
                // які вимірюються масою, для точного значення як змінюється 
                // об'єм рідини при вкидані деяких твердих продуктів необхідно проводити
                // дослідження, але як для прикладу приймемо, що 10 гр при попаданні
                // в рідину перетворюється в 5 мл, тобто 1 г = 0,5 мл

                #region Сума напою
                foreach (var i in Drinks)
                {
                    if (i.TypeOfValue == TypeValue.Volume)
                    {
                        sum += i.Size;
                    }
                    else
                    {
                        sum += 0.5 * i.Size;
                    }
                }
                #endregion

                #region Сума добавок
                foreach (var i in Additivs)
                {
                    if (i.TypeOfValue == TypeValue.Volume)
                    {
                        sum += i.Size;
                    }
                    else
                    {
                        sum += 0.5 * i.Size;
                    }
                }
                #endregion

                return Capacity - sum;
            }
        }
        /// <summary>
        /// Ціна замовлення
        /// </summary>
        public double Price { get; private set; }

        /// <summary>
        /// Список нопоїв, null - не вибрано
        /// </summary>
        public List<IDrink> Drinks
        {
            get
            {
                if (drinks == null || !IsGlass)
                {
                    drinks = new List<IDrink>();
                }

                // так як можна отримати доступ через Add() і поламати логіку
                // необхідно зробити чистку і тут
                if (drinks?.Count > 1)
                {
                    // копіюємо в локальну область для обробки
                    var temp = drinks;
                    // робимо очищення по першому напою
                    temp = temp.Where(t => t.Name == temp[0].Name)
                        .Select(t => t)
                        .ToList();

                    drinks = temp;
                }

                return drinks;
            }
            set
            {
                if (!IsGlass || value?.Count == 0)
                {
                    return;
                }

                // необхідно недопустити змішування напоїв

                // присвоюємо дані
                drinks = value;
                // копіюємо в локальну область для обробки
                var temp = drinks;
                // робимо очищення по першому напою
                temp = temp.Where(t => t.Name == temp[0].Name)
                    .Select(t => t)
                    .ToList();

                drinks = temp;

                if (drinks == null)
                {
                    drinks = new List<IDrink>();
                }
            }
        }
        /// <summary>
        /// Список добавок, null - не вибрано
        /// </summary>
        public List<IAdditiv> Additivs
        {
            get
            {
                if (additivs == null || !IsGlass)
                {
                    additivs = new List<IAdditiv>();
                }

                return additivs;
            }
            set
            {
                if (!IsGlass || value?.Count == 0)
                {
                    return;
                }

                // присвоюємо дані
                additivs = value;

                if (additivs == null)
                {
                    additivs = new List<IAdditiv>();
                }
            }
        }

        /// <summary>
        /// Очищення замовлення
        /// </summary>
        public void Clear()
        {
            this.Drinks?.Clear();
            this.Additivs?.Clear();
            this.isGlass = false;
        }

        /// <summary>
        /// Об'єм скаканчика (вільно)/(загальний об'єм)
        /// </summary>
        /// <returns></returns>
        public string Volume()
            => IsGlass ? $"{Free:F2}/{Capacity:F2}" : string.Empty;
    }
}
