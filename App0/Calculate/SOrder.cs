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

                return drinks;
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
        /// Додати напій
        /// </summary>
        /// <param name="drink">Напій</param>
        public bool AddDrink(IDrink drink)
        {
            // якщо стаканчика немає, додавати нічого не можна
            if (!IsGlass)
            {
                return false;
            }

            // необхідно не допустити переповнення стаканчика
            // розрахунок об'єму продукту який пробують додати
            double size = (drink.TypeOfValue == TypeValue.Volume) ? drink.Size : drink.Size * 0.5;

            // якщо перший раз то ініціалізуємо
            if (drinks == null)
            {
                drinks = new List<IDrink>();
                // якщо є вільне місце то додаємо продукт
                if (Free >= size)
                {
                    drinks.Add(drink);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // необхідно недопустити змішування напоїв
            if (drinks[0].Name == drink.Name && Free >= size)
            {
                drinks.Add(drink);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Додати добавку
        /// </summary>
        /// <param name="additiv">Добавка</param>
        public bool AddAdditiv(IAdditiv additiv)
        {
            // якщо стаканчика немає, додавати нічого не можна
            if (!IsGlass)
            {
                return false;
            }

            // необхідно не допустити переповнення стаканчика
            // розрахунок об'єму продукту який пробують додати
            double size = (additiv.TypeOfValue == TypeValue.Volume) ? additiv.Size : additiv.Size * 0.5;

            // якщо перший раз то ініціалізуємо
            if (additivs == null)
            {
                additivs = new List<IAdditiv>();
                // якщо є вільне місце то додаємо продукт
                if (Free >= size)
                {
                    additivs.Add(additiv);
                    return true;
                }
                else
                {
                    return false;
                }
            }

            // необхідно недопустити переповнення в наступних випадках
            if (Free >= size)
            {
                additivs.Add(additiv);
                return true;
            }

            return false;
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
