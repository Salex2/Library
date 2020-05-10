using Library.Data;
using Library.Data.Models;
using LibraryData;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LibraryServices
{
    public class LibraryAssetService : ILibraryAsset
    {
        private LibraryContext _context;

        public LibraryAssetService(LibraryContext context)
        {
            _context = context;
        }

        public void Add(LibraryAsset newAsset)
        {

            _context.Add(newAsset);
            _context.SaveChanges();
        }

        public IEnumerable<LibraryAsset> GetAll()
        {
            return _context.LibraryAssets.Include(asset => asset.Status)
                                         .Include(asset => asset.Location);
        }

        

        public LibraryAsset GetById(int id)
        {
            return _context.LibraryAssets
               .Include(a => a.Status)
               .Include(a => a.Location)
               .FirstOrDefault(a => a.Id == id);
        }

        public LibraryBranch GetCurrentLocation(int id)
        {
            return GetById(id).Location;
        }

        public string GetDeweyIndex(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).DeweyIndex;
            }
            else return "";
        }

        public string GetISBM(int id)
        {
            if (_context.Books.Any(book => book.Id == id))
            {
                return _context.Books.FirstOrDefault(book => book.Id == id).ISBN;
            }
            else return "";
        }

        public string GetTitle(int id)
        {
            return _context.LibraryAssets
                   .FirstOrDefault(x => x.Id == id)
                   .Title;
        }

        public string GetType(int id)
        {
            var book = _context.LibraryAssets.OfType<Book>()
                .Where(x => x.Id == id);
            return book.Any() ? "book" : "video";
        }

        public string GetAuthorOrDirector(int id)
        {
            var isBook = _context.LibraryAssets.OfType<Book>()
                .Where(x => x.Id == id).Any();

            var isVideo = _context.LibraryAssets.OfType<Video>()
                .Where(x => x.Id == id).Any();

            return isBook ?
                _context.Books.FirstOrDefault(x => x.Id == id).Author :
                _context.Videos.FirstOrDefault(x => x.Id == id).Director
                ?? "Unknown";   
        }
    }
}
