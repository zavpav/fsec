using System.Collections.Generic;
using System.Threading.Tasks;
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
        public async Task<IEnumerable<WordInfo>> List()
        {
            return await this.Basket.GetProfanityWordList();
        }

        [Route("[controller]/[action]/{word}")]
        [Produces("application/json")]
        public async Task<BasketEditResult> Add(string word)
        {
            return await this.Basket.Add(word);
        }

        [Route("[controller]/[action]/{word}")]
        [Produces("application/json")]
        public async Task<BasketEditResult> Remove(string word)
        {
            return await this.Basket.Remove(word);
        }
    }
}