using Library.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Models.Patron
{
    public class PatronDetailModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int LibraryCardId { get; set; }
        public string Adress { get; set; }
        public DateTime MemberSincer { get; set; }
        public string Telephone { get; set; }
        public string HomeLibraryBranch { get; set; }
        public decimal OverdueFees { get; set; }
        public IEnumerable<Checkout> AssetsCheckedOut { get; set; }
        public IEnumerable<CheckoutHistory> CheckOutHistory { get; set; }
        public IEnumerable<Hold> Holds { get; set; }
    }
}
