using Knowit.Umbraco.TokenReplacement.Service;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Knowit.Umbraco.TokenReplacement.Middleware
{
	public class TokenReplacementMiddleWare
	{
		private readonly RequestDelegate _next;
		private readonly ICmsTokenReplacer _cmsTokenReplacer;
		private readonly ICultureExtractor _cultureExtractor;
		public TokenReplacementMiddleWare(RequestDelegate next, ICmsTokenReplacer cmsTokenReplacer, ICultureExtractor cultureExtractor)
		{
			_next = next;
			_cmsTokenReplacer = cmsTokenReplacer;
			_cultureExtractor = cultureExtractor;
		}

		public async Task InvokeAsync(HttpContext context)
		{
#if DEBUG
			Stopwatch sw = new Stopwatch();
			sw.Start();
#endif

			if(context.Request.Path == null || context.Request.Path.Value == null)
			{
				await _next(context);
				return;
			}

			string path = context.Request.Path.Value;
		
			if (path.Contains("/umbraco") && !path.Contains("/delivery/api")) { // todo exclude other paths based on config
				await _next(context);
				return;
			}
			// Intercept the response stream
			var originalBodyStream = context.Response.Body;		
			using var modifiedBodyStream = new MemoryStream();
			context.Response.Body = modifiedBodyStream;

			// Continue processing (let MVC generate the page)
			await _next(context);

			// Set the pointer of the stream to the beginning
			modifiedBodyStream.Seek(0, SeekOrigin.Begin);

			// Read the stream into a string
			var htmlContent = await new StreamReader(modifiedBodyStream).ReadToEndAsync();

			// get culture
			var host = context.Request.Host.Value;
			string culture = _cultureExtractor.GetCultureFromUrl(host,path);
			// Replace tokens in the HTML content
			htmlContent = _cmsTokenReplacer.Parse(htmlContent, culture);

			// Write the modified content back to the original stream
			var modifiedContentBytes = Encoding.UTF8.GetBytes(htmlContent);
			context.Response.Body = originalBodyStream;
			await context.Response.Body.WriteAsync(modifiedContentBytes);

#if DEBUG
			var test = sw.ElapsedMilliseconds;
			Console.Write(test); // Breakpoint here if you'd like to know the timings
#endif
		}
	}
}
