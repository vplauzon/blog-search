using Microsoft.Azure.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SearchFunction
{
    public class Post
    {
        [Key]
        public string Id { get; set; }

        [IsRetrievable(true)]
        public string Url { get; set; }

        [IsSearchable, IsSortable]
        public string Title { get; set; }

        [IsSearchable, IsFilterable]
        public string Content { get; set; }

        public string Excerpt { get; set; }

        [IsSortable]
        public DateTime Published { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Categories { get; set; }

        [IsSearchable, IsFilterable, IsFacetable]
        public string[] Tags { get; set; }
    }
}