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
            return   _context.Checkouts
                .Where(x => x.LibraryAsset.Id == id)
                .Any();

            
        }

        public void MarkFound(int assetId)
        {
            var now = DateTime.Now;
           

            UpdateAssetStatus(assetId, "Available");
            RemoveExistingCheckouts(assetId);
            //remove any existing checkeouts on the item


            CloseExistingCheckoutHistory(assetId, now);
            //close any existing checkout history

           
            _context.SaveChanges();

        }

        private void UpdateAssetStatus(int assetId, string newStatus)
        {
           var item = _context.LibraryAssets
                .FirstOrDefault(x => x.Id == assetId);
            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(x => x.Name == newStatus);
        }

        private void CloseExistingCheckoutHistory(int assetId, DateTime now)
        {
            var history = _context.CheckoutHistories
                 .FirstOrDefault(x => x.LibraryAsset.Id == assetId
                     && x.CheckedIn == null);

            if (history != null)
            {
                _context.Update(history);
                history.CheckedIn = now;
            }
        }

        private void RemoveExistingCheckouts(int assetId)
        {
            var checkout = _context.Checkouts
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId);

            if (checkout != null)
            {   
                _context.Remove(checkout);
            }
        }

        public void MarkLost(int assetId)
        {
            UpdateAssetStatus(assetId, "Lost");

            _context.SaveChanges();


        }

        public void PlaceHold(int assetId, int libraryCardId)
        {
            var now = DateTime.Now;

            var asset = _context.LibraryAssets
                .Include(x => x.Status)
               .FirstOrDefault(x => x.Id == assetId);

            var card = _context.LibraryCards
                .FirstOrDefault(x => x.Id == libraryCardId);

            if(asset.Status.Name == "Available")
            {
                UpdateAssetStatus(assetId, "On Hold");
            }

            var hold = new Hold
            {
                HoldPlaced = now,
                LibraryAsset = asset,
                LibraryCard = card
            };

            _context.Add(hold);
            _context.SaveChanges();
        }

        public void CheckInItem(int assetId)
        {
            var now = DateTime.Now;

            var item = _context.LibraryAssets
                .FirstOrDefault(x => x.Id == assetId);

           

            //remove any existing checkouts on the item
            RemoveExistingCheckouts(assetId);

            //close any existing checkout history
            CloseExistingCheckoutHistory(assetId, now);

            //look for existing hold on the item
            var currentHolds = _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == assetId);

            //if there are holds,checkout the item to the libraryCard with the earliest hold
            if (currentHolds.Any())
            {
                CheckoutToEarliestHold(assetId, currentHolds);
                return;
            }


            ////otherwise,update the item status to available.
            ///
            UpdateAssetStatus(assetId, "Available");

            _context.SaveChanges();
        }

        private void CheckoutToEarliestHold(int assetId, IQueryable<Hold> currentHolds)
        {
            var earliestHold = currentHolds
                .OrderBy(holds => holds.HoldPlaced)
                .FirstOrDefault();

            var card = earliestHold.LibraryCard;

            _context.Remove(earliestHold);
            _context.SaveChanges();
            CheckoutItem(assetId, card.Id);

            
        }

        public void CheckoutItem(int id, int libraryCardId)
        {
            if (IsCheckedOut(id)) return;

            var item = _context.LibraryAssets
                .Include(a => a.Status)
                .First(a => a.Id == id);

            _context.Update(item);

            item.Status = _context.Statuses
                .FirstOrDefault(a => a.Name == "Checked Out");

            var now = DateTime.Now;

            var libraryCard = _context.LibraryCards
                .Include(c => c.Checkouts)
                .FirstOrDefault(a => a.Id == libraryCardId);

            var checkout = new Checkout
            {
                LibraryAsset = item,
                LibraryCard = libraryCard,
                Since = now,
                Until = GetDefaultCheckOutTime(now)
            };

            _context.Add(checkout);

            var checkoutHistory = new CheckoutHistory
            {
                CheckedOut = now,
                LibraryAsset = item,
                LibraryCard = libraryCard
            };

            _context.Add(checkoutHistory);
            _context.SaveChanges();
        }

       

        private DateTime GetDefaultCheckOutTime(DateTime now)
        {
            return now.AddDays(30);
        }

        public string GetCurrentHoldPatron(int holdId)
        {
            var hold = _context.Holds
                 .Include(x => x.LibraryAsset)
                 .Include(x => x.LibraryCard)
                 .FirstOrDefault(x => x.Id == holdId);

            var cardId = hold?.LibraryCard.Id;

            var patron = _context.Patrons.Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.LibraryCard.Id == cardId);

            return patron?.FirstName + " " + patron?.LastName;
        }

        public DateTime GetCurrentHoldPlaced(int holdId)
        {
            return  _context.Holds
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.Id == holdId)
                .HoldPlaced; 
        }

       public string GetCurrentCheckoutPatron(int assetId)
        {
            var checkout = GetCheckOutByAssetId(assetId);   
           
            if (checkout == null)
            {
                return "";
            };

            var cardId = checkout.LibraryCard.Id;

            var patron = _context.Patrons
                .Include(x => x.LibraryCard)
                .FirstOrDefault(x => x.LibraryCard.Id == cardId);

            return patron.FirstName + " " + patron.LastName;
        }

        private Checkout GetCheckOutByAssetId(int assetId)
        {
            return _context.Checkouts
                .Include(x => x.LibraryAsset)
                .Include(x => x.LibraryCard)
                .Where(x => x.LibraryAsset.Id == assetId)
                .FirstOrDefault(x => x.LibraryAsset.Id == assetId);
        }
    }
}
