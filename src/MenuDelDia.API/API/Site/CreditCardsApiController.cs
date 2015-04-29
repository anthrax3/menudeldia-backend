using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using MenuDelDia.API.Models;
using MenuDelDia.Entities.Enums;

namespace MenuDelDia.API.API.Site
{
    public class CreditCardsApiController : ApiBaseController
    {

        public IList<CardModel> LoadCards(IList<Guid> selectedCardIds = null)
        {
            return CurrentAppContext.Cards.Select(c => new CardModel
               {
                   Id = c.Id,
                   Name = c.Name,
                   Type = (c.CardType == CardType.Credit ? "Crédito" : "Débito"),
               }).ToList();
        }

        [HttpGet]
        [Route("api/creditcards")]
        public HttpResponseMessage Get()
        {
            var result = LoadCards();
            return Request.CreateResponse(HttpStatusCode.OK, result);
        }
    }
}
