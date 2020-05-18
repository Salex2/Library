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
    public class LibraryBranchService : ILibraryBranch
    {
        private LibraryContext _context;

        public LibraryBranchService(LibraryContext context)
        {
            _context = context;
        }
        public void Add(LibraryBranch newBranch)
        {
            _context.Add(newBranch);
            _context.SaveChanges();
        }

        public LibraryBranch Get(int branchId)
        {
            return _context.LibraryBranches
                .Include(x => x.Patrons)
                .Include(x => x.LibraryAssets)
                .FirstOrDefault(x => x.Id == branchId);

        }

        public IEnumerable<LibraryBranch> GetAll()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<LibraryAsset> GetAssets(int branchId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<string> GetBranchHours(int branchId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Patron> GetPatrons(int branchId)
        {
            throw new NotImplementedException();
        }

        public bool IsBranchOpen(int branchId)
        {
            throw new NotImplementedException();
        }
    }
}
