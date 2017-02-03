using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AbcBank.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;

namespace AbcBank.Controllers
{
    [Authorize(Roles = "Manager, Banker")]
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

        public string MyMethod(Microsoft.AspNetCore.Http.HttpContext context)
        {
            var host = $"{context.Request.Scheme}://{context.Request.Host}";
            return host;
        }

        [AllowAnonymous]
        public IActionResult Login(string CardNumber, string CardPin)
        {
            var referrer = Request.Headers["Referer"].ToString();
            var current = MyMethod(HttpContext);
            var response = new Dictionary<string, string>();

            if (current != referrer)
            {
                if (!_context.Cards.Any(x => x.CardNumber == CardNumber && x.CardPin == ConvertStringtoMD5(CardPin)))
                {
                    response["status"] = "fail";
                    response["token"] = "Invalid pin.";
                    return Json(JsonConvert.SerializeObject(response));
                }
                var currentRequestToken = RandomStringToken();
                var card = _context.Cards.FirstOrDefault(
                    x => x.CardNumber == CardNumber && x.CardPin == ConvertStringtoMD5(CardPin));
                card.Token = currentRequestToken;
                _context.Cards.Update(card);
                _context.SaveChanges();

                response.Add("status", "pass");
                response.Add("token", currentRequestToken);
                return Json(JsonConvert.SerializeObject(response));
            }
            return RedirectToAction("Index", "Dashboard");
        }

        public string RandomStringToken()
        {
            var random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var token =  new string(Enumerable.Repeat(chars, 64)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            if (!_context.Cards.Any(x => x.Token == token))
            {
                return token;
            }
            return RandomStringToken();
        }
    }
}
