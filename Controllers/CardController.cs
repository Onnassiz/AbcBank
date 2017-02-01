using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;

namespace AbcBank.Controllers
{
    public class CardController : Controller
    {
        private readonly MyDbContext _context;

        public CardController(MyDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var result = _context.Cards.Join(_context.Accounts, x => x.AccountId, y => y.Id,
                    (card, account) => new {card, account}
            );

            var model = new List<CardJoinView>();

            foreach (var item in result)
            {
                model.Add(new CardJoinView
                {
                    Id = item.card.Id,
                    CardName = item.card.CardName,
                    CardNumber = item.card.CardNumber,
                    AccountNumber = item.account.AccountName
                });
            }
            return View(model);
        }

        public IActionResult Create()
        {
            ViewBag.Account = new SelectList(_context.Accounts.OfType<Current>().ToList(), "Id", "AccountNameAndNumber");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Card card)
        {
            ViewBag.Account = new SelectList(_context.Accounts.OfType<Current>().ToList(), "Id", "AccountNameAndNumber");
            if (ModelState.IsValid)
            {
                card.CardPin =  ConvertStringtoMD5(card.CardPin);
                _context.Cards.Add(card);
                _context.SaveChanges();
                TempData["Response"] = "Card has been sucessfully registered.";
                return RedirectToAction("Index");
            }
            return View();
        }

        public string ConvertStringtoMD5(string strword)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(strword);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("x2"));
            }
            return sb.ToString();
        }

        public IActionResult Delete(string Id)
        {
            _context.Cards.Remove(_context.Cards.Find(Id));
            _context.SaveChanges();
            _context.SaveChanges();
            TempData["Response"] = "Item Deleted";
            return RedirectToAction("Index");
        }

    }
}
