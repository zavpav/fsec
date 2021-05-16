using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using ProfanityList.WordList;

namespace CheckProfanityKrestel.Controllers
{
    [ApiController]
    public class ProfanityBasketController : Controller
    {
        private IProfanityListService Basket { get; }

        public ProfanityBasketController(IProfanityListService basket)
        {
            this.Basket = basket;
        }

        [HttpGet]
        [Route("[controller]")]
        [Route("[controller]/[action]")]
        public IEnumerable<string> List()
        {
            return this.Basket.GetProfanityWordList();
        }

        [Route("[controller]/[action]/{word}")]
        [Produces("application/json")]
        public BasketEditResult Add(string word)
        {
            return this.Basket.Add(word);
        }

        [Route("[controller]/[action]/{word}")]
        [Produces("application/json")]
        public BasketEditResult Remove(string word)
        {
            return this.Basket.Remove(word);
        }
    }
}