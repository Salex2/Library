using Library.Data.Models;
using Library.Models.Patron;
using LibraryData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Controllers
{
    public class PatronController : Controller
    {
        private IPatron _patron;
        public PatronController(IPatron patron)
        {
            _patron = patron;
        }

        public IActionResult Index()
        {
            var allPatrons = _patron.GetAll();

            var patronModels = allPatrons.Select(x => new PatronDetailModel
            {
                Id = x.Id,
                FirstName = x.FirstName,
                LastName = x.LastName,
                LibraryCardId = x.LibraryCard.Id,
                OverdueFees = x.LibraryCard.Fees,
                HomeLibraryBranch = x.HomeLibraryBranch.Name
            
            }).ToList();

            var model = new PatronIndexModel()
            {
                Patrons = patronModels
            };

            return View(model);

         }

        public IActionResult Detail(int patronId)
        {
            var patron = _patron.Get(patronId);

            var model = new PatronDetailModel
            {
                LastName = patron.LastName,
                FirstName = patron.FirstName,
                Adress = patron.Address,
                HomeLibraryBranch = patron.HomeLibraryBranch.Name,
                MemberSincer = patron.LibraryCard.Created,
                OverdueFees = patron.LibraryCard.Fees,
                LibraryCardId = patron.LibraryCard.Id,
                AssetsCheckedOut = _patron.GetCheckouts(patronId).ToList() ?? new List<Checkout>(),
                CheckOutHistory = _patron.GetCheckOutHistory(patronId),
                Holds = _patron.GetHolds(patronId)

            };

            return View(model);
        }

    }
}
