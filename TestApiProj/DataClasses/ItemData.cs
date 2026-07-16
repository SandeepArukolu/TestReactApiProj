using TestApiProj.MainEntity;

namespace TestApiProj.DataClasses
{
    public static class ItemData
    {

        public static readonly List<Items> Items = new()
        {
            new Items
            {
                ItemId = 1,
                ItemName = "Mobile Phones",
                ItemQty = 100,
                ItemPrice = 25000,
                ItemDiscount = 10
            },
            new Items
            {
                ItemId = 2,
                ItemName = "Dell-Laptop",
                ItemQty = 80,
                ItemPrice = 55000,
                ItemDiscount = 10
            },
            new Items
            {
                ItemId = 3,
                ItemName = "Chairs",
                ItemQty = 500,
                ItemPrice = 2000,
                ItemDiscount = 4
            },
            new Items
            {
                ItemId = 4,
                ItemName = "HeadPhones",
                ItemQty = 500,
                ItemPrice = 1200,
                ItemDiscount = 10
            },
            new Items
            {
                ItemId = 1002,
                ItemName = "Water Bottels",
                ItemQty = 400,
                ItemPrice = 400,
                ItemDiscount = 7
            },
            new Items
            {
                ItemId = 2002,
                ItemName = "Laptop Bags",
                ItemQty = 1000,
                ItemPrice = 800,
                ItemDiscount = 10
            },
            new Items
            {
                ItemId = 3002,
                ItemName = "LED TV",
                ItemQty = 400,
                ItemPrice = 34000,
                ItemDiscount = 20
            },
            new Items
            {
                ItemId = 3003,
                ItemName = "Refrigerators",
                ItemQty = 150,
                ItemPrice = 25000,
                ItemDiscount = 15
            },
            new Items
            {
                ItemId = 4002,
                ItemName = "AC",
                ItemQty = 20,
                ItemPrice = 40000,
                ItemDiscount = 16
            },
            new Items
            {
                ItemId = 4003,
                ItemName = "Washing Machine",
                ItemQty = 100,
                ItemPrice = 25000,
                ItemDiscount = 10,
                ItemTax = 4
            },
            new Items
            {
                ItemId = 4004,
                ItemName = "Speakers",
                ItemQty = 400,
                ItemPrice = 10000,
                ItemDiscount = 2,
                ItemTax = 6
            },
            new Items
            {
                ItemId = 4005,
                ItemName = "HeadPhones",
                ItemQty = 88,
                ItemPrice = 888,
                ItemDiscount = 9,
                ItemTax = 4
            }
        };
    }
}
