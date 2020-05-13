using Library.Data;
using Library.Models.Catalog;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Library.Models.Catalog.AssetDetailModel;

namespace Library.Controllers
{
    public class CatalogController : Controller
    {
        private readonly ILibraryAsset _assets;
        private readonly ICheckout _checkouts;

        public CatalogController(ILibraryAsset assets,ICheckout checkouts)//injectam in constructor cele 2 interfete 
        {
            _assets = assets;
            _checkouts = checkouts;
        }

        public IActionResult Index()
        {
            var assetModels = _assets.GetAll();

            var listingResult = assetModels
                .Select(result   => new AssetIndexListingModel
                {
                    Id = result.Id,
                    ImageUrl = result.ImageUrl,
                    AuthorOrDirector = _assets.GetAuthorOrDirector(result.Id),
                    DeweyCallNumber = _assets.GetDeweyIndex(result.Id),
                    Title = result.Title,
                    Type = _assets.GetType(result.Id)
                });

            var model = new AssetIndexModel()
            {
                Assets = listingResult

            };

            return View(model);

        }

        /*public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImgUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                ISBN = _assets.GetISBM(id)
            };
            return View(model);
        } */

        public IActionResult Detail(int id)
        {
            var asset = _assets.GetById(id);

            var currentHolds = _checkouts.GetCurrentHolds(id)
                .Select(x => new AssetHoldModel
                {
                    HoldPlaced = _checkouts.GetCurrentHoldPlaced(x.Id),
                    PatronName = _checkouts.GetCurrentPatron(x.Id)
                });

            var model = new AssetDetailModel
            {
                AssetId = id,
                Title = asset.Title,
                Type = _assets.GetType(id),
                Year = asset.Year,
                Cost = asset.Cost,
                Status = asset.Status.Name,
                ImgUrl = asset.ImageUrl,
                AuthorOrDirector = _assets.GetAuthorOrDirector(id),
                CurentLocation = _assets.GetCurrentLocation(id).Name,
                DeweyCallNumber = _assets.GetDeweyIndex(id),
                CheckOutHistory = _checkouts.GetCheckoutHistory(id),
                ISBN = _assets.GetISBM(id),
                LatestCheckout = _checkouts.GetLatestCheckout(id),
                PatroName = _checkouts.GetCurrentCheckoutPatron(id),
                CurrentHolds = currentHolds

            };


            return View(model);   
        }
    }
}
