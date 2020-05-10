using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Library.Data;
using Library.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryServices
{
    public class CheckoutService : ICheckout
    {
        private LibraryContext _context;
        public CheckoutService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(Checkout newCheckout)
        {
            _context.Add(newCheckout);
            _context.SaveChanges();
        }


        public IEnumerable<Checkout> GetAll()
        {
            return _context.Checkouts;
        }

        public Checkout GetById(int checkoutId)
        {
            return GetAll().FirstOrDefault(x => x.Id == checkoutId);
        }

        public IEnumerable<CheckoutHistory> GetCheckoutHistory(int id)
        {
            return _context.CheckoutHistories
                .Include(x=>x.LibraryAsset) ///acest includ il folosim pt a fi sigur ca LlibraryAsset si LibraryCard au valori;altfel nu ar avea valori
                .Include(x=>x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == id);
        }

       

        public IEnumerable<Hold> GetCurrentHolds(int id)
        {
            return _context.Holds
                .Include(x => x.LibraryAsset)
                .Where(x => x.LibraryAsset.Id == id);
        }

        public string GetCurrentPatron(int id)
        {
            throw new NotImplementedException();
        }

        public Checkout GetLatestCheckout(int id)
        {
            return _context.Checkouts
                .Where(x => x.LibraryAsset.Id == id)
                .OrderByDescending(x => x.Since)
                .FirstOrDefault();
        }

        public int GetNumberOfCopies(int id)
        {
            throw new NotImplementedException();
        }

        public bool IsCheckedOut(int id)
        {
            throw new NotImplementedException();
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
            var item = _context.LibraryAssets
                .FirstOrDefault(x => x.Id == assetId);
            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(x => x.Name == "Available");

            //remove any existing checkeouts on the item

            var checkout = _context.Checkouts
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId);

            if(checkout != null)
            {
                _context.Remove(checkout);
            }

            //close any existing checkout history

            var history = _context.CheckoutHistories
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId
                    && x.CheckedIn == null);

            if(history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
            _context.SaveChanges();

        }

        public void MarkLost(int assetId)
        {
            var item = _context.LibraryAssets
                .FirstOrDefault(x => x.Id == assetId);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(x => x.Name == "Lost");

            _context.SaveChanges();


        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public void CheckInItem(int id)
        {
            throw new NotImplementedException();
        }

        public void CheckoutItem(int id, int libraryCardId)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentHoldPatron(int id)
        {
            throw new NotImplementedException();
        }

        public string GetCurrentHoldPlaced(int id)
        {
            throw new NotImplementedException();
        }
    }
}
