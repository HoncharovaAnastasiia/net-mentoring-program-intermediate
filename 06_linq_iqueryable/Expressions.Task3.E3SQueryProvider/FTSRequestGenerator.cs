using Expressions.Task3.E3SQueryProvider.Attributes;
using Expressions.Task3.E3SQueryProvider.Models.Request;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Expressions.Task3.E3SQueryProvider
{
    public class FtsRequestGenerator
    {
        private readonly string _FTSSearchTemplate = @"/searchFts";
        private readonly string _baseAddress;

        #region Constructors
        
        public FtsRequestGenerator(string baseAddress)
        {
            _baseAddress = baseAddress;
        }

        #endregion

        #region public methods

        public Uri GenerateRequestUrl<T>(string query = "*", int start = 0, int limit = 10)
        {
            return GenerateRequestUrl(typeof(T), query, start, limit);
        }

        public Uri GenerateRequestUrl<T>(IEnumerable<string> statements, int start = 0, int limit = 10)
        {
            return GenerateRequestUrl(typeof(T), statements, start, limit);
        }

        public Uri GenerateRequestUrl(Type type, string query = "*", int start = 0, int limit = 10)
        {
            var ftsQueryRequest = new FtsQueryRequest(query)
            {
                Start = start,
                Limit = limit
            };

            return GetUrl(type, ftsQueryRequest);
        }

        public Uri GenerateRequestUrl(Type type, IEnumerable<string> statements, int start = 0, int limit = 10)
        {
            var ftsQueryRequest = new FtsQueryRequest(statements)
            {
                Start = start,
                Limit = limit
            };

            return GetUrl(type, ftsQueryRequest);
        }

        #endregion

        private Uri GetUrl(Type type, FtsQueryRequest ftsQueryRequest)
        {
            string metaTypeName = GetMetaTypeName(type);

            var ftsQueryRequestString = JsonConvert.SerializeObject(ftsQueryRequest);

            var uri = BindByName($"{_baseAddress}{_FTSSearchTemplate}",
                new Dictionary<string, string>
                {
                    { "metaType", metaTypeName },
                    { "query", ftsQueryRequestString }
                });

            return uri;
        }

        private static Uri BindByName(string baseAddress, Dictionary<string, string> queryParams)
            => new Uri(QueryHelpers.AddQueryString(baseAddress, queryParams));

        private static string GetMetaTypeName(Type type)
        {
            var attributes = type.GetCustomAttributes(typeof(E3SMetaTypeAttribute), false);

            if (attributes.Length == 0)
                throw new Exception($"Entity {type.FullName} do not have attribute E3SMetaType");

            return ((E3SMetaTypeAttribute)attributes[0]).Name;
        }
    }
}
