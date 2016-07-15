﻿using Glimpse.Core.Extensibility;

namespace Glimpse.Core.Framework
{
    /// <summary>
    /// Defines methods to required to implement a Glimpse framework provider. 
    /// Framework providers allow Glimpse to work generically against any .NET based web development framework.
    /// </summary>
    /// <remarks>
    /// Required by any different Framework Provider - i.e. ASP.NET, Self Hosted WebAPI, 
    /// NancyFX, etc. See Glimpse.AspNet.AspNetFrameworkProvider
    /// as reference implementation.
    /// </remarks>
    public interface IFrameworkProvider
    {
        /// <summary>
        /// Gets the Http request store.
        /// </summary>
        /// <value>The Http request store.</value>
        /// <remarks>
        /// A request store is a place for Glimpse to store data that lives and dies with an Http request.
        /// </remarks>
        /// <example>
        /// In ASP.NET, <c>HttpContext.Items</c> is a request store.
        /// </example>
        IDataStore HttpRequestStore { get; }

        /// <summary>
        /// Gets the Http server store.
        /// </summary>
        /// <value>The Http server store.</value>
        /// <remarks>
        /// A server store is a place for Glimpse to store data a persists across Http requests.
        /// </remarks>
        /// <example>
        /// In ASP.NET, <c>HttpContext.Application</c> is a server store.
        /// </example>
        IDataStore HttpServerStore { get; }

        /// <summary>
        /// Gets the runtime context.
        /// </summary>
        /// <value>The runtime context.</value>
        /// <remarks>
        /// Instance of the underlying context that the web development framework uses.
        /// </remarks>
        /// <example>
        /// In ASP.NET, <c>HttpContext</c> is the runtime context.
        /// </example>
        object RuntimeContext { get; }

        /// <summary>
        /// Gets the request metadata.
        /// </summary>
        /// <value>The request metadata.</value>
        /// <remarks>
        /// An instance of <see cref="IRequestMetadata"/> which provides relevant metadata about an Http request.
        /// </remarks>
        /// <example>
        /// In ASP.NET, a <c>HttpRequest</c> contains must data required for creating a <see cref="IRequestMetadata"/>.
        /// </example>
        IRequestMetadata RequestMetadata { get; }

        /// <summary>
        /// Sets the Http response header.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetHttpResponseHeader(string name, string value);

        /// <summary>
        /// Sets the Http response status code.
        /// </summary>
        /// <param name="statusCode">The status code.</param>
        void SetHttpResponseStatusCode(int statusCode);

        /// <summary>
        /// Sets the cookie.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        void SetCookie(string name, string value);

        /// <summary>
        /// Injects the Http response body.
        /// </summary>
        /// <param name="htmlSnippet">The HTML snippet.</param>
        /// <remarks>
        /// Inserts the given html snippet into the html document just before the end <c>&lt;/body&gt;</c> tag.
        /// </remarks>
        void InjectHttpResponseBody(string htmlSnippet);

        /// <summary>
        /// Writes the Http response.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <remarks>
        /// Used by the resource infrastructure to output binary content (i.e. embedded content,
        /// images, etc).
        /// <seealso cref="Glimpse.Core.Extensibility.IResourceResult"/>
        /// <seealso cref="Glimpse.Core.Extensibility.IResource"/>
        /// </remarks>
        void WriteHttpResponse(byte[] content);

        /// <summary>
        /// Writes the Http response.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <remarks>
        /// Used by the resource infrastructure to output string content (i.e. generated strings,
        /// JSON objects, etc).
        /// <seealso cref="IResourceResult"/>
        /// </remarks>
        void WriteHttpResponse(string content);
    }
}
