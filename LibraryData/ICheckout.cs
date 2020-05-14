using System;
using System.Collections.Generic;
using Library.Data.Models;

namespace Library.Data
{
    public interface ICheckout
    {
        IEnumerable<Checkout> GetAll();
        Checkout GetById(int id);
        void Add(Checkout newCheckout);
        IEnumerable<CheckoutHistory> GetCheckoutHistory(int id);
        void PlaceHold(int assetId, int libraryCardId);
        void CheckoutItem(int id, int libraryCardId);
        void CheckInItem(int id);
        Checkout GetLatestCheckout(int id);
        int GetNumberOfCopies(int id);
        bool IsCheckedOut(int id);
        string GetCurrentCheckoutPatron(int assetId);
        


        string GetCurrentHoldPatron(int id);
        DateTime GetCurrentHoldPlaced(int id);
        string GetCurrentPatron(int id);
        IEnumerable<Hold> GetCurrentHolds(int id);

        void MarkLost(int assetId);
        void MarkFound(int assetId);
    }
}