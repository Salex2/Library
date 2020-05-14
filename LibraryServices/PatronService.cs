using Library.Data;
using Library.Data.Models;
using LibraryData;
using LibraryData.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibraryServices
{
    public class PatronService : IPatron
    {

        private LibraryContext _context;

        public PatronService(LibraryContext context)
        {
            _context = context;
        }


        public void Add(Patron patron)
        {
            _context.Add(patron);
            _context.SaveChanges();
        }

        public Patron Get(int id)
        {
            return GetAll()
                .FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<Patron> GetAll()
        {
            return _context.Patrons
               .Include(x => x.LibraryCard)
               .Include(x => x.HomeLibraryBranch);
        }

        public IEnumerable<CheckoutHistory> GetCheckOutHistory(int patronId)
        {
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.CheckoutHistories
                .Include(y => y.LibraryCard)
                .Include(y => y.LibraryAsset)
                .Where(y => y.LibraryCard.Id == cardId)
                .OrderByDescending(y => y.CheckedOut);
        }

        public IEnumerable<Checkout> GetCheckouts(int patronId)
        {
            var cardId = Get(patronId).LibraryCard.Id;

            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryCard.Id == cardId);
        }

        public IEnumerable<Hold> GetHolds(int patronId)
        {
            throw new NotImplementedException();
        }
    }
}
