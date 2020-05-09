using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Catalog
{
    public class AssetDetailModel
    {//this is basicaly what we will show on the page
        public int AssetId { get; set; }
        public string Title { get; set; }
        public string AuthorOrDirector { get; set; }
        public string Type { get; set; }
        public int Year { get; set; }
        public string ISBN { get; set; }
        public string DeweyCallNumber { get; set; }
        public string Status { get; set; }
        public decimal Cost { get; set; }
        public string CurentLocation { get; set; }
        public string ImgUrl { get; set; }
        public string PatroName { get; set; }
        public Checkout LatestCheckout { get; set; }
        public IEnumerable<CheckoutHistory> CheckOutHistory { get; set; }
        public IEnumerable<AssetHoldModel> CurrentHolds { get; set; }

        public class AssetHoldModel
        {
            public string PatronName { get; set; }
            public DateTime HoldPlaced { get; set; }
        }



    }
}
