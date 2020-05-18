using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Library.Data.Models;
using Library.Models.Branch;
using LibraryData;
using Microsoft.AspNetCore.Mvc;

namespace Library.Controllers
{
    public class BranchController : Controller
    {
        private readonly ILibraryBranch _branch;

        public BranchController(ILibraryBranch branch)
        {
            _branch = branch;
        }

        public IActionResult Index()
        {
            var branches = _branch.GetAll().Select(branch => new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                IsOpen = _branch.IsBranchOpen(branch.Id),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count()

            });

            var model = new BranchIndexModel()
            {
                Branches = branches
            };

            return View(model);
        }

        public IActionResult Detail(int id)
        {
            var branch = _branch.Get(id);

            var model = new BranchDetailModel
            {
                Id = branch.Id,
                Name = branch.Name,
                Telephone = branch.Telephone,
                OpenDate = branch.OpenDate.ToString("yyyy-mm-dd"),
                NumberOfAssets = _branch.GetAssets(branch.Id).Count(),
                NumberOfPatrons = _branch.GetPatrons(branch.Id).Count(),
                TotalAssetValue = _branch.GetAssets(id).Sum(x => x.Cost),
                ImageUrl = branch.ImageUrl,
                HoursOpen = _branch.GetBranchHours(id)
            };
        }
    }
}