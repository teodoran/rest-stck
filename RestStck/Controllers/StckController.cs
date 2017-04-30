using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FSharp.Collections;
using RestStck.Models;
using CoreStck;

namespace RestStck.Controllers
{
    [Route("/")]
    public class StckController : Controller
    {
        private static readonly Dictionary<string, EvaluationContext> contexts = new Dictionary<string, EvaluationContext>();
        
        // GET /stck
        [HttpGet("/stck")]
        public JsonResult Get()
        {
            var context = new EvaluationContext(){
                Heap = Interpreter.standardLibrary,
                Stack = FSharpList<int>.Empty
            };
            
            var contextKey = context.Hash();
            if(!contexts.ContainsKey(contextKey)){
                contexts.Add(contextKey, context);
            }

            var result = new EvaluationResult(){
                Stack = context.Stack,
                Expression = context.Expression,
                Error = null,
                _links = HeapTokenLinks(context.Heap, contextKey, HttpContext.Request.Host.ToUriComponent())
            };

            var response = Json(result);
            response.ContentType = "application/hal+json";

            return response;
        }

        // GET /stck/contexts/AA33D25C475DA8870F21773B7BE287AB/tokens/zero
        [HttpGet("/stck/contexts/{contextKey}/tokens/{token}")]
        public JsonResult Get(string token, string contextKey)
        {
            EvaluationContext context;
            bool contextExists = contexts.TryGetValue(contextKey, out context);
            if (!contextExists) {
                Response.StatusCode = 404;
                return Json("Context not found");
            }

            if (token.Equals("eval"))
            {
                try
                {
                    var evaluated = Interpreter.evaluate(context.Heap, context.Stack, String.Join(" ", context.Expression));
                    context = new EvaluationContext(){
                        Heap = evaluated.Item1,
                        Stack = evaluated.Item2
                    };
                }
                catch (Exception ex) {
                    // Ignore all the exceptions!
                    #pragma warning disable 0169
                    var ignored = ex;
                    #pragma warning restore 0169
                }
            } else {
                context.Expression.Add(token);
            }

            var newContextKey = context.Hash();
            if(!contexts.ContainsKey(newContextKey)){
                contexts.Add(newContextKey, context);
            }

            var result = new EvaluationResult(){
                Stack = context.Stack,
                Expression = context.Expression,
                Error = Interpreter.error,
                _links = HeapTokenLinks(context.Heap, newContextKey, HttpContext.Request.Host.ToUriComponent())
            };

            var response = Json(result);
            response.ContentType = "application/hal+json";

            return response;
        }

        private Dictionary<string, HalLink> HeapTokenLinks(FSharpList<Tuple<string,FSharpList<String>>> heap, string contextKey, string host){
            Dictionary<string, HalLink> links;
            if(heap.IsEmpty){
                links = new Dictionary<string, HalLink>();
                links.Add("eval", new HalLink{href = String.Format("http://{0}/stck/contexts/{1}/tokens/{2}", host, contextKey, "eval")});
            } else {
                var link = new HalLink{href = String.Format("http://{0}/stck/contexts/{1}/tokens/{2}", host, contextKey, heap.Head.Item1)};
                links = HeapTokenLinks(heap.Tail, contextKey, host);
                links.Add(heap.Head.Item1, link);
            }

            return links;
        }
    }
}
