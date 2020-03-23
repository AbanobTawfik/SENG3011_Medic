using System;
using System.Collections.Generic;

namespace MedicApi.Models
{
    public class ApiGetArticlesResponse
    {
        public ApiResponseMetadata meta { get; set; }
        public List<Article> articles { get; set; }

        public ApiGetArticlesResponse(DateTime accessed_time,
                                      List<Article> articles)
        {
            meta = new ApiResponseMetadata(accessed_time);
            this.articles = articles;
        }
    }
}
