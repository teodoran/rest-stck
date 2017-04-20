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
        
        // GET /2%203%20%2B
        [HttpGet("/{expression}")]
        public JsonResult Get(string expression)
        {
            var context = new EvaluationContext(){
                Heap = Interpreter.standardLibrary,
                Stack = FSharpList<int>.Empty
            };
            
            return Json(Evaluate(expression, context));
        }

        // GET /2%203%20%2B/contexts/4d5b2fc3-ca5f-493e-ae0f-69f3d7eae498
        [HttpGet("/{expression}/contexts/{contextKey}")]
        public JsonResult Get(string expression, string contextKey)
        {
            EvaluationContext context;
            bool contextExists = contexts.TryGetValue(contextKey, out context);
            if (!contextExists) {
                Response.StatusCode = 404;
                return Json("Context not found");
            }
            
            return Json(Evaluate(expression, context));
        }

        private EvaluationResult Evaluate(string expression, EvaluationContext context){
            var evaluated = Interpreter.evaluate(context.Heap, context.Stack, expression);
            var newContext = new EvaluationContext(){
                Heap = evaluated.Item1,
                Stack = evaluated.Item2
            };

            var contextKey = newContext.Hash();
            if(!contexts.ContainsKey(contextKey)){
                contexts.Add(contextKey, newContext);
            }

            var result = new EvaluationResult(){
                Expression = expression,
                Context = newContext,
                Error = Interpreter.error
            };

            return result;
        }
    }
}
