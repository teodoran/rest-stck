using System;
using System.Text;
using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.FSharp.Collections;
using RestStck.Models;
using CoreStck;

namespace RestStck.Controllers
{
    [Route("/")]
    public class StckController : Controller
    {
        private static readonly Dictionary<string,Tuple<FSharpList<Tuple<string,FSharpList<String>>>,FSharpList<Int32>,FSharpList<string>>> contexts = new Dictionary<string,Tuple<FSharpList<Tuple<string,FSharpList<String>>>,FSharpList<Int32>,FSharpList<string>>>();
        
        // GET /stck
        [HttpGet("/stck")]
        public JsonResult Get()
        {
            var result = EvaluateToken(null, Interpreter.initialContext);
            return HalResponse(result);
        }

        // GET /stck/contexts/AA33D25C475DA8870F21773B7BE287AB/tokens/zero
        [HttpGet("/stck/contexts/{contextKey}/tokens/{token}")]
        public JsonResult Get(string contextKey, string token)
        {
            Tuple<FSharpList<Tuple<string,FSharpList<String>>>,FSharpList<Int32>,FSharpList<string>> oldContext;
            bool contextExists = contexts.TryGetValue(contextKey, out oldContext);
            if (!contextExists) {
                Response.StatusCode = 404;
                return Json("Context not found");
            }

            var result = EvaluateToken(token, oldContext);
            return HalResponse(result);
        }

        private EvaluationResult EvaluateToken(string token, Tuple<FSharpList<Tuple<string,FSharpList<String>>>,FSharpList<Int32>,FSharpList<string>> oldContext)
        {
            var context = Interpreter.evaluate(oldContext.Item1, oldContext.Item2, oldContext.Item3, token);
            var key = StoreContext(context);

            var result = new EvaluationResult()
            {
                Stack = context.Item2,
                Expression = context.Item3,
                Error = Interpreter.error,
                _links = HeapTokenLinks(context.Item1, key, HttpContext.Request)
            };
            Interpreter.error = null;

            return result;
        }

        private string StoreContext(Tuple<FSharpList<Tuple<string,FSharpList<String>>>,FSharpList<Int32>,FSharpList<string>> context)
        {
            var hash = MD5.Create().ComputeHash(Encoding.UTF8.GetBytes(context.ToString()));
            var key = BitConverter.ToString(hash).Replace("-", "");

            if(!contexts.ContainsKey(key))
            {
                contexts.Add(key, context);
            }

            return key;
        }

        private Dictionary<string, HalLink> HeapTokenLinks(FSharpList<Tuple<string,FSharpList<String>>> heap, string contextKey, HttpRequest request)
        {
            // Request.Url.Scheme
            Dictionary<string, HalLink> links;
            if(heap.IsEmpty)
            {
                links = new Dictionary<string, HalLink>();
                links.Add("eval", TokenLink("eval", contextKey, request));
            } else
            {
                links = HeapTokenLinks(heap.Tail, contextKey, request);
                links.Add(heap.Head.Item1, TokenLink(heap.Head.Item1, contextKey, request));
            }

            return links;
        }

        private HalLink TokenLink(string token, string contextKey, HttpRequest request)
        {
            return new HalLink
            {
                href = String.Format("{0}://{1}/stck/contexts/{2}/tokens/{3}", request.Scheme, request.Host.ToUriComponent(), contextKey, token)
            };
        }

        private JsonResult HalResponse(EvaluationResult result)
        {
            var response = Json(result);
            response.ContentType = "application/hal+json";
            return response;
        }
    }
}
