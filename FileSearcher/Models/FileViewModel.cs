using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace FileSearcher.Models
{
    public class FileViewModel
    {
        public List<MyFile>? Files { get; set; }
        public string? SearchString { get; set; }
    }
}